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

namespace IoTSharp.Cicada
{
    public partial class frmDevices : AdminBase<Device>
    {
        public frmDevices()
        {
            InitializeComponent();
        }

        public Customer Customer { get; set; }

        private void bbiNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            DoNew();
        }

        private void bbiEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            DoEdit();
        }

        private void bbiDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            DoDelete();
        }

        private void bbiRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            DoRefresh();
        }

        private IoTSharp.Sdk.DevicesClient Client = null;

        public override Task<FileResponse> Put(Device obj, CancellationToken token)
        {
            return Client.PutDeviceAsync(obj.Id,  new DevicePutDto(){   Id=obj.Id, Name=obj.Name }, token);
        }

        public override Task<Device> Post(Device obj, CancellationToken token)
        {
            return Client.PostDeviceAsync( new  DevicePostDto () {  DeviceType=obj.DeviceType, Name=obj.Name} , token);
        }

        public override Task<ICollection<Device>> GetAllAsync(CancellationToken token)
        {
            return Client.GetDevicesAllAsync(Customer.Id, token);
        }

        public override Task<Device> Delete(Device obj, CancellationToken token)
        {
            return Client.DeleteDeviceAsync(obj.Id, token);
        }

        private void frmCustomerAdmin_Load(object sender, EventArgs e)
        {
            InitializeGridView(gridView1, colId);
            Client = SdkClient.Create<DevicesClient>();
            enumKeyValueBindingSource.BindingEnum<DeviceType>();
        }

        private void bbiPrintPreview_ItemClick(object sender, ItemClickEventArgs e)
        {
            gridControl.ShowPrintPreview();
        }

        private void btnUserAdmin_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private async void btnAttTest_ItemClick(object sender, ItemClickEventArgs e)
        {
            var dev = SdkClient.Create<DevicesClient>();
            var dis = new Dictionary<string, object>();
            dis.Add("boolvalue", true);
            dis.Add("jsonvalue", new { a = 1, b = "sss", c = false, e = DateTime.Now });
            dis.Add("longvalue", 2342343L);
            dis.Add("Doublevalue", 2332.322);
            await dev.Attributes2Async(txtToken.EditValue.ToString(), dis);
            await ReloadLatest();
        }

        private async void btnGetToken_ItemClickAsync(object sender, ItemClickEventArgs e)
        {
            try
            {
                var row = FocusedRow;
                if (row != null)
                {
                    var dev = SdkClient.Create<DevicesClient>();
                    var ids = await dev.GetIdentityAsync(row.Id);
                    txtToken.EditValue = ids.IdentityId;
                    //XtraMessageBox.Show(ids.ToJson());
                    lblInfo.Caption = "已经获取到该设备Token";
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private async void deviceBindingSource_CurrentChangedAsync(object sender, EventArgs e)
        {
            btnGetToken.PerformClick();
            await ReloadLatest();
        }

        private async Task ReloadLatest()
        {
            try
            {
                Device row = null;
                this.Invoke((MethodInvoker)delegate
              {
                  row = FocusedRow;
                  if (row != null)
                  {
                      rpgModBus.Enabled = row.DeviceType == DeviceType.Gateway;
                      if (row.DeviceType == DeviceType.Gateway)
                      {
                          if (modBusConfigBindingSource.Count==0)
                          {
                              modBusConfigBindingSource.Add( new ModBusConfig()
                              {
                                  Address = "100",
                                  KeyNameOrPrefix = "ModBus",
                                  ModBusUri = new Uri("modbus://127.0.0.1:502/1"),
                                  Lenght = 1,
                                  ValueType = "String",
                                  DataType = "Telemetry"
                              });
                          }
                      }
                  }
                  
              });

                if (row != null)
                {
                    var dev = SdkClient.Create<DevicesClient>();
                    var al = await dev.GetTelemetryLatestAllAsync(row.Id);
                    var tl = await dev.GetAttributeLatestAllAsync(row.Id);
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

        private async void btnTTest_ItemClickAsync(object sender, ItemClickEventArgs e)
        {
            var dev = SdkClient.Create<DevicesClient>();
            var dis = new Dictionary<string, object>();
            dis.Add("boolvalue", true);
            dis.Add("jsonvalue", new { a = 1, b = "sss", c = false, e = DateTime.Now });
            dis.Add("longvalue", 2342343L);
            dis.Add("Doublevalue", 2332.322);
            await dev.TelemetryAsync(txtToken.EditValue.ToString(), dis);
            await ReloadLatest();
        }
        DateTime lastdate = DateTime.MinValue;
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Subtract(lastdate).TotalSeconds>5)
            {
                Task.Run(async () => await ReloadLatest());
            }
        }

        private void BtnModBus_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private async void  BtnConfigBus_ItemClickAsync(object sender, ItemClickEventArgs e)
        {
            var mc = modBusConfigBindingSource.Current as ModBusConfig;

            var dev = SdkClient.Create<DevicesClient>();
            var dis = new Dictionary<string, object>();
            dis.Add("ModBusConfig", mc);
            await dev.Attributes2Async(txtToken.EditValue.ToString(), dis);
            await ReloadLatest();


        }

        private void BtnReadModBus_ItemClick(object sender, ItemClickEventArgs e)
        {
            var al = attributeLatestBindingSource.Current as AttributeLatest;
            if (al != null)
            {
                var mconfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ModBusConfig>(al.Value_Json);
                if (mconfig!=null)
                {
                    modBusConfigBindingSource.DataSource = mconfig;
                }
            }
        }

        private void BarButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void BarButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}