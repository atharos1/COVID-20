using COVID_20.Models;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace COVID_20.Persistence {
    public class DbInitializer {
        public static void Initialize(AppDbContext context) {
            context.Database.EnsureCreated();

            if (context.Cases.Any())
                return;

            //Initialize database date on creation

            context.SaveChanges();
        }
    }
}
