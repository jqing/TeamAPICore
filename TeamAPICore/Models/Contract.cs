using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamAPICore.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public string Suite { get; set; }
        public string Project { get; set; }
        public string ProjectId { get; set; }
        public string Design { get; set; }
        public string Status { get; set; }
        public bool GSTRebate { get; set; }
        public string LegalLevel { get; set; }
        public string LegalUnit { get; set; }
        public string MunicipalLevel { get; set; }
        public string MunicipalUnit {get;set;}
        public List<string> BicycleLocker { get; set; }
        public List<string> Storage { get; set; }
        public List<string> ParkingSpace { get; set; }
        public List<string> Locker { get; set; }
        public double OfferPrice { get; set; }
        public DateTime ConstructionStartDate { get; set; }
        public DateTime FinalClosedDate { get; set; }
        public DateTime PossessionDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public ServiceAddress ServiceAddress { get; set; }
        public List<Purchaser> Purchasers { get; set; }
        public Solicitor Solicitor { get; set; }
        public OccupancyFee OccupancyFees { get; set; }
        public List<Deposit> Deposits { get; set; }
        public string FloorPlan { get; set; }
        public string APS { get; set; }

    }
}
