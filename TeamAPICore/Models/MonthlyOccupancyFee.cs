using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamAPICore.Models
{
    public class MonthlyOccupancyFee
    {
        public InterestAmount FinancialComponent { get; set; }
        public InterestAmount RealtyTax { get; set; }
        public CommonExpense CommonExpenses { get; set; }
        public Double OtherProductExpenses { get; set; }
        public Double CrossPhaseExpenses { get; set; }
        public Double Total => FinancialComponent.Amount + RealtyTax.Amount + CommonExpenses.Total + OtherProductExpenses + CrossPhaseExpenses;

    }
}
