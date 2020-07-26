using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace Yapped.Grids.Generic
{
    public partial class FormCellEdit : Form
    {
        public FormCellEdit()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            comboBox.Text = LoadValue();
            base.OnLoad(e);
        }

        public GridCellType DataType { get; set; }
        public (string, object)[] EnumValues { get; set; }
        public object Value { get; set; }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                try
                {
                    StoreValue(comboBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
            base.OnClosing(e);
        }

        private void OnComboBoxTextChanged(object sender, System.EventArgs e)
        {
        }

        private void OnComboBoxSelectedIndexChanged(object sender, System.EventArgs e)
        {
        }

        private string LoadValue()
        {
            switch (DataType)
            {
                case GridCellType.None:
                    return string.Empty;
                case GridCellType.Byte:
                case GridCellType.SByte:
                case GridCellType.UInt16:
                case GridCellType.Int16:
                case GridCellType.UInt32:
                case GridCellType.Int32:
                case GridCellType.UInt64:
                case GridCellType.Int64:
                case GridCellType.Single:
                case GridCellType.Double:
                case GridCellType.String:
                    return Convert.ToString(Value);
                case GridCellType.Boolean:
                    return (((bool)Value) ? 1 : 0).ToString();
                case GridCellType.Enum:
                    return "NOTIMPL";
                case GridCellType.HexByte:
                case GridCellType.HexSByte:
                    return $"0x{Value:X2}";
                case GridCellType.HexUInt16:
                case GridCellType.HexInt16:
                    return $"0x{Value:X4}";
                case GridCellType.HexUInt32:
                case GridCellType.HexInt32:
                    return $"0x{Value:X8}";
                case GridCellType.HexUInt64:
                case GridCellType.HexInt64:
                    return $"0x{Value:X16}";
                default:
                    throw new NotSupportedException();
            }
        }

        private void StoreValue(string text)
        {
            text = text.Trim();
            var fromBase = 10;
            if (text.StartsWith("0x", true, CultureInfo.CurrentCulture))
            {
                text = text.Substring(2);
                fromBase = 16;
            }
            switch (DataType)
            {
                case GridCellType.None:
                    break;
                case GridCellType.Byte:
                case GridCellType.HexByte:
                    Value = Convert.ToByte(text, fromBase);
                    break;
                case GridCellType.SByte:
                case GridCellType.HexSByte:
                    Value = Convert.ToSByte(text, fromBase);
                    break;
                case GridCellType.UInt16:
                case GridCellType.HexUInt16:
                    Value = Convert.ToUInt16(text, fromBase);
                    break;
                case GridCellType.Int16:
                case GridCellType.HexInt16:
                    Value = Convert.ToInt16(text, fromBase);
                    break;
                case GridCellType.UInt32:
                case GridCellType.HexUInt32:
                    Value = Convert.ToUInt32(text, fromBase);
                    break;
                case GridCellType.Int32:
                case GridCellType.HexInt32:
                    Value = Convert.ToInt32(text, fromBase);
                    break;
                case GridCellType.UInt64:
                case GridCellType.HexUInt64:
                    Value = Convert.ToUInt64(text, fromBase);
                    break;
                case GridCellType.Int64:
                case GridCellType.HexInt64:
                    Value = Convert.ToInt64(text, fromBase);
                    break;
                case GridCellType.Single:
                    Value = Convert.ToSingle(text);
                    break;
                case GridCellType.Double:
                    Value = Convert.ToDouble(text);
                    break;
                case GridCellType.Boolean:
                    Value = Convert.ToInt32(text) != 0 ? true : false;
                    break;
                case GridCellType.Enum:
                    throw new NotImplementedException();
                case GridCellType.String:
                    Value = text;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
