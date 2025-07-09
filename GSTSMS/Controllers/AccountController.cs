


using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using GSTSMSHelper;
using GSTSMSLibrary.Account;

namespace GSTSMS.Controllers
{
    public class AccountController : Controller
    {
        BALAccount obj = new BALAccount();

        [HttpGet]
        public ActionResult LoginRK()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginRK(Account model)
        {
            if (!ModelState.IsValid)
                return View(model);

            SqlDataReader dr = await obj.Login(model);

            if (dr.Read())
            {
                Session["Email"] = dr["Email"].ToString();
                Session["Password"] = dr["Password"].ToString();
                Session["RoleId"] = Convert.ToInt32(dr["RoleId"]);
                Session["RoleName"] = dr["RoleName"].ToString();
                //Session["ProfilePic"] = dr["ProfilePic"] != DBNull.Value
                //    ? dr["ProfilePic"].ToString()
                //    : "~/Images/default-profile.png";

                string roleId = dr["RoleId"].ToString();

                if (roleId == "2")
                {
                    return RedirectToAction("Dashboard", "AccountManager");
                }
                else
                {
                    ViewBag.Message = "Unauthorized role.";
                    return View(model);
                }
            }

            ViewBag.Message = "Invalid email or password.";
            return View(model);
        }

         //---------------- LANDING PAGE ---------------- //
        [HttpGet]
        public ActionResult LandingPage()
        {
            return View();
        }



        

    }

}







