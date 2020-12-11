using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COVID_20.Models {
    public class DatabaseUpdate {
        public int ID { get; set; }
        public DateTime? Timestamp { get; set; }
        public int LoadedRows { get; set; }
    }
}
