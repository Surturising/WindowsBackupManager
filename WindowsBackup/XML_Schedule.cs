using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsBackup
{
    public class XML_Schedule
    {
        private bool ismonday;
        private bool istuesday;
        private bool iswednesday;
        private bool isthursday;
        private bool isfriday;
        private bool issaturday;
        private bool issunday;
        private bool isshutdown;
        private int hours;
        private int minutes;

        public bool IsMonday
        {
            get { return ismonday; }
            set { ismonday = value; }
        }
        
        public bool IsTuesday
        {
            get { return istuesday; }
            set { istuesday = value; }
        }

        public bool IsWednesday
        {
            get { return iswednesday; }
            set { iswednesday = value; }
        }

        public bool IsThursday
        {
            get { return isthursday; }
            set { isthursday = value; }
        }

        public bool IsFriday
        {
            get { return isfriday; }
            set { isfriday = value; }
        }

        public bool IsSaturday
        {
            get { return issaturday; }
            set { issaturday = value; }
        }

        public bool IsSunday
        {
            get { return issunday; }
            set { issunday = value; }
        }

        public bool IsShutdown
        {
            get { return isshutdown; }
            set { isshutdown = value; }
        }

        public int Hours
        {
            get { return hours; }
            set { hours = value; }
        }

        public int Minutes
        {
            get { return minutes; }
            set { minutes = value; }
        }
    }
}
