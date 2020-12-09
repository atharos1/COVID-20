using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COVID_20.Models;
using COVID_20.Persistence;

namespace COVID_20.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProvincesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProvincesController(AppDbContext context) {
            _context = context;
        }

        // GET: api/Provinces
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Province>>> GetProvinces() {
            return await _context.Provinces.ToListAsync();
        }
    }
}
