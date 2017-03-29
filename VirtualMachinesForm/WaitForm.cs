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

namespace VirtualMachinesForm
{
    public partial class WaitForm : Form
    {
        private Thread thread;        

        public string Message
        {
            set
            {
                if (!String.IsNullOrEmpty(value))
                    label1.Text = value;
            }
        }

        public WaitForm()
        {
            InitializeComponent();
        }

        public void Start()
        {
            thread = new Thread(() => ShowDialog());
            thread.Start();
        }

        public void Stop()
        {
            if(thread != null)
                thread.Abort();
            Close();
        }
    }
}
