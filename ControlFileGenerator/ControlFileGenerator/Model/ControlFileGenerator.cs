using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace ControlFileGenerator.Model
{
   public class ControlFileGenerator
    {
       /// <summary>
       /// 
       /// </summary>
       internal static byte[] imageFile;
       
       /// <summary>
       /// ControlFileGenerator file genertor constructor
       /// </summary>
       public ControlFileGenerator()
       {
           if (imageFile == null)
           {
               Init();
           }
       
       }

       /// <summary>
       /// 
       /// </summary>
       private void Init()
       {
           string filePath = GlobalApplication.getResourcePath("XSLT") + "\\DummyImage.TIF";
           FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
           imageFile = new byte[System.Convert.ToInt32(fileStream.Length)];
           fileStream.Read(imageFile, 0, System.Convert.ToInt32(fileStream.Length));
       }

       /// <summary>
       /// Generate a Quantum Control file from the passed 
       /// </summary>
       /// <param name="seedValue"></param>
       /// <param name="noofCases"></param>
       /// <param name="dropLocation"></param>
       /// <param name="caseType"></param>
       /// <param name="AccountType"></param>
       /// <param name="TransferType"></param>
       /// <returns></returns>
       public bool GenerateControlFile(int seedValue, int noofCases, string dropLocation, string caseType, string AccountType, string TransferType)
       { 
           // Set up the date time
           DateTime dT = DateTime.Now;
           int CaseCounter = 1;

           // Choose a file location
           string location = dropLocation + @"\";

           // Create a GUID that is reused for t he whole case
           string caseGuid = System.Guid.NewGuid().ToString();

           // Build output file name
           string outputFileName = string.Format(location+ ConfigurationManager.AppSettings["OutPutControlFileName"], caseGuid);

           int imageCounter = 1;

           // write out the image required
           for (int i = 1; i <= noofCases * 2; i++)
           { 
               // Build the file Name
               string imageFileName = string.Format(location + ConfigurationManager.AppSettings["OutPutImageFileName"], caseGuid);

               // Write the image output
               FileStream output = new FileStream(imageFileName, FileMode.Create, FileAccess.Write);
               output.Write(imageFile, 0, imageFile.Length);
               output.Close();
           }

           StringBuilder sb;
           CaseGenerator casGen = new CaseGenerator();
           // Define the output
           TextWriter writer = new StreamWriter(outputFileName, false);
           for (int i = 0; i < noofCases; i++)
           {
               sb = new StringBuilder();
               Case oCase = casGen.CreateCase(i + seedValue);

               // Create Logic for Length
               sb.Append("~^Folder~Document Folder");
               sb.Append(Environment.NewLine);
               sb.Append("Attribute~PROCESSNAME~STR~").Append(GlobalApplication.ProcessName);
               sb.Append(Environment.NewLine);
               sb.Append("Attribute~DATESCANNED~STR~").Append(dT.Date.ToShortDateString());
               sb.Append(Environment.NewLine);
               sb.Append("Attribute~TIMESCANNED~STR~").Append(dT.Hour + ":" + dT.Minute + ":" + dT.Second);
               sb.Append(Environment.NewLine);
               sb.Append("Attribute~BATCHNO~STR~").Append("CaseGen");
               sb.Append(Environment.NewLine);
               sb.Append("Attribute~POSTPROCESS~STR~").Append("N");
               sb.Append(Environment.NewLine);
               sb.Append(Environment.NewLine);

               ////************************************ Transfer Application Entries ********************************

               sb.Append("Attribute~CASESEQUENCENO~STR~").Append(CaseCounter);
               sb.Append(Environment.NewLine);
               if (TransferType == "CASH ISA")
               {
                   sb.Append("ImageDoc~SF215 - Transfer Application Form");
               }
               else
               {
                   sb.Append("ImageDoc~SF279 - Transfer Application Form");
               }

               sb.Append(Environment.NewLine);
               sb.Append(Environment.NewLine);

               writer.Write(sb.ToString());           
           }

           writer.Close();
           writer = null;
           sb = null;
           return true;
       }

    }
}
