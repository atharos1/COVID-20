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

        /*[HttpGet("province/{provinceId}")]
        public ActionResult<IEnumerable<Case>> Get(int provinceId, [FromQuery] CaseFilterViewModel filters) {
            var cases = FilterCases(filters, _context.Cases.Where(c => c.LoaderProvinceID == provinceId));

            return Ok(cases.ToListAsync());
        }

        [HttpGet("province/{provinceId}/count")]
        public ActionResult<int> Count(int provinceId, [FromQuery] CaseFilterViewModel filters) {
            var cases = FilterCases(filters, _context.Cases.Where(c => c.LoaderProvinceID == provinceId));

            return Ok(cases.Count());
        }*/

        // GET: Cases
        [HttpGet]
        public ActionResult<IEnumerable<Case>> Get([FromQuery] CaseFilterViewModel filters) {
            var cases = FilterCases(filters);

            return Ok(cases.ToList());
        }

        [HttpGet("summary")]
        public ActionResult<List<CasesStatsViewModel>> Summary([FromQuery] CaseFilterViewModel filters) {
            Province currProvince = filters.ProvinceID != null && filters.ProvinceID != 0 ?
                _context.Provinces.Find(filters.ProvinceID)
                : new Province { ID = 0, Name = "Argentina" };

            var prevAccumulatedCases = 0;
            var prevAccumulatedDeaths = 0;
            if(filters.From != null) {
                var casesUntilStartDate = _context.Cases.Where(c => c.CaseOpeningDate < filters.From);

                prevAccumulatedCases = casesUntilStartDate.Count();
                prevAccumulatedDeaths = casesUntilStartDate.Where(c => c.Deceased == true).Count();
            }

            var result = FilterCases(filters)
                .OrderBy(c => c.CaseOpeningDate)
                .GroupBy(c => c.CaseOpeningDate)
                .Select(g => new CasesStatsViewModel {
                    Province = currProvince,
                    Timestamp = g.Key,
                    ConfirmedCases = g.Count(),
                    ConfirmedDeaths = g.Where(c => c.Deceased == true).Count(),
                }).ToList();

            if(result.Count >= 0) {
                var casesUntilStartDate = _context.Cases.Where(c => c.CaseOpeningDate < filters.From);

                result[0].AccumulatedDeaths = (filters.From != null ? casesUntilStartDate.Where(c => c.Deceased == true).Count() : 0) + result[0].ConfirmedDeaths;
                result[0].AccumulatedCases = (filters.From != null ? casesUntilStartDate.Count() : 0) + result[0].ConfirmedCases;
                result[0].Mortality = (float)result[0].AccumulatedDeaths / result[0].AccumulatedCases;

                for(int i = 1; i < result.Count; i++) {
                    result[i].AccumulatedCases = result[i - 1].AccumulatedCases + result[i].ConfirmedCases;
                    result[i].AccumulatedDeaths = result[i - 1].AccumulatedDeaths + result[i].ConfirmedDeaths;
                    result[i].Mortality = (float)result[i].AccumulatedDeaths / result[i].AccumulatedCases;
                }
            }

            return Ok(result);
        }

        [HttpGet("stats")]
        public ActionResult<IEnumerable<CasesStatsViewModel>> Stats([FromQuery] CaseFilterViewModel filters) {

            DateTime mostRecentRegisterDate = _context.Cases.OrderByDescending(c => c.CSVLastUpdated).First().CSVLastUpdated;

            CaseFilterViewModel internalFilter = new CaseFilterViewModel {
                ProvinceID = filters.ProvinceID,
                From = new DateTime(mostRecentRegisterDate.Year, mostRecentRegisterDate.Month, mostRecentRegisterDate.Day, 0, 0, 0),
                To = new DateTime(mostRecentRegisterDate.Year, mostRecentRegisterDate.Month, mostRecentRegisterDate.Day, 0, 0, 0).AddDays(1)
            };

            if(filters.ProvinceID != null) {
                return Summary(internalFilter).Value;
            } else {

                List<CasesStatsViewModel> result = new List<CasesStatsViewModel>();

                internalFilter.ProvinceID = null;
                result.Add(Summary(internalFilter).Value[0]);

                foreach(Province province in _context.Provinces.ToList()) {
                    internalFilter.ProvinceID = province.ID;
                    result.Add(Summary(internalFilter).Value[0]);
                }

                return result;

            }
        }

        [HttpGet("count")]
        public ActionResult<int> Count([FromQuery]CaseFilterViewModel filters) {
            var cases = FilterCases(filters);

            return Ok(cases.Count());
        }

        private IQueryable<Case> FilterCases(CaseFilterViewModel filters, IQueryable<Case> cases = null) {
            if (cases == null) cases = _context.Cases;

            if (filters.ProvinceID != null)
                cases = cases.Where(c => c.LoaderProvinceID == filters.ProvinceID);

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
