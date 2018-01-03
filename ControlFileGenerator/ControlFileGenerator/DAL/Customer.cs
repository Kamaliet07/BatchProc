using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Wpf_Customer.DAL
{
    class Customer
    {
        private int _count = 0;
        public int COUNT
        {
            get { return _count; }
            set { _count = value; }
        }

        private DateTime _TimeStamp = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        public DateTime TimeStamp
        {
            get { return _TimeStamp; }
            set { _TimeStamp = value; }
        }

        private string _fname = string.Empty;
        public string FNAME
        {
            get { return _fname; }
            set { _fname = value; }
        }

        private string _lname = string.Empty;
        public string LNAME
        {
            get { return _lname; }
            set { _lname = value; }
        }

        private string _accno = string.Empty;
        public string ACCNO
        {
            get { return _accno; }
            set { _accno = value; }
        }

        private DateTime _dob = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        public DateTime DOB
        {
            get { return _dob; }
            set { _dob = value; }
        }

        private string _postcd = string.Empty;
        public string POSTCD
        {
            get { return _postcd; }
            set { _postcd = value; }
        }

        private string _nino = string.Empty;
        public string NINO
        {
            get { return _nino; }
            set { _nino = value; }
        }        
    }

    // The DAL will read the information from the XML file and load into a List
    class DAL_OCUSMA
    {
        public static List<Customer> LoadOCUSMA()
        {
            List<Customer> ListCustomerRecords = new List<Customer>();
            // Execute the query using the LINQ to XML
            var custs = from c in XElement.Load("B:/Kamal_Document/Control File Generator/ControlFileGenerator/ControlFileGenerator/XML/AccountHolder.xml").Elements("row") 
                        select c;            
            foreach (var customer in custs)
            {
                Customer lCustomer = new Customer
                {
                    FNAME = customer.Element("OFNAME").Value,
                    LNAME = customer.Element("OLNAME").Value,
                    ACCNO = customer.Element("OACCNO").Value,
                    POSTCD = customer.Element("OPOSTCD").Value,
                    DOB = Convert.ToDateTime(customer.Element("ODOB").Value),
                    NINO = customer.Element("ONINO").Value,
                    
                };                
                ListCustomerRecords.Add(lCustomer);
            }
            return ListCustomerRecords;
        }
    }
}
