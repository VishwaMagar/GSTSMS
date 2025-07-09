using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GSTSMSLibrary.Security
{
    public class Security
    {
        #region*********************************************************************Saniya Shaikh***********************************************************

        public string WorkerCode { get; set; }
        public string WorkerName { get; set; }
      
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }

        public string TimeType { get; set; }
        public Dictionary<string, string> AttendanceDays { get; set; } = new Dictionary<string, string>();
        public int SrNo { get; set; }
        public List<Security> Attendancelist { get; set; }

        #endregion


        #region*********************************************************************Saniya Shaikh***********************************************************
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }

        public int VisitorId { get; set; }
        public string VisitorCode { get; set; }
        public int VisitorTypeId { get; set; }
        public string Reason { get; set; }
        public string FlatCode { get; set; }
        public string VisitorName { get; set; }
        public string VisitorContact { get; set; }
        public string SelectedWing { get; set; }
        public int SelectedType { get; set; }
        public string ShiftTiming { get; set; }
        public int SelectedVehicleType { get; set; }
        public int SubTypeId { get; set; }
        public string SocietyCode { get; set; }
        public string VehicleNumber { get; set; }
        public string RoleName { get; set; }
        public string ServiceProvide { get; set; }
        public string ParkingCode { get; set; }
        public int StatusId { get; set; }
        public int VisitorCount { get; set; }
        public int FlatId { get; set; }
        public int PhoneNumber { get; set; }
        public string SubType { get; set; }
        public string VehicleType { get; set; }
        public string FloorName { get; set; }
        public string Status { get; set; }
        public string Document { get; set; }
        public List<Security> LstOfWorker { get; set; }
        public string WingName { get; set; }
        public List<SelectListItem> WingsName { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Flats { get; set; } = new List<SelectListItem>();

        #endregion

      

    }
}
