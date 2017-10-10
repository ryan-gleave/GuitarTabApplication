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
using System.Xml;

namespace GuitarTabApplication
{
    /// <summary>
    /// Interaction logic for TabView.xaml
    /// </summary>
    public partial class TabView : UserControl
    {
        private Tab tab;

        public TabView()
        {
            InitializeComponent();
        }
        
        public TabView(Tab tab) //used by file loading
        {
            InitializeComponent();
            rtbContent.Document.Blocks.Clear();
            rtbContent.Document.Blocks.Add(new Paragraph(new Run(tab.Content)));
            testText.Text = tab.Title + " by " + tab.Artist + " @ " + tab.URL;

            this.tab = tab;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //todo make this a binding so when changes are made the tab.Content variable to automatically updated
            tab.Content = new TextRange(rtbContent.Document.ContentStart, rtbContent.Document.ContentEnd).Text;

            //if artist/title is not available then ask the user to provide this information
            //save content and artist and title to xml file
            try
            {
                string filename = "test2.tab.xml";
                if (tab.Filename != "") filename = tab.Filename;
                XmlWriter writer = XmlWriter.Create(filename);
                writer.WriteStartDocument();
                writer.WriteStartElement("tab");
                writer.WriteElementString("title", tab.Title);
                writer.WriteElementString("artist", tab.Artist);
                writer.WriteElementString("url", tab.URL);
                writer.WriteElementString("content", tab.Content);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                MessageBox.Show("Saved tab to file");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("Something went wrong trying to save this file");
            }
        }
    }
}
