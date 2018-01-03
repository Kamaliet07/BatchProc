using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlFileGenerator.Model
{
    public class Case
    {

        public Case()
        { 
        
        }

        private Int64 id;
        private string title;
        private string firstname;
        private string lastname;
        private string nino;
        private string sortcode;
        private DateTime dob;
        private string accnumber;
        private string gender;
        private string employeeid;
        private string gwpid;
        private string urn;

        public Int64 Id
        {
            get { return id; }
            set { id = value; }        
        }

        public string Title
        {
            get { return title; }        
        }

        public string FirstName
        {
            get { return firstname; }
        }

        public string LastName
        {
            get { return lastname; }
        }

        public string Nino
        {
            get { return nino; }
        }

        public string Gender
        {
            get { return gender; }
        }

        public DateTime DOB
        {
            get { return dob; }
        }

        public string SortCode
        {
            get { return sortcode; }
        }

        public string AccNumber
        {
            get { return accnumber; }
        }

        public string EmployeeID
        {
            get { return employeeid; }
        }

        public string GWPID
        {
            get { return gwpid; }
        }

        public string URN
        {
            get { return urn; }
        }

        public Case(int uniqueid, string newtitle, string newfirstname, string newlastname, string newgender, DateTime newdob, string newnino, string newsortcode, string newaccnumber, string newemployeeid, string newgwpid, string urn )
        {
            this.id = uniqueid;
            this.title = newtitle;
            this.firstname = newfirstname;
            this.lastname = newlastname;
            this.gender = newgender;
            this.dob= newdob;
            this.nino= newnino;
            this.sortcode = newsortcode;
            this.accnumber = newaccnumber;
            this.employeeid = newemployeeid;
            this.gwpid = newgwpid;
            this.urn = urn;        
        }
    }
}
