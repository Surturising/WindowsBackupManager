using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

namespace WindowsBackup
{
    /// <summary>
    /// Interaktionslogik für Page_Restore.xaml
    /// </summary>
    public partial class Page_Restore : Page
    {
        string xmlUserdataDrives = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager\\userdata_drives.xml";
        string backupFolder = "\\WindowsBackupManager";
        string windowsImageBackupFolder = "\\WindowsImageBackup";
        string anleitungRestoreImage = "PDf\\Anleitung_RestoreImage.pdf";

        public Page_Restore()
        {
            InitializeComponent();

            LoadBackups();
        }


        private void LoadBackups()
        {

            if (File.Exists(xmlUserdataDrives))
            {
                XmlSerializer xs = new XmlSerializer(typeof(XML_Drives));
                FileStream read = new FileStream(xmlUserdataDrives, FileMode.Open, FileAccess.Read, FileShare.Read);

                XML_Drives daten = (XML_Drives)xs.Deserialize(read);

                //Einlesen der BackupDrives

                foreach (string serialnumber in daten.backupDrives)
                {

                    try
                    {
                        DriveInformation drive = new DriveInformation();

                        drive = drive.GetDriveInformation(serialnumber);

                        //MessageBox.Show(drive.ToString());
                        DirectoryInfo backupInfo = new DirectoryInfo(drive.DeviceID + backupFolder);

                        foreach (DirectoryInfo directoryInfo in backupInfo.GetDirectories())
                        {


                            DirectoryInfo windowsImageBackup = new DirectoryInfo(directoryInfo.FullName + windowsImageBackupFolder);


                            if (windowsImageBackup.Exists)
                            {

                                ComboBoxItem backup = new ComboBoxItem();
                                backup.Content = directoryInfo.Name;
                                backup.ToolTip = directoryInfo.FullName;

                                cb_Backups.Items.Add(backup);
                            }

                        }
                    }
                    catch (System.IO.DriveNotFoundException)
                    {
                        TextBlock driveNotFound = new TextBlock();
                        driveNotFound.Text = $"Achtung: Das Laufwerk mit der Seriennummer {serialnumber} konnte nicht gefunden werden.\nBackups von diesem Laufwerk können nicht angezeigt werden.\n";
                        driveNotFound.Background = Brushes.Red;

                        sp_DrivesNotFound.Children.Add(driveNotFound);
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {

                    }


                }
            }
        }

        private void bt_RestoreData_Click(object sender, RoutedEventArgs e)
        {

            //Suchen der Festplatte

            try
            {
                /*
                DirectoryInfo backupFolder = new DirectoryInfo((cb_Backups.SelectedItem as ComboBoxItem).ToolTip.ToString());

                Process mountBackup = new Process();
                mountBackup.StartInfo.FileName = "powershell.exe";
                //mountBackup.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                mountBackup.StartInfo.Verb = "runas";
                mountBackup.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{mountVHDPSScript} -BackupFolder {backupFolder.FullName}\"";
                mountBackup.Start();
                mountBackup.WaitForExit();
                */
            }
            catch (System.NullReferenceException)
            {

                MessageBox.Show("Bitte wählen Sie eine Sicherung aus.", "Keine Sicherung gewählt", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            



        }

        private void bt_RestoreWindowsImage_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                DirectoryInfo backupFolder = new DirectoryInfo((cb_Backups.SelectedItem as ComboBoxItem).ToolTip.ToString() + "\\WindowsImageBackup");

                Process copyBackup = new Process();
                copyBackup.StartInfo.FileName = "MoveBackup.exe";
                copyBackup.StartInfo.Verb = "runas";
                copyBackup.StartInfo.Arguments = $"/Backup: {backupFolder.FullName}";
                copyBackup.Start();
                copyBackup.WaitForExit();

                if (copyBackup.ExitCode == 0)
                {
                    MessageBox.Show("Führen Sie die Schritte der PDF Datei aus.");

                    Process.Start(anleitungRestoreImage);
                }
                else
                {
                    MessageBox.Show("Beim Kopiervorgang der Sicherung ist etwas schiefgegangen.", "Fehler beim Kopiervorgang", MessageBoxButton.OK, MessageBoxImage.Error);
                }


            }
            catch (System.NullReferenceException)
            {

                MessageBox.Show("Bitte wählen Sie eine Sicherung aus.", "Keine Sicherung gewählt", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            



        }
    }
}
