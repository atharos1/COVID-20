using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COVID_20.Persistence {
    public class DbInitializer {
        public static void Initialize(AppDbContext context) {
            context.Database.EnsureCreated();

            //Initialize database date on creation

            context.SaveChanges();
        }
    }
}
