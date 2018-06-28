using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeamAPICore.Models;
using TeamAPICore.Settings;

namespace TeamAPICore.Context
{
    public class ContractContext : BaseContext
    {

        public ContractContext(IOptions<ApplicationOption> option) : base(option)
        {
        }

        public Contract GetContractFromData(DataSet dataset)
        {
            DataTable ContractInfo = null;
            DataTable OccupancyFee = null;
            DataTable Purchasers = null;
            DataTable Deposits = null;
            DataTable Products = null;
            DataTable Documents = null;
            DataTable Correspondences = null;
            DataTable Communications = null;
            DataTable Deficiency = null;
            DataTable RoadMap = null;

            if (dataset.Tables.Count >= 1)
                ContractInfo = dataset.Tables[0];
            if (dataset.Tables.Count >= 2)
                OccupancyFee = dataset.Tables[1];
            if (dataset.Tables.Count >= 3)
                Purchasers = dataset.Tables[2];
            if (dataset.Tables.Count >= 4)
                Deposits = dataset.Tables[3];
            if (dataset.Tables.Count >= 5)
                Products = dataset.Tables[4];
            if (dataset.Tables.Count >= 6)
                Documents = dataset.Tables[5];
            if (dataset.Tables.Count >= 7)
                Correspondences = dataset.Tables[6];
            if (dataset.Tables.Count >= 8)
                Communications = dataset.Tables[7];
            if (dataset.Tables.Count >= 9)
                Deficiency = dataset.Tables[8];
            if (dataset.Tables.Count >= 10)
                RoadMap = dataset.Tables[9];

            Contract contract = null;
            if (ContractInfo != null && ContractInfo.Rows.Count == 1)
            {
                var ContractRow = ContractInfo.Rows[0];
                var contractid = Convert.ToString(ContractRow["ContractId"]);
                if (int.TryParse(contractid, out int id))
                {
                    contract = new Contract
                    {
                        Id = id,
                        Personid = Convert.ToInt32(ContractRow["PersonId"]),
                        FinalClosedDate = GetDate(Convert.ToString(ContractRow["Final_Closed_Date"])),
                        PossessionDate = GetDate(Convert.ToString(ContractRow["PossessionDate"])),
                        ConstructionStartDate = GetDate(Convert.ToString(ContractRow["ConstructionStart"])),
                        RegistrationDate = GetDate(Convert.ToString(ContractRow["RegistrationDate"])),
                        Project = Convert.ToString(ContractRow["ProjectName"]),
                        ProjectId = Convert.ToString(ContractRow["ProjectID"]),
                        Suite = Convert.ToString(ContractRow["Suite"]),
                        Design = Convert.ToString(ContractRow["Design"]),
                        LegalLevel = Convert.ToString(ContractRow["LegalLevel"]),
                        LegalUnit = Convert.ToString(ContractRow["LegalUnit"]),
                        MunicipalLevel = Convert.ToString(ContractRow["MunicipalLevel"]),
                        MunicipalUnit = Convert.ToString(ContractRow["MunicipalUnit"]),
                        OfferPrice = GetDouble(Convert.ToString(ContractRow["OfferPrice"])),
                        APS = new List<PersistedDocument>(),
                        FloorPlan = "",
                        Locker = new List<string>(),
                        ParkingSpace = new List<string>(),
                        BicycleLocker = new List<string>(),
                        Storage = new List<string>(),
                        Status = "",
                        GSTRebate = Convert.ToInt32(ContractRow["GSTRebate"]) == 1,
                        Solicitor = new Solicitor
                        {
                            Name = Convert.ToString(ContractRow["SolicitorName"]),
                            Address = new Address
                            {
                                Suite = Convert.ToString(ContractRow["SolicitorSuite"]),
                                StreetAddress = Convert.ToString(ContractRow["SolicitorAddress"]),
                                City = Convert.ToString(ContractRow["SolicitorCity"]),
                                Province = Convert.ToString(ContractRow["SolicitorProvince"]),
                                PostalCode = Convert.ToString(ContractRow["SolicitorPostal"]),
                                Country = Convert.ToString(ContractRow["SolicitorCountry"])
                            },
                            Fax = Convert.ToString(ContractRow["SolicitorFax"]),
                            Firm = Convert.ToString(ContractRow["SolicitorCompany"]),
                            Phone = Convert.ToString(ContractRow["SolicitorBusiness"])
                        },
                        Purchasers = new List<Purchaser>(),
                        Deposits = new List<Deposit>(),
                        Documents = new List<PersistedDocument>(),
                        Communications = new List<DigitalCommunication>(),
                        Correspondences = new List<PersistedDocument>(),
                        Deficiencies = new List<Deficiency>()
                    };

                    //OccupancyFee
                    if (OccupancyFee != null && OccupancyFee.Rows.Count == 1)
                    {
                        var occupancyRow = OccupancyFee.Rows[0];
                        contract.OccupancyFees = new OccupancyFee
                        {
                            NewAct = Convert.ToInt32(occupancyRow["NewAct"]) == 1,
                            SalePrice = GetDouble(Convert.ToString(occupancyRow["SalePrice"])),
                            Deposits = GetDouble(Convert.ToString(occupancyRow["Deposits"])),
                            MoneyDue = GetDouble(Convert.ToString(occupancyRow["BalanceDueEscrow"])),
                            MortgageAmount = GetDouble(Convert.ToString(occupancyRow["UnadjustedBalanceDue"])),
                            MonthlyOccupancyFees = new MonthlyOccupancyFee
                            {
                                CommonExpenses = new CommonExpense
                                {
                                    Dewlling = GetDouble(Convert.ToString(occupancyRow["DwellingCommonExpenses"])),
                                    Parking = GetDouble(Convert.ToString(occupancyRow["ParkingCommonExpenses"])),
                                    Locker = GetDouble(Convert.ToString(occupancyRow["LockerCommonExpenses"])),
                                    BulkInternet = GetDouble(Convert.ToString(occupancyRow["BulkInternetAmount"])),

                                },
                                CrossPhaseExpenses = GetDouble(Convert.ToString(occupancyRow["CrossPhaseExpenses"])),
                                OtherProductExpenses = GetDouble(Convert.ToString(occupancyRow["OtherProductExpenses"])),
                                RealtyTax = new InterestAmount
                                {
                                    Amount = GetDouble(Convert.ToString(occupancyRow["MonthlyRealtyTaxes"])),
                                    InterestRate = GetDouble(Convert.ToString(occupancyRow["RealtyTaxRate"]))
                                },
                                FinancialComponent = new InterestAmount
                                {
                                    Amount = GetDouble(Convert.ToString(occupancyRow["MonthlyInterimWebInterest"])),
                                    InterestRate = GetDouble(Convert.ToString(occupancyRow["WebInterimOccupancyInterestRate"]))
                                }

                            }
                        };

                    }

                    //Purchasers
                    if (Purchasers != null)
                    {
                        foreach (DataRow row in Purchasers.Rows)
                        {

                            var addressForService = Convert.ToInt32(row["AddressForService"]);

                            Address address = new Address
                            {
                                Suite = Convert.ToString(row["Suite"]),
                                StreetAddress = Convert.ToString(row["Address"]),
                                City = Convert.ToString(row["City"]),
                                Province = Convert.ToString(row["Province"]),
                                PostalCode = Convert.ToString(row["PostalCode"]),
                                Country = Convert.ToString(row["Country"])
                            };
                            Purchaser purchaser = new Purchaser
                            {
                                Address = address,
                                Email = Convert.ToString(row["EmailAddress"]),
                                Name = Convert.ToString(row["Name"]),
                                Phone = Convert.ToString(row["HomePhone"])
                            };
                            contract.Purchasers.Add(purchaser);
                            if (addressForService == 1)
                            {
                                contract.ServiceAddress = new ServiceAddress
                                {
                                    Address = address,
                                    Phone = purchaser.Phone,
                                    Email = purchaser.Email

                                };

                            }

                        }
                    }

                    //Deposits
                    if (Deposits != null)
                    {
                        foreach (DataRow row in Deposits.Rows)
                        {

                            Deposit deposit = new Deposit
                            {
                                CheckNumber = Convert.ToString(row["ChequeNumber"]),
                                Amount = GetDouble(Convert.ToString(row["Amount"])),
                                Status = Convert.ToString(row["ChequeStatus"]),
                                DueDate = Convert.ToString(row["DateDue"]),
                                ReceivedDate = Convert.ToDateTime(row["Received"]),
                                Type = Convert.ToString(row["DepositType"])
                            };
                            contract.Deposits.Add(deposit);
                        }
                    }

                    //Products
                    if (Products != null)
                    {
                        foreach (DataRow row in Products.Rows)
                        {

                            string p = Convert.ToString(row["ProductName"]);
                            string name = Convert.ToString(row["Suite"]);
                            if (name.ToUpper() == "TBA")
                                name = "to be assigned";
                            switch (p.ToUpper())
                            {
                                case "PARKING":
                                    contract.ParkingSpace.Add(name);
                                    break;
                                case "LOCKER":
                                    contract.Locker.Add(name);
                                    break;
                                case "BICYCLE":
                                    contract.BicycleLocker.Add(name);
                                    break;
                                case "STORAGE":
                                    contract.Storage.Add(name);
                                    break;
                                default:
                                    break;
                            }

                        }
                    }

                    //Documents
                    if (Documents != null)
                    {
                        foreach (DataRow row in Documents.Rows)
                        {
                            var d = new PersistedDocument
                            {
                                Id = Convert.ToInt32(row["PersistedDocumentId"]),
                                Description = Convert.ToString(row["PresentationName"]),
                                FileName = Path.GetFileNameWithoutExtension(Convert.ToString(row["TemplateName"])),
                                Extension = Convert.ToString(row["FileExtension"]),
                                Date = Convert.ToDateTime(row["ActivityDate"]),
                                Hash = Convert.ToString(row["FileHash"])
                            };
                            var isAPS = Convert.ToBoolean(row["isAPS"]);
                            var isTarion = Convert.ToBoolean(row["isTarion"]);
                            if (isAPS)
                                contract.APS.Add(d);
                            contract.Documents.Add(d);
                        }
                    }

                    //Correspondences
                    if (Correspondences != null)
                    {
                        foreach (DataRow row in Correspondences.Rows)
                        {
                            var d = new PersistedDocument
                            {
                                Id = Convert.ToInt32(row["PersistedDocumentId"]),
                                Description = Convert.ToString(row["CommentText"]),
                                FileName = Path.GetFileNameWithoutExtension(Convert.ToString(row["FileName"])),
                                Extension = Convert.ToString(row["FileExtension"]),
                                Date = Convert.ToDateTime(row["Timestamp"]),
                                Hash = Convert.ToString(row["FileHash"])
                            };
                            contract.Correspondences.Add(d);
                        }
                    }

                    //Communications
                    if (Communications != null)
                    {
                        foreach (DataRow row in Communications.Rows)
                        {
                            var d = new DigitalCommunication
                            {
                                Id = Convert.ToInt32(row["id"]),
                                Subject = Convert.ToString(row["Subject"]),
                                EmailAddress = Convert.ToString(row["EmailAddress"]),
                                Date = Convert.ToDateTime(row["DateSent"])
                            };
                            contract.Communications.Add(d);
                        }
                    }

                    //Deficiencies
                    if (Deficiency != null)
                    {
                        foreach (DataRow row in Deficiency.Rows)
                        {
                            var d = new Deficiency
                            {
                                Id = Convert.ToInt32(row["id"]),
                                Description = Convert.ToString(row["Description"]),
                                Status = Convert.ToString(row["Status"]),
                                Date = Convert.ToDateTime(row["Inspection"]),
                                PermissionToEnter = Convert.ToBoolean(row["PermissionToEnter"])
                            };
                            contract.Deficiencies.Add(d);
                        }
                    }
                    //RoadMap
                    if(RoadMap != null && RoadMap.Rows.Count == 1)
                    {
                        DataRow row = RoadMap.Rows[0];
                        contract.RoadMap = new RoadMap
                        {
                            ColourAppointmentDate = GetDate(Convert.ToString(row["ColourAppointment"])),
                            ConstructionStartDate = GetDate(Convert.ToString(row["ConstructionStartDate"])),
                            ElectricalAppointmentDate = GetDate(Convert.ToString(row["ElectricalAppointment"])),
                            FinalClosedDate = GetDate(Convert.ToString(row["FinalClosedDate"])),
                            LawyerID = Convert.ToInt32(row["LawyerID"]),
                            MortgageApproved = Convert.ToBoolean(row["MortgageApproved"]),
                            MoveInDate = GetDate(Convert.ToString(row["MoveInDate"])),
                            OfferAcceptedDate = GetDate(Convert.ToString(row["OfferAcceptedDate"])),
                            OneYearWarrantyDate = GetDate(Convert.ToString(row["OneYearWarranty"])),
                            OriginalTentativePossessionDate = GetDate(Convert.ToString(row["OriginalTentativePossession"])),
                            RegistrationDate = GetDate(Convert.ToString(row["RegistrationDate"])),
                            RevisedPossessionDate = GetDate(Convert.ToString(row["RevisedPossessionDate"])),
                            TenYearWarrantyDate = GetDate(Convert.ToString(row["TenYearWarranty"])),
                            TwoYearWarrantyDate = GetDate(Convert.ToString(row["TwoYearWarranty"])),
                            UpgradeAppointmentDate = GetDate(Convert.ToString(row["UpgradeAppointment"]))
                        };
                    }

                }

            }
            return contract;

        }


        private DataRow[] FilterDataTable(string contractid,DataTable table)
        {
            return table.Select("ContractId = " + contractid);
        }

        public Contract GetContract(int contractId, string CRMId)
        {
            string sql = "sp_GetContractInfoByID";
            List<Contract> contracts = new List<Contract>();
            DataSet dataset = new DataSet();


            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CRMID", CRMId);
                    cmd.Parameters.AddWithValue("@ContractID", contractId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        // Fill the DataSet using default values for DataTable names, etc
                        da.Fill(dataset);
                    }
                }
            }
            return GetContractFromData(dataset);
        }

        public async Task<List<ContractSmall>> GetContractListAsync(string CRMId)
        {
            string sql = @"Select c.ContractID,cp.Suite,cp.Design,pr.Name,pr.ProjectID
From Contracts c, Projects pr, Contract_Product cp
 Where c.ContractID = cp.ContractID And c.ProjectID = pr.ProjectID
AND c.Contract_Canceled = 0 And cp.ProductID in (select dscid from prj_feature_dsc where reported = 1)
And c.ContractID in (select pu.contractid
from Contract_Purchaser pu
join Contracts c on c.contractid = pu.contractid and c.contract_canceled = 0
join Person prs on prs.personid = pu.personid and isnull(prs.CRMContactID,'') = @CRMID )";
            List<ContractSmall> contracts = new List<ContractSmall>();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@CRMID", CRMId);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var contractid = Convert.ToString(reader["ContractID"]);
                        if (int.TryParse(contractid, out int id))
                        {

                            contracts.Add(new ContractSmall
                            {
                                Id = id,
                                Project = Convert.ToString(reader["Name"]),
                                ProjectId = Convert.ToString(reader["ProjectID"]),
                                Suite = Convert.ToString(reader["Suite"]),
                                Design = Convert.ToString(reader["Design"]),
                            });
                        }
                    }
                }
            }
            return contracts;
        }

        public async Task<int> GetPersonIdAsync(string CRMId)
        {
            string sql = "Select personid FROM Person Where isnull(CRMContactID,'') = @CRMID";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@CRMID", CRMId);
                    var theId = await cmd.ExecuteScalarAsync();
                    if (int.TryParse(Convert.ToString(theId), out int personId))
                        return personId;
                }
            }
            return 0;
        }

        public async Task<string> GetFileHashAsync(int id)
        {
            string sql = "Select filehash FROM DE_PersistedDocument Where PersistedDocumentId = @id";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", id);
                    return Convert.ToString(await cmd.ExecuteScalarAsync());
                }
            }
        }
        public async Task<string> GetEmailBodyAsync(int id)
        {
            string sql = "Select message FROM DC_Sendgrid Where SendGridID = @id";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", id);
                    return Convert.ToString(await cmd.ExecuteScalarAsync());
                }
            }
        }

    }
}
