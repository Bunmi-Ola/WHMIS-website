using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WHMIS_Project.Models
{
    public class DataSheet_Utility
    {

        string conString = "";

        public List<Safety_DataSheet> docSelectAll()
        {

            List<Safety_DataSheet> chemicalList = new List<Safety_DataSheet>();

            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT Id, chemicalName, fileName, [Last Revision Date], [CAS Number] FROM safetyDataSheet";
            using (con)
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    chemicalList.Add(new Safety_DataSheet(
                        (int)reader["Id"],
                        (string)reader["chemicalName"],
                        (string)reader["fileName"],
                         (DateTime)reader["Last Revision Date"],
                         System.DBNull.Value.Equals(reader["CAS Number"]) ? "" :
                                      (string)reader["CAS Number"]));
                }
            }
            return chemicalList;

        }

        public void docInsert(string chemicalName, string fileName, byte[]fileDoc, String CAS, DateTime RevisionDate)
        {

            if (String.IsNullOrEmpty(CAS) == true)
            {
                CAS = "null";
            }

            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            cmd.CommandText = "INSERT INTO safetyDataSheet(chemicalName, fileName, fileDocument,[Last Revision Date],[CAS Number])" +
                " VALUES(@ChemicalName, @FileName, @FileDocument, @LRD, @CAS)";
            cmd.Parameters.AddWithValue("@ChemicalName", chemicalName);
            cmd.Parameters.AddWithValue("@FileName", fileName);
            cmd.Parameters.AddWithValue("@FileDocument", fileDoc);
            cmd.Parameters.AddWithValue("@LRD", RevisionDate);
            cmd.Parameters.AddWithValue("@CAS", CAS);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            docSelectAll();
        }

        public byte[] getDocbyId(int id)
        {

            byte[] bytes;
          
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT fileDocument FROM safetyDataSheet where Id=@Id";
            cmd.Parameters.AddWithValue("Id", id);
            using (con)
            {
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    sdr.Read();
                    bytes = (byte[])sdr["fileDocument"];
                    
                }
                con.Close();
            }
       
            return bytes;

        }

        public Boolean CheckDoc(byte[] bytes, int ID)
        {
            Boolean check = false ;
            
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            if (ID == 0)
            {
                cmd.CommandText = "SELECT * FROM safetyDataSheet where fileDocument=@fileDocument";
            }
            else
            {
                cmd.CommandText = "SELECT * FROM safetyDataSheet where fileDocument=@fileDocument AND Id<>@Id";
                cmd.Parameters.AddWithValue("@Id", ID);
            }
           
            cmd.Parameters.AddWithValue("@fileDocument", bytes);
            using (con)
            {
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    sdr.Read();
                    if (sdr.HasRows)
                    {
                        check = true;
                    }

                }
                con.Close();
            }

            return check;

        }

        public Boolean CheckChemicalName(string chemicalName, int ID)
        {
            Boolean check = false;

            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            if (ID == 0)
            {
                cmd.CommandText = "SELECT * FROM safetyDataSheet where chemicalName=@chemicalName";
            }
            else
            {
                cmd.CommandText = "SELECT * FROM safetyDataSheet where chemicalName=@chemicalName AND Id<>@Id";
                cmd.Parameters.AddWithValue("@Id", ID);
            }
           
            cmd.Parameters.AddWithValue("@chemicalName", chemicalName);
           
            using (con)
            {
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    sdr.Read();
                    if (sdr.HasRows)
                    {
                        check = true;
                    }

                }
                con.Close();
            }

            return check;

        }

        public Safety_DataSheet getInfoByID(int id)
        {

            List<Safety_DataSheet> SD = new List<Safety_DataSheet>();

            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT Id,chemicalName,fileName, [Last Revision Date], [CAS Number] " +
                "FROM safetyDataSheet where Id=@Id";

            cmd.Parameters.AddWithValue("Id", id);

            using (con)
            {
                con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    SD.Add(new Safety_DataSheet(
                        Convert.ToInt32(reader["Id"]),
                        (string)reader["chemicalName"].ToString(),
                       (string)reader["fileName"].ToString(),
                         (DateTime)reader["Last Revision Date"],
                        (string)reader["CAS Number"].ToString()
                    ));
                }
              
            }
            return SD[0];

        }

        public void InfoUpdate(Safety_DataSheet InfoToUpdate)
        {
            SqlConnection con = new SqlConnection(conString);

            if (String.IsNullOrEmpty(InfoToUpdate.CAS)== true) {
                InfoToUpdate.CAS = "null";
                     }
          

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "UPDATE safetyDataSheet SET chemicalName=@Name,[CAS Number]=@CAS,  [Last Revision Date]=@LRD WHERE Id=@Id";
            cmd.Parameters.AddWithValue("@Name", InfoToUpdate.ChemicalName);
            cmd.Parameters.AddWithValue("@CAS", InfoToUpdate.CAS);           
            cmd.Parameters.AddWithValue("@Id", InfoToUpdate.Id);
            cmd.Parameters.AddWithValue("@LRD", InfoToUpdate.lastRevisionDate);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void InfoUpdatewithDocReplace(Safety_DataSheet InfoToUpdate, byte[] fileDoc)
        {
            SqlConnection con = new SqlConnection(conString);

            if (String.IsNullOrEmpty(InfoToUpdate.CAS) == true)
            {
                InfoToUpdate.CAS = "null";
            }          

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "UPDATE safetyDataSheet SET chemicalName=@Name,[CAS Number]=@CAS, fileName=@fileName, fileDocument=@fileDocument, [Last Revision Date]=@LRD WHERE Id=@Id";
            cmd.Parameters.AddWithValue("@Name", InfoToUpdate.ChemicalName);
            cmd.Parameters.AddWithValue("@CAS", InfoToUpdate.CAS);
            cmd.Parameters.AddWithValue("@fileName", InfoToUpdate.FileDocName);
            cmd.Parameters.AddWithValue("@fileDocument", fileDoc);
            cmd.Parameters.AddWithValue("@LRD", InfoToUpdate.lastRevisionDate);
            cmd.Parameters.AddWithValue("@Id", InfoToUpdate.Id);
            

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void InfoDelete(int id)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "DELETE FROM safetyDataSheet WHERE Id=@Id";

            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UserInsert(login User)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "INSERT INTO Users(UserName, Password, IsMaster)" +
                " VALUES(@UserName, @Password, @master)";
            cmd.Parameters.AddWithValue("@UserName", User.UserName);
            cmd.Parameters.AddWithValue("@Password", User.NewPassword);
            cmd.Parameters.AddWithValue("@master", User.IsMaster);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            
        }

        [HttpPost]
        public login logIn(login user)
        {
           
            login user2 = new login(0, "null", "null", false);

            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
         
                cmd.CommandText = "SELECT * FROM Users where UserName = @userName AND Password =@password";

            if (String.IsNullOrEmpty(user.NewPassword))
            {
                cmd.Parameters.AddWithValue("@password", user.Password);
            }
            else
            {
                cmd.Parameters.AddWithValue("@password", user.NewPassword);
            }
            cmd.Parameters.AddWithValue("@userName", user.UserName);
            
            using (con)
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    user2 = new login(
                        (int)reader["Id"],
                        (string)reader["UserName"],
                        (string)reader["Password"],
                        (bool)reader["IsMaster"]
                    );
                }
            }
           
            return user2;
        }

        public List<login> userSelectAll()
        {

            List<login> userList = new List<login>();

            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT Id, UserName, Password, isMaster FROM Users";
            using (con)
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    userList.Add(new login(
                        (int)reader["Id"],
                        (string)reader["UserName"],
                        (string)reader["Password"],
                         (bool)reader["isMaster"]));
                }
            }
            return userList;

        }

        public Boolean CheckUserName(String Email, String Password)
        {
            Boolean check = false;

            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            if (string.IsNullOrEmpty(Password))
            {
                cmd.CommandText = "SELECT * FROM Users where UserName =@userName";
            }
            else
            {
                cmd.CommandText = "SELECT * FROM Users where UserName = @userName AND Password =@password";
                cmd.Parameters.AddWithValue("@password", Password);              
            }


            cmd.Parameters.AddWithValue("@userName", Email);
            using (con)
            {
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    sdr.Read();
                    if (sdr.HasRows)
                    {
                        check = true;
                    }

                }
                con.Close();
            }

            return check;

        }

        public void UserPasswordUpdate(String Email, String NewPassword)
        {
            SqlConnection con = new SqlConnection(conString);

         
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            cmd.CommandText = "UPDATE Users SET Password=@password WHERE UserName =@userName";
            cmd.Parameters.AddWithValue("@userName", Email);
            cmd.Parameters.AddWithValue("@password", NewPassword);
           

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public void UserDelete(int id)
        {
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "DELETE FROM Users WHERE Id=@Id";

            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}