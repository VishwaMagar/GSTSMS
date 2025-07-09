using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTSMSLibrary.Account
{
    public class Account
    {
        // 🔐 LOGIN FIELDS
        //[Required]
        //[EmailAddress]
        //public string Email { get; set; }

        //[Required(ErrorMessage = "Password is required")]
        //[DataType(DataType.Password)]
        //public string Password { get; set; }

        //[Display(Name = "OTP")]
        //public string Otp { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "New Password")]
        //public string NewPassword { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm New Password")]
        //[Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        //public string ConfirmPassword { get; set; }

        //public int RoleId { get; set; }
        //public string RoleName { get; set; }
        //public string StaffName { get; set; }
        //public int StaffId { get; set; }
        //public string StaffCode { get; set; }

   
        
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public int RoleId { get; set; }
            public string RoleName { get; set; }
            public string StaffName { get; set; }
            public int StaffId { get; set; }
            public string StaffCode { get; set; }
            public string ProfilePic { get; set; }
        }
    }


    // 📝 ENQUIRY FIELDS (for Chairman form)
    //public int EnquiryID { get; set; }

    //public string EnquiryCode { get; set; }

    //[Required(ErrorMessage = "Chairman name is required")]
    //public string ChairmanName { get; set; }

    //[Required(ErrorMessage = "Society name is required")]
    //public string SocietyName { get; set; }

    //[Required(ErrorMessage = "Number of members is required")]
    //[Range(1, 1000, ErrorMessage = "Enter a valid number")]
    //public int NoOfMember { get; set; }

    //[Required]
    //public string Address { get; set; }

    //[Required]
    //[Phone]
    //public string ContactNo { get; set; }

    //[EmailAddress]
    //public string ChairmanEmail { get; set; }

    //public int StatusId { get; set; } = 18; // Default status for enquiry
    //public DateTime EnquiryDate { get; set; } = DateTime.Now;
