using System;
using System.ComponentModel;

namespace COVID_20.ViewModels {
    public class CaseFilterViewModel {
        public int? ProvinceID { get; set; }
        public bool? ICU { get; set; }
        public bool? Dead { get; set; }
        public PatientSex? Sex { get; set; }
        public CaseStatus? Status { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public bool? PublicFunding { get; set; }
        public int? AgeLowerBound { get; set; }
        public int? AgeUpperBound { get; set; }

        public enum CaseStatus {
            Confirmado,
            Sospechoso,
            Descartado
        }

        public enum PatientSex {
            M,
            F,
            NR
        }
    }
}