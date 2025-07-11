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


    
    }
}
       


    
