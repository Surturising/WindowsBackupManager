using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MoveBackup
{
    class MoveBackup
    {
        static int Main(string[] args)
        {
            if (args.Contains("/Backup:") && args.Length == 2)
            {
                try
                {
                    DirectoryInfo backupFolder = new DirectoryInfo(args[1]);

                    backupFolder.MoveTo(backupFolder.Root.FullName + "\\WindowsImageBackup");

                    Console.WriteLine($"BackupFolder {backupFolder.Name} wurde erfolgreich in das Routverzeinis kopiert.");

                    return 0;
                }
                catch (Exception)
                {
                    Console.WriteLine("Beim Kopiervorgang ist etwas schiefgegangen");
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
