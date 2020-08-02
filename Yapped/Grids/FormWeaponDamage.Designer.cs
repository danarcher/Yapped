namespace Yapped.Grids
{
    partial class FormWeaponDamage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWeaponDamage));
            this.strengthUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.agilityUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.magicUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.faithUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.luckUpDown = new System.Windows.Forms.NumericUpDown();
            this.panel = new System.Windows.Forms.Panel();
            this.updateButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.strengthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.agilityUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.magicUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.faithUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.luckUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // strengthUpDown
            // 
            this.strengthUpDown.Location = new System.Drawing.Point(12, 38);
            this.strengthUpDown.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.strengthUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.strengthUpDown.Name = "strengthUpDown";
            this.strengthUpDown.Size = new System.Drawing.Size(120, 23);
            this.strengthUpDown.TabIndex = 1;
            this.strengthUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.strengthUpDown.ValueChanged += new System.EventHandler(this.UpDown_ValueChanged);
            this.strengthUpDown.Enter += new System.EventHandler(this.UpDown_Enter);
            this.strengthUpDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UpDown_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Strength";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Agility";
            // 
            // agilityUpDown
            // 
            this.agilityUpDown.Location = new System.Drawing.Point(12, 82);
            this.agilityUpDown.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.agilityUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.agilityUpDown.Name = "agilityUpDown";
            this.agilityUpDown.Size = new System.Drawing.Size(120, 23);
            this.agilityUpDown.TabIndex = 3;
            this.agilityUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.agilityUpDown.ValueChanged += new System.EventHandler(this.UpDown_ValueChanged);
            this.agilityUpDown.Enter += new System.EventHandler(this.UpDown_Enter);
            this.agilityUpDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UpDown_MouseUp);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Magic";
            // 
            // magicUpDown
            // 
            this.magicUpDown.Location = new System.Drawing.Point(12, 126);
            this.magicUpDown.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.magicUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.magicUpDown.Name = "magicUpDown";
            this.magicUpDown.Size = new System.Drawing.Size(120, 23);
            this.magicUpDown.TabIndex = 5;
            this.magicUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.magicUpDown.ValueChanged += new System.EventHandler(this.UpDown_ValueChanged);
            this.magicUpDown.Enter += new System.EventHandler(this.UpDown_Enter);
            this.magicUpDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UpDown_MouseUp);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Faith";
            // 
            // faithUpDown
            // 
            this.faithUpDown.Location = new System.Drawing.Point(12, 170);
            this.faithUpDown.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.faithUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.faithUpDown.Name = "faithUpDown";
            this.faithUpDown.Size = new System.Drawing.Size(120, 23);
            this.faithUpDown.TabIndex = 7;
            this.faithUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.faithUpDown.ValueChanged += new System.EventHandler(this.UpDown_ValueChanged);
            this.faithUpDown.Enter += new System.EventHandler(this.UpDown_Enter);
            this.faithUpDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UpDown_MouseUp);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 196);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "Luck";
            // 
            // luckUpDown
            // 
            this.luckUpDown.Location = new System.Drawing.Point(12, 214);
            this.luckUpDown.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.luckUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.luckUpDown.Name = "luckUpDown";
            this.luckUpDown.Size = new System.Drawing.Size(120, 23);
            this.luckUpDown.TabIndex = 9;
            this.luckUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.luckUpDown.ValueChanged += new System.EventHandler(this.UpDown_ValueChanged);
            this.luckUpDown.Enter += new System.EventHandler(this.UpDown_Enter);
            this.luckUpDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UpDown_MouseUp);
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.Location = new System.Drawing.Point(138, 20);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(858, 697);
            this.panel.TabIndex = 11;
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(15, 243);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(117, 23);
            this.updateButton.TabIndex = 10;
            this.updateButton.Text = "&Calculate";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // FormWeaponDamage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.luckUpDown);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.faithUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.magicUpDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.agilityUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.strengthUpDown);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormWeaponDamage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Weapon Damage Calculator";
            ((System.ComponentModel.ISupportInitialize)(this.strengthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.agilityUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.magicUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.faithUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.luckUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown strengthUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown agilityUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown magicUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown faithUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown luckUpDown;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Button updateButton;
    }
}