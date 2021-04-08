using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace WindowsBackup
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string xmlUserdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager";
            string mountVHDPSScript = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager" + "\\Mount-VHDXBackup.ps1";
            string copyVHDBackupScript = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager" + "\\Move-VHDXBackup.ps1";

            if (Directory.Exists(xmlUserdataFolder) == false)
            {
                Directory.CreateDirectory(xmlUserdataFolder);
            }

            if (File.Exists(mountVHDPSScript) == false)
            {
                File.Copy("..\\..\\Scripts\\Mount-VHDXBackup.ps1", mountVHDPSScript);
            }

            Frame.Content = new Page_Home();
            SetMenuBox(btn_Home);
        }

        #region SideMenu
        private void btn_Home_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = new Page_Home();
            SetMenuBox(btn_Home);
        }

        private void btn_Festplattenverwaltung_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = new Page_Drives();
            SetMenuBox(btn_Festplattenverwaltung);
        }

        private void btn_Zeitplan_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = new Page_Schedule();
            SetMenuBox(btn_Zeitplan);
        }

        private void btn_Mail_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = new Page_Mail();
            SetMenuBox(btn_Mail);
        }

        private void btn_Ruecksicherung_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = new Page_Restore();
            SetMenuBox(btn_Ruecksicherung);
        }

        /// <summary>
        /// Läd die jeweilige Seite und färbt den Button entsprechend ein
        /// </summary>
        /// <param name="button"></param>
        private void SetMenuBox(Button button)
        {


            foreach (Button item in stack_MainMenu.Children)
            {
                item.Background = Brushes.LightGray;
            }

            button.Background = Brushes.LightBlue;
        }

        #endregion

        //Schließen der Anwendung durch Exit Button
        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


    }
}
