using GSTSMSHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace GSTSMSLibrary.SuperAdmin
{
    public class BALSuperAdmin
    {
        MSSQL obj = new MSSQL();
        /// <summary>
        /// Retrieves all active subscription plans from the database.
        /// </summary>
        /// <returns>DataTable containing all subscription plans.</returns>
        public async Task<DataTable> GetAllPlansAsync()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "@Flag", "GetAllPlansRK" }
            };

            using (var dr = await obj.ExecuteStoreProcedureReturnDataReader("SuperAdmin", parameters))
            {
                DataTable dt = new DataTable();
                dt.Load(dr);
                return dt;
            }
        }

        /// <summary>
        /// Retrieves all features associated with a specific subscription plan.
        /// </summary>
        /// <param name="planId">The ID of the subscription plan.</param>
        /// <returns>DataTable containing features of the specified plan.</returns>
        public async Task<DataTable> GetFeaturesByPlanIdAsync(int planId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "@Flag", "GetFeatureByPlanIdRK" },
                { "@PlanID", planId.ToString() }
            };

            using (var dr = await obj.ExecuteStoreProcedureReturnDataReader("SuperAdmin", parameters))
            {
                DataTable dt = new DataTable();
                dt.Load(dr);
                return dt;
            }
        }
        /// <summary>
        /// Checks if the user already has an active subscription based on email.
        /// </summary>
        /// <param name="chairmanEmail">Chairman's email address.</param>
        /// <returns>SuperAdmin object with subscription info if active subscription exists; otherwise null.</returns>
        public async Task<SuperAdmin> CheckActiveSubscriptionAsync(string chairmanEmail)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
    {
        { "@Flag", "CheckActiveSubscriptionRK" },
        { "@ChairmanEmail", chairmanEmail }
    };

            using (SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("SuperAdmin", parameters))
            {
                if (await dr.ReadAsync())
                {
                    return new SuperAdmin
                    {
                        Subscription_Name = dr["PlanName"].ToString(),
                        StartDate = Convert.ToDateTime(dr["StartDate"]),
                        ExpieryDate = Convert.ToDateTime(dr["ExpieryDate"])
                    };
                }
            }

            return null;
        }
        /// <summary>
        /// Checks if the user has already submitted an enquiry using their email.
        /// </summary>
        /// <param name="email">Chairman's email address.</param>
        /// <returns>True if enquiry exists; otherwise false.</returns>
        public async Task<bool> IsEnquirySubmittedAsync(string email)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
    {
        { "@Flag", "CheckEnquiryBeforeOTPRK" },
        { "@ChairmanEmail", email }
    };

            using (var dr = await obj.ExecuteStoreProcedureReturnDataReader("SuperAdmin", parameters))
            {
                return dr.HasRows;
            }
        }
        /// <summary>
        /// Inserts a new subscription entry for a chairman after successful payment.
        /// </summary>
        /// <param name="planName">Name of the selected plan.</param>
        /// <param name="startDate">Start date of the subscription.</param>
        /// <param name="expiryDate">Expiry date of the subscription.</param>
        /// <param name="chairmanEmail">Chairman's email address.</param>
        /// <returns>Returns 1 if insertion successful; otherwise 0.</returns>
        public async Task<int> InsertSubscriptionAsync(string planName, DateTime startDate, DateTime expiryDate, string chairmanEmail)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
    {
        { "@Flag", "InsertSubscriptionRK" },
        { "@Subscription_Name", planName },
        { "@StartDate", startDate.ToString("yyyy-MM-dd") },
        { "@ExpieryDate", expiryDate.ToString("yyyy-MM-dd") },
        { "@ChairmanEmail", chairmanEmail }
    };

            return await obj.ExecuteStoreProcedureReturnInt("SuperAdmin", parameters);
        }
        /// <summary>
        /// Retrieves all public testimonials for display on the landing page.
        /// </summary>
        /// <returns>List of PublicTestimonial objects.</returns>
        public async Task<List<PublicTestimonial>> GetPublicTestimonialsAsync()
        {
            List<PublicTestimonial> testimonials = new List<PublicTestimonial>();

            Dictionary<string, string> parameters = new Dictionary<string, string>
    {
        { "@Flag", "GetPublicTestimonialsRK" }
    };

            using (SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("SuperAdmin", parameters))
            {
                while (await dr.ReadAsync())
                {
                    testimonials.Add(new PublicTestimonial
                    {
                        TestimonialId = Convert.ToInt32(dr["TestimonialId"]),
                        Name = dr["Name"].ToString(),
                        Role = dr["Role"].ToString(),
                        Society = dr["Society"].ToString(),
                        Message = dr["Message"].ToString(),
                        Rating = Convert.ToInt32(dr["Rating"]),
                        CreatedDate = Convert.ToDateTime(dr["CreatedDate"])
                    });
                }
            }

            return testimonials;
        }


    }

}
