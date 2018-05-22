using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamAPICore.Models
{
    public class CommonExpense
    {
        public Double Dewlling { get; set; }
        public Double Parking { get; set; }
        public Double Locker { get; set; }
        public Double BulkInternet { get; set; }
        public Double Total => Dewlling + Parking + Locker + BulkInternet;
    }
}
