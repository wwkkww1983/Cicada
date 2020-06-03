﻿using IoTSharp.Sdk.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IoTSharp.Cicada
{
    public partial class frmMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnLogin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmLogin fl = new frmLogin();
            var result = fl.ShowDialog(this);
            if (result == DialogResult.OK)
            {

                sessionBindingSource.DataSource = Sdk.Http.SdkClient.Session;
            }
            else if (result == DialogResult.No)
            {
                frmInstaller installer = new frmInstaller();
                if (installer.ShowDialog() == DialogResult.OK)
                {
                    btnLogin.PerformClick();
                }
            }
            else if (result == DialogResult.Cancel)
            {
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            sessionBindingSource.DataSource = new  Session(null);
        }

        private void SetMenuAndBar()
        {
        }

        private void btnExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Application.Exit();
        }

        private void btnTen_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ShowMdiChildren<frmTenantAdmin>();
        }

        private void btnDevices_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ShowMdiChildren<frmDevices>(opt =>
            {
                var cust = IoTSharp.Sdk.Http.SdkClient.Create<IoTSharp.Sdk.Http.DevicesClient>();
                opt.Customer = IoTSharp.Sdk.Http.SdkClient.MyInfo.Customer;
                opt.Text = $"设备管理-{opt.Customer.Name}";
            });
        }

        private void BtnLogout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
    }
}