using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COVID_20.Models {
    public class Case {
        public int ID { get; set; }
        public int CSVCaseId { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }

        public string ResidenceCountryName { get; set; }
        public Province ResidenceProvince { get; set; }
        public District ResidenceDistrict { get; set; }
        public Province LoaderProvince { get; set; }

        public DateTime? SymptomsStartDate { get; set; }
        public DateTime? DiagnoseDate { get; set; }
        public DateTime? CaseOpeningDate { get; set; }
        public int SepiOpening { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public bool IntensiveCare { get; set; }
        public DateTime? IntensiveCareDate { get; set; }
        public bool Deceased { get; set; }
        public DateTime? DeceasedDate { get; set; }
        public bool Respirator { get; set; }

        public bool PublicFounding { get; set; }

        public string Classification { get; set; }
        public string ClassificationDetail { get; set; }

        public DateTime CSVLastUpdated { get; set; }
    }
}
