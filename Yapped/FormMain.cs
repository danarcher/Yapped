using Semver;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Yapped.Grids;
using Yapped.Grids.Generic;
using CellType = SoulsFormats.PARAM.CellType;
using GameType = Yapped.GameMode.GameType;

namespace Yapped
{
    public partial class FormMain : Form
    {
        private const string UPDATE_URL = "https://www.nexusmods.com/sekiro/mods/121?tab=files";
        private static Properties.Settings settings = Properties.Settings.Default;

        private string regulationPath;
        private IBinder regulation;
        private bool encrypted;
        private string lastFindRowPattern, lastFindFieldPattern;

        private Grid paramsGrid, rowsGrid, cellsGrid;
        private SelectionMemory memory;

        public FormMain()
        {
            InitializeComponent();
            regulation = null;
            lastFindRowPattern = "";

            var largeFont = new Font("Segoe UI", 12.0f);
            paramsGrid = new Grid();
            splitContainer2.Panel1.Controls.Add(paramsGrid);
            paramsGrid.Font = largeFont;
            paramsGrid.Dock = DockStyle.Fill;
            paramsGrid.BringToFront();

            rowsGrid = new Grid();
            splitContainer1.Panel1.Controls.Add(rowsGrid);
            rowsGrid.Font = largeFont;
            rowsGrid.Dock = DockStyle.Fill;
            rowsGrid.BringToFront();

            cellsGrid = new Grid();
            splitContainer1.Panel2.Controls.Add(cellsGrid);
            cellsGrid.Font = largeFont;
            cellsGrid.Dock = DockStyle.Fill;
            cellsGrid.BringToFront();

            memory = new SelectionMemory();
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            Text = "Yapped " + Application.ProductVersion;

            Location = settings.WindowLocation;
            if (settings.WindowSize.Width >= MinimumSize.Width && settings.WindowSize.Height >= MinimumSize.Height)
                Size = settings.WindowSize;
            if (settings.WindowMaximized)
                WindowState = FormWindowState.Maximized;

            toolStripComboBoxGame.ComboBox.DisplayMember = "Name";
            toolStripComboBoxGame.Items.AddRange(GameMode.Modes);
            var game = (GameType)Enum.Parse(typeof(GameType), settings.GameType);
            toolStripComboBoxGame.SelectedIndex = Array.FindIndex(GameMode.Modes, m => m.Game == game);
            if (toolStripComboBoxGame.SelectedIndex == -1)
                toolStripComboBoxGame.SelectedIndex = 0;

            regulationPath = settings.RegulationPath;
            hideUnusedParamsToolStripMenuItem.Checked = settings.HideUnusedParams;
            verifyDeletionsToolStripMenuItem.Checked = settings.VerifyRowDeletion;
            splitContainer2.SplitterDistance = settings.SplitterDistance2;
            splitContainer1.SplitterDistance = settings.SplitterDistance1;

            memory.Load(settings.DGVIndices);
            LoadParams();

            Octokit.GitHubClient gitHubClient = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("Yapped"));
            try
            {
                Octokit.Release release = await gitHubClient.Repository.Release.GetLatest("JKAnderson", "Yapped");
                if (SemVersion.Parse(release.TagName) > Application.ProductVersion)
                {
                    updateToolStripMenuItem.Visible = true;
                }
            }
            // Oh well.
            catch { }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            settings.WindowMaximized = WindowState == FormWindowState.Maximized;
            if (WindowState == FormWindowState.Normal)
            {
                settings.WindowLocation = Location;
                settings.WindowSize = Size;
            }
            else
            {
                settings.WindowLocation = RestoreBounds.Location;
                settings.WindowSize = RestoreBounds.Size;
            }

            settings.GameType = ((GameMode)toolStripComboBoxGame.SelectedItem).Game.ToString();
            settings.RegulationPath = regulationPath;
            settings.HideUnusedParams = hideUnusedParamsToolStripMenuItem.Checked;
            settings.VerifyRowDeletion = verifyDeletionsToolStripMenuItem.Checked;
            settings.SplitterDistance2 = splitContainer2.SplitterDistance;
            settings.SplitterDistance1 = splitContainer1.SplitterDistance;
            settings.DGVIndices = memory.Save();
        }

        private void LoadParams()
        {
            string resDir = GetResRoot();
            Dictionary<string, PARAM.Layout> layouts = Util.LoadLayouts($@"{resDir}\Layouts");
            Dictionary<string, ParamInfo> paramInfo = ParamInfo.ReadParamInfo($@"{resDir}\ParamInfo.xml");
            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;
            LoadParamsResult result = Util.LoadParams(regulationPath, paramInfo, layouts, gameMode, hideUnusedParamsToolStripMenuItem.Checked);

            if (result == null)
            {
                exportToolStripMenuItem.Enabled = false;
            }
            else
            {
                encrypted = result.Encrypted;
                regulation = result.ParamBND;
                exportToolStripMenuItem.Enabled = encrypted;
                paramsGrid.Host = new ParamsGridHost(memory, rowsGrid, cellsGrid) { DataSource = result.ParamWrappers };
                toolStripStatusLabel1.Text = regulationPath;
            }
        }

        #region File Menu
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofdRegulation.FileName = regulationPath;
            if (ofdRegulation.ShowDialog() == DialogResult.OK)
            {
                regulationPath = ofdRegulation.FileName;
                LoadParams();
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (BinderFile file in regulation.Files)
            {
                var paramFiles = ((ParamsGridHost)paramsGrid.Host).DataSource;
                foreach (var paramFile in paramFiles)
                {
                    if (Path.GetFileNameWithoutExtension(file.Name) == paramFile.Name)
                        file.Bytes = paramFile.Param.Write();
                }
            }

            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;
            if (!File.Exists(regulationPath + ".bak"))
                File.Copy(regulationPath, regulationPath + ".bak");

            if (encrypted)
            {
                if (gameMode.Game == GameType.DarkSouls2)
                    Util.EncryptDS2Regulation(regulationPath, regulation as BND4);
                else if (gameMode.Game == GameType.DarkSouls3)
                    SFUtil.EncryptDS3Regulation(regulationPath, regulation as BND4);
                else
                    Util.ShowError("Encryption is only valid for DS2 and DS3.");
            }
            else
            {
                if (regulation is BND3 bnd3)
                    bnd3.Write(regulationPath);
                else if (regulation is BND4 bnd4)
                    bnd4.Write(regulationPath);
            }
            SystemSounds.Asterisk.Play();
        }

        private void RestoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(regulationPath + ".bak"))
            {
                DialogResult choice = MessageBox.Show("Are you sure you want to restore the backup?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (choice == DialogResult.Yes)
                {
                    try
                    {
                        File.Copy(regulationPath + ".bak", regulationPath, true);
                        LoadParams();
                        SystemSounds.Asterisk.Play();
                    }
                    catch (Exception ex)
                    {
                        Util.ShowError($"Failed to restore backup\r\n\r\n{regulationPath}.bak\r\n\r\n{ex}");
                    }
                }
            }
            else
            {
                Util.ShowError($"There is no backup to restore at:\r\n\r\n{regulationPath}.bak");
            }
        }

        private void ExploreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Path.GetDirectoryName(regulationPath));
            }
            catch
            {
                SystemSounds.Hand.Play();
            }
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var bnd4 = regulation as BND4;
            fbdExport.SelectedPath = Path.GetDirectoryName(regulationPath);
            if (fbdExport.ShowDialog() == DialogResult.OK)
            {
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
                    Files = regulation.Files.Where(f => f.Name.EndsWith(".param")).ToList()
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
                    Files = regulation.Files.Where(f => f.Name.EndsWith(".stayparam")).ToList()
                };

                string dir = fbdExport.SelectedPath;
                try
                {
                    paramBND.Write($@"{dir}\gameparam_dlc2.parambnd.dcx");
                    stayBND.Write($@"{dir}\stayparam.parambnd.dcx");
                }
                catch (Exception ex)
                {
                    Util.ShowError($"Failed to write exported parambnds.\r\n\r\n{ex}");
                }
            }
        }
        #endregion

        #region Edit Menu
        private void AddRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateRow("Add a new row...");
        }

        private void DuplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rowsGrid.SelectedRowIndex < 0)
            {
                Util.ShowError("You can't duplicate a row without one selected!");
                return;
            }

            int index = rowsGrid.SelectedRowIndex;
            ParamWrapper wrapper = ((RowsGridHost)rowsGrid.Host).DataSource;
            PARAM.Row oldRow = wrapper.Rows[index];
            PARAM.Row newRow;
            if ((newRow = CreateRow("Duplicate a row...")) != null)
            {
                for (int i = 0; i < oldRow.Cells.Count; i++)
                {
                    newRow.Cells[i].Value = oldRow.Cells[i].Value;
                }
            }
        }

        private PARAM.Row CreateRow(string prompt)
        {
            if (paramsGrid.SelectedRowIndex < 0)
            {
                Util.ShowError("You can't create a row with no param selected!");
                return null;
            }

            PARAM.Row result = null;
            var newRowForm = new FormNewRow(prompt);
            if (newRowForm.ShowDialog() == DialogResult.OK)
            {
                long id = newRowForm.ResultID;
                string name = newRowForm.ResultName;
                ParamWrapper paramWrapper = ((RowsGridHost)rowsGrid.Host).DataSource;
                if (paramWrapper.Rows.Any(row => row.ID == id))
                {
                    Util.ShowError($"A row with this ID already exists: {id}");
                }
                else
                {
                    result = new PARAM.Row(id, name, paramWrapper.Layout);
                    paramWrapper.Rows.Add(result);
                    paramWrapper.Rows.Sort((r1, r2) => r1.ID.CompareTo(r2.ID));

                    int index = paramWrapper.Rows.FindIndex(row => ReferenceEquals(row, result));
                    rowsGrid.SelectedRowIndex = index;
                    rowsGrid.ScrollToSelection();
                }
            }
            return result;
        }

        private void DeleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rowsGrid.SelectedRowIndex >= 0)
            {
                DialogResult choice = DialogResult.Yes;
                if (verifyDeletionsToolStripMenuItem.Checked)
                    choice = MessageBox.Show("Are you sure you want to delete this row?",
                        "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (choice == DialogResult.Yes)
                {
                    int rowIndex = rowsGrid.SelectedRowIndex;
                    ((RowsGridHost)rowsGrid.Host).DataSource.Rows.RemoveAt(rowIndex);

                    // If you remove a row it automatically selects the next one, but if you remove the last row
                    // it doesn't automatically select the previous one
                    if (rowIndex == ((RowsGridHost)rowsGrid.Host).RowCount)
                    {
                        --rowsGrid.SelectedRowIndex;
                    }
                }
            }
        }

        private void ImportNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool replace = MessageBox.Show("If a row already has a name, would you like to skip it?\r\n" +
                "Click Yes to skip existing names.\r\nClick No to replace existing names.",
                "Importing Names", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

            string namesDir = $@"{GetResRoot()}\Names";
            List<ParamWrapper> paramFiles = ((ParamsGridHost)paramsGrid.Host).DataSource;
            foreach (ParamWrapper paramFile in paramFiles)
            {
                if (File.Exists($@"{namesDir}\{paramFile.Name}.txt"))
                {
                    var names = new Dictionary<long, string>();
                    string nameStr = File.ReadAllText($@"{namesDir}\{paramFile.Name}.txt");
                    foreach (string line in Regex.Split(nameStr, @"\s*[\r\n]+\s*"))
                    {
                        if (line.Length > 0)
                        {
                            Match match = Regex.Match(line, @"^(\d+) (.+)$");
                            long id = long.Parse(match.Groups[1].Value);
                            string name = match.Groups[2].Value;
                            names[id] = name;
                        }
                    }

                    foreach (PARAM.Row row in paramFile.Param.Rows)
                    {
                        if (names.ContainsKey(row.ID))
                        {
                            if (replace || row.Name == null || row.Name == "")
                                row.Name = names[row.ID];
                        }
                    }
                }
            }

            rowsGrid.Invalidate();
        }

        private void ExportNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string namesDir = $@"{GetResRoot()}\Names";
            List<ParamWrapper> paramFiles = ((ParamsGridHost)paramsGrid.Host).DataSource;
            foreach (ParamWrapper paramFile in paramFiles)
            {
                StringBuilder sb = new StringBuilder();
                foreach (PARAM.Row row in paramFile.Param.Rows)
                {
                    string name = (row.Name ?? "").Trim();
                    if (name != "")
                    {
                        sb.AppendLine($"{row.ID} {name}");
                    }
                }

                try
                {
                    File.WriteAllText($@"{namesDir}\{paramFile.Name}.txt", sb.ToString());
                }
                catch (Exception ex)
                {
                    Util.ShowError($"Failed to write name file: {paramFile.Name}.txt\r\n\r\n{ex}");
                    break;
                }
            }
        }

        private void FindRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var findForm = new FormFind("Find row with name...");
            if (findForm.ShowDialog() == DialogResult.OK)
            {
                FindRow(findForm.ResultPattern);
            }
        }

        private void FindNextRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindRow(lastFindRowPattern);
        }

        private void FindRow(string pattern)
        {
            if (paramsGrid.SelectedRowIndex < 0)
            {
                Util.ShowError("You can't search for a row when there are no rows!");
                return;
            }

            int startIndex = rowsGrid.SelectedRowIndex >= 0 ? rowsGrid.SelectedRowIndex + 1 : 0;
            List<PARAM.Row> rows = ((RowsGridHost)rowsGrid.Host).DataSource.Rows;
            int index = -1;

            for (int i = 0; i < rows.Count; i++)
            {
                if ((rows[(startIndex + i) % rows.Count].Name ?? "").ToLower().Contains(pattern.ToLower()))
                {
                    index = (startIndex + i) % rows.Count;
                    break;
                }
            }

            if (index != -1)
            {
                rowsGrid.SelectedRowIndex = index;
                rowsGrid.ScrollToSelection();
                lastFindRowPattern = pattern;
            }
            else
            {
                Util.ShowError($"No row found matching: {pattern}");
            }
        }

        private void GotoRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var gotoForm = new FormGoto();
            if (gotoForm.ShowDialog() == DialogResult.OK)
            {
                if (paramsGrid.SelectedRowIndex < 0)
                {
                    Util.ShowError("You can't goto a row when there are no rows!");
                    return;
                }

                long id = gotoForm.ResultID;
                List<PARAM.Row> rows = ((RowsGridHost)rowsGrid.Host).DataSource.Rows;
                int index = rows.FindIndex(row => row.ID == id);

                if (index != -1)
                {
                    rowsGrid.SelectedRowIndex = index;
                    rowsGrid.ScrollToSelection();
                }
                else
                {
                    Util.ShowError($"Row ID not found: {id}");
                }
            }
        }

        private void FindFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var findForm = new FormFind("Find field with name...");
            if (findForm.ShowDialog() == DialogResult.OK)
            {
                FindField(findForm.ResultPattern);
            }
        }

        private void FindNextFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindField(lastFindFieldPattern);
        }

        private void FindField(string pattern)
        {
            if (rowsGrid.SelectedRowIndex < 0)
            {
                Util.ShowError("You can't search for a field when there are no fields!");
                return;
            }

            int startIndex = cellsGrid.SelectedRowIndex >= 0 ? cellsGrid.SelectedRowIndex + 1 : 0;
            var cells = ((CellsGridHost)cellsGrid.Host).DataSource;
            int index = -1;

            for (int i = 0; i < cells.Length; i++)
            {
                if ((cells[(startIndex + i) % cells.Length].Name ?? "").ToLower().Contains(pattern.ToLower()))
                {
                    index = (startIndex + i) % cells.Length;
                    break;
                }
            }

            if (index != -1)
            {
                cellsGrid.SelectedRowIndex = index;
                cellsGrid.ScrollToSelection();
                lastFindFieldPattern = pattern;
            }
            else
            {
                Util.ShowError($"No field found matching: {pattern}");
            }
        }
        #endregion

        private void UpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(UPDATE_URL);
        }

        //private void DgvParams_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        //{
        //    if (e.RowIndex >= 0)
        //    {
        //        var paramWrapper = (ParamWrapper)dgvParams.Rows[e.RowIndex].DataBoundItem;
        //        e.ToolTipText = paramWrapper.Description;
        //    }
        //}

        //private void DgvRows_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        //{
        //    // ID
        //    if (e.ColumnIndex == 0)
        //    {
        //        bool parsed = int.TryParse((string)e.FormattedValue, out int value);
        //        if (!parsed || value < 0)
        //        {
        //            Util.ShowError("Row ID must be a positive integer.\r\nEnter a valid number or press Esc to cancel.");
        //            e.Cancel = true;
        //        }
        //    }
        //}

        //private void FormatDgvCells()
        //{
        //    foreach (DataGridViewRow row in dgvCells.Rows)
        //    {
        //        var cell = (PARAM.Cell)row.DataBoundItem;
        //        if (cell.Enum != null)
        //        {
        //            var paramWrapper = (ParamWrapper)dgvParams.SelectedCells[0].OwningRow.DataBoundItem;
        //            PARAM.Layout layout = paramWrapper.Layout;
        //            PARAM.Enum pnum = layout.Enums[cell.Enum];
        //            if (pnum.Any(v => v.Value.Equals(cell.Value)))
        //            {
        //                row.Cells[2] = new DataGridViewComboBoxCell
        //                {
        //                    DataSource = pnum,
        //                    DisplayMember = "Name",
        //                    ValueMember = "Value",
        //                    ValueType = cell.Value.GetType()
        //                };
        //            }
        //        }
        //        else if (cell.Type == CellType.b8 || cell.Type == CellType.b16 || cell.Type == CellType.b32)
        //        {
        //            row.Cells[2] = new DataGridViewCheckBoxCell();
        //        }
        //        else
        //        {
        //            row.Cells[2].ValueType = cell.Value.GetType();
        //        }
        //    }
        //}

        //private void DgvCells_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        //{
        //    if (e.ColumnIndex != 2)
        //        return;

        //    DataGridViewRow row = dgvCells.Rows[e.RowIndex];
        //    if (!(row.Cells[2] is DataGridViewComboBoxCell))
        //    {
        //        var cell = (PARAM.Cell)row.DataBoundItem;
        //        if (cell.Type == CellType.x8)
        //        {
        //            e.Value = $"0x{e.Value:X2}";
        //            e.FormattingApplied = true;
        //        }
        //        else if (cell.Type == CellType.x16)
        //        {
        //            e.Value = $"0x{e.Value:X4}";
        //            e.FormattingApplied = true;
        //        }
        //        else if (cell.Type == CellType.x32)
        //        {
        //            e.Value = $"0x{e.Value:X8}";
        //            e.FormattingApplied = true;
        //        }
        //    }
        //}

        //private void DgvCells_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        //{
        //    if (e.ColumnIndex != 2)
        //        return;

        //    DataGridViewRow row = dgvCells.Rows[e.RowIndex];
        //    try
        //    {
        //        if (!(row.Cells[2] is DataGridViewComboBoxCell))
        //        {
        //            var cell = (PARAM.Cell)row.DataBoundItem;
        //            if (cell.Type == CellType.x8)
        //                Convert.ToByte((string)e.FormattedValue, 16);
        //            else if (cell.Type == CellType.x16)
        //                Convert.ToUInt16((string)e.FormattedValue, 16);
        //            else if (cell.Type == CellType.x32)
        //                Convert.ToUInt32((string)e.FormattedValue, 16);
        //        }
        //    }
        //    catch
        //    {
        //        e.Cancel = true;
        //        dgvCells.EditingPanel.BackColor = Color.Pink;
        //        dgvCells.EditingControl.BackColor = Color.Pink;
        //        SystemSounds.Hand.Play();
        //    }
        //}

        //private void DgvCells_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        //{
        //    if (e.ColumnIndex != 2)
        //        return;

        //    DataGridViewRow row = dgvCells.Rows[e.RowIndex];
        //    if (!(row.Cells[2] is DataGridViewComboBoxCell))
        //    {
        //        var cell = (PARAM.Cell)row.DataBoundItem;
        //        if (cell.Type == CellType.x8)
        //        {
        //            e.Value = Convert.ToByte((string)e.Value, 16);
        //            e.ParsingApplied = true;
        //        }
        //        else if (cell.Type == CellType.x16)
        //        {
        //            e.Value = Convert.ToUInt16((string)e.Value, 16);
        //            e.ParsingApplied = true;
        //        }
        //        else if (cell.Type == CellType.x32)
        //        {
        //            e.Value = Convert.ToUInt32((string)e.Value, 16);
        //            e.ParsingApplied = true;
        //        }
        //    }
        //}

        //private void DgvCells_DataError(object sender, DataGridViewDataErrorEventArgs e)
        //{
        //    e.Cancel = true;
        //    if (dgvCells.EditingPanel != null)
        //    {
        //        dgvCells.EditingPanel.BackColor = Color.Pink;
        //        dgvCells.EditingControl.BackColor = Color.Pink;
        //    }
        //    SystemSounds.Hand.Play();
        //}

        //private void DgvCells_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        //{
        //    if (e.RowIndex >= 0 && e.ColumnIndex == 1)
        //    {
        //        var cell = (PARAM.Cell)dgvCells.Rows[e.RowIndex].DataBoundItem;
        //        e.ToolTipText = cell.Description;
        //    }
        //}

        private string GetResRoot()
        {
            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;
            if (!Directory.Exists("res"))
            {
                return $@"..\..\..\..\dist\res\{gameMode.Directory}";
            }
            return $@"res\{gameMode.Directory}";
        }
    }
}
