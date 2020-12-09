using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COVID_20.Models;
using COVID_20.Persistence;
using System.Net.Http;
using System.IO;
using CsvHelper;
using System.Globalization;
using Npgsql.Bulk;

namespace COVID_20.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConfigurationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("lastUpdate")]
        public ActionResult<DateTime> LastUpdated() {
            return _context.Cases.OrderByDescending(c => c.ID).Last().CSVLastUpdated;
        }


        [HttpPost("loadNewCasesFromExternalSource")]
        public async Task<ActionResult<string>> UpdateFromDatabaseAsync() {
            static bool SpanishStringToBool(string str) => str == "SI";

            Dictionary<int, Province> provinceMap = new Dictionary<int, Province>();
            Dictionary<int, District> districtMap = new Dictionary<int, District>();

            List<Case> cases = new List<Case>();
            var uploader = new NpgsqlBulkUploader(_context);

            foreach (var province in _context.Provinces.ToList()) {
                provinceMap.Add(province.CsvID, province);
            }

            foreach (var district in _context.Districts.ToList()) {
                districtMap.Add(district.CsvID, district);
            }

            int currCount = 0;

            int maxCsvID = _context.Cases.Any() ? _context.Cases.Max(r => r.CsvID) : 0;

            var pageToGet = "https://sisa.msal.gov.ar/datos/descargas/covid-19/files/Covid19Casos.csv";

            using (var client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(pageToGet))
            using (HttpContent content = response.Content)
            using (var stream = (MemoryStream)await content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture)) {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read()) {
                    if (csv.GetField<int>("id_evento_caso") <= maxCsvID)
                        continue;

                    if (!provinceMap.ContainsKey(csv.GetField<int>("residencia_provincia_id"))) {
                        var n = new Province {
                            CsvID = csv.GetField<int>("residencia_provincia_id"),
                            Name = csv.GetField<string>("residencia_provincia_nombre")
                        };

                        _context.Provinces.Add(n);
                        _context.SaveChanges();

                        provinceMap.Add(n.CsvID, n);
                    }

                    if (!provinceMap.ContainsKey(csv.GetField<int>("carga_provincia_id"))) {
                        var n = new Province {
                            CsvID = csv.GetField<int>("carga_provincia_id"),
                            Name = csv.GetField<string>("carga_provincia_nombre")
                        };

                        _context.Provinces.Add(n);
                        _context.SaveChanges();

                        provinceMap.Add(n.CsvID, n);
                    }

                    if (!districtMap.ContainsKey(csv.GetField<int>("residencia_departamento_id"))) {
                        var n = new District {
                            CsvID = csv.GetField<int>("residencia_departamento_id"),
                            Name = csv.GetField<string>("residencia_departamento_nombre")
                        };

                        _context.Districts.Add(n);
                        _context.SaveChanges();

                        districtMap.Add(n.CsvID, n);
                    }

                    Case c = new Case {
                        CsvID = csv.GetField<int>("id_evento_caso"),
                        Sex = csv.GetField<string>("sexo"),
                        Age = csv.GetField<int?>("edad"),
                        ResidenceCountryName = csv.GetField<string>("residencia_pais_nombre"),
                        ResidenceProvinceID = provinceMap[csv.GetField<int>("residencia_provincia_id")].ID,
                        ResidenceDistrictID = districtMap[csv.GetField<int>("residencia_departamento_id")].ID,
                        LoaderProvinceID = provinceMap[csv.GetField<int>("carga_provincia_id")].ID,
                        SymptomsStartDate = csv.GetField<DateTime?>("fecha_inicio_sintomas"),
                        DiagnoseDate = csv.GetField<DateTime?>("fecha_diagnostico"),
                        CaseOpeningDate = csv.GetField<DateTime?>("fecha_apertura"),
                        SepiOpening = csv.GetField<int?>("sepi_apertura"),
                        AdmissionDate = csv.GetField<DateTime?>("fecha_internacion"),
                        IntensiveCare = SpanishStringToBool(csv.GetField<string>("cuidado_intensivo")),
                        IntensiveCareDate = csv.GetField<DateTime?>("fecha_cui_intensivo"),
                        Deceased = SpanishStringToBool(csv.GetField<string>("fallecido")),
                        DeceasedDate = csv.GetField<DateTime?>("fecha_fallecimiento"),
                        Respirator = SpanishStringToBool(csv.GetField<string>("asistencia_respiratoria_mecanica")),
                        PublicFounding = (csv.GetField<string>("origen_financiamiento") != "Privado"),
                        Classification = csv.GetField<string>("clasificacion_resumen"),
                        ClassificationDetail = csv.GetField<string>("clasificacion_resumen"),
                        CSVLastUpdated = csv.GetField<DateTime>("ultima_actualizacion")
                    };

                    cases.Add(c);

                    currCount++;

                    if (currCount % 100000 == 0) {
                        uploader.Insert(cases);

                        cases = new List<Case>();

                        _context.SaveChanges();
                    }
                }

                uploader.Insert(cases);
                _context.SaveChanges();

                return Ok($"Loaded {currCount} new cases");
            }
        }
    }
}
