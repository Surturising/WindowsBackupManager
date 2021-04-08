using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO;

namespace WindowsBackup
{
    public class DriveInformation
    {
        private string deviceID;
        private string volumeSerialNumber;
        private double size;
        private double freeSpace;
        private double usedSpace;
        private string volumeName;

        public string DeviceID
        {
            get { return deviceID; }
            private set { deviceID = value; }
        }

        public string VolumeSerialNumber
        {
            get { return volumeSerialNumber; }
            private set { volumeSerialNumber = value; }
        }
        
        public double Size
        {
            get { return size; }
            private set { size = value; }
        }

        public double FreeSpace
        {
            get { return freeSpace; }
            private set { freeSpace = value; }
        }

        public double UsedSpace
        {
            get { return usedSpace; }
            private set { usedSpace = value; }
        } 

        public string VolumeName
        {
            get { return volumeName; }
            private set { volumeName = value; }
        }


        /// <summary>
        /// Gibt Informationen über ein Laufwerk mit der entsprechenden Seriennummer
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns>DriveInformation</returns>
        public DriveInformation GetDriveInformation(string serialNumber)
        {

            //Seriennummer in Großbuchstaben umwandeln.

            serialNumber = serialNumber.ToUpper();

            DriveInformation erg = new DriveInformation();
            ManagementObjectSearcher mgmtObjSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_logicaldisk");
            ManagementObjectCollection colDisks = mgmtObjSearcher.Get();

            //Alle Laufwerke finden und hinzufügen
            bool driveExists = false;
            foreach (ManagementObject objDisk in colDisks)
            {

                if (Convert.ToString(objDisk["VolumeSerialNumber"]) == serialNumber)
                {
                    erg.DeviceID = Convert.ToString(objDisk["DeviceID"]);

                    erg.VolumeSerialNumber = Convert.ToString(objDisk["VolumeSerialNumber"]);

                    erg.Size = Convert.ToDouble((objDisk["Size"]));

                    erg.FreeSpace = Convert.ToDouble((objDisk["FreeSpace"]));
                    
                    erg.UsedSpace = erg.Size - erg.FreeSpace;

                    erg.VolumeName = Convert.ToString(objDisk["VolumeName"]);

                    driveExists = true;
                }
            }

            //Falls die Seriennummer nicht vorhanden => Exception
            if (driveExists == false)
            {
                throw new DriveNotFoundException();
            }

            return erg;
        }

        /// <summary>
        /// Gibt Informationen über ein Laufwerk mit dem entsprechenden Laufwerksbuchstaben
        /// </summary>
        /// <param name="laufwerkbuchstabe"></param>
        /// <returns>DriveInformation</returns>
        public DriveInformation GetDriveInformation(char laufwerkbuchstabe = 'c')
        {

            //Laufwerksbuchstabe in Großbuchstaben umwandeln

            laufwerkbuchstabe = Convert.ToChar(laufwerkbuchstabe.ToString().ToUpper());

            DriveInformation erg = new DriveInformation();
            ManagementObjectSearcher mgmtObjSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_logicaldisk");
            ManagementObjectCollection colDisks = mgmtObjSearcher.Get();

            //Alle Laufwerke finden und hinzufügen
            
            bool driveExists = false;
            foreach (ManagementObject objDisk in colDisks)
            {

                if (Convert.ToString(objDisk["DeviceID"]) == $"{laufwerkbuchstabe}:")
                {
                    erg.DeviceID = Convert.ToString(objDisk["DeviceID"]);

                    erg.VolumeSerialNumber = Convert.ToString(objDisk["VolumeSerialNumber"]);

                    erg.Size = Convert.ToDouble((objDisk["Size"]));

                    erg.FreeSpace = Convert.ToDouble((objDisk["FreeSpace"]));

                    erg.UsedSpace = erg.Size - erg.FreeSpace;

                    erg.VolumeName = Convert.ToString(objDisk["VolumeName"]);

                    driveExists = true;
                }
            }

            //Falls der Laufwerksbuchstabe nicht vorhanden => Exception
            if (driveExists == false)
            {
                throw new DriveNotFoundException();
            }

            return erg;
        }

        /// <summary>
        /// Gibt eine Liste mit allen angeschlossenen Laufwerken zurück. Die Liste enthält DriveInformation Objekte
        /// </summary>
        /// <returns>List<DriveInformation></returns>
        public static List<DriveInformation> GetAllDrives()
        {
            //Objekt das der Liste hinzugefügt wird
            DriveInformation driveInformation = new DriveInformation();

            //Liste mit allen angelschlossenen Laufwerken
            List<DriveInformation> erg = new List<DriveInformation>();

            //Per WMI auf alle Laufwerke zugreifen
            ManagementObjectSearcher mgmtObjSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_logicaldisk");
            ManagementObjectCollection colDisks = mgmtObjSearcher.Get();

            //Alle Laufwerke finden und hinzufügen
            foreach (ManagementObject objDisk in colDisks)
            {
                erg.Add(driveInformation.GetDriveInformation($"{objDisk["VolumeSerialNumber"]}"));
            }

            //Ergebnis zurückgeben aller Laufwerke als DriveInformation
            return erg;
        }

        public override string ToString()
        {
            return $"{deviceID}\t{volumeSerialNumber}\t{(size / 1024 / 1024 / 1024):F2}\t{(freeSpace / 1024 / 1024 / 1024):F2}\t{(usedSpace / 1024 / 1024 / 1024):F2}\t{VolumeName}";
        }
    }
}
