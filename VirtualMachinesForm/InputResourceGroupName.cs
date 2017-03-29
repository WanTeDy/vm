using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualMachinesForm
{
    public partial class InputResourceGroupName : Form
    {
        public string GroupName { get; set; }
        public InputResourceGroupName()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            GroupName = inputTextBox.Text;
            if (String.IsNullOrEmpty(GroupName))
            {
                MessageBox.Show("Введите пожалуйста название группы!", "Ошибка", MessageBoxButtons.OK);
                return;
            }
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
