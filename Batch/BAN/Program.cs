using System;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Mail;
using PTMSBatchUtility;
using System.Net.Mail;

namespace PTMS_BAN_UnpdSuplSndBoDue
{
    class Program
    {
        static void Main(string[] args)
        {
            string batchCode = string.Empty;
            string batchName = string.Empty;
            int executionID = 0;
            string[] returnMessage;
            Utility util = new Utility();

            try
            {
                //Get the Batch Code from App Config 
                batchCode = ConfigurationSettings.AppSettings["BatchCode"];                          
                
                //if (args.Length > 0)
                //    executionID = Convert.ToInt32(args[0]);

                // [01/17/2012] Input parameter validation added by Kaushal for TFS #8660
                if (args.Length > 0)
                {
                    if ((int.TryParse(args[0], out executionID)) == false || executionID <= 0 || executionID > Int32.MaxValue)
                    {
                        Console.WriteLine("Parameter should be Numeric and It should be between 1 and " + Int32.MaxValue + ", Invalid Parameter : " + args[0].ToString());
                        //Console.ReadKey();
                        Environment.Exit(1);
                        return;
                    }
                }
                else
                {
                    string executionIDConsole = string.Empty;
                    Console.WriteLine("Please provide the Execution ID as parameter : ");
                    executionIDConsole = Console.ReadLine();
                    if ((int.TryParse(executionIDConsole, out executionID)) == false || executionID <= 0 || executionID > Int32.MaxValue)
                    {
                        Console.WriteLine("Parameter should be Numeric and It should be between 1 and 2147483647, Invalid Parameter : " + executionIDConsole.ToString());
                        Environment.Exit(1);
                        return;
                    }
                }

                Database dbUnPaidSuplSndBoDue = DatabaseFactory.CreateDatabase();
                DbCommand dbgetExecutionID = dbUnPaidSuplSndBoDue.GetStoredProcCommand("usp_BAN_Batch_UpdateBatchProcessingStatus");
                dbUnPaidSuplSndBoDue.AddInParameter(dbgetExecutionID, "@ExecutionId", DbType.Int32, executionID);
                dbUnPaidSuplSndBoDue.AddInParameter(dbgetExecutionID, "@batchCode", DbType.String, batchCode);
                dbUnPaidSuplSndBoDue.AddOutParameter(dbgetExecutionID, "@NewExecutionId", DbType.Int32, 0);

                dbUnPaidSuplSndBoDue.ExecuteDataSet(dbgetExecutionID);
                executionID = Convert.ToInt32(dbUnPaidSuplSndBoDue.GetParameterValue(dbgetExecutionID, "@NewExecutionId"));

                if (executionID == 0)
                {
                    Environment.Exit(1);
                    return;
                }


                //Get the Batch Name
                batchName = BatchUtility.GetBatchNameBasedOnCode(batchCode, executionID);

                //check to avoid duplicate runs
                if (!BatchUtility.IsDuplicateRun())
                {
                    //Validate the Execution ID
                    returnMessage = BatchUtility.ValidateExecutionID(executionID, batchCode, string.Empty);

                    //Log Information of Invalid Execution ID
                    if (returnMessage[1] != "True")
                    {
                        Console.WriteLine("Invalid Execution ID : " + executionID.ToString());
                        Console.ReadKey();
                        Environment.Exit(1);
                        return;
                    }

                    //log batch start message as type "Information"
                    BatchUtility.LogMessage(batchName + " batch started at --  " + System.DateTime.Now.ToString() + ".", executionID, "INFO");         //CHECK

                    BatchUtility.LogMessage(returnMessage[0], executionID, "INFO");

                    DbCommand dbgetBatchParameters = dbUnPaidSuplSndBoDue.GetStoredProcCommand("USP_BAN_BATCH_UnpaidSecBothInstDue");
                    dbUnPaidSuplSndBoDue.AddInParameter(dbgetBatchParameters, "ExecutionID", DbType.Int32, executionID);

                    DataTable Details = new DataTable("Assessment Year and 2nd Delinquent Date");

                    using (IDataReader drDueDate = dbUnPaidSuplSndBoDue.ExecuteReader(dbgetBatchParameters))
                        Details.Load(drDueDate);

                    string AssessmentYear = string.Empty;
                    DateTime DelinquentDate;
                    if (Details.Rows.Count > 0)
                    {
                        AssessmentYear = Details.Rows[0].ItemArray[0].ToString();
                        DelinquentDate = Convert.ToDateTime(Details.Rows[0].ItemArray[1].ToString());
                    }
                                       
                    string paramMsg = string.Empty;
                    paramMsg = "Batch runs with the following filter criteria: ";
                    paramMsg = paramMsg + Environment.NewLine + "1.Assessment Year = " + AssessmentYear;
                    paramMsg = paramMsg + Environment.NewLine + "2.2nd Installment Delinquent Date < Batch run Date";
                    paramMsg = paramMsg + Environment.NewLine + "3.Roll Type is Supplemental";
                    paramMsg = paramMsg + Environment.NewLine + "4.Tax Bill Type is Secured";
                    paramMsg = paramMsg + Environment.NewLine + "5.Tax bill is latest and approved.";
                    paramMsg = paramMsg + Environment.NewLine + "6.Tax bill is already printed.";
                    paramMsg = paramMsg + Environment.NewLine + "7.Reminder Notice is not already printed.";
                    paramMsg = paramMsg + Environment.NewLine + "8.Tax bill 2nd installment payment status is unpaid.";
                    paramMsg = paramMsg + Environment.NewLine + "9.APN type is NOT = Tracking Parcel";
                    paramMsg = paramMsg + Environment.NewLine + "10.APN type is NOT = Holding Parcel";

                    //BatchUtility.LogMessage(paramMsg, executionID, "PARAM");
                                       
                    DbCommand dbgetTaxBillCount = dbUnPaidSuplSndBoDue.GetStoredProcCommand("USP_BAN_BATCH_UnpaidSupSecOrBothInstDue1");
                    
                    dbUnPaidSuplSndBoDue.AddInParameter(dbgetTaxBillCount, "ExecutionID", DbType.Int32, executionID);
                    DataTable taxbillsdt = new DataTable("Supplemental Second or Both Installments Due Taxbills");

                    using (IDataReader drTaxBills = dbUnPaidSuplSndBoDue.ExecuteReader(dbgetTaxBillCount))
                        taxbillsdt.Load(drTaxBills);

                    if (taxbillsdt.Rows.Count == 0)
                    {
                        BatchUtility.LogMessage("No records found matching the criteria " + paramMsg, executionID, "INFO");
                        BatchUtility.LogMessage("No. of Tax Bills Processed = 0", executionID, "INFO");
                        //util.LogNoDataProcessed(executionID, 8);
                        BatchUtility.UpdatedBatchProcessingStatusCode(true, executionID);
                        BatchUtility.UpdateExecutionEndDate(true, executionID);


                        Environment.Exit(0);
                        return;
                    }

                    else
                    {
                        // Get the list of potential Tax bill Details
                        SqlDatabase dbGetTaxBillDetails = (SqlDatabase)DatabaseFactory.CreateDatabase();
                        DataTable taxBillDetails = new DataTable("Unpaid Supplemental Second or Both Installments Due Taxbills");
                        DbCommand cmdgetTaxBillDetails = dbGetTaxBillDetails.GetStoredProcCommand("USP_BAN_BATCH_UnpaidSupSecOrBothInstDue2");
                        dbGetTaxBillDetails.AddInParameter(cmdgetTaxBillDetails, "MYTABLEPARAM", SqlDbType.Structured, taxbillsdt);
                        dbGetTaxBillDetails.AddInParameter(cmdgetTaxBillDetails, "ExecutionID", DbType.Int32, executionID);

                        using (IDataReader drTaxBillDetails = dbGetTaxBillDetails.ExecuteReader(cmdgetTaxBillDetails))
                            taxBillDetails.Load(drTaxBillDetails);

                        if (taxBillDetails.Rows.Count == 0)
                        {
                            BatchUtility.LogMessage("Error occurred while getting the details for Tax Bills", executionID, "EXCEP");
                            util.LogNoDataProcessed(executionID, 8);
                            BatchUtility.UpdatedBatchProcessingStatusCode(false, executionID);
                            BatchUtility.UpdateExecutionEndDate(false, executionID);                            
                            Environment.Exit(1);
                            return;
                        }

                        //Filling Header Record into HeaderRow String
                        string headerLine = string.Empty;
                        string line2 = string.Empty;
                        string batchCodel = string.Empty;
                        batchCodel = ConfigurationSettings.AppSettings["BatchCode"];
                        line2 = " Report: TC11IB-70 As of ";
                        string n = DateTime.Now.ToString("MM-dd-yy");

                        headerLine = "Supplem 2nd unpaid notice".PadRight(25);//name
                        headerLine = headerLine + line2.PadRight(25);//Line 2
                        headerLine = headerLine + n.PadRight(8);//rundate
                        headerLine = headerLine + " Form F074-571";
                        headerLine = headerLine + "".PadRight(13);//Filler of length 13
                        headerLine = headerLine + "F4".PadRight(2);
                        headerLine = headerLine + "".PadRight(2);//Filler of length 2
                        headerLine = headerLine + taxBillDetails.Rows.Count.ToString().PadLeft(15);
                        headerLine = headerLine + "".PadRight(8);//Filler of length 8
                        headerLine = headerLine + "F7".PadRight(12);
                        headerLine = headerLine + "F8".PadRight(4);
                        headerLine = headerLine + "F9".PadRight(8);
                        headerLine = headerLine + "F10".PadRight(4);
                        headerLine = headerLine + "F11".PadRight(10);
                        headerLine = headerLine + "F12".PadRight(10);
                        headerLine = headerLine + "F13".PadRight(10);
                        headerLine = headerLine + "F14".PadRight(10);
                        headerLine = headerLine + "F15".PadRight(10);
                        headerLine = headerLine + "F16".PadRight(40);
                        headerLine = headerLine + "F17".PadRight(91);
                        headerLine = headerLine + "F18".PadRight(91);
                        headerLine = headerLine + "F19".PadRight(91);
                        headerLine = headerLine + "F20".PadRight(91);
                        headerLine = headerLine + "F21".PadRight(91);
                        headerLine = headerLine + "F22".PadRight(91);
                        headerLine = headerLine + "F23".PadRight(91);
                        headerLine = headerLine + "F24".PadRight(91);
                        headerLine = headerLine + "F25".PadRight(91);
                        headerLine = headerLine + "F26".PadRight(91);
                        headerLine = headerLine + "F27".PadRight(91);
                        headerLine = headerLine + "F28".PadRight(91);
                        headerLine = headerLine + "F29".PadRight(91);
                        headerLine = headerLine + "F30".PadRight(91);
                        headerLine = headerLine + "F31".PadRight(91);
                        headerLine = headerLine + "F32".PadRight(91);
                        headerLine = headerLine + "F33".PadRight(91);
                        headerLine = headerLine + "F34".PadRight(91);
                        headerLine = headerLine + "F35".PadRight(91);
                        headerLine = headerLine + "F36".PadRight(91);
                        headerLine = headerLine + "F37".PadRight(91);
                        headerLine = headerLine + "F38".PadRight(91);
                        headerLine = headerLine + "F39".PadRight(91);
                        headerLine = headerLine + "F40".PadRight(91);
                        headerLine = headerLine + "F41".PadRight(40);
                        headerLine = headerLine + "F42".PadRight(15);
                        headerLine = headerLine + "F43".PadRight(4);
                        headerLine = headerLine + "F44".PadRight(13);
                        headerLine = headerLine + "F45".PadRight(13);
                        headerLine = headerLine + "F46".PadRight(6);
                        headerLine = headerLine + "F47".PadRight(8);
                        headerLine = headerLine + "F48".PadRight(13);
                        headerLine = headerLine + "F49".PadRight(90);
                        headerLine = headerLine + "F50".PadRight(90);
                        headerLine = headerLine + "F51".PadRight(47);
                        headerLine = headerLine + "F52".PadRight(47);
                        headerLine = headerLine + "F53".PadRight(70);
                        headerLine = headerLine + "F54".PadRight(21);

                        int FileMaxRecCount = Convert.ToUInt16(util.GetConfigValue("TTC", "FSSI Interface File Max Record Count", "BAN FSSI Interface File Max Record Count", executionID));

                        int Reminder = taxbillsdt.Rows.Count % FileMaxRecCount;
                        int FilesCount = taxbillsdt.Rows.Count / FileMaxRecCount;

                        if (Reminder > 0 && Reminder < FileMaxRecCount)
                            FilesCount += 1;

                        StringBuilder[] fileContent = new StringBuilder[FilesCount];


                        int rowno = 0;
                        int RemRecCount = FileMaxRecCount;

                        for (int j = 0; j < FilesCount; j++)
                        {
                            fileContent[j] = new StringBuilder();
                            fileContent[j].Append(headerLine);

                            //Condition Modified by Vishal
                            //To Fix TFS - 7255
                            //if (j == (FilesCount - 1))
                            //    RemRecCount = Reminder;
                            if (j == (FilesCount - 1) && (Reminder != 0))
                                RemRecCount = Reminder;
                            //Modification End
                                                        
                            for (int i = 0; i < RemRecCount; i++)
                            {
                                fileContent[j].Append(System.Environment.NewLine);

                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["Addr1"]).PadRight(25)); //Address Line 1
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["Addr2"]).PadRight(25)); //Address Line 2
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["City"]).PadRight(35)); //City
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["State"]).PadRight(2)); //State
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["ZipCode"]).PadRight(10)); //ZipCode
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["Country"]).PadRight(15)); //Country
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["APN"]).PadRight(12)); //APN
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["APNSuffix"]).PadRight(4)); //APNSuffix
                                fileContent[j].Append("".PadRight(8)); //CMPRevisionDATE
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["NAX"]).PadRight(4)); //NAX
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["RUNDATE"]).PadRight(10)); //RUNDATE
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["PAIDBYDTE"]).PadRight(10)); //PAIDBYDTE
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["PENACCDTE"]).PadRight(10)); //PENACCDTE
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["ASMNTDTE"]).PadRight(10)); //ASMNTDTE
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["DLQDTE"]).PadRight(10)); //DLQDTE
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["ASENME"]).PadRight(40)); //ASENME
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L1"]).PadRight(91)); //L1
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L2"]).PadRight(91)); //L2
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L3"]).PadRight(91)); //L3

                                fileContent[j].Append("".PadRight(91)); //L4
                                //fileContent[j].Append(util.fixedNumber(taxBillDetails.Rows[rowno]["L5"].ToString(), 91, executionID)); //L5
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L5"]).PadRight(91)); //L5
                                fileContent[j].Append("".PadRight(91)); //L6

                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L7"]).PadRight(91)); //L7
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L8"]).PadRight(91)); //L8
                                fileContent[j].Append("".PadRight(91)); //L9
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L10"]).PadRight(91)); //L10
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L11"]).PadRight(91)); //L11

                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L12"]).PadRight(91)); //L12                                 
                                fileContent[j].Append("".PadRight(91)); //L13 FILLER

                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L14"]).PadRight(91)); //L14
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L15"]).PadRight(91)); //L15
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L16"]).PadRight(91)); //L16
                                fileContent[j].Append("".PadRight(91)); //L17
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L18"]).PadRight(91)); //L18
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L19"]).PadRight(91)); //L19
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L20"]).PadRight(91)); //L20

                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L21"]).PadRight(91)); //L21
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["L22"]).PadRight(91)); //L22
                                fileContent[j].Append("".PadRight(91)); //L23
                                fileContent[j].Append("".PadRight(91)); //L24
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["UNPLIT"]).PadRight(40)); //UNPLIT
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["PARCEL"]).PadRight(15)); //PARCEL
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["TAXYR"]).PadRight(4)); //TAXYR

                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["INSTAX"]).PadLeft(13)); //INSTAX
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["BASPEN"]).PadLeft(13)); //BASPEN
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["PENCST"]).PadLeft(6)); //PENCST
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["FEES"]).PadLeft(8)); //FEES
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["TOTDUE"]).PadLeft(13)); //TOTDUE
                                fileContent[j].Append("".PadRight(90)); //L25
                                fileContent[j].Append("".PadRight(90)); //L26

                                fileContent[j].Append("".PadRight(47)); //STBMSG1
                                fileContent[j].Append("".PadRight(47)); //STBMSG2
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["SCNLNE"]).PadRight(70)); //SCNLNE
                                fileContent[j].Append(Convert.ToString(taxBillDetails.Rows[rowno]["KEYLINE"]).PadRight(21)); //KEYLINE

                                rowno += 1;
                            }

                            fileContent[j].Append(System.Environment.NewLine);
                        }


                        //Upload All Interface File(s) into County FTP
                        string countyFTPServer = util.GetConfigValue("PTMS", "DataCenter_FTP", "ServerAddress", executionID);
                        string countyFTPUserName = util.GetConfigValue("PTMS", "DataCenter_FTP", "UserName", executionID);
                        string countyFTPPassword = util.GetConfigValue("PTMS", "DataCenter_FTP", "Password", executionID);


                        //Upload All Interface File(s) into Vendor FTP
                        string vendorFTPServer = util.GetConfigValue("PTMS", "Vendor_FTP", "ServerAddress", executionID);
                        string vendorFTPUserName = util.GetConfigValue("PTMS", "Vendor_FTP", "UserName", executionID);
                        string vendorFTPPassword = util.GetConfigValue("PTMS", "Vendor_FTP", "Password", executionID);

                        string directoryName = string.Empty;
                        directoryName = util.GetDirectoryName(executionID);
                        string path = "BillingAndNotices";
                        ArrayList lstCountyFTPLocation = new ArrayList();
                        lstCountyFTPLocation.Add(countyFTPServer);
                        lstCountyFTPLocation.Add(path);
                        lstCountyFTPLocation.Add("Reminder Notices");
                        lstCountyFTPLocation.Add(directoryName);
                        string exportLocationFolder = BatchUtility.ReturnExportFileLocation(lstCountyFTPLocation, countyFTPUserName, countyFTPPassword, executionID, 1);
                        //string exportLocationFolder = util.CreateDirectoryOnFTP(path, countyFTPServer, countyFTPUserName, countyFTPPassword, directoryName, executionID, 1);
                        //string vendorexportLocationFolder = util.CreateDirectoryOnFTP(path, vendorFTPServer, vendorFTPUserName, vendorFTPPassword, directoryName, executionID, 1);
                        if (string.IsNullOrEmpty(exportLocationFolder))
                        {
                            BatchUtility.UpdateExecutionEndDate(false, executionID);
                            BatchUtility.UpdatedBatchProcessingStatusCode(false, executionID);
                            Environment.Exit(1);
                        }
                        ArrayList lstFilePath=new ArrayList();
                        lstFilePath.Add(vendorFTPServer);
                        //lstFilePath.Add(path);
                        //lstFilePath.Add("Reminder Notices");
                        lstFilePath.Add(directoryName);
                        string vendorexportLocationFolder=BatchUtility.ReturnExportFileLocation(lstFilePath,vendorFTPUserName,vendorFTPPassword,executionID,1);
                        if (string.IsNullOrEmpty(vendorexportLocationFolder))
                        {
                            BatchUtility.UpdateExecutionEndDate(false, executionID);
                            BatchUtility.UpdatedBatchProcessingStatusCode(false, executionID);
                            Environment.Exit(1);
                        }
                        string fileName = string.Empty;
                        string countyFTPUpload = string.Empty;
                        string vendorFTPUpload = string.Empty;
                        string[] UploadedFileNames = new string[0];

                        for (int j = 0; j < FilesCount; j++)
                        {
                            string fileNo = (j + 1).ToString("000");
                            fileName = util.GetFileName(executionID, fileNo);
                            countyFTPUpload = util.UploadFile(countyFTPServer, countyFTPUserName, countyFTPPassword, fileName, exportLocationFolder, fileContent[j], 0, executionID);
                            if (countyFTPUpload == "1") // if county FTP upload is success
                            {
                                Array.Resize(ref UploadedFileNames, (UploadedFileNames.Length + 1));
                                UploadedFileNames[j] = fileName.ToString().Trim();

                                //upload to vendor FTP
                                vendorFTPUpload = util.UploadFile(vendorFTPServer, vendorFTPUserName, vendorFTPPassword, fileName, vendorexportLocationFolder, fileContent[j], 0, executionID);
                                if (vendorFTPUpload != "1") // if vendor FTP upload is success                                
                                {
                                    BatchUtility.LogMessage("Vendor FTP Upload Failed", executionID, "EXCEP");
                                    //Delete File(s) from County FTP Server 
                                    DeleteAllFiles(UploadedFileNames, executionID, exportLocationFolder, true);
                                    //Update Batch ControlM table with failure flag
                                    BatchUtility.UpdatedBatchProcessingStatusCode(false, executionID);
                                    BatchUtility.UpdateExecutionEndDate(false, executionID);
                                    
                                    Environment.Exit(1);
                                    return;
                                }
                            }
                            else
                            {
                                BatchUtility.LogMessage("County FTP Upload Failed", executionID, "EXCEP");
                                //Delete File(s) from County FTP Servers
                                DeleteAllFiles(UploadedFileNames, executionID, exportLocationFolder, false);
                                //Delete File(s) from Vendor FTP Servers
                                DeleteAllFiles(UploadedFileNames, executionID, exportLocationFolder, true);
                                //Update Batch ControlM table with failure flag
                                BatchUtility.UpdatedBatchProcessingStatusCode(false, executionID);
                                BatchUtility.UpdateExecutionEndDate(false, executionID);
                                
                                Environment.Exit(1);
                                return;
                            }
                        }

                        if (countyFTPUpload == "1" && vendorFTPUpload == "1")//County FTP and Vendor FTP Upload Success
                        {
                            //Update IsReminderNoticePrinted flag to ‘1’ in TXB.TaxBillInstallment table 
                            // Insert the Activity “First installment reminder notice printed” in CM.ActivityLog table for all Tax Bill ID’s
                            string output = string.Empty;
                            SqlDatabase dbu = (SqlDatabase)DatabaseFactory.CreateDatabase();
                            DbCommand cmdUpdateDetails = dbu.GetStoredProcCommand("USP_BAN_BATCH_UnpaidSupSecOrBothInstDue3");
                            dbu.AddInParameter(cmdUpdateDetails, "MYTABLEPARAM", SqlDbType.Structured, taxbillsdt);
                            dbu.AddInParameter(cmdUpdateDetails, "ExecutionID", DbType.Int32, executionID);

                            dbu.AddOutParameter(cmdUpdateDetails, "Result", DbType.String, 50);

                            dbu.ExecuteDataSet(cmdUpdateDetails);
                            output = Convert.ToString(dbu.GetParameterValue(cmdUpdateDetails, "Result"));

                            if (output == "Success")
                            {
                                BatchUtility.LogMessage("No. of records processed : " + taxBillDetails.Rows.Count.ToString(), executionID, "SUC");
                                BatchUtility.LogMessage("Interface Files Generated and Uploaded to County and Vendor FTP Successfully.", executionID, "SUC");

                                //Send Email to the vendor that the file has been uploaded
                                // If both the uploads are success then send the email to the vendor.
                                string subject = string.Empty;
                                string body = string.Empty;

                                string s = string.Empty;

                                if (DateTime.Now.Month >= 10)
                                    s = DateTime.Now.Month.ToString() + "/";
                                else
                                    s = "0" + DateTime.Now.Month.ToString() + "/";

                                if (DateTime.Now.Day >= 10)
                                    s = s + DateTime.Now.Day.ToString() + "/";
                                else
                                    s = s + "0" + DateTime.Now.Day.ToString() + "/";

                                s = s + DateTime.Now.Year.ToString();

                                subject = "Reminder notice data file(s) for supplemental 2nd or both installments is generated. Dated: " + s; 
                                body = "Dear User," + System.Environment.NewLine + System.Environment.NewLine;
                                body = body + "The batch: " + batchName + " has generated the interface file(s) and placed at the following locations:" + System.Environment.NewLine + System.Environment.NewLine;
                                body = body + "County FTP: " + countyFTPServer + path + "/" + directoryName + System.Environment.NewLine;
                                body = body + "Vendor FTP: " + vendorFTPServer + "/" + directoryName + System.Environment.NewLine + System.Environment.NewLine;
                                body = body + "Folder Name: " + directoryName + System.Environment.NewLine + System.Environment.NewLine;
                                body = body + "This is a system generated email. Please do not reply to it." + System.Environment.NewLine;
                                body = body + System.Environment.NewLine;
                                body = body + "Regards," + System.Environment.NewLine;
                                body = body + "PTMS";

                               string ToMailID = util.GetConfigValue("TTC", "TTCEmailIDForBAN", "emailID", executionID);
                                string emailDelimeter = util.GetConfigValue("TTC", "Email Delimiter", "Email Delimiter", executionID);
                                //ToMailID += util.GetConfigValue("TTC", "PrintingVendorEmailIDForBAN", "emailID", executionID);
                                string CCmailID = util.GetConfigValue("TTC", "PrintingVendorEmailIDForBAN", "emailID", executionID);
                                string fromEmailID = util.GetConfigValue("TTC", "TTCFromEmailIDForBAN", "emailID",executionID);
                                ArrayList lstToEmailID = new ArrayList();
                                lstToEmailID.Add(ToMailID);
                                lstToEmailID.Add(CCmailID);


                                int res = util.SendEmail(lstToEmailID, fromEmailID, subject, body, executionID);

                                if (res == 1)
                                {
                                    BatchUtility.LogMessage(batchName + " An email has been sent to the TTC department user and printing vendor regarding the data file(s) generation", executionID, "SUC");
                                    BatchUtility.LogMessage(batchName + " completed successfully.", executionID, "SUC");
                                    //Update Batch ControlM table with success flag
                                    BatchUtility.UpdatedBatchProcessingStatusCode(true, executionID);
                                    BatchUtility.UpdateExecutionEndDate(true, executionID);
                                    Environment.Exit(0);
                                    return;
                                }
                                else
                                {
                                    StringBuilder mailMessage = new StringBuilder(string.Empty);

                                    mailMessage.AppendLine("There was a problem with sending e-mail. Send email manually to the TTC department user and printing vendor with the following details:");
                                    mailMessage.AppendLine(string.Empty);
                                    mailMessage.AppendLine("------------");
                                    mailMessage.AppendLine("To email ID:");
                                    mailMessage.AppendLine("------------");
                                    mailMessage.AppendLine(ToMailID + emailDelimeter + CCmailID);
                                    mailMessage.AppendLine(string.Empty);
                                    mailMessage.AppendLine("------------");
                                    mailMessage.AppendLine("From email ID:");
                                    mailMessage.AppendLine("------------");
                                    mailMessage.AppendLine(fromEmailID);
                                    mailMessage.AppendLine(string.Empty);
                                    mailMessage.AppendLine("------------");
                                    mailMessage.AppendLine("Subject:");
                                    mailMessage.AppendLine("------------");
                                    mailMessage.AppendLine(subject);
                                    mailMessage.AppendLine(string.Empty);
                                    mailMessage.AppendLine("------------");
                                    mailMessage.AppendLine("Body of the email:");
                                    mailMessage.AppendLine("------------");
                                    mailMessage.AppendLine(body);

                                    BatchUtility.LogMessage(mailMessage.ToString(), executionID, "SUC");
                                    //BatchUtility.LogMessage("Error occured while sending email.", executionID, "EXCEP");
                                    //BatchUtility.LogMessage("Please send email to vendor manually by using the following body text : " + Environment.NewLine + body, executionID, "EXCEP");
                                    //Update Batch ControlM table with Success flag and set status remained as Approve
                                    BatchUtility.UpdatedBatchProcessingStatusCode(true, executionID);
                                    BatchUtility.UpdateExecutionEndDate(true, executionID);
                                    Environment.Exit(1);
                                    return;
                                }
                            }
                            else
                            {
                                BatchUtility.LogMessage("Error Occured while Updating records", executionID, "EXCEP");
                                //Delete File(s) from FTP Server
                                DeleteAllFiles(UploadedFileNames, executionID, exportLocationFolder, false);
                                //Delete File(s) from vendor FTP Server 
                                DeleteAllFiles(UploadedFileNames, executionID, vendorexportLocationFolder, true);
                                //Update Batch ControlM table with failure flag
                                BatchUtility.UpdatedBatchProcessingStatusCode(false, executionID);
                                BatchUtility.UpdateExecutionEndDate(false, executionID);
                                Environment.Exit(1);
                                return;
                            }
                        }
                        else //Failed
                        {
                            BatchUtility.LogMessage("Upload to County Data Center FTP Failed", executionID, "EXCEP");
                            //Delete File(s) from FTP Server
                            DeleteAllFiles(UploadedFileNames, executionID, exportLocationFolder, false);
                            //Delete File(s) from vendor FTP Server 
                            DeleteAllFiles(UploadedFileNames, executionID, vendorexportLocationFolder, true);
                            //Update Batch ControlM table with failure flag
                            BatchUtility.UpdatedBatchProcessingStatusCode(false, executionID);
                            BatchUtility.UpdateExecutionEndDate(false, executionID);
                            Environment.Exit(1);
                            return;
                        }
                    }
                }
            }
            catch (Exception ep)
            {
                BatchUtility.UpdatedBatchProcessingStatusCode(false, executionID);
                BatchUtility.UpdateExecutionEndDate(false, executionID);
                BatchUtility.LogMessage(ep.Message, executionID, "EXCEP");
                Environment.Exit(1);
                return;
            }
        }

        #region Delete All Files
        /// <summary>
        /// Delete All Files
        /// </summary>
        /// <param name="UploadedFileNames"></param>
        /// <param name="executionID"></param>
        /// <param name="exportLocationFolder"></param>
        /// <param name="IsVendorFTP"></param>
        static void DeleteAllFiles(string[] UploadedFileNames, int executionID, string exportLocationFolder, bool IsVendorFTP)
        {
            try
            {
                if (UploadedFileNames.Count() > 0)
                {
                    string FTPServer, FTPUserName, FTPPassword;
                    Utility Util = new Utility();
                    FTPServer = Util.GetConfigValue("PTMS", "DataCenter_FTP", "ServerAddress", executionID);
                    FTPUserName = Util.GetConfigValue("PTMS", "DataCenter_FTP", "UserName", executionID);
                    FTPPassword = Util.GetConfigValue("PTMS", "DataCenter_FTP", "Password", executionID);

                    if (IsVendorFTP == true)
                    {
                        FTPServer = Util.GetConfigValue("PTMS", "Vendor_FTP", "ServerAddress", executionID);
                        FTPUserName = Util.GetConfigValue("PTMS", "Vendor_FTP", "UserName", executionID);
                        FTPPassword = Util.GetConfigValue("PTMS", "Vendor_FTP", "Password", executionID);
                    }

                    for (int j = 0; j < UploadedFileNames.Count(); j++)
                    {
                        string RetDelMsg = Util.DeleteFromFTP(FTPServer, FTPUserName, FTPPassword, exportLocationFolder + UploadedFileNames[j], executionID, 1);
                        if (RetDelMsg != "1")
                            BatchUtility.LogMessage("Deleting Files(s) From FTP Location Got Failed", executionID, "EXCEP");
                    }
                }
            }
            catch (Exception ep)
            {
                BatchUtility.LogMessage(ep.Message, executionID, "EXCEP");
            }
        }
        #endregion

    }



    class Utility
    {
        #region Fixed String With Specified Length to handle Proper length
        /// <summary>
        /// Method to handle Blank Character string Type
        /// </summary>
        /// <param name="field"></param>
        /// <param name="length"></param>
        /// <param name="executionID"></param>
        /// <returns></returns>
        public string fixedString(string field, int length, int executionID)
        {
            string fixedstr = string.Empty;
            try
            {
                int stringlength = field.Length;
                if (stringlength != length)
                {
                    if (stringlength < length)
                    {
                        string spaces = "";
                        for (int i = 1; i <= (length - stringlength); i++)
                        {
                            spaces += " ";
                        }
                        fixedstr = field + spaces;
                    }
                    else
                    {
                        fixedstr = field.Substring(0, length);
                    }
                }
                else
                    fixedstr = field;

            }
            catch (Exception e)
            {
                BatchUtility.LogMessage(e.Message, executionID, "EXCEP");
            }
            return fixedstr;
        }

        #endregion

        #region Update Batch Status
        /// <summary>
        /// update Batch Status
        /// </summary>
        /// <param name="ExecutionID"></param>
        /// <param name="Type"></param>
        public void UpdateBatchStatus(int ExecutionID, string Type)
        {
            try
            {
                SqlDatabase dba = (SqlDatabase)DatabaseFactory.CreateDatabase();
                DbCommand cmdUpdateBatchStatus = dba.GetStoredProcCommand("USP_BAN_UpdateBatchStatus");
                dba.AddInParameter(cmdUpdateBatchStatus, "ExecutionID", DbType.Int32, ExecutionID);
                dba.AddInParameter(cmdUpdateBatchStatus, "Type", DbType.String, "S");
                dba.AddInParameter(cmdUpdateBatchStatus, "BatchCode", DbType.String, ConfigurationSettings.AppSettings["BatchCode"]);
                dba.ExecuteDataSet(cmdUpdateBatchStatus);
            }
            catch (Exception e)
            {
                BatchUtility.LogMessage(e.Message + System.DateTime.Now.ToString(), ExecutionID, "EXEC");
            }
        }

        #endregion

        #region Fixed Number String to handle blank No Type for Proper Length HAndeling
        /// <summary>
        /// Fixed Number String to handle blank No Type for Proper Length HAndeling
        /// </summary>
        /// <param name="field"></param>
        /// <param name="length"></param>
        /// <param name="executionID"></param>
        /// <returns></returns>
        public string fixedNumber(string field, int length, int executionID)
        {
            string fixedNum = string.Empty;
            field = field.Replace(".", "");
            try
            {
                int numberlength = field.Length;
                if (numberlength != length)
                {
                    if (numberlength < length)
                    {
                        string appendZeros = "";
                        for (int i = 1; i <= (length - numberlength); i++)
                        {
                            appendZeros += "0";
                        }
                        fixedNum = appendZeros + field;
                    }
                    else
                    {
                        fixedNum = field.Substring(0, length);
                    }
                }
                else
                    fixedNum = field;
            }
            catch (Exception e)
            {
                BatchUtility.LogMessage(e.Message, executionID, "EXCEP");
            }
            return fixedNum;
        }

        #endregion

        #region Delete Batch OutPut
        /// <summary>
        /// Delete Batch Output
        /// </summary>
        /// <param name="ExecutionID"></param>
        public void DeleteBatchOutput(int ExecutionID)
        {
            try
            {
                string output = string.Empty;
                SqlDatabase dbDeleteDetails = (SqlDatabase)DatabaseFactory.CreateDatabase();
                DbCommand cmdDeleteDetails = dbDeleteDetails.GetStoredProcCommand("usp_BAN_Batch_DeleteBatchOutputSUPPL");
                dbDeleteDetails.AddInParameter(cmdDeleteDetails, "ExecutionID", DbType.Int32, ExecutionID);
                dbDeleteDetails.AddOutParameter(cmdDeleteDetails, "Result", DbType.String, 50);

                dbDeleteDetails.ExecuteDataSet(cmdDeleteDetails);
                output = Convert.ToString(dbDeleteDetails.GetParameterValue(cmdDeleteDetails, "Result"));

                if (output == "Success")
                    BatchUtility.LogMessage("Deleted Records From Batch Results & Exceptions tables", ExecutionID, "EXCEP");
                else
                    BatchUtility.LogMessage("Error occured while deleting records From Batch Results & Exceptions tables", ExecutionID, "EXCEP");
            }
            catch (Exception e)
            {
                BatchUtility.LogMessage(e.Message, ExecutionID, "EXCEP");

            }
        }

        #endregion

        #region Function To Send Email
        /// <summary>
        /// Send Email By Using Net.Mail
        /// </summary>
        /// <param name="mainMailID"></param>
        /// <param name="CCmailID"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="executionID"></param>
        /// <returns></returns>
        public int SendEmail(ArrayList lstToMailID, string configvalueMailFrom, string subject, string body, int executionID)
        {
            int SentEmail = 0;
            try
            {
                if (configvalueMailFrom != "" && lstToMailID.Count > 0)
                {
                    //MailMessage mail = new MailMessage();
                    //mail.To = mainMailID;
                    //mail.Cc = CCmailID;
                    //string configvalueMailFrom = GetConfigValue("PTMS", "TTCFromMailID", "TTCFromMailID", executionID);
                    //mail.From = configvalueMailFrom;
                    //mail.Subject = subject;
                    //mail.Body = body;

                    //string configvalueSmtpServer = GetConfigValue("PTMS", "SMTPServer", "Address", executionID);
                    //SmtpMail.SmtpServer = configvalueSmtpServer;
                    //SmtpMail.Send(mail);
                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                    for (int i = 0; i < lstToMailID.Count; i++)
                    {
                        mail.To.Add(lstToMailID[i].ToString());
                    }
                    //mail.To.Add(mainMailID);
                    //mail.CC.Add(CCmailID);
                    //string configvalueMailFrom = GetConfigValue("PTMS", "TTCFromMailID", "TTCFromMailID", executionID);
                    mail.From = new MailAddress(configvalueMailFrom);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.Priority = System.Net.Mail.MailPriority.Normal;
                    SmtpClient mSmtpClient = new SmtpClient();
                    string configvalueSmtpServer = GetConfigValue("PTMS", "SMTPServer", "Address", executionID);
                    mSmtpClient.Host = configvalueSmtpServer;
                    mSmtpClient.Send(mail);
                    SentEmail = 1;

                    SentEmail = 1;
                }
                else
                {
                    BatchUtility.LogMessage("Check your FromEmailID or To EmailID list ", executionID, "INFO");
                }
            }
            catch (Exception ep)
            {
                BatchUtility.LogMessage(ep.Message, executionID, "INFO");
            }
            return SentEmail;
        }
        
        #endregion

        #region Get Config Values
        /// <summary>
        /// Get Configurational Values
        /// </summary>
        /// <param name="department"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="executionID"></param>
        /// <returns></returns>
        public string GetConfigValue(string department, string type, string name, int executionID)
        {
            DataSet configvalue = null;
            string Value = string.Empty;
            try
            {
                SqlDatabase database = (SqlDatabase)DatabaseFactory.CreateDatabase();
                DbCommand command = database.GetStoredProcCommand("dbo.usp_CM_GetConfigValue");
                database.AddInParameter(command, "Department", DbType.String, department);
                database.AddInParameter(command, "Type", DbType.String, type);
                database.AddInParameter(command, "Name", DbType.String, name);
                configvalue = database.ExecuteDataSet(command);
                Value = configvalue.Tables[0].Rows[0]["Value"].ToString();
            }
            catch (Exception e)
            {
                BatchUtility.LogMessage(e.Message, executionID, "EXCEP");
            }
            return Value;
        }
        
        #endregion

        #region Get Proper Directory Name
        /// <summary>
        /// Function to Retrive Proper Directory Name in a Specified Format
        /// </summary>
        /// <param name="executionID"></param>
        /// <returns></returns>
        public string GetDirectoryName(int executionID)
        {
            string DName = string.Empty;
            try
            {
                string s = string.Empty;

                if (DateTime.Now.Month >= 10)
                    s = DateTime.Now.Month.ToString();
                else
                    s = "0" + DateTime.Now.Month.ToString();

                if (DateTime.Now.Day >= 10)
                    s = s + DateTime.Now.Day.ToString();
                else
                    s = s + "0" + DateTime.Now.Day.ToString();

                s = s + DateTime.Now.Year.ToString() + "_";

                if (DateTime.Now.Hour >= 10)
                    s = s + DateTime.Now.Hour.ToString();
                else
                    s = s + "0" + DateTime.Now.Hour.ToString();

                if (DateTime.Now.Minute >= 10)
                    s = s + DateTime.Now.Minute.ToString();
                else
                    s = s + "0" + DateTime.Now.Minute.ToString();
                

                DName = executionID.ToString() + "_Supplemental_SecondOrBoth_" + s;
            }
            catch (Exception e)
            {
                BatchUtility.LogMessage(e.Message, executionID, "EXCEP");

            } return DName;
        }

        #endregion

        #region Get Proper File Name to Upload File on FTP

        public string GetFileName(int executionID, string fileNo)
        {
            string FName = string.Empty;
            try
            {
                string s = string.Empty;

                if (DateTime.Now.Month >= 10)
                    s = DateTime.Now.Month.ToString();
                else
                    s = "0" + DateTime.Now.Month.ToString();

                if (DateTime.Now.Day >= 10)
                    s = s + DateTime.Now.Day.ToString();
                else
                    s = s + "0" + DateTime.Now.Day.ToString();

                s = s + DateTime.Now.Year.ToString() + "_";

                if (DateTime.Now.Hour >= 10)
                    s = s + DateTime.Now.Hour.ToString();
                else
                    s = s + "0" + DateTime.Now.Hour.ToString();

                if (DateTime.Now.Minute >= 10)
                    s = s + DateTime.Now.Minute.ToString();
                else
                    s = s + "0" + DateTime.Now.Minute.ToString();

                //string s = DateTime.Now.Month.ToString("00") + "/" + DateTime.Now.Day.ToString("00") + "/" + DateTime.Now.Year.ToString("0000");
                //string HH = DateTime.Now.Hour.ToString();
                //string MM = DateTime.Now.Minute.ToString();
                //string executionDatetime = s.Replace("/", "") + "_" + HH + MM;
                FName = "OCT004" + "." + ConfigurationSettings.AppSettings["BatchCode"].ToString() + "." + executionID.ToString() + "." + s + "." + fileNo;
            }
            catch (Exception e)
            {
                BatchUtility.LogMessage(e.Message, executionID, "EXCEP");

            } return FName;
        }

        #endregion

        #region Method to Create Directory on FTP
        /// <summary>
        /// Function to create Directory on FTP
        /// </summary>
        /// <param name="path"></param>
        /// <param name="inFTPServerAndPath"></param>
        /// <param name="inUsername"></param>
        /// <param name="inPassword"></param>
        /// <param name="inNewDirectory"></param>
        /// <param name="executionID"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public string CreateDirectoryOnFTP(string path, String inFTPServerAndPath, String inUsername, String inPassword, String inNewDirectory, int executionID, int Count)
        {
            string exportLocationPath = string.Empty;
            FtpWebResponse makeDirectoryResponse = null;
            try
            {
                // Step 1 - Open a request using the full URI, ftp://ftp.server.tld/path/file.ext
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(inFTPServerAndPath + path + "/" + inNewDirectory);

                // Step 2 - Configure the connection request
                request.Credentials = new NetworkCredential(inUsername, inPassword);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;
                string fullPath = string.Empty;

                bool IsThere = FtpDirectoryExists(inFTPServerAndPath + path + "/" + inNewDirectory, inUsername, inPassword);
                if (IsThere == false)
                {
                    request = (FtpWebRequest)FtpWebRequest.Create(inFTPServerAndPath + path + "/" + inNewDirectory);
                    request.Credentials = new NetworkCredential(inUsername, inPassword);
                    request.UsePassive = true;
                    request.UseBinary = true;
                    request.KeepAlive = false;
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;

                    request.Proxy = null;
                    // Step 3 - Call GetResponse() method to actually attempt to create the directory
                     makeDirectoryResponse = (FtpWebResponse)request.GetResponse();
                }
                exportLocationPath = path + "/" + inNewDirectory + "/";
            }
            catch (Exception e)
            {
                makeDirectoryResponse.Close();
                if (Count <= 5)
                    CreateDirectoryOnFTP(path, inFTPServerAndPath, inUsername, inPassword, inNewDirectory, executionID, ++Count);
                else
                    BatchUtility.LogMessage(e.Message, executionID, "EXCEP");

            }

            return exportLocationPath;
        }

        #endregion

        #region Functions to Check Directory Exists OR NOT
        /// <summary>
        /// Method to check Directory Existance
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="ftpUser"></param>
        /// <param name="ftpPassword"></param>
        /// <returns></returns>
        public bool FtpDirectoryExists(string directoryPath, string ftpUser, string ftpPassword)
        {
            FtpWebResponse response=null;
            FtpWebRequest request = null;
            bool IsExists = true;
            try
            {
                request = (FtpWebRequest)WebRequest.Create(directoryPath);
                request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                request.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;
                response = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                request = null;
                response = null;
                IsExists = false;
                
            }
            return IsExists;
        }

        #endregion

        #region Method to Upload File ON FTP Location
        /// <summary>
        /// Functions to Upload Text file on FTP Location
        /// </summary>
        /// <param name="exportLocation"></param>
        /// <param name="FTPUserName"></param>
        /// <param name="FTPPassword"></param>
        /// <param name="fileName"></param>
        /// <param name="ExportLocationFolder"></param>
        /// <param name="fileContent"></param>
        /// <param name="Count"></param>
        /// <param name="executionID"></param>
        /// <returns></returns>
        public string UploadFile(string exportLocation, string FTPUserName, string FTPPassword, string fileName, string ExportLocationFolder, StringBuilder fileContent, int Count, int executionID)
        {
            string Msg = "1";
            try
            {
                string exportLocation1 = exportLocation + ExportLocationFolder;
                Encoding enc = Encoding.ASCII;
                byte[] S = enc.GetBytes(fileContent.ToString());
                FtpWebRequest FTPWebReq = (FtpWebRequest)WebRequest.Create(exportLocation1 + fileName + ".txt");
                FTPWebReq.Method = WebRequestMethods.Ftp.UploadFile;
                FTPWebReq.Credentials = new NetworkCredential(FTPUserName, FTPPassword);
                Stream ftpStream = FTPWebReq.GetRequestStream();
                ftpStream.Write(S, 0, S.Length);
                ftpStream.Close();
                FTPWebReq = null;
            }
            catch (Exception ex)
            {
                if (Count <= 5)//If the file was not uploaded in the first go try for 5 more times
                {
                    Msg = UploadFile(exportLocation, FTPUserName, FTPPassword, fileName, ExportLocationFolder, fileContent, ++Count, executionID);
                }
                else
                {
                    Msg = "0";
                    BatchUtility.LogMessage(ex.Message, executionID, "EXCEP");
                }
            }
            return Msg;
        }

        #endregion

        #region DELETE FILE FROM FTP When Any Exception occurs
        /// <summary>
        /// Function To Remove File FROM FTP
        /// </summary>
        /// <param name="ftpServerIP"></param>
        /// <param name="FTPUserName"></param>
        /// <param name="FTPPassword"></param>
        /// <param name="DestWhereFileWillBe"></param>
        /// <param name="executionID"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public string DeleteFromFTP(string ftpServerIP, string FTPUserName, string FTPPassword, string DestWhereFileWillBe, int executionID, int Count)
        {
            string msg = "1";
            try
            {
                FtpWebRequest FTPWebDelReq = (FtpWebRequest)WebRequest.Create(ftpServerIP + DestWhereFileWillBe + ".txt");
                FTPWebDelReq.Credentials = new NetworkCredential(FTPUserName, FTPPassword);
                FTPWebDelReq.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response = (FtpWebResponse)FTPWebDelReq.GetResponse();
                response.Close();
                FTPWebDelReq = null;
            }
            catch (Exception Ex)
            {
                if (Count <= 5)//If the file was not uploaded in the first go try for 5 more times
                {
                    msg = DeleteFromFTP(ftpServerIP, FTPUserName, FTPPassword, DestWhereFileWillBe, executionID, ++Count);
                }
                else
                {
                    msg = "0";
                    BatchUtility.LogMessage(Ex.Message, executionID, "EXCEP");
                }
            }
            return msg;
        }

        #endregion

        public string AssessmentValFormat(string Val, int length)
        {
            string retval = string.Empty;

            if (Val != null && Val != "")
                retval = String.Format("{0:0,0}", Convert.ToDecimal(Val)).PadLeft(12);
            else
                retval = "".PadRight(12);

            return retval;
        }

        #region Method to Enter Processing Result Data Explicitely
        /// <summary>
        /// Method to Enter Processing Result Data Explicitely
        /// </summary>
        /// <param name="executionID"></param>
        /// <param name="dataProcessedType"></param>

        public void LogNoDataProcessed(int executionID, int dataProcessedType)
        {

            try
            {
                string output = string.Empty;
                SqlDatabase dbu = (SqlDatabase)DatabaseFactory.CreateDatabase();
                DbCommand cmdUpdateDetails2 = dbu.GetStoredProcCommand("usp_BAN_Batch_UpdateBatchRSNoDataProcessed");
                dbu.AddInParameter(cmdUpdateDetails2, "ExecutionID", DbType.Int32, executionID);
                dbu.AddInParameter(cmdUpdateDetails2, "dataProcessedType", DbType.Int32, dataProcessedType);
                dbu.AddOutParameter(cmdUpdateDetails2, "Result", DbType.String, 50);
                dbu.ExecuteDataSet(cmdUpdateDetails2);
                output = Convert.ToString(dbu.GetParameterValue(cmdUpdateDetails2, "Result"));

                if (output == "Success")
                    BatchUtility.LogMessage("Records Successfully Updated On Batch Processing Result with no Data Processed", executionID, "EXCEP");
                else
                    BatchUtility.LogMessage("Error occured while Updating records in Batch Results for no Processing of Data", executionID, "EXCEP");
            }
            catch (Exception e)
            {
                BatchUtility.LogMessage(e.Message, executionID, "EXCEP");

            }

        }

        #endregion
    }

}

