using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamAPICore.Models
{
    public class Deficiency
    {
        public int Id { get; set; }
        public bool PermissionToEnter { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}
