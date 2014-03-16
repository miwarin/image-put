using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImagePut
{
    public class Config
    {
        public static String Source
        {
            get
            {
                return Properties.Settings.Default.Source;
            }
            set
            {
                Properties.Settings.Default.Source = value;
                Properties.Settings.Default.Save();
            }
        }

        public static String Destination
        {
            get
            {
                return Properties.Settings.Default.Destination;
            }
            set
            {
                Properties.Settings.Default.Destination = value;
                Properties.Settings.Default.Save();
            }
        }

        public static String HostName
        {
            get
            {
                return Properties.Settings.Default.HostName;
            }
            set
            {
                Properties.Settings.Default.HostName = value;
                Properties.Settings.Default.Save();
            }
        }

        public static String UserName
        {
            get
            {
                return Properties.Settings.Default.UserName;
            }
            set
            {
                Properties.Settings.Default.UserName = value;
                Properties.Settings.Default.Save();
            }
        }

        public static String Password
        {
            get
            {
                return Properties.Settings.Default.Password;
            }
            set
            {
                Properties.Settings.Default.Password = value;
                Properties.Settings.Default.Save();
            }
        }

        public static String FingerPrint
        {
            get
            {
                return Properties.Settings.Default.FingerPrint;
            }
            set
            {
                Properties.Settings.Default.FingerPrint = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
