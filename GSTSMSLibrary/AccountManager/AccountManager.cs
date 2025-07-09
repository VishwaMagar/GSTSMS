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

    }
}
