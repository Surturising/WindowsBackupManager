using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Xml.Serialization;

namespace WindowsBackup
{
    /// <summary>
    /// Interaktionslogik für Page_Drives.xaml
    /// </summary>
    public partial class Page_Drives : Page
    {
        string xmlUserdataDrives = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager\\userdata_drives.xml";
        public Page_Drives()
        {
            InitializeComponent();



            LoadPage();
        }

        private void bt_Save_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                XML_Drives daten = new XML_Drives();

                //Fügt die Seriennummern der angehakte Laufwerke dem XML Dokument hinzu
                foreach (object drive in Stack_SystemDrives.Children)
                {
                    if ((drive is CheckBox == true) && ((drive as CheckBox).IsChecked == true))
                    {
                        string serialnumber = Convert.ToString((drive as CheckBox).Content).Split('\t')[1];

                        daten.systemDrivesSerialnumbers.Add(serialnumber);
                    }
                }

                //Fügt die Seriennummern der Sicherungslaufwerke der XML Datei hinzu

                foreach (object drive in lb_backupDrives.Items)
                {
                    if ((drive is TextBlock == true))
                    {
                        try
                        {
                            string serialnumber = Convert.ToString((drive as TextBlock).Text).Split('\t')[1];

                            daten.backupDrives.Add(serialnumber);
                        }
                        catch (System.IndexOutOfRangeException)
                        {

                            daten.backupDrives.Add((drive as TextBlock).Text);
                        }
                        
                    }
                }

                //Speichern der XML Datei
                SaveXMLData.SaveXML(daten, xmlUserdataDrives);

                //Erfolgsmeldung
                MessageBox.Show("Daten gespeichert");
                
                //Daten neu aus XML laden
                LoadPage();

            }
            catch (System.IO.IOException)
            {
                MessageBox.Show("Prozess kann nicht auf die userdata_drives.xml zugreifen. Bitte noch einmal auf 'Einstellungen speichern' klicken!", "Fehler beim Speichern der Laufwerksdaten", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception)
            {

                MessageBox.Show("Unbekannter Fehler", "Unbekannter Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        /// <summary>
        /// Entfernt eine Sicherungslaufwerk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_entfernen_Click(object sender, RoutedEventArgs e)
        {

            if ((lb_backupDrives.SelectedItem as TextBlock).Text.Contains('\t'))
            {

                CheckBox newSystemDrive = new CheckBox();
                newSystemDrive.Content = (lb_backupDrives.SelectedItem as TextBlock).Text.ToString();
                newSystemDrive.Checked += cb_updateAvailableBackupDrive_Checked;
                newSystemDrive.Unchecked += cb_updateAvailableBackupDrive_Unchecked;
                Stack_SystemDrives.Children.Add(newSystemDrive);

                TextBlock newAvailableBackupDrive = (lb_backupDrives.SelectedItem as TextBlock);
                newAvailableBackupDrive.Background = Brushes.White;
                lb_backupDrives.Items.Remove(lb_backupDrives.SelectedItem);
                lb_availableBackupDrives.Items.Add(newAvailableBackupDrive);
            }
            else
            {
                lb_backupDrives.Items.Remove(lb_backupDrives.SelectedItem);
            }

        }

        private void bt_hinzufuegen_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                //Überprüfen ob für die angehakten Laufwerke genügend Speicherpalatz vorhanden ist.

                double sizeNeeded = 0.00;
                double sizeAvailableBackupDrive = 0.00;

                //Ermittlung der benötigten Speichergröße
                foreach (object systemDrive in Stack_SystemDrives.Children)
                {
                    if (systemDrive is CheckBox && (systemDrive as CheckBox).IsChecked == true)
                    {
                        DriveInformation systemDriveInformation = new DriveInformation();
                        systemDriveInformation = systemDriveInformation.GetDriveInformation((systemDrive as CheckBox).Content.ToString().Split('\t')[1]);

                        sizeNeeded += (systemDriveInformation.Size - systemDriveInformation.FreeSpace);
                    }
                }

                DriveInformation availableBackupDrive = new DriveInformation();
                availableBackupDrive = availableBackupDrive.GetDriveInformation((lb_availableBackupDrives.SelectedItem as TextBlock).Text.ToString().Split('\t')[1]);
                sizeAvailableBackupDrive += availableBackupDrive.Size;

                if (sizeNeeded > sizeAvailableBackupDrive)
                {
                    MessageBox.Show("Die Festplatte hat zu wenig Speicherplatz für die ausgewählten Laufwerke frei.", "Nicht genug Speicherplatz frei!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    TextBlock item = (lb_availableBackupDrives.SelectedItem as TextBlock);
                    lb_availableBackupDrives.Items.Remove(lb_availableBackupDrives.SelectedItem);

                    lb_backupDrives.Items.Add(item);

                    //Löschen aus SystemDrives

                    foreach (object drive in Stack_SystemDrives.Children)
                    {
                        if ((drive is CheckBox == true) && (drive as CheckBox).Content.ToString() == item.Text.ToString())
                        {
                            Stack_SystemDrives.Children.Remove(drive as CheckBox);
                            break;
                        }
                    }
                }
            }
            catch (System.NullReferenceException)
            {
            }
        }

        /// <summary>
        /// Läd die Seite neu. und lädt die aktuelle User XML.
        /// </summary>
        private void LoadPage()
        {

            //Löschen der aktuellen Daten

            Stack_SystemDrives.Children.Clear();
            lb_availableBackupDrives.Items.Clear();
            lb_backupDrives.Items.Clear();

            //Hinzufügen Systemdrives
            foreach (DriveInformation drive in DriveInformation.GetAllDrives())
            {
                CheckBox checkDrive = new CheckBox();
                checkDrive.Content = drive;
                checkDrive.Checked += cb_updateAvailableBackupDrive_Checked;
                checkDrive.Unchecked += cb_updateAvailableBackupDrive_Unchecked;

                Stack_SystemDrives.Children.Add(checkDrive);
            }

            //Einlesen der XML Daten
            if (File.Exists(xmlUserdataDrives))
            {
                XmlSerializer xs = new XmlSerializer(typeof(XML_Drives));
                FileStream read = new FileStream(xmlUserdataDrives, FileMode.Open, FileAccess.Read, FileShare.Read);

                XML_Drives daten = (XML_Drives)xs.Deserialize(read);

                //Einlesen der BackupDrives

                foreach (string serialnumber in daten.backupDrives)
                {
                    DriveInformation drive = new DriveInformation();
                    
                    //Neues Texblock Objekt für Listbox
                    TextBlock backupDrive = new TextBlock();

                    try
                    {
                        drive = drive.GetDriveInformation(serialnumber);

                        backupDrive.Text = $"{drive}";
                        backupDrive.Background = Brushes.Green;
                        backupDrive.ToolTip = "Sicherungsfestplatte ist angeschlossen :)";

                        foreach (object disk in Stack_SystemDrives.Children)
                        {
                            if (disk is CheckBox)
                            {
                                if ((disk as CheckBox).Content.ToString().Split('\t')[1] == drive.VolumeSerialNumber)
                                {
                                    Stack_SystemDrives.Children.Remove((disk as CheckBox));
                                    break;
                                }
                            }
                        }
                    }
                    catch (DriveNotFoundException)
                    {
                        backupDrive.Text = serialnumber;
                        backupDrive.Background = Brushes.Red;
                        backupDrive.ToolTip = "Sicherungsfestplatte ist nicht angeschlossen";
                    }

                    lb_backupDrives.Items.Add(backupDrive);
                }

                //Überprüfung ob eine Systemdrive schon als BackupDrive registriert

                //Einlesen der Systemdrives checks
                foreach (string serialnumber in daten.systemDrivesSerialnumbers)
                {
                    foreach (object systemdrive in Stack_SystemDrives.Children)
                    {
                        if ((systemdrive is CheckBox) == true && (systemdrive as CheckBox).Content.ToString().Contains(serialnumber))
                        {
                            (systemdrive as CheckBox).IsChecked = true;
                        }
                    }
                }

                //Falls die Systemfestplatte nicht zur sicherung angehakt ist und die Festplatte noch nicht als Sicherungsfestplatte vorhanden ist wird diese den Verfügbaren zugeordnet.
                foreach (object systemdrive  in Stack_SystemDrives.Children)
                {
                    //Überprüfe Seriennummer von BackupDrive und Systemdrive

                    //Nur wenn das Element in dem Stack der Systemdrives eine Checkbox ist
                    if (systemdrive is CheckBox)
                    {
                        //Nur wenn das Laufwerk nicht zur Sicherung angehakt ist.
                        if ((systemdrive as CheckBox).IsChecked == false)
                        {
                            //Überprüfung ob Laufwerk bereits bei den BackupDrives vorhanden ist
                            TextBlock drive = new TextBlock();
                            drive.Text = (systemdrive as CheckBox).Content.ToString();

                            //Überprüfung ob Laufwerk bereits als BackupDrive registriert ist

                            bool isAlreadyBackupDrive = false;
                            foreach (TextBlock backupDrive in lb_backupDrives.Items)
                            {
                                if (backupDrive.Text.ToString() == (systemdrive as CheckBox).Content.ToString())
                                {
                                    isAlreadyBackupDrive = true;
                                }
                            }

                            if (isAlreadyBackupDrive == false)
                            {
                                //Hinzufügen des Laufwerks zu den verfügbaren BackupDrives.

                                lb_availableBackupDrives.Items.Add(drive);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Löscht beim anhaken eines Systemlaufwerkes das Laufwerk aus den verfügbaren Sicherungslaufwerken.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_updateAvailableBackupDrive_Checked(object sender, RoutedEventArgs e)
        {
            
            //Beim anhaken aus verfügbaren Sicherungslaufwerken löschen
            foreach (TextBlock availableBackupDrive in lb_availableBackupDrives.Items)
            {
                if ((sender as CheckBox).Content.ToString() == availableBackupDrive.Text.ToString())
                {
                    lb_availableBackupDrives.Items.Remove(availableBackupDrive);
                    break;
                }
            }
        }

        /// <summary>
        /// Fügt beim abhaken eines Systemlaufwerkes das Laufwerk wieder als verfügbares Sicherungslaufwerk hinzu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_updateAvailableBackupDrive_Unchecked(object sender, RoutedEventArgs e)
        {
            TextBlock newAvailableBackupDrive = new TextBlock();
            newAvailableBackupDrive.Text = (sender as CheckBox).Content.ToString();

            lb_availableBackupDrives.Items.Add(newAvailableBackupDrive);
        }
    }
}
