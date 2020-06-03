using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using  IoTSharp.Sdk.Http;
using DevExpress.XtraLayout.Utils;
using IoTSharp.Cicada.Models;

namespace IoTSharp.Cicada
{
    public partial class XtraDataEditor : DevExpress.XtraEditors.XtraUserControl
    {
        public XtraDataEditor()
        {
            InitializeComponent();
        }
        DataStorage _dataStorage = null;
        [DefaultValue(null)]
        public DataStorage DataStorage
        {
            get
            {
                if (this.IsHandleCreated && !this.IsDisposed && !this.DesignMode)
                {
                    _dataStorage = attributeLatestBindingSource.Current as DataStorage;
                }
                return _dataStorage;
            }
            set
            {
                _dataStorage = value;
                if (this.IsHandleCreated && !this.IsDisposed && !this.DesignMode)
                {
                    if (_dataStorage != null)
                    {
                        attributeLatestBindingSource.DataSource = _dataStorage;
                        ItemForValue_Binary.Visibility = LayoutVisibility.Never;
                        ItemForValue_Boolean.Visibility = LayoutVisibility.Never;
                        ItemForValue_DateTime.Visibility = LayoutVisibility.Never;
                        ItemForValue_Double.Visibility = LayoutVisibility.Never;
                        ItemForValue_Json.Visibility = LayoutVisibility.Never;
                        ItemForValue_Long.Visibility = LayoutVisibility.Never;
                        ItemForValue_String.Visibility = LayoutVisibility.Never;
                        ItemForValue_XML.Visibility = LayoutVisibility.Never;
                        var ctl = this.Parent;
                        ctl.Width = 400;
                        switch (_dataStorage.Type)
                        {
                            case DataType.Binary:
                                ItemForValue_Binary.Visibility = LayoutVisibility.Always;
                                ctl.Height = 100 + ItemForValue_Binary.Height;
                                break;
                            case DataType.Boolean:
                                ItemForValue_Boolean.Visibility = LayoutVisibility.Always;
                                ctl.Height = 100 + ItemForValue_Boolean.Height;
                                break;
                            case DataType.DateTime:
                                ItemForValue_DateTime.Visibility = LayoutVisibility.Always;
                                ctl.Height = 100 + ItemForValue_DateTime.Height;
                                break;
                            case DataType.Double:
                                ItemForValue_Double.Visibility = LayoutVisibility.Always;
                                ctl.Height = 100 + ItemForValue_Double.Height;
                                break;
                            case DataType.Json:
                                ItemForValue_Json.Visibility = LayoutVisibility.Always;
                                ctl.Height = 400;
                                ctl.Width = 600;
                                break;
                            case DataType.Long:
                                ItemForValue_Long.Visibility = LayoutVisibility.Always;
                                ctl.Height = 100 + ItemForValue_Long.Height;
                                break;
                            case DataType.String:
                                ItemForValue_String.Visibility = LayoutVisibility.Always;
                                ctl.Height = 100 + ItemForValue_String.Height;
                                break;
                            case DataType.XML:
                                ItemForValue_XML.Visibility = LayoutVisibility.Always;
                                ctl.Height = 100 + ItemForValue_XML.Height;
                                break;
                            default:
                                break;
                        }
                       
                    }
                }
            }
        }
    }
}
