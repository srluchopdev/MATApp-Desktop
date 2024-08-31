namespace MATApp_Desktop
{
    partial class Form1
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
            btnAssign = new Button();
            lstColliders = new ListBox();
            lstAssignments = new ListBox();
            cmbIPs = new ComboBox();
            SuspendLayout();
            // 
            // btnAssign
            // 
            btnAssign.Location = new Point(281, 422);
            btnAssign.Name = "btnAssign";
            btnAssign.Size = new Size(290, 29);
            btnAssign.TabIndex = 0;
            btnAssign.Text = "asignar colliders a tracker\r\n";
            btnAssign.UseVisualStyleBackColor = true;
            btnAssign.Click += btnAssign_Click;
            // 
            // lstColliders
            // 
            lstColliders.BackColor = Color.DeepSkyBlue;
            lstColliders.BorderStyle = BorderStyle.FixedSingle;
            lstColliders.Font = new Font("Segoe UI", 13F);
            lstColliders.ForeColor = SystemColors.Menu;
            lstColliders.FormattingEnabled = true;
            lstColliders.ItemHeight = 23;
            lstColliders.Location = new Point(12, 12);
            lstColliders.Name = "lstColliders";
            lstColliders.Size = new Size(244, 439);
            lstColliders.TabIndex = 1;
            lstColliders.SelectedIndexChanged += lstColliders_SelectedIndexChanged;
            // 
            // lstAssignments
            // 
            lstAssignments.BackColor = Color.DeepSkyBlue;
            lstAssignments.BorderStyle = BorderStyle.FixedSingle;
            lstAssignments.Font = new Font("Segoe UI", 13F);
            lstAssignments.ForeColor = SystemColors.Menu;
            lstAssignments.FormattingEnabled = true;
            lstAssignments.ItemHeight = 23;
            lstAssignments.Location = new Point(594, 12);
            lstAssignments.Name = "lstAssignments";
            lstAssignments.Size = new Size(230, 439);
            lstAssignments.TabIndex = 2;
            lstAssignments.SelectedIndexChanged += lstAssignments_SelectedIndexChanged;
            // 
            // cmbIPs
            // 
            cmbIPs.BackColor = Color.DeepSkyBlue;
            cmbIPs.Font = new Font("Segoe UI", 13F);
            cmbIPs.ForeColor = SystemColors.Window;
            cmbIPs.FormattingEnabled = true;
            cmbIPs.Location = new Point(281, 12);
            cmbIPs.Name = "cmbIPs";
            cmbIPs.Size = new Size(290, 31);
            cmbIPs.TabIndex = 3;
            cmbIPs.SelectedIndexChanged += cmbIPs_SelectedIndexChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Indigo;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(837, 471);
            Controls.Add(cmbIPs);
            Controls.Add(lstAssignments);
            Controls.Add(lstColliders);
            Controls.Add(btnAssign);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "Form1";
            Text = "MATApp Descktop";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button btnAssign;
        private ListBox lstColliders;
        private ListBox lstAssignments;
        private ComboBox cmbIPs;
    }
}