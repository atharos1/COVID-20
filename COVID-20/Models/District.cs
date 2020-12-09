using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COVID_20.Models {
    public class District {
        public int ID { get; set; }
        public int CsvID { get; set; }
        public string Name { get; set; }

        public override int GetHashCode() {
            return ID;
        }

        public override bool Equals(object other) {
            return Equals(other as District);
        }

        public virtual bool Equals(District other) {
            if (other == null) { return false; }
            if (object.ReferenceEquals(this, other)) { return true; }

            return this.ID == other.ID;
        }
    }
}
