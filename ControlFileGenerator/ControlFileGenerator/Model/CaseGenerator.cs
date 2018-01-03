using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlFileGenerator.Model
{
   public class CaseGenerator
    {
       // 200 boys name
       private readonly string[] Firstnames = { "Kyle", "Irvin", "Guy", "Claude", "Rone", "Maurice", "Gustav"
                                              
                                              
                                              
                                              
                                              };

       // 200 boys name
       private readonly string[] GirlasNames = { "Irene", "Wanda", "Marie", "Sarah", "Frances", "Kelly", "Margaret", "Heather"
                                              
                                              
                                              
                                              
                                              };

       // 200 boys name
       private readonly string[] LastNames = { "Hudson", "Tyler", "Jenkins", "Wong", "Morales", "Abbott", "Tate", "Bowman"
                                              
                                              
                                              
                                              
                                              };

       
       private readonly string[] Gender =     { "Male", "Female" };

       
       private readonly string[] Femaletitles = { "Mrs", "Miss", "Ms"};

       
       private readonly string[] NinoPrefixes = { "AA", "AB", "AE", "AH", "AK", "AL", "AM", "AP", "AR", "AS", "AT", "AW", "AX", "AY", "AZ",
                                                  "BA", "BB", "BE", "BH", "BK", "BL", "BM", "BT", "CA", "CB", "CE", "CH", "CK", "CL", "CR",
                                                  "EA", "EB", "EE", "EH", "EK", "EL", "EM", "EP", "ER", "ES", "ET", "EW", "EX", "EY", "EZ", "GY",
                                                  "HA", "HB", "HE", "HH", "HK", "HL", "HM", "HP", "HR", "HS", "HT", "HW", "HX", "HY", "HZ", 
                                                  "JA", "JB", "JE", "JH", "JK", "JL", "JM", "JP", "JR", "JS", "JT", "JW", "JX", "JY", "jz"                                                                                             
                                              
                                              };

       private readonly string[] NinoSuffixes = { "A", "B", "C", "D" };




       public Case CreateCase(int uniqueId)
       {
           string accnumber = string.Empty;
           string gender = string.Empty;
           string title = string.Empty;
           string firstname = string.Empty;
           string lastname = string.Empty;
           string nino = string.Empty;
           string sortcode = string.Empty;
           string employeeid = string.Empty;
           string gwpid = string.Empty;

           DateTime dob;
           Random rand = new Random(uniqueId);
           int rnd = rand.Next(900000000) + 100000000;
           accnumber = rnd.ToString();
           if ((rnd % 2) == 0)
           {
               gender = "Male";
               title = "Mr";
               firstname = Firstnames[rnd % 197];
           }
           else
           {
               gender = "Female";
               title = Femaletitles[rnd % 3];
               firstname = GirlasNames[rnd % 197];           
           }

           lastname = LastNames[rnd % 3];
           sortcode = (rnd % 988313 + 100000).ToString();
           nino = string.Concat(NinoPrefixes[rnd % 239], (rnd % 899999 + 100000).ToString(), NinoSuffixes[rnd % 4]).ToUpper();
           dob = new DateTime((rnd % DateTime.MaxValue.Ticks) * 20000000).AddYears(1930).Date;

           string strPadZerp = rnd.ToString();
           char pad = '0';

           string strPadEight = string.Empty;
           char FirstDigitPad = '8';
           strPadEight = strPadZerp.PadLeft(11, pad);
           gwpid = strPadEight.PadLeft(12, FirstDigitPad);

           var RandomURNLeftResult = rand.Next(98111, 99999).ToString();
           Random RandomRightURN = new Random();
           var RandomURNRightResult = rand.Next(0111, 0999).ToString();
           string uRNNumber = "CISA00" + RandomURNLeftResult.ToString() + "0000" + RandomURNRightResult;

           Case ocase = new Case(uniqueId, title, firstname, lastname, gender, dob, nino, sortcode, accnumber, employeeid, gwpid, uRNNumber);
           return ocase;
       
       }
    }
}
