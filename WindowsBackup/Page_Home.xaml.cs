using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Diagnostics;
using System.Windows.Media;
using System.Xml.Serialization;

namespace WindowsBackup
{
    /// <summary>
    /// Interaktionslogik für Page_Home.xaml
    /// </summary>
    public partial class Page_Home : Page
    {

        string xmlUserdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager";
        string xmlUserdataDrives = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager\\userdata_drives.xml";
        string backupFolder = "\\WindowsBackupManager";
        string windowsImageBackupFolder = "\\WindowsImageBackup";
        string xmlUserdataSchedule = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager\\userdata_schedule.xml";
        string backupSuccessLog = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager\\backupSuccessLog.txt";

        public Page_Home()
        {
            InitializeComponent();

            if (File.Exists(backupSuccessLog))
            {
                LoadPage();
            }
            
        }


        /// <summary>
        /// Löscht die Userdaten des Programms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_DeleteConfig_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                DirectoryInfo userdataFolder = new DirectoryInfo(xmlUserdata);

                if (userdataFolder.Exists)
                {

                    //Löschen der Userdaten
                    userdataFolder.Delete(true);
                    userdataFolder.Create();

                    //Erfolgsmeldung falls Userdaten gelöscht wurden
                    MessageBox.Show("Konfigurationsdaten wurden erfolgreich gelöscht");
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void bt_StartBackup_Click(object sender, RoutedEventArgs e)
        {

            //Fragen ob PC nach Sicherung heruntergefahren werden soll.
            MessageBoxResult result = MessageBox.Show("Sollder PC nach der Sicherung heruntergefahren werden?", "ShutdownPC?", MessageBoxButton.YesNo, MessageBoxImage.Question);

            //Starten des Backups
            Process scheduledTask = new Process();
            scheduledTask.StartInfo.FileName = "StartBackup.exe";
            scheduledTask.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            scheduledTask.StartInfo.Verb = "runas";
            scheduledTask.Start();
            scheduledTask.WaitForExit();



            if (result == MessageBoxResult.Yes )
            {
                Process.Start("shutdown.exe", "-s -t 60");
            }
        }


        private void LoadPage()
        {
            string[] backupLog = File.ReadAllText(backupSuccessLog).Split('\n');

            string lastSuccessfullBackup = "";
            bool islastBackupSuccessfull = false;
            string lastBackupDate = "";
            int sumSuccesfullBackups = 0;
            int sumFailedBackups = 0;
            
            foreach (string log in backupLog)
            {
                if (log.Contains("True"))
                {
                    //AddCanvas(ref stack, log.Split(' ')[0].Replace(".", ""), true);
                    lastSuccessfullBackup = log.Split(' ')[0];
                    islastBackupSuccessfull = true;
                    sumFailedBackups++;
                }
                else
                {
                    islastBackupSuccessfull = false;
                    sumSuccesfullBackups++;
                }

                lastBackupDate = log.Split(' ')[0];
            }

            if (File.Exists(xmlUserdataSchedule))
            {
               
                XmlSerializer xs = new XmlSerializer(typeof(XML_Schedule));
                FileStream read = new FileStream(xmlUserdataSchedule, FileMode.Open, FileAccess.Read, FileShare.Read);

                XML_Schedule daten = (XML_Schedule)xs.Deserialize(read);


                if (true)
                {

                }

            }


            //Daten Laden
            tb_lastBackupSuccess.Text = lastSuccessfullBackup;

            if (islastBackupSuccessfull == true)
            {
                tb_isLastBackupSuccess.Text = "Erfolgreich";
                tb_isLastBackupSuccess.Foreground = Brushes.Green;
                tb_isLastBackupSuccess.ToolTip = $"Das Backup vom {lastBackupDate} war erfolgreich.";
            }
            else
            {
                tb_isLastBackupSuccess.Text = "Fehlgeschlagen";
                tb_isLastBackupSuccess.Foreground = Brushes.Red;
                tb_isLastBackupSuccess.ToolTip = $"Das Backup vom {lastBackupDate} ist fehlgeschlagen.";
            }

            tb_sumFailedBackups.Text = sumFailedBackups.ToString();
            tb_sumFailedBackups.ToolTip = $"{sumFailedBackups}/{sumFailedBackups + sumSuccesfullBackups} Backups fehlgeschlagen.";
            
            tb_sumSuccesfullBackups.Text = sumSuccesfullBackups.ToString();
            tb_sumSuccesfullBackups.ToolTip = $"{sumSuccesfullBackups}/{sumFailedBackups + sumSuccesfullBackups} Backups erfolgreich.";
        }
    }
}
