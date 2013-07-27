using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SystemInvoice.Documents.Forms
    {
    public partial class AprrovalsReplaceDialog : Form
        {
        public enum ApprovalsItemFormDialogResult { Yes, No, YesForAll, NoForAll }

        private string message = "";
        public AprrovalsReplaceDialog(string message)
            {
            this.message = message;
            InitializeComponent();
            this.Load += AprrovalsReplaceDialog_Load;
            }

        void AprrovalsReplaceDialog_Load( object sender, EventArgs e )
            {
            this.infoLabel.Text = message;
            //this.infoLabel.Width = this.Width - 10;
            //this.infoLabel.Height = this.Height - 10;
            this.infoLabel.TextAlign = ContentAlignment.MiddleCenter;            
            }
        }
    }
