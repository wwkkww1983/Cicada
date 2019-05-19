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
using DevExpress.XtraBars;
using System.ComponentModel.DataAnnotations;
using IoTSharp.Sdk;

namespace IoTSharp.Cicada
{
    public partial class frmUserAdmin : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public frmUserAdmin()
        {
            InitializeComponent();
        }
        void bbiPrintPreview_ItemClick(object sender, ItemClickEventArgs e)
        {
            gridControl.ShowRibbonPrintPreview();
        }

        Sdk.AccountClient Client = null;
        public Customer Customer    { get; set; }
        private void FrmUserAdmin_Load(object sender, EventArgs e)
        {
            Client = Sdk.SdkClient.Create<AccountClient>();
            Task.Run(Reload);
        }

        private async Task Reload()
        {
            BindingList<UserItemDto> dataSource = new BindingList<UserItemDto>(( await Client.AllAsync(Customer.Id)).ToList()) ;
            gridControl.DataSource = dataSource;
            bsiRecordsCount.Caption = "RECORDS : " + dataSource.Count;
        }

        private async void BbiNew_ItemClickAsync(object sender, ItemClickEventArgs e)
        {
            frmCreateUser frm = new frmCreateUser() {  Customer= Customer };
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var result = await Client.RegisterAsync(frm.RegisterDto);
                    XtraMessageBox.Show(result.Succeeded ? "创建成功" : "创建失败");
                }
                catch (SwaggerException  ex )
                {
                    XtraMessageBox.Show(ex.Message);
                }
                await Reload();
            }
        }

        private void BbiRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            Task.Run(Reload);
        }
    }
}