using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace GuitarTabApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<UserControl> previous = new List<UserControl>();
        private UserControl selectedViewModel = null;
        public UserControl SelectedViewModel
        {
            get { return selectedViewModel; }
            set
            {
                if (selectedViewModel != null)
                    previous.Add(selectedViewModel);
                selectedViewModel = value;
                value.DataContext = this;
                btnBack.IsEnabled = previous.Count > 0;
                OnPropertyChanged("SelectedViewModel");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            SelectedViewModel = new MainView();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (previous != null && previous.Count > 0)
            {
                UserControl prev = previous.Last();
                previous.Remove(prev);
                SelectedViewModel = prev;
                previous.Remove(previous.Last());
                btnBack.IsEnabled = previous.Count > 0;
            }
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
