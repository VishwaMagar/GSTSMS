using GSTSMSLibrary.Secretary;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using crypto;
using System.Net.Mail;
using System.Net;
using GSTSMSLibrary.Account;
using System.Globalization;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace GSTSMS.Controllers
{
    public class SecretaryController : Controller
    {
        BALSecretary obj = new BALSecretary();
        // GET: Secretary
        public ActionResult Index()
        {
            return View();
        }

        #region*********************************************************************Priti Chavhan***********************************************************

        // Fetches staff information from the database and populates the model.
        [HttpGet]
        public async Task<ActionResult> ProfilePC()
        {
            if (Session["StaffCode"] == null)
                return RedirectToAction("Login", "Account");


            string staffcode = Session["StaffCode"].ToString();
            SqlDataReader dr = null;
            Secretary model = new Secretary();

            try
            {
                dr = await obj.GetStaffInfo(staffcode);

                if (await dr.ReadAsync())
                {
                    model.StaffName = dr["StaffName"].ToString();
                    model.Email = dr["Email"].ToString();
                    model.Password = dr["Password"].ToString();
                    model.StaffCode = dr["StaffCode"].ToString();
                    model.ProfilePic = dr["ProfilePic"] != DBNull.Value ? dr["ProfilePic"].ToString() : "~/Images/default-profile.png";

                    Session["ProfilePic"] = model.ProfilePic;
                    Session["StaffName"] = model.StaffName;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching staff info: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while loading your profile. Please try again.";
                return RedirectToAction("Dashboard", "Home");
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }
            return View("ProfilePC", model);
        }

        // Allows staff to update their name, email, profile picture, and change password.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProfilePC(Secretary model, HttpPostedFileBase ProfilePic, string AvatarUrl)
        {
            if (Session["StaffCode"] == null)
                return RedirectToAction("Login", "Account");

            string staffcode = Session["StaffCode"].ToString();
            model.StaffCode = staffcode;

            string currentProfilePicPath = Session["ProfilePic"] != null ? Session["ProfilePic"].ToString() : "~/Images/default-profile.png";

            if (!string.IsNullOrEmpty(AvatarUrl))
            {
                model.ProfilePic = AvatarUrl;
            }
            else if (ProfilePic != null && ProfilePic.ContentLength > 0)
            {
                string fileName = Path.GetFileName(ProfilePic.FileName);
                string uploadDir = Server.MapPath("~/Content/img/");

                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                string path = Path.Combine(uploadDir, fileName);
                ProfilePic.SaveAs(path);
                model.ProfilePic = "~/Content/img/" + fileName;
            }
            else
            {

                model.ProfilePic = currentProfilePicPath;
            }

            bool passwordChangeAttempted = !string.IsNullOrEmpty(model.NewPassword);

            if (passwordChangeAttempted)
            {
                if (string.IsNullOrEmpty(model.ConfirmPassword) || model.NewPassword != model.ConfirmPassword)
                {
                    TempData["ErrorMessage"] = "New password and confirm password must match.";
                    model.NewPassword = null;
                    model.ConfirmPassword = null;
                    return View("ProfilePC", model);
                }
            }
            else
            {

                model.NewPassword = null;
                model.ConfirmPassword = null;
            }
            if (!passwordChangeAttempted)
            {
                model.Password = null;
            }
            int updateResult = await obj.UpdateProfile(model);

            if (updateResult == 1)
            {

                Session["StaffName"] = model.StaffName;
                Session["ProfilePic"] = model.ProfilePic;

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("ProfilePC");
            }
            else
            {

                TempData["ErrorMessage"] = "An error occurred while updating the profile. Please try again.";

                model.NewPassword = null;
                model.ConfirmPassword = null;

                return View("ProfilePC", model);
            }
        }

        // In your SecretaryController.cs (or wherever RemoveProfilePic is)

        [HttpPost]
        public async Task<ActionResult> RemoveProfilePic()
        {
            if (Session["StaffCode"] == null)
            {
                return Json(new { success = false, message = "Session expired. Please log in again." });
            }
            string staffCode = Session["StaffCode"].ToString();
            try
            {
                int removeResult = await obj.RemoveProfilePicture(staffCode);

                if (removeResult == 1)
                {
                    Session["ProfilePic"] = null;
                    return Json(new { success = true, message = "Profile picture removed successfully. Please refresh the page." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to remove profile picture." });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing profile picture: {ex.Message}");
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        // GET: Displays a list of all available policies.
        // Inside your Controller file (e.g., SecretaryController.cs)
        [HttpGet]
        public async Task<ActionResult> Policies()
        {
            if (Session["RoleId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int loggedInRoleId = Convert.ToInt32(Session["RoleId"]);

            // Fetch policies
            DataSet dsPolicies = await obj.PolicyList(loggedInRoleId);
            List<Secretary> lstPolicy = new List<Secretary>();

            if (dsPolicies != null && dsPolicies.Tables.Count > 0 && dsPolicies.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsPolicies.Tables[0].Rows.Count; i++)
                {
                    Secretary sec = new Secretary();
                    sec.PolicyId = Convert.ToInt32(dsPolicies.Tables[0].Rows[i]["PolicyId"]);
                    sec.PolicyName = dsPolicies.Tables[0].Rows[i]["PolicyName"].ToString();
                    sec.Description = dsPolicies.Tables[0].Rows[i]["Description"].ToString(); // Policy Description
                    sec.CreatedBy = dsPolicies.Tables[0].Rows[i]["CreatedBy"].ToString();
                    sec.CreatedDate = Convert.ToDateTime(dsPolicies.Tables[0].Rows[i]["CreatedDate"]).Date;
                    sec.PenaltyCost = dsPolicies.Tables[0].Rows[i]["PenaltyCost"].ToString();
                    sec.RoleId = Convert.ToInt32(dsPolicies.Tables[0].Rows[i]["RoleId"]);
                    sec.SocietyCode = dsPolicies.Tables[0].Rows[i]["SocietyCode"].ToString();

                    sec.TermsAndConditions = new List<Secretary>();
                    DataSet dsTerms = await obj.GetPolicyTerms(sec.PolicyId);

                    if (dsTerms != null && dsTerms.Tables.Count > 0 && dsTerms.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < dsTerms.Tables[0].Rows.Count; j++)
                        {

                            Secretary termSec = new Secretary
                            {
                                TermsId = Convert.ToInt32(dsTerms.Tables[0].Rows[j]["TermsId"]),
                                PolicyId = Convert.ToInt32(dsTerms.Tables[0].Rows[j]["PolicyId"]),
                                Terms_Code = dsTerms.Tables[0].Rows[j]["TermsCode"].ToString(),
                                Title = dsTerms.Tables[0].Rows[j]["Title"].ToString(),
                                Description = dsTerms.Tables[0].Rows[j]["Description"].ToString()
                            };
                            sec.TermsAndConditions.Add(termSec);
                        }
                    }
                    lstPolicy.Add(sec);
                }
            }
            return View(lstPolicy);
        }

        // GET: Displays the terms and conditions for a specific policy.
        [HttpGet]
        public async Task<ActionResult> TermsAndCondition(int policyId)
        {
            DataSet ds = await obj.GetTermsAndConditionsByPolicyId(policyId);
            List<Secretary> termsList = new List<Secretary>();

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    termsList.Add(new Secretary
                    {
                        TermsId = Convert.ToInt32(row["TermsId"]),
                        Title = row["Title"].ToString(),
                        Description = row["Description"].ToString(),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                    });
                }
            }
            ViewBag.PolicyId = policyId;
            return View("TermsAndCondition", termsList);
        }

        #endregion

        #region*********************************************************************Payal Gogawale***********************************************************

        public async Task<ActionResult> VendorList1PG(DateTime? StartDate, DateTime? EndDate)
        {
            try
            {
                string staffCode = Session["StaffCode"]?.ToString();
                if (string.IsNullOrEmpty(staffCode))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Set default date range if not provided
                if (StartDate == null)
                    StartDate = new DateTime(DateTime.Today.Year, 1, 1);
                if (EndDate == null)
                    EndDate = DateTime.Today;

                // Set ViewBag for date inputs
                ViewBag.StartDate = StartDate.Value.ToString("yyyy-MM-dd");
                ViewBag.EndDate = EndDate.Value.ToString("yyyy-MM-dd");

                Secretary objU = new Secretary();
                List<Secretary> lstVendors = new List<Secretary>();

                // Call the BAL method with the correct parameters
                DataSet ds = await obj.listVendorPG(staffCode, StartDate.Value, EndDate.Value);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Secretary objRow = new Secretary();
                        objRow.VendorCode = ds.Tables[0].Rows[i]["VendorCode"].ToString();
                        objRow.VendorName = ds.Tables[0].Rows[i]["VendorName"].ToString();
                        objRow.Email = ds.Tables[0].Rows[i]["Email"].ToString();
                        objRow.PhoneNumber = ds.Tables[0].Rows[i]["PhoneNumber"].ToString();
                        objRow.ServiceProvide = ds.Tables[0].Rows[i]["ServiceProvide"].ToString();
                        objRow.Address = ds.Tables[0].Rows[i]["Address"].ToString();
                        objRow.JoiningDate = ds.Tables[0].Rows[i]["JoiningDate"] != DBNull.Value
                            ? Convert.ToDateTime(ds.Tables[0].Rows[i]["JoiningDate"])
                            : (DateTime?)null;
                        objRow.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                        lstVendors.Add(objRow);
                    }
                }

                objU.VendorList = lstVendors;
                return View(objU);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<ActionResult> _ViewVendorPG(string id)
        {
            try
            {
                var secretary = new Secretary { VendorCode = id };

                SqlDataReader dr = await obj.ViewVendorPG(secretary);
                using (dr)
                {
                    if (await dr.ReadAsync())
                    {
                        secretary = MapVendorReader(dr);
                    }
                }


                return PartialView("_ViewVendorPG", secretary);
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"_ViewVendorPG Error: {ex.Message}");
                var emptySecretary = new Secretary { VendorCode = id };
                return PartialView("_ViewVendorPG", emptySecretary);
            }
        }

        /// <summary>
        /// Helper method to map SqlDataReader to Secretary model
        /// </summary>
        public Secretary MapVendorReader(SqlDataReader dr)
        {
            return new Secretary
            {
                //VendorCode = dr["VendorCode"]?.ToString() ?? "",
                VendorName = dr["VendorName"]?.ToString() ?? "",
                ServiceProvide = dr["ServiceProvide"]?.ToString() ?? "",
                Email = dr["Email"]?.ToString() ?? "",
                PhoneNumber = dr["PhoneNumber"]?.ToString() ?? "",
                Address = dr["Address"]?.ToString() ?? "",
                Location = dr["Location"]?.ToString() ?? ""
            };
        }

        #endregion


        #region*********************************************************************Sanika Jaykar***********************************************************

        // ✅ Get calendar events for Secretary role (Meetings and Events)
        [HttpGet]
        public async Task<ActionResult> GetCalendarSJ()
        {
            Secretary objC = new Secretary();
            objC.RoleId = Convert.ToInt32(Session["RoleId"]);


            DataSet ds = await obj.CalenderSJ(objC);
            List<object> events = new List<object>();

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                events = ds.Tables[0].AsEnumerable().Select(row => new
                {
                    id = row["Type"].ToString() == "Event"
                        ? $"E-{row["EventCode"]}"
                        : $"M-{row["MeetingCode"]}",

                    title = row["Title"]?.ToString(),

                    meetingTitle = row["Type"].ToString() == "Meeting"
                        ? row["Title"]?.ToString()
                        : null,

                    start = Convert.ToDateTime(row["StartDate"]).ToString("yyyy-MM-ddTHH:mm:ss"),

                    end = row["EndDate"] != DBNull.Value
                        ? Convert.ToDateTime(row["EndDate"]).ToString("yyyy-MM-ddTHH:mm:ss")
                        : null,

                    type = row["Type"]?.ToString(),
                    eventId = row["EventCode"]?.ToString(),
                    meetingId = row["MeetingCode"]?.ToString(),
                    allDay = row["EndDate"] == DBNull.Value,

                    color = row["Type"].ToString() == "Meeting" ? "#007bff" : "#28a745",

                    description = row["Description"]?.ToString(),   // <-- Add this line

                    location = row["Location"]?.ToString(),         // ✅ add this line

                    meetingAgenda = row["MeetingAgenda"]?.ToString()  // <-- Add this line here

                }).ToList<object>();



            }

            ViewBag.CalendarEvents = JsonConvert.SerializeObject(events);
            return View();
        }

        // ✅ Get calendar events for Secretary role (Meetings and Events)

        [HttpGet]

        public async Task<ActionResult> _LoadEventDetailsSJ(string type, int? eventId, int? meetingId)
        {
            Secretary objP = new Secretary();

            Secretary objR = new Secretary
            {
                EventId = eventId ?? 0,
                MeetingId = meetingId ?? 0
            };

            DataSet ds = await obj.CalenderSJ(objR);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].AsEnumerable()
                    .FirstOrDefault(r => r["Type"]?.ToString() == type);

                if (type == "Event")
                {
                    objP.EventId = eventId ?? 0;
                    objP.Title = row["Title"]?.ToString();
                    objP.StartDate = Convert.ToDateTime(row["StartDate"]);
                    objP.EndDate = row["EndDate"] != DBNull.Value ? Convert.ToDateTime(row["EndDate"]) : (DateTime?)null;
                    objP.Type = "Event";
                    objP.SubType = row["SubType"]?.ToString();
                    objP.Description = row["Description"]?.ToString();
                    objP.Location = row["Location"]?.ToString();
                }
                else if (type == "Meeting")
                {
                    objP.MeetingId = meetingId ?? 0;
                    objP.MeetingTitle = row["Title"]?.ToString() ?? "";
                    objP.Title = row["Title"]?.ToString() ?? "";
                    objP.StartDate = Convert.ToDateTime(row["StartDate"]);
                    objP.EndDate = Convert.ToDateTime(row["StartDate"]);
                    objP.Type = "Meeting";
                    objP.Location = row["Location"]?.ToString() ?? "";
                    objP.MeetingAgenda = row["MeetingAgenda"]?.ToString(); // ✅ Add this line
                }



            }

            return PartialView("_LoadEventDetailsSJ", objP);
        }

        // ✅ Fetch list of all notifications for the Secretary dashboard

        [HttpGet]
        public async Task<JsonResult> GetNotificationsSJ()
        {
            try
            {
                Secretary objS = new Secretary();
                objS.RoleId = Convert.ToInt32(Session["RoleId"]);

                var data = await obj.NotificationSJ(objS);
                List<dynamic> notifications = new List<dynamic>();

                if (data != null && data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in data.Tables[0].Rows)
                    {
                        DateTime sendTime = DateTime.Now; // Default to now if DBNull
                        if (row.Table.Columns.Contains("SendTime") && row["SendTime"] != DBNull.Value)
                        {
                            sendTime = Convert.ToDateTime(row["SendTime"]);
                        }

                        notifications.Add(new
                        {
                            NotificationId = row["NotificationId"] != DBNull.Value ? Convert.ToInt32(row["NotificationId"]) : 0,
                            Type = row["Type"].ToString(),
                            Id = row["Id"].ToString(),
                            Title = row["Title"].ToString(),
                            Description = row["Description"].ToString(),
                            ExtraInfo = row.Table.Columns.Contains("ExtraInfo") && row["ExtraInfo"] != DBNull.Value ? row["ExtraInfo"].ToString() : null,
                            SendTime = sendTime.ToString("o", CultureInfo.InvariantCulture) // Format as ISO 8601
                        });
                    }
                }

                return Json(notifications, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ✅ Load full details of a specific notification by ID

        [HttpGet]
        public async Task<ActionResult> _DetailsNotificationsSJ(int id)
        {
            DataSet data = await obj.GetNotificationDetailsSJ(id);

            if (data != null && data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
            {
                DataRow row = data.Tables[0].Rows[0];

                // Handle SendTime for the details view as well
                DateTime sendTime = DateTime.Now; // Default to now if DBNull
                if (row.Table.Columns.Contains("SendTime") && row["SendTime"] != DBNull.Value)
                {
                    sendTime = Convert.ToDateTime(row["SendTime"]);
                }

                Secretary model = new Secretary
                {
                    NotificationId = Convert.ToInt32(row["NotificationId"]),
                    Type = row["Type"].ToString(),
                    Title = row["Title"].ToString(),
                    Description = row["Description"].ToString(),
                    SendTime = sendTime,
                    FullName = row.Table.Columns.Contains("MemberName") ? row["MemberName"].ToString() : null,
                    Ratings = row.Table.Columns.Contains("Ratings") && float.TryParse(row["Ratings"].ToString(), out float ratingValue) ? ratingValue : 0f,

                    // New additions
                    EventCode = row.Table.Columns.Contains("EventId") && row["EventId"] != DBNull.Value ? row["EventId"].ToString() : null,
                    MeetingCode = row.Table.Columns.Contains("MeetingId") && row["MeetingId"] != DBNull.Value ? row["MeetingId"].ToString() : null
                };


                // Additionally, if you're displaying StartDate/EndDate, ensure they are handled similarly
                if (row.Table.Columns.Contains("StartDate") && row["StartDate"] != DBNull.Value)
                {
                    model.StartDate = Convert.ToDateTime(row["StartDate"]);
                }
                if (row.Table.Columns.Contains("EndDate") && row["EndDate"] != DBNull.Value)
                {
                    model.EndDate = Convert.ToDateTime(row["EndDate"]);
                }
                // Populate other fields as needed for the details model

                return PartialView("_DetailsNotificationsSJ", model);
            }

            return HttpNotFound();
        }

        // ✅ Mark all notifications as read for Secretary

        [HttpPost]
        public async Task<JsonResult> MarkAllNotificationsAsReadSJ()
        {
            var result = await obj.MarkAllNotificationsAsReadSJ();
            return Json(new { success = result });
        }

        // ✅ Mark all notifications as read for Secretary

        [HttpPost]
        public async Task<JsonResult> MarkNotificationAsReadSJ(int id)
        {
            try
            {
                var result = await obj.MarkNotificationAsReadSJ(id); // Call your new BAL method
                return Json(new { success = result, notificationId = id }); // Return the ID for UI update
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error marking notification {id} as read: {ex.Message}");
                return Json(new { success = false, message = "Failed to mark notification as read." });
            }
        }

        #endregion

        #region*********************************************************************Suyog Kulkarni***********************************************************


        public ActionResult FacilitySK()
        {
            return View();
        }

        public async Task<ActionResult> ShowListFacilitySK()
        {
            Secretary objU = new Secretary(); // This is your model for the view

            // Call the BAL method using your BALSecretary instance
            DataSet ds = await obj.ShowFacilitySK(); // 'obj' here correctly refers to BALSecretary instance

            List<Secretary> allFacility = new List<Secretary>();

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0) // Added safety check for rows
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Secretary objS = new Secretary()
                    {
                        FacilityId = Convert.ToInt32(row["FacilityId"] ?? 0),
                        FacilityName = row["FacilityName"]?.ToString() ?? string.Empty,
                        Description = row["Description"]?.ToString() ?? string.Empty,
                        Path = row["Document"]?.ToString() ?? string.Empty,
                    };

                    allFacility.Add(objS);
                }
            }

            objU.AllFacility = allFacility; // Assign the list to the 'Secretary' model object

            // Corrected line: Return the 'Secretary' model object (objU)
            return View(objU);
        }

        // ... rest of your controller methods ...

        // You also have the same issue in ViewFacilitySK and FetchFacilityByIdSK
        public async Task<ActionResult> ViewFacilitySK(int? facilityId)
        {
            if (!facilityId.HasValue)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Facility ID is required.");
            }

            Secretary objU = new Secretary() // This is your view model
            {
                FacilityId = facilityId.Value
            };

            DataSet ds = await obj.ViewFacilitySK(objU); // obj (BALSecretary) calls method on objU (Secretary model)

            List<Secretary> allFacility = new List<Secretary>();

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Secretary objS = new Secretary()
                    {
                        FacilityId = Convert.ToInt32(row["FacilityId"] ?? 0),
                        FacilityName = row["FacilityName"]?.ToString() ?? string.Empty,
                        Description = row["Description"]?.ToString() ?? string.Empty,
                        Path = row["Document"]?.ToString() ?? string.Empty,
                    };
                    allFacility.Add(objS);
                }
            }

            objU.AllFacility = allFacility;

            // Corrected line: Return objU, which is the Secretary model for the partial view
            return PartialView("ViewFacilitySK", objU);
        }

        public async Task<ActionResult> FetchFacilityByIdSK(int id)
        {
            Secretary objU = new Secretary { FacilityId = id };
            Session["FacilityId"] = id; // Consider removing session for cleaner practice if not strictly needed

            SqlDataReader dr = await obj.FetchFacilityByIdSK(id);
            if (dr.Read())
            {
                objU.FacilityName = dr["FacilityName"].ToString();
                objU.Description = dr["Description"].ToString();
                objU.FacilityId = id;
            }
            dr.Close(); // Crucial to close the DataReader!

            // Corrected line: Return objU (the Secretary model) to the partial view
            return PartialView("FetchFacilityByIdSK", objU);
        }


        // Same issue in AddFacilitySK POST method
        [HttpPost]
        public async Task<ActionResult> AddFacilitySK(Secretary objU)
        {
            // ... your save logic ...

            await obj.SaveFacilitySK(objU); // 'obj' is BALSecretary, 'objU' is the Secretary model

            TempData["Message"] = "Facility added successfully.";

            // Corrected line: Return objU, the Secretary model
            // Or better, redirect to avoid double submission issues:
            return RedirectToAction("ShowListFacilitySK");
            // If you absolutely must return the same view, pass objU:
            // return View("AddFacilitySK", objU);
        }
        #endregion

        #region*********************************************************************Sandhya Hivare***********************************************************
        [HttpGet]
        public async Task<ActionResult> SendEmailSH()
        {
            string staffCode = Session["StaffCode"]?.ToString();
            ViewBag.SecretaryEmail = Session["Email"]?.ToString();

            var dsWing = await new BALSecretary().GetWingByStaffCode(staffCode);

            if (dsWing.Tables[0].Rows.Count > 0)
            {
                ViewBag.WingName = dsWing.Tables[0].Rows[0]["WingName"].ToString();
            }
            else
            {
                ViewBag.WingName = "Not Assigned";
            }

            return View();
        }


        [HttpPost]
        public async Task<ActionResult> SendEmailSH(string subject, string description, string SelectedMemberEmails, HttpPostedFileBase attachment)
        {
            string fromEmail = Session["Email"]?.ToString();
            string appPassword = "bdgq gfsr mqdb ebec";

            if (string.IsNullOrEmpty(SelectedMemberEmails))
            {
                TempData["Message"] = "Please select at least one member.";
                return RedirectToAction("SendEmailSH");
            }

            string[] emailList = SelectedMemberEmails.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                foreach (var toEmail in emailList)
                {
                    MailMessage mail = new MailMessage
                    {
                        From = new MailAddress(fromEmail),
                        Subject = subject,
                        Body = description,
                        IsBodyHtml = true
                    };
                    mail.To.Add(toEmail.Trim());

                    if (attachment != null && attachment.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(attachment.FileName);
                        mail.Attachments.Add(new Attachment(attachment.InputStream, fileName));
                    }

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential(fromEmail, appPassword);
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(mail);
                    }
                }

                TempData["Message"] = "Emails sent successfully!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Error sending emails: {ex.Message}";
            }

            return RedirectToAction("SendEmailSH");
        }




        [HttpGet]
        public async Task<JsonResult> GetFlatsByStaffCode()
        {
            string staffCode = Session["StaffCode"]?.ToString();
            var ds = await new BALSecretary().GetWingByStaffCode(staffCode);

            var wings = ds.Tables[0].AsEnumerable().Select(row => new
            {
                WingName = row["WingName"].ToString()
            });

            return Json(wings, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetMemberEmailsByWingName()
        {
            if (Session["AssignWingId"] == null)
                return Json(new { success = false, message = "Session expired." }, JsonRequestBehavior.AllowGet);

            int wingId = Convert.ToInt32(Session["AssignWingId"]);
            var ds = await new BALSecretary().GetMemberEmailsByWingName(wingId);

            var members = ds.Tables[0].AsEnumerable().Select(row => new
            {
                Email = row["Email"].ToString(),
                FullName = row["FullName"].ToString(),
                FlatCode = row["FlatCode"].ToString(),
            });

            return Json(members, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region*********************************************************************Sanika Patil Secratery Report***********************************************************


        public ActionResult ReportSP()
        {
            string StaffCode = Session["StaffCode"].ToString();
            return View();

        }
        public ActionResult ComplaintReportSP()
        {

            return View("ComplaintReportsSP");
        }
        public ActionResult VisitorReportSP()
        {
            return View("VisitorReportSP");
        }
        public ActionResult MaintenanceReportSP()
        {
            return View("MaintenanceReportSP");
        }
        public ActionResult ParkingReportSP()
        {
            return View("ParkingReportSP");
        }
        public ActionResult ExpneseReportSP()
        {
            return View("ExpneseReportSP");
        }
        [HttpGet]
        public async Task<JsonResult> GetComplaintDataSP(DateTime fromDate, DateTime toDate, string filterType)
        {


            string StaffCode = Session["StaffCode"].ToString();
            DataSet ds = new DataSet();

            ds = await obj.ComplaintDataSP(fromDate, toDate, filterType, StaffCode);

            List<Secretary> secretaryList = new List<Secretary>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Secretary obju = new Secretary
                {
                    Dates = ds.Tables[0].Rows[i]["DateLabel"]?.ToString(),
                    Solve = Convert.ToInt32(ds.Tables[0].Rows[i]["Solve"]),
                    Pending = Convert.ToInt32(ds.Tables[0].Rows[i]["Pending"]),
                    Inprogress = Convert.ToInt32(ds.Tables[0].Rows[i]["inprogress"]),
                    Escalate = Convert.ToInt32(ds.Tables[0].Rows[i]["Escalate"])
                };
                secretaryList.Add(obju);
            }

            return Json(secretaryList, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public async Task<JsonResult> GetVisitorDataSP(DateTime fromDate, DateTime toDate)
        {
            string StaffCode = Session["StaffCode"].ToString();
            DataSet ds = new DataSet();

            ds = await obj.VisitorDataSP(fromDate, toDate, StaffCode);
            List<Secretary> secretaryList = new List<Secretary>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                secretaryList.Add(new Secretary
                {

                    Total = Convert.ToInt32(row["TotalVisitors"]),
                    Present = Convert.ToInt32(row["PresentVisitors"]),
                    Delivery = Convert.ToInt32(row["Delivery"]),
                    PresentDelivery = Convert.ToInt32(row["PresentDelivery"]),
                    Guest = Convert.ToInt32(row["Guest"]),
                    PresentGuest = Convert.ToInt32(row["PresentGuest"]),
                    Worker = Convert.ToInt32(row["Worker"]),
                    PresentWorker = Convert.ToInt32(row["PresentWorker"]),
                    Vendor = Convert.ToInt32(row["Vendor"]),
                    PresentVendor = Convert.ToInt32(row["PresentVendor"])
                });
            }

            return Json(secretaryList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<JsonResult> GetMaintenanceDataSP(DateTime fromDate, DateTime toDate)
        {

            string StaffCode = Session["StaffCode"].ToString();
            DataSet ds = new DataSet();

            ds = await obj.MaintenanceDataSP(fromDate, toDate, StaffCode);
            List<Secretary> secretaryList = new List<Secretary>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {

                Secretary obju = new Secretary
                {

                    Dates = ds.Tables[0].Rows[i]["DateLabel"]?.ToString(), // ✅ Treat as string label
                    PaidMaintenance = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalPaidAmount"]),
                    UnPaidMaintenance = Convert.ToInt32(ds.Tables[0].Rows[i]["UnpaidAmount"]),
                    PaidMembers = Convert.ToInt32(ds.Tables[0].Rows[i]["PaidMembers"]),
                    UnPaidMembers = Convert.ToInt32(ds.Tables[0].Rows[i]["UnpaidMembers"])


                };

                secretaryList.Add(obju);
            }

            return Json(secretaryList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<JsonResult> ParkingDataSP(string fromDate, string toDate)
        {
            string StaffCode = Session["StaffCode"].ToString();

            DataSet ds = await obj.ParkingDataSP(fromDate, toDate, StaffCode);

            List<Secretary> parkingList = new List<Secretary>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                parkingList.Add(new Secretary
                {
                    ParkingCode = row["ParkingCode"]?.ToString(),
                    SlotStatus = row["SlotStatus"]?.ToString(),
                    ParkingType = row["ParkingType"]?.ToString()
                });
            }

            return Json(parkingList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<PartialViewResult> _GetComplaintListByFilterSP(string filterType, string dateLabel, string statusName)
        {

            string StaffCode = Session["StaffCode"].ToString();
            Secretary objs = new Secretary();
            objs.Status = statusName;
            var list = await obj.ComplaintListSP(filterType, dateLabel, statusName, StaffCode);

            return PartialView("_GetComplaintListByFilterSP", list);

        }
        [HttpGet]
        public async Task<PartialViewResult> _VisitorListSP(string visitorType, string fromDate, string toDate, string Serisename)
        {


            string StaffCode = Session["StaffCode"].ToString();
            var list = await obj.VisitorListDataSP(visitorType, fromDate, toDate, StaffCode);
            if (Serisename == "CheckIn")
            {
                list = list.Where(x => x.CheckOut == null).ToList();
            }
            return PartialView("_VisitorListSP", list);

        }
        [HttpGet]
        public async Task<JsonResult> GetExpenseDataSP(DateTime fromDate, DateTime toDate, string filterType)
        {
            string StaffCode = Session["StaffCode"].ToString();
            DataSet ds = new DataSet();

            ds = await obj.GETExpenseDataSP(fromDate, toDate, filterType, StaffCode);

            List<Secretary> expenseList = new List<Secretary>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Secretary exp = new Secretary
                {
                    Dates = ds.Tables[0].Rows[i]["DateLabel"]?.ToString(),
                    PaidExpense = Convert.ToDecimal(ds.Tables[0].Rows[i]["PaidExpense"]?.ToString()),
                    UnpaidExpense = Convert.ToDecimal(ds.Tables[0].Rows[i]["UnpaidExpense"]?.ToString()),
                    TotalExpense = Convert.ToDecimal(ds.Tables[0].Rows[i]["TotalExpenses"]?.ToString()),

                };
                expenseList.Add(exp);
            }

            return Json(expenseList, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<PartialViewResult> _ParkingListPartialSP(string status, DateTime fromDate, DateTime toDate, string code)
        {
            string StaffCode = Session["StaffCode"].ToString();

            DataSet ds = await obj.ParkingListSP(status, fromDate, toDate, code, StaffCode);

            List<Secretary> model = new List<Secretary>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                model.Add(new Secretary
                {
                    SrNo = Convert.ToInt32(row["SrNo"]),
                    WingName = row["WingName"]?.ToString(),
                    ParkingCode = row["ParkingCode"]?.ToString(),
                    ParkingSlotId = Convert.ToInt32(row["ParkingSlotId"]?.ToString()),
                    VisitorName = row.Table.Columns.Contains("VisitorName") ? row["VisitorName"]?.ToString() : null,
                    VehicleNumber = row.Table.Columns.Contains("VehicleNumber") ? row["VehicleNumber"]?.ToString() : null,
                    CheckIn = row.Table.Columns.Contains("CheckIn") ? Convert.ToDateTime(row["CheckIn"]) : (DateTime?)null
                });
            }

            return PartialView("_ParkingListPartialSP", model);
        }


        [HttpGet]
        public async Task<PartialViewResult> _MaintenanceListPartialSP(string dateLabel, string statusName)
        {
            string StaffCode = Session["StaffCode"].ToString();

            DataSet ds = await obj.MaintenanceListSP(dateLabel, statusName, StaffCode);

            List<Secretary> model = new List<Secretary>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Secretary item = new Secretary();

                if (statusName == "Paid Maintenance")
                {
                    item.SrNo = Convert.ToInt32(row["SrNo"]);
                    item.MemberCode = row["MemberCode"]?.ToString();
                    item.FullName = row["FullName"]?.ToString();
                    item.FlatCode = row["FlatCode"]?.ToString();
                    item.PhoneNumber = row["PhoneNumber"]?.ToString();
                    item.PaidAmount = Convert.ToDecimal(row["PaidAmount"]?.ToString());
                    item.PaidDate = Convert.ToDateTime(row["PaidDate"]);
                    item.MaintenanceName = row["MaintenanceName"]?.ToString();
                    item.Description = row["Description"]?.ToString();
                }
                else if (statusName == "Unpaid Maintenance")
                {
                    item.SrNo = Convert.ToInt32(row["SrNo"]);
                    item.MemberCode = row["MemberCode"]?.ToString();
                    item.FullName = row["FullName"]?.ToString();
                    item.FlatCode = row["FlatCode"]?.ToString();
                    item.PhoneNumber = row["PhoneNumber"]?.ToString();
                    item.RemainingAmount = Convert.ToDecimal(row["RemainingAmount"]?.ToString());
                    item.DeadlineDate = Convert.ToDateTime(row.Table.Columns.Contains("EndDate") ? Convert.ToDateTime(row["EndDate"]) : (DateTime?)null);
                }

                model.Add(item);
            }

            return PartialView("_MaintenanceListPartialSP", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> _ExpenseDetailsPartialSP(string selectedDate, string statusName, string filterType)
        {
            string StaffCode = Session["StaffCode"].ToString();

            Secretary objs = new Secretary();
            objs.ExpenseName = statusName;
            var model = await obj.ExpenseDetailSP(selectedDate, filterType, StaffCode, statusName);
            return PartialView("_ExpenseDetailsPartialSP", model);
        }


        #endregion

        #region******************************Siddhesh Balkavade Dashboard****************************************************************************



        //-------------------------------------------------------------------------------------------------------------------------------------------
        // Dashboard
        public async Task<ActionResult> DashboardSB(DateTime? startDate = null, DateTime? endDate = null)
        {

            BALSecretary bal = new BALSecretary();

            string StaffCode = Session["StaffCode"].ToString();

            //Maintenance
            decimal TotalMaintenance = await bal.GetTotalMaintenanceAmountSB(StaffCode, startDate, endDate);
            decimal collectedAmount = await bal.GetCollectedMaintenanceAmountSB(StaffCode, startDate, endDate);
            decimal DueAmount = await bal.GetDueMaintenanceAmountSB(StaffCode, startDate, endDate);

            double collectedPercentage = TotalMaintenance == 0 ? 0 : Math.Round((double)(collectedAmount / TotalMaintenance) * 100, 1);
            double duePercentage = 100 - collectedPercentage;

            //--------------------------------------------------------------------------------------------------
            //Expence 
            decimal TotalExpence = await bal.GetTotalExpenceAmountSB(StaffCode, startDate, endDate);
            decimal ExpenceAmount = await bal.GetCompletedExpenceAmountSB(StaffCode, startDate, endDate);
            decimal PendingExpenceAmount = await bal.GetPendingExpenceAmountSB(StaffCode, startDate, endDate);

            double ExpencePercentage = TotalExpence == 0 ? 0 : Math.Round((double)(ExpenceAmount / TotalExpence) * 100, 1);
            double PendingExpencePercentage = 100 - ExpencePercentage;

            //--------------------------------------------------------------------------------------------------

            var Guest = await bal.GetGuestVisitorCountSB(StaffCode, startDate, endDate);
            var Delivery = await bal.GetDeliveryVisitorCountSB(StaffCode, startDate, endDate);
            var Vendor = await bal.GetVendorVisitorCountSB(StaffCode, startDate, endDate);
            var Service = await bal.GetServiceVisitorCountSB(StaffCode, startDate, endDate);
            var Worker = await bal.GetWorkerVisitorCountSB(StaffCode, startDate, endDate);

            //Maintenance
            ViewBag.CollectedMaintenance = collectedAmount.ToString("N2");
            ViewBag.DueMaintenance = DueAmount.ToString("N2");
            ViewBag.TotalMaintenance = TotalMaintenance.ToString("N2");
            ViewBag.CollectedPercentage = collectedPercentage;
            ViewBag.DuePercentage = duePercentage;

            //Expence
            ViewBag.Expence = ExpenceAmount.ToString();
            ViewBag.PendingExpenceAmount = PendingExpenceAmount.ToString("N2");
            ViewBag.TotalExpence = TotalExpence.ToString("N2");
            ViewBag.ExpencePercentage = ExpencePercentage;
            ViewBag.PendingExpencePercentage = PendingExpencePercentage;

            ViewBag.GuestVisitorCount = Guest.ToString();
            ViewBag.DeliveryVisitorCount = Delivery.ToString();
            ViewBag.VendorVisitorCount = Vendor.ToString();
            ViewBag.ServiceVisitorCount = Service.ToString();
            ViewBag.WorkerVisitorCount = Worker.ToString();
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetDashboardDataSB(DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();

            string StaffCode = Session["StaffCode"].ToString();
            //Maintenance
            decimal TotalMaintenance = await bal.GetTotalMaintenanceAmountSB(StaffCode, startDate, endDate);
            decimal collectedAmount = await bal.GetCollectedMaintenanceAmountSB(StaffCode, startDate, endDate);
            decimal DueAmount = await bal.GetDueMaintenanceAmountSB(StaffCode, startDate, endDate);

            double collectedPercentage = TotalMaintenance == 0 ? 0 : Math.Round((double)(collectedAmount / TotalMaintenance) * 100, 1);
            double duePercentage = 100 - collectedPercentage;

            //----------------------------------------------------------------------------------------------------------------------
            //Expence 
            decimal TotalExpence = await bal.GetTotalExpenceAmountSB(StaffCode, startDate, endDate);
            decimal ExpenceAmount = await bal.GetCompletedExpenceAmountSB(StaffCode, startDate, endDate);
            decimal PendingExpenceAmount = await bal.GetPendingExpenceAmountSB(StaffCode, startDate, endDate);

            double ExpencePercentage = TotalExpence == 0 ? 0 : Math.Round((double)(ExpenceAmount / TotalExpence) * 100, 1);
            double PendingExpencePercentage = 100 - ExpencePercentage;

            //----------------------------------------------------------------------------------------------------------------------------

            var Guest = await bal.GetGuestVisitorCountSB(StaffCode, startDate, endDate);
            var Delivery = await bal.GetDeliveryVisitorCountSB(StaffCode, startDate, endDate);
            var Vendor = await bal.GetVendorVisitorCountSB(StaffCode, startDate, endDate);
            var Service = await bal.GetServiceVisitorCountSB(StaffCode, startDate, endDate);
            var Worker = await bal.GetWorkerVisitorCountSB(StaffCode, startDate, endDate);

            var data = new
            {
                HasMaintenanceData = TotalMaintenance > 0,
                HasExpenseData = TotalExpence > 0,

                //--------------------------------------------------

                CollectedMaintenance = collectedAmount.ToString("N2"),
                DueMaintenance = DueAmount.ToString("N2"),
                TotalMaintenance = TotalMaintenance.ToString("N2"),
                CollectedPercentage = collectedPercentage,
                DuePercentage = duePercentage,

                //-------------------------------------------------
                ExpenceAmount = ExpenceAmount.ToString("N2"),
                PendingExpenceAmount = PendingExpenceAmount.ToString("N2"),
                TotalExpence = TotalExpence.ToString("N2"),
                ExpencePercentage = ExpencePercentage,
                PendingExpencePercentage = PendingExpencePercentage,

                //------------------------------------------------------

                Guest = Guest.ToString(),
                Delivery = Delivery.ToString(),
                Vendor = Vendor.ToString(),
                Service = Service.ToString(),
                Worker = Worker.ToString()

            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CollectedMaintenanceListSB(DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();

            string StaffCode = Session["StaffCode"].ToString();

            var data = await bal.GetCollectedMaintenanceListSB(StaffCode, startDate, endDate);
            return PartialView("_CollectedMaintenancePartialSB", data);
        }

        public async Task<ActionResult> DueMaintenanceListSB(DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();

            string StaffCode = Session["StaffCode"].ToString();

            var data = await bal.GetDueMaintenanceListSB(StaffCode, startDate, endDate);
            return PartialView("_DueMaintenancePartialSB", data);
        }

        public async Task<ActionResult> CompletedExpenceListSB(DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();

            string StaffCode = Session["StaffCode"].ToString();

            var data = await bal.GetCompletedExpenceListSB(StaffCode, startDate, endDate);
            return PartialView("_CompletedExpencePartialSB", data);
        }

        public async Task<ActionResult> PendingExpenceListSB(DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();

            string StaffCode = Session["StaffCode"].ToString();

            var data = await bal.GetPendingExpenceListSB(StaffCode, startDate, endDate);
            return PartialView("_PendingExpencePartialSB", data);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        //Visitor Parking

        public async Task<JsonResult> GetVisitorParkingChartDataSB(DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();

            string StaffCode = Session["StaffCode"].ToString();

            var result = await bal.GetVisitorParkingCountSB(StaffCode, startDate, endDate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<PartialViewResult> GetOccupiedVisitorListSB(DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();
            string StaffCode = Session["StaffCode"].ToString();

            var list = await bal.GetOccupiedVisitorListSB(StaffCode, startDate, endDate);
            return PartialView("_OccupiedVisitorListPartialSB", list);
        }

        public async Task<ActionResult> GetUnoccupiedSlotListSB(DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();
            string StaffCode = Session["StaffCode"].ToString();
            var data = await bal.GetUnoccupiedSlotListSB(StaffCode, startDate, endDate);
            return PartialView("_UnoccupiedSlotListPartialSB", data);
        }


        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Today's Visitors (Guest,Delivery,Vendor, Worker)

        // Guest
        public async Task<PartialViewResult> GetGuestVisitorListSB(DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();
            string StaffCode = Session["StaffCode"].ToString();
            var list = await bal.GetGuestListSB(StaffCode, startDate, endDate);
            return PartialView("_GuestVisitorPartialSB", list);
        }
        // Delivery
        public async Task<PartialViewResult> GetDeliveryVisitorListSB(DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();
            string StaffCode = Session["StaffCode"].ToString();
            var list = await bal.GetDeliveryListSB(StaffCode, startDate, endDate);
            return PartialView("_DeliveryVisitorPartialSB", list);
        }

        // Vendor
        public async Task<PartialViewResult> GetVendorVisitorListSB(DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();
            string StaffCode = Session["StaffCode"].ToString();
            var list = await bal.GetVendorListSB(StaffCode, startDate, endDate);
            return PartialView("_VendorVisitorPartialSB", list);
        }

        // Service
        public async Task<PartialViewResult> GetServiceVisitorListSB(DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();
            string StaffCode = Session["StaffCode"].ToString();
            var list = await bal.GetServiceListSB(StaffCode, startDate, endDate);
            return PartialView("_ServiceVisitorPartialSB", list);
        }

        // Worker
        public async Task<PartialViewResult> GetWorkerVisitorListSB(DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();
            string StaffCode = Session["StaffCode"].ToString();
            var list = await bal.GetWorkerListSB(StaffCode, startDate, endDate);
            return PartialView("_WorkerVisitorPartialSB", list);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------

        // Worker Attendance

        public async Task<JsonResult> WorkerAttendanceChartDataSB(string timePeriod, DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();
            string StaffCode = Session["StaffCode"].ToString();
            var result = await bal.WorkerAttendaneDataSB(StaffCode, startDate, endDate);
            DataTable dt = result.Tables[0];

            if (timePeriod == "Today" || timePeriod == "Yesterday")
            {

                var data = dt.AsEnumerable().Select(row => new
                {
                    TotalWorkers = Convert.ToInt32(row["TotalWorkers"]),
                    PresentCount = Convert.ToInt32(row["PresentCount"]),
                    AbsentCount = Convert.ToInt32(row["AbsentCount"])
                }).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var data = dt.AsEnumerable().Select(row => new
                {
                    Date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd"),
                    TotalWorkers = Convert.ToInt32(row["TotalWorkers"]),
                    Present = Convert.ToInt32(row["PresentCount"]),
                    Absent = Convert.ToInt32(row["AbsentCount"])
                }).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<PartialViewResult> GetPresentWorkerListSB(DateTime? startDate = null, DateTime? endDate = null, DateTime? selectedDate = null)
        {
            BALSecretary bal = new BALSecretary();

            string StaffCode = Session["StaffCode"].ToString();

            if (selectedDate.HasValue)
            {
                startDate = selectedDate.Value.Date;
                endDate = selectedDate.Value.Date;
            }
            var list = await bal.GetPresentListSB(StaffCode, startDate, endDate);
            return PartialView("_PresentPartialSB", list);
        }

        public async Task<ActionResult> GetAbsentWorkerListSB(DateTime? startDate = null, DateTime? endDate = null, DateTime? selectedDate = null)
        {
            BALSecretary bal = new BALSecretary();

            string StaffCode = Session["StaffCode"].ToString();

            if (selectedDate.HasValue)
            {
                startDate = selectedDate.Value.Date;
                endDate = selectedDate.Value.Date;
            }
            var data = await bal.GetAbsentListSB(StaffCode, startDate, endDate);
            return PartialView("_AbsentPartialSB", data);
        }


        //-----------------------------------------------------------------------------------------------------------------------------------

        // Residant Complaints

        public async Task<JsonResult> ResidantComplaintsChartDataSB(DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();
            string StaffCode = Session["StaffCode"].ToString();

            var result = await bal.ResidantComplaintsDataSB(StaffCode, startDate, endDate);
            DataTable dt = result.Tables[0];

            var data = dt.AsEnumerable().Select(row => new
            {
                StatusId = Convert.ToInt32(row["StatusId"]),
                Status = row["Status"].ToString(),
                ComplaintCount = Convert.ToInt32(row["ComplaintCount"])
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public async Task<PartialViewResult> GetComplaintsByStatusSB(string status, DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();

            string StaffCode = Session["StaffCode"].ToString();

            if (status == "Inprogress")
            {
                var list = await bal.GetInProgressListSB(StaffCode, startDate, endDate);
                return PartialView("_InProgressPartialSB", list);
            }
            else if (status == "Pending")
            {
                var list = await bal.GetPendingListSB(StaffCode, startDate, endDate);
                return PartialView("_PendingPartialSB", list);
            }
            else if (status == "Escalate")
            {
                var list = await bal.GetEscalateListSB(StaffCode, startDate, endDate);
                return PartialView("_EscalatePartialSB", list);
            }
            else if (status == "Solved")
            {
                var list = await bal.GetSolvedListSB(StaffCode, startDate, endDate);
                return PartialView("_SolvedPartialSB", list);
            }


            return PartialView("_EmptyPartial");
        }

        //-----------------------------------------------------------------------------------------------------------------------------------

        // Upcoming Meetings 

        public async Task<JsonResult> GetUpcomingMeetingsSB(DateTime? UPMtoday = null)
        {
            BALSecretary bal = new BALSecretary();
            string StaffCode = Session["StaffCode"].ToString();

            var result = await bal.UpcomingMeetingSB(StaffCode, UPMtoday);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetUpcomingEventSB(DateTime? UPMtoday = null)
        {
            BALSecretary bal = new BALSecretary();
            string StaffCode = Session["StaffCode"].ToString();
            var result = await bal.UpcomingEventSB(StaffCode, UPMtoday);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> TOP_5_ExpencectlrSBgraph(DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();
            string StaffCode = Session["StaffCode"].ToString();
            var result = await bal.TOP_5_ExpenceSBgraph(StaffCode, startDate, endDate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> TOP_5_UnpaidMaintenanceCtlSBgraph(DateTime? startDate = null, DateTime? endDate = null)
        {
            BALSecretary bal = new BALSecretary();
            string StaffCode = Session["StaffCode"].ToString();
            var result = await bal.TOP_5_UnpaidMaintenanceCtlSBgraph(StaffCode, startDate, endDate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        #endregion

        #region*********************************************************************Ankita And Satwashil***********************************************************

        /// Loads the RatingCardGraph view with percentage-wise rating data (1 to 5 stars)
        /// for the currently assigned wing. This data is used to populate the rating chart.
       

        public async Task<ActionResult> RatingCardGraphAJ()
        {
            int assignWingId = Convert.ToInt32(Session["AssignWingId"]);

            Secretary objU = new Secretary();  // Final object to send to view
            List<Secretary> lstRatingData = new List<Secretary>(); // List to hold multiple rows

            DataSet ds = await obj.GetCountCardInGraph(assignWingId);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Secretary objRow = new Secretary();
                    objRow.Ratings = ds.Tables[0].Rows[i]["Ratings"] != DBNull.Value
                              ? Convert.ToSingle(ds.Tables[0].Rows[i]["Ratings"])
                              : (float?)null; objRow.RatingPercentage = Convert.ToDecimal(ds.Tables[0].Rows[i]["RatingPercentage"]);

                    lstRatingData.Add(objRow);
                }
            }

            objU.RatingPercentageList = lstRatingData;

            return View(objU);
        }

       
        /// Loads a partial view (_LoadStarFeedback) with feedback details for a specific star rating,
        /// filtered by the current user's assigned wing. This is typically triggered when the user clicks
      

        [HttpGet]
        public async Task<ActionResult> _LoadStarFeedbackAJ(int star)
        {
            try
            {
                Secretary model = new Secretary();
                model.FeedbackList = new List<Secretary>();
                model.FeedbackType = $"{star} Star";

                int assignWingId = Convert.ToInt32(Session["AssignWingId"]);
                DataSet ds = await obj.StartList(assignWingId, star); // This method should run your correct SQL query

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        model.FeedbackList.Add(new Secretary
                        {
                            FullName = row["FullName"]?.ToString(),
                            Ratings = row["Ratings"] != DBNull.Value ? Convert.ToSingle(row["Ratings"]) : (float?)null,
                            EventName = row["EventName"]?.ToString(), // ✅ Added this line
                            EventDate = row["EventDate"]?.ToString()
                        });
                    }
                }

                return PartialView("_LoadStarFeedbackAJ", model);
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }

        
        /// Retrieves event data using the BALSecretary layer, maps each row to a Secretary object,
        /// and returns the list as part of a model to a partial view.
        

        public async Task<ActionResult> _LoadEventListSD()
        {
            int assignWingId = Convert.ToInt32(Session["AssignWingId"]); // Move this line to the top

            DataSet ds = new DataSet();
            BALSecretary ac = new BALSecretary();
            ds = await ac.ViewEventSD(assignWingId); // Now this works correctly

            List<Secretary> list = new List<Secretary>();
            int srno = 1;

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Secretary obju = new Secretary
                    {
                        SrNo = srno++,
                        EventCode = row["EventCode"].ToString(),
                        EventName = row["EventName"].ToString(),
                        Organizer = row["StaffName"].ToString(),
                        FromDate = DateTime.TryParse(row["FromDate"]?.ToString(), out var date) ? date.Date : default,
                        ToDate = DateTime.TryParse(row["ToDate"]?.ToString(), out var date1) ? date1.Date : default,
                        Time = row["CreatedDate"].ToString(),
                        Location = row["PlaceName"].ToString()
                    };

                    list.Add(obju);
                }
            }

            Secretary objList = new Secretary
            {
                FeedbackList = list
            };

            return PartialView("_LoadEventListSD", objList);

        }
        #endregion


        #region*********************************************************************Sandhya Hivare Notice Management***********************************************************

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Account model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    BALSecretary bal = new BALSecretary();
                    DataSet dt = new DataSet();
                    dt = await bal.GetStaffDetailsByEmail(model.Email);
                    if (dt != null && dt.Tables.Count > 0 && dt.Tables[0].Rows.Count > 0)
                    {
                        Session["Email"] = dt.Tables[0].Rows[0]["Email"].ToString();
                        //Session["AppPassword"] = dt.Tables[0].Rows[0]["AppPassword"].ToString(); 
                        Session["StaffCode"] = dt.Tables[0].Rows[0]["StaffCode"].ToString();
                        if (dt.Tables[0].Rows[0]["AssignWingId"] != DBNull.Value)
                        {
                            Session["AssignWingId"] = dt.Tables[0].Rows[0]["AssignWingId"].ToString();
                        }

                        return RedirectToAction("Dashboard", "Home");

                    }
                    return RedirectToAction("Dashboard", "Home");
                }

                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Login error: " + ex.Message);
                }
            }

            return View(model);
        }




        [HttpGet]
        public async Task<ActionResult> GetListSH(DateTime? StartDate, DateTime? EndDate)
        {
            if (StartDate == null)
                StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            if (EndDate == null)
                EndDate = DateTime.Now;

            ViewBag.StartDate = StartDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = EndDate.Value.ToString("yyyy-MM-dd");

            DataSet ds = new DataSet();


            //DataSet ds = new DataSet();
            ds = await obj.ShowData(StartDate.Value, EndDate.Value);
            Secretary objUDetails = new Secretary();
            List<Secretary> lstUserDtl = new List<Secretary>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Secretary obju = new Secretary();
                obju.NoticeId = Convert.ToInt32(ds.Tables[0].Rows[i]["NoticeId"]);
                obju.NoticeCode = ds.Tables[0].Rows[i]["NoticeCode"].ToString();
                obju.NoticeTitle = ds.Tables[0].Rows[i]["NoticeTitle"].ToString();
                obju.Description = ds.Tables[0].Rows[i]["Description"].ToString();
                obju.StartDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["StartDate"].ToString());
                obju.DeadlineDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["DeadLineDate"].ToString());
                obju.SendTo = ds.Tables[0].Rows[i]["SendToDisplayName"].ToString();
                obju.SubType = ds.Tables[0].Rows[i]["SubType"].ToString();
                obju.Document = ds.Tables[0].Rows[i]["Document"].ToString();
                lstUserDtl.Add(obju);
            }

            objUDetails.UserList = lstUserDtl;
            return View(objUDetails);
        }
        public async Task<List<SelectListItem>> ForType()
        {
            DataSet ds = await Task.Run(() => new BALSecretary().GetForType());
            List<SelectListItem> ForTypeList = new List<SelectListItem>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ForTypeList.Add(new SelectListItem
                {
                    Value = dr["RoleId"].ToString(),
                    Text = dr["RoleName"].ToString()
                });
            }

            return ForTypeList;
        }

        public async Task<List<SelectListItem>> SUbType()
        {
            DataSet ds = await Task.Run(() => new BALSecretary().GetSubType());
            List<SelectListItem> SubTypeList = new List<SelectListItem>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                SubTypeList.Add(new SelectListItem
                {
                    Value = dr["SubTypeId"].ToString(),
                    Text = dr["SubType"].ToString()
                });
            }

            return SubTypeList;
        }




        public async Task<List<SelectListItem>> SocityCode()
        {
            DataSet ds = await Task.Run(() => new BALSecretary().GetSocityCode());
            List<SelectListItem> SocitCodeyList = new List<SelectListItem>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                SocitCodeyList.Add(new SelectListItem
                {
                    Value = dr["SocietyId"].ToString(),
                    Text = dr["SocityCode"].ToString()
                });
            }

            return SocitCodeyList;
        }

        public async Task<List<SelectListItem>> MemberCode(string staffCode)
        {
            DataSet ds = await Task.Run(() => new BALSecretary().GetMemberName(staffCode));
            List<SelectListItem> MemberNameList = new List<SelectListItem>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                MemberNameList.Add(new SelectListItem
                {
                    Value = dr["MemberCode"].ToString(),
                    Text = dr["FullName"].ToString()
                });
            }

            return MemberNameList;
        }


        public async Task<List<SelectListItem>> WingId(string wingId)
        {
            DataSet ds = await Task.Run(() => new BALSecretary().GetWingId(wingId));
            List<SelectListItem> WingList = new List<SelectListItem>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                WingList.Add(new SelectListItem
                {
                    Value = dr["WingId"].ToString(),
                    Text = dr["WingName"].ToString()
                });
            }

            return WingList;
        }

        [HttpGet]
        public async Task<JsonResult> GetSendToList(string type)
        {
            string staffCode = Session["StaffCode"]?.ToString();
            List<SelectListItem> list = new List<SelectListItem>();

            if (type == "Member")
            {
                list = await MemberCode(staffCode);  // ✅ directly use controller method
            }
            else if (type == "Wing")
            {
                list = await WingId("");  // ✅ directly use controller method
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public async Task<ActionResult> _RegisterSH()
        {
            Secretary model = new Secretary();
            BALSecretary bal = new BALSecretary();

            // Check if StaffCode exists in Session
            if (Session["StaffCode"] != null)
            {
                model.SendBy = Session["StaffCode"].ToString();
            }

            var roleDS = await bal.GetForType();
            var subtypeDS = await bal.GetSubType();
            // var memberNameDS = await bal.GetMemberName();
            string staffCode = Session["StaffCode"]?.ToString();
            var memberNameDS = await bal.GetMemberName(staffCode);


            if (roleDS.Tables.Count > 0)
            {
                model.Role = roleDS.Tables[0].AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["RoleId"].ToString(),
                    Text = row["RoleName"].ToString()
                }).ToList();
            }

            if (subtypeDS.Tables.Count > 0)
            {
                model.NoticeType = subtypeDS.Tables[0].AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["SubTypeId"].ToString(),
                    Text = row["SubType"].ToString()
                }).ToList();
            }

            if (memberNameDS.Tables.Count > 0 && memberNameDS.Tables[0].Rows.Count > 0)
            {
                model.FullNameList = memberNameDS.Tables[0].AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["MemberCode"].ToString(),
                    Text = row["FullName"].ToString()
                }).ToList();
            }
            //return View(model); 

            return PartialView("_RegisterSH", model);
        }


        [HttpPost]
        public async Task<ActionResult> _RegisterSH(Secretary objU, HttpPostedFileBase DocumentUpload)
        {
            if (Session["StaffCode"] != null)
            {
                objU.SendBy = Session["StaffCode"].ToString();
            }

            if (ModelState.IsValid)
            {
                objU.CreatedDate = DateTime.Now;

                // file upload
                if (DocumentUpload != null && DocumentUpload.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/Documents/Secretary");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string fileName = Path.GetFileName(DocumentUpload.FileName);
                    string filePath = Path.Combine(folderPath, fileName);
                    DocumentUpload.SaveAs(filePath);

                    objU.Document = fileName;  // ✅ Path नाही, फक्त file name save करा
                }
                else
                {
                    objU.Document = "NO FILE";
                }


                BALSecretary bal = new BALSecretary();
                await bal.SaveData(objU);

                TempData["Message"] = "Notice sent successfully.";
                TempData["AlertType"] = "success";
                return RedirectToAction("GetListSH");
                // return PartialView("Register", objU);
            }


            BALSecretary obj = new BALSecretary();

            var roleDS = await obj.GetForType();
            var subtypeDS = await obj.GetSubType();
            //var memberNameDS = await obj.GetMemberName();
            string staffCode = Session["StaffCode"]?.ToString();
            var memberNameDS = await obj.GetMemberName(staffCode);

            if (roleDS.Tables.Count > 0)
            {
                objU.Role = roleDS.Tables[0].AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["RoleId"].ToString(),
                    Text = row["RoleName"].ToString()
                }).ToList();
            }

            if (subtypeDS.Tables.Count > 0)
            {
                objU.NoticeType = subtypeDS.Tables[0].AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["SubTypeId"].ToString(),
                    Text = row["SubType"].ToString()
                }).ToList();
            }
            if (memberNameDS.Tables.Count > 0)
            {
                objU.FullNameList = memberNameDS.Tables[0].AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["MemberCode"].ToString(),
                    Text = row["FullName"].ToString()
                }).ToList();
            }

            return View("_RegisterSH", objU);


            //return View("GetList",objU);
        }

        [HttpGet]
        public async Task<ActionResult> _DetailsSH(int id)
        {
            Secretary user = new Secretary { NoticeId = id };
            SqlDataReader dr = await obj.GetById(user);

            if (dr.Read())
            {
                //user.NoticeId =  Convert.ToInt32(dr ["NoticeId"].ToString());
                user.NoticeCode = dr["NoticeCode"]?.ToString();
                //  user.SelectedNoticeType = Convert.ToInt32(dr["SubTypeId"]);
                user.SubType = dr["SubType"]?.ToString();
                user.NoticeTitle = dr["NoticeTitle"]?.ToString();
                user.Description = dr["Description"]?.ToString();
                user.StartDate = Convert.ToDateTime(dr["StartDate"]);
                user.DeadlineDate = Convert.ToDateTime(dr["DeadLineDate"]);
                // user.StartDate = Convert.DateTime(dr["StartDate"]).ToString("yyyy-MM-dd");
                user.Document = dr["Document"]?.ToString();

            }

            return PartialView("_DetailsSH", user);

        }


        
        public ActionResult ViewDocument(string fileName)
        {
            string folderPath = Server.MapPath("~/Documents/Secretary");
            string filePath = Path.Combine(folderPath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                string contentType = MimeMapping.GetMimeMapping(filePath);
                return File(filePath, contentType);
            }

            return HttpNotFound("File not found");
        }





        [HttpPost]
        public async Task<ActionResult> _EditSH(Secretary objU)
        {
            BALSecretary obj = new BALSecretary();
            await obj.UpdateData(objU.NoticeId, objU);
            return RedirectToAction("GetListSH");
        }

        [HttpGet]
        public async Task<ActionResult> _EditSH(int id)
        {
            Secretary objU = new Secretary { NoticeId = id };
            BALSecretary obj = new BALSecretary();

            using (SqlDataReader dr = await obj.GetById(objU))
            {
                if (dr.Read())
                {
                    objU.NoticeId = Convert.ToInt32(dr["NoticeId"]);
                    objU.NoticeTitle = dr["NoticeTitle"]?.ToString();
                    objU.Description = dr["Description"]?.ToString();
                    objU.StartDate = Convert.ToDateTime(dr["StartDate"]);
                    objU.DeadlineDate = Convert.ToDateTime(dr["DeadLineDate"]);
                    objU.SubType = dr["SubType"]?.ToString();

                    // ✅ Assign selected dropdown value
                    objU.SelectedNoticeType = dr["SubTypeId"] != DBNull.Value
                        ? Convert.ToInt32(dr["SubTypeId"])
                        : 0;

                }
            }

            var subtypeDS = await obj.GetSubType();
            if (subtypeDS.Tables.Count > 0)
            {
                objU.NoticeType = subtypeDS.Tables[0].AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["SubTypeId"].ToString(),
                    Text = row["SubType"].ToString()
                }).ToList();
            }

            return PartialView("_EditSH", objU);
        }

        #endregion


        #region*********************************************************************Ajay Ugale***********************************************************
        [HttpGet]
        public async Task<ActionResult> VisitorShowAU(DateTime? StartDate, DateTime? EndDate)
        {

            string StaffCode = Session["StaffCode"]?.ToString();


            if (StartDate == null)
                StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            if (EndDate == null)
                EndDate = DateTime.Now;

            ViewBag.StartDate = StartDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = EndDate.Value.ToString("yyyy-MM-dd");

            DataSet ds = new DataSet();

            ds = await obj.ShowList( StaffCode ,StartDate.Value, EndDate.Value);


            Secretary obj1 = new Secretary();
            List<Secretary> lst = new List<Secretary>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {

                Secretary obju = new Secretary();
                obju.VisitorName = ds.Tables[0].Rows[i]["VisitorName"].ToString();
                obju.Reason = ds.Tables[0].Rows[i]["Reason"].ToString();
                obju.FullName = ds.Tables[0].Rows[i]["OwnerName"].ToString();
                obju.VehicleNumber = ds.Tables[0].Rows[i]["VehicleNumber"].ToString();
                obju.FlatCode = ds.Tables[0].Rows[i]["FlatNo"].ToString();
                obju.VisitorContact = ds.Tables[0].Rows[i]["VisitorContact"].ToString();
                obju.CheckIn = Convert.ToDateTime(ds.Tables[0].Rows[i]["CheckIn"].ToString());

                if (ds.Tables[0].Rows[i]["CheckOut"] != DBNull.Value)
                {
                    obju.CheckOut = Convert.ToDateTime(ds.Tables[0].Rows[i]["CheckOut"]);
                }

                obju.Status = ds.Tables[0].Rows[i]["Status"].ToString();
                lst.Add(obju);
            }
            obj1.LstOfWorker = lst;

            return View("VisitorShowAU", obj1);
        }

        #endregion


        #region******************************************************* Vivek Kumbhar #Maintance# *****************************************************************************************

        [NonAction]
        public async Task<byte[]> GenerateMaintenancePDF(Secretary obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BALSecretary bal = new BALSecretary();

                Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // Define fonts
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, new BaseColor(34, 34, 34));
                var subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 14, new BaseColor(85, 85, 85));
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.BLACK);
                var totalFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                var messageFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, new BaseColor(102, 102, 102));

                // Define colors
                var headerColor = new BaseColor(70, 130, 180); // Steel Blue
                var alternateRowColor = new BaseColor(245, 245, 245); // Light Gray
                var totalRowColor = new BaseColor(34, 139, 34); // Forest Green
                var borderColor = new BaseColor(200, 200, 200); // Light border

                // ============ ENHANCED HEADER SECTION ============
                // Main Title with underline
                Paragraph mainTitle = new Paragraph("Maintenace Notice", titleFont);
                mainTitle.Alignment = Element.ALIGN_CENTER;
                mainTitle.SpacingAfter = 5f;
                doc.Add(mainTitle);

                // Add underline
                PdfPTable underlineTable = new PdfPTable(1);
                underlineTable.WidthPercentage = 60;
                underlineTable.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell underlineCell = new PdfPCell();
                underlineCell.Border = Rectangle.BOTTOM_BORDER;
                underlineCell.BorderColor = headerColor;
                underlineCell.BorderWidth = 2f;
                underlineCell.FixedHeight = 5f;
                underlineTable.AddCell(underlineCell);
                doc.Add(underlineTable);

                // Subtitle/Message Section
                string customMessage = "Green Valley Society "; // You can change this
                Paragraph subtitle = new Paragraph(customMessage, subtitleFont);
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 20f;
                doc.Add(subtitle);

                // Optional additional message
                string additionalMessage = "This document contains detailed breakdown of maintenance charges and schedule information.";

                if (!string.IsNullOrEmpty(additionalMessage))
                {
                    Paragraph messageText = new Paragraph(additionalMessage, messageFont);
                    messageText.Alignment = Element.ALIGN_CENTER;
                    messageText.SpacingAfter = 25f;
                    doc.Add(messageText);
                }


                PdfPTable mainInfoTable = new PdfPTable(1);
                mainInfoTable.WidthPercentage = 100;
                mainInfoTable.SpacingAfter = 20f;

                PdfPCell infoBorderCell = new PdfPCell();
                infoBorderCell.BorderColor = borderColor;
                infoBorderCell.BorderWidth = 1f;
                infoBorderCell.Padding = 15f;

                // Inner info table
                PdfPTable infoTable = new PdfPTable(2);
                infoTable.WidthPercentage = 100;
                infoTable.SetWidths(new float[] { 25f, 75f });

                // Info rows with better styling
                AddInfoRow(infoTable, "Schedule Name:", obj.MaintanceNamae, boldFont, normalFont);
                AddInfoRow(infoTable, "Total Amount:", $"₹{obj.TotalAmount:N2}", boldFont, normalFont);
                AddInfoRow(infoTable, "Start Date:", $"{obj.CreateDates:dd MMMM yyyy}", boldFont, normalFont);
                AddInfoRow(infoTable, "Deadline:", $"{obj.DeadlineDate:dd MMMM yyyy}", boldFont, normalFont);

                infoBorderCell.AddElement(infoTable);
                mainInfoTable.AddCell(infoBorderCell);
                doc.Add(mainInfoTable);

                // ============ MAINTENANCE BREAKDOWN TABLE ============
                // Section title
                Paragraph tableTitle = new Paragraph("MAINTENANCE BREAKDOWN", boldFont);
                tableTitle.SpacingBefore = 10f;
                tableTitle.SpacingAfter = 10f;
                doc.Add(tableTitle);

                // Main table
                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 10f, 60f, 30f }); // S.No, Name, Amount

                // Table Headers with enhanced styling
                AddHeaderCell(table, "S.No.", headerFont, headerColor);
                AddHeaderCell(table, "Maintenance Description", headerFont, headerColor);
                AddHeaderCell(table, "Amount (₹)", headerFont, headerColor);

                // Get maintenance details
                DataSet dt = new DataSet();
                dt = await bal.GetMaintenanceDetailsRecordVK(obj);

                // Data rows
                int serialNo = 1;
                decimal calculatedTotal = 0;

                foreach (var item in obj.MonthMaintenanceList)
                {
                    BaseColor rowColor = (serialNo % 2 == 0) ? alternateRowColor : BaseColor.WHITE;

                    // Serial Number
                    AddDataCell(table, serialNo.ToString(), normalFont, rowColor, Element.ALIGN_CENTER);

                    // Maintenance Name
                    AddDataCell(table, item.MainName, normalFont, rowColor, Element.ALIGN_LEFT);
                    if (obj.MainType != 40 || obj.MainType != 12)
                    {

                        item.Amount = item.Amount / obj.MemberCount;
                    }

                    // Amount
                    AddDataCell(table, $"₹{item.Amount:N2}", normalFont, rowColor, Element.ALIGN_RIGHT);

                    calculatedTotal += decimal.Parse(item.Amount.ToString());
                    serialNo++;
                }

                // Total row with enhanced styling
                PdfPCell totalLabelCell = new PdfPCell(new Phrase("GRAND TOTAL", totalFont));
                totalLabelCell.BackgroundColor = totalRowColor;
                totalLabelCell.HorizontalAlignment = Element.ALIGN_CENTER;
                totalLabelCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                totalLabelCell.Padding = 10f;
                totalLabelCell.Colspan = 2;
                totalLabelCell.BorderWidth = 1f;
                table.AddCell(totalLabelCell);

                PdfPCell totalAmountCell = new PdfPCell(new Phrase($"₹{calculatedTotal:N2}", totalFont));
                totalAmountCell.BackgroundColor = totalRowColor;
                totalAmountCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                totalAmountCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                totalAmountCell.Padding = 10f;
                totalAmountCell.BorderWidth = 1f;
                table.AddCell(totalAmountCell);

                doc.Add(table);

                // ============ FOOTER SECTION ============
                doc.Add(new Paragraph(" ", normalFont) { SpacingBefore = 20f });

                // Footer message
                Paragraph footerMessage = new Paragraph("Thank you for your prompt payment. For any queries, please contact the maintenance office.", messageFont);
                footerMessage.Alignment = Element.ALIGN_CENTER;
                footerMessage.SpacingAfter = 10f;
                doc.Add(footerMessage);

                // Generation info
                Paragraph generatedInfo = new Paragraph($"Generated on: {DateTime.Now:dd MMMM yyyy, hh:mm tt}",
                    FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.GRAY));
                generatedInfo.Alignment = Element.ALIGN_RIGHT;
                doc.Add(generatedInfo);

                doc.Close();
                return ms.ToArray();
            }

        }

        void AddInfoRow(PdfPTable table, string label, string value, iTextSharp.text.Font labelFont, iTextSharp.text.Font valueFont)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label, labelFont));
            labelCell.Border = Rectangle.NO_BORDER;
            labelCell.PaddingBottom = 8f;
            labelCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(labelCell);

            PdfPCell valueCell = new PdfPCell(new Phrase(value, valueFont));
            valueCell.Border = Rectangle.NO_BORDER;
            valueCell.PaddingBottom = 8f;
            valueCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(valueCell);
        }

        void AddHeaderCell(PdfPTable table, string text, iTextSharp.text.Font font, BaseColor backgroundColor)
        {
            PdfPCell headerCell = new PdfPCell(new Phrase(text, font));
            headerCell.BackgroundColor = backgroundColor;
            headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
            headerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            headerCell.Padding = 10f;
            headerCell.BorderWidth = 1f;
            headerCell.BorderColor = BaseColor.WHITE;
            table.AddCell(headerCell);
        }

        void AddDataCell(PdfPTable table, string text, iTextSharp.text.Font font, BaseColor backgroundColor, int alignment)
        {
            PdfPCell dataCell = new PdfPCell(new Phrase(text, font));
            dataCell.BackgroundColor = backgroundColor;
            dataCell.HorizontalAlignment = alignment;
            dataCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            dataCell.Padding = 8f;
            dataCell.BorderWidth = 0.5f;
            dataCell.BorderColor = BaseColor.WHITE;
            table.AddCell(dataCell);
        }

        public void SendMaintenanceEmailWithAttachment(List<string> recipientEmails, string subject, string body, byte[] pdfBytes, string pdfFileName)
        {

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("vivekkumbhar1009@gmail.com");

                foreach (var email in recipientEmails)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        mail.To.Add(email);
                    }
                }

                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                if (pdfBytes != null)
                {
                    Attachment pdfAttachment = new Attachment(new MemoryStream(pdfBytes), pdfFileName);
                    mail.Attachments.Add(pdfAttachment);
                }

                try
                {
                    using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential("vivekkumbhar1009@gmail.com", "gcpa uvxq tbrt vvml");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                catch (Exception ex)
                {
                    System.IO.File.WriteAllText(@"C:\EmailErrorLog.txt", ex.ToString());
                    throw;
                }
            }

        }

        public async Task<ActionResult> MainMaintenanceVk()
        {

            Secretary vk = new Secretary();
            BALSecretary bal = new BALSecretary();
            DataSet dt = new DataSet();
            dt = await bal.GetMaintenanceSubDetailsVK(vk);
            //vk.staffcode = Session["StaffCode"].ToString();
            if (Session["staffcode"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            vk.StaffCode = Session["staffcode"].ToString();
            ViewBag.paidlist = "paidlist";
            ViewBag.UNpaidlist = "UNpaidlist";
            return View(vk);

        }
        /// <summary>
        /// this method use for getting Schedule maintenance details  
        /// </summary>
        /// <returns> Schedule maintenance list dataset</returns>
        public async Task<ActionResult> ScheduleMaintenancesVK(string staffcode)
        {

            var model = new Secretary();
            Secretary vr = new Secretary();
            BALSecretary balsec = new BALSecretary();
            DataSet ds = new DataSet();
            vr.StaffCode = staffcode;
            SqlDataReader vk;
            vk = await balsec.GetWingResIDentCountVK(vr);
            if (vk.Read())
            {
                vr.MemberCount = Convert.ToInt32(vk["memcount"].ToString());
            }
            ds = await balsec.GetMaintenanceDetailsVK(vr);
            DateTime dt = DateTime.Now;
            List<Secretary> lstUserDtl = new List<Secretary>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DateTime vrs = Convert.ToDateTime(ds.Tables[0].Rows[i]["StartDate"].ToString());
                if (dt >= vrs)
                {

                    Secretary obju = new Secretary();
                    obju.StaffCode = staffcode;
                    obju.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["MaintenanceDetailsId"].ToString());
                    obju.MaintanceNamae = ds.Tables[0].Rows[i]["MaintenanceName"].ToString();
                    obju.MaintenanceCode = ds.Tables[0].Rows[i]["MaintenanceCode"].ToString();
                    obju.CreateDates = ds.Tables[0].Rows[i]["StartDate"].ToString();
                    obju.TotalMainAmount = decimal.Parse(ds.Tables[0].Rows[i]["Amount"].ToString());
                    obju.MemberCount = vr.MemberCount;
                    DateTime rk = Convert.ToDateTime(ds.Tables[0].Rows[i]["DeadlineDate"].ToString());
                    obju.CompleteDates = rk.ToShortDateString();
                    SqlDataReader dr = await balsec.GetMntHistoryVK(obju);
                    if (dr.Read())
                    {
                        double unpaidamount = Convert.ToDouble(dr["UnpaidAmount"].ToString());
                        if (unpaidamount > 0.00)
                        {
                            obju.TotalAmount = decimal.Parse(dr["TotalPaidAmount"].ToString());
                            obju.Amount = decimal.Parse(dr["UnpaidAmount"].ToString());
                            lstUserDtl.Add(obju);
                        }
                    }

                }
            }

            vr.List = lstUserDtl;

            return PartialView("_ScheduleMaintenancesVK", vr);
        }
        /// <summary>
        /// this method use for getting shedule maintenace list 
        /// </summary>
        /// <param name="staffcode"></param>
        /// <returns></returns>

        public async Task<ActionResult> ScheduleMaintenancesListVK(string staffcode)
        {

            var model = new Secretary();
            Secretary vr = new Secretary();
            BALSecretary balsec = new BALSecretary();
            DataSet ds = new DataSet();
            vr.StaffCode = staffcode;
            SqlDataReader vk;
            vk = await balsec.GetWingResIDentCountVK(vr);
            if (vk.Read())
            {
                vr.MemberCount = Convert.ToInt32(vk["memcount"].ToString());
            }
            ds = await balsec.GetMaintenanceDetailsVK(vr);
            DateTime dt = DateTime.Now;
            List<Secretary> lstUserDtl = new List<Secretary>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DateTime vrs = Convert.ToDateTime(ds.Tables[0].Rows[i]["StartDate"].ToString());
                if (dt < vrs)
                {
                    Secretary obju = new Secretary();
                    obju.StaffCode = staffcode;
                    obju.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["MaintenanceDetailsId"].ToString());
                    obju.MaintanceNamae = ds.Tables[0].Rows[i]["MaintenanceName"].ToString();
                    obju.MaintenanceCode = ds.Tables[0].Rows[i]["MaintenanceCode"].ToString();
                    obju.CreateDates = ds.Tables[0].Rows[i]["StartDate"].ToString();
                    obju.TotalMainAmount = decimal.Parse(ds.Tables[0].Rows[i]["Amount"].ToString());
                    obju.MemberCount = vr.MemberCount;
                    DateTime rk = Convert.ToDateTime(ds.Tables[0].Rows[i]["DeadlineDate"].ToString());
                    obju.CompleteDates = rk.ToShortDateString();
                    SqlDataReader dr = await balsec.GetMntHistoryVK(obju);
                    if (dr.Read())
                    {
                        double unpaidamount = Convert.ToDouble(dr["UnpaidAmount"].ToString());
                        if (unpaidamount > 0.00)
                        {
                            obju.TotalAmount = decimal.Parse(dr["TotalPaidAmount"].ToString());
                            obju.Amount = decimal.Parse(dr["UnpaidAmount"].ToString());
                            lstUserDtl.Add(obju);
                        }
                    }

                }

            }
            vr.List = lstUserDtl;

            return PartialView("_ScheduleMaintenancesListVK", vr);
        }
        /// <summary>
        /// this method use for display paid Mentenance Histroy
        /// </summary>
        /// <param name="staffcode"></param>
        /// <returns></returns>
        public async Task<ActionResult> MentenanceHistroyVK(string staffcode)
        {

            var model = new Secretary();
            Secretary vr = new Secretary();
            BALSecretary balsec = new BALSecretary();
            DataSet ds = new DataSet();
            vr.StaffCode = staffcode;
            SqlDataReader vp;
            vp = await balsec.GetWingResIDentCountVK(vr);
            if (vp.Read())
            {
                vr.MemberCount = Convert.ToInt32(vp["memcount"].ToString());
            }
            ds = await balsec.GetMaintenanceDetailsVK(vr);

            List<Secretary> lstUserDtl = new List<Secretary>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Secretary obju = new Secretary();
                obju.StaffCode = staffcode;
                obju.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["MaintenanceDetailsId"].ToString());
                obju.MaintanceNamae = ds.Tables[0].Rows[i]["MaintenanceName"].ToString();
                obju.MaintenanceCode = ds.Tables[0].Rows[i]["MaintenanceCode"].ToString();

                obju.CreateDates = ds.Tables[0].Rows[i]["StartDate"].ToString();
                obju.CompleteDates = ds.Tables[0].Rows[i]["DeadlineDate"].ToString();
                obju.MemberCount = vr.MemberCount;
                SqlDataReader dr = await balsec.GetMntHistoryVK(obju);
                if (dr.Read())
                {
                    double unpaidamount = Convert.ToDouble(dr["UnpaidAmount"].ToString());
                    if (unpaidamount <= 0.00)
                    {
                        SqlDataReader vk = await balsec.GetCompliDateVK(obju);
                        if (vk.Read())
                        {
                            DateTime rk = Convert.ToDateTime(vk["PaidDate"].ToString());
                            obju.CreateDates = rk.ToShortDateString();
                        }
                        obju.TotalAmount = decimal.Parse(dr["TotalPaidAmount"].ToString());
                        string membercount = obju.MemberCount.ToString();

                        obju.TotalMainAmount = obju.TotalAmount / obju.MemberCount;
                        lstUserDtl.Add(obju);
                    }
                }

            }

            vr.List = lstUserDtl;
            ViewBag.paid = "paid";
            return PartialView("_ScheduleMaintenancesVK", vr);
        }




        /// <summary>
        /// this is addschdulemaintenance 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> PendingMaintenanceVK(string staffcode)
        {
            Secretary vk = new Secretary();
            BALSecretary bal = new BALSecretary();
            DataSet dss = new DataSet();
            DataSet dt = new DataSet();
            vk.StaffCode = staffcode;
            dss = await bal.GetVendorList();
            SqlDataReader vp;
            vp = await bal.GetWingResIDentCountVK(vk);
            if (vp.Read())
            {
                vk.MemberCount = Convert.ToInt32(vp["memcount"].ToString());
            }
            return PartialView("_AddScheduleMaintenanceVK", vk);
        }
        /// <summary>
        /// this method is use to save database schedule maintenance 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> PendingMaintenanceVK(Secretary model, string staffcode)
        {
            BALSecretary bal = new BALSecretary();
            DataSet vendorDs = await bal.GetVendorList();
            DataSet floorDs = await bal.GetFloorList(model);
            model.StaffCode = staffcode;
            SqlDataReader d;
            d = await bal.GetWingsID(model);
            if (d.Read())
            {
                model.WingID = d["AssignWingId"].ToString();
            }


            if (ModelState.IsValid)
            {
                if (model.ID > 0)
                {
                    await bal.UpdateScheduleMainVK(model);
                }
                else
                {
                    await bal.SaveScheduleMainVK(model);
                }

                return Json(new { success = true });
            }


            ViewBag.mode = "addedit";
            return PartialView("_AddScheduleMaintenanceVK", model);
        }


        /// <summary>
        /// this method use to select maintenance details 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> DetailsVK(int id, string staffcode, string message)
        {
            float amountmaintance;
            BALSecretary bal = new BALSecretary();
            Secretary user = new Secretary();
            user.ID = id;
            user.StaffCode = staffcode;
            SqlDataReader dr;
            SqlDataReader vp;
            vp = await bal.GetWingResIDentCountVK(user);
            if (vp.Read())
            {
                user.MemberCount = Convert.ToInt32(vp["memcount"].ToString());
            }
            dr = await bal.GetDatailsVK(user);
            List<Secretary> lstUserDtl12 = new List<Secretary>();
            if (dr.Read())
            {
                Secretary obju = new Secretary();
                obju.MaintenanceCode = dr["MaintenanceCode"].ToString();
                DataSet dts = new DataSet();
                dts = await bal.GetMaintenanceSubDetailsVK(obju);
                for (int i = 0; i < dts.Tables[0].Rows.Count; i++)
                {
                    Secretary objus = new Secretary();
                    objus.StaffCode = staffcode;
                    objus.MainName = dts.Tables[0].Rows[i]["MaintanceNameID"].ToString();
                    objus.Amount = decimal.Parse(dts.Tables[0].Rows[i]["Amount"].ToString());
                    lstUserDtl12.Add(objus);
                }
                obju.MeetingList = lstUserDtl12;
                obju.StaffCode = staffcode;
                obju.MaintanceNamae = dr["MaintenanceName"].ToString();
                obju.MaintenanceCode = dr["MaintenanceCode"].ToString();
                obju.Description = dr["Description"].ToString();
                amountmaintance = float.Parse(dr["Amount"].ToString());
                DateTime dt = Convert.ToDateTime(dr["StartDate"].ToString());
                obju.CreateDates = dt.ToShortDateString();
                obju.CompleteDates = dr["DeadlineDate"].ToString();
                obju.StaffCode = staffcode;
                SqlDataReader ds = await bal.GetMntHistoryVK(obju);
                if (ds.Read())
                {
                    double unpaidamount = Convert.ToDouble(ds["UnpaidAmount"].ToString());

                    if (unpaidamount <= 0.00)
                    {
                        SqlDataReader vk = await bal.GetCompliDateVK(obju);
                        if (vk.Read())
                        {
                            DateTime rk = Convert.ToDateTime(vk["PaidDate"].ToString());
                            obju.CompleteDates = rk.ToShortDateString();
                        }
                        obju.Amount = 0;
                        obju.TotalAmount = decimal.Parse(ds["TotalPaidAmount"].ToString());
                        obju.TotalMntAmount = obju.Amount + obju.TotalAmount;
                        DataSet vrk = new DataSet();
                        vrk = await bal.GetPaidMemListVK(obju);
                        List<Secretary> paidlist = new List<Secretary>();
                        for (int i = 0; i < vrk.Tables[0].Rows.Count; i++)
                        {
                            Secretary obj = new Secretary();
                            obj.FloorCode = vrk.Tables[0].Rows[i]["FlatCode"].ToString();
                            obj.MemeberCode = vrk.Tables[0].Rows[i]["MemberCode"].ToString();
                            DateTime sk = Convert.ToDateTime(vrk.Tables[0].Rows[i]["PaidDate"].ToString());
                            obj.CreateDates = sk.ToShortDateString();
                            obj.FloorName = vrk.Tables[0].Rows[i]["Amount"].ToString();
                            paidlist.Add(obj);
                        }
                        obju.List = paidlist;

                        ViewBag.massage = message;
                        ViewBag.amountmaintance = amountmaintance;
                        return PartialView("_DetailsVK", obju);

                    }
                    else
                    {
                        obju.Amount = decimal.Parse(ds["UnpaidAmount"].ToString());
                        obju.TotalAmount = decimal.Parse(ds["TotalPaidAmount"].ToString());
                        obju.TotalMntAmount = obju.Amount + obju.TotalAmount;
                        DataSet vrk = new DataSet();
                        vrk = await bal.GetPaidMemListVK(obju);
                        List<Secretary> paidlist = new List<Secretary>();
                        for (int i = 0; i < vrk.Tables[0].Rows.Count; i++)
                        {
                            Secretary obj = new Secretary();
                            obj.FloorCode = vrk.Tables[0].Rows[i]["FlatCode"].ToString();
                            obj.MemeberCode = vrk.Tables[0].Rows[i]["MemberCode"].ToString();
                            DateTime sk = Convert.ToDateTime(vrk.Tables[0].Rows[i]["PaidDate"].ToString());
                            obj.CreateDates = sk.ToShortDateString();
                            obj.FloorName = vrk.Tables[0].Rows[i]["Amount"].ToString();
                            paidlist.Add(obj);
                        }
                        obju.List = paidlist;
                        DataSet srk = new DataSet();
                        srk = await bal.GetUnpaidMemListVK(obju);
                        List<Secretary> unpaidlist = new List<Secretary>();
                        for (int i = 0; i < srk.Tables[0].Rows.Count; i++)
                        {
                            Secretary obj = new Secretary();
                            obj.MemeberCode = srk.Tables[0].Rows[i]["MemberCode"].ToString();
                            obj.FloorCode = srk.Tables[0].Rows[i]["FlatCode"].ToString();
                            unpaidlist.Add(obj);
                        }
                        obju.MonthMaintenanceList = unpaidlist;
                        ViewBag.massage = message;
                        ViewBag.amountmaintance = amountmaintance;
                        return PartialView("_DetailsVK", obju);
                    }

                }
                ViewBag.amountmaintance = amountmaintance;
                ViewBag.massage = message;
                return PartialView("_DetailsVK", obju);
            }

            ViewBag.massage = message;
            return PartialView("_DetailsVK", user);
        }
        /// <summary>
        /// this is method use to select recored deleteted 
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> DeleteMaintenanceVK(int id)
        {
            Secretary bal = new Secretary();
            BALSecretary obj = new BALSecretary();
            bal.ID = id;
            await obj.DeleteMaintenance(bal);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<PartialViewResult> LoadScheduleMaintenanceVK(List<int> ids, string staffcode)
        {
            Secretary obj = new Secretary();
            BALSecretary bal = new BALSecretary();
            obj.StaffCode = staffcode;
            List<Secretary> maintenanceList = new List<Secretary>();
            decimal total = 0;
            for (int i = 0; i < ids.Count; i++)
            {
                Secretary sec = new Secretary();
                sec.ID = ids[i];

                SqlDataReader dr = await bal.GetselMainVK(sec);

                if (dr.Read())
                {
                    sec.MaintanceNamae = dr["MaintanceName"].ToString();
                    sec.Amount = decimal.Parse(dr["Amount"].ToString());
                    maintenanceList.Add(sec);
                    total = total + sec.Amount;
                }

            }

            obj.List = maintenanceList;

            ViewBag.TotalAmount = total;
            return PartialView("_MaintenanceScheduleVK", obj);
        }

        public async Task<ActionResult> GetMaintenanceListVK(string staffcode)
        {
            var model = new Secretary();
            Secretary vr = new Secretary();
            BALSecretary balsec = new BALSecretary();
            DataSet ds = new DataSet();
            vr.StaffCode = staffcode;
            ds = await balsec.GetFixMaintenanceVK(vr);
            List<Secretary> lstUserDtl = new List<Secretary>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Secretary obju = new Secretary();
                obju.ID = int.Parse(ds.Tables[0].Rows[i]["ID"].ToString());
                obju.MaintanceNamae = ds.Tables[0].Rows[i]["MaintanceName"].ToString();
                obju.Amount = decimal.Parse(ds.Tables[0].Rows[i]["Amount"].ToString());
                lstUserDtl.Add(obju);
            }
            vr.List = lstUserDtl;
            return View("_FixMaintenaceVK", vr);
        }
        [HttpPost]
        public async Task<ActionResult> SaveMaintenanceVK(Secretary obj, string staffcode)
        {
            BALSecretary bal = new BALSecretary();
            Secretary obju = new Secretary();
            obj.StaffCode = staffcode;
            if (obj.MaintenanceCode == null)
            {


                SqlDataReader vp;
                vp = await bal.GetWingResIDentCountVK(obj);
                if (vp.Read())
                {
                    obj.MemberCount = Convert.ToInt32(vp["memcount"].ToString());
                }
                SqlDataReader d;
                d = await bal.GetWingsID(obj);
                if (d.Read())
                {
                    obj.WingID = d["AssignWingId"].ToString();
                }
                if (obj.MaintanceNamae == "Monthly Maintenance")
                {
                    obj.MainType = 12;
                }
                else
                {
                    obj.MainType = 14;
                    obj.TotalAmount = obj.TotalAmount / obj.MemberCount;
                }
                await bal.SaveScheduleMainVK(obj);
                SqlDataReader dvt;
                dvt = await bal.GetMainteCodeVK(obj);
                if (dvt.Read())
                {
                    obju.MaintenanceCode = dvt["MaintenanceCode"].ToString();
                }

                foreach (var item in obj.MonthMaintenanceList)
                {

                    obju.MainName = item.MainName;
                    if (obj.MaintanceNamae == "Monthly Maintenance")
                    {
                        obju.Amount = item.Amount;
                    }
                    else
                    {

                        obju.Amount = item.Amount / obj.MemberCount;
                    }
                    obju.StartDate = Convert.ToDateTime(obj.CreateDates);
                    await bal.SaveMaintVK(obju);
                }
                obj.Amount = obju.Amount;

                //byte[] pdf =await GenerateMaintenancePDF(obj);


                //List<string> emails = new List<string>();

                //SqlDataReader rd = await bal.GetResidentEmailsVK(obj);
                //    while (rd.Read())
                //    {
                //        string emai = rd["emails"].ToString();
                //        emails.Add(emai);
                //    }

                //    string subject = "Monthly Maintenance Notice";
                //    string body = $@"
                //    <div style='font-family:Segoe UI, sans-serif; color:#333; max-width:600px; margin:auto; border:1px solid #ddd; padding:20px; border-radius:8px; box-shadow:0 0 10px rgba(0,0,0,0.05);'>
                //      <h2 style='color:#2e6da4; text-align:center;'> Monthly Maintenance Notice </h2>

                //      <p style='font-size:16px;'>Dear Member,</p>


                //        <br /><br />
                //        Please find below the details of this month’s maintenance schedule. We kindly request you to review the information and ensure the payment of 
                //        <strong style='color:#d9534f;'>₹{obj.TotalAmount:N2}</strong> is made by <strong>{obj.CompleteDates:dd MMMM yyyy}</strong>.
                //      </p>

                //      <div style='background-color:#f9f9f9; border-left:5px solid #5cb85c; padding:15px; margin:20px 0; font-size:14.5px;'>
                //        Your support is crucial to maintaining a clean, secure, and well-managed community. Thank you for your continued cooperation.
                //      </div>

                //      <p style='font-size:15px;'>
                //        Should you have any questions or require any assistance, please feel free to contact the Society Office.
                //      </p>
                //    </div>";
                //    SendMaintenanceEmailWithAttachment(emails, subject, body, pdf, "Maintenance.pdf");
                return Json(new { success = true });
            }
            else
            {
                SqlDataReader vp;
                vp = await bal.GetWingResIDentCountVK(obj);
                if (vp.Read())
                {
                    obj.MemberCount = Convert.ToInt32(vp["memcount"].ToString());
                }
                SqlDataReader d;
                d = await bal.GetWingsID(obj);
                if (d.Read())
                {
                    obj.WingID = d["AssignWingId"].ToString();
                }
                if (obj.MaintanceNamae == "Monthly Maintenance")
                {
                    obj.MainType = 12;
                }
                else
                {
                    obj.MainType = 14;
                    obj.TotalAmount = obj.TotalAmount / obj.MemberCount;
                }
                await bal.UpdateMaintenance(obj);
                await bal.DeleteSubMaintenanceVk(obj);

                foreach (var item in obj.MonthMaintenanceList)
                {
                    obju.MaintenanceCode = obj.MaintenanceCode;
                    obju.MainName = item.MainName;
                    if (obj.MaintanceNamae == "Monthly Maintenance")
                    {
                        obju.Amount = item.Amount;
                    }
                    else
                    {

                        obju.Amount = item.Amount / obj.MemberCount;
                    }
                    obju.StartDate = Convert.ToDateTime(obj.CreateDates);
                    await bal.SaveMaintVK(obju);
                }
                obj.Amount = obju.Amount;
                return Json(new { success = true });
            }
        }

        [HttpGet]
        public async Task<ActionResult> AddScheduleMaintenance()
        {
            Secretary vk = new Secretary();
            BALSecretary bal = new BALSecretary();
            DataSet dt = new DataSet();
            dt = await bal.GetFixMaintenanceVK(vk);
            return View("_AddScheduleMaintenanceVK");
        }
        [HttpGet]
        public async Task<ActionResult> ResidentDetailsVK(string membercode)
        {
            Secretary vr = new Secretary();
            BALSecretary bal = new BALSecretary();
            vr.MemeberCode = membercode;
            DataSet dt = new DataSet();
            SqlDataReader dr;
            dr = await bal.GetmemInfoVK(vr);
            if (dr.Read())
            {
                vr.FullName = dr["FullName"].ToString();
                vr.Adrees = dr["PermanentAddress"].ToString();
                vr.AlterNo = dr["AlternateNumber"].ToString();
                vr.MobailNo = dr["PhoneNumber"].ToString();
                vr.FlatCode = dr["FlatCode"].ToString();

            }
            dt = await bal.GetMemmenyListVK(vr);
            List<Secretary> lstUserDtl = new List<Secretary>();
            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {

                Secretary obju = new Secretary();

                obju.MaintanceNamae = dt.Tables[0].Rows[i]["MaintenanceName"].ToString();
                obju.Amount = decimal.Parse(dt.Tables[0].Rows[i]["paidamount"].ToString());
                DateTime sk = Convert.ToDateTime(dt.Tables[0].Rows[i]["PaidDate"].ToString());
                obju.CreateDates = sk.ToShortDateString();
                lstUserDtl.Add(obju);
            }
            vr.List = lstUserDtl;
            return View("_ResidentDetailsVK", vr);
        }

        public async Task<ActionResult> EditMainteanaceVK(int id, string staffcode)
        {
            Secretary obju = new Secretary();
            BALSecretary bal = new BALSecretary();
            obju.StaffCode = staffcode;
            obju.ID = id;
            SqlDataReader dr;
            SqlDataReader vp;
            vp = await bal.GetWingResIDentCountVK(obju);
            if (vp.Read())
            {
                obju.MemberCount = Convert.ToInt32(vp["memcount"].ToString());
            }
            dr = await bal.GetDatailsVK(obju);
            List<Secretary> lstUserDtl12 = new List<Secretary>();
            List<Secretary> lstUserDtl13 = new List<Secretary>();
            if (dr.Read())
            {

                obju.MaintenanceCode = dr["MaintenanceCode"].ToString();
                DataSet dts = new DataSet();
                dts = await bal.GetMaintenanceSubDetailsVK(obju);
                int type = int.Parse(dr["MaintenanceType"].ToString());
                if (type == 37 || type == 14)
                {

                    for (int i = 0; i < dts.Tables[0].Rows.Count; i++)
                    {
                        Secretary objus = new Secretary();

                        objus.MainName = dts.Tables[0].Rows[i]["MaintanceNameID"].ToString();
                        decimal amount = decimal.Parse(dts.Tables[0].Rows[i]["Amount"].ToString());
                        objus.Amount = amount * obju.MemberCount;
                        lstUserDtl12.Add(objus);
                    }
                    obju.MonthMaintenanceList = lstUserDtl12;
                    obju.MaintanceNamae = dr["MaintenanceName"].ToString();
                    obju.MaintenanceCode = dr["MaintenanceCode"].ToString();
                    obju.Description = dr["Description"].ToString();
                    float amountmaintance = float.Parse(dr["Amount"].ToString());
                    DateTime dt = Convert.ToDateTime(dr["StartDate"].ToString());
                    obju.StartDate = Convert.ToDateTime(dr["StartDate"].ToString());
                    obju.DeadlineDate = Convert.ToDateTime(dr["DeadlineDate"].ToString());


                    return View("_AddScheduleMaintenanceVK", obju);
                }
                else
                {
                    for (int i = 0; i < dts.Tables[0].Rows.Count; i++)
                    {
                        Secretary objus = new Secretary();

                        objus.MaintanceNamae = dts.Tables[0].Rows[i]["MaintanceNameID"].ToString();

                        objus.Amount = decimal.Parse(dts.Tables[0].Rows[i]["Amount"].ToString());
                        lstUserDtl12.Add(objus);
                    }
                    DataSet dtss = new DataSet();
                    dtss = await bal.suselectedfixmaintenace(obju);
                    for (int i = 0; i < dtss.Tables[0].Rows.Count; i++)
                    {
                        Secretary objus = new Secretary();

                        objus.MaintanceNamae = dtss.Tables[0].Rows[i]["MaintanceName"].ToString();
                        objus.Amount = decimal.Parse(dtss.Tables[0].Rows[i]["Amount"].ToString());
                        lstUserDtl13.Add(objus);
                    }
                    obju.List = lstUserDtl12;
                    obju.MonthMaintenanceList = lstUserDtl13;
                    obju.MaintanceNamae = dr["MaintenanceName"].ToString();
                    obju.MaintenanceCode = dr["MaintenanceCode"].ToString();
                    obju.Description = dr["Description"].ToString();
                    obju.TotalMainAmount = decimal.Parse(dr["Amount"].ToString());
                    DateTime dt = Convert.ToDateTime(dr["StartDate"].ToString());
                    obju.StartDate = Convert.ToDateTime(dr["StartDate"].ToString());
                    obju.DeadlineDate = Convert.ToDateTime(dr["DeadlineDate"].ToString());

                    return View("_MaintenanceScheduleVK", obju);
                }
            }
            return View("_AddScheduleMaintenanceVK", obju);
        }
        [HttpPost]
        public async Task DeleteMainteanaceVK(string id)
        {
            BALSecretary bal = new BALSecretary();
            Secretary obj = new Secretary();
            obj.MaintenanceCode = id;
            await bal.DeleteSubMaintenanceVk(obj);
            await bal.DeleteMaintenanceVK(obj);
        }
        #endregion

    }
}
       


    
