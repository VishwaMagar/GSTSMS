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

namespace GSTSMSLibrary.AccountManager
{
    public class BALAccountManager

    {
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



        #region********************************************************************* Maintaince Management ***********************************************************


        MSSQL db = new MSSQL();

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

    }


}
