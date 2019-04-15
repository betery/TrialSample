using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrialSample
{
    class TrialTimeManager
    {
        private string dayExpiredStr = "";
        private bool isFirstStart = false;
        public bool isExpired = false;

        /// <summary>
        /// The constructor.
        /// </summary>
        public TrialTimeManager()
        {
            isFirstStart = CheckExistRegistry();

            if (isFirstStart)
            {
                SetNewDate();
                isExpired = false;
            }
            else
            {
                CheckExpired();
            }
        }

        /// <summary>
        /// Sets the new date +31 days add for trial.
        /// </summary>
        private void SetNewDate()
        {
            DateTime newDate = DateTime.Now.AddDays(31);
            dayExpiredStr = newDate.ToLongDateString();
            StoreDate(dayExpiredStr);
        }

        /// <summary>
        /// Checks if expire or NOT.
        /// </summary>
        private void CheckExpired()
        {
            string d = "";
            using (Microsoft.Win32.RegistryKey key =
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\TrialSample"))
            {
                d = (String)key.GetValue("Date");
            }
            DateTime now;
            if (false == DateTime.TryParse(d, out now))
            {
                MessageBox.Show("System date invalid!");
                return;
            }
            
            int day = (now.Subtract(DateTime.Now)).Days;
            if (day > 7)
            {
                if (MessageBox.Show("Trial expired. Visit site to purchase license?",
                        "Trial Expired!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Process.Start("http://www.google.com");
                }
            }
            else if (0 < day && day <= 30)
            {
                string daysLeft = string.Format("{0} days more to expire", now.Subtract(DateTime.Now).Days);
                MessageBox.Show(daysLeft);
                isExpired = true;
            }
            else if (day <= 0)
            {
                MessageBox.Show("System date invalid!");
                /* Fill with more code, once it has expired, what will happen nex! */
            }
        }

        /// <summary>
        /// Stores the new date value in registry.
        /// </summary>
        /// <param name="value"></param>
        private void StoreDate(string value)
        {
            try
            {
                using (Microsoft.Win32.RegistryKey key =
                    Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SOFTWARE\TrialSample"))
                {
                    key.SetValue("Date", value, Microsoft.Win32.RegistryValueKind.String);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static bool CheckExistRegistry()
        {
            Microsoft.Win32.RegistryKey winLogonKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\TrialSample", false);
            return null == winLogonKey;
        }
    }
}
