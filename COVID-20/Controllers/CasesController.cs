using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using COVID_20.Models;
using COVID_20.Persistence;
using System.Net.Http;
using System.IO;
using CsvHelper;
using System.Globalization;
using Npgsql.Bulk;
using COVID_20.ViewModels;

namespace COVID_20.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CasesController : Controller {
        private readonly AppDbContext _context;

        public CasesController(AppDbContext context) {
            _context = context;
        }

        [HttpGet("province/{provinceId}")]
        public ActionResult<IEnumerable<Case>> Get(int provinceId, [FromQuery] CaseFilterViewModel filters) {
            var cases = FilterCases(filters, _context.Cases.Where(c => c.LoaderProvinceID == provinceId));

            return Ok(cases.ToListAsync());
        }

        [HttpGet("province/{provinceId}/count")]
        public ActionResult<int> Count(int provinceId, [FromQuery] CaseFilterViewModel filters) {
            var cases = FilterCases(filters, _context.Cases.Where(c => c.LoaderProvinceID == provinceId));

            return Ok(cases.Count());
        }

        // GET: Cases
        [HttpGet]
        public ActionResult<IEnumerable<Case>> Get([FromQuery] CaseFilterViewModel filters) {
            var cases = FilterCases(filters);

            return Ok(cases.ToListAsync());
        }

        [HttpGet("count")]
        public ActionResult<int> Count([FromQuery]CaseFilterViewModel filters) {
            var cases = FilterCases(filters);

            return Ok(cases.Count());
        }

        private IQueryable<Case> FilterCases(CaseFilterViewModel filters, IQueryable<Case> cases = null) {
            if (cases == null) cases = _context.Cases;

            if (filters.ICU != null)
                cases = cases.Where(c => c.IntensiveCare == filters.ICU);

            if (filters.Dead != null)
                cases = cases.Where(c => c.Deceased == filters.Dead);

            if (filters.Status != null)
                cases = cases.Where(c => c.Classification == Enum.GetName(typeof(CaseFilterViewModel.CaseStatus), filters.Status));

            if (filters.From != null)
                cases = cases.Where(c => c.CaseOpeningDate >= filters.From);

            if (filters.To != null)
                cases = cases.Where(c => c.CaseOpeningDate <= filters.To);

            return cases;
        }
    }
}
