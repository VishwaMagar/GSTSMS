using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Xml.Linq;

namespace GSTSMSLibrary.AccountManager
{
    public class AccountManager
    {


        #region********************************************************************* Society Ac.Details Bank ***********************************************************



        public string BankCode { get; set; }
        //public string Accountnumber { get; set; }
        public string IFSCCode { get; set; }
        //public string UpiId { get; set; }
        public decimal OpeningBalance { get; set; }
       // public int AccountTypeid { get; set; }
        public string AllCode { get; set; }
        public bool IsActive { get; set; } = true;
        public string BankName { get; set; }
        public string Branch { get; set; }
        public string BankHolderName { get; set; }
        public string AccountNo { get; set; }
        public DateTime AddedDate { get; set; }
        public int BankId { get; set; }

        public decimal Amount { get; set; }
        public string PaymentByName { get; set; }
        public List<AccountManager> TransactionList { get; set; }

#endregion

        #region********************************************************************* Maintaince Management ***********************************************************


        public int SrNo { get; set; }
        public string WingName { get; set; }
        public string FlatType { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string Status { get; set; }

        public DateTime? PaidDate { get; set; }

        public string MaintenanceId { get; set; }
        public decimal ChargeAmount { get; set; }
        public string EntityCode { get; set; }


        public string MemberCode { get; set; }


        public int? MaintananceTypeId { get; set; }


#endregion

        #region********************************************************************* Community Send Email ***********************************************************



        public string FromEmailAddress { get; set; }

        public string ToEmailAddress { get; set; }

        public List<string> ToEmailAddresses { get; set; }

        public string Subject { get; set; }

        public string EmailBodyMessage { get; set; }

        public string Attachment { get; set; }

        public string FlatCode { get; set; }
        public bool IsSelected { get; set; }  // Helps retain checkbox state after POST

        public int MemberId { get; set; }
        public int StaffId { get; set; }
        public string StaffCode { get; set; }
        public string ContactNumber { get; set; }
        public List<string> ContactNumbers { get; set; }



        public string WhatsAppMessage { get; set; }

        #endregion







       


        #region********************************************************************* Community Notice  ***********************************************************

        public string NoticeCode { get; set; }
        public string NoticeTitle { get; set; }
        public string Description { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public string Document { get; set; }
        public string DocumentPath { get; set; }
        public int SubTypeId { get; set; } = 71;

     
        public List<string> AttachmentList { get; set; }

        public string SendBy { get; set; }
        public string SendByRole { get; set; }

      
        public List<string> SelectedMemberCodes { get; set; }

        public int WingId { get; set; }
     
        public int NoticeAnnouncementId { get; set; }
        public string NoticeAnnouncementCode { get; set; }




        #endregion


        #region********************************************************************* Expence ***********************************************************

        // Transaction
        public string TransactionCode { get; set; }
        public decimal TransactionAmount { get; set; }
        public string PaymentMode { get; set; }


        public int DocumentId { get; set; }
       

        public int ExpenseId { get; set; }
        public string ExpenseCode { get; set; }
        public string PaymentTo { get; set; }
        public int ExpenseTypeId { get; set; }
        //public string ExpenseTypeName { get; set; }
        public string ExpenseName { get; set; }
        public string AddedBy { get; set; }
      
        public int StatusId { get; set; }
        public string StatusName { get; set; }

        // --- Vendor Info ---
        public string VendorName { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public string Address { get; set; }

        // --- Documents ---
        public string Document1 { get; set; } // PAN or any other doc
        public string Document2 { get; set; } // GST or other doc


        //public DateTime? DocumentDate { get; set; } = DateTime.Now;

        // --- Bank Info ---
      
        public string UPIId { get; set; }
        public int AccountTypeId { get; set; } // fixed

       


        public int ServiceSubTypeId { get; set; }

        public string SubTypeName { get; set; }




        public string ExpenseType { get; set; }
        public string VendorCode { get; set; }

        //public string GSTType { get; set; }

        public String DocumentBase64String { get; set; }

        //public string ExpenseFor { get; set; }
        //public string VendorType { get; set; }

        public DateTime Date { get; set; }

        //public int AmountToPay { get; set; }

        //public string StaffName { get; set; }

        public int GSTTypeId { get; set; }
        public string GSTTypeName { get; set; }

        #endregion


        #region********************************************************************* Event Management ***********************************************************


        public int EBudgetId { get; set; }
        public string EventCode { get; set; }
        public string EventName { get; set; }
        public string EventHandlerName { get; set; }

        public decimal? AllocatedBudget { get; set; }
        public decimal? ActualCost { get; set; }
        public DateTime? BudgetAddedDate { get; set; }
        public int BudgetStatus { get; set; }
        public string BudgetStatusName { get; set; }

        public string BankAccount { get; set; }


        public string TransactionId_ChequeId { get; set; }
        public string PaymentId { get; set; }
        public int PaymentModeId { get; set; }


        #endregion



        #region********************************************************************* Worker Pay.Manage ***********************************************************

       
        public string Password { get; set; }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string StaffName { get; set; }
       
        public string ProfilePic { get; set; }

        public string BankName_Code { get; set; }
       
        public string PaymentBy { get; set; }
        public string PaidTo { get; set; }
       
        public string PaymentPurpose { get; set; }
       
        public int TransactionType { get; set; }

        public string WorkerCode { get; set; }
        public string AttendanceMonth { get; set; }
        public string Contact { get; set; }
        public DateTime DateOfJoining { get; set; }
        public decimal BaseSalary { get; set; }
        public string AccountNumber { get; set; }
       
        public string WorkerUPI { get; set; }
        public int DaysPresent { get; set; }
        public decimal PerdayPayment { get; set; }
        public decimal AmountToBePaid { get; set; }

        public string PaymentStatus { get; set; }
        public string TransactionRef { get; set; }
        public DateTime MonthDate { get; set; }

        public bool IsPaid { get; set; }
        public int SerialNumber { get; set; }
       

        #endregion



        #region********************************************************************* Reports  ***********************************************************



        public DateTime PossessionDate { get; set; }
        public string Gender { get; set; }
        public int FamilyMemberCount { get; set; }
        public int NoOfVehicle { get; set; }
        public DateTime RegisterationDate { get; set; }

        public string WorkerName { get; set; }
        public string WorkerContactNo { get; set; }
        public DateTime? JoiningDate { get; set; }
        public DateTime? RegisterDate { get; set; }
      

        public string RegistrationDate { get; set; }

      
        public decimal Percentage { get; set; }



        public string ExpenseTypeName { get; set; }
      
        public DateTime? ExpenseDate { get; set; }  // Stored as string because formatted date (dd/MM/yyyy)

        public string MonthName { get; set; }      //COMMONNNNNNNN




        /////////////////////////////////////////////////////////vraj ////////////////////////////////////////////////////////////////
        ///
        // For the grouped bar chart (Event Budget)
        public string MonthYear { get; set; }
       

        public int Month { get; set; }
        public int Year { get; set; }

        // For event budget detail popup
        public DateTime FromDate { get; set; }
        public decimal BudgetDifference => (AllocatedBudget ?? 0) - (ActualCost ?? 0);
        public int MonthNumber { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }

        // 🔽 NEW: For clicked month income/expense breakdown
        public string TypeLabel { get; set; }          // "Income" / "Expense"




        #endregion


        #region********************************************************************* Society Ac.Details Cash ***********************************************************

        public int TransactionId { get; set; }
       
        public string PaidToName { get; set; }
        public string PaymentModeName { get; set; }
     
        public string TransactionNature { get; set; }
       
        ///

        public string AccountType { get; set; }
        public int TransactionTypeId { get; set; }
      
        public string ChecqueNo { get; set; }
      

        public string AttachmentPath { get; set; }
        public string Type { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverCode { get; set; }
       
        public string MaintenanceCode { get; set; }
        public string OtherReceiver { get; set; }
       
        public string PDFPath { get; set; }
        public string Attchment { get; set; }
        public string SalarySlip { get; set; }

        public List<AccountManager> lstTransactionType { get; set; }


        public class ReceiptViewModel
        {
            public string TransactionCode { get; set; }
            public string PaidDate { get; set; }
            public string PaymentBy { get; set; }
            public string PaidTo { get; set; }
            public decimal Amount { get; set; }
            public string AmountInWords { get; set; }
            public string PaymentPurpose { get; set; }
            public string PaymentMode { get; set; }
            public List<ReceiptItem> Items { get; set; }
        }



        public class ReceiptItem
        {
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public bool IsPenalty { get; set; }
        }



        #endregion




        #region********************************************************************* Complaints ***********************************************************



        // ✅ Complaint List Properties

        public string ComplaintType { get; set; }
        public string Complaint { get; set; }
        public DateTime ComplaintDate { get; set; }
        public string AssignBy { get; set; }
       



        public int ComplaintId { get; set; }
        public string SecretoryName { get; set; }



        #endregion

    }
}
