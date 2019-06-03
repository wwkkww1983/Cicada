using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using IoTSharp.Sdk;

namespace IoTSharp.Cicada
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BonusSkins.Register();
#if DEBUG
            SdkClient.BaseURL = Properties.Settings.Default.ApiUrl;
#else
            SdkClient.BaseURL = "http://www.iotsharp.net";
#endif
            Application.Run(new frmMain());
        }
    }
}