using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamAPICore.Models
{
    public class Deposit
    {
        public Double Amount { get; set; }
        public string DueDate { get; set; }
        public string CheckNumber { get; set; }
        public string Status { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string Type { get; set; }
    }
}
