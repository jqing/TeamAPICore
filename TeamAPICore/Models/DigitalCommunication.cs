using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamAPICore.Models
{
    public class DigitalCommunication
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Subject { get; set; }
        public string EmailAddress { get; set; }
        public string URL { get; set; }
    }
}
