using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace GSTSMSLibrary.Secretary
{
    public class Secretary
    {
        #region*********************************************************************Priti Chavhan***********************************************************

        public int StaffId { get; set; }
        public int RoleId { get; set; }
        public string Password { get; set; }
        public string StaffName { get; set; }
        public string ProfilePic { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Document { get; set; }

        public int PolicyId { get; set; }
        public string PolicyName { get; set; }
        public string CreatedBy { get; set; }
        public string PenaltyCost { get; set; }
        public int TermsId { get; set; }
        public string Terms_Code { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
        public string FullName { get; set; }
        public string SocietyCode { get; set; }
        public String Name { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Secretary> TermsAndConditions { get; set; } = new List<Secretary>(); // Initialize to avoid null reference
        #endregion

        #region*********************************************************************Payal Gogawale***********************************************************

        [Required(ErrorMessage = "Vendor Name is required.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Vendor Name must contain only alphabets.")]
        public string VendorName { get; set; }

        [Required(ErrorMessage = "Service Provider is required.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Service Provider must contain only alphabets.")]
        public string ServiceProvide { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone Number must be 10 digits.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Joining Date is required.")]
        [DataType(DataType.Date)]
        public DateTime? JoiningDate { get; set; }
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Alternate Phone Number must be 10 digits.")]
        public string AlternatePhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Account Number is required.")]
        [RegularExpression(@"^\d{9,18}$", ErrorMessage = "Account Number must be between 9 to 18 digits.")]
        public string AccountNo { get; set; }

        [Required(ErrorMessage = "IFSC Code is required.")]
        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC Code format.")]
        public string IFSCCode { get; set; }

        [Required(ErrorMessage = "Bank For is required.")]
        public string BankFor { get; set; }

        [Required(ErrorMessage = "Account Type is required.")]
        public string AccountType { get; set; }

        [RegularExpression(@"^[\w.-]+@[\w.-]+$", ErrorMessage = "Invalid UPI ID.")]
        public string UPIId { get; set; }

        public decimal? OpeningBalance { get; set; }

        [Display(Name = "Create Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string VendorCode { get; set; }
        public List<Secretary> VendorList { get; set; }
        public string StaffCode { get; set; }
        public string Status { get; set; }
        #endregion

        #region*********************************************************************Sanika Jaykar***********************************************************

        public string Type { get; set; }
        public string MeetingTitle { get; set; }
        public int EventId { get; set; }
        public int MeetingId { get; set; }
        public string MeetingAgenda { get; set; }
        public DateTime SendTime { get; set; }
        public int NotificationId { get; set; }
        public string EventCode { get; set; }
        public string MeetingCode { get; set; }
        public float? Ratings { get; set; }
        #endregion

        #region*********************************************************************Suyog Kulkarni***********************************************************

        public List<Secretary> AllFacility { get; set; }

        public int FacilityId { get; set; }

        [Required(ErrorMessage = "Facility Name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Facility Name must be between 3-100 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-\.\,\(\)]*$", ErrorMessage = "Invalid characters in Facility Name")]
        public string FacilityName { get; set; }
        public string Path { get; set; }
        #endregion

        #region*********************************************************************Sanika Patil Secratery Report***********************************************************
        public int SrNo { get; set; }
        public string PaymentMode { get; set; }
        public string Dates { get; set; }
        public int Total { get; set; }
        public int Present { get; set; }
        public int PresentDelivery { get; set; }
        public int PresentGuest { get; set; }
        public int PresentWorker { get; set; }
        public int PresentVendor { get; set; }
        public DateTime ComplaintDate { get; set; }
        public int Solve { get; set; }
        public int Pending { get; set; }
        public int Inprogress { get; set; }
        public int Delivery { get; set; }
        public int Guest { get; set; }
        public int Worker { get; set; }
        public int Vendor { get; set; }
        public string WorkerName { get; set; }
        public string ExpenseName { get; set; }
        public int Escalate { get; set; }
        public int PaidMaintenance { get; set; }
        public int UnPaidMaintenance { get; set; }
        public string MemberCode { get; set; }
        public string VisitorName { get; set; }
        public string VisitorContact { get; set; }
        public string VisitorCode { get; set; }
        public string VehicleNumber { get; set; }
        public string Reason { get; set; }
        public string FlatCode { get; set; }
        public int PaidMembers { get; set; }
        public int UnPaidMembers { get; set; }
        public string ParkingCode { get; set; }
        public string SlotStatus { get; set; }
        public string ParkingType { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string WingName { get; set; }
        public int ParkingSlotId { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public DateTime PaidDate { get; set; }
        public string MaintenanceName { get; set; }
        public string ExpenseCode { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidExpense { get; set; }
        public decimal UnpaidExpense { get; set; }
        public decimal TotalExpense { get; set; }
        public string TransactionId { get; set; }


        #endregion

        #region********************************************************Siddhesh Balkavade Dashboard*****************************************************

        public string FloorName { get; set; }

        public int TotalVisitorSlots { get; set; }

        public int OccupiedSlots { get; set; }

        public int UnoccupiedSlots { get; set; }

        public string ExpenceType { get; set; }

        public DateTime Date { get; set; }

        public DateTime MeetingDate { get; set; }
        
        public string MeetingLocation { get; set; }

        public string EventName { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public string Place { get; set; }

        public string VisitorType { get; set; }

        public string Flat { get; set; }

        public string MaintenceName { get; set; }

        public string ExpenceFor { get; set; }

        public string VehicleType { get; set; }

        #endregion


        #region*********************************************************************Ankita And Satwashil***********************************************************
        public decimal RatingPercentage { get; set; }
        public List<Secretary> FeedbackList { get; set; } = new List<Secretary>();
        public string Time { get; set; }
        public string Organizer { get; set; }
        public string FeedbackType { get; set; }
        public string EventDate { get; set; }
        public List<Secretary> RatingPercentageList { get; set; }
        #endregion

        #region*********************************************************************Sandhya Hivare Notice Management ***********************************************************

        public string DisplayType { get; set; }
        public int NoticeId { get; set; }

        [Display(Name = "Notice Code")]
        public string NoticeCode { get; set; }
        [Display(Name = "Notice Title")]
        [Required(ErrorMessage = "Insert into Notice Title")]
        public string NoticeTitle { get; set; }

        [Required(ErrorMessage = "Please select Start Date.")]
        [DataType(DataType.Date)]

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Please select DeadLine Date.")]
        [DataType(DataType.Date)]

        [Display(Name = "DeadLine Date")]
        public DateTime DeadlineDate { get; set; }

        public string SendTo { get; set; }

        public string SendBy { get; set; }


        [Display(Name = "Sub Type")]
        public string SubType { get; set; }

        public string SelectedFullName { get; set; }

        [Display(Name = "Notice Type")]
        public int SelectedNoticeType { get; set; }

        [Display(Name = "For Type")]
        public int SelectedForType { get; set; }

        [Display(Name = "Society Code")]
        public string SelectedSocityCode { get; set; }
        public string SelectedMember { get; set; }

        public string Members { get; set; }
        public string SendToType { get; set; }

        public List<SelectListItem> FullNameList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> WingList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> SocityCode { get; set; } = new List<SelectListItem>();

        // public List<SelectListItem> SocietyCode { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Role { get; set; } = new List<SelectListItem>();

        [Display(Name = "Notice Type")]
        public List<SelectListItem> NoticeType { get; set; } = new List<SelectListItem>();

        public List<Secretary> UserList { get; set; } = new List<Secretary>();

        #endregion

        #region*********************************************************************Ajay Ugale***********************************************************

        public List<Secretary> LstOfWorker { get; set; }

        #endregion


        #region*********************************************************************Vivek Kumbhar***********************************************************

        public int ID { get; set; }
        public string MaintanceNamae { get; set; }
        public string MainName { get; set; }
        public int MemberCount { get; set; }
        public string MemeberCode { get; set; }
        public string FloorCode { get; set; }

        public decimal TotalMntAmount { get; set; }
        public int MainType { get; set; }

        public string MaintenanceCode { get; set; }
        public string MobailNo { get; set; }
        public decimal TotalMainAmount { get; set; }
       
        public List<Secretary> MonthMaintenanceList { get; set; }
        public List<Secretary> List { get; set; }
        public List<Secretary> MeetingList { get; set; }
        public string WingID { get; set; }
        public string CreateDates { get; set; }
        public string CompleteDates { get; set; }

        public string Adrees { get; set; }
        public string AlterNo { get; set; }
        #endregion
    }
}
