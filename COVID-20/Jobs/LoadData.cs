using COVID_20.Models;
using COVID_20.Persistence;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace COVID_20.Jobs {
    public static class LoadData {
        public static async Task LoadDataMethod(AppDbContext context) {
            static bool SpanishStringToBool(string str) => str == "SI";

            ISet<Province> provinceSet = new HashSet<Province>();
            ISet<District> districtSet = new HashSet<District>();
            IList<Case> cases = new List<Case>();

            var maxID = 0; //context.Cases.DefaultIfEmpty().Max(r => r == null ? 0 : r.ID);


            var pageToGet = $"https://sisa.msal.gov.ar/datos/descargas/covid-19/files/Covid19Casos.csv";

            using (var client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(pageToGet))
            using (HttpContent content = response.Content)
            using (var stream = (MemoryStream)await content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture)) {
                csv.Read();
                csv.ReadHeader();

                //var maxID = context.Cases.DefaultIfEmpty().Max(r => r == null ? 0 : r.ID);

                while (csv.Read()) {
                    if (csv.GetField<int>("id_evento_caso") <= maxID)
                        continue;

                    Province ResidenceProvince = new Province {
                        ID = csv.GetField<int>("residencia_provincia_id"),
                        Name = csv.GetField<string>("residencia_provincia_nombre")
                    };
                    provinceSet.Add(ResidenceProvince);

                    District ResidenceDistrict = new District {
                        ID = csv.GetField<int>("residencia_departamento_id"),
                        Name = csv.GetField<string>("residencia_departamento_nombre")
                    };
                    districtSet.Add(ResidenceDistrict);

                    Province LoaderProvince = new Province {
                        ID = csv.GetField<int>("carga_provincia_id"),
                        Name = csv.GetField<string>("carga_provincia_nombre")
                    };
                    provinceSet.Add(LoaderProvince);

                    Case c = new Case {
                        ID = csv.GetField<int>("id_evento_caso"),
                        Sex = csv.GetField<string>("sexo"),
                        Age = csv.GetField<int?>("edad"),
                        ResidenceCountryName = csv.GetField<string>("residencia_pais_nombre"),
                        ResidenceProvince = ResidenceProvince,
                        ResidenceDistrict = ResidenceDistrict,
                        LoaderProvince = LoaderProvince,
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
                }

                //await using var transaction = await context.Database.BeginTransactionAsync();

                using (var transaction = await context.Database.BeginTransactionAsync()) {

                    foreach (var province in provinceSet) {
                        context.Add(province);
                    }

                    foreach (var distict in districtSet) {
                        context.Add(distict);
                    }

                    foreach (var c in cases) {
                        context.Add(c);
                    }

                    transaction.Commit();
                }

            }

            Console.WriteLine("jajas");
        }
    }
}
