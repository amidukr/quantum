using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Quantum
{
    public partial class JoinWindow : System.Windows.Forms.Form
    {
        public JoinWindow()
        {
            InitializeComponent();
        }

        private void joinButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public String HostAddress
        {
            get
            {
                return hostName.Text;
            }
        }
    }
}
