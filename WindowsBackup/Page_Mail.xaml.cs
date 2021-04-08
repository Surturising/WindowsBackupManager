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
using System.Xml.Serialization;
using System.Net.Mail;
using System.Net;

namespace WindowsBackup
{
    /// <summary>
    /// Interaktionslogik für Page_Mail.xaml
    /// </summary>
    public partial class Page_Mail : Page
    {
        //Membervariablen

        private string empfaenger = "empfaenger@mail.de";
        string xmlUserdataMail = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WindowsBackupManager\\userdata_mail.xml";


        public Page_Mail()
        {
            InitializeComponent();


            if (File.Exists(xmlUserdataMail))
            {
                XmlSerializer xs = new XmlSerializer(typeof(XML_Mail));
                FileStream read = new FileStream(xmlUserdataMail, FileMode.Open, FileAccess.Read, FileShare.Read);

                XML_Mail daten = (XML_Mail)xs.Deserialize(read);

                tb_Absender.Text = daten.Absender;
                tb_Smtp.Text = daten.Smtp;
                tb_Username.Text = daten.Username;

                foreach (string newEmpfaenger in daten.empfaenger)
                {
                    TextBlock text = new TextBlock();
                    text.Text = newEmpfaenger;
                    lb_Empfaenger.Items.Add(text);
                }
                pwb_Passwort.Password = daten.Password;
            }

            }

        #region Buttons

        private void bt_Hinzufuegen_Click(object sender, RoutedEventArgs e)
        {
            TextBlock textblock = new TextBlock();
            string newEmfaenger = lbi_NewEmpfaenger.Text;

            //Testen ob Email richtig ist
            if (newEmfaenger.Contains('@') && newEmfaenger.Split('@')[1].Contains('.'))
            {
                //Hinzufügen des neuen E-Mail Empfängers.
                textblock.Text = lbi_NewEmpfaenger.Text;
                lb_Empfaenger.Items.Add(textblock);
                lbi_NewEmpfaenger.Text = empfaenger;
            }
            else
            {
                MessageBox.Show("Ungültige Mail Adresse", "Fehler beim hinzufügen eines neuen Emfängers", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void bt_Entfernen_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Empfaenger.SelectedItem != lbi_NewEmpfaenger)
            {
                lb_Empfaenger.Items.Remove(lb_Empfaenger.SelectedItem);
            }
        }

        private void lbi_NewEmpfaenger_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (lbi_NewEmpfaenger.Text == empfaenger)
            {
                lbi_NewEmpfaenger.Text = "";
            }

        }

        private void bt_Save_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                #region Speichern der Daten in XML
                XML_Mail daten = new XML_Mail();

                daten.Absender = tb_Absender.Text;

                daten.Smtp = tb_Smtp.Text;
                daten.Username = tb_Username.Text;

                //Empfänger speichern hinzufügen
                foreach (object item in lb_Empfaenger.Items)
                {
                    if (item is TextBlock)
                    {
                        daten.empfaenger.Add((item as TextBlock).Text);
                    }
                }

                //Passwort speichern

                daten.Password = pwb_Passwort.Password;

                SaveXMLData.SaveXML(daten, xmlUserdataMail);

                #endregion

                #region Testen E-Mailsendung

                NetworkCredential login = new NetworkCredential(tb_Username.Text, pwb_Passwort.Password);

                foreach (string empfaenger in daten.empfaenger)
                {

                    MailMessage msg = new MailMessage(tb_Absender.Text, empfaenger, "Test E-Mail Sicherung", "Das ist ein erster test. Bitte anschnallen!");
                    msg.BodyEncoding = Encoding.UTF8;
                    msg.SubjectEncoding = Encoding.UTF8;
                    msg.IsBodyHtml = true;

                    SmtpClient client = new SmtpClient(tb_Smtp.Text);
                    client.Credentials = login;
                    client.EnableSsl = true;
                    client.Send(msg);
                }

                #endregion

                MessageBox.Show("Daten gespeichert\nBitte überprüfen Sie ihren E-Mail Posteingang");
            }
            catch (System.Net.Mail.SmtpException)
            {

                MessageBox.Show("Für den SMTP-Server ist eine sichere Verbindung erforderlich,\n oder der Client wurde nicht authentifiziert.\nDie Serverantwort war: 5.7.0 Authentication Required.", "Fehler E-Mail", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show("Prozess kann nicht auf die userdata_mail.xml zugreifen. Bitte noch einmal auf 'Einstellungen speichern' klicken!", "Fehler beim Speichern der Maildaten", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch(Exception)
            {
                MessageBox.Show("Unbekannter Fehler", "Unbekannter Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #endregion


    }
}
