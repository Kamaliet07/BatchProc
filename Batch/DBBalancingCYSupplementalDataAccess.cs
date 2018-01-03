#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using PTMSBatchUtility;
#endregion

namespace PTW_RC_DBBalancingCYSupplemental
{
    class DBBalancingCYSupplementalDataAccess
    {
        #region Function To Get The Batch Header ID By Calling The Control Totals For Roll Batch
        /// <summary>
        /// Function To Get The Batch Header ID By Calling The Control Totals For Roll Batch
        /// </summary>
        /// <param name="executionID"></param>
        /// <returns></returns>
        public static int[] RetrieveControlTotals(int executionID, Boolean isCurrentAssessmentYear)
        {
            string rollType = string.Empty;
            //int returnValue=0;
            int[] returnValue = new int[2];
            rollType = "SUPPL";


            try
            {
                Database database = DatabaseFactory.CreateDatabase();
                string sqlCommand = "usp_RC_ControlTotalCaller";
                DbCommand dbCommand = database.GetStoredProcCommand(sqlCommand);
                dbCommand.CommandTimeout = 0;
                database.AddInParameter(dbCommand, "@RollTypeCode", DbType.String, rollType);
                database.AddInParameter(dbCommand, "@IsCurrentYear", DbType.Int32, isCurrentAssessmentYear);
                database.AddInParameter(dbCommand, "@executionID", DbType.Int32, executionID);
                //database.AddInParameter(dbCommand, "@CreatedBy", DbType.String, createdBy);
                database.AddOutParameter(dbCommand, "@returnCode", DbType.Int32, 0);
                database.ExecuteNonQuery(dbCommand);
                returnValue[0] = Convert.IsDBNull(database.GetParameterValue(dbCommand, "returnCode")) ? 0 : Convert.ToInt32(database.GetParameterValue(dbCommand, "returnCode"));
                returnValue[1] = returnValue[0];

            }
            catch (Exception ex)
            {
                BatchUtility.LogMessage(ex.Message + "--" + DateTime.Now.ToString(), executionID, "EXCEP");
            }
            return returnValue;

        }
        #endregion
    }
}
