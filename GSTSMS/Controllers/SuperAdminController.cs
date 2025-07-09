using System;
using System.Collections.Generic;
using System.Linq;
using Razorpay.Api;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using GSTSMSLibrary.SuperAdmin;
using System.Data;
using System.Threading.Tasks;
using GSTSMSHelper;

namespace GSTSMS.Controllers
{
    public class SuperAdminController : Controller
    {
        // GET: SuperAdmin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LandingPage()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Subscription()
        {
            return View(); // If you're showing a standalone payment page
        }

        // =========================
        // 🔹 POST: Create Razorpay Order
        [HttpPost]
        public JsonResult CreateOrder(string planName, int amount)
        {
            try
            {
                string key = ConfigurationManager.AppSettings["RazorpayKey"];
                string secret = ConfigurationManager.AppSettings["RazorpaySecret"];

                RazorpayClient client = new RazorpayClient(key, secret);

                Dictionary<string, object> options = new Dictionary<string, object>
            {
                { "amount", amount * 100 }, // Razorpay accepts in paise
                { "currency", "INR" },
                { "receipt", "society_rcpt_" + Guid.NewGuid().ToString("N").Substring(0, 8) },
                { "payment_capture", 1 }
            };

                Razorpay.Api.Order order = client.Order.Create(options);

                return Json(new
                {
                    success = true,
                    orderId = order["id"].ToString(),
                    razorpayKey = key
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetPlansAndFeatures()
        {
            BALSuperAdmin bal = new BALSuperAdmin();

            var plans = await bal.GetAllPlansAsync();
            List<object> planData = new List<object>();

            foreach (DataRow plan in plans.Rows)
            {
                int planId = Convert.ToInt32(plan["PlanID"]);
                var features = await bal.GetFeaturesByPlanIdAsync(planId);

                List<string> featureList = new List<string>();
                foreach (DataRow feature in features.Rows)
                {
                    featureList.Add(feature["FeatureName"].ToString());
                }

                planData.Add(new
                {
                    PlanID = plan["PlanID"],
                    PlanName = plan["SubscriptionName"],
                    Duration = plan["Duration"],
                    Amount = plan["Amount"],
                    NoOfMember = plan["NoOfMember"],
                    Features = featureList
                });
            }

            return Json(planData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> CheckActiveSubscription(string email)
        {
            try
            {
                BALSuperAdmin bal = new BALSuperAdmin();
                var plan = await bal.CheckActiveSubscriptionAsync(email);

                if (plan != null)
                {
                    return Json(new
                    {
                        success = true,
                        plan = plan.Subscription_Name,
                        start = plan.StartDate.ToString("dd MMM yyyy"),
                        expiry = plan.ExpieryDate.ToString("dd MMM yyyy")
                    });

                }

                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<JsonResult> SendOtpIfEnquiryExists(string email)
        {
            BALSuperAdmin bal = new BALSuperAdmin();
            if (!await bal.IsEnquirySubmittedAsync(email))
            {
                return Json(new { success = false, message = "Oops! Looks like you haven’t submitted the enquiry form yet. Please fill it out first to continue with the subscription." });
            }

            string otp = new Random().Next(100000, 999999).ToString();
            Session["SignupOtp"] = otp;
            Session["OtpEmail"] = email;

            EmailHelper emailHelper = new EmailHelper();
            await emailHelper.SendSubscriptionOtpEmail(email, otp);

            return Json(new { success = true, message = "OTP sent to your email." });
        }


        [HttpPost]
        public JsonResult VerifyOtp(string enteredOtp)
        {
            string actualOtp = Session["SignupOtp"]?.ToString();

            if (actualOtp != null && actualOtp == enteredOtp)
            {
               
                Session.Remove("SignupOtp");
                Session.Remove("OtpEmail");

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> SaveSubscriptionAfterPayment(string planName, DateTime startDate, DateTime expiryDate, string chairmanEmail)
        {
            try
            {
                BALSuperAdmin bal = new BALSuperAdmin();
                int result = await bal.InsertSubscriptionAsync(planName, startDate, expiryDate, chairmanEmail);

                if (result > 0)
                {
                    return Json(new { success = true, message = "Subscription saved successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to save subscription." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<JsonResult> SendSubscriptionReceipt(string email, string planName, decimal amount, string duration, DateTime startDate, DateTime expiryDate, string paymentId)
        {
            try
            {
                EmailHelper helper = new EmailHelper();
                await helper.SendSubscriptionReceiptEmail(email, planName, amount, duration, startDate, expiryDate, paymentId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetPublicTestimonials()
        {
            try
            {
                BALSuperAdmin bal = new BALSuperAdmin();
                List<PublicTestimonial> testimonials = await bal.GetPublicTestimonialsAsync();

                return Json(testimonials, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}