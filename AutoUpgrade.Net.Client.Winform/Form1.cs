using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoUpgrade.Net.Client.Winform
{
    public partial class Form1 : Form, IUILinker
    {
        private SynchronizationContext synchronizationContext;
        public Form1()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }

        public bool Version { get; set; }
        public bool UpgradeLog { get; set; }
        public bool Skipable { get; set; }
        public bool RemindMeLaterable { get; set; }

        public bool Skip => true;

        public TimeSpan RemindMeLater => TimeSpan.FromMilliseconds(0);

        public void OutputMessage(string msg)
        {
            synchronizationContext.Send((obj) =>
            {
                this.Text = msg;
            }, null);
        }

        public void Upgrade()
        {
        }

        protected async override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            await UpgradeProcess.DoUpgradeAsync(this);
            this.Close();
        }
    }
}
