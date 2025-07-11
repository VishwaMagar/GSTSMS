using GSTSMSHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace GSTSMSLibrary.AccountManager
{
    public class BALAccountManager

    {

        MSSQL db = new MSSQL();


        #region********************************************************************* Dashboard ***********************************************************



        public async Task<decimal> GetDueMaintenanceAmount()
        {
            decimal maintenanceDue = 0;
            var param = new Dictionary<string, string> { { "@Flag", "DueMaintenance" } };
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);
            if (dt.Rows.Count > 0)
            {
                maintenanceDue = Convert.ToDecimal(dt.Rows[0][0]);
            }
            return maintenanceDue;
        }

        public async Task<decimal> CashinHand()
        {
            decimal cashInHand = 0;
            var param = new Dictionary<string, string> { { "@Flag", "CashinHand" } };
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);
            if (dt.Rows.Count > 0)
            {
                cashInHand = Convert.ToDecimal(dt.Rows[0][0]);
            }
            return cashInHand;
        }

        public async Task<decimal> TotalBankBalance()
        {
            decimal totalBankBalance = 0;
            var param = new Dictionary<string, string> { { "@Flag", "TotalBalance" } };
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);
            if (dt.Rows.Count > 0)
            {
                totalBankBalance = Convert.ToDecimal(dt.Rows[0][0]);
            }
            return totalBankBalance;
        }

        public async Task<int> TotalComplaints()
        {
            var param = new Dictionary<string, string> { { "@Flag", "TotalComplaints" } };
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0][0]) : 0;
        }

        public async Task<int> PendingComplaints()
        {
            var param = new Dictionary<string, string> { { "@Flag", "PendingComplaints" } };
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0][0]) : 0;
        }

        public async Task<int> SolvedComplaints()
        {
            var param = new Dictionary<string, string> { { "@Flag", "SolvedComplaints" } };
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0][0]) : 0;
        }

        public async Task<DataSet> BankBalance()
        {
            var param = new Dictionary<string, string>
    {
        { "@Flag", "BankBalance" }
    };

            MSSQL db = new MSSQL();
            DataSet ds = await db.ExecuteStoreProcedureReturnDS("sp_SMS", param);

            return ds;
        }


        public async Task<DataTable> GetEventExpensesChartData(int month)
        {
            var param = new Dictionary<string, string>
        {
            {"@Flag", "EventExpens"},
            {"@Month", month.ToString()}
        };
            return await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);
        }

        public async Task<List<Dictionary<string, string>>> GetTop5RedListMembers()
        {
            var members = new List<Dictionary<string, string>>();
            var param = new Dictionary<string, string> { { "@Flag", "RedListMember" } };
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);

            foreach (DataRow row in dt.Rows)
            {
                members.Add(new Dictionary<string, string>
            {
                {"Name", row["FullName"].ToString()},
                {"Wing", row["WingName"].ToString()},
                {"FlatNo", row["FlatNo"].ToString()},
                {"MonthsPending", row["MaintenancePending"].ToString()},
                {"AmountPending", string.Format("₹ {0:N2}", row["MaintenanceDuePending"])}
            });
            }
            return members;
        }

        public async Task<Dictionary<string, decimal>> GetMonthlyTransactionSummary(int month)
        {
            var summary = new Dictionary<string, decimal>();
            var param = new Dictionary<string, string>
        {
            {"@Flag", "TotalCreditedAndDebited"},
            {"@Month", month.ToString()}
        };
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);
            if (dt.Rows.Count > 0)
            {
                summary["Credited"] = Convert.ToDecimal(dt.Rows[0]["TotalCreditedCash"]);
                summary["Debited"] = Convert.ToDecimal(dt.Rows[0]["TotalDebitedCash"]);
                summary["Balance"] = Convert.ToDecimal(dt.Rows[0]["TotalOpeningBalance"]);
            }
            else
            {
                summary["Credited"] = 0;
                summary["Debited"] = 0;
                summary["Balance"] = 0;
            }
            return summary;
        }

        public async Task<Dictionary<string, int>> GetWorkerPaymentChartAsync(int month)
        {
            var chartData = new Dictionary<string, int>();
            var param = new Dictionary<string, string>
        {
            {"@Flag", "WorkerPayment"},
            {"@Month", month.ToString()}
        };
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);
            chartData["Completed"] = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["Completed"]) : 0;
            chartData["Pending"] = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["Pending"]) : 0;
            return chartData;
        }

        public async Task<Dictionary<int, string>> GetWingListAsync()
        {
            var result = new Dictionary<int, string>();
            var param = new Dictionary<string, string> { { "@Flag", "TotalWing" } };
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);
            foreach (DataRow row in dt.Rows)
            {
                result[Convert.ToInt32(row["WingId"])] = row["WingName"].ToString();
            }
            return result;
        }

        public async Task<Dictionary<string, decimal>> GetMaintenanceStatusChartAsync(int month, string wingId)
        {
            var result = new Dictionary<string, decimal>();
            int parsedWingId = 0;
            if (!string.IsNullOrEmpty(wingId) && wingId.ToLower() != "all")
            {
                int.TryParse(wingId, out parsedWingId);
            }
            var param = new Dictionary<string, string>
        {
            {"@Month", month.ToString()},
            {"@WingId", parsedWingId.ToString()},
            {"@Flag", "MaintenanceStatus"}
        };
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                result["Paid"] = Convert.ToDecimal(row["PaidPercentage"]);
                result["Unpaid"] = Convert.ToDecimal(row["UnpaidPercentage"]);
                result["TotalAmount"] = Convert.ToDecimal(row["TotalAmount"]);
                result["PendingAmount"] = Convert.ToDecimal(row["PendingAmount"]);
            }
            return result;
        }


        #endregion




        #region********************************************************************* Society Ac.Details Bank ***********************************************************



        // <summary>
        /// This method fetches bank account details from the database, adds bank name/branch using IFSC code, and returns the full list. 
        /// with flag 'FetchBankDetails'. For each record, it retrieves additional
        /// bank information (Bank Name and Branch) using the IFSC code via external method.
        /// </summary>

        /// <returns>
        /// A list of AccountManager objects containing complete bank details:
        /// BankId, AccountNo, OpeningBalance, AddedDate, BankName, Branch, BankHolderName.
        /// </returns>
        public async Task<List<AccountManager>> GetAllBankDetailsSS()
        {
            try
            {
                MSSQL db = new MSSQL();
                var parameters = new Dictionary<string, string>
        {
            { "@Flag", "FetchBankDetailsSS" }
        };

                DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);
                List<AccountManager> list = new List<AccountManager>();

                foreach (DataRow row in dt.Rows)
                {
                    string ifsc = row["IFSCCode"].ToString();
                    var bankInfo = await GetBankDetailsFromIfscSS(ifsc);

                    list.Add(new AccountManager
                    {
                        BankCode = row["BankCode"].ToString(),
                        IFSCCode = ifsc,
                        BankId = Convert.ToInt32(row["BankId"]),
                        AccountNo = row["BankAccountNumber"].ToString(),
                        OpeningBalance = row.Field<decimal?>("Balance") ?? 0,
                        AddedDate = row.Field<DateTime?>("Date") ?? DateTime.MinValue,
                        BankName = bankInfo?.Bank ?? "N/A",
                        Branch = bankInfo?.Branch ?? "N/A",
                        BankHolderName = row["BankHolderName"].ToString()
                    });
                }

                return list;
            }
            catch (Exception)
            {
                // Optionally log the error
                return new List<AccountManager>();
            }
        }
        /// <summary>
        /// Disables a bank account in the database by setting its status to inactive,
        /// using the flag 'DisableBankSS' in the stored procedure.
        /// </summary>
        ///
        /// <param name="bankId">
        /// The unique identifier of the bank account to be disabled.
        /// </param>
        ///
        /// <returns>
        /// Returns true if the account was successfully disabled (rows affected > 0),
        /// otherwise returns false.
        /// </returns>

        public async Task<bool> DisableBankAccountAsyncSS(int bankId)
        {
            MSSQL db = new MSSQL();
            var parameters = new Dictionary<string, string>
    {
        { "@Flag", "DisableBankSS" },
        { "@BankId", bankId.ToString() }
    };

            int rowsAffected = await db.ExecuteStoreProcedureReturnInt("sp_SMS", parameters);
            return rowsAffected > 0;
        }


        /// <summary>
        /// Show transaction list to each bank through the transaction
        /// 03/07/2025
        /// MS
        /// </summary>
        /// <param name="bankCode"></param>
        /// <returns>when click on bankdetails list on view icon open the transaction list</returns>
        public async Task<List<AccountManager>> GetTransactionStatementByBankMS(string BankCode)
        {
            try
            {
                MSSQL db = new MSSQL();
                var parameters = new Dictionary<string, string>
        {
            { "@Flag", "FetchTransactionStatementMS" },
            { "@BankCode", BankCode }
        };

                DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);
                List<AccountManager> list = new List<AccountManager>();

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new AccountManager
                    {
                        SrNo = Convert.ToInt32(row["SrNo"]),
                        PaymentByName = row["SenderReceiver"].ToString(),
                        PaidDate = Convert.ToDateTime(row["PaidDate"]),
                        Amount = Convert.ToDecimal(row["Amount"]),
                        Status = row["Status"].ToString()
                    });
                }

                return list;
            }
            catch (Exception)
            {
                // Optionally log the error
                return new List<AccountManager>();
            }
        }

        /// <summary>
        /// Calls the Razorpay IFSC API using the given IFSC code and retrieves
        /// bank details like bank name and branch.
        /// </summary>
        /// <param name="ifsc">The IFSC code of the bank branch.</param>
        /// <returns>
        /// An IfscApiResponse object containing bank name and branch information,
        /// or null if the API call fails.
        /// </returns>

        private async Task<IfscApiResponse> GetBankDetailsFromIfscSS(string ifsc)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync($"https://ifsc.razorpay.com/{ifsc}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<IfscApiResponse>(json);
                    }
                }
            }
            catch (Exception)
            {
                // Optionally log the error
            }
            return null;
        }

        /// <summary>
        /// Represents the response from the Razorpay IFSC API containing
        /// the bank name and branch name for a given IFSC code.
        /// </summary>
        public class IfscApiResponse
        {
            [JsonProperty("BANK")]
            public string Bank { get; set; }

            [JsonProperty("BRANCH")]
            public string Branch { get; set; }
        }




        /// <summary>
        /// this method create to Add Society Bank and save in database
        ///  04/07/2025
        ///  MS
        /// </summary>
        /// <param name="objbal"></param>
        /// <returns></returns>
        public async Task<bool> AddBankSocietyMS(AccountManager objbal)
        {
            try
            {
                MSSQL db = new MSSQL();

                var parameters = new Dictionary<string, string>
{
    { "@flag", "AddBankSocietyMS" },
    { "@AllCode", objbal.AllCode ?? "SC001" },
    { "@AccountTypeId", objbal.AccountTypeId.ToString() },
    { "@OpeningBalance", objbal.OpeningBalance.ToString() },
    { "@IFSCCode", objbal.IFSCCode },
    { "@UPIId", objbal.UPIId ?? string.Empty },
    { "@AccountNo", objbal.AccountNo },
    { "@AddedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
    { "@ISActive", objbal.IsActive.ToString() }
};

                await db.ExecuteStoreProcedureReturnDataReader("sp_SMS", parameters);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }




        /// <summary>
        /// fetch in the dropdown BankType like Saving ,Current 
        ///  04/07/2025
        ///  Ms
        /// </summary>
        /// <returns></returns>
        public async Task<DataSet> FetchAccountTypeMS()
        {
            try
            {
                MSSQL db = new MSSQL();

                var parameters = new Dictionary<string, string>
        {
            { "@flag", "FetchAccountTypeMS" }
        };

                DataSet ds = await db.ExecuteStoreProcedureReturnDS("sp_SMS", parameters);
                return ds;
            }
            catch (Exception)
            {

                return new DataSet();
            }
        }



        #endregion





        #region********************************************************************* Society Ac.Details Cash ***********************************************************



        public async Task<List<AccountManager>> CashTransactionDD()
        {
            MSSQL db = new MSSQL();
            var parameters = new Dictionary<string, string>
        {
            { "@Flag", "FetchCashTransactionDD" }
        };

            var dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);
            var transactions = new List<AccountManager>();

            foreach (DataRow dr in dt.Rows)
            {
                transactions.Add(new AccountManager
                {
                    TransactionId = Convert.ToInt32(dr["TransactionId"]),
                    TransactionCode = dr["TransactionCode"].ToString(),
                    EntityCode = dr["EntityCode"].ToString(),
                    PaymentByName = dr["PaymentByName"].ToString(),
                    PaidToName = dr["PaidToName"].ToString(),
                    Amount = Convert.ToDecimal(dr["Amount"]),
                    PaymentModeName = dr["PaymentModeName"].ToString(),
                    PaymentPurpose = dr["PaymentPurpose"].ToString(),
                    TransactionId_ChequeId = dr["TransactionId_ChequeId"].ToString(),
                    PaidDate = Convert.ToDateTime(dr["PaidDate"]),
                    TransactionNature = dr["TransactionNature"].ToString(),
                    SubTypeName = dr["SubTypeName"].ToString(),
                    Document = dr["Document"].ToString()
                });
            }

            return transactions;
        }
        ///////////////////////////////////////////
        ////////////////////////////////////////////////////////////// Shruti Mane ///////////////////////////////////////////////////////////////////////






        /// <summary>
        /// Fetches the list of available transaction types from the database.
        /// </summary>

        public async Task<DataSet> FetchTransactionTypeAsync()
        {
            var db = new MSSQL();
            var parameters = new Dictionary<string, string>
   {
    { "@Flag", "TransactionType" }
    };
            return await db.ExecuteStoreProcedureReturnDS("sp_SMS", parameters);
        }


        /// <summary>
        /// Fetches the list of all workers from the database.
        /// </summary>
        public async Task<DataSet> FetchWorkerAsync()
        {
            var db = new MSSQL();
            var parameters = new Dictionary<string, string>
{
    { "@Flag", "FetchWorker" }
};
            return await db.ExecuteStoreProcedureReturnDS("sp_SMS", parameters);
        }

        /// <summary>
        /// Fetches the list of all registered members from the database.
        /// </summary>

        public async Task<DataSet> FetchMemberAsync()
        {
            var db = new MSSQL();
            var parameters = new Dictionary<string, string>
{
    { "@Flag", "Fetchmembers" }
};
            return await db.ExecuteStoreProcedureReturnDS("sp_SMS", parameters);
        }


        /// <summary>
        /// Fetches the list of all vendors from the database.
        /// </summary>


        public async Task<DataSet> FetchVendorAsync()
        {
            var db = new MSSQL();
            var parameters = new Dictionary<string, string>
{
    { "@Flag", "FetchVendor" }
};
            return await db.ExecuteStoreProcedureReturnDS("sp_SMS", parameters);
        }

        /// <summary>
        /// Fetches the list of event handlers from the database.
        /// </summary>

        public async Task<DataSet> FetchEventHandlersAsync()
        {
            var db = new MSSQL();
            var parameters = new Dictionary<string, string>
{
    { "@Flag", "FetchEventHandlers" }
};
            return await db.ExecuteStoreProcedureReturnDS("sp_SMS", parameters);
        }

        /// <summary>
        /// Fetches all maintenance records for a specific month and year.
        /// </summary>
        /// <param /name=""/>AccountManager object containing Month and Year</param>

        //        public async Task<DataSet> FetchMaintenanceAsync(AccountManager obju1)
        //        {
        //            var db = new MSSQL();
        //            var parameters = new Dictionary<string, string>
        //{
        //    { "@Flag", "FetchingMaintenance" },
        //    { "@Month", obju1.Month },
        //    { "@Year", obju1.Year }
        //};
        //            return await db.ExecuteStoreProcedureReturnDS("sp_SMS", parameters);
        //        }


        public async Task<DataSet> FetchMaintenanceAsync(AccountManager obju1)
        {
            var db = new MSSQL();

            var parameters = new Dictionary<string, string>
    {
        { "@Flag", "FetchingMaintenance" },
        { "@Month", obju1.Month.ToString() },
        { "@Year", obju1.Year.ToString() }
    };

            return await db.ExecuteStoreProcedureReturnDS("sp_SMS", parameters);
        }

        /// <summary>
        /// Fetches members associated with a specific maintenance code.
        /// </summary>
        /// <param /name=""/>AccountManager object containing MaintenanceCode</param>

        public async Task<DataSet> FetchMaintenancebyMaintenanceAsync(AccountManager obju)
        {
            var db = new MSSQL();
            var parameters = new Dictionary<string, string>
{
    { "@Flag", "FetchingMembersASperwing" },
    { "@MaintenanceCode", obju.MaintenanceCode }
};
            return await db.ExecuteStoreProcedureReturnDS("sp_SMS", parameters);
        }

        /// <summary>
        /// Saves a new cash or cheque transaction to the database.
        /// Returns the generated TransactionCode if successful.
        /// </summary>
        /// <param /name="objAcc">/AccountManager object containing transaction data</param>
        /// <returns>Generated TransactionCode or null</returns>


        public async Task<string> SaveDataAsync(AccountManager objAcc)
        {
            var db = new MSSQL();
            var parameters = new Dictionary<string, string>
{
    { "@Flag", "SavingCashChequeTransaction" },
    { "@BankCodeName", objAcc.BankName },
    { "@EntityCode", objAcc.EntityCode },
    { "@PaymentBy", objAcc.PaymentBy },
    { "@PaidTo", objAcc.PaidTo },
    { "@Amount", objAcc.Amount.ToString() },
    { "@PaymentMode", objAcc.PaymentMode.ToString() },
    { "@PaymentPurpose", objAcc.PaymentPurpose },
    { "@ChequeId", objAcc.ChecqueNo },
    { "@TransactionType", objAcc.TransactionId.ToString() },
    { "@AttachmentPath", objAcc.AttachmentPath }
};

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["TransactionCode"].ToString();
            }

            return null;
        }
        /// <summary>
        /// Retrieves receipt details for a given transaction code.
        /// </summary>
        /// <param /name="transactionCode">/The code of the transaction</param>
        /// <returns>DataTable containing receipt information</returns>

        public async Task<DataTable> GetReceiptDataAsync(string transactionCode)
        {
            var db = new MSSQL();
            var parameters = new Dictionary<string, string>
{
    { "@Flag", "ReceiptData1" },
    { "@TransactionCode", transactionCode }
};

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);
            return dt;
        }


        /// <summary>
        /// Retrieves detailed maintenance item data for a given maintenance code.
        /// </summary>
        /// <param /name = "maintenanceCode" >/ The code of the maintenance entry</param>
        /// <returns>DataTable containing maintenance item details</returns>

        public async Task<DataTable> GetMaintenanceItemsAsync(string maintenanceCode)
        {
            var db = new MSSQL();
            var parameters = new Dictionary<string, string>
{
    { "@Flag", "ReceiptData2" },
    { "@MaintenanceCode", maintenanceCode }
};

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);
            return dt;
        }


        /// <summary>
        /// Saves the generated PDF path for a receipt in the database.
        /// </summary>
        /// <param /name="objAcc">/AccountManager object with transaction and PDF path</param>
        /// <returns>The result as a string (e.g., success flag or path)</returns>
        public async Task<string> SaveReceiptpdfpathAsync(AccountManager objAcc)
        {
            var db = new MSSQL();

            var parameters = new Dictionary<string, string>
{
    { "@Flag", "InsertReceiptPdf" },
    { "@PaymentMode", objAcc.PaymentMode.ToString() },
    { "@TransactionCode", objAcc.TransactionCode },
    { "@AttachmentPath", objAcc.AttachmentPath }
};

            // Call your helper to execute and get scalar result
            object result = await db.ExecuteStoreProcedureReturnObj("sp_SMS", parameters);

            return result?.ToString();
        }


        /// <summary>
        /// Retrieves salary slip details for a worker using their WorkerCode.
        /// </summary>
        /// <param /name="WorkerCode">/The unique code of the worker</param>
        /// <returns>DataTable containing worker salary slip details</returns>
        public async Task<DataTable> GenerateSalarySlip(string WorkerCode)
        {
            var db = new MSSQL();
            var parameters = new Dictionary<string, string>
{
    { "@Flag", "FetchingWorkerDetails" },
    { "@WorkerCode", WorkerCode }
};

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);
            return dt;
        }

        /// <summary>
        /// Marks the specified expense as paid in the database by calling the stored procedure
        /// with flag 'UpdateExpensepaid'. This sets StatusId = 10 for the given ExpenseCode.
        /// </summary>
        /// <param /name="expenseCode"/>The unique code of the expense to update.</param>


        public async Task MarkExpenseAsPaidAsync(string expenseCode)
        {
            var db = new MSSQL();
            var parameters = new Dictionary<string, string>
{
    { "@Flag", "UpdateExpensepaid" },
    { "@ExpenseCode", expenseCode }
};

            await db.ExecuteStoreProcedure("sp_SMS", parameters);
        }


        /// <summary>
        /// Marks the specified event budget as paid in the database by calling the stored procedure
        /// with flag 'UpdateEventpaid'. This sets BudgetStatus = 10 for the given EventCode.
        /// </summary>
        /// <param /name="eventCode">/The unique code of the event to update.</param>


        public async Task MarkEventBudgetAsPaidAsync(string eventCode)
        {
            var db = new MSSQL();
            var parameters = new Dictionary<string, string>
{
    { "@Flag", "UpdateEventpaid" },
    { "@EventCode", eventCode }
};

            await db.ExecuteStoreProcedure("sp_SMS", parameters);
        }





        #endregion


        #region********************************************************************* Maintaince Management ***********************************************************




        // ✅ Fetch Member List with Maintenance Info   ////  Date-04/07/2025
        public async Task<List<AccountManager>> GetMemberMaintenanceListSY()
        {
            List<AccountManager> list = new List<AccountManager>();

            try
            {
                Dictionary<string, string> para = new Dictionary<string, string>
            {
                { "@flag", "FetchMemberListSY" }
            };

                DataSet ds = await db.ExecuteStoreProcedureReturnDS("sp_SMS", para);

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        AccountManager obj = new AccountManager();

                        obj.SrNo = Convert.ToInt32(dr["Sr No"]);
                        obj.EntityCode = dr["EntityCode"].ToString();
                        obj.MemberCode = dr["MemberCode"].ToString();
                        obj.FullName = dr["Full Name"].ToString();
                        obj.Email = dr["Email"].ToString();
                        obj.WingName = dr["Wing Name"].ToString();
                        obj.FlatType = dr["Flat Type"].ToString();

                        obj.PaidDate = dr["PaidDate"] != DBNull.Value ? Convert.ToDateTime(dr["PaidDate"]) : (DateTime?)null;

                        if (dr["MaintananceTypeId"] != DBNull.Value)
                            obj.MaintananceTypeId = Convert.ToInt32(dr["MaintananceTypeId"]);

                        obj.TotalAmount = Convert.ToDecimal(dr["Total Amount"]);
                        obj.PaidAmount = Convert.ToDecimal(dr["Paid Amount"]);
                        obj.Status = dr["Status"].ToString();

                        list.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                // Optionally log the error or handle it accordingly
                Console.WriteLine("Error in GetMemberMaintenanceListSY: " + ex.Message);
                // You may also log to file or a logging system
            }

            return list;
        }

        // ✅ Show Member Maintenance Split Details
        public async Task<List<AccountManager>> GetMemberMaintenanceDetailsSY(string memberCode, string maintainanceTypeId, string entityCode)
        {
            List<AccountManager> list = new List<AccountManager>();

            try
            {
                var parameters = new Dictionary<string, string>
            {
                { "@flag", "MemberMaintenanceDetailsSY" },
                { "@MemberCode", memberCode },
                { "@MaintananceTypeId", maintainanceTypeId },
                { "@EntityCode", entityCode }
            };

                MSSQL objSql = new MSSQL();
                DataTable dt = await objSql.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new AccountManager
                    {
                        FullName = row["FullName"]?.ToString(),
                        FlatType = row["FlatType"]?.ToString(),
                        PaidDate = string.IsNullOrWhiteSpace(row["PaidDate"]?.ToString()) ? (DateTime?)null : Convert.ToDateTime(row["PaidDate"]),
                        MaintenanceId = row["MaintenanceId"]?.ToString(),
                        ChargeAmount = row["ChargeAmount"] != DBNull.Value ? Convert.ToDecimal(row["ChargeAmount"]) : 0,
                        TotalAmount = row["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(row["TotalAmount"]) : 0
                    });
                }
            }
            catch (Exception ex)
            {
                // Optionally log the error or handle it accordingly
                Console.WriteLine("Error in GetMemberMaintenanceDetailsSY: " + ex.Message);
            }

            return list;
        }

#endregion


        #region********************************************************************* Community Send Email ***********************************************************





        /// <summary>
        /// Sends email with optional attachment using async SMTP. 03/07/2025
        /// </summary>
        public async Task<string> SendEMailVM(AccountManager emailProps, HttpPostedFileBase postedFile)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailProps.ToEmailAddress))
                    return "❌ Failed: Recipient email is missing.";
                if (string.IsNullOrWhiteSpace(emailProps.Subject))
                    return "❌ Failed: Email subject is required.";

                var helper = new GSTSMSHelper.EmailHelper();

                string finalBody = helper.SendEmailAccountant(emailProps.FromEmailAddress, emailProps.ContactNumber);

                string host = ConfigurationManager.AppSettings["EmailHost"];
                int port = int.Parse(ConfigurationManager.AppSettings["EmailPort"]);
                string username = ConfigurationManager.AppSettings["EmailUsername"];
                string password = ConfigurationManager.AppSettings["EmailPassword"];

                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress(emailProps.FromEmailAddress);
                    message.To.Add(emailProps.ToEmailAddress);
                    message.Subject = emailProps.Subject;
                    message.Body = finalBody;
                    message.IsBodyHtml = true;

                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        message.Attachments.Add(new Attachment(postedFile.InputStream, postedFile.FileName));
                    }

                    using (SmtpClient smtpClient = new SmtpClient(host, port))
                    {
                        smtpClient.Credentials = new NetworkCredential(username, password);
                        smtpClient.EnableSsl = true;
                        await smtpClient.SendMailAsync(message);
                    }
                }

                return $"✅ Sent: {emailProps.ToEmailAddress}";
            }
            catch (Exception ex)
            {
                return $"❌ Failed: {emailProps.ToEmailAddress} → {ex.Message}";
            }
        }




        /// <summary>
        /// Gets accountant email/contact info   03/07/2025
        /// </summary>
        public async Task<AccountManager> GetAccountantDetailsVM()
        {
            AccountManager accountant = new AccountManager();

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Flag", "FetchAccountantVM")
            };

            DataTable dt = await GSTSMSHelper.MSSQL.ExecuteStoredProcedure( "sp_SMS", parameters);



            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                accountant.StaffId = Convert.ToInt32(row["StaffId"]);
                accountant.StaffCode = row["StaffCode"].ToString();
                accountant.FromEmailAddress = row["Email"].ToString();
                accountant.FlatCode = row["FlatCode"].ToString();
                accountant.ContactNumber = row["ContactNumber"].ToString();
            }

            return accountant;
        }




        /// <summary>
        /// Returns member list for a wing or specific IDs .  03/07/2025
        /// </summary>
        public async Task<List<AccountManager>> GetMembersVM(string wing, List<int> specificMemberIds = null)
        {
            List<AccountManager> members = new List<AccountManager>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@WingName", wing),
                new SqlParameter("@MemberIds", specificMemberIds != null && specificMemberIds.Count > 0
                    ? string.Join(",", specificMemberIds)
                    : (object)DBNull.Value),
                new SqlParameter("@Flag", "FetchMemberVM")
            };

          
            DataTable dt = await GSTSMSHelper.MSSQL.ExecuteStoredProcedure( "sp_SMS", parameters);

            foreach (DataRow row in dt.Rows)
            {
                members.Add(new AccountManager
                {
                    MemberId = Convert.ToInt32(row["MemberId"]),
                    FullName = row["FullName"].ToString(),
                    FlatCode = row["FlatCode"].ToString(),
                    ToEmailAddress = row["Email"].ToString()
                });
            }

            return members;
        }


        #endregion

        #region*********************************************************************  Community Complaints ***********************************************************
        public async Task<DataSet> FetchAllComplaintsAsyncSB()
        {
            try
            {
                var parameters = new Dictionary<string, string> // ✅ Use string for both key and value
        {
            { "@Flag", "ComplaintFetch" }
        };

                return await db.ExecuteStoreProcedureReturnDS("sp_SMS", parameters); // ✅ Correct method
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to fetch complaints: " + ex.Message, ex);
            }
        }



        // ✅ Return mapped List<AccountManager> of Complaints
        public async Task<List<AccountManager>> GetAllComplaintsAsync()
        {
            List<AccountManager> complaints = new List<AccountManager>();
            try
            {
                DataSet ds = await FetchAllComplaintsAsyncSB();
                int serial = 1;

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    complaints.Add(new AccountManager
                    {
                        SerialNumber = serial++,
                        ComplaintId = Convert.ToInt32(row["ComplaintId"]),
                        ComplaintType = row["ComplaintName"].ToString(),
                        Complaint = row["Description"].ToString(),
                        ComplaintDate = Convert.ToDateTime(row["ComplaintDate"]),
                        AssignBy = row["RaisedBy"].ToString(),
                        StatusName = row["StatusName"].ToString()
                    });
                }

                return complaints;
            }
            catch (Exception ex)
            {
                throw new Exception("Error mapping complaint data: " + ex.Message, ex);
            }
        }
        public async Task<AccountManager> GetResolvedComplaintDetailsByIdAsync(int ComplaintId)
        {
            AccountManager complaint = null;

            try
            {
                MSSQL db = new MSSQL();

                // ✅ Use Dictionary<string, string> as required by your MSSQL class
                var parameters = new Dictionary<string, string>
{
    { "@flag", "viewfetchcomplaint" },
    { "@complainId", ComplaintId.ToString() } // 👈 convert int to string
};

                DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    complaint = new AccountManager
                    {
                        ComplaintId = Convert.ToInt32(row["ComplaintId"]),
                        SecretoryName = row["Secretary Name"].ToString(),
                        ComplaintType = row["Complaint Type"].ToString(),
                        Description = row["Complaint Description"].ToString(),
                        ComplaintDate = Convert.ToDateTime(row["ComplaintDate"]),
                        Status = row["Status"].ToString(),
                        DocumentPath = row.Table.Columns.Contains("DocumentPath") ? row["DocumentPath"].ToString() : null
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching resolved complaint details", ex);
            }

            return complaint;
        }

        #endregion


        #region********************************************************************* Community Notice  ***********************************************************

        /// <summary>
        ///  This method fetches Fetches a list of all notices from the database,
        /// with the flag 'FetchNotice'. Each notice includes title, description, and publish date.
        /// </summary>

        /// <returns>
        /// A list of AccountManager objects containing notice details such as:
        /// NoticeAnnouncementId, NoticeTitle, Description, and PublishDate.
        /// </returns>

        public async Task<List<AccountManager>> GetAllNoticeListSS()
        {
            MSSQL db = new MSSQL();
            var parameters = new Dictionary<string, string>
{
    { "@Flag", "FetchNoticeSS" },

};

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);
            List<AccountManager> list = new List<AccountManager>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AccountManager
                {
                    NoticeAnnouncementId = Convert.ToInt32(row["NoticeAnnouncementId"]),
                    NoticeAnnouncementCode = row["NoticeAnnouncementCode"].ToString(),
                    NoticeTitle = row["NoticeTitle"].ToString(),
                    Description = row["Description"].ToString(),
                    PublishDate = row.Field<DateTime?>("PublishDate"),
                    EndDate = row.Field<DateTime?>("EndDate"),
                    SendBy = row["SendBy"].ToString(),
                    SendByRole = row["SendByRole"].ToString(),
                    CreatedDate = row.Field<DateTime?>("CreatedDate"),
                    Document = row["Document"].ToString()
                });
            }

            return list;
        }




        //public async Task<DataTable> ShowData()
        //{
        //    SqlParameter[] parameters = {
        //        new SqlParameter("@Flag", "ShowData")
        //    };
        //    return await MSSQL.ExecuteStoredProcedureAsync("GSTSMS", "sp_SMS", parameters);
        //}



        /// <summary>
        /// THIS METHOD IS FOR SAVE NOTICE
        /// </summary>

        public async Task<string> SaveNoticeRM(AccountManager prop)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Flag", "SaveNotice"),
                new SqlParameter("@NoticeTitle", prop.NoticeTitle ?? string.Empty),
                new SqlParameter("@Description", prop.Description ?? string.Empty),
                new SqlParameter("@PublishDate", prop.PublishDate),
                new SqlParameter("@EndDate", prop.EndDate),
                new SqlParameter("@SendBy", prop.SendBy),
                new SqlParameter("@CreatedDate", DateTime.Now),
                new SqlParameter("@Document", string.IsNullOrEmpty(prop.Document) ? "" : prop.Document),
                new SqlParameter("@WingName", string.IsNullOrEmpty(prop.WingName) ? DBNull.Value : (object)prop.WingName),
                new SqlParameter("@MemberIds", prop.SelectedMemberCodes != null ? string.Join(",", prop.SelectedMemberCodes) : (object)DBNull.Value)
            };

            DataTable dt = await MSSQL.ExecuteStoredProcedure("sp_SMS", parameters);
            return dt.Rows.Count > 0 ? dt.Rows[0][0].ToString() : "";
        }


        public async Task InsertNoticeLogByWingRM(string noticeCode, string wingName)
        {
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@Flag", "InsertNoticeLog"),
                new SqlParameter("@NoticeCode", noticeCode),
                new SqlParameter("@WingName", wingName)
            };

            await MSSQL.ExecuteStoredProcedure( "sp_SMS", param);
        }

        public async Task InsertNoticeLogsByMembersRM(string noticeCode, List<string> memberCodes)
        {
            if (string.IsNullOrWhiteSpace(noticeCode) || memberCodes == null) return;

            foreach (var memberCode in memberCodes)
            {
                if (string.IsNullOrWhiteSpace(memberCode)) continue;

                SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@Flag", "InsertNoticeLog"),
                    new SqlParameter("@NoticeCode", noticeCode),
                    new SqlParameter("@MemberCode", memberCode)
                };

                await MSSQL.ExecuteStoredProcedure( "sp_SMS", param);
            }
        }

        /// <summary>
        /// THIS METHOD IS FOR THE GETMEMBERS
        /// </summary>

        public async Task<List<AccountManager>> GetMembersRM(string wing, List<int> memberIds = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@WingName", wing),
                new SqlParameter("@MemberIds", DBNull.Value),
                new SqlParameter("@Flag", "FetchMember")
            };

            DataTable dt = await MSSQL.ExecuteStoredProcedure ("sp_SMS", parameters);
            List<AccountManager> members = new List<AccountManager>();

            foreach (DataRow row in dt.Rows)
            {
                members.Add(new AccountManager
                {
                    MemberId = Convert.ToInt32(row["MemberId"]),
                    MemberCode = row["MemberCode"].ToString(),
                    FullName = row["FullName"].ToString(),
                    FlatCode = row["FlatCode"].ToString(),
                    ToEmailAddress = row["Email"].ToString()
                });
            }

            return members;
        }

        /// <summary>
        /// THIS METHOD IS FOR THE SEND EMAIL
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>

        public async Task<string> SendEmailRM(AccountManager prop)
        {
            try
            {
                if (prop.ToEmailAddresses == null || !prop.ToEmailAddresses.Any())
                    return "❌ Failed: No recipient email provided.";

                string host = ConfigurationManager.AppSettings["EmailHost"];
                int port = Convert.ToInt32(ConfigurationManager.AppSettings["EmailPort"]);
                string fromEmail = ConfigurationManager.AppSettings["EmailFrom"];
                string username = ConfigurationManager.AppSettings["EmailUsername"];
                string password = ConfigurationManager.AppSettings["EmailPassword"];

                Regex emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

                using (SmtpClient smtp = new SmtpClient(host, port))
                {
                    smtp.Credentials = new NetworkCredential(username, password);
                    smtp.EnableSsl = true;

                    foreach (var email in prop.ToEmailAddresses)
                    {
                        if (string.IsNullOrWhiteSpace(email) || !emailRegex.IsMatch(email))
                            continue;

                        MailMessage message = new MailMessage
                        {
                            From = new MailAddress(fromEmail),
                            Subject = prop.Subject,
                            IsBodyHtml = true,
                            Body = $"Dear Member,<br/><br/>{prop.Description}<br/><br/>Regards,<br/>Green Valley Society"
                        };

                        message.To.Add(email);

                        if (prop.AttachmentList != null)
                        {
                            foreach (var file in prop.AttachmentList)
                            {
                                if (!string.IsNullOrWhiteSpace(file))
                                {
                                    string safeFile = Path.GetFileName(file);
                                    string path = HostingEnvironment.MapPath("~/Uploads/Notices/" + safeFile);
                                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                                    {
                                        message.Attachments.Add(new Attachment(path));
                                    }
                                }
                            }
                        }



                        await smtp.SendMailAsync(message);
                    }
                }

                return "✅ Emails sent successfully.";
            }
            catch (Exception ex)
            {
                return $"❌ Failed: {ex.Message}";
            }
        }


        /// <summary>
        /// THIS METHOD IS FOR GET NOTICE BY ITS CODE
        /// </summary>
        /// <param name="noticeCode"></param>
        /// <returns></returns>
        public async Task<DataTable> GetNoticeByCodeRM(string noticeCode)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Flag", "ShowData"),
                new SqlParameter("@NoticeCode", noticeCode)
            };

            return await MSSQL.ExecuteStoredProcedure( "sp_SMS", parameters);
        }

        /// <summary>
        /// THIS METHOD IS FOR UPDATE NOTICE
        /// </summary>

        public async Task UpdateNotice(AccountManager prop)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Flag", "UpdateNotice"),
                new SqlParameter("@NoticeCode", prop.NoticeCode),
                new SqlParameter("@NoticeTitle", prop.NoticeTitle ?? ""),
                new SqlParameter("@Description", prop.Description ?? ""),
                new SqlParameter("@PublishDate", prop.PublishDate),
                new SqlParameter("@EndDate", prop.EndDate),
                new SqlParameter("@Document", string.IsNullOrEmpty(prop.Document) ? DBNull.Value : (object)prop.Document)
            };

            await MSSQL.ExecuteStoredProcedure( "sp_SMS", parameters);
        }

        #endregion


        #region********************************************************************* Expence***********************************************************



        public async Task<List<AccountManager>> GetAllExpenses()
        {
            var parameters = new Dictionary<string, string>
        {
            { "@Flag", "FetchExpenseList" }
        };

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);
            List<AccountManager> list = new List<AccountManager>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AccountManager
                {
                    ExpenseId = Convert.ToInt32(row["ExpenseId"]),
                    ExpenseCode = row["ExpenseCode"].ToString(),
                    PaymentTo = row["PaymentTo"].ToString(),
                    VendorName = row["VendorName"].ToString(),
                    ExpenseTypeId = Convert.ToInt32(row["ExpenseTypeId"]),
                    ExpenseType = row["ExpenseTypeName"].ToString(),
                    ExpenseName = row["ExpenseName"].ToString(),
                    WingName = row["WingName"].ToString(),
                    AddedBy = row["AddedBy"].ToString(),
                    Description = row["Description"].ToString(),
                    AddedDate = Convert.ToDateTime(row["AddedDate"]),
                    Amount = Convert.ToDecimal(row["Amount"]),
                    StatusId = Convert.ToInt32(row["StatusId"]),
                    StatusName = row["StatusName"].ToString(),
                    IFSCCode = row["IFSCCode"].ToString()
                });
            }

            return list;
        }

        public async Task<bool> SaveTransactionPS(string expenseCode, string paymentId, int paymentModeId)
        {
            var parameters = new Dictionary<string, string>
        {
            { "@Flag", "SaveTransactionPS" },
            { "@ExpenseCode", expenseCode },
            { "@PaymentId", paymentId },
            { "@PaymentModeId", paymentModeId.ToString() }
        };

            int result = await db.ExecuteStoreProcedureReturnInt("sp_SMS", parameters);
            return result > 0;
        }

        public async Task<Dictionary<string, string>> GetIFSCBankDetailsPS(string expenseCode)
        {
            var parameters = new Dictionary<string, string>
        {
            { "@Flag", "GetIFSCByExpenseCodePS" },
            { "@ExpenseCode", expenseCode }
        };

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new Dictionary<string, string>
            {
                { "IFSCCode", row["IFSCCode"].ToString() },
                { "VendorName", row["VendorName"].ToString() },
                { "Amount", row["Amount"].ToString() }
            };
            }

            return null;
        }

        public async Task<AccountManager> GetIFSCByCodePS(string expenseCode)
        {
            var parameters = new Dictionary<string, string>
        {
            { "@Flag", "GetExpenseByCodePS" },
            { "@ExpenseCode", expenseCode }
        };

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);

            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            return new AccountManager
            {
                ExpenseId = Convert.ToInt32(row["ExpenseId"]),
                ExpenseCode = row["ExpenseCode"].ToString(),
                PaymentTo = row["PaymentTo"].ToString(),
                ExpenseTypeId = Convert.ToInt32(row["ExpenseTypeId"]),
                ExpenseName = row["ExpenseName"].ToString(),
                WingId = Convert.ToInt32(row["WingId"]),
                AddedBy = row["AddedBy"].ToString(),
                Description = row["Description"].ToString(),
                AddedDate = Convert.ToDateTime(row["AddedDate"]),
                Amount = Convert.ToDecimal(row["Amount"]),
                StatusId = Convert.ToInt32(row["StatusId"]),
                IFSCCode = row["IFSCCode"].ToString(),
                VendorName = row["VendorName"].ToString()
            };
        }

        public async Task<AccountManager> GetFullExpenseDetailsPS(string expenseCode)
        {
            var parameters = new Dictionary<string, string>
        {
            { "@Flag", "GetFullExpenseDetailsPS" },
            { "@ExpenseCode", expenseCode }
        };

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);

            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            return new AccountManager
            {
                ExpenseCode = expenseCode,
                ExpenseName = row["ExpenseName"].ToString(),
                Description = row["Description"].ToString(),
                AddedDate = Convert.ToDateTime(row["AddedDate"]),
                TransactionCode = row["TransactionCode"].ToString(),
                TransactionAmount = Convert.ToDecimal(row["TransactionAmount"]),
                PaidDate = Convert.ToDateTime(row["PaidDate"]),
                PaymentMode = row["PaymentModeName"].ToString(),
                VendorName = row["VendorName"].ToString(),
                Email = row["Email"].ToString(),
                PhoneNumber = row["PhoneNumber"].ToString(),
                Address = row["Address"].ToString(),
                DocumentId = Convert.ToInt32(row["DocumentId"]),
                Document = row["Document"].ToString()
            };
        }

        public async Task<List<AccountManager>> GetServiceSubTypes()
        {
            var parameters = new Dictionary<string, string>
        {
            { "@Flag", "GetServiceSubTypes" }
        };

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);
            List<AccountManager> list = new List<AccountManager>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AccountManager
                {
                    SubTypeId = Convert.ToInt32(row["SubTypeId"]),
                    SubTypeName = row["SubTypeName"].ToString()
                });
            }

            return list;
        }




        public async Task<bool> RegisterVendor(AccountManager model)
        {
            var parameters = new Dictionary<string, string>
{
    { "@Flag", "InsertVendorDetails" },
    { "@VendorName", model.VendorName },
    { "@ServiceSubTypeId", model.ServiceSubTypeId.ToString() },
    { "@Email", model.Email },
    { "@PhoneNumber", model.PhoneNumber },
    { "@AlternatePhoneno", model.AlternatePhoneNumber },
    { "@Address", model.Address },
    { "@SubTypeId1", "46" },
    { "@Document1", model.Document1 },
    { "@SubTypeId2", "44" },
    { "@Document2", model.Document2 },
    { "@Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
    { "@OpeningBalance", model.OpeningBalance.ToString() },
    { "@IFSCCode", model.IFSCCode },
    { "@UPIId", model.UPIId },
    { "@AccountNo", model.AccountNo }
};

            try
            {
                await db.ExecuteStoreProcedureReturnInt("sp_SMS", parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }





        //public async Task<bool> InsertExpenseAsync(AccountManager obj)
        //{
        //    var parameters = new Dictionary<string, string>
        //    {
        //        { "@Flag", "insertExpenseDD" },
        //        { "@ExpenseCode", obj.ExpenseCode ?? "" },
        //        { "@PaymentTo", obj.PaymentTo ?? "" },
        //        { "@ExpenseTypeId", obj.ExpenseTypeId.ToString() },
        //        { "@ExpenseName", obj.ExpenseName ?? "" },
        //        { "@WingId", obj.WingId.ToString() },
        //        { "@AddedBy", obj.AddedBy ?? "" },
        //        { "@Description", obj.Description ?? "" },
        //        { "@AddedDate", obj.Date.ToString("yyyy-MM-dd HH:mm:ss") },
        //        { "@Amount", obj.Amount.ToString() },
        //        { "@Document", obj.DocumentBase64String ?? "" },
        //        { "@GSTTypeId", obj.GSTTypeId.ToString() }
        //    };

        //    int rows = await db.ExecuteStoreProcedureReturnInt("sp_SMS", parameters);
        //    return rows > 0;
        //}



        public async Task<string> InsertExpenseAsync(AccountManager obj)
        {
            var parameters = new Dictionary<string, string>
{
    { "@Flag", "insertExpenseDD" },
    { "@ExpenseCode", obj.ExpenseCode ?? "" },
    { "@PaymentTo", obj.PaymentTo ?? "" },
    { "@ExpenseTypeId", obj.ExpenseTypeId.ToString() },
    { "@ExpenseName", obj.ExpenseName ?? "" },
    { "@WingId", obj.WingId.ToString() },
    { "@AddedBy", obj.AddedBy ?? "" },
    { "@Description", obj.Description ?? "" },
    { "@AddedDate", obj.Date.ToString("yyyy-MM-dd HH:mm:ss") },
    { "@Amount", obj.Amount.ToString() },
    { "@Document", obj.DocumentBase64String ?? "" },
    { "@GSTTypeId", obj.GSTTypeId.ToString() }
};

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);
            return dt.Rows.Count > 0 ? dt.Rows[0]["ExpenseCode"].ToString() : null;
        }

        public async Task<List<AccountManager>> GetVendorNamesAsync()
        {
            var parameters = new Dictionary<string, string> { { "@Flag", "fetchVendorNameDD" } };
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);

            List<AccountManager> list = new List<AccountManager>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AccountManager
                {
                    VendorCode = row["VendorCode"].ToString(),
                    VendorName = row["VendorName"].ToString()
                });
            }
            return list;
        }

        public async Task<List<AccountManager>> GetWingNamesAsync()
        {
            var parameters = new Dictionary<string, string> { { "@Flag", "fetchWingNameDD" } };
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);

            List<AccountManager> list = new List<AccountManager>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AccountManager
                {
                    WingId = Convert.ToInt32(row["WingId"]),
                    WingName = row["WingName"].ToString()
                });
            }
            return list;
        }

        public async Task<List<AccountManager>> GetGSTTypeAsync()
        {
            var parameters = new Dictionary<string, string> { { "@Flag", "fetchGSTTypeDD" } };
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);

            List<AccountManager> list = new List<AccountManager>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AccountManager
                {
                    GSTTypeId = Convert.ToInt32(row["GSTTypeId"]),
                    GSTTypeName = row["GSTTypeName"].ToString()
                });
            }
            return list;
        }

        public async Task<List<AccountManager>> GetVendorTypesAsync()
        {
            var parameters = new Dictionary<string, string> { { "@Flag", "fetchVendorType" } };
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);

            List<AccountManager> list = new List<AccountManager>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AccountManager
                {
                    SubTypeId = Convert.ToInt32(row["SubTypeId"]),
                    SubTypeName = row["SubTypeName"].ToString()
                });
            }
            return list;
        }


        public async Task<List<AccountManager>> GetVendorTypeAsync()
        {
            var parameters = new Dictionary<string, string> { { "@Flag", "fetchVendorTypeDD" } };
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);

            List<AccountManager> list = new List<AccountManager>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AccountManager
                {
                    SubTypeId = Convert.ToInt32(row["SubTypeId"]),
                    SubTypeName = row["SubTypeName"].ToString()
                });
            }
            return list;
        }


        public async Task<List<SelectListItem>> GetVendorsByTypeAsync(int vendorType)
        {
            var parameters = new Dictionary<string, string>();

            if (vendorType == 38)
                parameters.Add("@Flag", "fetchRegularVendorNameDD");
            else if (vendorType == 39)
                parameters.Add("@Flag", "fetchOtherVendorNameDD");
            else
                return new List<SelectListItem>();

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SelectListItem
                {
                    Value = row["VendorCode"].ToString(),
                    Text = row["VendorName"].ToString()
                });
            }

            return list;
        }


        #endregion



        #region********************************************************************* Event Management ***********************************************************


        //************************************************************** Savita Gawali ***********************************************************************************************************************************************************



        /// <summary>
        ///  This method fetches Fetches a list of all events from the database,
        /// with the flag 'FetchEvents'. Each event includes budgeting and handler information.
        /// </summary>

        /// <returns>
        /// A list of AccountManager objects containing event details such as:
        /// EventCode, EBudgetId, EventName, EventHandlerName, CreatedDate, AllocatedBudget,
        /// ActualCost, BudgetAddedDate, BudgetStatus, BudgetStatusName, and IFSC code of the handler.
        /// </returns>

        public async Task<List<AccountManager>> GetAllEventListSS()
        {
            MSSQL db = new MSSQL();
            var parameters = new Dictionary<string, string>
        {
            { "@Flag", "FetchEventsSS" }
        };

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);

            List<AccountManager> list = new List<AccountManager>();


            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AccountManager
                {

                    EventCode = row["EventCode"].ToString(),
                    EBudgetId = row.Field<int?>("EBudgetId") ?? 0,

                    EventName = row["EventName"].ToString(),
                    EventHandlerName = row["EventHandlerName"].ToString(),
                    CreatedDate = row.Field<DateTime?>("CreatedDate") ?? DateTime.MinValue,
                    AllocatedBudget = row.Field<decimal?>("AllocatedBudget") ?? 0,
                    ActualCost = row.Field<decimal?>("ActualCost") ?? 0,
                    BudgetAddedDate = row.Field<DateTime?>("BudgetAddedDate") ?? DateTime.MinValue,
                    BudgetStatus = Convert.ToInt32(row["BudgetStatus"]),
                    BudgetStatusName = row["BudgetStatusName"].ToString(),
                    IFSCCode = row["IFSCCode"].ToString(),
                });
            }

            return list;
        }

        /// <summary>
        /// Sends a budget approval request by emailing the admin and then updating
        /// the status in the database to "Approved" if the email is successfully sent.
        /// </summary>
        /// <param name="EBudgetId">The ID of the event budget to approve.</param>

        /// <returns>
        /// True if the email is sent and the database update is successful; otherwise, false.
        /// </returns>


        public async Task<bool> SendRequestAsyncSS(int EBudgetId)
        {
            // ✅ Step 1: Send Email to Admin
            EmailHelper email = new EmailHelper();
            bool emailSent = await email.SendEmailAsync(
                "shendremukesh12@gmail.com",
                "Budget Request Approval",
                $"Budget request with ID {EBudgetId} is ready for approval. Please review it."
            );

            if (!emailSent)
            {
                // ❌ Email failed, so don't approve
                return false;
            }

            // ✅ Step 2: Now update DB status to Approved
            MSSQL db = new MSSQL();

            var parameters = new Dictionary<string, string>
{
    { "@Flag", "UpdateToApprovedSS" },
    { "@EBudgetId", EBudgetId.ToString() }


};

            int rowsAffected = await db.ExecuteStoreProcedureReturnInt("sp_SMS", parameters);
            return rowsAffected > 0;
        }

        /// <summary>
        ///  This method fetches Fetches a list of all notices from the database,
        /// with the flag 'FetchNotice'. Each notice includes title, description, and publish date.
        /// </summary>

        /// <returns>
        /// A list of AccountManager objects containing notice details such as:
        /// NoticeAnnouncementId, NoticeTitle, Description, and PublishDate.
        /// </returns>


        //************************************************************** Pradnya Mane ***********************************************************************************************************************************************************



        public async Task<List<AccountManager>> GetApprovedEventsAsync()
        {
            MSSQL db = new MSSQL();

            var parameters = new Dictionary<string, string>
        {
            { "@Flag", "FetchApprovedEvent" }
        };

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);
            List<AccountManager> list = new List<AccountManager>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AccountManager
                {
                    EventName = row["EventName"].ToString(),
                    CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.MinValue,
                    EventHandlerName = row["EventHandlerName"].ToString(),
                    PhoneNumber = row["PhoneNumber"].ToString()
                });
            }

            return list;
        }

        public async Task<decimal> GetOpeningBalanceAsync()
        {
            MSSQL db = new MSSQL();

            var parameters = new Dictionary<string, string>
        {
            { "@Flag", "FetchOpeningBalance" }
        };

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);
            if (dt.Rows.Count > 0 && dt.Rows[0]["OpeningBalance"] != DBNull.Value)
            {
                return Convert.ToDecimal(dt.Rows[0]["OpeningBalance"]);
            }

            return 0;
        }

        public async Task<bool> SubmitBudgetAsync(string eventName, decimal allocatedBudget)
        {
            MSSQL db = new MSSQL();

            var parameters = new Dictionary<string, string>
        {
            { "@Flag", "SaveAllocatedBudget" },
            { "@EventName", eventName },
            { "@ActualCost", "0" },
            { "@BudgetAddedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
            { "@AllocatedBudget", allocatedBudget.ToString() },
            { "@BudgetStatus", "11" }
        };

            int result = await db.ExecuteStoreProcedureReturnInt("sp_SMS", parameters);
            return result > 0;
        }

        public async Task<AccountManager> GetEventDetailsByCodeAsync(string eventCode)
        {
            MSSQL db = new MSSQL();

            var parameters = new Dictionary<string, string>
        {
            { "@Flag", "FetchUpdateEvent" },
            { "@EventCode", eventCode }
        };

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new AccountManager
                {
                    EventCode = row["EventCode"].ToString(),
                    EventName = row["EventName"].ToString(),
                    CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.MinValue,
                    EventHandlerName = row["EventHandlerName"].ToString(),
                    PhoneNumber = row["PhoneNumber"].ToString(),
                    AllocatedBudget = row["AllocatedBudget"] != DBNull.Value ? Convert.ToDecimal(row["AllocatedBudget"]) : 0,
                    BankAccount = row["BankAccount"].ToString(),
                    IFSCCode = row["IFSCCode"].ToString()
                };
            }

            return null;
        }

        public async Task<int> UpdateActualCostAsync(string eventCode, decimal actualCost)
        {
            MSSQL db = new MSSQL();

            var parameters = new Dictionary<string, string>
        {
            { "@Flag", "UpdateActualCost" },
            { "@EventCode", eventCode },
            { "@ActualCost", actualCost.ToString() }
        };

            int result = await db.ExecuteStoreProcedureReturnInt("sp_SMS", parameters);
            return result;
        }

        public async Task<AccountManager> GetEventDetailsForViewAsync(string eventCode)
        {
            MSSQL db = new MSSQL();

            var parameters = new Dictionary<string, string>
        {
            { "@Flag", "ViewEventDetails" },
            { "@EventCode", eventCode }
        };


            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new AccountManager
                {
                    EventCode = row["EventCode"].ToString(),
                    EventName = row["EventName"].ToString(),
                    CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.MinValue,
                    EventHandlerName = row["EventHandlerName"].ToString(),
                    PhoneNumber = row["PhoneNumber"].ToString(),
                    AllocatedBudget = row["AllocatedBudget"] != DBNull.Value ? Convert.ToDecimal(row["AllocatedBudget"]) : 0,
                    ActualCost = row["ActualCost"] != DBNull.Value ? Convert.ToDecimal(row["ActualCost"]) : 0,
                    BankAccount = row["BankAccount"].ToString(),
                    IFSCCode = row["IFSCCode"].ToString()
                };
            }

            return null;
        }



        //Shubham********************************************************************************

        /// <summary>
        /// Saviing the payment in transaction table
        /// </summary>
        /// <param name="eventCode"></param>
        /// <param name="transactionId"></param>
        /// <param name="paymentMode"></param>
        /// <returns></returns>

        public async Task<bool> SavePaymentSVTransactionSV(string eventCode, string transactionId, int paymentMode)
        {
            MSSQL db = new MSSQL();
            var parameters = new Dictionary<string, string>
        {
            { "@Flag", "InsertTransactionSV" },
            { "@EventCode", eventCode },
            { "@TransactionId_ChequeId", transactionId },
            { "@PaymentMode", paymentMode.ToString() }
        };

            int rowsAffected = await db.ExecuteStoreProcedureReturnInt("sp_SMS", parameters);

            // You can log this to a file or console
            Console.WriteLine("Rows affected: " + rowsAffected);

            return rowsAffected > 0;
        }


        #endregion



        #region********************************************************************* Worker Pay Manage ***********************************************************







        public async Task<List<AccountManager>> FetchWorkerInformationADAsync()
        {
            try
            {
                // Create parameters array
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@Flag", "FetchWorkerInformationAD")
                };

                // Call the stored procedure and get the DataTable
                DataTable dt = await MSSQL.ExecuteStoredProcedure("sp_SMS", parameters);

                List<AccountManager> workers = new List<AccountManager>();
                int serialNo = 1;

                foreach (DataRow row in dt.Rows)
                {
                    string paymentStatus = row["PaymentStatus"].ToString();

                    workers.Add(new AccountManager
                    {
                        WorkerCode = row["WorkerCode"].ToString(),
                        RoleName = row["Role"].ToString(),
                        StaffName = row["WorkerName"].ToString(),
                        Contact = row["Contact"].ToString(),
                        DateOfJoining = Convert.ToDateTime(row["Date"]),
                        BaseSalary = Convert.ToDecimal(row["BaseSalary"]),
                        AccountNumber = row["AccountNo"].ToString(),
                        IFSCCode = row["IFSC Code"].ToString(),
                        WorkerUPI = row["Worker UPI"].ToString(),
                        AttendanceMonth = row["AttendanceMonth"].ToString(),
                        DaysPresent = Convert.ToInt32(row["DaysPresent"]),
                        PerdayPayment = Convert.ToDecimal(row["PerdayPayment"]),
                        AmountToBePaid = Convert.ToDecimal(row["AmountToBePaid"]),
                        IsPaid = paymentStatus.Trim().ToLower() == "paid",
                        SerialNumber = serialNo++,
                        IsSelected = false
                    });
                }

                return workers;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to fetch worker data: " + ex.Message, ex);
            }
        }



        public async Task ProcessWorkerPaymentAsync(AccountManager input)
        {
            int paymentModeId = await GetPaymentModeFromRazorpayAsync(input.TransactionId_ChequeId);
            if (paymentModeId == 0 && int.TryParse(input.PaymentMode.ToString(), out int fallback))
                paymentModeId = fallback;

            if (!await IsSocietyBalanceSufficient(input.Amount))
                throw new Exception("Society balance insufficient.");

            var transaction = new AccountManager
            {
                BankName_Code = input.BankName_Code ?? "BANC086",
                TransactionCode = input.TransactionCode,
                EntityCode = input.EntityCode,
                PaymentBy = input.PaymentBy ?? "STF002",
                PaidTo = input.PaidTo,
                UPIId = input.UPIId,
                Amount = input.Amount,
                PaymentMode = paymentModeId.ToString(),
                PaymentPurpose = input.PaymentPurpose ?? "Monthly Salary",
                TransactionId_ChequeId = input.TransactionId_ChequeId,
                PaidDate = DateTime.Now,
                TransactionType = input.TransactionType != 0 ? input.TransactionType : 27
            };

            await InsertTransactionAsync(transaction);
        }

        public async Task InsertTransactionAsync(AccountManager transaction)
        {
            var parameters = new Dictionary<string, string>
        {
            { "@flag", "TransactionPaymentbyAccountant" },
            { "@BankName_Code", transaction.BankName_Code },
            { "@TransactionCode", transaction.TransactionCode ?? "" },
            { "@EntityCode", transaction.EntityCode ?? "" },
            { "@PaymentBy", transaction.PaymentBy },
            { "@PaidTo", transaction.PaidTo },
            { "@Amount", transaction.Amount.ToString(System.Globalization.CultureInfo.InvariantCulture) },
            { "@PaymentMode", transaction.PaymentMode.ToString() },
            { "@PaymentPurpose", transaction.PaymentPurpose },
            { "@TransactionId_ChequeId", transaction.TransactionId_ChequeId },
            //{ "@PaidDate", transaction.PaidDate.ToString("yyyy-MM-dd HH:mm:ss") },
            { "@TransactionType", transaction.TransactionType.ToString() },
            { "@UPIId", transaction.UPIId ?? "" }
        };

            await db.ExecuteStoreProcedure("sp_SMS", parameters);
        }




        public async Task<DataTable> FetchSingleWorkerPaymentData(string workerCode, string attendanceMonth)
        {
            var parameters = new Dictionary<string, string>
    {
        { "@flag", "FetchSingleWorkerPaymentData" },
        { "@WorkerCode", workerCode },
        { "@AttendanceMonth", attendanceMonth }
    };

            return await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);
        }




        public async Task<DataTable> FetchWorkerPaymentDetails(string workerCode, string attendanceMonth)
        {
            var parameters = new Dictionary<string, string>
    {
        { "@flag", "FetchWorkerPaymentWithTxn" },
        { "@WorkerCode", workerCode },
        { "@AttendanceMonth", attendanceMonth }
    };

            return await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);
        }




        private async Task<int> GetPaymentModeFromRazorpayAsync(string paymentId)
        {
            try
            {
                var key = "rzp_test_tnu8pNChRc5VBE";
                var secret = "wVSn4S0P2BpbvIiol3zLUzLG";
                var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{key}:{secret}"));

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
                    var response = await client.GetAsync($"https://api.razorpay.com/v1/payments/{paymentId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var json = Newtonsoft.Json.Linq.JObject.Parse(jsonString);
                        string method = json["method"]?.ToString();

                        if (method == "upi") return 33;
                        if (method == "netbanking") return 35;
                        if (method == "card") return 36;
                        if (method == "wallet") return 37;
                    }
                }
            }
            catch (Exception)
            {
                // Log error if needed
            }

            return 0;
        }

        private async Task<bool> IsSocietyBalanceSufficient(decimal amount)
        {
            BALAccountManager bal = new BALAccountManager();
            decimal balance = await bal.GetSocietyBalance();
            return balance >= amount;
        }













        public async Task<bool> IsPaymentAlreadyDone(string workerCode, string attendanceMonth)
        {
            DateTime paidMonth = DateTime.ParseExact(attendanceMonth, "yyyy-MM", null);

            var parameters = new Dictionary<string, string>
    {
        { "@flag", "CheckPaymentExists" },
        { "@PaidTo", workerCode },
        { "@PaidDate", paidMonth.ToString("yyyy-MM-dd") }
    };

            object result = await db.ExecuteStoreProcedureReturnObj("sp_SMS", parameters);
            int count = Convert.ToInt32(result ?? 0);
            return count > 0;
        }


        public async Task<decimal> GetSocietyBalance()
        {
            var parameters = new Dictionary<string, string>
    {
        { "@flag", "GetSocietyBalance" }
    };

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);
            if (dt.Rows.Count > 0)
                return Convert.ToDecimal(dt.Rows[0]["OpeningBalance"]);
            return 0;
        }

    


        #endregion


        #region********************************************************************* Reports  ***********************************************************



        //*FOR  COUNT ALL MEMEBERS IN  SOCIETY AND SHOW IN CARD/TAB**************************************************************************
        public async Task<int> GetTotalMemberCount()
        {
            MSSQL db = new MSSQL();
            Dictionary<string, string> param = new Dictionary<string, string>
        {
            { "@Flag", "CountMembersNK" }

        };

            object result = await db.ExecuteStoreProcedureReturnObj("sp_SMS", param);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        //*SHOW ALL MEMBER LIST  WHEN CLICK ON CARD/TAB**************************************************************************

        public async Task<List<AccountManager>> GetAllMemberDetails()
        {
            MSSQL db = new MSSQL();
            Dictionary<string, string> param = new Dictionary<string, string>
{
    { "@Flag", "FetchAllMemberDetailsNK" }
};

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);

            List<AccountManager> members = new List<AccountManager>();

            foreach (DataRow row in dt.Rows)
            {
                members.Add(new AccountManager
                {
                    FlatCode = row["FlatCode"].ToString(),
                    FullName = row["FullName"].ToString(),
                    Email = row["Email"].ToString(),
                    PhoneNumber = row["PhoneNumber"].ToString(),
                    PossessionDate = Convert.ToDateTime(row["PossessionDate"]),
                    Gender = row["Gender"].ToString(),
                    FamilyMemberCount = Convert.ToInt32(row["FamilyMemberCount"]),
                    NoOfVehicle = Convert.ToInt32(row["NoofVehicle"]),
                    RegisterationDate = Convert.ToDateTime(row["RegisterationDate"])
                });
            }

            return members;
        }



        //*FOR  COUNT ALL  WORKERS IN  SOCIETY AND SHOW IN CARD/TAB**************************************************************************

        public async Task<int> GetTotalWorkerCount()
        {
            MSSQL db = new MSSQL();
            Dictionary<string, string> param = new Dictionary<string, string>
        {
            { "@Flag", "CountWorkerNK" }

        };

            object result = await db.ExecuteStoreProcedureReturnObj("sp_SMS", param);
            return result != null ? Convert.ToInt32(result) : 0;
        }


        //*SHOW ALL Worker LIST  WHEN CLICK ON CARD/TAB**************************************************************************
        public async Task<List<AccountManager>> GetAllWorkerDetails()
        {
            MSSQL db = new MSSQL();
            Dictionary<string, string> param = new Dictionary<string, string>
{
    { "@Flag", "FetchAllWorkerDetailsNK" }
};

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);

            List<AccountManager> workers = new List<AccountManager>();

            foreach (DataRow row in dt.Rows)
            {
                workers.Add(new AccountManager
                {
                    SubTypeName = row["SubTypeName"].ToString(),
                    WorkerName = row["WorkerName"].ToString(),
                    WorkerContactNo = row["WorkerContactNo"].ToString(),
                    JoiningDate = Convert.ToDateTime(row["JoiningDate"]),
                    RegisterDate = Convert.ToDateTime(row["RegisterDate"]),
                    Address = row["Address"].ToString(),
                    BaseSalary = row["BaseSalary"] != DBNull.Value ? Convert.ToDecimal(row["BaseSalary"]) : 0,

                });
            }

            return workers;
        }


        //*FOR  COUNT ALL  Vendor IN  SOCIETY AND SHOW IN CARD/TAB**************************************************************************

        public async Task<int> GetTotalVendorCount()
        {
            MSSQL db = new MSSQL();
            Dictionary<string, string> param = new Dictionary<string, string>
        {
            { "@Flag", "CountVendorNK" }

        };

            object result = await db.ExecuteStoreProcedureReturnObj("sp_SMS", param);
            return result != null ? Convert.ToInt32(result) : 0;
        }


        //*SHOW ALL Vendor LIST  WHEN CLICK ON CARD/TAB**************************************************************************
        public async Task<List<AccountManager>> GetAllVendorDetails()
        {
            MSSQL db = new MSSQL();
            Dictionary<string, string> param = new Dictionary<string, string>
{
    { "@Flag", "FetchAllVendorDetailsNK" }
};

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);

            List<AccountManager> vendors = new List<AccountManager>();

            foreach (DataRow row in dt.Rows)
            {
                vendors.Add(new AccountManager
                {
                    VendorName = row["VendorName"].ToString(),
                    SubTypeName = row["SubTypeName"].ToString(),
                    Email = row["Email"].ToString(),
                    PhoneNumber = row["PhoneNumber"].ToString(),
                    Address = row["Address"].ToString(),
                    RegistrationDate = row["RegistrationDate"].ToString()

                });
            }

            return vendors;
        }



        //METHOD  FOR PIE CHART OF DIRECT AND INDIRECT EXPENCE**********************************************************  




        public async Task<List<AccountManager>> GetMonthlyExpensePieDataAsync(int month, int year)
        {
            MSSQL db = new MSSQL();
            Dictionary<string, string> param = new Dictionary<string, string>
    {
        { "@Month", month.ToString() },
        { "@Year", year.ToString() },
        { "@Flag", "ShowpiechartDirectIndirectNK" }
    };

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);

            List<AccountManager> list = new List<AccountManager>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AccountManager
                {
                    ExpenseType = row["ExpenseType"].ToString(),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    Percentage = Convert.ToDecimal(row["Percentage"])
                });
            }

            return list;
        }


        //For LIST OF dIRECT INDIRECT GRAPH WHEN CLICK ON EACH SLICE*************************

        public async Task<List<AccountManager>> GetExpenseDetailsByType(int expenseTypeId, int month, int year)
        {
            Dictionary<string, string> para = new Dictionary<string, string>()
{
    { "@flag", "ShowListDirectIndirectNK" },
    { "@ExpenseTypeId", expenseTypeId.ToString() },
    { "@Month", month.ToString() },
    { "@Year", year.ToString() }
};

            MSSQL db = new MSSQL();
            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", para);

            return dt.AsEnumerable().Select(row => new AccountManager
            {
                ExpenseTypeName = row["ExpenseTypeName"].ToString(),
                ExpenseName = row["ExpenseName"].ToString(),
                WingName = row["WingName"].ToString(),
                PaidTo = row["PaidTo"].ToString(),
                Description = row["Description"].ToString(),
                ExpenseDate = DateTime.TryParse(row["ExpenseDate"]?.ToString(), out DateTime tempDate)
                                ? tempDate
                                : (DateTime?)null,
                Amount = Convert.ToDecimal(row["Amount"])
            }).ToList();
        }

        public async Task<List<AccountManager>> GetMonthlyTotalWorkerSalaryAsync()
        {
            MSSQL db = new MSSQL();
            Dictionary<string, string> param = new Dictionary<string, string>
{
    { "@Flag", "GetWorkerSalaryTotalPerMonth" }
};

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);

            List<AccountManager> list = new List<AccountManager>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AccountManager
                {
                    MonthName = row["MonthName"].ToString(),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"])
                });
            }

            return list;
        }

        //for  show list of worker payment when click on column********************

        public async Task<List<AccountManager>> GetWorkerSalaryListByMonthAsync(int month, int year)
        {
            MSSQL db = new MSSQL();
            Dictionary<string, string> param = new Dictionary<string, string>
{
    { "@Flag", "GetWorkerSalaryListByMonth" },
    { "@Month", month.ToString() },
    { "@Year", year.ToString() }
};

            DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", param);

            return dt.AsEnumerable().Select(row => new AccountManager
            {
                WorkerName = row["WorkerName"].ToString(),
                SubTypeName = row["SubTypeName"].ToString(),
                WorkerContactNo = row["WorkerContactNo"].ToString(),
                Address = row["Address"].ToString(),
                Amount = Convert.ToDecimal(row["Amount"]),
                PaidDate = Convert.ToDateTime(row["PaidDate"])
            }).ToList();
        }








        //VIRAJ   BALLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL  10-7-2025


        // 🔹 1. For Bar Chart: Event Budget by Month
        public async Task<List<AccountManager>> GetBudgetVsActualDataAsync(int month, int year)
        {
            MSSQL obj = new MSSQL();
            var result = new List<AccountManager>();

            var parameters = new Dictionary<string, string>
 {
     { "@flag", "eventbudgetbymonth" },
     { "@Month", month.ToString() },
     { "@Year", year.ToString() },
     { "@Email", "" },
     { "@Password", "" }
 };

            using (SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("sp_SMS", parameters))
            {
                while (dr.Read())
                {
                    var fromDate = dr["FromDate"] != DBNull.Value ? Convert.ToDateTime(dr["FromDate"]) : DateTime.MinValue;

                    result.Add(new AccountManager
                    {
                        EventName = dr["EventName"].ToString(),
                        AllocatedBudget = Convert.ToDecimal(dr["AllocatedBudget"]),
                        ActualCost = Convert.ToDecimal(dr["ActualCost"]),
                        FromDate = fromDate,
                        Month = fromDate.Month,
                        Year = fromDate.Year,
                        MonthYear = fromDate.ToString("MMMM yyyy")
                    });
                }
            }

            return result;
        }

        // 🔹 2. For Modal: All Event Budget Details
        public async Task<List<AccountManager>> GetAllEventBudgetDetailsAsync()
        {
            MSSQL obj = new MSSQL();
            var result = new List<AccountManager>();

            var parameters = new Dictionary<string, string>
 {
     { "@flag", "eventbudgetdetails" },
     { "@Email", "" },
     { "@Password", "" }
 };

            using (SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("sp_SMS", parameters))
            {
                while (dr.Read())
                {
                    var fromDate = Convert.ToDateTime(dr["FromDate"]);

                    result.Add(new AccountManager
                    {
                        EventName = dr["EventName"].ToString(),
                        AllocatedBudget = Convert.ToDecimal(dr["AllocatedBudget"]),
                        ActualCost = Convert.ToDecimal(dr["ActualCost"]),
                        FromDate = fromDate,
                        Month = fromDate.Month,
                        Year = fromDate.Year,
                        MonthYear = fromDate.ToString("MMMM yyyy")
                    });
                }
            }

            return result;
        }

        // 🔹 3. For Line Chart: Monthly Income vs Expense for a Year
        public async Task<List<AccountManager>> GetMonthlyIncomeExpenseAsync(int year)
        {
            MSSQL obj = new MSSQL();
            var result = new List<AccountManager>();

            var parameters = new Dictionary<string, string>
 {
     { "@flag", "incomeexpensechart" },
     { "@Year", year.ToString() },
     { "@Email", "" },
     { "@Password", "" }
 };

            using (SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("sp_SMS", parameters))
            {
                while (dr.Read())
                {
                    result.Add(new AccountManager
                    {
                        MonthName = dr["MonthName"].ToString(),
                        MonthNumber = Convert.ToInt32(dr["MonthNumber"]),
                        Year = Convert.ToInt32(dr["Year"]),
                        TotalIncome = Convert.ToDecimal(dr["TotalIncome"]),
                        TotalExpense = Convert.ToDecimal(dr["TotalExpense"]),
                        MonthYear = $"{dr["MonthName"]} {dr["Year"]}"
                    });
                }
            }

            return result;
        }

        // 🔹 4. For Modal: Income vs Expense Details by Month/Year
        public async Task<List<AccountManager>> GetIncomeExpenseDetailsByMonthAsync(int month, int year)
        {
            MSSQL obj = new MSSQL();
            var result = new List<AccountManager>();

            var parameters = new Dictionary<string, string>
 {
     { "@flag", "incomeexpensedetails" },
     { "@Month", month.ToString() },
     { "@Year", year.ToString() },
     { "@Email", "" },
     { "@Password", "" }
 };

            using (SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("sp_SMS", parameters))
            {
                while (dr.Read())
                {
                    result.Add(new AccountManager
                    {
                        MonthName = dr["MonthName"].ToString(),
                        TypeLabel = dr["TypeLabel"].ToString(),
                        Amount = Convert.ToDecimal(dr["Amount"]),
                        PaymentPurpose = dr["PaymentPurpose"].ToString()
                    });
                }
            }

            return result;
        }



        #endregion


    }

}



