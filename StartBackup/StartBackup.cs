using System;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using WindowsBackup;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace StartBackup
{
    class StartBackup
    {


        static void Main(string[] args)
        {
            string xmlUserdataDrives = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager\\userdata_drives.xml";
            string xmlUserdataSchedule = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager\\userdata_schedule.xml";
            string xmlUserdataMail = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager\\userdata_mail.xml";
            string backupLogFolder = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\Logs\\WindowsBackup";
            string backupSuccessLog = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager\\backupSuccessLog.txt";
            bool isBackupSuccess = true;

            Console.WriteLine("Windows Backup Manager\nPROGRAMM START");
            Console.WriteLine();
            //LogFile
            string logFile = "log.txt";

            //Einlesen der XML Daten
            #region einlesen XML DAten
            //Einlesen der XML Daten

            //Einlesen der zu sichernden Festplatten und der Sicherungsfestplatten
            if (File.Exists(xmlUserdataDrives))
            {
                XmlSerializer xs = new XmlSerializer(typeof(XML_Drives));
                FileStream read = new FileStream(xmlUserdataDrives, FileMode.Open, FileAccess.Read, FileShare.Read);

                XML_Drives datenDrives = (XML_Drives)xs.Deserialize(read);

                string systemDrives = "";
                string backupDrive = "";
                double neededSpace = 0.00;
                double freeSpace = 0.00;

                //Ermittlung der zu sichernden Festplatten

                foreach (string serialNumber in datenDrives.systemDrivesSerialnumbers)
                {

                    try
                    {
                        DriveInformation driveInformation = new DriveInformation();
                        driveInformation = driveInformation.GetDriveInformation(serialNumber);
                        neededSpace += driveInformation.UsedSpace;

                        string message = $"Zu sicherndes Laufwerk gefunden: {driveInformation}";
                        Console.WriteLine(message);

                        systemDrives += $"{driveInformation.DeviceID},";
                        WriteLog(logFile, message, true);

                        //Sicherungsfestplatte suchen

                    }
                    catch (DriveNotFoundException)
                    {
                        string message = $"Zu sicherndes Laufwerk mit Seriennummer: {serialNumber} nicht gefunden.";
                        WriteLog(logFile, message, false);
                        Console.WriteLine(message);
                    }

                }

                foreach (string backupDriveSerialNumber in datenDrives.backupDrives)
                {
                    try
                    {
                        DriveInformation backupDriveInformation = new DriveInformation();
                        backupDriveInformation = backupDriveInformation.GetDriveInformation(backupDriveSerialNumber);

                        freeSpace = backupDriveInformation.FreeSpace;
                        string message = $"Sicherungslaufwerk gefunden: {backupDriveInformation}";
                        WriteLog(logFile, message, true);
                        Console.WriteLine(message);

                        if (freeSpace > neededSpace)
                        {
                            backupDrive += backupDriveInformation.DeviceID;

                            //Beenden da ein Sicherungslaufwerk gefunden wurde.
                            break;
                        }
                        else
                        {
                            throw new System.IO.IOException();
                        }



                    }
                    catch (DriveNotFoundException)
                    {
                        string message = $"Sicherungslaufwerk mit der Seriennummer {backupDriveSerialNumber} nicht gefunden.";

                        WriteLog(logFile, message, false);
                        Console.WriteLine(message);

                    }
                    catch (System.IO.IOException)
                    {
                        DriveInformation drive = new DriveInformation();
                        drive = drive.GetDriveInformation(backupDriveSerialNumber);
                        string message = $"Nicht genug Speicherplatz auf {drive.DeviceID} mit Seriennummer {drive.VolumeSerialNumber}.";

                        WriteLog(logFile, message, false);
                        Console.WriteLine(message);
                    }
                }

                //Starten der Sicherung

                //Anlegen der Ordner

                DirectoryInfo destination = new DirectoryInfo($"{backupDrive}\\WindowsBackupManager\\" + DateTime.Today.ToShortDateString().Replace(".", ""));
                DirectoryInfo source = new DirectoryInfo($"{backupDrive}\\WindowsImageBackup");
                if (!destination.Exists)
                {
                    destination.Create();
                }

                if (systemDrives != "" && backupDrive != "")
                {

                    //Letztes Komma von Sytemdrives entfernen
                    systemDrives = systemDrives.Substring(0, systemDrives.Length - 1);

                    //Benötigen Festplattenspeicher berechnen

                    try
                    {



                        string message = "Starten der Sicherung";
                        WriteLog(logFile, message, true);
                        Console.WriteLine(message);

                        StartWindowsBackup(systemDrives, backupDrive);

                        //Überprüfen der Backup Log Dateien von WBAdmin

                        //Überprüfen ob WindowsBackupOrdner existiert

                        DirectoryInfo logFolder = new DirectoryInfo(backupLogFolder);

                        if (logFolder.Exists == true)
                        {
                            List<FileInfo> backupLogs = new List<FileInfo>();

                            foreach (FileInfo file in logFolder.GetFiles())
                            {
                                if (file.CreationTime.ToShortDateString() == DateTime.Today.ToShortDateString())
                                {
                                    string[] logFileText = File.ReadAllText(file.FullName).Split('\n');
                                    string ergLog = "";
                                    foreach (string text in logFileText)
                                    {

                                        if (text.Length > 1)
                                        {
                                            ergLog += $"{text}\n";

                                            //Überprüfen ob Sicherung erfolgreich

                                            if (text.Contains("wurde erfolgreich gesichert.") == false)
                                            {
                                                isBackupSuccess = false;
                                            }

                                        }

                                    }

                                    WriteLog(logFile, ergLog.Trim(), isBackupSuccess);
                                    Console.WriteLine(ergLog.Trim());
                                    
                                }
                                
                            }

                            //Inhalt der Logdateien auswerten.

                        }
                        else
                        {
                            message = $"Der Ordner{backupLogFolder} konnte nicht gefunden werden.";

                            WriteLog(logFile, message, false);
                            Console.WriteLine(message);
                        }

                        //Verschieben der Sicherung in Ordner WindowsBackupManager

                        Directory.Move(source.FullName, destination.FullName + "\\WindowsImageBackup");


                        BackupSuccess(backupSuccessLog, isBackupSuccess);

                        

                        WriteLog(logFile, $"Backup in Ordner {destination.FullName} verschoben", true);
                    }
                    
                    catch(System.IO.IOException)
                    {
                        string message = $"Der Zugriff auf den Pfad {backupDrive}\\WindowsImageBackup wurde verweigert.";

                        isBackupSuccess = false;

                        BackupSuccess(backupSuccessLog, isBackupSuccess);

                        WriteLog(logFile, message, false);
                        Console.WriteLine(message);
                    }
                    catch (Exception)
                    {
                        string message = "Es ist ein Fehler aufgetreten :( Möglicherweise wurde das Programm ohne Adminrechte gestartet";
                        WriteLog(logFile, message, false);
                        Console.WriteLine(logFile, message);
                    }
                    
                }
                else
                {
                    if (systemDrives == "")
                    {
                        isBackupSuccess = false;
                        Console.WriteLine("Es wurde kein zu sicherndes Laufwerk gefunden.");
                        WriteLog(logFile, "Es wurde kein zu sicherndes Laufwerk gefunden.", false);
                         BackupSuccess(backupSuccessLog, isBackupSuccess);
                    }
                    else if (backupDrive == "")
                    {
                        isBackupSuccess = false;
                        Console.WriteLine("Es wurde kein passendes Sicherungslaufwerk gefunden.");
                        WriteLog(logFile, "Es wurde kein passendes Sicherungslaufwerk gefunden.", false);
                        BackupSuccess(backupSuccessLog, isBackupSuccess);
                    }

                    
                }

                //Mail Einstellungen Einlesen

                if (File.Exists(xmlUserdataMail))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(XML_Mail));
                    FileStream xmlReader = new FileStream(xmlUserdataMail, FileMode.Open, FileAccess.Read, FileShare.Read);

                    XML_Mail datenMail = (XML_Mail)xml.Deserialize(xmlReader);


                    try
                    {

                        NetworkCredential login = new NetworkCredential(datenMail.Username, datenMail.Password);

                        foreach (string empfaenger in datenMail.empfaenger)
                        {

                            //LogFile verschieben

                            string newLogFilePath = $"{backupDrive}\\WindowsBackupManager\\" + DateTime.Today.ToShortDateString().Replace(".", "") + "\\log.txt";
                            FileInfo logFileInfo = new FileInfo(logFile);

                            if (File.Exists(newLogFilePath))
                            {
                                File.AppendAllText(newLogFilePath, File.ReadAllText(logFile));

                                File.Delete(logFile);

                                logFile = newLogFilePath;
                            }
                            else
                            {
                                logFileInfo.MoveTo(newLogFilePath);
                                
                            }

                            logFile = newLogFilePath;

                            string message = $"Logfile nach {logFileInfo.FullName} verschoben";
                            WriteLog(logFile, message, true);
                            Console.WriteLine();

                            MailMessage msg = new MailMessage(datenMail.Absender, empfaenger, "Windows Backup Manager " + Environment.MachineName, File.ReadAllText(logFile));
                            msg.BodyEncoding = Encoding.UTF8;
                            msg.SubjectEncoding = Encoding.UTF8;
                            msg.IsBodyHtml = false;

                            SmtpClient client = new SmtpClient(datenMail.Smtp);
                            client.Credentials = login;
                            client.EnableSsl = true;
                            client.Send(msg);

                            message = "E-Mail wurde versandt";

                            Console.WriteLine(message);
                        }
                    }
                    catch (Exception)
                    {
                        string message = "Fehler beim versenden der E-Mail";
                        WriteLog(logFile, message, false);
                        Console.WriteLine(message);
                    }
                    

                }

            }

            #endregion
            //Nach Sicherung evtl herunterfahren
            try
            {
                if (File.Exists(xmlUserdataSchedule))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(XML_Schedule));
                    FileStream read = new FileStream(xmlUserdataSchedule, FileMode.Open, FileAccess.Read, FileShare.Read);

                    XML_Schedule datenSchedule = (XML_Schedule)xs.Deserialize(read);

                    if (datenSchedule.IsShutdown)
                    {
                        string message = $"PC herunterfahren";
                        Console.WriteLine(message);

                        Process.Start("shutdown.exe", "-s -t 60");

                        WriteLog(logFile, message, true);
                    }

                }
            }
            catch (Exception)
            {
                string message = $"Pc konnte nicht heruntergefahren werden";
                Console.WriteLine(message);

                WriteLog(logFile, message, false);
            }

            Console.WriteLine("PROGRAMM ENDE");


            //Konsole offen lassen.
            System.Threading.Thread.Sleep(5000);
        }

        private static void StartWindowsBackup(string systemDrives, string backupDrive)
        {

            string arguments = $" start backup -backuptarget:{backupDrive} -include:{systemDrives} -allcritical -vssfull -quiet";

            Console.WriteLine(arguments) ;
            
            Process backupProcess = new Process();
            backupProcess.StartInfo.FileName = @"c:\windows\sysnative\wbadmin.exe";
            backupProcess.StartInfo.Arguments = arguments;

            backupProcess.Start();
            backupProcess.WaitForExit();
        }

        private static void WriteLog(string file, string message, bool isSuccess)
        {

            string success = "";
            if (isSuccess)
            {
                success = "+ ";
            }
            else
            {
                success = "\t- ";
            }

            File.AppendAllText(file, $"{success}{DateTime.Now} {message}\n");

        }

        private static void BackupSuccess(string logfile, bool success)
        {
            string message = $"{DateTime.Today.ToShortDateString()} {success}";

            if (File.Exists(logfile))
            {
                if (File.ReadAllText(logfile).Contains(message))
                {
                }
                else
                {
                    File.AppendAllText(logfile, "\n" + message);
                }
                
            }
            else
            {
                File.WriteAllText(logfile, message);
            }
        }
    }
}
