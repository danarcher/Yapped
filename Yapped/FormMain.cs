using Semver;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yapped.Grids;
using Yapped.Grids.Generic;
using GameType = Yapped.GameMode.GameType;

namespace Yapped
{
    public partial class FormMain : Form
    {
        private const string UPDATE_URL = "https://www.nexusmods.com/sekiro/mods/121?tab=files";
        private static Properties.Settings settings = Properties.Settings.Default;

        private ParamRoot root;
        private string lastFindRowPattern, lastFindFieldPattern;

        private Grid paramsGrid, rowsGrid, cellsGrid;
        private SelectionMemory memory;

        private ToolStripMenuItem toolsMenu;
        private ToolStripMenuItem toolsWeaponDamage;

        public FormMain()
        {
            InitializeComponent();
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

            var fileExit = new ToolStripMenuItem("E&xit");
            fileToolStripMenuItem.DropDownItems.Add("-");
            fileToolStripMenuItem.DropDownItems.Add(fileExit);
            fileExit.Click += (s, e) => Close();

            toolsMenu = new ToolStripMenuItem("&Tools");
            menuStrip1.Items.Add(toolsMenu);
            toolsWeaponDamage = new ToolStripMenuItem("&Weapon Damage");
            toolsMenu.DropDownItems.Add(toolsWeaponDamage);
            toolsWeaponDamage.Click += (s, e) => FormWeaponDamage.ShowDialog(largeFont, root);

            EnableDisable();
        }

        protected override void OnLoad(EventArgs e)
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

            hideUnusedParamsToolStripMenuItem.Checked = settings.HideUnusedParams;
            verifyDeletionsToolStripMenuItem.Checked = settings.VerifyRowDeletion;
            splitContainer2.SplitterDistance = settings.SplitterDistance2;
            splitContainer1.SplitterDistance = settings.SplitterDistance1;

            memory.Load(settings.DGVIndices);
            LoadParams(settings.RegulationPath);

            _ = CheckForUpdatesAsync();

            base.OnLoad(e);
        }

        private async Task CheckForUpdatesAsync()
        {
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
            settings.RegulationPath = root?.Path ?? settings.RegulationPath;
            settings.HideUnusedParams = hideUnusedParamsToolStripMenuItem.Checked;
            settings.VerifyRowDeletion = verifyDeletionsToolStripMenuItem.Checked;
            settings.SplitterDistance2 = splitContainer2.SplitterDistance;
            settings.SplitterDistance1 = splitContainer1.SplitterDistance;
            settings.DGVIndices = memory.Save();
        }

        private void LoadParams(string path)
        {
            try
            {
                root = ParamRoot.Load(path,
                    (GameMode)toolStripComboBoxGame.SelectedItem,
                    hideUnusedParamsToolStripMenuItem.Checked,
                    GetResRoot());
                paramsGrid.Host = new ParamsGridHost(memory, rowsGrid, cellsGrid) { DataSource = root };
                EnableDisable();
            }
            catch (Exception ex)
            {
                exportNamesToolStripMenuItem.Enabled = false;
                Util.ShowError(ex.Message);
            }

        }

        private void EnableDisable()
        {
            var loaded = root != null;
            exportToolStripMenuItem.Enabled = loaded;
            saveToolStripMenuItem.Enabled = loaded;
            restoreToolStripMenuItem.Enabled = loaded;
            exploreToolStripMenuItem.Enabled = loaded;
            exportToolStripMenuItem.Enabled = loaded && root.CanExport;
            editToolStripMenuItem.Enabled = loaded;
            toolsMenu.Enabled = loaded;
            toolStripStatusLabel1.Text = root?.Path ?? string.Empty;
        }

        #region File Menu
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofdRegulation.FileName = root?.Path;
            if (ofdRegulation.ShowDialog() == DialogResult.OK)
            {
                LoadParams(ofdRegulation.FileName);
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            root.Save((GameMode)toolStripComboBoxGame.SelectedItem);
            SystemSounds.Asterisk.Play();
        }

        private void RestoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var backupPath = root.Path + ".bak";
            if (File.Exists(backupPath))
            {
                DialogResult choice = MessageBox.Show("Are you sure you want to restore the backup?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (choice == DialogResult.Yes)
                {
                    try
                    {
                        File.Copy(backupPath, root.Path, true);
                        LoadParams(root.Path);
                        SystemSounds.Asterisk.Play();
                    }
                    catch (Exception ex)
                    {
                        Util.ShowError($"Failed to restore backup\r\n\r\n{backupPath}\r\n\r\n{ex}");
                    }
                }
            }
            else
            {
                Util.ShowError($"There is no backup to restore at:\r\n\r\n{backupPath}");
            }
        }

        private void ExploreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Path.GetDirectoryName(root.Path));
            }
            catch
            {
                SystemSounds.Hand.Play();
            }
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fbdExport.SelectedPath = Path.GetDirectoryName(root.Path);
            if (fbdExport.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    root.Export(fbdExport.SelectedPath);
                }
                catch (Exception ex)
                {
                    Util.ShowError(ex.Message);
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
            foreach (ParamWrapper paramFile in root)
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
            foreach (ParamWrapper paramFile in root)
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
