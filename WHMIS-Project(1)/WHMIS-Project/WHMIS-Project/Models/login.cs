using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WHMIS_Project.Models
{
    public class login
    {

        private int id;
        private string userName;
        private string password;
        private string oldPassword;
        private string newPassword;
        private string confirmPassword;
        private bool isMaster;
        private bool rememberMe;

        public login() { }

        public List<login> Items { get; set; }
        public login(int Id, string UserName, string Password, bool IsMaster)
        {
            id = Id;
            userName = UserName;
            password = Password;
            isMaster = IsMaster;
        }
        public login(string UserName, string Password, bool IsMaster)
        {
            
            userName = UserName;
            password = Password;
            isMaster = IsMaster;
        }
        public login(int Id, string UserName, string OldPassword, string NewPassword, string ConfirmPassword, bool RememberMe)
        {
            id = Id;
            userName = UserName;
            oldPassword = OldPassword;
            newPassword = NewPassword;
            confirmPassword = ConfirmPassword;
            rememberMe = RememberMe;
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [Required(ErrorMessage = "Please enter Email address.")]
        [MaxLength(100, ErrorMessage = "Your Email can not be more than 100 characters long.")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@saskpolytech[\.]ca", ErrorMessage = "Check email address format!")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        [Required(ErrorMessage = "Please enter your password.")]
        [MinLength(6, ErrorMessage = "Your password must be at least 6 characters long.")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [Required(ErrorMessage = "Please enter the old password.")]
        [MinLength(6, ErrorMessage = "Your password must be at least 6 characters long.")]
        public string OldPassword
        {
            get { return oldPassword; }
            set { oldPassword = value; }
        }

        [Required(ErrorMessage = "Please enter the new password.")]
        [MinLength(6, ErrorMessage = "Your password must be at least 6 characters long.")]
        public string NewPassword
        {
            get { return newPassword; }
            set { newPassword = value; }
        }

        [Required(ErrorMessage = "Please enter the new password.")]
        [Compare("NewPassword", ErrorMessage = "The new password and Confirm password fields do not match.")]
        [MinLength(6, ErrorMessage = "Your password must be at least 6 characters long.")]
        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set { confirmPassword = value; }
        }
        public bool IsMaster
        {
            get { return isMaster; }
            set { isMaster = value; }
        }
        public bool RememberMe
        {
            get { return rememberMe; }
            set { rememberMe = value; }
        }
    }

}