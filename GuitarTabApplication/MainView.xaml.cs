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
using System.Net;
using System.IO;
using System.Xml;

namespace GuitarTabApplication
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        List<Tab> tabs = new List<Tab>();

        public MainView()
        {
            InitializeComponent();

            //Get directory
            //enumerate through all files in the directory and read them in as tabs
            foreach (var f in Directory.EnumerateFiles(Directory.GetCurrentDirectory()))
            {
                if (!f.EndsWith(".tab.xml")) continue;

                Tab tab = new Tab();
                tab.Filename = f.Split('/').Last();

                XmlReader reader = XmlReader.Create(f);
                string currentElement = "";
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            currentElement = reader.Name;
                            break;
                        case XmlNodeType.Text:
                            if (currentElement.Equals("title"))
                            {
                                tab.Title = reader.Value;
                            }
                            if (currentElement.Equals("artist"))
                            {
                                tab.Artist = reader.Value;
                            }
                            if (currentElement.Equals("url"))
                            {
                                tab.URL = reader.Value;
                            }
                            if (currentElement.Equals("content"))
                            {
                                tab.Content = reader.Value;
                            }
                            break;
                    }
                }

                tabs.Add(tab);
            }

            lvTabs.ItemsSource = tabs;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Tab tab = new Tab();
                string webpage = GetHTML(txtUrl.Text);

                HtmlAgilityPack.HtmlDocument htdoc = new HtmlAgilityPack.HtmlDocument();
                htdoc.LoadHtml(webpage);
                
                //get tab content
                foreach (HtmlAgilityPack.HtmlNode n in htdoc.DocumentNode.SelectNodes("//pre"))
                {
                    HtmlAgilityPack.HtmlAttribute f = n.Attributes.FirstOrDefault(i => i.Name.Equals("class") && i.Value.Contains("js-tab-content"));
                    if (f != null)
                    {
                        tab.Content = n.InnerText;
                    }
                }

                //get tab title 
                //< div class="t_title"><div><h1 itemProp = "name" > Title </ h1 ></ div ></ div >
                tab.Title = htdoc.DocumentNode.Descendants("h1")
                    .FirstOrDefault(i => i.Attributes.Contains("itemProp") && i.Attributes["itemProp"].Value.Contains("name"))
                    .InnerText;

                //get tab artist (current implementation is kinda bad)
                //< div class="t_author"> by <a>innerText</a>
                tab.Artist = htdoc.DocumentNode.Descendants("div")
                    .FirstOrDefault(i => i.Attributes.Contains("class") && i.Attributes["class"].Value.Contains("t_autor"))
                    .Descendants("a").FirstOrDefault()
                    .InnerText;

                tab.URL = txtUrl.Text;

                //update view
                (this.DataContext as MainWindow).SelectedViewModel = new TabView(tab);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("An error occured while loading that URL");
            }
        }

        private string GetHTML(string urlAddress)
        {
            /* Used stackoverflow.com/questions/16642196/get-html-code-from-website-in-c-sharp */
            string data = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                data = readStream.ReadToEnd();
                readStream.Close();

                response.Close();
            }
            return data;
        }

        private void lvTabs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            ListView listView = (ListView)sender;
            if (listView.SelectedIndex == -1) return;

            Tab tab = (Tab)listView.SelectedItem;
            (this.DataContext as MainWindow).SelectedViewModel = new TabView(tab);
        }
    }
}
