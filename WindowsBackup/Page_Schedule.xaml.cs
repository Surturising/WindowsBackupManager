using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

namespace WindowsBackup
{
    /// <summary>
    /// Interaktionslogik für Page_Schedule.xaml
    /// </summary>
    public partial class Page_Schedule : Page
    {

        string xmlUserdataSchedule = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager\\userdata_schedule.xml";

        public Page_Schedule()
        {
            InitializeComponent();

            if (File.Exists(xmlUserdataSchedule))
            {
                XmlSerializer xs = new XmlSerializer(typeof(XML_Schedule));
                FileStream read = new FileStream(xmlUserdataSchedule, FileMode.Open, FileAccess.Read, FileShare.Read);

                XML_Schedule daten = (XML_Schedule)xs.Deserialize(read);

                cb_Monday.IsChecked = daten.IsMonday;
                cb_Tuesday.IsChecked = daten.IsTuesday;
                cb_Wednesday.IsChecked = daten.IsWednesday;
                cb_Thursday.IsChecked = daten.IsThursday;
                cb_Friday.IsChecked = daten.IsFriday;
                cb_Saturday.IsChecked = daten.IsSaturday;
                cb_Sunday.IsChecked = daten.IsSunday;
                cb_Shutdown.IsChecked = daten.IsShutdown;

                if (daten.Minutes == 0 || daten.Minutes == 1 || daten.Minutes == 2 || daten.Minutes == 3 || daten.Minutes == 4 || daten.Minutes == 5 || daten.Minutes == 6 || daten.Minutes == 7 || daten.Minutes == 8 || daten.Minutes == 9)
                {
                    tb_Minutes.Text = "0" + Convert.ToString(daten.Minutes);
                }
                else
                {
                    tb_Minutes.Text = Convert.ToString(daten.Minutes);
                }

                if (daten.Hours == 0 || daten.Hours == 1 || daten.Hours == 2 || daten.Hours == 3 || daten.Hours == 4 || daten.Hours == 5 || daten.Hours == 6 || daten.Hours == 7 || daten.Hours == 8 || daten.Hours == 9)
                {
                    tb_Hours.Text = "0" + Convert.ToString(daten.Hours);
                }
                else
                {
                    tb_Hours.Text = Convert.ToString(daten.Hours);
                }

                
                
            }
        }

        private void bt_Save_Click(object sender, RoutedEventArgs e)
        {
            //Neues Daten Elemente zum Speichern der Einstellungen
            XML_Schedule daten = new XML_Schedule();

            // Checkboxen speichern
            if (cb_Monday.IsChecked == true)
            {
                daten.IsMonday = true;
            }
            else
            {
                daten.IsMonday = false;
            }

            if (cb_Tuesday.IsChecked == true)
            {
                daten.IsTuesday = true;
            }
            else
            {
                daten.IsTuesday = false;
            }

            if (cb_Wednesday.IsChecked == true)
            {
                daten.IsWednesday = true;
            }
            else
            {
                daten.IsWednesday = false;
            }

            if (cb_Thursday.IsChecked == true)
            {
                daten.IsThursday = true;
            }
            else
            {
                daten.IsThursday = false;
            }


            if (cb_Friday.IsChecked == true)
            {
                daten.IsFriday = true;
            }
            else
            {
                daten.IsFriday = false;
            }


            if (cb_Saturday.IsChecked == true)
            {
                daten.IsSaturday = true;
            }
            else
            {
                daten.IsSaturday = false;
            }


            if (cb_Sunday.IsChecked == true)
            {
                daten.IsSunday = true;
            }
            else
            {
                daten.IsSunday = false;
            }

            if (cb_Shutdown.IsChecked == true)
            {
                daten.IsShutdown = true;
            }
            else
            {
                daten.IsShutdown = false;
            }

            //Speichern der Uhrzeit

            try
            {
                int hours = Convert.ToInt32(tb_Hours.Text);
                int minutes = Convert.ToInt32(tb_Minutes.Text);
                //Testen ob Hours unter 24 und nicht negativ.
                if (hours > 24 || hours < 0 || minutes > 59 || minutes < 0)
                {
                    throw new System.FormatException();
                }
                else
                {
                    // Setzen der Uhrzeit
                    daten.Hours = hours;
                    daten.Minutes = minutes;
                }



                //Speichern der Userdaten
                SaveXMLData.SaveXML(daten, xmlUserdataSchedule);
                tb_Hours.Background = Brushes.White;
                tb_Minutes.Background = Brushes.White;

                #region Task für Aufgabenplanung

                
                //Pfad zur Exe Datei
                FileInfo startBackupExe = new FileInfo("StartBackup.exe");
                string exe = startBackupExe.FullName;

                //Zeiteinstellungen Tage

                string weekdays = "";

                if (cb_Monday.IsChecked == true)
                {
                    weekdays += "mon,";
                }
                if (cb_Tuesday.IsChecked == true)
                {
                    weekdays += "tue,";
                }
                if (cb_Wednesday.IsChecked == true)
                {
                    weekdays += "wed,";
                }
                if (cb_Thursday.IsChecked == true)
                {
                    weekdays += "thu,";
                }
                if (cb_Friday.IsChecked == true)
                {
                    weekdays += "fri,";
                }
                if (cb_Saturday.IsChecked == true)
                {
                    weekdays += "sat,";
                }
                if (cb_Sunday.IsChecked == true)
                {
                    weekdays += "sun,";
                }

                //Letztes Komma löschen
                weekdays = weekdays.Substring(0, weekdays.Length - 1);

                //Zeiteinstellungen für Uhrzeit

                string time = tb_Hours.Text.ToString() + ":" + tb_Minutes.Text.ToString();

                //Anlegen der Aufgabe in Aufgabenplanung
                Process scheduledTask = new Process();
                scheduledTask.StartInfo.FileName = "schtasks.exe";
                scheduledTask.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                scheduledTask.StartInfo.Verb = "runas";
                scheduledTask.StartInfo.Arguments = "/create /TN \"StartWindowsBackup\" /tr " + "\"" + exe + "\"" + " /sc weekly /d " + weekdays + " /st " + time + " /RL HIGHEST /F";
                scheduledTask.Start();

                #endregion
                //Erfolgsmeldung
                MessageBox.Show("Daten gespeichert");

            }
            catch (System.FormatException)
            {
                MessageBox.Show("Die Eingabezeichenfolge hat das falsche Format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                tb_Hours.Background = Brushes.Red;
                tb_Minutes.Background = Brushes.Red;
            }
            catch (System.IO.IOException)
            {

                MessageBox.Show("Prozess kann nicht auf die userdata_schedule.xml zugreifen. Bitte noch einmal auf 'Einstellungen speichern' klicken!", "Fehler beim Speichern der Userdaten", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (System.ComponentModel.Win32Exception) 
            {
                MessageBox.Show("Das anlegen der geplanten Aufgabe wurde vom Benuzter abgebrochen. Einstellungen wurden nicht gespeichert!", "Abbruch durch Benutzer", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }
    }
}
