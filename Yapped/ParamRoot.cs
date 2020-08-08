using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SoulsFormats;
using static Yapped.GameMode;

namespace Yapped
{
    /// <summary>
    /// A persistent collection of params.
    /// </summary>
    internal class ParamRoot : IList<ParamWrapper>
    {
        private DCX.Type dcxType = DCX.Type.None;
        private bool encrypted;
        private IBinder binder;
        private List<ParamWrapper> wrappers;

        private ParamRoot()
        {
        }

        public ParamWrapper this[int index]
        {
            get => wrappers[index];
            set => throw new NotSupportedException();
        }

        public ParamWrapper this[string name]
        {
            get => wrappers.First(x => string.Equals(x.Name, name));
        }

        public int Count => wrappers.Count;

        #region IList Implementation
        IEnumerator IEnumerable.GetEnumerator() => wrappers.GetEnumerator();
        IEnumerator<ParamWrapper> IEnumerable<ParamWrapper>.GetEnumerator() => wrappers.GetEnumerator();
        bool ICollection<ParamWrapper>.IsReadOnly => true;
        void ICollection<ParamWrapper>.Add(ParamWrapper item) => throw new NotSupportedException();
        void ICollection<ParamWrapper>.Clear() => throw new NotSupportedException();
        bool ICollection<ParamWrapper>.Contains(ParamWrapper item) => wrappers.Contains(item);
        void ICollection<ParamWrapper>.CopyTo(ParamWrapper[] array, int arrayIndex) => throw new NotSupportedException();
        bool ICollection<ParamWrapper>.Remove(ParamWrapper item) => throw new NotSupportedException();
        int IList<ParamWrapper>.IndexOf(ParamWrapper item) => wrappers.IndexOf(item);
        void IList<ParamWrapper>.Insert(int index, ParamWrapper item) => throw new NotSupportedException();
        void IList<ParamWrapper>.RemoveAt(int index) => throw new NotSupportedException();
        #endregion

        public string Path { get; private set; }

        public bool CanExport => encrypted && (binder is BND4);

        public static ParamRoot Load(string path, GameMode gameMode, bool hideUnusedParams, string resDir)
        {
            var layouts = LoadLayouts($@"{resDir}\Layouts");
            var paramInfo = ParamInfo.ReadParamInfo($@"{resDir}\ParamInfo.xml");

            if (!File.Exists(path))
            {
                throw new IOException($"Parambnd not found:\r\n{path}\r\nPlease browse to the Data0.bdt or parambnd you would like to edit.");
            }

            var instance = new ParamRoot();
            instance.Path = path;
            try
            {
                byte[] bytes;
                if (DCX.Is(path))
                {
                    bytes = DCX.Decompress(path, out instance.dcxType);
                }
                else
                {
                    bytes = File.ReadAllBytes(path);
                }

                if (BND4.Is(bytes))
                {
                    instance.binder = BND4.Read(bytes);
                    instance.encrypted = false;
                }
                else if (BND3.Is(bytes))
                {
                    instance.binder = BND3.Read(bytes);
                    instance.encrypted = false;
                }
                else if (gameMode.Game == GameMode.GameType.DarkSouls2)
                {
                    instance.binder = DecryptDS2Regulation(bytes);
                    instance.encrypted = true;
                }
                else if (gameMode.Game == GameMode.GameType.DarkSouls3)
                {
                    instance.binder = DecryptDS3Regulation(bytes);
                    instance.encrypted = true;
                }
                else
                {
                    throw new FormatException("Unrecognized file format.");
                }
            }
            catch (DllNotFoundException ex) when (ex.Message.Contains("oo2core_6_win64.dll"))
            {
                throw new DllNotFoundException("In order to load Sekiro params, you must copy oo2core_6_win64.dll from Sekiro into Yapped's lib folder.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load parambnd:\r\n{path}\r\n\r\n{ex}", ex);
            }

            instance.wrappers = new List<ParamWrapper>();
            foreach (var file in instance.binder.Files.Where(f => f.Name.EndsWith(".param")))
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(file.Name);
                if (paramInfo.ContainsKey(name))
                {
                    if (paramInfo[name].Blocked || paramInfo[name].Hidden && hideUnusedParams)
                        continue;
                }

                try
                {
                    var param = PARAM.Read(file.Bytes);
                    PARAM.Layout layout = null;
                    if (layouts.ContainsKey(param.ID))
                        layout = layouts[param.ID];

                    string description = null;
                    if (paramInfo.ContainsKey(name))
                        description = paramInfo[name].Description;

                    var wrapper = new ParamWrapper(name, param, layout, description);
                    instance.wrappers.Add(wrapper);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to load param file: {name}.param\r\n\r\n{ex}", ex);
                }
            }

            instance.wrappers.Sort();
            return instance;
        }

        private static Dictionary<string, PARAM.Layout> LoadLayouts(string directory)
        {
            var layouts = new Dictionary<string, PARAM.Layout>();
            var failed = false;
            if (Directory.Exists(directory))
            {
                foreach (string path in Directory.GetFiles(directory, "*.xml"))
                {
                    string paramID = System.IO.Path.GetFileNameWithoutExtension(path);
                    try
                    {
                        PARAM.Layout layout = PARAM.Layout.ReadXMLFile(path);
                        layouts[paramID] = layout;
                    }
                    catch (Exception ex)
                    {
                        Util.ShowError($"Failed to load layout {paramID}.txt\r\n\r\n{ex}");
                        failed = true;
                    }
                }
            }
            if (failed)
            {
                throw new Exception("Not all layouts were loaded.");
            }
            return layouts;
        }

        public void Save(GameMode gameMode)
        {
            foreach (var file in binder.Files)
            {
                foreach (var wrapper in wrappers)
                {
                    if (System.IO.Path.GetFileNameWithoutExtension(file.Name) == wrapper.Name)
                        file.Bytes = wrapper.Param.Write();
                }
            }

            if (!File.Exists(Path + ".bak"))
                File.Copy(Path, Path + ".bak");

            if (encrypted)
            {
                switch (gameMode.Game)
                {
                    case GameType.DarkSouls2:
                        EncryptDS2Regulation(Path, binder as BND4);
                        break;
                    case GameType.DarkSouls3:
                        SFUtil.EncryptDS3Regulation(Path, binder as BND4);
                        break;
                    default:
                        throw new NotSupportedException("Encryption is only valid for DS2 and DS3.");
                }
            }
            else
            {
                byte[] bytes;
                if (binder is BND3 bnd3)
                {
                    bytes = bnd3.Write();
                }
                else if (binder is BND4 bnd4)
                {
                    bytes = bnd4.Write();
                }
                else
                {
                    throw new NotSupportedException("Unsupported regulation format.");
                }
                if (dcxType != DCX.Type.None)
                {
                    bytes = DCX.Compress(bytes, dcxType);
                }
                File.WriteAllBytes(Path, bytes);
            }
        }

        public void Export(string path)
        {
            if (!(binder is BND4 bnd4))
            {
                throw new NotSupportedException("Unsupported format for export.");
            }
            BND4 paramBND = new BND4
            {
                BigEndian = false,
                Compression = DCX.Type.DarkSouls3,
                Extended = 0x04,
                Flag1 = false,
                Flag2 = false,
                Format = Binder.Format.x74,
                Timestamp = bnd4.Timestamp,
                Unicode = true,
                Files = binder.Files.Where(f => f.Name.EndsWith(".param")).ToList()
            };

            BND4 stayBND = new BND4
            {
                BigEndian = false,
                Compression = DCX.Type.DarkSouls3,
                Extended = 0x04,
                Flag1 = false,
                Flag2 = false,
                Format = Binder.Format.x74,
                Timestamp = bnd4.Timestamp,
                Unicode = true,
                Files = binder.Files.Where(f => f.Name.EndsWith(".stayparam")).ToList()
            };

            try
            {
                paramBND.Write($@"{path}\gameparam_dlc2.parambnd.dcx");
                stayBND.Write($@"{path}\stayparam.parambnd.dcx");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to write exported parambnds.\r\n\r\n{ex}", ex);
            }
        }

        private static readonly byte[] ds3RegulationKey = Encoding.ASCII.GetBytes("ds3#jn/8_7(rsY9pg55GFN7VFL#+3n/)");

        private static BND4 DecryptDS3Regulation(byte[] bytes)
        {
            bytes = DecryptByteArray(ds3RegulationKey, bytes);
            return BND4.Read(bytes);
        }

        private static byte[] DecryptByteArray(byte[] key, byte[] secret)
        {
            byte[] iv = new byte[16];
            byte[] encryptedContent = new byte[secret.Length - 16];

            Buffer.BlockCopy(secret, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(secret, iv.Length, encryptedContent, 0, encryptedContent.Length);

            using (MemoryStream ms = new MemoryStream())
            using (AesManaged cryptor = new AesManaged())
            {
                cryptor.Mode = CipherMode.CBC;
                cryptor.Padding = PaddingMode.None;
                cryptor.KeySize = 256;
                cryptor.BlockSize = 128;

                using (CryptoStream cs = new CryptoStream(ms, cryptor.CreateDecryptor(key, iv), CryptoStreamMode.Write))
                {
                    cs.Write(encryptedContent, 0, encryptedContent.Length);
                }
                return ms.ToArray();
            }
        }

        private static readonly byte[] ds2RegulationKey = {
            0x40, 0x17, 0x81, 0x30, 0xDF, 0x0A, 0x94, 0x54, 0x33, 0x09, 0xE1, 0x71, 0xEC, 0xBF, 0x25, 0x4C };

        private static BND4 DecryptDS2Regulation(byte[] bytes)
        {
            byte[] iv = new byte[16];
            iv[0] = 0x80;
            Array.Copy(bytes, 0, iv, 1, 11);
            iv[15] = 1;
            byte[] input = new byte[bytes.Length - 32];
            Array.Copy(bytes, 32, input, 0, bytes.Length - 32);
            using (var ms = new MemoryStream(input))
            {
                byte[] decrypted = CryptographyUtility.DecryptAesCtr(ms, ds2RegulationKey, iv);
                File.WriteAllBytes("ffff.bnd", decrypted);
                return BND4.Read(decrypted);
            }
        }

        private static void EncryptDS2Regulation(string path, BND4 bnd)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            bnd.Write(path);
        }
    }
}
