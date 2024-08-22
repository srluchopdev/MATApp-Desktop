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
            btnAssign.Location = new Point(262, 362);
            btnAssign.Name = "btnAssign";
            btnAssign.Size = new Size(290, 29);
            btnAssign.TabIndex = 0;
            btnAssign.Text = "asignar colliders a tracker\r\n";
            btnAssign.UseVisualStyleBackColor = true;
            btnAssign.Click += btnAssign_Click;
            // 
            // lstColliders
            // 
            lstColliders.FormattingEnabled = true;
            lstColliders.ItemHeight = 15;
            lstColliders.Location = new Point(12, 12);
            lstColliders.Name = "lstColliders";
            lstColliders.Size = new Size(244, 379);
            lstColliders.TabIndex = 1;
            // 
            // lstAssignments
            // 
            lstAssignments.FormattingEnabled = true;
            lstAssignments.ItemHeight = 15;
            lstAssignments.Location = new Point(558, 12);
            lstAssignments.Name = "lstAssignments";
            lstAssignments.Size = new Size(230, 379);
            lstAssignments.TabIndex = 2;
            // 
            // cmbIPs
            // 
            cmbIPs.FormattingEnabled = true;
            cmbIPs.Location = new Point(262, 12);
            cmbIPs.Name = "cmbIPs";
            cmbIPs.Size = new Size(290, 23);
            cmbIPs.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(cmbIPs);
            Controls.Add(lstAssignments);
            Controls.Add(lstColliders);
            Controls.Add(btnAssign);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button btnAssign;
        private ListBox lstColliders;
        private ListBox lstAssignments;
        private ComboBox cmbIPs;
    }
}