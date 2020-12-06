using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using COVID_20.Models;
using COVID_20.Persistence;

namespace COVID_20.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CasesController : Controller {
        private readonly AppDbContext _context;

        public CasesController(AppDbContext context) {
            _context = context;
        }

        // GET: Cases
        [HttpGet]
        public ActionResult<IEnumerable<Case>> Get() {
            return Ok(_context.Cases.ToListAsync());
        }
    }
}
