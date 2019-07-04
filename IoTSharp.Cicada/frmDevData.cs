using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using IoTSharp.Sdk;
using System.Threading;
using IoTSharp.Cicada.Models;
using DevExpress.XtraBars.Ribbon;

namespace IoTSharp.Cicada
{
    public partial class frmDevData : RibbonForm
    {
        public frmDevData()
        {
            InitializeComponent();
        }

        public Customer Customer { get; set; }
 
 

        private void frmDevData_Load(object sender, EventArgs e)
        {
            Client = SdkClient.Create<DevicesClient>();
            enumKeyValueBindingSource.BindingEnum<DeviceType>();
        }


        DevicesClient Client;

        private void btnGetToken_ItemClickAsync(object sender, ItemClickEventArgs e)
        {
        }
        public Device Device { get; set; }

        private async Task ReloadLatest()
        {
            try
            {
                Device row = null;
                this.Invoke((MethodInvoker)async delegate
              {
                  var dev = SdkClient.Create<DevicesClient>();
                  var ids = await dev.GetIdentityAsync( Device.Id);
                  txtToken.EditValue = ids.IdentityId;
                  //XtraMessageBox.Show(ids.ToJson());
                  lblInfo.Caption = "已经获取到该设备Token";
              });

                if (row != null)
                {
                    var dev = SdkClient.Create<DevicesClient>();
                    var al = await dev.GetAttributeLatestAllAsync(row.Id);
                    var tl = await dev.GetTelemetryLatestAllAsync(row.Id);
                    this.Invoke((MethodInvoker)delegate
                    {
                        attributeLatestBindingSource.DataSource = al;
                        telemetryLatestBindingSource.DataSource = tl;
                    });
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lblInfo.Caption = ex.Message;
                });
            }
        }

        
        DateTime lastdate = DateTime.MinValue;
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Subtract(lastdate).TotalSeconds>5)
            {
                Task.Run(async () => await ReloadLatest());
            }
        }
    }
}