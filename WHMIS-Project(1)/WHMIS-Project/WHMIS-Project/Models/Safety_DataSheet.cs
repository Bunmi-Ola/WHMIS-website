using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WHMIS_Project.Models
{
    public class Safety_DataSheet
    {
        private int id;
        private string chemicalName;
        private string fileName;
        private HttpPostedFile fileDocument;
        private string cas;
        private DateTime LastRevisionDate;
        private bool isExpired;
        private bool isEditable;

        public List<Safety_DataSheet> Items { get; set; }

        public Safety_DataSheet() { }

        public Safety_DataSheet(int Id, string ChemicalName, string FileName, HttpPostedFile FileDocument)
        {
            id = Id;
            chemicalName = ChemicalName;
            fileName = FileName;
            fileDocument = FileDocument;
        }
        public Safety_DataSheet(int Id, string ChemicalName, string FileName, DateTime lastRevisionDate, string CAS)
        {
            id = Id;
            chemicalName = ChemicalName;
            fileName = FileName;
            cas = CAS;
            LastRevisionDate = lastRevisionDate;
            isExpired = IsExpired(lastRevisionDate);
        }


        public Safety_DataSheet(int Id, string ChemicalName, string FileName, string CAS, bool IsEditable)
        {
            id = Id;
            chemicalName = ChemicalName;
            fileName = FileName;
            cas = CAS;
            isEditable = IsEditable;
        }
        public Safety_DataSheet(string ChemicalName, string FileName, HttpPostedFile FileDocument, string CAS)
        {
            chemicalName = ChemicalName;
            fileName = FileName;
            fileDocument = FileDocument;
            cas = CAS;
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [Required(ErrorMessage = "Please enter the product name")]
        public string ChemicalName
        {
            get { return chemicalName; }
            set { chemicalName = value; }
        }

        [Required]
        public string FileDocName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        [Required]
        public HttpPostedFile FileDocument
        {
            get { return fileDocument; }
            set { fileDocument = value; }
        }

        public string CAS
        {
            get { return cas; }
            set { cas = value; }
        }

        [Required]     
        public DateTime lastRevisionDate
        {
            get { return LastRevisionDate; }
            set { LastRevisionDate = value; }
        }

        public string getlastRevisionDate
        {
            get { return lastRevisionDate.ToShortDateString(); }
           
        }

        public bool IsExpired (DateTime LastRevisionDate)
        {
           
            bool isExpired = false;

            double years = ((Convert.ToDateTime(DateTime.Now) - Convert.ToDateTime(LastRevisionDate)).TotalDays) / 365;

                if (years > 3)
                {
                isExpired = true;
                }

            return isExpired;
        }

        public bool Expired
        {
            get { return isExpired; }
        }
        public bool IsEditable
        {
            get { return isEditable; }
            set { isEditable = value; }
        }

        public string ExpiredText()
        {

            if (isExpired)
            {
                return "The last revision date for this SDS is more than three years";
            }
            else return "";
        }

    }
}