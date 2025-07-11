using GSTSMSHelper;
using GSTSMSLibrary.Account;
using GSTSMSLibrary.AccountManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static GSTSMSLibrary.AccountManager.AccountManager;


namespace GSTSMS.Controllers
{
    public class AccountManagerController : Controller
    {
        // GET: AccountManager
        public ActionResult Index()
        {
            return View();

        }
        //public ActionResult Dashboard()
        //{
        //    return View();
        //}

        BALAccountManager objDashbaord = new BALAccountManager();

        #region********************************************************************* Dashboard ***********************************************************








        public async Task<ActionResult> Dashboard()
        {


            decimal dueMaintenance = await objDashbaord.GetDueMaintenanceAmount();
            ViewBag.MaintenanceDue = dueMaintenance;

            decimal cashInHand = await objDashbaord.CashinHand();
            ViewBag.CashInHand = cashInHand;

            int TotalComplaints = await objDashbaord.TotalComplaints();
            ViewBag.TotalComplaints = TotalComplaints;

            int PendingComplaints = await objDashbaord.PendingComplaints();
            ViewBag.PendingComplaints = PendingComplaints;

            int SolvedComplaints = await objDashbaord.SolvedComplaints();
            ViewBag.SolvedComplaints = SolvedComplaints;

            decimal TotalBankBalance = await objDashbaord.TotalBankBalance();
            ViewBag.TotalBankBalance = TotalBankBalance;



            DataSet ds = await objDashbaord.BankBalance();


            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var dt = ds.Tables[0];


                foreach (DataRow row in dt.Rows)
                {
                    string bankName = row["BankShortCode"].ToString();
                    decimal amount = Convert.ToDecimal(row["OpeningBalance"]);

                    if (bankName == "KKBK")
                    {

                        ViewBag.KKBK = amount;
                    }
                    else if (bankName == "HDFC")
                    {
                        ViewBag.HDFC = amount;
                    }
                }
            }


            return View();



        }








        [HttpGet]
        public async Task<JsonResult> GetEventExpenses(string month)
        {
            try
            {
                var parameters = new Dictionary<string, string>
       {
           { "@Flag", "EventExpens" },
           { "@Month", month }
       };

                MSSQL db = new MSSQL();
                DataTable dt = await db.ExecuteStoreProcedureReturnDataTable("sp_SMS", parameters);

                var labels = new List<string>();
                var allocated = new List<decimal>();
                var actual = new List<decimal>();

                foreach (DataRow row in dt.Rows)
                {
                    labels.Add(row["EventName"].ToString());
                    allocated.Add(Convert.ToDecimal(row["TotalAllocatedBudget"]));
                    actual.Add(Convert.ToDecimal(row["TotalActualCost"]));
                }

                return Json(new
                {
                    labels = labels,
                    allocated = allocated,
                    actual = actual
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = true,
                    message = "An error occurred while loading chart data.",
                    details = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetWingNames()
        {
            var wingsDict = await new BALAccountManager().GetWingListAsync();

            // Convert Dictionary<int, string> to List of anonymous objects for JS
            var wingList = wingsDict.Select(kvp => new { WingId = kvp.Key, WingName = kvp.Value }).ToList();

            return Json(wingList, JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        public async Task<JsonResult> GetTop5RedListMembers()
        {
            var result = new Dictionary<string, object>();
            try
            {
                var bal = new BALAccountManager();
                var data = await bal.GetTop5RedListMembers();

                result["status"] = "success";
                result["data"] = data;
            }
            catch (Exception ex)
            {
                result["status"] = "error";
                result["message"] = "Failed to fetch red list members.";
                result["details"] = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }





        [HttpGet]
        public async Task<JsonResult> GetBankTransactionSummary(int month)
        {
            var result = new Dictionary<string, object>();

            try
            {
                var bal = new BALAccountManager();
                var data = await bal.GetMonthlyTransactionSummary(month);

                result["status"] = "success";
                result["data"] = data;
            }
            catch (Exception ex)
            {
                result["status"] = "error";
                result["message"] = "Error fetching transaction summary.";
                result["details"] = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<JsonResult> GetWorkerPaymentChart(int month)
        {
            try
            {
                var bal = new BALAccountManager();
                var chartData = await bal.GetWorkerPaymentChartAsync(month); // returns Dictionary<string, int> with keys "Completed" and "Pending"
                return Json(chartData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Completed = 0, Pending = 0 }, JsonRequestBehavior.AllowGet);
            }
        }








        [HttpGet]
        public async Task<JsonResult> GetMaintenanceStatusChart(int month, string wing)
        {
            try
            {
                var bal = new BALAccountManager();
                var result = await bal.GetMaintenanceStatusChartAsync(month, wing);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Paid = 0,
                    Unpaid = 0,
                    TotalAmount = 0,
                    PendingAmount = 0,
                    Error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }















        #endregion





        #region********************************************************************* Society Ac.Details Bank ***********************************************************


        public async Task<ActionResult> BankDetailsListSS()
        {
            BALAccountManager bal = new BALAccountManager();
            List<AccountManager> list = await bal.GetAllBankDetailsSS();
            return View(list);  // Strongly typed model passed
        }
        /// <summary>
        /// Handles the POST request to disable a bank account by its ID.
        /// Calls the business logic layer to perform the disable operation.
        /// </summary>
        /// 
        /// <param name="bankId">
        /// The unique identifier of the bank account to disable.
        /// </param>
        /// 
        /// <returns>
        /// A JSON result indicating whether the operation was successful (`success:

        [HttpPost]
        public async Task<JsonResult> DisableBank(int bankId)
        {
            BALAccountManager bal = new BALAccountManager();
            bool isDisabled = await bal.DisableBankAccountAsyncSS(bankId);
            return Json(new { success = isDisabled });
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
        // GET: Load form partial
        // GET

        [HttpGet]
        public async Task<PartialViewResult> GetAddBankSocietyMS()
        {
            var model = new AccountManager();
            ViewBag.AccountTypeList = await GetAccountTypesAsync();
            return PartialView("_GetAddBankSocietyMS", model);
        }


        //[HttpPost]
        //public async Task<ActionResult> GetAddBankSocietyMS(AccountManager model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.AccountTypeList = await GetAccountTypesAsync(); // 🧠 retain list on postback
        //        return View("_GetAddBankSocietyMS", model);
        //    }

        //    var result = await new BALAccountManager().AddBankSocietyMS(model);
        //    if (result)
        //        return Content("success");

        //    ViewBag.AccountTypeList = await GetAccountTypesAsync(); // if error, reload list again
        //    return View("_GetAddBankSocietyMS", model);
        //}


        [HttpPost]
        public async Task<ActionResult> GetAddBankSocietyMS(AccountManager model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AccountTypeList = await GetAccountTypesAsync();
                return PartialView("_GetAddBankSocietyMS", model);
            }

            var result = await new BALAccountManager().AddBankSocietyMS(model);
            if (result)
                return Json("success", JsonRequestBehavior.AllowGet); // ✅ SweetAlert compatible

            ViewBag.AccountTypeList = await GetAccountTypesAsync();
            return Json("fail", JsonRequestBehavior.AllowGet); // ✅ SweetAlert compatible
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




        #region********************************************************************* Society Ac.Details Cash ***********************************************************



        private static List<AccountManager> _transactionCache;

        public async Task<ActionResult> CashTansactionListDD(string type = "Credited")
        {
            BALAccountManager bal = new BALAccountManager();
            _transactionCache = await bal.CashTransactionDD();

            var filtered = string.IsNullOrEmpty(type) ? _transactionCache
                : _transactionCache.Where(t => t.TransactionNature.Equals(type, StringComparison.OrdinalIgnoreCase)).ToList();

            return View(filtered);
        }

        public ActionResult GetTransactionDetails(int id)
        {
            var transaction = _transactionCache?.FirstOrDefault(t => t.TransactionId == id);
            if (transaction == null)
                return HttpNotFound();

            return PartialView("_ViewTransactionPartialDD", transaction);
        }


        //////////////////////////////
        ///

        //////////////////////////// Shruti Mane////////////////////////////////////////////////////


        BALAccountManager objACC = new BALAccountManager();

       

        [HttpGet]
        public ActionResult CashTransaction()
        {
            return View();
        }

        // <summary>
        /// Returns the partial view for the Cash Transaction form.
        /// Initializes the receiver dropdown and transaction type list.
        /// </summary>
        public async Task<PartialViewResult> CashTransactionPage()
        {
            await PopulateTransactionList();
            ViewBag.ReceiverList = new List<SelectListItem>();
            return PartialView("_CashTransactionPage", new AccountManager());
        }



        /// <summary>
        /// Saves a cash or cheque transaction including file upload, 
        /// logic for credit/debit, and entity assignment. 
        /// Returns transaction metadata as JSON.
        /// </summary>
        /// <param /name="objprop"/>AccountManager transaction data</param>



        /// <summary>
        /// Handles file upload, sets PaymentBy/PaidTo/EntityCode, saves transaction data,
        /// and updates expense or event payment status if applicable.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> SaveCashTransaction(AccountManager objprop)
        {
            try
            {
                // 1. Handle file upload (PDF only, max 2MB)
                HttpPostedFileBase file = Request.Files["AttachmentPath"];
                if (file != null && file.ContentLength > 0)
                {
                    string ext = Path.GetExtension(file.FileName);
                    if (!ext.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                        return Json(new { success = false, message = "Only PDF files are allowed." });

                    if (file.ContentLength > 2 * 1024 * 1024)
                        return Json(new { success = false, message = "File must be under 2MB." });

                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string uniqueName = fileName + "_" + DateTime.Now.Ticks + ext;

                    string physicalPath = Server.MapPath("~/Content/Documents/TransactionReceipts/");
                    if (!Directory.Exists(physicalPath))
                        Directory.CreateDirectory(physicalPath);

                    string savedPath = Path.Combine(physicalPath, uniqueName);
                    file.SaveAs(savedPath);

                    objprop.AttachmentPath = $"/Content/Documents/TransactionReceipts/{uniqueName}";
                }

                // 2. Set PaymentBy, PaidTo, EntityCode
                if (objprop.TransactionId == 26) // Credit
                {
                    objprop.PaymentBy = objprop.MemberCode;
                    objprop.EntityCode = objprop.MaintenanceCode;
                    objprop.PaidTo = "STF002";
                }
                else if (objprop.TransactionId == 27) // Debit
                {
                    objprop.PaymentBy = "STF002";
                    objprop.PaidTo = (objprop.Type == "Other") ? objprop.OtherReceiver : objprop.ReceiverCode;

                    switch (objprop.Type)
                    {
                        case "Vendor":
                            objprop.EntityCode = objprop.ExpenseCode;
                            break;

                        case "EventHandler":
                            objprop.EntityCode = objprop.EventCode;
                            break;

                        case "Worker":
                            objprop.EntityCode = null;
                            break;
                    }
                }

                // 3. Save Transaction
                string transactionCode = await objACC.SaveDataAsync(objprop);

                // 4. Update Expense or Event status if applicable
                if (objprop.TransactionId == 27)
                {
                    if (objprop.Type == "Vendor" && !string.IsNullOrEmpty(objprop.ExpenseCode))
                        await objACC.MarkExpenseAsPaidAsync(objprop.ExpenseCode);

                    else if (objprop.Type == "EventHandler" && !string.IsNullOrEmpty(objprop.EventCode))
                        await objACC.MarkEventBudgetAsPaidAsync(objprop.EventCode);
                }

                // 5. Return result
                return Json(new
                {
                    success = true,
                    transactionCode = transactionCode,
                    transactionId = objprop.TransactionId,
                    receiverType = objprop.Type,
                    receiverCode = objprop.ReceiverCode
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error while saving: " + ex.Message });
            }
        }










        /// <summary>
        /// Converts a numeric amount to a string in currency format with "only".
        /// </summary>
        /// <param /name="amount"/>Amount in decimal</param>
        /// <returns>Formatted string</returns>


        private string ConvertAmountToWords(decimal amount)
        {
            // You can use your own logic or NuGet packages for better conversion.
            return amount.ToString("N2") + " only";
        }


        /// <summary>
        /// Loads receipt details and returns the Receipt PDF view for preview.
        /// </summary>
        /// <param /name="transactionCode"/>Transaction code to lookup</param>

        public async Task<ActionResult> ReceiptPdf(string transactionCode)
        {
            if (string.IsNullOrEmpty(transactionCode))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Transaction code is required.");

            // 1. Fetch the transaction data
            var dtTransaction = await objACC.GetReceiptDataAsync(transactionCode);
            if (dtTransaction == null || dtTransaction.Rows.Count == 0)
                return HttpNotFound("Transaction not found.");

            var row = dtTransaction.Rows[0];

            // 2. Load maintenance items if any
            string maintenanceCode = row["EntityCode"]?.ToString();
            List<ReceiptItem> items = new List<ReceiptItem>();

            if (!string.IsNullOrEmpty(maintenanceCode))
            {
                var dtItems = await objACC.GetMaintenanceItemsAsync(maintenanceCode);
                if (dtItems != null && dtItems.Rows.Count > 0)
                {
                    foreach (DataRow itemRow in dtItems.Rows)
                    {
                        items.Add(new ReceiptItem
                        {
                            Description = itemRow["MaintenanceId"].ToString(),
                            Amount = Convert.ToDecimal(itemRow["Amount"]),
                            IsPenalty = false // Or true if your logic requires
                        });
                    }
                }
            }

            // 3. If no items found, create a default item
            if (!items.Any())
            {
                items.Add(new ReceiptItem
                {
                    Description = row["PaymentPurpose"].ToString(),
                    Amount = Convert.ToDecimal(row["Amount"]),
                    IsPenalty = false
                });
            }

            // 4. Build the model
            var model = new ReceiptViewModel
            {
                TransactionCode = row["TransactionCode"].ToString(),
                PaidDate = Convert.ToDateTime(row["PaidDate"]).ToString("dd MMMM yyyy"),
                PaymentBy = row["FullName"].ToString(),
                PaidTo = "Housing Society",
                Amount = Convert.ToDecimal(row["Amount"]),
                AmountInWords = ConvertAmountToWords(Convert.ToDecimal(row["Amount"])),
                PaymentPurpose = row["PaymentPurpose"].ToString(),
                PaymentMode = Convert.ToInt32(row["PaymentMode"]) == 32 ? "Cash" : "Cheque",
                Items = items
            };

            return View("ReceiptPdf", model);
        }

        /// <summary>
        /// Generates and downloads a Receipt PDF for the given transaction code.
        /// Also saves the file to server's TransactionReceipts folder.
        /// </summary>
        /// <param/* name="transactionCode"*/>Transaction code</param>

        public async Task<ActionResult> ReceiptPdfDownload(string transactionCode)
        {
            if (string.IsNullOrEmpty(transactionCode))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Transaction code is required.");

            // 1. Fetch transaction data
            var dtTransaction = await objACC.GetReceiptDataAsync(transactionCode);
            if (dtTransaction == null || dtTransaction.Rows.Count == 0)
                return HttpNotFound("Transaction not found.");

            var row = dtTransaction.Rows[0];

            // 2. Load maintenance items if any
            string maintenanceCode = row["EntityCode"]?.ToString();
            List<ReceiptItem> items = new List<ReceiptItem>();

            if (!string.IsNullOrEmpty(maintenanceCode))
            {
                var dtItems = await objACC.GetMaintenanceItemsAsync(maintenanceCode);
                if (dtItems != null && dtItems.Rows.Count > 0)
                {
                    foreach (DataRow itemRow in dtItems.Rows)
                    {
                        items.Add(new ReceiptItem
                        {
                            Description = itemRow["MaintenanceId"].ToString(),
                            Amount = Convert.ToDecimal(itemRow["Amount"]),
                            IsPenalty = false
                        });
                    }
                }
            }

            // 3. If no items found, create default
            if (!items.Any())
            {
                items.Add(new ReceiptItem
                {
                    Description = row["PaymentPurpose"].ToString(),
                    Amount = Convert.ToDecimal(row["Amount"]),
                    IsPenalty = false
                });
            }

            // 4. Build the model
            var model = new ReceiptViewModel
            {
                TransactionCode = row["TransactionCode"].ToString(),
                PaidDate = Convert.ToDateTime(row["PaidDate"]).ToString("dd MMMM yyyy"),
                PaymentBy = row["FullName"].ToString(),
                PaidTo = "Housing Society",
                Amount = Convert.ToDecimal(row["Amount"]),
                AmountInWords = ConvertAmountToWords(Convert.ToDecimal(row["Amount"])),
                PaymentPurpose = row["PaymentPurpose"].ToString(),
                PaymentMode = Convert.ToInt32(row["PaymentMode"]) == 32 ? "Cash" : "Cheque",
                Items = items
            };

            // 5. Create the Rotativa PDF generator
            var pdf = new Rotativa.ViewAsPdf("ReceiptPdf", model)
            {
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = new Rotativa.Options.Margins(5, 5, 5, 5),
                CustomSwitches = "--print-media-type --disable-smart-shrinking --zoom 0.9 --no-pdf-compression",
                MinimumFontSize = 10
            };

            // 6. Generate PDF binary
            byte[] pdfBytes = pdf.BuildFile(ControllerContext);

            // 7. Save the PDF to your TransactionReceipts folder
            string folderPath = Server.MapPath("~/Content/Documents/TransactionReceipts/");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = $"Receipt_{model.TransactionCode}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string fullFilePath = Path.Combine(folderPath, fileName);
            System.IO.File.WriteAllBytes(fullFilePath, pdfBytes);

            // 8. Optionally: save the relative path to database if needed
            // objprop.AttachmentPath = "/Content/Documents/Facility/TransactionReceipts/" + fileName;

            // 9. Return the PDF as a download to the user
            return File(pdfBytes, "application/pdf", fileName);
        }


        /// <summary>
        /// Generates a receipt PDF, saves it to disk, updates the DB with the file path,
        /// and returns the result as JSON.
        /// </summary>
        /// <param /name="transactionCode"/>Transaction code to generate PDF for</param>


        [HttpPost]
        public async Task<JsonResult> GenerateReceiptPdf(string transactionCode)
        {
            if (string.IsNullOrEmpty(transactionCode))
                return Json(new { success = false, message = "Transaction code is required." });

            // 1. Fetch transaction data
            var dtTransaction = await objACC.GetReceiptDataAsync(transactionCode);
            if (dtTransaction == null || dtTransaction.Rows.Count == 0)
                return Json(new { success = false, message = "Transaction not found." });

            var row = dtTransaction.Rows[0];

            // 2. Load maintenance items if any
            string maintenanceCode = row["EntityCode"]?.ToString();
            List<ReceiptItem> items = new List<ReceiptItem>();

            if (!string.IsNullOrEmpty(maintenanceCode))
            {
                var dtItems = await objACC.GetMaintenanceItemsAsync(maintenanceCode);
                if (dtItems != null && dtItems.Rows.Count > 0)
                {
                    foreach (DataRow itemRow in dtItems.Rows)
                    {
                        items.Add(new ReceiptItem
                        {
                            Description = itemRow["MaintenanceId"].ToString(),
                            Amount = Convert.ToDecimal(itemRow["Amount"]),
                            IsPenalty = false
                        });
                    }
                }
            }

            if (!items.Any())
            {
                items.Add(new ReceiptItem
                {
                    Description = row["PaymentPurpose"].ToString(),
                    Amount = Convert.ToDecimal(row["Amount"]),
                    IsPenalty = false
                });
            }

            var model = new ReceiptViewModel
            {
                TransactionCode = row["TransactionCode"].ToString(),
                PaidDate = Convert.ToDateTime(row["PaidDate"]).ToString("dd MMMM yyyy"),
                PaymentBy = row["FullName"].ToString(),
                PaidTo = "Housing Society",
                Amount = Convert.ToDecimal(row["Amount"]),
                AmountInWords = ConvertAmountToWords(Convert.ToDecimal(row["Amount"])),
                PaymentPurpose = row["PaymentPurpose"].ToString(),
                PaymentMode = Convert.ToInt32(row["PaymentMode"]) == 32 ? "Cash" : "Cheque",
                Items = items
            };

            var pdf = new Rotativa.ViewAsPdf("ReceiptPdf", model)
            {
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = new Rotativa.Options.Margins(5, 5, 5, 5),
                CustomSwitches = "--print-media-type --disable-smart-shrinking --zoom 0.9 --no-pdf-compression",
                MinimumFontSize = 10
            };

            byte[] pdfBytes = pdf.BuildFile(ControllerContext);

            string folderPath = Server.MapPath("~/Content/Documents/TransactionReceipts/");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = $"Receipt_{model.TransactionCode}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string relativePath = "~/Content/Documents/TransactionReceipts/" + fileName;
            string fullPath = Path.Combine(folderPath, fileName);
            System.IO.File.WriteAllBytes(fullPath, pdfBytes);

            // 3. Save PDF path to the database
            var acc = new AccountManager
            {
                PaymentMode = row["PaymentMode"]?.ToString(),
                TransactionCode = model.TransactionCode,
                AttachmentPath = relativePath
            };

            string saveResult = await objACC.SaveReceiptpdfpathAsync(acc);

            // Return JSON instead of file download
            return Json(new
            {
                success = true,
                message = "PDF generated, saved, and path stored successfully.",
                filePath = Url.Content(relativePath),
                saveResult = saveResult
            });
        }

        /// <summary>
        /// Populates ViewBags with lists for transaction types, maintenance entries, and member list
        /// based on current month/year. Optionally preselects a transaction type.
        /// </summary>
        /// <param /name="selectedTransactionId"/>Optional selected transaction type ID</param>

        private async Task PopulateTransactionList(int? selectedTransactionId = null)
        {
            var ds = await objACC.FetchTransactionTypeAsync();
            ViewBag.TransactionList = new SelectList(ds.Tables[0].AsEnumerable()
                .Select(row => new SelectListItem
                {
                    Value = row["SubTypeId"].ToString(),
                    Text = row["SubTypeName"].ToString()
                }), "Value", "Text", selectedTransactionId);

            //var obju = new AccountManager { Month = DateTime.Now.Month.ToString(), Year = DateTime.Now.Year.ToString() };

            var obju = new AccountManager
            {
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year
            };


            var ds1 = await objACC.FetchMaintenanceAsync(obju);
            var maintenanceList = ds1.Tables[0].AsEnumerable().Select(row => new SelectListItem
            {
                Value = row["MaintananceCode"].ToString(),
                Text = row["MaintenanceDisplayName"].ToString()
            }).ToList();
            ViewBag.MaintenanceList = new SelectList(maintenanceList, "Value", "Text");

            if (maintenanceList.Any())
            {
                obju.MaintenanceCode = maintenanceList[0].Value;
                var ds2 = await objACC.FetchMaintenancebyMaintenanceAsync(obju);
                var memberList = ds2.Tables[0].AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["MemberCode"].ToString(),
                    Text = row["FullName"].ToString()
                }).ToList();
                ViewBag.ReceiverList = new SelectList(memberList, "Value", "Text");
            }
        }


        /// <summary>
        /// Returns a list of receivers (members, workers, vendors, event handlers) 
        /// based on the given type.
        /// </summary>
        /// <param /name="type"/>Receiver type (e.g., Member, Worker)</param>

        [HttpGet]
        public async Task<JsonResult> GetReceiversByType(string type)
        {
            DataSet ds = null;
            switch (type)
            {
                case "Member":
                    ds = await objACC.FetchMemberAsync();
                    break;
                case "Worker":
                    ds = await objACC.FetchWorkerAsync();
                    break;
                case "Vendor":
                    ds = await objACC.FetchVendorAsync();
                    break;
                case "EventHandler":
                    ds = await objACC.FetchEventHandlersAsync();
                    break;
            }

            //var receivers = ds?.Tables[0].AsEnumerable().Select(row => new
            //{
            //    Value = row["ReceiverCode"].ToString(),
            //    Text = row["ReceiverName"].ToString(),
            //    EntityCode = row.Table.Columns.Contains("ExpenseCode") ? row["ExpenseCode"].ToString() :
            //                 row.Table.Columns.Contains("EventCode") ? row["EventCode"].ToString() : null
            //}).ToList();

            var receivers = ds?.Tables[0].AsEnumerable().Select(row => new
            {
                Value = row["ReceiverCode"].ToString(),
                Text = row["ReceiverName"].ToString(),
                EntityCode = row.Table.Columns.Contains("ExpenseCode") ? row["ExpenseCode"].ToString() :
                  row.Table.Columns.Contains("EventCode") ? row["EventCode"].ToString() : null,
                Amount = row.Table.Columns.Contains("TotalAmount") ? row["TotalAmount"].ToString() : null
            }).ToList();

            return Json(receivers, JsonRequestBehavior.AllowGet);




        }

        //[HttpGet]
        //public async Task<JsonResult> GetMembersByMaintenance(string maintenanceCode)
        //{
        //    var obju = new AccountManager { MaintenanceCode = maintenanceCode };
        //    var ds = await objACC.FetchMaintenancebyMaintenanceAsync(obju);
        //    var members = ds.Tables[0].AsEnumerable().Select(row => new SelectListItem
        //    {
        //        Value = row["MemberCode"].ToString(),
        //        Text = row["FullName"].ToString()
        //    }).ToList();

        //    return Json(members, JsonRequestBehavior.AllowGet);
        //}


        /// <summary>
        /// Fetches members associated with the selected maintenance code and also 
        /// retrieves total amount from the second table (if available).
        /// </summary>
        /// <param /name="maintenanceCode"/>Selected maintenance code</param>




        [HttpGet]
        public async Task<JsonResult> GetMembersByMaintenance(string maintenanceCode)
        {
            var obju = new AccountManager { MaintenanceCode = maintenanceCode };
            var ds = await objACC.FetchMaintenancebyMaintenanceAsync(obju);

            // Members list
            var members = ds.Tables[0].AsEnumerable().Select(row => new SelectListItem
            {
                Value = row["MemberCode"].ToString(),
                Text = row["FullName"].ToString()
            }).ToList();

            // Total amount from second table
            decimal totalAmount = 0;
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                decimal.TryParse(ds.Tables[1].Rows[0]["TotalAmount"].ToString(), out totalAmount);
            }

            return Json(new { members, totalAmount }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Returns a list of maintenance entries for a given month and year.
        /// </summary>
        /// <param /name="month"/>Month value</param>
        /// <param name="year">Year value</param>

        [HttpGet]
        public async Task<JsonResult> GetMaintenanceListByMonthYear(int month, int year)
        {
            var obj = new AccountManager { Month = month, Year = year };
            var ds = await objACC.FetchMaintenanceAsync(obj);
            var list = ds.Tables[0].AsEnumerable().Select(row => new SelectListItem
            {
                Value = row["MaintananceCode"].ToString(),
                Text = row["MaintenanceDisplayName"].ToString()
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Generates a salary slip PDF for a given worker.
        /// Fetches worker salary details and saves PDF to server.
        /// </summary>
        /// <param /name="workerCode"/>Worker code to generate slip for</param>


        [HttpPost]
        public async Task<ActionResult> GenerateSalarySlip(string workerCode)
        {
            try
            {
                // 1. Fetch salary data from BAL as DataTable
                DataTable dt = await objACC.GenerateSalarySlip(workerCode); // Make sure this returns Task<DataTable>

                if (dt == null || dt.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "No salary data found for this worker." });
                }

                DataRow row = dt.Rows[0];

                // 2. Convert to dictionary for view model
                var salarySlipData = new Dictionary<string, string>
 {
     { "WorkerName", row["ReceiverName"]?.ToString() ?? "N/A" },
     { "Contact", row["Contact"]?.ToString() ?? "N/A" },
     { "Role", row["Role"]?.ToString() ?? "N/A" },
     { "Date", row["Date"]?.ToString() ?? "N/A" },
     { "AttendanceMonth", row["AttendanceMonth"]?.ToString() ?? "N/A" },
     { "DaysPresent", row["DaysPresent"]?.ToString() ?? "0" },
     { "BaseSalary", row["BaseSalary"]?.ToString() ?? "0" },
     { "TotalAmount", row["TotalAmount"]?.ToString() ?? "0" },
     { "PaymentModeName", "Cash" }, // You can update this based on your logic
     { "TransactionRef", "Salary Payment" },
     { "IFSC Code", row["IFSC Code"]?.ToString() ?? "N/A" },
     { "Worker UPI", row["Worker UPI"]?.ToString() ?? "N/A" }
 };

                // 3. File name and path
                string fileName = $"SalarySlip_{workerCode}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string folderPath = Server.MapPath("~/Content/Documents/SalarySlips/");
                string fullPath = Path.Combine(folderPath, fileName);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // 4. Generate PDF using Rotativa
                var pdfResult = new Rotativa.ViewAsPdf("SalarySlipView", salarySlipData)
                {
                    PageSize = Rotativa.Options.Size.A4,
                    PageOrientation = Rotativa.Options.Orientation.Portrait,
                    PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10),
                    CustomSwitches = "--print-media-type --disable-smart-shrinking"
                };


                byte[] pdfBytes = pdfResult.BuildFile(ControllerContext);
                System.IO.File.WriteAllBytes(fullPath, pdfBytes);

                return Json(new
                {
                    success = true,
                    message = "Salary slip generated successfully.",
                    filePath = Url.Content("~/Content/Documents/SalarySlips/" + fileName)
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error: " + ex.Message
                });
            }
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





        #region********************************************************************* Complaints ***********************************************************


        // ✅ Complaint List via BAL
        public async Task<ActionResult> ComplaintList()
        {
            try
            {
                BALAccountManager bal = new BALAccountManager();
                List<AccountManager> complaints = await bal.GetAllComplaintsAsync();
                return View("ComplaintList", complaints);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading complaints: " + ex.Message;
                return View("ComplaintList", new List<AccountManager>());
            }
        }



        public async Task<ActionResult> ViewResolvedComplaintDetails(int complaintId)
        {
            if (complaintId <= 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            BALAccountManager obj = new BALAccountManager();
            var data = await obj.GetResolvedComplaintDetailsByIdAsync(complaintId);

            if (data == null)
                return HttpNotFound();

            return PartialView("_ResolvedComplaintDetails", data);
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
        [HttpGet]
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


        #region********************************************************************* Event Management ***********************************************************



        //*********************************************************************************************************************************************************** Savita  Gawali  ***********************************************************************************************************************************************************



        //public async Task<ActionResult> BankList()
        //{
        //    BALAccountManager bal = new BALAccountManager();
        //    List<AccountManager> list = await bal.GetAllBankDetailsSS();
        //    return View(list);
        //}

        /// <summary>
        /// Retrieves all event-related data including budget info and passes it to the EventList view.
        /// </summary>

        /// <returns>EventList view with a list of AccountManager objects containing event details.</returns>

        public async Task<ActionResult> EventList()
        {
            BALAccountManager bal = new BALAccountManager();
            List<AccountManager> eventList = await bal.GetAllEventListSS();
            return View(eventList);
        }

        /// <summary>
        /// Sends a budget approval request via email and updates its status in the database.
        /// Called via AJAX POST.
        /// </summary>
        /// <param name="id">The budget ID to be approved.</param>

        /// <returns>JSON indicating success or failure.</returns>

        [HttpPost]
        public async Task<JsonResult> SendRequestAjax(int id)
        {
            try
            {
                var result = await new BALAccountManager().SendRequestAsyncSS(id);

                if (result)
                    return Json(new { success = true, message = "Budget Approved Successfully." });
                else
                    return Json(new { success = false, message = "No rows affected in DB." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "EX: " + ex.Message });
            }
        }







        //*********************************************************** Pradnya Mane  **************************************************************************************************





        [HttpGet]
        public async Task<ActionResult> CreateBudget()
        {
            BALAccountManager obj = new BALAccountManager();
            var approvedEvents = await obj.GetApprovedEventsAsync();

            // Fetch opening balance
            decimal openingBalance = await obj.GetOpeningBalanceAsync();

            ViewBag.OpeningBalance = openingBalance.ToString("N2"); // formatted with comma separator

            return PartialView("_CreateBudget", approvedEvents);
        }




        [HttpPost]

        public async Task<ActionResult> SubmitBudget(string EventName, decimal AllocatedBudget)
        {
            try
            {
                BALAccountManager bal = new BALAccountManager();
                bool isSaved = await bal.SubmitBudgetAsync(EventName, AllocatedBudget);
                TempData["Success"] = isSaved ? "Budget added successfully!" : "Failed to add budget.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }

            return RedirectToAction("EventList");
        }



        [HttpPost]
        public async Task<JsonResult> GetEventDetails(string eventName)
        {
            BALAccountManager obj = new BALAccountManager();
            var approvedEvents = await obj.GetApprovedEventsAsync();
            var selectedEvent = approvedEvents.FirstOrDefault(e => e.EventName == eventName);

            if (selectedEvent != null)
            {
                return Json(new
                {
                    selectedEvent.EventHandlerName,
                    CreatedDateString = selectedEvent.CreatedDate.HasValue
                        ? selectedEvent.CreatedDate.Value.ToString("yyyy-MM-dd")
                        : "",
                    selectedEvent.PhoneNumber
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }




        public async Task<ActionResult> UpdateDetails(string eventCode)
        {
            BALAccountManager bal = new BALAccountManager();
            var eventDetails = await bal.GetEventDetailsByCodeAsync(eventCode);

            if (eventDetails == null)
            {
                return HttpNotFound();
            }

            return PartialView("_UpdateDetails", eventDetails);
        }




        [HttpPost]
        public async Task<ActionResult> UpdateActualCost(string eventCode, decimal actualCost)
        {
            try
            {
                BALAccountManager bal = new BALAccountManager();
                int result = await bal.UpdateActualCostAsync(eventCode, actualCost);

                TempData["Success"] = result > 0 ? "Actual cost updated successfully!" : "Failed to update actual cost.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }

            return RedirectToAction("EventList");
        }




        public async Task<ActionResult> ViewBudget(string eventCode)
        {
            BALAccountManager bal = new BALAccountManager();
            var eventDetails = await bal.GetEventDetailsForViewAsync(eventCode);

            if (eventDetails == null)
            {
                return HttpNotFound();
            }

            return PartialView("_ViewBudget", eventDetails);
        }



 







  // Shubham Vaidya 



        /// <summary>
        /// Doing the payment and saving it in transssaction table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]

        public async Task<JsonResult> SavePaymentSV(AccountManager model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.EventCode) || string.IsNullOrEmpty(model.TransactionId_ChequeId))
                    return Json(new { success = false, message = "Missing required fields." });

                int finalPaymentMode = GetPaymentModeFromRazorpaySV(model.PaymentId);

                if (finalPaymentMode == 0)
                    finalPaymentMode = model.PaymentModeId;

                BALAccountManager bal = new BALAccountManager();
                bool isSaved = await bal.SavePaymentSVTransactionSV(model.EventCode, model.TransactionId_ChequeId, finalPaymentMode);

                return Json(new { success = isSaved, message = isSaved ? "Saved" : "SP returned false" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Exception: " + ex.Message });
            }
        }


        /// <summary>
        /// Forsaving the payment mode wether it is upi or net banking
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>

        private int GetPaymentModeFromRazorpaySV(string paymentId)
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
            catch (Exception)
            {
                // Log error here
            }

            return 0; // unknown or failed to fetch
        }



        /// <summary>
        /// Opening razorpay page for payments
        /// </summary>
        /// <param name="eventCode"></param>
        /// <returns></returns>

        public async Task<ActionResult> LoadPaymentPartialSV(string eventCode)
        {
            if (string.IsNullOrEmpty(eventCode))
                return new HttpStatusCodeResult(400, "Missing event code");

            BALAccountManager bal = new BALAccountManager();
            var events = await bal.GetAllEventListSS();

            var selectedEvent = events.FirstOrDefault(e => e.EventCode == eventCode);

            if (selectedEvent == null)
                return HttpNotFound("Event not found");

            return PartialView("_PaymentPartial", selectedEvent);
        }







        #endregion



        #region********************************************************************* Worker Pay Manage ***********************************************************




        public async Task<ActionResult> FetchingWorkersData()
        {
            try
            {
                BALAccountManager bal = new BALAccountManager();

                // Call the BAL method which returns List<AccountManager>
                List<AccountManager> workers = await bal.FetchWorkerInformationADAsync();

                return View(workers); // Pass the list to view
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Failed to load worker data: " + ex.Message;
                return View(new List<AccountManager>()); // Return empty list on error
            }
        }




        [HttpPost]
        public async Task<ActionResult> ProcessPayment(AccountManager model)
        {
            try
            {
                await new BALAccountManager().ProcessWorkerPaymentAsync(model);

                return Json(new
                {
                    success = true,
                    message = "💸 Payment successful! Worker payment processed & recorded."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"❌ Payment failed: {ex.Message}" });
            }
        }



        [HttpGet]
        public async Task<ActionResult> GetWorkerPaymentDetails(string workerCode, string attendanceMonth)
        {
            BALAccountManager bal = new BALAccountManager();
            var dt = await bal.FetchWorkerPaymentDetails(workerCode, attendanceMonth);

            if (dt != null && dt.Rows.Count > 0)
            {
                return PartialView("_ViewWorkerDetailsAD", dt.Rows[0]);
            }
            else
            {
                return Content("");
            }
        }




        [HttpGet]
        public async Task<ActionResult> FetchSingleWorkerPaymentData(string workerCode, string attendanceMonth)
        {
            BALAccountManager bal = new BALAccountManager();
            var dt = await bal.FetchSingleWorkerPaymentData(workerCode, attendanceMonth);

            if (dt != null && dt.Rows.Count > 0)
            {
                var dr = dt.Rows[0]; // ✅ Extract single row here
                return PartialView("_WorkerPaymentFormAD", dr); // pass only single DataRow
            }
            else
            {
                return Content("😶 No payment data found for this worker.");
            }
        }

    




        #endregion



        #region********************************************************************* Reports  ***********************************************************

       
        
        //FOR SHOW TOTAL MEMEBERS COUNT****************************************************************************** 
        public async Task<ActionResult> AuditReport()
        {

            BALAccountManager bal = new BALAccountManager();
            int totalMembers = await bal.GetTotalMemberCount();
            int totalWorkers = await bal.GetTotalWorkerCount();
            int totalVendor = await bal.GetTotalVendorCount();
            ViewBag.TotalMembers = totalMembers;
            ViewBag.TotalWorkers = totalWorkers;
            ViewBag.TotalVendor = totalVendor;
            return View();
        }

        //FOR SHOW MEMBER LIST WHEN CLICK ON CARD/TAB*************************************************************** 
        public async Task<ActionResult> MemberList()
        {
            BALAccountManager bal = new BALAccountManager();
            var members = await bal.GetAllMemberDetails();
            return PartialView("_MemberList", members);
        }


        //FOR SHOW WORKERLIST LIST WHEN CLICK ON CARD/TAB*************************************************************** 
        public async Task<ActionResult> WorkerList()
        {
            BALAccountManager bal = new BALAccountManager();
            var worker = await bal.GetAllWorkerDetails();
            return PartialView("_WorkerListNK", worker);
        }

        //FOR SHOW VENDOR LIST WHEN CLICK ON CARD/TAB*************************************************************** 
        public async Task<ActionResult> VendorList()
        {
            BALAccountManager bal = new BALAccountManager();
            var Vendor = await bal.GetAllVendorDetails();
            return PartialView("_VendorListNK", Vendor);
        }
        //FOR SHOW PIE CHART FOR DIRECT AND INDIRECT EXPENCE*************************************************************** 

        [HttpPost]
        public async Task<JsonResult> ShowPieChartForExpence(int month, int year)
        {
            var data = await new BALAccountManager().GetMonthlyExpensePieDataAsync(month, year);
            var chartData = data.Select(x => new
            {
                name = x.ExpenseType,
                y = x.Percentage
            }).ToList();

            return Json(chartData, JsonRequestBehavior.AllowGet);
        }


        //FOR SHOW LIST WHEN CLICK ON  PIE CHART EACH SLICE*************************************************************** 
        [HttpPost]
        public async Task<ActionResult> GetExpenseDetailsByType(string expenseTypeName, int month, int year)
        {
            int expenseTypeId = (expenseTypeName == "Direct Expense") ? 36 : 37;

            BALAccountManager bal = new BALAccountManager();
            var list = await bal.GetExpenseDetailsByType(expenseTypeId, month, year);

            return PartialView("_ExpenceDetailsListPartialNK", list);
        }


        //FOR SHOW COLUMN CHART FOR WORKER SALARY*************************************************************** 
        public async Task<JsonResult> GetWorkerSalaryTotalPerMonth()
        {
            BALAccountManager bal = new BALAccountManager();
            var data = await bal.GetMonthlyTotalWorkerSalaryAsync();

            var result = data.Select(x => new
            {
                Month = x.MonthName,
                Amount = x.TotalAmount
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }




        //for  show list of worker payment when click on column********************

        [HttpPost]
        public async Task<ActionResult> GetWorkerSalaryListByMonth(int month, int year)
        {
            BALAccountManager bal = new BALAccountManager();
            var data = await bal.GetWorkerSalaryListByMonthAsync(month, year);
            return PartialView("_WorkerSalaryListPartialNK", data);
        }







        //Viraj  BALLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL 10-7-2025



        // 🔹 Chart 1: Budget vs Actual
        [HttpGet]
        public async Task<JsonResult> GetBudgetVsActualChartData(int? month, int? year)
        {
            BALAccountManager _bal = new BALAccountManager();
            try
            {
                int selectedMonth = month ?? DateTime.Now.Month;
                int selectedYear = year ?? DateTime.Now.Year;

                var events = await _bal.GetBudgetVsActualDataAsync(selectedMonth, selectedYear);

                return Json(new
                {
                    Month = selectedMonth,
                    Year = selectedYear,
                    MonthLabel = new DateTime(selectedYear, selectedMonth, 1).ToString("MMMM yyyy"),
                    Events = events
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<PartialViewResult> EventBudgetDetails(int? month, int? year)
        {
            BALAccountManager _bal = new BALAccountManager();
            try
            {
                List<AccountManager> model;

                if (month.HasValue && year.HasValue)
                {
                    model = await _bal.GetBudgetVsActualDataAsync(month.Value, year.Value);
                    ViewBag.MonthLabel = new DateTime(year.Value, month.Value, 1).ToString("MMMM yyyy");
                }
                else
                {
                    model = await _bal.GetAllEventBudgetDetailsAsync();
                    ViewBag.MonthLabel = "All Months";
                }

                return PartialView("_EventBudgetDetailsPartial", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading budget details: " + ex.Message;
                return PartialView("_EventBudgetDetailsPartial", new List<AccountManager>());
            }
        }

        // 🔹 Chart 2: Income vs Expense
        [HttpGet]
        public async Task<JsonResult> GetMonthlyIncomeExpenseChartData(int? year)
        {
            BALAccountManager _bal = new BALAccountManager();
            try
            {
                int selectedYear = year ?? DateTime.Now.Year;
                var data = await _bal.GetMonthlyIncomeExpenseAsync(selectedYear);
                return Json(new { year = selectedYear, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<PartialViewResult> IncomeExpenseDetails(int month, int year)
        {
            BALAccountManager _bal = new BALAccountManager();
            try
            {
                var model = await _bal.GetIncomeExpenseDetailsByMonthAsync(month, year);
                ViewBag.MonthLabel = new DateTime(year, month, 1).ToString("MMMM yyyy");
                return PartialView("_IncomeExpenseList", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading income/expense details: " + ex.Message;
                return PartialView("_IncomeExpenseList", new List<AccountManager>());
            }
        }



        #endregion

    }
}