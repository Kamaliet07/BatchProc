#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PTMSBatchUtility;
using System.Configuration;
using System.Data;
using System.Collections;
using Microsoft.Reporting.WebForms;
using System.IO;
#endregion

namespace PTW_RC_DBBalancingCYSupplemental
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] returnValue = new int[2];
            int executionID = 0;
            string batchCode = string.Empty;
            string batchName = string.Empty;
            string[] returnMessage;
            bool IsCurrentAssessmentYear = true;
            int headerID = 0;
            string batchParameters = string.Empty;
            bool isSuccess = false;
           // string createdBy = "SYSTEM";
            try
            {
                batchCode = ConfigurationSettings.AppSettings["BatchCode"];
                executionID = BatchUtility.CreateaExecutionID(batchCode);
                //check to avoid duplicate runs
                if (!BatchUtility.IsDuplicateRun())
                {

                    //Fetch Batch Name from DB
                    batchName = BatchUtility.GetBatchNameBasedOnCode(batchCode, executionID);
                    //log batch start message as type "Information"
                    BatchUtility.LogMessage(batchName + " Batch Started at --" + DateTime.Now.ToString() + ".", executionID, "INFO");
                    returnMessage = BatchUtility.ValidateExecutionID(executionID, batchCode, "");

                    if (returnMessage[1] == "True")
                    {

                        //Log Information of Valid ExecutionID
                        BatchUtility.LogMessage(returnMessage[0], Convert.ToInt32(returnMessage[2]), "INFO");
                        //Actual Batch Call
                        returnValue = DBBalancingCYSupplementalDataAccess.RetrieveControlTotals(executionID, IsCurrentAssessmentYear);
                        
                        if (returnValue[0] >0)
                        {
                              headerID=returnValue[1];
                              BatchUtility.LogMessage(batchName + "  Completed Creating Control Totals With Header ID " + headerID.ToString() + " -- " + DateTime.Now.ToString(), Convert.ToInt32(returnMessage[2]), "INFO");
                            SaveReportOnBase(executionID, headerID,"RC0006");
                            SaveReportOnBase(executionID, headerID, "RC0018");

                            //Log Batch Success
                            BatchUtility.LogMessage(batchName + " Batch execution finished successfully.", executionID, "SUC");
                            //Updating the Execution End date
                            BatchUtility.UpdateExecutionEndDate(true, Convert.ToInt32(returnMessage[2]));
                            isSuccess = true;

                        }
                        else
                        {
                            //Log Batch Failure
                            BatchUtility.LogMessage(batchName + " Batch execution failed.", executionID, "FAIL");
                            //Updating the Execution End date
                            BatchUtility.UpdateExecutionEndDate(false, Convert.ToInt32(returnMessage[2]));
                           
                        }
                    }
                    else
                    {
                        //Log Information of Invalid Execution ID
                        BatchUtility.LogMessage(returnMessage[0], Convert.ToInt32(returnMessage[2]), "INFO");
                        
                    }
                }
                else
                {
                    //Log Information of duplicate running batch
                    BatchUtility.LogMessage("Batch already in progress. ", executionID, "INFO");
                    
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                //Log Batch Exceptions
                BatchUtility.LogMessage(ex.Message + "--" + DateTime.Now.ToString(), executionID, "EXCEP");
                
            }
            finally
            {
                //Final Log Batch Information
                BatchUtility.LogMessage(batchName + " Batch execution finished at" + "--" + DateTime.Now.ToString() + ".", executionID, "INFO");
                BatchUtility.SendExitCodeToControlM(isSuccess);
            }
        }

        #region Code to Save Report Onbase
        /// <summary>
        /// This functions save report to OnBase ftp
        /// </summary>
        /// <param name="executionID">Execution Id of the batch</param>
        /// <param name="headerId">The parameter required to generate the report</param>
        /// <param name="ReportCode">Report Code from PTMS which has to be generated.</param>
        /// <returns></returns>
        protected static bool SaveReportOnBase(int executionID, int headerId, string ReportCode)
        {
            //DataSet documentGroup = null;
            string documentTypeGroupName = string.Empty;
            string byDate = string.Empty;
            DataTable getReportserver = null;
            string reportServer = string.Empty;
            string reportLocation = string.Empty;
            string sFTPLocation = string.Empty;
            string sFTPUserName = string.Empty;
            string sFTPPassword = string.Empty;
            string Msg = string.Empty;
            DataTable getDocumentTypeForReports = null;
            string onbaseDocumentType = string.Empty;
            string onbaseDocumentGroup = string.Empty;
            string[] installmentSecuredReportParameters = null;
            bool onBaseSuccess = false;
            string folder = ConfigurationSettings.AppSettings["ReportFolder"];

            //Declaring Index File Keys variables

            string indexFileKey = string.Empty;
            string[] indexFileKeyParameters = null;
            try
            {
                getDocumentTypeForReports = BatchUtility.GetDocumentGroupAndTypeValue(ReportCode);
                if (getDocumentTypeForReports != null && getDocumentTypeForReports.Rows.Count > 0
                     && getDocumentTypeForReports.Columns.Contains("OnbaseDocumentGroup")
                     && getDocumentTypeForReports.Columns.Contains("OnbaseDocumentType")
                    //&& (!string.IsNullOrEmpty(getDocumentTypeForReports.Rows[0]["OnbaseDocumentGroup"].ToString()))
                    // && (!string.IsNullOrEmpty(getDocumentTypeForReports.Rows[0]["OnbaseDocumentType"].ToString()))
                    )
                {
                    onbaseDocumentGroup = getDocumentTypeForReports.Rows[0]["OnbaseDocumentGroup"].ToString();
                    onbaseDocumentType = getDocumentTypeForReports.Rows[0]["OnbaseDocumentType"].ToString();
                }

                ////TO BE ADDED//
                //documentGroup = BatchUtility.GetConfigValue("AC", "PEN", "DocumentGroup");
                //if (documentGroup != null && documentGroup.Tables.Count > 0 && documentGroup.Tables[0] != null && documentGroup.Tables[0].Rows.Count > 0)
                //{
                //    documentTypeGroupName = documentGroup.Tables[0].Rows[0]["Value"].ToString();
                //}
                //TO BE CHANGED//

                getReportserver = BatchUtility.getReportServerDetails(folder);

                if (getReportserver != null && getReportserver.Rows.Count > 0
                     && getReportserver.Columns.Contains("ReportServerName")
                     && getReportserver.Columns.Contains("FolderName")
                     && (!string.IsNullOrEmpty(getReportserver.Rows[0]["ReportServerName"].ToString()))
                      && (!string.IsNullOrEmpty(getReportserver.Rows[0]["FolderName"].ToString())))
                {
                    reportServer = getReportserver.Rows[0]["ReportServerName"].ToString();
                    reportLocation = getReportserver.Rows[0]["FolderName"].ToString();



                    DataSet FTPLocation = BatchUtility.GetConfigValue("PTMS", "OnBase_FTP", "ServerAddress");
                    DataSet FTPUserName = BatchUtility.GetConfigValue("PTMS", "OnBase_FTP", "UserName");
                    DataSet FTPPassword = BatchUtility.GetConfigValue("PTMS", "OnBase_FTP", "Password");
                    if (FTPLocation != null && FTPLocation.Tables.Count > 0 && FTPLocation.Tables[0] != null && FTPLocation.Tables[0].Rows.Count > 0) //Null Check Implemented
                    {
                        sFTPLocation = FTPLocation.Tables[0].Rows[0]["Value"].ToString();
                    }
                    if (FTPUserName != null && FTPUserName.Tables.Count > 0 && FTPUserName.Tables[0] != null && FTPUserName.Tables[0].Rows.Count > 0) //Null Check Implemented
                    {
                        sFTPUserName = FTPUserName.Tables[0].Rows[0]["Value"].ToString();
                    }
                    if (FTPPassword != null && FTPPassword.Tables.Count > 0 && FTPPassword.Tables[0] != null && FTPPassword.Tables[0].Rows.Count > 0) //Null Check Implemented
                    {
                        sFTPPassword = FTPPassword.Tables[0].Rows[0]["Value"].ToString();
                    }
                    string onBaseFTPMessageForSecuredReport = string.Empty;
                    string exportLocation = Environment.CurrentDirectory + @"\";
                    string reportParameters = headerId.ToString();
                    installmentSecuredReportParameters = reportParameters.Split(',');

                    //Writing Index File Key to String Array

                    if (!string.IsNullOrEmpty(onbaseDocumentType) && !string.IsNullOrEmpty(onbaseDocumentGroup))
                    {
                        indexFileKey = "ExecutionID," + executionID.ToString() + ",RunDate," + DateTime.Now.ToShortDateString();

                        indexFileKeyParameters = indexFileKey.Split(',');

                    }

                    //Confirm
                    string reportName = ConfigurationSettings.AppSettings[ReportCode];
                    String ReportValue = BatchUtility.GetFileName(reportName, executionID);
                    byte[] OutputbytesForSecuredReport = ExportReportAs(reportServer, reportLocation + reportName, exportLocation, reportName, "pdf", reportName, executionID, installmentSecuredReportParameters);
                    onBaseFTPMessageForSecuredReport = BatchUtility.UploadtoOnBaseFTP(sFTPLocation, sFTPUserName, sFTPPassword
                                                                                , ReportValue + ".pdf", OutputbytesForSecuredReport, onbaseDocumentType, installmentSecuredReportParameters, 0, indexFileKeyParameters
                                                                                , onbaseDocumentType, onbaseDocumentGroup, "Roll Corrections");
                    if (onBaseFTPMessageForSecuredReport == "1")
                        onBaseSuccess = true;

                }
            }
            catch (Exception Ex)
            {
                Msg = "Error Details : " + Ex.Message.ToString();
                BatchUtility.LogMessage(Ex.Message + "--" + DateTime.Now.ToString(), executionID, "EXCEP");
                throw Ex;
            }
            return onBaseSuccess;
        }

        #endregion

        #region Function to export report
        /// <summary>
        /// Function to export report to byte array
        /// </summary>
        /// <param name="RptServerUrl">Report Server URL</param>
        /// <param name="RptPath">Path at which report resides in the Report Server</param>
        /// <param name="ExportPath">Path where report will be exported temporarily</param>
        /// <param name="ExportFName">Name of the file which has to be generated</param>
        /// <param name="ExportFormat">Format in which report has to be exported</param>
        /// <param name="reportName">Actula name of RDL</param>
        /// <param name="executionID">Execution ID of theb atch generating this report.</param>
        /// <returns></returns>
        public static byte[] ExportReportAs(String RptServerUrl, String RptPath, String ExportPath, String ExportFName, String ExportFormat, string reportName, int executionID, string[] reportParameters)
        {
            byte[] bytes = null;
            string Msg = string.Empty;
            ReportViewer RptViewer = new ReportViewer();
            try
            {
                RptViewer.ProcessingMode = ProcessingMode.Remote;
                RptViewer.ServerReport.ReportServerUrl = new @Uri(RptServerUrl);
                RptViewer.ServerReport.ReportPath = RptPath;
                RptViewer.ServerReport.Refresh();
                ReportParameter[] param = null;
                param = new ReportParameter[reportParameters.Length];
                param[0] = new ReportParameter("BatchHeaderID", reportParameters[0]);
                //param[1] = new ReportParameter("InstallmentNo", reportParameters[1]);

                //param[0] = new ReportParameter("ASSESSMENTYEAR", assessmentYear.ToString());
                //param[1] = new ReportParameter("ExecutionID", executionID);
                //param[2] = new ReportParameter("ROLLDELIVERYNO", rollDeliveryNo);
                //param[3] = new ReportParameter("ROLLDELIVERYNO", rollDeliveryNo);

                RptViewer.ServerReport.SetParameters(param);

                switch (ExportFormat.ToUpper())
                {
                    case "PDF": ExportFName += ".pdf";
                        break;
                    case "XML": ExportFName += ".xml";
                        break;
                    case "EXCEL": ExportFName += ".xls";
                        break;
                    case "WORD": ExportFName += ".doc";
                        break;
                }
                Warning[] warnings;
                string[] streamids;
                string mimeType, encoding, filenameExtension;
                bytes = RptViewer.ServerReport.Render(ExportFormat, null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            }
            catch (Exception Ex)
            {
                Msg = "Error Details : " + Ex.Message.ToString();
                BatchUtility.LogMessage(Ex.Message + "--" + DateTime.Now.ToString(), executionID, "EXCEP");
                throw Ex;
            }
            return bytes;
        }
        #endregion
    }
}



    






