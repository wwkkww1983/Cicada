using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using  IoTSharp.Sdk.Http;

namespace IoTSharp.Cicada
{
    public partial class frmCreateUser : DevExpress.XtraEditors.XtraForm
    {
        public frmCreateUser()
        {
            InitializeComponent();
        }

        private void FrmCreateUser_Load(object sender, EventArgs e)
        {
            registerDtoBindingSource.DataSource = new Sdk.Http.RegisterDto() {  CustomerId = Customer.Id };
            Text = $"为{Customer?.Name}创建用户";
        }
        public Customer Customer { get; set; }

        public RegisterDto RegisterDto
        {
            get
            {

                return registerDtoBindingSource.Current as RegisterDto;
            }
            set
            {
                registerDtoBindingSource.DataSource = value;
            }

        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}