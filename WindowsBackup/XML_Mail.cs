using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WindowsBackup
{
    public class XML_Mail
    {
        public List<string> empfaenger = new List<string>();

        private string absender;
        private string smtp;
        private string username;
        private string password;
        //Properties

        public string Absender
        {
            get { return absender; }
            set { absender = value; }
        }

        public string Smtp
        {
            get { return smtp; }
            set { smtp = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }
    }
}
