using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Yapped
{
    public partial class FormNewRow : Form
    {
        public string Prompt { get; set; }
        public long ResultID { get; set; }
        public string ResultName { get; set; }

        public FormNewRow()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            Text = Prompt;
            idUpDown.Value = ResultID;
            nameTextBox.Text = ResultName;
            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ResultID = (long)idUpDown.Value;
            ResultName = nameTextBox.Text.Length > 0 ? nameTextBox.Text : null;
            base.OnClosing(e);
        }

        private void nudID_Enter(object sender, EventArgs e)
        {
            idUpDown.Select(0, idUpDown.Text.Length);
        }
    }
}
