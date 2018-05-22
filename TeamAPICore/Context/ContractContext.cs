using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public async Task<List<Contract>> GetContractsAsync(string CRMId)
        {
            string sql = "sp_GetContractInfoByCRMID";
            List<Contract> contracts = new List<Contract>();
            DataSet dataset = new DataSet();


            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CRMID", CRMId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        // Fill the DataSet using default values for DataTable names, etc
                        da.Fill(dataset);
                    }
                }

                DataTable table = dataset.Tables[0];
                DataTable Purchasers = dataset.Tables[1];
                DataTable Deposits = dataset.Tables[2];
                DataTable Products = dataset.Tables[3];
                foreach (DataRow row in table.Rows)
                    contracts.Add(await GetContractFromDataAsync(row, Purchasers, Deposits, Products, con));

            }
            return contracts;
        }

        public async Task<Contract> GetContractFromDataAsync(DataRow ContractRow,DataTable Purchasers,DataTable Deposits,DataTable Products,SqlConnection con)
        {
            var contractid = Convert.ToString(ContractRow["ContractId"]);
            if (int.TryParse(contractid, out int id))
            {
                Contract contract = new Contract
                {
                    Id = id,
                    FinalClosedDate = Convert.ToDateTime(ContractRow["Final_Closed_Date"]),
                    PossessionDate = Convert.ToDateTime(ContractRow["PossessionDate"]),
                    ConstructionStartDate = Convert.ToDateTime(ContractRow["ConstructionStart"]),
                    RegistrationDate = Convert.ToDateTime(ContractRow["RegistrationDate"]),
                    Project = Convert.ToString(ContractRow["ProjectName"]),
                    ProjectId = Convert.ToString(ContractRow["ProjectID"]),
                    Suite = Convert.ToString(ContractRow["Suite"]),
                    Design = Convert.ToString(ContractRow["Design"]),
                    LegalLevel = Convert.ToString(ContractRow["LegalLevel"]),
                    LegalUnit = Convert.ToString(ContractRow["LegalUnit"]),
                    MunicipalLevel = Convert.ToString(ContractRow["MunicipalLevel"]),
                    MunicipalUnit = Convert.ToString(ContractRow["MunicipalUnit"]),
                    OfferPrice = getDouble(Convert.ToString(ContractRow["OfferPrice"])),
                    APS = "",
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
                    Deposits = new List<Deposit>()
                };

                //OccupancyFee
                var sql = "sp_GetOccupancyFee";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ContractID", contract.Id);
                    var reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        contract.OccupancyFees = new OccupancyFee
                        {
                            NewAct = Convert.ToInt32(reader["NewAct"]) == 1,
                            SalePrice = getDouble(Convert.ToString(reader["SalePrice"])),
                            Deposits = getDouble(Convert.ToString(reader["Deposits"])),
                            MoneyDue = getDouble(Convert.ToString(reader["BalanceDueEscrow"])),
                            MortgageAmount = getDouble(Convert.ToString(reader["UnadjustedBalanceDue"])),
                            MonthlyOccupancyFees = new MonthlyOccupancyFee
                            {
                                CommonExpenses = new CommonExpense
                                {
                                    Dewlling = getDouble(Convert.ToString(reader["DwellingCommonExpenses"])),
                                    Parking = getDouble(Convert.ToString(reader["ParkingCommonExpenses"])),
                                    Locker = getDouble(Convert.ToString(reader["LockerCommonExpenses"])),
                                    BulkInternet = getDouble(Convert.ToString(reader["BulkInternetAmount"])),

                                },
                                CrossPhaseExpenses = getDouble(Convert.ToString(reader["CrossPhaseExpenses"])),
                                OtherProductExpenses = getDouble(Convert.ToString(reader["OtherProductExpenses"])),
                                RealtyTax = new InterestAmount
                                {
                                    Amount = getDouble(Convert.ToString(reader["MonthlyRealtyTaxes"])),
                                    InterestRate = getDouble(Convert.ToString(reader["RealtyTaxRate"]))
                                },
                                FinancialComponent = new InterestAmount
                                {
                                    Amount = getDouble(Convert.ToString(reader["MonthlyInterimWebInterest"])),
                                    InterestRate = getDouble(Convert.ToString(reader["WebInterimOccupancyInterestRate"]))
                                }

                            }
                        };
                    }
                }

                //Purchasers
                foreach (DataRow row in FilterDataTable(contractid, Purchasers))
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

                //Deposits
                foreach (DataRow row in FilterDataTable(contractid, Deposits))
                {

                    Deposit deposit = new Deposit
                    {
                        CheckNumber = Convert.ToString(row["ChequeNumber"]),
                        Amount = getDouble(Convert.ToString(row["Amount"])),
                        Status = Convert.ToString(row["ChequeStatus"]),
                        DueDate = Convert.ToString(row["DateDue"]),
                        ReceivedDate = Convert.ToDateTime(row["Received"]),
                        Type = Convert.ToString(row["DepositType"])
                    };
                    contract.Deposits.Add(deposit);
                }

                //Products
                foreach (DataRow row in FilterDataTable(contractid, Products))
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

                return contract;
            }
            else
            {
                return new Contract();
            }
        }

        private DataRow[] FilterDataTable(string contractid,DataTable table)
        {
            return table.Select("ContractId = " + contractid);
        }

        public async Task<Contract> GetContractAsync(int contractId,string CRMId)
        {
            string sql = "sp_GetContractInfoByContractID";
            List<Contract> contracts = new List<Contract>();
            DataSet dataset = new DataSet();


            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ContractID", contractId);
                    cmd.Parameters.AddWithValue("@CRMID", CRMId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        // Fill the DataSet using default values for DataTable names, etc
                        da.Fill(dataset);
                    }
                }

                DataTable table = dataset.Tables[0];
                DataTable Purchasers = dataset.Tables[1];
                DataTable Deposits = dataset.Tables[2];
                DataTable Products = dataset.Tables[3];
                if (table.Rows.Count == 1)
                    return await GetContractFromDataAsync(table.Rows[0], Purchasers, Deposits, Products, con);
                else
                    return new Contract();
            }
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
    }
}
