﻿using System;
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
using System.Threading;

namespace IoTSharp.Cicada
{
    public partial class frmLogin : DevExpress.XtraEditors.XtraForm
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private async void btnLogin_ClickAsync(object sender, EventArgs e)
        {
            var Client = SdkClient.Create<IoTSharp.Sdk.Http.AccountClient>();
            string username = txtUserName.Text;
            string password = txtPassword.Text;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblInfo.Text = "密码或用户名为空";
            }
            else
            {
                try
                {
                    var result = await Client.LoginAsync(username, password);
                    if (result.IsLogin)
                    {
                        DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        lblInfo.Text = "密码错误";
                    }
                }
                catch (SwaggerException sex)
                {
                    var result = sex.ToResult();
                    if (result != null)
                    {
                        lblInfo.Text = $"错误代码:{result.Code},错误:{result.Msg}";
                    }
                    else
                    {
                        lblInfo.Text = sex.Message;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private bool _isMouseDown;
        private Point _formLocation; //form的location
        private Point _mouseOffset;

        private void frmLogin_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isMouseDown = true;
                _formLocation = Location;
                _mouseOffset = MousePosition;
            }
        }

        private void frmLogin_MouseUp(object sender, MouseEventArgs e)
        {
            _isMouseDown = false;
        }

        private void frmLogin_MouseMove(object sender, MouseEventArgs e)
        {
            //鼠标的按下位置
            if (_isMouseDown)
            {
                Point pt = MousePosition;
                int x = _mouseOffset.X - pt.X;
                int y = _mouseOffset.Y - pt.Y;

                Location = new Point(_formLocation.X - x, _formLocation.Y - y);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            cts.Cancel(false);
            DialogResult = DialogResult.Cancel;
        }

        private CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        private void frmLogin_Load(object sender, EventArgs e)
        {
            lblVersion.Text = "V" + AppUtils.AssemblyVersion;
            btnLogin.Enabled = false;
            lblInfo.Text = "正在检查服务器...";

            cts.Token.Register(() => Console.WriteLine(DateTime.Now.ToString() + "取消"));
            Task.Run(async () =>
            {
                try
                {
                    var Client = SdkClient.Create<IoTSharp.Sdk.Http.InstallerClient>();
                    var fr = await Client.InstanceAsync();
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (fr.Installed)
                        {
                            btnLogin.Enabled = true;
                            lblInfo.Text = "服务器就绪";
                            lblVersion.Text = fr.Version;
                        }
                        else
                        {
                            DialogResult = DialogResult.No;
                        }
                    });
                }
                catch (SwaggerException se)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        lblInfo.Text = se.ToResult().ToString();
                    });
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        lblInfo.Text = ex.Message;
                    });
                }
            }, cts.Token);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AppUtils.ShowFileByExplorer("http://www.iotsharp.net/");
        }
    }
}