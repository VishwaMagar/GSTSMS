using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using GSTSMSHelper;

namespace GSTSMSLibrary.Account
{



    public class BALAccount
    {
        MSSQL obj = new MSSQL();

        public async Task<SqlDataReader> Login(Account objU)
        {
            Dictionary<string, string> GetData = new Dictionary<string, string>
            {
                { "@flag", "LoginRK" },
                { "@Email", objU.Email },
                { "@Password", objU.Password }
            };

            SqlDataReader dr = await obj.ExecuteStoreProcedureReturnDataReader("sp_SMS", GetData);
            return dr;
        }
    }
}