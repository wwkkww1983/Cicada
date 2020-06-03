using  IoTSharp.Sdk.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataType = IoTSharp.Sdk.Http.DataType;

namespace IoTSharp.Cicada.Models
{
    public class DeviceDataDto
    {
        public DeviceDataDto()
        {

        }
        public DeviceDataDto(DataStorage storage)
        {
            DataStorage = storage;
        }
      
        public DataStorage DataStorage { get; set; }

        public string KeyName { get { return DataStorage?.KeyName; }  }

        public DataSide DataSide { get { return (DataStorage?.DataSide).GetValueOrDefault(DataSide.AnySide); }}

        public DataCatalog Catalog { get { return (DataStorage?.Catalog).GetValueOrDefault( DataCatalog.AttributeLatest); } }

        public DataType Type { get { return (DataStorage?.Type).GetValueOrDefault(DataType.String ); } }

        public System.DateTime DateTime { get { return (DataStorage?.DateTime).GetValueOrDefault(DateTime.MinValue); }    }

      

    }
}
