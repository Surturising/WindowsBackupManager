using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RemoveBackup
{
    class RemoveBackup
    {
        
        static int Main(string[] args)
        {

            string backupSuccessLog = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager\\backupSuccessLog.txt";

            if (args.Contains("/Backup:"))
            {
                try
                {
                    string[] backupPath = args[1].Split(';');
                    

                    foreach (string path in backupPath)
                    {
                        string newSuccessLog = "";
                        DirectoryInfo backup = new DirectoryInfo(path);

                        if (backup.Exists == true)
                        {
                            backup.Delete(true);

                            if (File.Exists(backupSuccessLog) == true)
                            {
                                //Durchsuche backupSuccessLog -> Lösche gelöschtes Backups aus backupSuccessLog

                                foreach (string successString in File.ReadAllLines(backupSuccessLog))
                                {
                                    string dateName = $"{backup.Name.Substring(0, 2)}.{backup.Name.Substring(2, 2)}.{backup.Name.Substring(4)}";

                                    if (!(successString.Contains($"{dateName}")))
                                    {
                                        newSuccessLog += successString + "\n";
                                    }

                                }

                                Console.WriteLine($"{newSuccessLog.Substring(0, newSuccessLog.Length - 1)}");
                                Console.ReadLine();

                                File.WriteAllText(backupSuccessLog, newSuccessLog.Substring(0, newSuccessLog.Length -1));
                            }

                            Console.WriteLine($"BackupFolder {backup.Name} wurde erfolgreich gelöscht.");
                        }
                    }

                    return 0;
                }
                catch (Exception)
                {
                    Console.WriteLine("Beim Löschvorgang ist etwas schiefgegangen");
                    return 1;
                }

            }
            else
            {
                Console.WriteLine("Falsche Syntax");
                return 1;
            }

        }
    }
}
