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
    /// Interaktionslogik für Page_Trash.xaml
    /// </summary>
    public partial class Page_Trash : Page
    {
        string xmlUserdataDrives = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager\\userdata_drives.xml";
        string backupFolder = "\\WindowsBackupManager";
        string windowsImageBackupFolder = "\\WindowsImageBackup";
        string anleitungRestoreImage = "PDf\\Anleitung_RestoreImage.pdf";
        string mountVHDPSScript = "Scripts\\Mount-VHDXBackup.ps1";

        public Page_Trash()
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
                                CheckBox backup = new CheckBox();
                                backup.Content = directoryInfo.Name;
                                backup.Margin = new Thickness(0, 0, 5, 5);
                                backup.ToolTip = directoryInfo.FullName;

                                wp_Backups.Children.Add(backup);
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

        /// <summary>
        /// Wenn Datum im Kalender ausgewählt wird -> Wird WrapPanel der Backups aktualisiert und Backups ausgewählt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void datep_SelectBackups_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (CheckBox check in wp_Backups.Children)
            {
                DirectoryInfo backupInfo = new DirectoryInfo(check.ToolTip.ToString());

                if (cbi_FilterLower.IsSelected == true)
                {
                    if (backupInfo.CreationTime < (sender as DatePicker).SelectedDate)
                    {
                        check.IsChecked = true;
                    }
                    else
                    {
                        check.IsChecked = false;
                    }
                }
                else
                {
                    if (backupInfo.CreationTime > (sender as DatePicker).SelectedDate)
                    {
                        check.IsChecked = true;
                    }
                    else
                    {
                        check.IsChecked = false;
                    }
                }
            }
        }

        /// <summary>
        /// Löschen der im Wrap Panel ausgewählten Backups durch start des Programms RemoveBackup.exe als Administrator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_RemoveBackups_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string arg = "/Backup: ";
                foreach (CheckBox backup in wp_Backups.Children)
                {
                    if (backup.IsChecked == true)
                    {
                        arg += $"{backup.ToolTip};";
                    }
                }

                Process deleteBackup = new Process();
                deleteBackup.StartInfo.FileName = "RemoveBackup.exe";
                deleteBackup.StartInfo.Verb = "runas";
                deleteBackup.StartInfo.Arguments = arg.Substring(0, arg.Length -1);
                deleteBackup.Start();
                deleteBackup.WaitForExit();

                if (deleteBackup.ExitCode == 0)
                {
                    MessageBox.Show("Backups wurden erfolgreich gelöscht");
                    wp_Backups.Children.Clear();
                    LoadBackups();
                }
                else
                {
                    MessageBox.Show("Beim Löschvorgang der Sicherung ist etwas schiefgegangen.", "Fehler beim Kopiervorgang", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (NullReferenceException)
            {

                MessageBox.Show("Bitte wählen Sie eine Sicherung aus.", "Keine Sicherung gewählt", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Der Löschvorgang wurde vom Benutzer abgebrochen", "Löschen Fehlgeschlagen", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Ändern der ausgewählten Backups im WrapPanel wenn die Filtermöglichkeit auf kleiner oder größer geändert wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (CheckBox check in wp_Backups.Children)
            {
                DirectoryInfo backupInfo = new DirectoryInfo(check.ToolTip.ToString());

                try
                {
                    if (cbi_FilterLower.IsSelected == true)
                    {
                        if (backupInfo.CreationTime < datep_SelectBackups.SelectedDate)
                        {
                            check.IsChecked = true;
                        }
                        else
                        {
                            check.IsChecked = false;
                        }
                    }
                    else
                    {
                        if (backupInfo.CreationTime > datep_SelectBackups.SelectedDate)
                        {
                            check.IsChecked = true;
                        }
                        else
                        {
                            check.IsChecked = false;
                        }
                    }
                }
                catch (NullReferenceException)
                {

                }
            }
        }
    }
}
