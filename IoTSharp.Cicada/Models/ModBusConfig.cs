using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTSharp.Cicada.Models
{
   public  class ModBusConfig
    {
        public string  DataType { get; set; }
        public string ValueType { get; set; }
        public string KeyNameOrPrefix { get; set; }
        public string Address { get; set; }
        public int Lenght { get; set; }
        public Uri ModBusUri { get; set; }
    }
}
