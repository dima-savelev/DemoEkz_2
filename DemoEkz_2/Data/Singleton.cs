using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEkz_2.Data
{
    public partial class DbEntities : DbContext
    {
        private static DbEntities context;

        public static DbEntities GetContext()
        {
            if (context == null)
            {
                context = new DbEntities();
            }
            return context;
        }
    }
    public partial class Agent
    {
        public string PathToIcon { get; set; }
        public int CountSales { get; set; }
        public int Discount { get; set; }
    }
}
