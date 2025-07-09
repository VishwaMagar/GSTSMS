using GSTSMSHelper;
using GSTSMSLibrary.Account;
using GSTSMSLibrary.AccountManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace GSTSMS.Controllers
{
    public class AccountManagerController : Controller
    {
        // GET: AccountManager
        public ActionResult Index()
        {
            return View();

        }
        public ActionResult Dashboard()
        {
            return View();
        }




        #region********************************************************************* Society Ac.Details Bank ***********************************************************


        /// <summary>
        /// Fetches all bank details and passes the list to the BankList view.
        /// </summary>

        /// <returns>BankList view with a list of AccountManager objects containing bank information.</returns>
        public async Task<ActionResult> BankDetailsListSS()
        {
            BALAccountManager bal = new BALAccountManager();
            List<AccountManager> list = await bal.GetAllBankDetailsSS();
            return View(list);  // Strongly typed model passed
        }

        /// <summary>
        /// Create view View BankDetails of Transaction
        /// </summary>
        /// <param name="bankCode"></param>
        /// <returns></returns>
        public async Task<ActionResult> ViewBankDetails(string bankCode)
        {
            BALAccountManager bal = new BALAccountManager();
            var allBanks = await bal.GetAllBankDetailsSS();
            var selectedBank = allBanks.FirstOrDefault(b => b.BankCode == bankCode);

            if (selectedBank == null)
                return HttpNotFound("Bank not found");

            selectedBank.TransactionList = await bal.GetTransactionStatementByBankMS(bankCode);

            return View(selectedBank);
        }

        /// <summary>
        /// get the Add Bank Society form generate view
        ///  04/07/2025
        ///  MS
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> AddBankSociety()
        {
            BALAccountManager bal = new BALAccountManager();
            ViewBag.AccountTypes = await GetAccountTypesAsync();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddBankSociety(AccountManager obj)
        {
            BALAccountManager bal = new BALAccountManager();

            if (ModelState.IsValid)
            {
                await bal.AddBankSocietyMS(obj);
                TempData["msg"] = "Bank account added successfully!";
                return RedirectToAction("AddBankSociety");
            }

            ViewBag.AccountTypes = await GetAccountTypesAsync();
            return View(obj);
        }

        public async Task<List<SelectListItem>> GetAccountTypesAsync()
        {
            BALAccountManager bal = new BALAccountManager();
            DataSet ds = await bal.FetchAccountTypeMS();
            List<SelectListItem> list = new List<SelectListItem>();

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new SelectListItem
                    {
                        Value = row["SubTypeId"].ToString(),
                        Text = row["SubTypeName"].ToString()
                    });
                }
            }

            return list;
        }




#endregion



        #region********************************************************************* Maintaince Management ***********************************************************


        BALAccountManager bal = new BALAccountManager();
        //This is for to fetch member maintainence list 
        public async Task<ActionResult> MaintenanceManagementSY()
        {
            var data = await bal.GetMemberMaintenanceListSY();
            return View(data);
        }







        //This is for fetch member maintainance details 
        [HttpPost]
        public async Task<ActionResult> MemberMaintenanceDetailsSY(string memberCode, string maintainanceTypeId, string EntityCode)
        {
            var list = await bal.GetMemberMaintenanceDetailsSY(memberCode, maintainanceTypeId, EntityCode);
            return PartialView("_MemberMaintenanceDetailsSY", list);
        }


#endregion


        #region********************************************************************* Community Send Email ***********************************************************
        public async Task<ActionResult> SendEMail(string wing = "All")
        {
            var bal = new BALAccountManager();
            var accountant = await bal.GetAccountantDetailsVM();

            var helper = new EmailHelper();

            string defaultBody = helper.SendEmailAccountant(accountant.FromEmailAddress, accountant.ContactNumber);

            var model = new AccountManager
            {
                FromEmailAddress = accountant.FromEmailAddress,
                ContactNumber = accountant.ContactNumber,
                EmailBodyMessage = defaultBody
            };

            ViewBag.SelectedWing = wing;

            var members = await bal.GetMembersVM(wing);
            ViewBag.MemberList = members;

            ViewBag.Message = TempData["Message"];
            return View(model);
        }

        // POST: SendEMail
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> SendEMail(AccountManager model, HttpPostedFileBase PostedFile)
        {
            var results = new List<string>();

            var bal = new BALAccountManager();

            if (model.ToEmailAddresses != null && model.ToEmailAddresses.Any())
            {
                foreach (var email in model.ToEmailAddresses)
                {
                    model.ToEmailAddress = email;

                    // Ensure FromEmailAddress and ContactNumber are set correctly from DB
                    if (string.IsNullOrEmpty(model.FromEmailAddress) || string.IsNullOrEmpty(model.ContactNumber))
                    {
                        var accountant = await bal.GetAccountantDetailsVM();
                        model.FromEmailAddress = accountant.FromEmailAddress;
                        model.ContactNumber = accountant.ContactNumber;
                    }

                    var result = await bal.SendEMailVM(model, PostedFile);
                    results.Add(result);
                }

                TempData["Message"] = string.Join("<br/>", results);
            }
            else
            {
                TempData["Message"] = "❌ No members were selected.";
            }

            string selectedWing = Request["wing"] ?? "All";
            ViewBag.SelectedWing = selectedWing;

            var memberList = await bal.GetMembersVM(selectedWing);
            foreach (var member in memberList)
            {
                member.IsSelected = model.ToEmailAddresses != null && model.ToEmailAddresses.Contains(member.ToEmailAddress);
            }
            ViewBag.MemberList = memberList;

            return View(model);
        }


        // Helper method (used in WhatsApp or fallback)
        public async Task<List<AccountManager>> GetMembers(string wing, List<int> memberIds = null)
        {
            var bal = new BALAccountManager(); // ✅ FIXED
            return await bal.GetMembersVM(wing, memberIds);
        }

        

        #endregion



        #region********************************************************************* Community Notice  ***********************************************************

        /// <summary>
        /// Retrieves all notice announcements and passes them to the NoticeList view.
        /// </summary>
        /// 
        /// <returns>NoticeList view with a list of notices.</returns>

        public async Task<ActionResult> NoticeList()
        {
            BALAccountManager bal = new BALAccountManager();
            List<AccountManager> list = await bal.GetAllNoticeListSS();
            return View(list);
        }


        //public async Task<ActionResult> NoticeList()
        //{
        //    var bal = new BALAccountManager();
        //    ViewBag.WorkerTable = await bal.ShowData();
        //    return View();
        //}

        public async Task<ActionResult> AddNotice()
        {
            var model = new AccountManager
            {
                SendBy = "STF002",
                FromEmailAddress = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"],
                Subject = "New Notice from Housing Society",
                EmailBodyMessage = "Dear Member,<br/><br/>Please find the latest notice.<br/><br/>Thanks,<br/>Green Valley Society"
            };

            ViewBag.SelectedWing = "All";
            ViewBag.MemberList = await GetMembers("All");
            return PartialView("_AddNotice", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> AddNotice(AccountManager model)
        {
            try
            {
                model.AttachmentList = new List<string>();
                model.SelectedMemberCodes = new List<string>();

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string folderPath = Server.MapPath("~/Uploads/Notices");
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        string fullPath = Path.Combine(folderPath, fileName);
                        file.SaveAs(fullPath);

                        model.AttachmentList.Add(fileName);
                        model.Document = fileName;
                    }
                }

                model.ToEmailAddresses = Request.Form.GetValues("ToEmailAddresses")?.ToList() ?? new List<string>();
                model.SelectedMemberCodes = new List<string>();

                if (model.ToEmailAddresses.Any())
                {
                    var allMembers = await GetMembers("All");
                    foreach (var email in model.ToEmailAddresses)
                    {
                        var member = allMembers.FirstOrDefault(m => m.ToEmailAddress == email);
                        if (member != null)
                        {
                            model.SelectedMemberCodes.Add(member.MemberCode);
                        }
                    }
                }

                model.Subject = "Notice: " + model.NoticeTitle;
                model.SendBy = "STF002";
                model.CreatedDate = DateTime.Now;

                var bal = new BALAccountManager();
                string noticeCode = await bal.SaveNoticeRM(model);

                //if (model.SelectedMemberCodes != null && model.SelectedMemberCodes.Count > 0)
                //    await bal.InsertNoticeLogsByMembers(noticeCode, model.SelectedMemberCodes);

                string mailStatus = await bal.SendEmailRM(model);

                TempData["Message"] = "✅ Notice saved. " + mailStatus;
                return RedirectToAction("NoticeList");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.SelectedWing = model.WingName ?? "All";
                ViewBag.MemberList = await GetMembers(ViewBag.SelectedWing);
                return PartialView("_AddNotice", model);
            }
        }

        //public async Task<List<AccountManager>> GetMembers(string wing, List<int> memberIds = null)
        //{
        //    var bal = new BALAccountManager();
        //    return await bal.GetMembersRM(wing, memberIds);
        //}

        public async Task<ActionResult> EditNotice(string noticeCode)
        {
            var bal = new BALAccountManager();
            var dt = await bal.GetNoticeByCodeRM(noticeCode);

            if (dt.Rows.Count == 0)
                return HttpNotFound();

            var row = dt.Rows[0];
            var model = new AccountManager
            {
                NoticeCode = row["NoticeAnnouncementCode"].ToString(),
                NoticeTitle = row["NoticeTitle"].ToString(),
                Description = row["Description"].ToString(),
                PublishDate = Convert.ToDateTime(row["PublishDate"]),
                EndDate = Convert.ToDateTime(row["EndDate"]),
                Document = row["Document"]?.ToString()
            };

            return PartialView("_EditNoticeModal", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateNotice(AccountManager model, HttpPostedFileBase PostedFile)
        {
            try
            {
                if (PostedFile != null && PostedFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(PostedFile.FileName);
                    string folderPath = Server.MapPath("~/Uploads/Notices");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string fullPath = Path.Combine(folderPath, fileName);
                    PostedFile.SaveAs(fullPath);

                    model.Document = fileName;
                }

                var bal = new BALAccountManager();
                await bal.UpdateNotice(model);

                TempData["Message"] = "✅ Notice updated successfully.";
                return RedirectToAction("NoticeList");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ " + ex.Message;
                return RedirectToAction("NoticeList");
            }
        }


        #endregion



        #region********************************************************************* Expence***********************************************************



        /// <summary>
        /// Fetches all expenses and displays them on the Index view.
        /// Purpose: Lists all expenses with basic details for the user.
        /// Date: 03-July-2025
        /// </summary>
        public async Task<ActionResult> ExpenseList()
        {
            BALAccountManager bal = new BALAccountManager();
            List<AccountManager> expenses = await bal.GetAllExpenses();
            return View(expenses); // Strongly typed list
        }



        /// <summary>
        /// Saves payment details such as Payment ID and detected Razorpay method (UPI, Netbanking, etc.).
        /// Purpose: Invoked via AJAX after successful Razorpay payment to persist data.
        /// Date: 03-July-2025
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> SavePayment(string expenseCode, string paymentId, int paymentModeId = 0)
        {
            int finalPaymentMode = GetPaymentModeFromRazorpay(paymentId);

            if (finalPaymentMode == 0) // fallback if API fails
                finalPaymentMode = paymentModeId;

            BALAccountManager bal = new BALAccountManager();
            bool result = await bal.SaveTransactionPS(expenseCode, paymentId, finalPaymentMode);
            return Json(new { success = result });
        }




        /// <summary>
        /// Contacts Razorpay API to determine the payment method (UPI, NetBanking, etc.) for a given Payment ID.
        /// Purpose: Used internally to determine the mode of payment made through Razorpay.
        /// Date: 03-July-2025
        /// </summary>
        private int GetPaymentModeFromRazorpay(string paymentId)
        {
            try
            {
                var key = "rzp_test_tnu8pNChRc5VBE";
                var secret = "wVSn4S0P2BpbvIiol3zLUzLG";

                var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{key}:{secret}"));
                var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create($"https://api.razorpay.com/v1/payments/{paymentId}");
                request.Headers["Authorization"] = "Basic " + credentials;
                request.Method = "GET";

                using (var response = (System.Net.HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new System.IO.StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    var json = Newtonsoft.Json.Linq.JObject.Parse(result);
                    string method = json["method"]?.ToString();

                    if (method == "upi") return 33;
                    if (method == "netbanking") return 35;
                }
            }
            catch (Exception )
            {
                // Log error here
            }

            return 0; // unknown or failed to fetch
        }



        /// <summary>
        /// Retrieves full details for a specific expense and returns a partial view (modal) to display them.
        /// Purpose: Opens detailed expense info in a modal for user review.
        /// Date: 03-July-2025
        /// </summary>
        /// 


        [HttpGet]
        public async Task<PartialViewResult> GetExpenseDetails(string expenseCode)
        {
            var details = await new BALAccountManager().GetFullExpenseDetailsPS(expenseCode);
            return PartialView("_ExpenseDetailsModal", details);
        }

        /// <summary>
        /// Retrieves IFSC-related information (vendor, IFSC code, etc.) for a given expense.
        /// Purpose: Opens a modal to view or use bank details related to the vendor.
        /// Date: 03-July-2025
        /// </summary>

        [HttpGet]
        public async Task<PartialViewResult> GetIFSCDetails(string expenseCode)
        {
            BALAccountManager bal = new BALAccountManager();
            AccountManager data = await bal.GetIFSCByCodePS(expenseCode);
            return PartialView("_IFSCModal", data);
        }





        [HttpGet]
        public async Task<ActionResult> RegisterVendor()
        {
            var bal = new BALAccountManager();
            ViewBag.ServiceSubTypes = await bal.GetServiceSubTypes();
            return PartialView("_RegisterVendor", new AccountManager());
        }

        [HttpPost]
        public async Task<ActionResult> RegisterVendor(AccountManager model)
        {
            try
            {
                var doc1 = Request.Files["Document1File"];
                var doc2 = Request.Files["Document2File"];

                if (doc1 != null && doc1.ContentLength > 0)
                {
                    var name1 = Path.GetFileName(doc1.FileName);
                    var path1 = Path.Combine(Server.MapPath("~/Content/Documents/"), name1);
                    doc1.SaveAs(path1);
                    model.Document1 = "~/Content/Documents/" + name1;
                }
                if (doc2 != null && doc2.ContentLength > 0)
                {
                    var name2 = Path.GetFileName(doc2.FileName);
                    var path2 = Path.Combine(Server.MapPath("~/Content/Documents/"), name2);
                    doc2.SaveAs(path2);
                    model.Document2 = "~/Content/Documents/" + name2;
                }

                bool result = await new BALAccountManager().RegisterVendor(model);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }














        [HttpGet]
        public async Task<PartialViewResult> AddExpensePage()
        {
            ViewBag.ExpenseTypeList = GetExpenseTypeList();
            ViewBag.VendorTypeList = await bal.GetVendorTypesAsync();
            ViewBag.VendorList = await GetVendorListAsync();
            ViewBag.WingList = await GetWingListAsync();
            ViewBag.VendorType = await GetVendorTypeListAsync();
            // ViewBag.GSTList = await GetGSTTypeListAsync();
            ViewBag.GSTList = await GetGSTTypeListAsync();


            return PartialView("_AddExpensePage", new AccountManager());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateExpense(AccountManager model, HttpPostedFileBase UploadedFile)
        {
            if (ModelState.IsValid)
            {
                model.AddedBy = "STF002";
                model.Date = (model.Date < new DateTime(1753, 1, 1)) ? DateTime.Now : model.Date;

                if (UploadedFile != null && UploadedFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(UploadedFile.FileName);
                    string folderPath = Server.MapPath("~/Content/Documents/");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                    string filePath = Path.Combine(folderPath, fileName);
                    UploadedFile.SaveAs(filePath);
                    model.DocumentBase64String = "/Content/Documents/" + fileName;
                }

                // 🔁 Get ExpenseCode after insert
                string expenseCode = await bal.InsertExpenseAsync(model);

                if (!string.IsNullOrEmpty(expenseCode))
                {
                    TempData["Success"] = "Expense inserted successfully. Code: " + expenseCode;
                    return RedirectToAction("ExpenseList");
                }
                TempData["Error"] = "Insertion failed.";
            }

            ViewBag.ExpenseTypeList = GetExpenseTypeList();
            ViewBag.VendorType = await GetVendorTypeListAsync();
            ViewBag.VendorList = await GetVendorListAsync();
            ViewBag.WingList = await GetWingListAsync();
            ViewBag.GSTList = await GetGSTTypeListAsync();
            return View("AddExpensePage", model);
        }

        public List<SelectListItem> GetExpenseTypeList()
        {
            return new List<SelectListItem>
     {
         new SelectListItem { Text = "Direct", Value = "36" },
         new SelectListItem { Text = "Indirect", Value = "37" }
     };
        }

        public async Task<List<SelectListItem>> GetVendorListAsync()
        {
            var vendorList = await bal.GetVendorNamesAsync();
            return vendorList.Select(v => new SelectListItem { Value = v.VendorCode, Text = v.VendorName }).ToList();
        }

        public async Task<List<SelectListItem>> GetWingListAsync()
        {
            var wingList = await bal.GetWingNamesAsync();
            return wingList.Select(w => new SelectListItem { Value = w.WingId.ToString(), Text = w.WingName }).ToList();
        }

        public async Task<List<SelectListItem>> GetGSTTypeListAsync()
        {
            var gstList = await bal.GetGSTTypeAsync();
            return gstList.Select(g => new SelectListItem
            {
                Value = g.GSTTypeId.ToString(),
                Text = g.GSTTypeName
            }).ToList();
        }


        public async Task<List<SelectListItem>> GetVendorTypeListAsync()
        {
            var vendorTypes = await bal.GetVendorTypeAsync();
            return vendorTypes.Select(g => new SelectListItem
            {
                Value = g.SubTypeId.ToString(),
                Text = g.SubTypeName
            }).ToList();
        }

        [HttpPost]
        public async Task<JsonResult> GetVendorsByType(int vendorType)
        {
            var vendorList = await bal.GetVendorsByTypeAsync(vendorType);
            return Json(vendorList);
        }



        #endregion


    }
}