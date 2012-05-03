using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

//http://blogs.msdn.com/b/adonet/archive/2011/03/15/ef-4-1-code-first-walkthrough.aspx
//http://msdn.microsoft.com/en-us/data/gg715119
namespace ClassLibraryDatabase
{
    [Serializable]
    public class KigangContext : DbContext
    {
        public DbSet<Therapie> Therapien { get; set; }
        public DbSet<Therapeut> Therapeuten { get; set; }
        public DbSet<Patient> Patients { get; set; }

    }
}
