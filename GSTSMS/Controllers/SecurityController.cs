using GSTSMSLibrary.Secretary;
using GSTSMSLibrary.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GSTSMS.Controllers
{
    public class SecurityController : Controller
    {
        BALSecurity obj = new BALSecurity();

        // GET: Security
        public ActionResult Index()
        {
            return View();
        }

        //#region*********************************************************************Saniya Shaikh***********************************************************

        //public async Task<ActionResult> AttendanceofWorkerSS(DateTime? StartDate, DateTime? EndDate)
        //{
        //    // Set default dates if not provided
        //    if (StartDate == null)
        //        StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        //    if (EndDate == null)
        //        EndDate = DateTime.Today;

        //    // Ensure EndDate is not in the future
        //    if (EndDate > DateTime.Today)
        //        EndDate = DateTime.Today;

        //    ViewBag.StartDate = StartDate.Value.ToString("yyyy-MM-dd");
        //    ViewBag.EndDate = EndDate.Value.ToString("yyyy-MM-dd");

        //    // Calculate date range for column headers
        //    var dateRange = new List<DateTime>();
        //    for (var date = StartDate.Value; date <= EndDate.Value; date = date.AddDays(1))
        //    {
        //        dateRange.Add(date);
        //    }
        //    ViewBag.DateRange = dateRange;

        //    try
        //    {
        //        DataSet ds = await obj.AttendanceListSS(StartDate.Value, EndDate.Value);
        //        Security objUdetails = new Security();
        //        List<Security> attendancelist = new List<Security>();

        //        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //            {
        //                Security att = new Security();
        //                att.AttendanceDays = new Dictionary<string, string>();

        //                att.SrNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SrNo"].ToString());
        //                att.WorkerName = ds.Tables[0].Rows[i]["Employee Name"].ToString();
        //                att.TimeType = ds.Tables[0].Rows[i]["TimeType"].ToString();

        //                // Store attendance data in dictionary for flexible date range
        //                foreach (DateTime date in dateRange)
        //                {
        //                    string dayKey = date.Day.ToString();
        //                    if (ds.Tables[0].Columns.Contains(dayKey))
        //                    {
        //                        att.AttendanceDays[dayKey] = ds.Tables[0].Rows[i][dayKey].ToString();
        //                    }
        //                }

        //                // Keep existing Day properties for backward compatibility
        //                for (int day = 1; day <= 31; day++)
        //                {
        //                    if (ds.Tables[0].Columns.Contains(day.ToString()))
        //                    {
        //                        var propertyName = "Day" + day;
        //                        var property = att.GetType().GetProperty(propertyName);
        //                        if (property != null)
        //                        {
        //                            property.SetValue(att, ds.Tables[0].Rows[i][day.ToString()].ToString());
        //                        }
        //                    }
        //                }

        //                // Calculate totals
        //                if (ds.Tables[0].Columns.Contains("TotalPresent"))
        //                    att.TotalPresent = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalPresent"].ToString());

        //                if (ds.Tables[0].Columns.Contains("TotalAbsent"))
        //                    att.TotalAbsent = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalAbsent"].ToString());

        //                attendancelist.Add(att);
        //            }
        //        }

        //        objUdetails.Attendancelist = attendancelist;
        //        return View(objUdetails);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log error and return empty model
        //        Security objUdetails = new Security();
        //        objUdetails.Attendancelist = new List<Security>();
        //        ViewBag.ErrorMessage = "Error loading attendance data: " + ex.Message;
        //        return View(objUdetails);
        //    }
        //}

        //#endregion


        //#region*********************************************************************Ajay Ugale***********************************************************


        //public async Task<ActionResult> VisitorDetailsAU()
        //{


        //    DataSet ds = await obj.ShowlistAU();
        //    Security obj1 = new Security();
        //    List<Security> lst = new List<Security>();

        //    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //    {
        //        Security obju = new Security();
        //        obju.VisitorId = Convert.ToInt32(ds.Tables[0].Rows[i]["VisitorId"]);
        //        obju.VisitorName = ds.Tables[0].Rows[i]["VisitorName"].ToString();
        //        obju.SubType = ds.Tables[0].Rows[i]["VisitorType"].ToString();
        //        obju.VisitorCount = Convert.ToInt32(ds.Tables[0].Rows[i]["VisitorCount"]);
        //        obju.WingName = ds.Tables[0].Rows[i]["WingName"].ToString();
        //        obju.VehicleType = ds.Tables[0].Rows[i]["VehicleType"].ToString();
        //        obju.VehicleNumber = ds.Tables[0].Rows[i]["VehicleNumber"].ToString();
        //        obju.ParkingCode = ds.Tables[0].Rows[i]["ParkingCode"].ToString();
        //        obju.CheckIn = Convert.ToDateTime(ds.Tables[0].Rows[i]["CheckIn"]);
        //        obju.Status = ds.Tables[0].Rows[i]["Status"].ToString();

        //        // ✅ CheckOut जर null नसेल तरच assign करा
        //        if (ds.Tables[0].Rows[i]["CheckOut"] != DBNull.Value)
        //        {
        //            obju.CheckOut = Convert.ToDateTime(ds.Tables[0].Rows[i]["CheckOut"]);
        //        }

        //        lst.Add(obju);
        //    }

        //    obj1.LstOfWorker = lst;
        //    return View(obj1);
        //}

        //public async Task<ActionResult> _AddVisitorsAU()
        //{

        //    Security obj = new Security();
        //    await GetWing(obj);
        //    await GetType(obj);
        //    await GetVehicleType(obj);
        //    await GetStatus(obj);
        //    // await GetPark(obj);

        //    return PartialView("_AddVisitorsAU", obj);
        //}


        //// Controllers/YourControllerName.cs (e.g., SecurityController or HomeController)
        //public async Task<ActionResult> GetWing(Security objU)
        //{
        //    // Retrieve SocietyCode from session.
        //    // Make sure 'Session["SocietyCode"]' is set correctly during login.
        //    string societyCode = Session["SocietyCode"]?.ToString();

        //    BALSecurity obj = new BALSecurity();
        //    // Pass the retrieved societyCode to the BAL method
        //    DataSet ds = await obj.GetWing(societyCode);

        //    List<SelectListItem> wingList = new List<SelectListItem>();

        //    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in ds.Tables[0].Rows)
        //        {
        //            wingList.Add(new SelectListItem
        //            {
        //                Value = dr["WingId"].ToString(),
        //                Text = dr["WingName"].ToString()
        //            });
        //        }
        //    }

        //    objU.WingsName = wingList;
        //    return View("_AddVisitorsAU", objU);
        //}



        //[HttpGet]
        //public async Task<JsonResult> FlatBind(int WingId)
        //{
        //    DataSet ds = new DataSet();
        //    BALSecurity obj = new BALSecurity();
        //    ds = await obj.GetFlats(WingId);
        //    List<SelectListItem> flatlist = new List<SelectListItem>();

        //    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in ds.Tables[0].Rows)
        //        {
        //            flatlist.Add(new SelectListItem
        //            {
        //                Value = dr["FlatId"].ToString() + "|" + dr["FlatCode"].ToString(),
        //                Text = dr["FlatCode"].ToString()
        //            });
        //        }
        //    }

        //    return Json(flatlist, JsonRequestBehavior.AllowGet);
        //}




        //[HttpGet]
        //public async Task<ActionResult> GetType(Security objU)
        //{
        //    DataSet ds = new DataSet();
        //    BALSecurity obj = new BALSecurity();
        //    ds = await obj.GetVisitorType();


        //    List<Security> typelist = new List<Security>();
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        typelist.Add(new Security
        //        {
        //            SubTypeId = Convert.ToInt32(row["SubTypeId"]),

        //            SubType = row["SubType"].ToString()

        //        });
        //    }
        //    ViewBag.type = typelist;

        //    return View("_AddVisitorsAU", objU);
        //}

        //public async Task<ActionResult> GetVehicleType(Security objU)
        //{


        //    DataSet ds = new DataSet();
        //    BALSecurity obj = new BALSecurity();
        //    ds = await obj.GetVehicleType();


        //    List<Security> vltypelist = new List<Security>();
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        vltypelist.Add(new Security
        //        {
        //            SubTypeId = Convert.ToInt32(row["SubTypeId"]),

        //            SubType = row["SubType"].ToString()

        //        });
        //    }
        //    ViewBag.Vehicletype = vltypelist;

        //    return View("_AddVisitorsAU", objU);
        //}

        //public async Task<ActionResult> GetStatus(Security objU)
        //{
        //    DataSet ds = new DataSet();
        //    BALSecurity obj = new BALSecurity();
        //    ds = await obj.GetVisitorsStatus();
        //    List<Security> statuslist = new List<Security>();
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        statuslist.Add(new Security
        //        {
        //            StatusId = Convert.ToInt32(row["StatusId"]),

        //            Status = row["Status"].ToString()

        //        });
        //    }
        //    ViewBag.Status = statuslist;

        //    return View("_AddVisitorsAU", objU);
        //}



        //[HttpGet]
        //public async Task<JsonResult> GetPark(int typeid)
        //{
        //    DataSet ds = new DataSet();
        //    BALSecurity obj = new BALSecurity();
        //    ds = await obj.VisitorParking(typeid);
        //    List<SelectListItem> parkilist = new List<SelectListItem>();
        //    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in ds.Tables[0].Rows)
        //        {
        //            parkilist.Add(new SelectListItem
        //            {
        //                Value = dr["ParkingCode"].ToString(),
        //                Text = dr["ParkingCode"].ToString()
        //            });
        //        }
        //    }

        //    return Json(parkilist, JsonRequestBehavior.AllowGet);
        //}


        //[HttpPost]

        //public async Task<ActionResult> Addvisitors(Security objA)

        //{
        //    BALSecurity objAdmin = new BALSecurity();
        //    await objAdmin.SaveVisitors(objA);

        //    ViewBag.Message = "Data save succesfully";

        //    return View("_AddVisitorsAU");
        //}



        //[HttpPost]
        //public async Task<ActionResult> Visitorcode(Security objA, HttpPostedFileBase document, string selectedText, string visitorName, string visitor, string FlatId, string selectedwing)
        //{
        //    BALSecurity obj2 = new BALSecurity();
        //    DataSet ds = await obj2.AddVisitorCode();

        //    if (selectedText == "guest" || selectedText == "delivery" || selectedText == "service")
        //    {
        //        string name = ds.Tables[0].Rows[0]["VisitorCode"].ToString();
        //        int number = int.Parse(name.Substring(3)) + 1;
        //        string newCode = "VIS" + number.ToString("D3");
        //        objA.VisitorCode = newCode;
        //    }
        //    else if (selectedText == "worker" || selectedText == "vendor")
        //    {
        //        objA.VisitorCode = visitorName;
        //        objA.VisitorName = visitor;
        //    }

        //    if (selectedwing.StartsWith("SOC"))
        //    {
        //        objA.FlatCode = selectedwing;
        //    }

        //    // Store FlatId
        //    if (!string.IsNullOrEmpty(FlatId))
        //    {
        //        objA.FlatId = Convert.ToInt32(FlatId);
        //    }

        //    if (document != null && document.ContentLength > 0)
        //    {
        //        string fileName = Path.GetFileName(document.FileName);
        //        string folderPath = Server.MapPath("~/Content/VisitorImages");
        //        if (!Directory.Exists(folderPath))
        //        {
        //            Directory.CreateDirectory(folderPath);
        //        }

        //        string uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        //        string filePath = Path.Combine(folderPath, uniqueFileName);
        //        document.SaveAs(filePath);
        //        string relativePath = $"/Content/VisitorImages/{uniqueFileName}";
        //        objA.Document = relativePath;
        //    }

        //    await obj2.SaveVisitors(objA);
        //    await obj2.SaveDocuments(objA);

        //    TempData["Message"] = "Visitors added successfully.";
        //    return RedirectToAction("VisitorDetailsAU", objA);
        //}





        //[HttpGet]
        //public async Task<ActionResult> _ViewVisitorsAU(int VisitorId)
        //{


        //    SqlDataReader dr = await obj.ViewVisitors(VisitorId);


        //    Security obju = new Security();
        //    List<Security> lst = new List<Security>();
        //    if (dr.Read())
        //    {

        //        obju.VisitorId = Convert.ToInt32(dr["VisitorId"].ToString());
        //        obju.VisitorName = dr["VisitorName"].ToString();
        //        obju.SubType = dr["VisitorType"].ToString();
        //        obju.VisitorCount = Convert.ToInt32(dr["VisitorCount"].ToString());
        //        obju.WingName = dr["WingName"].ToString();
        //        obju.FlatCode = dr["FlatCode"].ToString();
        //        obju.FloorName = dr["FloorName"].ToString();
        //        obju.VehicleType = dr["VehicleType"].ToString();
        //        obju.VehicleNumber = dr["VehicleNumber"].ToString();
        //        obju.ParkingCode = dr["ParkingCode"].ToString();
        //        obju.CheckIn = Convert.ToDateTime(dr["CheckIn"].ToString());

        //        if (dr["CheckOut"] != DBNull.Value && !string.IsNullOrEmpty(dr["CheckOut"].ToString()))
        //        {
        //            obju.CheckOut = Convert.ToDateTime(dr["CheckOut"].ToString());
        //        }
        //        else
        //        {
        //            obju.CheckOut = null;
        //        }

        //        // obju.Date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd "));
        //        obju.Status = dr["Status"].ToString();
        //        obju.Document = dr["Document"].ToString();

        //    }


        //    return PartialView("_ViewVisitorsAU", obju);
        //}

        //[HttpPost]
        //public JsonResult CheckOut(int id)
        //{
        //    try
        //    {
        //        BALSecurity obj = new BALSecurity();
        //        obj.Checkout(new Security(), id);

        //        return Json(new
        //        {
        //            success = true,
        //            message = "Checked out successfully!",
        //            checkoutTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
        //            redirectUrl = Url.Action("VisitorDetailsAU", "Security") // जेथे redirect करायचं आहे
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Error: " + ex.Message });
        //    }
        //}


        //[HttpGet]
        //public async Task<JsonResult> GetWorkers()
        //{
        //    try
        //    {
        //        var ds = await new BALSecurity().ShowWorker();
        //        var workerList = new List<object>();

        //        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            foreach (DataRow dr in ds.Tables[0].Rows)
        //            {
        //                workerList.Add(new
        //                {
        //                    Value = dr["WorkerCode"].ToString(),
        //                    Text = dr["WorkerName"].ToString(),
        //                    ContactNo = dr["ContactNo"].ToString(),
        //                    WingName = dr["WingName"].ToString(),
        //                    ShiftTiming = dr["ShiftTiming"].ToString(),
        //                    Document = dr["Document"].ToString(),
        //                    RoleName = dr["SubType"].ToString()  // Add this line
        //                });
        //            }
        //        }

        //        return Json(workerList, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            error = true,
        //            message = "An error occurred while fetching worker list.",
        //            details = ex.Message
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[HttpGet]
        //public async Task<JsonResult> GetVendor()
        //{
        //    try
        //    {
        //        var ds = await new BALSecurity().Showvendor();
        //        var vendorList = new List<object>();

        //        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            foreach (DataRow dr in ds.Tables[0].Rows)
        //            {
        //                vendorList.Add(new
        //                {
        //                    Value = dr["VendorCode"].ToString(),
        //                    Text = dr["VendorName"].ToString(),
        //                    PhoneNumber = dr["PhoneNumber"].ToString(),
        //                    ServiceProvide = dr["ServiceProvide"].ToString() // Add this
        //                });
        //            }
        //        }

        //        return Json(vendorList, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            error = true,
        //            message = "An error occurred while fetching vendor list.",
        //            details = ex.Message
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //#endregion



    }
}
