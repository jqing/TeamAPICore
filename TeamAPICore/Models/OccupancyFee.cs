using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamAPICore.Models
{
    public class OccupancyFee
    {
        public Double SalePrice { get; set; }
        public Double Deposits { get; set; }
        public Double MoneyDue { get; set; }
        public Double MortgageAmount { get; set; }
        public bool NewAct { get; set; }
        public MonthlyOccupancyFee MonthlyOccupancyFees { get; set; }
    }
}
