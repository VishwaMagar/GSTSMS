using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTSMSLibrary.SuperAdmin
{
    public class SuperAdmin
    {
        // Plan details
        public int PlanID { get; set; }
        public string PlanCode { get; set; }
        public string Subscription_Name { get; set; }
        public string Duration { get; set; }
        public decimal Amount { get; set; }
        public int NoOfMember { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDelete { get; set; }

        // Feature details (optional mapping)
        public int FeatureID { get; set; }
        public string FeatureName { get; set; }
        public bool IsActive { get; set; }

        // ✅ Add these fields to fix the error
        public DateTime StartDate { get; set; }
        public DateTime ExpieryDate { get; set; }
        // For UI or extra processing
        public List<string> FeatureList { get; set; }

        public SuperAdmin()
        {
            FeatureList = new List<string>();
        }


    }
    public class PublicTestimonial
    {
        public int TestimonialId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Society { get; set; }
        public string Message { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}
