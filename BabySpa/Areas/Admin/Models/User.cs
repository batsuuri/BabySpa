using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BabySpa.Areas.Admin.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string EmailSignature { get; set; }
        public string Position { get; set; }
        public int isactive { get; set; }
        public string Duty { get; set; }
        public string AvatarPhoto { get; set; }
    }
}