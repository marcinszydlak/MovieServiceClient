using ClientWPF.Helpers;
using MovieServiceClient.MovieService;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ClientServiceClient cs;
        public List<SeanceModel> seances;
        public List<SeanceData> seanceDatas;
        public List<MovieModel> movieModels;
        public ClientModel loggedUser;
        public List<ReservationModel> reservations;
        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            LoginWindow login = new LoginWindow(this);
            login.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MainTabControl.IsEnabled = false;
            login.ShowDialog();
            if (loggedUser != null)
            {
                MainTabControl.IsEnabled = true;
                UzytkownikLabel.Content = $"{loggedUser.Login} ({loggedUser.Name} {loggedUser.Surname})";
            }
            else
            {
                this.Close();
            }
            
        }

        private void ZatwierdzWyszukiwanieButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<SeanceData> data = new List<SeanceData>();
                if (!string.IsNullOrEmpty(TitleTextBox.Text) && DataOdDatePicker.SelectedDate.HasValue && DataDoDatePicker.SelectedDate.HasValue)
                {
                    if (DataDoDatePicker.SelectedDate.Value > DataOdDatePicker.SelectedDate.Value)
                    {
                        seances = cs.GetSeancesByTitle(TitleTextBox.Text).Where(x => x.SeanceDate > DataOdDatePicker.SelectedDate.Value && x.SeanceDate < DataDoDatePicker.SelectedDate.Value).ToList();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(TitleTextBox.Text))
                    {
                        seances = cs.GetSeancesByTitle(TitleTextBox.Text).ToList();
                    }
                }

                foreach (SeanceModel model in seances)
                {
                    data.Add(new SeanceData(model));
                }
                movieModels = new List<MovieModel>();
                foreach (int i in data.Select(x => x.MovieID).Distinct().ToList())
                {
                    movieModels.Add(cs.GetMovie(i));
                }
                ZnalezioneSeanseDataGrid.ItemsSource = data;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ZnalezioneSeanseDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            System.ComponentModel.PropertyDescriptor pd = (System.ComponentModel.PropertyDescriptor)e.PropertyDescriptor;
            e.Column.IsReadOnly = true;
            e.Column.Header = pd.DisplayName;
            if (pd.DisplayName == "Identyfikator filmu" || pd.DisplayName == "Numer sali")
            {
                e.Column.Visibility = Visibility.Hidden;
            }
        }

        private void ZnalezioneSeanseDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id = ((SeanceData)((DataGrid)sender).SelectedItem).MovieID;
            MovieModel model = movieModels.Where(x => x.MovieID == id).First();
            TytulFilmuDane.Content = model.Title;
            RezyserDane.Content = model.Regisseur;
            RokWydaniaDane.Content = model.PublicationDate;
            OcenaDane.Content = model.Note;

            try
            {
                using (var ms = new MemoryStream(model.ImageContent))
                {
                    Image m = new Image();
                    m.DataContext = ms;
                }
            }
            catch(Exception)
            {

            }
            
        }

        private void Rezerwuj_Click(object sender, RoutedEventArgs e)
        {
            if (ZnalezioneSeanseDataGrid.SelectedItem != null)
            {
                OknoRezerwacji okno = new OknoRezerwacji(this,(SeanceData)ZnalezioneSeanseDataGrid.SelectedItem);
                okno.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                okno.ShowDialog();
            }
        }


        private void Wyszukaj_Click(object sender, RoutedEventArgs e)
        {
            using (ClientServiceClient cs = new ClientServiceClient())
            {
                reservations = cs.GetReservations(loggedUser.ClientID).ToList();
                RezerwacjeDataGrid.ItemsSource = reservations;
            }
        }
    }
}
