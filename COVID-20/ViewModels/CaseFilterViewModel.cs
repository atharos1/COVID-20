using System;

namespace COVID_20.ViewModels {
    public class CaseFilterViewModel {
        public int? ProvinceID { get; set; }
        public bool? ICU { get; set; }
        public bool? Dead { get; set; }
        public CaseStatus? Status { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public enum CaseStatus {
            Confirmado,
            Sospechoso,
            Descartado
        }
    }
}