using GSTSMSHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTSMSLibrary.Security
{
    public class BALSecurity
    {
        MSSQL obj = new MSSQL();

        #region*********************************************************************Saniya Shaikh***********************************************************

        /// <summary>
        /// Retrieves the details of worker attendance
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        /// 

        public async Task<DataSet> AttendanceListSS(DateTime? fromDate, DateTime? toDate)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("@Flag", "ShowWorkerAttendanceSS");
            para.Add("@StartDate", fromDate?.ToString("yyyy-MM-dd") ?? "");
            para.Add("@EndDate", toDate?.ToString("yyyy-MM-dd") ?? "");
            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Security", para);
            return ds;
        }

        #endregion


        #region*********************************************************************Ajay Ugale***********************************************************


        //public List<Security> lstofworker { get; set; }

        /// <summary>
        /// This Method is used for showing visitors  
        /// </summary>
        /// <returns></returns>
        public async Task<DataSet> ShowlistAU()
            {

                Dictionary<string, string> Getdata = new Dictionary<string, string>();
                Getdata.Add("@Flag", "showvisitorsAU");



                DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Security", Getdata);
                return ds;
            }


        /// <summary>
        /// This method is used for a Geting a Wings
        /// </summary>
        /// <returns></returns>

        public async Task<DataSet> GetWing(string societyCode) 
        {
            Dictionary<string, string> getwing = new Dictionary<string, string>();
            getwing.Add("@Flag", "ShowWingAU");
            getwing.Add("@SocietyCode", societyCode); 

            DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Security", getwing);
            return ds;
        }

        /// <summary> 
        /// This method is used for a Geting a Flats
        /// </summary>
        /// <param name="WingId"></param>
        /// <returns></returns>

        public async Task<DataSet> GetFlats(int WingId)
            {
                Dictionary<string, string> getflat = new Dictionary<string, string>();
                getflat.Add("@Flag", "ShowFlatAU");
                getflat.Add("@WingId", Convert.ToInt32(WingId).ToString());
                DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Security", getflat);
                return ds;
            }



            /// <summary>
            /// This Method  is used for a getting Visitor Type like a Delivery,Guest,Service,Vendor,Worker
            /// </summary>
            /// <returns></returns>

            public async Task<DataSet> GetVisitorType()
            {
                Dictionary<string, string> getwing = new Dictionary<string, string>();
                getwing.Add("@Flag", "ShowingVisitorTypeAU");
                DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Security", getwing);
                return ds;
            }

            /// <summary>
            /// This Method is used for getting a vehicle Type 
            /// </summary>
            /// <returns></returns>
            public async Task<DataSet> GetVehicleType()
            {
                Dictionary<string, string> getwing = new Dictionary<string, string>();
                getwing.Add("@Flag", "VisitorVTypeAU");
                DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Security", getwing);
                return ds;
            }

            /// <summary>
            /// This Method is used for getting a Visitors Status
            /// </summary>
            /// <returns></returns>
            public async Task<DataSet> GetVisitorsStatus()
            {
                Dictionary<string, string> getwing = new Dictionary<string, string>();
                getwing.Add("@Flag", "VisitorStatusAU");
                DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Security", getwing);
                return ds;
            }
            /// <summary>
            /// This Method is used for getting a visitors Alloted parking
            /// </summary>
            /// <returns></returns>
            public async Task<DataSet> VisitorParking(int typeid)
            {
                Dictionary<string, string> getwing = new Dictionary<string, string>();
                getwing.Add("@Flag", "allocateparkingAU");
                getwing.Add("@VehicleTypeID", Convert.ToInt32(typeid).ToString());
                DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Security", getwing);
                return ds;

            }
            /// <summary>
            /// This Method is used for a Save Visitors
            /// </summary>
            /// <param name="objU"></param>
            /// <returns></returns>
            public async Task SaveVisitors(Security objU)
            {
                Dictionary<string, string> Data = new Dictionary<string, string>();
                Data.Add("@Flag", "AddVisitorsAU");
                Data.Add("@VisitorCode", objU.VisitorCode);
                Data.Add("@VisitorName", objU.VisitorName);
                Data.Add("@VisitorTypeId", Convert.ToInt32(objU.SelectedType).ToString());
                Data.Add("@VisitorCount", Convert.ToString(objU.VisitorCount));
                Data.Add("@Reason", objU.Reason);


                if (!string.IsNullOrEmpty(objU.FlatCode))
                {
                    if (objU.FlatCode.Length == 1 && objU.FlatCode[0] >= 'A' && objU.FlatCode[0] <= 'Z')
                    {
                        Data.Add("@FlatCode", Convert.ToString(objU.FlatId));
                    }
                    else
                    {
                        Data.Add("@FlatCode", objU.FlatCode);
                    }
                }
                else
                {
                    Data.Add("@FlatCode", Convert.ToString(objU.FlatId));
                }

                Data.Add("@SubTypeId", Convert.ToInt32(objU.SelectedVehicleType).ToString());
                Data.Add("@VehicleNumber", objU.VehicleNumber);
                Data.Add("@VisitorContact", objU.VisitorContact);
                Data.Add("@ParkingCode", objU.ParkingCode);
                Data.Add("@CheckIn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                await obj.ExecuteStoreProcedure("Security", Data);
            }



            /// <summary>
            /// This method is used for a add the visitorcode and that is autoincreament
            /// </summary>
            /// <returns></returns>
            public async Task<DataSet> AddVisitorCode()
            {
                Dictionary<string, string> getcode = new Dictionary<string, string>();
                getcode.Add("@Flag", "AddVISCodeAU");
                DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Security", getcode);
                return ds;
            }



            /// <summary>
            /// This Method is used for Save the visitors Documents
            /// </summary>
            /// <param name="objU"></param>
            /// <returns></returns>
            public async Task SaveDocuments(Security objU)


            {
                Dictionary<string, string> Data = new Dictionary<string, string>();
                Data.Add("@Flag", "SaveDocumentsAU");
                Data.Add("@SubTypeId", Convert.ToString(objU.SubTypeId));
                Data.Add("@Document", objU.Document);
                Data.Add("@VisitorCode", objU.VisitorCode);
                Data.Add("@Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                await obj.ExecuteStoreProcedure("Security", Data);
            }

            /// <summary>
            /// this method is used for view the particular visitors Details
            /// </summary>
            /// <param name="VisitorId"></param>
            /// <returns></returns>
            public async Task<SqlDataReader> ViewVisitors(int VisitorId)
            {


                Dictionary<string, string> Getdata = new Dictionary<string, string>();
                Getdata.Add("@Flag", "ViewVisitorsAU");
                Getdata.Add("@VisitorId", Convert.ToInt32(VisitorId).ToString());

                SqlDataReader dr = await obj .ExecuteStoreProcedureReturnDataReader("Security", Getdata);
                return dr;
            }

            /// <summary>
            /// This Method for update the visitor Status, "Checkout"
            /// </summary>
            /// <param name="objU"></param>
            /// <param name="VisitorId"></param>
            /// <returns></returns>

            public async Task Checkout(Security objU, int VisitorId)
            {
                Dictionary<string, string> Data = new Dictionary<string, string>();
                Data.Add("@Flag", "CheckOutAU");
                Data.Add("@VisitorId", Convert.ToInt32(VisitorId).ToString());
                await obj.ExecuteStoreProcedure("Security", Data);
            }

            /// <summary>
            /// This method is used for the Show the Worker List
            /// </summary>
            /// <returns></returns>
            public async Task<DataSet> ShowWorker()
            {
                Dictionary<string, string> getworker = new Dictionary<string, string>();
                getworker.Add("@Flag", "ShowWorkerAU");
                DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Security", getworker);
                return ds;
            }


            /// <summary>
            /// This method is used for the show the vendor list 
            /// </summary>
            /// <returns></returns>
            public async Task<DataSet> Showvendor()
            {
                Dictionary<string, string> getwing = new Dictionary<string, string>();
                getwing.Add("@Flag", "ShowVendorAU");
                DataSet ds = await obj.ExecuteStoreProcedureReturnDS("Security", getwing);
                return ds;
            }
        }
    #endregion



}

