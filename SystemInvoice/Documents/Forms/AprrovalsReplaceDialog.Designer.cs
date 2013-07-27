namespace SystemInvoice.Documents.Forms
    {
    partial class AprrovalsReplaceDialog
        {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
            {
            if (disposing && (components != null))
                {
                components.Dispose();
                }
            base.Dispose( disposing );
            }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
            {
            this.yesBtn = new DevExpress.XtraEditors.SimpleButton();
            this.yesForAllBtn = new DevExpress.XtraEditors.SimpleButton();
            this.noBtn = new DevExpress.XtraEditors.SimpleButton();
            this.noForAllBtn = new DevExpress.XtraEditors.SimpleButton();
            this.infoLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // yesBtn
            // 
            this.yesBtn.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.yesBtn.Location = new System.Drawing.Point(11, 157);
            this.yesBtn.Name = "yesBtn";
            this.yesBtn.Size = new System.Drawing.Size(67, 28);
            this.yesBtn.TabIndex = 0;
            this.yesBtn.Text = "Да";
            // 
            // yesForAllBtn
            // 
            this.yesForAllBtn.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.yesForAllBtn.Location = new System.Drawing.Point(84, 157);
            this.yesForAllBtn.Name = "yesForAllBtn";
            this.yesForAllBtn.Size = new System.Drawing.Size(83, 28);
            this.yesForAllBtn.TabIndex = 1;
            this.yesForAllBtn.Text = "Да для всех";
            // 
            // noBtn
            // 
            this.noBtn.DialogResult = System.Windows.Forms.DialogResult.No;
            this.noBtn.Location = new System.Drawing.Point(173, 157);
            this.noBtn.Name = "noBtn";
            this.noBtn.Size = new System.Drawing.Size(67, 28);
            this.noBtn.TabIndex = 2;
            this.noBtn.Text = "Нет";
            // 
            // noForAllBtn
            // 
            this.noForAllBtn.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.noForAllBtn.Location = new System.Drawing.Point(246, 157);
            this.noForAllBtn.Name = "noForAllBtn";
            this.noForAllBtn.Size = new System.Drawing.Size(92, 28);
            this.noForAllBtn.TabIndex = 3;
            this.noForAllBtn.Text = "Нет для всех";
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Location = new System.Drawing.Point(8, 9);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(0, 13);
            this.infoLabel.TabIndex = 4;
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AprrovalsReplaceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 203);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.noForAllBtn);
            this.Controls.Add(this.noBtn);
            this.Controls.Add(this.yesForAllBtn);
            this.Controls.Add(this.yesBtn);
            this.Name = "AprrovalsReplaceDialog";
            this.Text = "AprrovalsReplaceDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

            }

        #endregion

        private DevExpress.XtraEditors.SimpleButton yesBtn;
        private DevExpress.XtraEditors.SimpleButton yesForAllBtn;
        private DevExpress.XtraEditors.SimpleButton noBtn;
        private DevExpress.XtraEditors.SimpleButton noForAllBtn;
        private System.Windows.Forms.Label infoLabel;

        }
    }