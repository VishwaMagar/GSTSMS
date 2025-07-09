using GSTSMSHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;

namespace GSTSMSLibrary.Secretary
{
    public class BALSecretary
    {
        MSSQL obj = new MSSQL();

        #region*********************************************************************Priti Chavhan***********************************************************


        /// <summary>
        /// Updates the profile details  including profile picture 
        /// </summary>
        /// <param name="objU">Secretary object containing profile details</param>
        public async Task<int> UpdateProfile(Secretary objU)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "UpdateProfilePC");
            para.Add("@StaffCode", objU.StaffCode);
            para.Add("@StaffName", objU.StaffName);
            para.Add("@Email", objU.Email);
            if (!string.IsNullOrEmpty(objU.NewPassword))
            {
                para.Add("@OldPassword", objU.Password);
                para.Add("@NewPassword", objU.NewPassword);
            }
            else
            {

                para.Add("@OldPassword", DBNull.Value.ToString());
                para.Add("@NewPassword", DBNull.Value.ToString());
            }

            if (!string.IsNullOrEmpty(objU.ProfilePic))
                para.Add("@Document", objU.ProfilePic);
            else
                para.Add("@Document", DBNull.Value.ToString());

            SqlDataReader dr = null;
            int updateResult = 0;

            try
            {
                dr = await obj.ExecuteStoreProcedureReturnDataReader("Secretary", para);
                if (dr.HasRows)
                {
                    if (await dr.ReadAsync())
                    {
                        if (dr["UpdateResult"] != DBNull.Value)
                        {
                            updateResult = Convert.ToInt32(dr["UpdateResult"]);
                        }
                    }
                }
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }
            return updateResult;
        }

        /// <summary>
        /// Removes the profile picture for a given staff code
        /// </summary>
        /// <param name="staffCode">The staff code for which to remove the profile picture</param>
        public async Task<int> RemoveProfilePicture(string staffCode)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "RemoveProfilePicPC");
            para.Add("@StaffCode", staffCode);

            SqlDataReader dr = null;
            int removeResult = 0;
            try
            {
                dr = await obj.ExecuteStoreProcedureReturnDataReader("Secretary", para);
                if (dr.HasRows && await dr.ReadAsync())
                {
                    if (dr["UpdateResult"] != DBNull.Value)
                    {
                        removeResult = Convert.ToInt32(dr["UpdateResult"]);
                    }
                }
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }
            return removeResult;
        }



        /// <summary>
        /// Retrieves the profile picture filename by the staffcode
        /// </summary>
        /// <param name="staffCode">The unique code of the staff</param>
        /// <returns>Profile picture path or null</returns>
        public async Task<string> GetProfilePicByCode(string staffCode)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "GetProfilePicPC");
            para.Add("@StaffCode", staffCode);

            SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("Secretary", para);
            if (await dr.ReadAsync())
            {
                return dr["Document"]?.ToString();
            }
            return null;
        }

        /// <summary>
        /// Retrieves secretary details by staff code
        /// </summary>
        /// <param name="staffCode">The unique staff code</param>
        /// <returns>DataReader containing secretary details</returns>
        public async Task<SqlDataReader> GetStaffInfo(string staffCode)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "GetStaffInfoPC");
            para.Add("@StaffCode", staffCode);
            return await obj.ExecuteStoreProcedureReturnDataReader("Secretary", para);
        }



        /// <summary>
        /// Retrieves the list of policies 
        /// </summary>
        /// <returns>DataSet containing policy list</returns>
        public async Task<DataSet> PolicyList(int loggedInRoleId)
        {
            Dictionary<string, String> Para = new Dictionary<string, string>();
            Para.Add("@Flag", "PolicyListPC");
            Para.Add("@LoggedInRoleId", loggedInRoleId.ToString());
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", Para);
            return ds;
        }

        // New method to get terms and conditions for a specific policy
        // Inside your BAL file
        public async Task<DataSet> GetPolicyTerms(int policyId)
        {
            Dictionary<string, String> Para = new Dictionary<string, string>();
            Para.Add("@Flag", "PolicyTermsListPC");
            Para.Add("@PolicyId", policyId.ToString());
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", Para);
            return ds;
        }
        /// <summary>
        /// Retrieves terms and conditions based on a given policy ID
        /// </summary>
        /// <param name="policyId">ID of the policy</param>
        /// <returns>DataSet containing terms and conditions</returns>
        public async Task<DataSet> GetTermsAndConditionsByPolicyId(int policyId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
     {
         { "@Flag", "TermsAndConditionsByPolicyPC" },
         { "@PolicyId", policyId.ToString() }
     };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", parameters);
            return ds;
        }
        #endregion


        #region*********************************************************************Payal Gogawale***********************************************************


        /// <summary>
        /// Retrieve Vendor list with date range
        /// </summary>
        /// <param name="StaffCode">Staff code</param>
        /// <param name="startDate">Start date for filtering</param>
        /// <param name="endDate">End date for filtering</param>
        /// <returns>DataSet containing vendor list</returns>
        public async Task<DataSet> listVendorPG(string StaffCode, DateTime startDate, DateTime endDate)
        {
            Dictionary<string, string> obj1 = new Dictionary<string, string>
            {
                { "@Flag", "listVendorPG" },
                { "@StartDate", startDate.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate.ToString("yyyy-MM-dd") },
                { "@StaffCode", StaffCode }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", obj1);
            return ds;
        }

        /// <summary>
        /// Retrieve Vendor list (overload without date parameters for backward compatibility)
        /// </summary>
        /// <param name="StaffCode">Staff code</param>
        /// <returns>DataSet containing vendor list</returns>
        public async Task<DataSet> listVendorPG(string StaffCode)
        {
            Dictionary<string, string> obj1 = new Dictionary<string, string>
            {
                { "@Flag", "listVendorPG" },
                { "@StaffCode", StaffCode }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", obj1);
            return ds;
        }

        /// <summary>
        /// Retrieve vendor details
        /// </summary>
        /// <param name="objS">Secretary object containing VendorCode</param>
        /// <returns>SqlDataReader with vendor details</returns>
        public async Task<SqlDataReader> ViewVendorPG(Secretary objS)
        {
            var parameters = new Dictionary<string, string>
            {
                { "@Flag", "ViewVendorPG" },
                { "@VendorCode", objS.VendorCode?.ToString() ?? "0" }
            };

            // Executes the stored procedure to view vendor details
            return await obj.ExecuteStoreProcedureReturnDataReader("Secretary", parameters);
        }
        #endregion

        #region*********************************************************************Sanika Jaykar***********************************************************

        /// <summary>
        /// Retrieves calendar data for a specific event and meeting in the Secretary module
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public async Task<DataSet> CalenderSJ(Secretary objs)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "CalenderSJ");
            para.Add("@EventCode", objs.EventCode ?? "0");
            para.Add("@MeetingCode", objs.MeetingCode ?? "0");
            para.Add("@RoleId", objs.RoleId.ToString());


            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }

        /// <summary>
        /// Retrieves a list of notifications based on the user's role in the Secretary module
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public async Task<DataSet> NotificationSJ(Secretary objs)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "NotificationSJ");
            para.Add("@RoleId", objs.RoleId.ToString());

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }

        /// <summary>
        /// Retrieves the details of a specific notification based on the notification ID
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public async Task<DataSet> GetNotificationDetailsSJ(int notificationId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "NotificationDetailsSJ");
            para.Add("@NotificationId", notificationId.ToString());

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }

        /// <summary>
        /// Marks all notifications as read for the current user in the Secretary module
        /// </summary>
        /// <returns></returns>
        public async Task<bool> MarkAllNotificationsAsReadSJ()
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "MarkAllAsReadSJ");

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return true;
        }


        /// <summary>
        /// Marks a specific notification as read based on the notification ID
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public async Task<bool> MarkNotificationAsReadSJ(int notificationId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "MarkAsReadSJ");
            para.Add("@NotificationId", notificationId.ToString());

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return true;
        }
        #endregion

        #region*********************************************************************Suyog Kulkarni***********************************************************



        /// <summary>
        /// Retrieves all facilities for the secretary module using the "ShowFacilitySK" flag.
        /// </summary>
        /// <returns>A DataSet containing the list of facilities.</returns>

        public async Task<DataSet> ShowFacilitySK()
        {

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Clear();
            param.Add("@Flag", "ShowFacilitySK");
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", param);
            return ds;
        }


        /// <summary>
        /// Saves a new facility to the database using the "SaveFacilitySK" flag.
        /// </summary>
        /// <param name="objU">An object containing the facility name and description.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>


        public async Task SaveFacilitySK(Secretary objU)
        {
            Dictionary<string, String> para = new Dictionary<string, String>();
            para.Add("@Flag", "SaveFacilitySK");
            para.Add("@FacilityName", objU.FacilityName);
            para.Add("@Description", objU.Description);
            MSSQL db = new MSSQL();

            await db.ExecuteQuery("Secretary", para);

        }

        /// <summary>
        /// Fetches a facility's data by its ID using the "FetchFacilitySK" flag.
        /// </summary>
        /// <param name="id">The unique ID of the facility.</param>
        /// <returns>A SqlDataReader with the facility details.</returns>


        public async Task<SqlDataReader> FetchFacilityByIdSK(int id)
        {
            Dictionary<string, String> para = new Dictionary<string, String>();

            para.Add("@Flag", "FetchFacilitySK");
            para.Add("@FacilityId", Convert.ToString(id));
            MSSQL db = new MSSQL();
            SqlDataReader dr = await db.ExecuteStoreProcedureReturnDataReader("Secretary", para);
            return dr;


        }

        /// <summary>
        /// Updates an existing facility's details using the "EditFacilitySK" flag.
        /// </summary>
        /// <param name="objU">An object containing the facility ID, name, and description to update.</param>
        /// <returns>A SqlDataReader with the result of the update operation.</returns>


        public async Task<SqlDataReader> EditFacilitySK(Secretary objU)
        {
            Dictionary<string, String> para = new Dictionary<string, String>();
            para.Add("@Flag", "EditFacilitySK");
            para.Add("@FacilityId", Convert.ToString(objU.FacilityId));
            para.Add("@FacilityName", objU.FacilityName);
            para.Add("@Description", objU.Description);
            MSSQL db = new MSSQL();
            SqlDataReader dr = await db.ExecuteStoreProcedureReturnDataReader("Secretary", para);
            return dr;


        }

        /// <summary>
        /// Retrieves detailed view information of a facility using the "ViewFacilitySK" flag.
        /// </summary>
        /// <param name="objU">An object containing the facility ID.</param>
        /// <returns>A DataSet with the facility view details.</returns>



        public async Task<DataSet> ViewFacilitySK(Secretary objU)
        {
            Dictionary<string, String> param = new Dictionary<string, String>();
            param.Add("@Flag", "ViewFacilitySK");
            param.Add("@FacilityId", Convert.ToString(objU.FacilityId));
            MSSQL db = new MSSQL();
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", param);
            return ds;

        }



        /// <summary>
        /// Deletes a facility by its ID using the "DeleteFacilitySK" flag.
        /// </summary>
        /// <param name="id">The unique ID of the facility to delete.</param>
        /// <returns>A SqlDataReader with the result of the delete operation.</returns>

        public async Task<SqlDataReader> DeleteFacilitySK(int id)
        {
            Dictionary<string, String> para = new Dictionary<string, String>();
            Secretary obju = new Secretary();
            para.Add("@Flag", "DeleteFacilitySK");
            para.Add("@FacilityId", Convert.ToString(id));
            MSSQL db = new MSSQL();
            SqlDataReader dr = await db.ExecuteStoreProcedureReturnDataReader("Secretary", para);
            return dr;
        }

        #endregion

        #region*********************************************************************Sandhya Hivare Email***********************************************************


        /// <summary>
        /// GetWing And Flat
        /// </summary>
        /// <param name="staffCode"></param>
        /// <returns></returns>
        public async Task<DataSet> GetWingByStaffCode(string staffCode)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "GetWingByStaffCodeSH");
            para.Add("@StaffCode", staffCode);

            return await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
        }


        /// <summary>
        /// GetMemebrEmail And Name
        /// </summary>
        /// <param name="wingid"></param>
        /// <returns></returns>
        public async Task<DataSet> GetMemberEmailsByWingName(int wingid)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "GetMemberEmailsByStaffWingSH");
            para.Add("@WingId", wingid.ToString());

            return await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
        }

        /// <summary>
        /// Sends an email to a specified user.
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public string SendEmailToUser(string toEmail, string subject, string body)
        {
            string fromEmail = "yourgmail@gmail.com";
            string appPassword = "lboa imah aovd qsup";

            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,

                    Body = body,
                    IsBodyHtml = false
                };
                mail.To.Add(toEmail);

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(fromEmail, appPassword);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }

                return "Email sent successfully.";
            }
            catch (Exception ex)
            {
                return $"Error while sending email: {ex.Message}";
            }
        }

        #endregion

        #region*********************************************************************Sanika Patil Secratery Report***********************************************************

        /// <summary>
        /// this method create for the get complaint data
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="filterType"></param>
        /// <returns> </returns>
        public async Task<DataSet> ComplaintDataSP(DateTime fromDate, DateTime toDate, string filterType, string StaffCode)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@Flag", "ComplaintReportSP");
            parameters.Add("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            parameters.Add("@ToDate", toDate.ToString("yyyy-MM-dd"));
            parameters.Add("@FilterType", filterType);
            parameters.Add("@StaffCode", StaffCode);
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", parameters);
            return ds;
        }
        /// <summary>
        /// this method create for the get Visitor data
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="filterType"></param>
        /// <returns> </returns>
        public async Task<DataSet> VisitorDataSP(DateTime fromDate, DateTime toDate, string StaffCode)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@Flag", "VisitorReportSP");
            parameters.Add("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            parameters.Add("@ToDate", toDate.ToString("yyyy-MM-dd"));
            parameters.Add("@StaffCode", StaffCode);

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", parameters);
            return ds;
        }
        /// <summary>
        /// this method create for the get Maintenance data
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="filterType"></param>
        /// <returns> </returns>
        public async Task<DataSet> MaintenanceDataSP(DateTime fromDate, DateTime toDate, string StaffCode)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@Flag", "MaintenanceReportSP");
            parameters.Add("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            parameters.Add("@ToDate", toDate.ToString("yyyy-MM-dd"));
            parameters.Add("@StaffCode", StaffCode);
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", parameters);
            return ds;
        }
        /// <summary>
        /// this method create for the get Parking data
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="filterType"></param>
        /// <returns> </returns>
        public async Task<DataSet> ParkingDataSP(string fromDate, string toDate, string StaffCode)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@Flag", "ParkingReportSP");
            parameters.Add("@FromDate", fromDate);
            parameters.Add("@ToDate", toDate);

            parameters.Add("@StaffCode", StaffCode);
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", parameters);
            return ds;
        }

        /// <summary>
        /// It is used to get particuler complaint data by clicking on specific bar 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="datelabel"></param>
        /// <param name="status"></param>
        /// <param name="StaffCode"></param>
        /// <returns></returns>
        public async Task<List<Secretary>> ComplaintListSP(string filter, string datelabel, string status, string StaffCode)
        {
            var para = new Dictionary<string, string>
        {
         { "@Flag", "ShowComplaintListSP" },
         { "@FilterType", filter },
         { "@DateLabel", datelabel },
         { "@StaffCode", StaffCode },
         { "@StatusName", status }
        };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (status == "Pending")
                    {
                        Secretary obj = new Secretary
                        {

                            SrNo = Convert.ToInt32(dr["SrNo"].ToString()),
                            MemberCode = dr["RaisedBy"].ToString(),

                            SubType = dr["SubType"].ToString(),
                            Description = dr["Description"].ToString(),
                            ComplaintDate = Convert.ToDateTime(dr["Date"].ToString())
                        };
                        list.Add(obj);
                    }
                    else
                    {
                        Secretary obj = new Secretary
                        {

                            SrNo = Convert.ToInt32(dr["SrNo"].ToString()),
                            MemberCode = dr["RaisedBy"].ToString(),
                            WorkerName = dr["AssignedPerson"].ToString(),
                            SubType = dr["SubType"].ToString(),
                            Description = dr["Description"].ToString(),
                            ComplaintDate = Convert.ToDateTime(dr["Date"].ToString())
                        };
                        list.Add(obj);
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// It is used to get particular data of visitor
        /// </summary>
        /// <param name="VisitorType"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="StaffCode"></param>
        /// <returns></returns>
        public async Task<List<Secretary>> VisitorListDataSP(string VisitorType, string fromDate, string toDate, string StaffCode)
        {
            var para = new Dictionary<string, string>
       {
        { "@Flag", "ShowVisitorListSP" },
         { "@VisitorType", VisitorType },
        { "@FromDate", fromDate},
          { "@StaffCode",StaffCode},
          { "@ToDate", toDate}
        };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        // v.VisitorCode, v.VisitorName, v.FlatCode, v.VisitorContact ,v.VehicleNumber, v.Reason,  MS.FullName
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        VisitorCode = dr["VisitorCode"]?.ToString(),
                        VisitorName = dr["VisitorName"]?.ToString(),
                        FlatCode = dr["FlatNumber"]?.ToString(),
                        FullName = dr["OwnerName"]?.ToString(),
                        VisitorContact = dr["VisitorContact"]?.ToString(),
                        VehicleNumber = dr["VehicleNumber"]?.ToString(),
                        Reason = dr["Reason"]?.ToString(),
                        CheckIn = dr["CheckIn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CheckIn"]),
                        CheckOut = dr["CheckOut"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CheckOut"]),



                    };
                    list.Add(obj);
                }
            }
            return list;
        }
        /// <summary>
        /// Purpose of this method is to get expense data of particular data
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="filterType"></param>
        /// <param name="StaffCode"></param>
        /// <returns></returns>
        public async Task<DataSet> GETExpenseDataSP(DateTime fromDate, DateTime toDate, string filterType, string StaffCode)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@Flag", "ExpenseDataSP");
            parameters.Add("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            parameters.Add("@ToDate", toDate.ToString("yyyy-MM-dd"));
            parameters.Add("@FilterType", filterType);
            parameters.Add("@StaffCode", StaffCode);
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", parameters);
            return ds;
        }
        /// <summary>
        /// This method is used to get allocated parking and unallocated parking information
        /// </summary>
        /// <param name="status"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="parkingcode"></param>
        /// <param name="StaffCode"></param>
        /// <returns></returns>
        public async Task<DataSet> ParkingListSP(string status, DateTime fromDate, DateTime toDate, string parkingcode, string StaffCode)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();


            if (status == "Allocated")
                parameters.Add("@Flag", "OccupiedListSP");
            else
                parameters.Add("@Flag", "UnoccupiedListSP");

            parameters.Add("@FromDate", fromDate.ToString("yyyy-MM-dd"));
            parameters.Add("@ToDate", toDate.ToString("yyyy-MM-dd"));
            parameters.Add("@ParkingCode", parkingcode);
            parameters.Add("@StaffCode", StaffCode);

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", parameters);
            return ds;
        }
        /// <summary>
        /// It is used to get paid and unpaid maintenance member list
        /// </summary>
        /// <param name="datelabel"></param>
        /// <param name="statusname"></param>
        /// <param name="StaffCode"></param>
        /// <returns></returns>
        public async Task<DataSet> MaintenanceListSP(string datelabel, string statusname, string StaffCode)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            if (statusname == "Paid Maintenance")
                parameters.Add("@Flag", "PaidMaintenanceList");
            else
                parameters.Add("@Flag", "UnpaidMaintenance");

            parameters.Add("@DateLabel", datelabel);
            parameters.Add("@StaffCode", StaffCode);

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", parameters);
            return ds;
        }

        /// <summary>
        /// Purpose of this method is to get expense data of particular data
        /// </summary>
        /// <param name="dateLabel"></param>
        /// <param name="filterType"></param>
        /// <param name="StaffCode"></param>
        /// <returns></returns>
        public async Task<List<Secretary>> ExpenseDetailSP(string dateLabel, string filterType, string StaffCode, string statusName)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();

            // Conditionally add the correct flag
            if (statusName == "Paid Expense")
            {
                para.Add("@Flag", "PaidExpenseListSP");
            }
            else
            {
                para.Add("@Flag", "UnPaidExpenseListSP");
            }

            // Add other parameters
            para.Add("@FilterType", filterType);
            para.Add("@DateLabel", dateLabel);
            para.Add("@StaffCode", StaffCode);

            // Execute the stored procedure
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);

            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {


                    Secretary item = new Secretary();
                    if (statusName == "Paid Expense")
                    {
                        decimal amount = 0;
                        decimal.TryParse(dr["Amount"]?.ToString(), out amount); ;
                        item.SrNo = Convert.ToInt32(dr["SrNo"]);
                        item.ExpenseCode = dr["ExpenseCode"]?.ToString();
                        item.Amount = amount;

                        item.VendorName = dr["VendorName"]?.ToString();
                        item.Description = dr["Description"]?.ToString();
                        item.PaymentMode = dr["PaymentMode"]?.ToString();
                        item.TransactionId = dr["TransactionIdChequeId"]?.ToString();
                        item.PaidDate = Convert.ToDateTime(dr["PaidDate"]);
                    }
                    else if (statusName == "Unpaid Expense")
                    {
                        item.SrNo = Convert.ToInt32(dr["SrNo"]);
                        item.ExpenseCode = dr["ExpenseCode"]?.ToString();
                        item.PaidAmount = Convert.ToDecimal(dr["PaidExpense"]?.ToString());
                        item.TotalAmount = Convert.ToDecimal(dr["Expected"]?.ToString());
                        item.Amount = Convert.ToDecimal(dr["UnpaidExpense"]?.ToString());
                        item.VendorName = dr["VendorName"]?.ToString();
                        item.Description = dr["Description"]?.ToString();

                    }

                    list.Add(item);
                }
            }

            return list;
        }



        #endregion

        #region*********************************************************************Siddhesh Balkavade Dashboard***********************************************************

        public async Task<decimal> GetTotalMaintenanceAmountSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            if (startDate.HasValue)
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, 1); // Start of the month

            if (endDate.HasValue)
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, DateTime.DaysInMonth(endDate.Value.Year, endDate.Value.Month)); // End of the month


            var para = new Dictionary<string, string>
            {
                { "@Flag", "ShowTotalMaintenanceSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            object result = await obj.ExecuteStoreProcedureReturnObj("Secretary", para);
            return result == DBNull.Value ? 0m : Convert.ToDecimal(result);
        }


        public async Task<decimal> GetCollectedMaintenanceAmountSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {


            if (startDate.HasValue)
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, 1); // Start of the month

            if (endDate.HasValue)
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, DateTime.DaysInMonth(endDate.Value.Year, endDate.Value.Month)); // End of the month


            var para = new Dictionary<string, string>
            {
                { "@Flag", "ShowCollectedMaintenaceCountSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            object result = await obj.ExecuteStoreProcedureReturnObj("Secretary", para);
            return result == DBNull.Value ? 0m : Convert.ToDecimal(result);
        }


        public async Task<decimal> GetDueMaintenanceAmountSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            if (startDate.HasValue)
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, 1); // Start of the month

            if (endDate.HasValue)
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, DateTime.DaysInMonth(endDate.Value.Year, endDate.Value.Month)); // End of the month



            var para = new Dictionary<string, string>
            {
                { "@Flag", "ShowDueMaintenaceCountSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            object result = await obj.ExecuteStoreProcedureReturnObj("Secretary", para);
            return (result == null || result == DBNull.Value) ? 0m : Convert.ToDecimal(result);
        }


        // completed maintenance payment list
        public async Task<List<Secretary>> GetCollectedMaintenanceListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            if (startDate.HasValue)
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, 1); // Start of the month

            if (endDate.HasValue)
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, DateTime.DaysInMonth(endDate.Value.Year, endDate.Value.Month)); // End of the month





            var para = new Dictionary<string, string>
            {
                { "@Flag", "ShowCollectedMaintenacelistSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        FullName = dr["FullName"].ToString(),
                        FloorName = dr["FloorName"].ToString(),
                        FlatCode = dr["FlatNumber"].ToString(),
                        Amount = Convert.ToDecimal(dr["Amount"]),
                        PaidDate = Convert.ToDateTime(dr["PaidDate"])
                    };
                    list.Add(obj);
                }
            }
            return list;
        }


        // Due maintenance payment list
        public async Task<List<Secretary>> GetDueMaintenanceListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (startDate.HasValue)
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, 1); // Start of the month

            if (endDate.HasValue)
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, DateTime.DaysInMonth(endDate.Value.Year, endDate.Value.Month)); // End of the month


            var para = new Dictionary<string, string>
            {
                { "@Flag", "ShowDueMaintenacelistSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        FullName = dr["FullName"].ToString(),
                        FloorName = dr["FloorName"].ToString(),
                        FlatCode = dr["FlatNumber"].ToString(),
                        Amount = Convert.ToDecimal(dr["Amount"]),
                        DeadlineDate = Convert.ToDateTime(dr["DeadlineDate"])
                    };
                    list.Add(obj);
                }
            }
            return list;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //Expence 

        //Total Expence AmountSB

        public async Task<decimal> GetTotalExpenceAmountSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            if (startDate.HasValue)
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, 1); // Start of the month

            if (endDate.HasValue)
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, DateTime.DaysInMonth(endDate.Value.Year, endDate.Value.Month)); // End of the month


            var para = new Dictionary<string, string>
            {
                { "@Flag", "TotalExpenceAmountSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            object result = await obj.ExecuteStoreProcedureReturnObj("Secretary", para);
            return result == DBNull.Value ? 0m : Convert.ToDecimal(result);
        }


        // Completed Expence Amount
        public async Task<decimal> GetCompletedExpenceAmountSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            if (startDate.HasValue)
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, 1); // Start of the month

            if (endDate.HasValue)
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, DateTime.DaysInMonth(endDate.Value.Year, endDate.Value.Month)); // End of the month


            var para = new Dictionary<string, string>
            {
                { "@Flag", "PaidExpenceAmountSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            object result = await obj.ExecuteStoreProcedureReturnObj("Secretary", para);
            return result == DBNull.Value ? 0m : Convert.ToDecimal(result);
        }

        //Pending Expence Amount
        public async Task<decimal> GetPendingExpenceAmountSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (startDate.HasValue)
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, 1); // Start of the month

            if (endDate.HasValue)
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, DateTime.DaysInMonth(endDate.Value.Year, endDate.Value.Month)); // End of the month

            var para = new Dictionary<string, string>
            {
                { "@Flag", "UnpaidExpenceAmountSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            object result = await obj.ExecuteStoreProcedureReturnObj("Secretary", para);
            return (result == null || result == DBNull.Value) ? 0m : Convert.ToDecimal(result);
        }


        //Completed Expence List
        public async Task<List<Secretary>> GetCompletedExpenceListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            if (startDate.HasValue)
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, 1); // Start of the month

            if (endDate.HasValue)
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, DateTime.DaysInMonth(endDate.Value.Year, endDate.Value.Month)); // End of the month


            var para = new Dictionary<string, string>
            {
                { "@Flag", "PaidExpenceListSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        VendorName = dr["PaidToName"].ToString(),
                        Description = dr["Description"].ToString(),
                        PaymentMode = dr["PaymentMode"].ToString(),
                        Amount = Convert.ToDecimal(dr["PaidAmount"]),
                        PaidDate = Convert.ToDateTime(dr["PaidDate"])

                    };
                    list.Add(obj);
                }
            }
            return list;
        }

        //Pending Expence List

        public async Task<List<Secretary>> GetPendingExpenceListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            if (startDate.HasValue)
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, 1); // Start of the month

            if (endDate.HasValue)
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, DateTime.DaysInMonth(endDate.Value.Year, endDate.Value.Month)); // End of the month


            var para = new Dictionary<string, string>
            {
                { "@Flag", "UnpaidExpenceListSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        VendorName = dr["PaidToName"].ToString(),
                        Description = dr["Description"].ToString(),
                        TotalAmount = Convert.ToDecimal(dr["TotalAmount"]),
                        PaidAmount = Convert.ToDecimal(dr["PaidAmount"]),
                        Amount = Convert.ToDecimal(dr["BalanceAmount"])
                    };
                    list.Add(obj);
                }
            }
            return list;
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Get Collection AmountSB
        public async Task<decimal> GetCollectionAmountSBSB(string StaffCode)
        {
            var para = new Dictionary<string, string>
            {
                { "@Flag", "ShowCollectionAmountSB" },
                { "@StaffCode", StaffCode }
            };

            object result = await obj.ExecuteStoreProcedureReturnObj("Secretary", para);
            return Convert.ToDecimal(result);
        }


        //----------------------------------------------------------------------------------------------------------------------------------------------

        //Visitor Parking Pie Chart
        public async Task<Secretary> GetVisitorParkingCountSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            var para = new Dictionary<string, string>
            {
                { "@Flag", "VisitorParkingCountSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            Secretary parkingData = new Secretary();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                parkingData.WingName = dr["WingName"].ToString();
                parkingData.TotalVisitorSlots = Convert.ToInt32(dr["TotalVisitorSlots"]);
                parkingData.OccupiedSlots = Convert.ToInt32(dr["OccupiedSlots"]);
                parkingData.UnoccupiedSlots = Convert.ToInt32(dr["UnoccupiedSlots"]);
            }

            return parkingData;
        }


        public async Task<List<Secretary>> GetOccupiedVisitorListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            var para = new Dictionary<string, string>
            {
                { "@Flag", "OccupiedListSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        VisitorType = dr["VisitorType"].ToString(),
                        VisitorName = dr["VisitorName"].ToString(),
                        VehicleNumber = dr["VehicleNumber"].ToString(),
                        ParkingCode = dr["ParkingCode"].ToString(),
                        CheckIn = dr["CheckIn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CheckIn"]),

                    };
                    list.Add(obj);
                }
            }
            return list;
        }


        public async Task<List<Secretary>> GetUnoccupiedSlotListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {


            var para = new Dictionary<string, string>
            {
                { "@Flag", "UnoccupiedListSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        ParkingCode = dr["ParkingCode"].ToString(),
                        VehicleType = dr["VehicleType"].ToString()
                    };
                    list.Add(obj);
                }
            }
            return list;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------

        // Today's Visitors (Guest,Delivery,Vendor, Worker)

        //Fetch Count in Avatar card
        public async Task<int> GetGuestVisitorCountSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            var para = new Dictionary<string, string>
            {
                { "@Flag", "ShowGuestVisitorCountSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }

            };

            var ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                return Convert.ToInt32(dr["TotalGuestVisitorCount"]);
            }

            return 0;
        }

        public async Task<int> GetDeliveryVisitorCountSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            var para = new Dictionary<string, string>
            {
                { "@Flag", "ShowDeliveryVisitorCountSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            var ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                return Convert.ToInt32(dr["TotalDeliveryVisitorCount"]);
            }

            return 0;
        }



        public async Task<int> GetVendorVisitorCountSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            var para = new Dictionary<string, string>
            {
                { "@Flag", "ShowVendorVisitorCountSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            var ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                return Convert.ToInt32(dr["TotalVendorVisitorCount"]);
            }

            return 0;
        }

        public async Task<int> GetServiceVisitorCountSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            var para = new Dictionary<string, string>
            {
                { "@Flag", "ShowServiceVisitorCountSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            var ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                return Convert.ToInt32(dr["TotalServiceVisitorCount"]);
            }

            return 0;
        }


        public async Task<int> GetWorkerVisitorCountSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            var para = new Dictionary<string, string>
            {
                { "@Flag", "ShowWorkerVisitorCountSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            var ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                return Convert.ToInt32(dr["TotalWorkerVisitorCount"]);
            }

            return 0;
        }


        // list for guest
        public async Task<List<Secretary>> GetGuestListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            var para = new Dictionary<string, string>
            {
                { "@Flag", "ShowGuestVisitorListSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }

            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        VisitorName = dr["VisitorName"].ToString(),
                        VisitorContact = dr["VisitorContact"].ToString(),
                        VehicleNumber = dr["VehicleNumber"].ToString(),
                        Reason = dr["Reason"].ToString(),
                        FlatCode = dr["FlatNumber"].ToString(),
                        FullName = dr["OwnerName"].ToString(),
                        CheckIn = dr["CheckIn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CheckIn"]),
                        CheckOut = dr["CheckOut"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CheckOut"]),

                    };
                    list.Add(obj);
                }
            }
            return list;
        }

        // list for Delivery
        public async Task<List<Secretary>> GetDeliveryListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            var para = new Dictionary<string, string>
            {
                { "@Flag", "ShowDeliveryVisitorListSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        VisitorName = dr["VisitorName"].ToString(),
                        VisitorContact = dr["VisitorContact"].ToString(),
                        VehicleNumber = dr["VehicleNumber"].ToString(),
                        Reason = dr["Reason"].ToString(),
                        FlatCode = dr["FlatNumber"].ToString(),
                        FullName = dr["OwnerName"].ToString(),
                        CheckIn = dr["CheckIn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CheckIn"]),
                        CheckOut = dr["CheckOut"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CheckOut"]),
                    };
                    list.Add(obj);
                }
            }
            return list;
        }

        // list for Vendor
        public async Task<List<Secretary>> GetVendorListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            var para = new Dictionary<string, string>
            {
                { "@Flag", "ShowVendorVisitorListSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        VisitorName = dr["VisitorName"].ToString(),
                        VisitorContact = dr["VisitorContact"].ToString(),
                        VehicleNumber = dr["VehicleNumber"].ToString(),
                        Reason = dr["Reason"].ToString(),
                        FlatCode = dr["FlatNumber"].ToString(),
                        FullName = dr["OwnerName"].ToString(),
                        CheckIn = dr["CheckIn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CheckIn"]),
                        CheckOut = dr["CheckOut"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CheckOut"]),
                    };
                    list.Add(obj);
                }
            }
            return list;
        }


        // list for Service
        public async Task<List<Secretary>> GetServiceListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            var para = new Dictionary<string, string>
            {
                { "@Flag", "ShowServiceVisitorListSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        VisitorName = dr["VisitorName"].ToString(),
                        VisitorContact = dr["VisitorContact"].ToString(),
                        VehicleNumber = dr["VehicleNumber"].ToString(),
                        Reason = dr["Reason"].ToString(),
                        FlatCode = dr["FlatNumber"].ToString(),
                        FullName = dr["OwnerName"].ToString(),
                        CheckIn = dr["CheckIn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CheckIn"]),
                        CheckOut = dr["CheckOut"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CheckOut"]),
                    };
                    list.Add(obj);
                }
            }
            return list;
        }

        // list for Worker
        public async Task<List<Secretary>> GetWorkerListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            var para = new Dictionary<string, string>
            {
                { "@Flag", "ShowWorkerVisitorListSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        VisitorName = dr["VisitorName"].ToString(),
                        VisitorContact = dr["VisitorContact"].ToString(),
                        VehicleNumber = dr["VehicleNumber"].ToString(),
                        Reason = dr["Reason"].ToString(),
                        CheckIn = dr["CheckIn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CheckIn"]),
                        CheckOut = dr["CheckOut"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CheckOut"]),
                    };
                    list.Add(obj);
                }
            }
            return list;
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------

        //Worker Attendance 


        public async Task<DataSet> WorkerAttendaneDataSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            var para = new Dictionary<string, string>
            {
                {"@Flag", "WorkerAttendaceDataSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }

            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }


        // list for Present 
        public async Task<List<Secretary>> GetPresentListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            var para = new Dictionary<string, string>
            {
                { "@Flag", "PresentWorkerListSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        WorkerName = dr["WorkerName"].ToString(),
                        CheckIn = dr["CheckIn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CheckIn"]),
                        CheckOut = dr["CheckOut"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["CheckOut"]),

                    };
                    list.Add(obj);
                }
            }
            return list;
        }

        // list for Absent 
        public async Task<List<Secretary>> GetAbsentListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            var para = new Dictionary<string, string>
            {
                { "@Flag", "AbsenttWorkerListSB" },
                 { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        WorkerName = dr["WorkerName"].ToString()

                    };
                    list.Add(obj);
                }
            }
            return list;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------

        //Residant Complaints

        public async Task<DataSet> ResidantComplaintsDataSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            var para = new Dictionary<string, string>
            {
                {"@Flag", "ResidantComplaintsSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }

            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }


        // list for InProgress
        public async Task<List<Secretary>> GetInProgressListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            var para = new Dictionary<string, string>
            {
                { "@Flag", "InprogressComplaintsSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        FullName = dr["FullName"].ToString(),
                        Description = dr["Description"].ToString(),
                        Date = Convert.ToDateTime(dr["Date"]),
                        FlatCode = dr["FlatNumber"].ToString()
                    };
                    list.Add(obj);
                }
            }
            return list;
        }


        // list for Pending
        public async Task<List<Secretary>> GetPendingListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            var para = new Dictionary<string, string>
            {
                { "@Flag", "PedingComplaintsSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        FullName = dr["FullName"].ToString(),
                        Description = dr["Description"].ToString(),
                        Date = Convert.ToDateTime(dr["Date"]),
                        FlatCode = dr["FlatNumber"].ToString()
                    };
                    list.Add(obj);
                }
            }
            return list;
        }



        // list for Escalate
        public async Task<List<Secretary>> GetEscalateListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            var para = new Dictionary<string, string>
            {
                { "@Flag", "EscalateComplaintsSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        FullName = dr["FullName"].ToString(),
                        Description = dr["Description"].ToString(),
                        Date = Convert.ToDateTime(dr["Date"]),
                        FlatCode = dr["FlatNumber"].ToString()
                    };
                    list.Add(obj);
                }
            }
            return list;
        }


        // list for Solved
        public async Task<List<Secretary>> GetSolvedListSB(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            var para = new Dictionary<string, string>
            {
                { "@Flag", "SolvedComplaintsSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }
            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SrNo = Convert.ToInt32(dr["SrNo"]),
                        FullName = dr["FullName"].ToString(),
                        Description = dr["Description"].ToString(),
                        Date = Convert.ToDateTime(dr["Date"]),
                        FlatCode = dr["FlatNumber"].ToString()
                    };
                    list.Add(obj);
                }
            }
            return list;
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------

        // Upcoming Meetings 

        public async Task<List<Secretary>> UpcomingMeetingSB(string StaffCode, DateTime? UPMtoday = null)
        {
            var para = new Dictionary<string, string>
            {
                { "@Flag", "UpcomingMeetingSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", UPMtoday?.ToString("yyyy-MM-dd") },

            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        MeetingTitle = dr["MeetingTitle"].ToString(),
                        StaffName = dr["CreatedBy"].ToString(),
                        MeetingAgenda = dr["MeetingAgenda"].ToString(),
                        MeetingDate = Convert.ToDateTime(dr["MeetingDate"]),
                        MeetingLocation = dr["Location"].ToString(),
                        CreatedDate = Convert.ToDateTime(dr["CreatedDate"]),
                    };
                    list.Add(obj);
                }
            }
            return list;
        }


        // Upcoming Event
        public async Task<List<Secretary>> UpcomingEventSB(string StaffCode, DateTime? UPMtoday = null)
        {
            var para = new Dictionary<string, string>
            {
                { "@Flag", "UpcomingEventSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", UPMtoday?.ToString("yyyy-MM-dd") },

            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> list = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Secretary obj = new Secretary
                    {
                        SubType = dr["EventType"].ToString(),
                        EventName = dr["EventName"].ToString(),
                        VendorName = dr["OrganizedBy"].ToString(),
                        Description = dr["Description"].ToString(),
                        Place = dr["Place"].ToString(),
                        FromDate = Convert.ToDateTime(dr["FromDate"]),
                        ToDate = Convert.ToDateTime(dr["ToDate"]),

                    };
                    list.Add(obj);
                }
            }
            return list;
        }


        // TOP 5 Expence graph
        public async Task<List<Secretary>> TOP_5_ExpenceSBgraph(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {

            if (startDate.HasValue)
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, 1); // Start of the month

            if (endDate.HasValue)
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, DateTime.DaysInMonth(endDate.Value.Year, endDate.Value.Month)); // End of the month


            var para = new Dictionary<string, string>
            {
                { "@Flag", "TOP_5_ExpenceSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }

            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> ExpenceData = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {


                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    ExpenceData.Add(new Secretary
                    {
                        ExpenceFor = dr["ExpenceFor"].ToString(),
                        ExpenceType = dr["ExpenseType"].ToString(),
                        Description = dr["Description"].ToString(),
                        VendorName = dr["VendorName"].ToString(),
                        Amount = Convert.ToDecimal(dr["Amount"])
                    });


                }
            }
            return ExpenceData;
        }

        // TOP 5 UnpaidMaintenance

        public async Task<List<Secretary>> TOP_5_UnpaidMaintenanceCtlSBgraph(string StaffCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (startDate.HasValue)
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, 1); // Start of the month

            if (endDate.HasValue)
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, DateTime.DaysInMonth(endDate.Value.Year, endDate.Value.Month)); // End of the month


            var para = new Dictionary<string, string>
            {
                { "@Flag", "TOP_5_UnpaidMaintenanceSB" },
                { "@StaffCode", StaffCode },
                { "@StartDate", startDate?.ToString("yyyy-MM-dd") },
                { "@EndDate", endDate?.ToString("yyyy-MM-dd") }

            };

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            List<Secretary> UnpaidMaintenaceData = new List<Secretary>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    UnpaidMaintenaceData.Add(new Secretary
                    {
                        FullName = dr["FullName"].ToString(),
                        Flat = dr["Flat"].ToString(),
                        MaintenceName = dr["MaintenceName"].ToString(),
                        Description = dr["Description"].ToString(),
                        Amount = Convert.ToDecimal(dr["Amount"]),
                        DeadlineDate = Convert.ToDateTime(dr["DeadlineDate"])

                    });



                }
            }
            return UnpaidMaintenaceData;
        }




        #endregion


        #region*********************************************************************Ankita And Satwashil***********************************************************

        /// <summary>
        /// Gets feedback count for dashboard graph.
        /// </summary>
        public async Task<DataSet> GetCountCardInGraph(int assignWingId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>
        {
            { "@Flag", "GetCardCountinGraphAJ" },
            { "@AssignWingId", assignWingId.ToString() }
        };
            return await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
        }


        /// <summary>
        /// Retrieves a list of members who have given a specific star rating in the selected wing.
        /// This is used to load feedback data dynamically based on the star rating clicked on the chart.
        ///
        /// Parameters:
        /// assignWingId - The ID of the wing to filter members.
        /// ratings - The star rating (1 to 5) selected by the user.
        /// 
        /// Returns:
        /// A DataSet containing the feedback details for members matching the selected rating and wing.
        /// </summary>

        public async Task<DataSet> StartList(int assignWingId, int ratings)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "StarListAJ");
            para.Add("@AssignWingId", assignWingId.ToString());
            para.Add("@Ratings", ratings.ToString());

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }

        /// <summary>
        /// Gets list of events for a given wing.
        /// </summary>
        public async Task<DataSet> ViewEventSD(int assignWingId)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
{
    { "@Flag", "ViewEventSD" },
    { "@AssignWingId", assignWingId.ToString() }
};
            MSSQL db = new MSSQL();
            return await db.ExecuteStoreProcedureReturnDS("Secretary", data);
        }
        #endregion

        #region*********************************************************************Sandhya Hivare Notice Management ***********************************************************

        /// <summary>
        /// Gets staff details using their email address.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<DataSet> GetStaffDetailsByEmail(string email)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "ShowLoginCodeSH");
            para.Add("@Email", email);
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }

        /// <summary>
        /// Retrieves notice data between specified start and end dates and show Notice Data.
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public async Task<DataSet> ShowData(DateTime? StartDate, DateTime? EndDate)
        {
            Dictionary<string, String> Para = new Dictionary<string, string>();
            Para.Add("@Flag", "ShowNoticeDataSH");
            Para.Add("@FromDate", StartDate?.ToString("yyyy-MM-dd"));
            Para.Add("@ToDate", EndDate?.ToString("yyyy-MM-dd"));

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", Para);
            return ds;
        }

        /// <summary>
        /// Gets the list of roles (ForType) from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<DataSet> GetForType()
        {
            Dictionary<string, String> Para = new Dictionary<string, string>();
            Para.Add("@Flag", "ShowRoleSH");
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", Para);
            return ds;
        }

        /// <summary>
        /// Gets the list of subtypes for notices.
        /// </summary>
        /// <returns></returns>
        public async Task<DataSet> GetSubType()
        {
            Dictionary<string, String> Para = new Dictionary<string, string>();
            Para.Add("@Flag", "ShowSubtypeSH");
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", Para);
            return ds;
        }

        /// <summary>
        /// Gets the display name values from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<DataSet> ShowDisplyeName()
        {
            Dictionary<string, String> Para = new Dictionary<string, string>();
            Para.Add("@Flag", "ShowDisplyeName");
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", Para);
            return ds;
        }

        /// <summary>
        /// Retrieves society codes from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<DataSet> GetSocityCode()
        {
            Dictionary<string, String> Para = new Dictionary<string, string>();
            Para.Add("@Flag", "ShowSocityCodeSH");
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", Para);
            return ds;
        }



        /// <summary>
        /// Gets the name of the member using their staff code.
        /// </summary>
        /// <param name="staffCode"></param>
        /// <returns></returns>
        public async Task<DataSet> GetMemberName(string staffCode)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "ShowMemberNameSH");
            para.Add("@StaffCode", staffCode);
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }

        public async Task<DataSet> GetWingId(string WingId)
        {
            Dictionary<string, String> Para = new Dictionary<string, string>();
            Para.Add("@Flag", "GetWingIdSH");
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", Para);
            return ds;
        }

        public async Task<DataSet> GetSecurityMember(string staffCode)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "GetSecurityMemberSH");

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }


        /// <summary>
        /// Saves new notice data to the database.
        /// </summary>
        /// <param name="objU"></param>
        /// <returns></returns>
        public async Task SaveData(Secretary objU)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "SaveNoticeSH");
            para.Add("@NoticeCode", objU.NoticeCode);
            para.Add("@NoticeTitle", objU.NoticeTitle);
            para.Add("@CreatedDate", objU.CreatedDate.ToString("yyyy-MM-dd"));
            para.Add("@DeadLineDate", objU.DeadlineDate.ToString("yyyy-MM-dd"));
            para.Add("@Description", objU.Description.ToString());
            para.Add("@SendBy", objU.SendBy);
            para.Add("@SendTo", objU.SelectedMember != null ? objU.SelectedMember.ToString() : "");
            para.Add("@SubtypeId", objU.SelectedNoticeType.ToString());
            para.Add("@ForType", objU.SelectedForType.ToString());
            para.Add("@StartDate", objU.StartDate.ToString("yyyy-MM-dd"));
            para.Add("@Document", !string.IsNullOrEmpty(objU.Document) ? objU.Document : "");

            await obj.ExecuteStoreProcedure("Secretary", para);
        }

        /// <summary>
        /// Updates an existing notice based on the given ID.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="objU"></param>
        /// <returns></returns>
        public async Task UpdateData(int? Id, Secretary objU)
        {
            if (!Id.HasValue) return;

            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "UpdateNoticeSH");
            para.Add("@NoticeId", objU.NoticeId.ToString());
            para.Add("@NoticeTitle", objU.NoticeTitle);
            para.Add("@Description", objU.Description);
            para.Add("@StartDate", objU.StartDate.ToString("yyyy-MM-dd"));
            para.Add("@DeadLineDate", objU.DeadlineDate.ToString("yyyy-MM-dd"));

            // para.Add("@SubtypeId", objU.SelectedNoticeType.ToString());
            // para.Add("@SubType", objU.SubType);
            para.Add("@SubTypeId", objU.SelectedNoticeType.ToString());

            // para.Add("@Document", objU.Document);

            await obj.ExecuteStoreProcedure("Secretary", para);
        }

        /// <summary>
        /// Retrieves a single notice record by its ID.
        /// </summary>
        /// <param name="objU"></param>
        /// <returns></returns>
        public async Task<SqlDataReader> GetById(Secretary objU)
        {
            Dictionary<string, string> Getdata = new Dictionary<string, string>();
            Getdata.Add("@Flag", "GetByCodeSH");
            Getdata.Add("@NoticeId", Convert.ToInt32(objU.NoticeId).ToString());
            SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("Secretary", Getdata);
            return dr;
        }

        #endregion

        #region*********************************************************************Ajay Ugale***********************************************************
        /// <summary>
        /// This Method is used for showing visitors
        /// </summary>
        /// <returns></returns>
        public async Task<DataSet> ShowList(string StaffCode ,DateTime? StartDate, DateTime? EndDate)
        {
            Dictionary<string, string> Getdata = new Dictionary<string, String>();
            Getdata.Add("@Flag", "ShowVisitorAU");
            Getdata.Add("@StartDate", StartDate?.ToString("yyyy-MM-dd"));
            Getdata.Add("@EndDate", EndDate?.ToString("yyyy-MM-dd"));
            Getdata.Add("@StaffCode", StaffCode);


            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", Getdata);
            return ds;


        }
        #endregion

        #region******************************************************* Vivek Kumbhar #Maintance# *****************************************************************************************
        /// <summary>
        /// this method use for geeting all meaintenance list
        /// </summary>
        /// <param name="vr"></param>
        /// <returns></returns>
        public async Task<DataSet> GetMaintenanceDetailsVK(Secretary vr)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getlistindetails");
            para.Add("@StaffCode", vr.StaffCode);
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }
        /// <summary>
        /// this method use for getting vendor list 
        /// </summary>
        /// <returns>dataset vendor list </returns>
        public async Task<DataSet> GetVendorList()
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getvenlist");
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }
        /// <summary>
        /// this is method use to getting floor list 
        /// </summary>
        /// <returns> floorID and floorcode</returns>

        public async Task<DataSet> GetFloorList(Secretary vk)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getfloor");
            para.Add("@StaffCode", vk.StaffCode);
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }
        /// <summary>
        /// this method use to save schedule maintenance 
        /// </summary>
        /// <param name="objU"></param>
        /// <returns>return nothing</returns>
        public async Task SaveScheduleMainVK(Secretary objU)
        {

            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "savemaintenance");
            para.Add("@Name", objU.MaintanceNamae);
            para.Add("@amount", objU.TotalAmount.ToString());
            para.Add("@WingId", objU.WingID.ToString());
            para.Add("@Description", objU.Description);
            para.Add("@typeId", objU.MainType.ToString());
            para.Add("@EndDate", objU.CompleteDates.ToString());
            para.Add("@StaffCode", objU.StaffCode);
            para.Add("@createdate", objU.CreateDates);
            await obj.ExecuteStoreProcedure("Secretary", para);
        }
        /// <summary>
        /// this method use for update maintenacnce task
        /// </summary>
        /// <param name="objU"></param>
        /// <returns></returns>
        public async Task UpdateScheduleMainVK(Secretary objU)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "UpdateSchedulemai");
            para.Add("@Name", objU.MaintanceNamae);
            para.Add("@amount", objU.Amount.ToString());
            para.Add("@VendorCode", objU.VendorCode.ToString());
            para.Add("@WingId", objU.WingID.ToString());
            para.Add("@Description", objU.Description);
            para.Add("@createdate", (DateTime.Now).ToString());
            para.Add("@Id", objU.ID.ToString());
            await obj.ExecuteStoreProcedure("Secretary", para);
        }
        /// <summary>
        /// this smethod use for grting whole mentanince details
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<SqlDataReader> GetDatailsVK(Secretary objU)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getlistbyID");
            para.Add("@Id", objU.ID.ToString());
            SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("Secretary", para);
            return dr;
        }
        /// <summary>
        /// this method use for delete maintenacne 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task DeleteMaintenance(Secretary objU)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getdelete");
            para.Add("@Id", objU.ID.ToString());
            await obj.ExecuteStoreProcedure("Secretary", para);
        }
        /// <summary>
        /// this method use for getting wings id
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<SqlDataReader> GetWingsID(Secretary objU)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getwingsid");
            para.Add("@StaffCode ", objU.StaffCode);
            SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("Secretary", para);
            return dr;
        }
        /// <summary>
        /// this method use for getting fix maintenace list 
        /// </summary>
        /// <param name="vr"></param>
        /// <returns>Dataset List of fix maintenance</returns>
        public async Task<DataSet> GetFixMaintenanceVK(Secretary vr)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getlist");
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }
        /// <summary>
        /// this method is use for getting maintence code
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<SqlDataReader> GetMainteCodeVK(Secretary objU)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getmaincode");
            para.Add("@StaffCode ", objU.StaffCode);
            SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("Secretary", para);
            return dr;
        }
        /// <summary>
        /// save maintence details in database 
        /// </summary>
        /// <param name="objU"></param>
        /// <returns></returns>
        public async Task SaveMaintVK(Secretary objU)
        {

            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "savemaindetails");
            para.Add("@Name", objU.MainName);
            para.Add("@amount", objU.Amount.ToString());
            para.Add("@createdate", objU.StartDate.ToString());
            para.Add("@NextCode", objU.MaintenanceCode);
            await obj.ExecuteStoreProcedure("Secretary", para);
        }
        /// <summary>
        /// this method use for getting first record date and last record date particular staff vice
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<SqlDataReader> GetFistLastDateVK(Secretary objU)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getlastfirstdate");
            para.Add("@StaffCode ", objU.StaffCode);
            SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("Secretary", para);
            return dr;
        }
        /// <summary>
        /// this method use for check maintenance paID or not and getting all information about maintenance
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<SqlDataReader> GetMntHistoryVK(Secretary objU)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getmaindetbrif");
            para.Add("@StaffCode ", objU.StaffCode);
            para.Add("@ToDate ", objU.CompleteDates.ToString());
            para.Add("@FromDate ", objU.CreateDates.ToString());
            para.Add("@mentenancecode", objU.MaintenanceCode);
            SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("Secretary", para);
            return dr;
        }
        /// <summary>
        /// this method use for get maintenace complit date  
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<SqlDataReader> GetCompliDateVK(Secretary objU)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getpaiddate");
            para.Add("@mentenancecode ", objU.MaintenanceCode);
            SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("Secretary", para);
            return dr;
        }
        /// <summary>
        /// this method use for getting paID Member list 
        /// </summary>
        /// <param name="vr"></param>
        /// <returns></returns>
        public async Task<DataSet> GetPaidMemListVK(Secretary vr)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getpaidmemlist");
            para.Add("@StaffCode ", vr.StaffCode);
            para.Add("@createdate", vr.CreateDates);
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }
        /// <summary>
        /// this method use for getting mentance details id visse
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        public async Task<DataSet> GetUnpaidMemListVK(Secretary vr)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "mntunpaidlist");
            para.Add("@StaffCode ", vr.StaffCode);
            para.Add("@createdate", vr.CreateDates);
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }

        /// <summary>
        /// this method use for getting mentance details id visse
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<SqlDataReader> GetselMainVK(Secretary objU)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getselMain");
            para.Add("@Id", objU.ID.ToString());
            SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("Secretary", para);
            return dr;
        }
        /// <summary>
        /// this method getting paID histrory of member
        /// </summary>
        /// <param name="vr"></param>
        /// <returns></returns>
        public async Task<DataSet> GetMemmenyListVK(Secretary vr)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getmemmentdatils");
            para.Add("@membercode", vr.MemeberCode);
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }
        /// <summary>
        /// this method use for getting maintenance record
        /// </summary>
        /// <param name="vr"></param>
        /// <returns></returns>
        public async Task<DataSet> GetMaintenanceDetailsRecordVK(Secretary vr)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getmaindatailrecord");
            para.Add("@mentenancecode", vr.MaintenanceCode);
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }
        /// <summary>
        /// this method use for getting info about resIDent
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<SqlDataReader> GetmemInfoVK(Secretary objU)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getmemdetail");
            para.Add("@membercode", objU.MemeberCode);
            SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("Secretary", para);
            return dr;
        }

        public async Task<SqlDataReader> GetWingResIDentCountVK(Secretary objU)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getmemcount");
            para.Add("@StaffCode", objU.StaffCode.ToString());
            SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("Secretary", para);
            return dr;
        }

        /// <summary>
        /// this method you for getting resident emails
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<SqlDataReader> GetResidentEmailsVK(Secretary objU)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "GetResidentEmailsVK");
            para.Add("@Wingid", objU.WingID.ToString());
            SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("Secretary", para);
            return dr;
        }
        public async Task<SqlDataReader> GetInfoMaintenanceDetailsVK(Secretary objU)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "GetInfoMaintDatilVK");
            para.Add("@mentenancecode ", objU.MaintenanceCode);
            SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("Secretary", para);
            return dr;
        }
        /// <summary>
        /// this is method use for getting info about sub maintenance list to pertuculer maintenance 
        /// </summary>
        /// <param name="vr"></param>
        /// <returns></returns>
        public async Task<DataSet> GetMaintenanceSubDetailsVK(Secretary vr)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "getmaintenancesub");
            para.Add("@mentenancecode", vr.MaintenanceCode);
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }
        /// <summary>
        ///  this is method Unuse for getting info about sub maintenance list to pertuculer maintenance 
        /// </summary>
        /// <param name="vr"></param>
        /// <returns></returns>

        public async Task UpdateMaintenance(Secretary vr)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "UpdateSchedulemai");
            para.Add("@amount", vr.TotalAmount.ToString());
            para.Add("@mentenancecode", vr.MaintenanceCode.ToString());
            para.Add("@Description", vr.Description);
            para.Add("@createdate", vr.CreateDates);
            para.Add("@EndDate", vr.CompleteDates.ToString());
            await obj.ExecuteStoreProcedure("Secretary", para);
        }
        /// <summary>
        /// this method use for delete sub maintenace 
        /// </summary>
        /// <param name="vr"></param>
        /// <returns></returns>
        public async Task DeleteSubMaintenanceVk(Secretary vr)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@flag", "submaintenancedelete");
            para.Add("@mentenancecode", vr.MaintenanceCode);
            await obj.ExecuteStoreProcedure("Secretary", para);
        }
        /// <summary>
        /// this method use for delete maintenace 
        /// </summary>
        /// <param name="vr"></param>
        /// <returns></returns>
        public async Task DeleteMaintenanceVK(Secretary vr)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@flag", "maintenancedelete");
            para.Add("@mentenancecode", vr.MaintenanceCode);
            await obj.ExecuteStoreProcedure("Secretary", para);
        }
        /// <summary>
        /// this method use for getting unselected sub maintenance
        /// </summary>
        /// <param name="vr"></param>
        /// <returns></returns>
        public async Task<DataSet> suselectedfixmaintenace(Secretary vr)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "suselectedfixmaint");
            para.Add("@mentenancecode", vr.MaintenanceCode);
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Secretary", para);
            return ds;
        }
        #endregion


    }
}


