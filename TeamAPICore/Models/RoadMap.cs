using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamAPICore.Models
{
    public class RoadMap
    {
        public DateTime? OfferAcceptedDate { get; set; }
        public DateTime? ConstructionStartDate { get; set; }
        public DateTime? ElectricalAppointmentDate { get; set; }
        public DateTime? ColourAppointmentDate { get; set; }
        public DateTime? UpgradeAppointmentDate { get; set; }
        public DateTime? RevisedPossessionDate { get; set; }
        public DateTime? MoveInDate { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? FinalClosedDate { get; set; }
        public DateTime? OneYearWarrantyDate { get; set; }
        public DateTime? TwoYearWarrantyDate { get; set; }
        public DateTime? TenYearWarrantyDate { get; set; }
        public DateTime? OriginalTentativePossessionDate { get; set; }
        public bool MortgageApproved { get; set; }
        public int LawyerID { get; set; }
    }
}
