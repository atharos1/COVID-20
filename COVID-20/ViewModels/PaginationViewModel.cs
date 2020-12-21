using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace COVID_20.ViewModels {
    public class PaginationViewModel {
        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;
        [DefaultValue(50)]
        public int PageSize { get; set; } = 50;
    }
}
