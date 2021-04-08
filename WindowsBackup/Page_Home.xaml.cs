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
        string backupSuccessLog = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager\\backupSuccessLog.txt";

        public Page_Home()
        {
            InitializeComponent();

            if (File.Exists(backupSuccessLog))
            {
                LoadPage(ref sp_DiagramsIsSuccessfull);
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

            LoadPage(ref sp_DiagramsIsSuccessfull);



            if (result == MessageBoxResult.Yes )
            {
                Process.Start("shutdown.exe", "-s -t 60");
            }
        }


        private void LoadPage(ref StackPanel stack)
        {
            string[] backupLog = File.ReadAllText(backupSuccessLog).Split('\n');


            foreach (string log in backupLog)
            {
                if (log.Contains("True"))
                {
                    AddCanvas(ref stack, log.Split(' ')[0].Replace(".", ""), true);
                }
                else
                {
                    AddCanvas(ref stack, log.Split(' ')[0].Replace(".", ""), false);
                }


            }

        }

        private static void AddCanvas(ref StackPanel stackPanel, string date, bool isSuccess)
        {

            //Farbe für Canvas auswählen
            Brush canvasBrush;
            string toolTip;

            //Ordner in Datum mit Punkten umwandeln
            date = date.Substring(0, 2) + "." + date.Substring(2, 2) + "." + date.Substring(4);

            if (isSuccess)
            {
                canvasBrush = Brushes.Green;

                toolTip = $"Die Sicherung am {date} wurde erfolgreich erstellt.";
            }
            else
            {
                canvasBrush = Brushes.Red;
                toolTip = $"Die Sicherung am {date} ist fehlgeschlagen.";
            }


            TransformGroup transformGroup = new TransformGroup();

            ScaleTransform scaleTransform = new ScaleTransform();
            SkewTransform skewTransform = new SkewTransform();
            RotateTransform rotateTransform = new RotateTransform();
            rotateTransform.Angle = -90.001;
            TranslateTransform translateTransform = new TranslateTransform();

            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(skewTransform);
            transformGroup.Children.Add(rotateTransform);
            transformGroup.Children.Add(translateTransform);


            TextBlock textBlock = new TextBlock();
            textBlock.Text = date;
            textBlock.RenderTransformOrigin = new Point(0.85, 2.6);
            textBlock.RenderTransform = transformGroup;


            Canvas canvas = new Canvas();
            canvas.Height = 100;
            canvas.Width = 30;
            canvas.Background = canvasBrush;
            canvas.HorizontalAlignment = HorizontalAlignment.Left;
            canvas.ToolTip = toolTip;
            canvas.Margin = new Thickness(20, 0, 0, 0);

            canvas.Children.Add(textBlock);

            stackPanel.Children.Add(canvas);
        }
    }
}
