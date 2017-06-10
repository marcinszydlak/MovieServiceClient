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
        public List<MovieModel> reservedMovieModel;
        List<ReservationPositionData> reservationData = new List<ReservationPositionData>();
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }



        private void ZnalezioneSeanseDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var q = ((SeanceData)((DataGrid)sender).SelectedItem);
            if (q != null)
            {
                int id = q.MovieID;
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
                catch (Exception)
                {

                }
            }
        }

        private void Rezerwuj_Click(object sender, RoutedEventArgs e)
        {
            if (ZnalezioneSeanseDataGrid.SelectedItem != null)
            {
                if (cs.GetReservations(loggedUser.ClientID).Where(x => x.SeanceID == ((SeanceData)(ZnalezioneSeanseDataGrid.SelectedItem)).SeanceID).FirstOrDefault() == null)
                {
                    OknoRezerwacji okno = new OknoRezerwacji(this, (SeanceData)ZnalezioneSeanseDataGrid.SelectedItem);
                    okno.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    okno.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Masz już rezerwacje na ten film");
                }
            }
        }

        private void Wyszukaj_Click(object sender, RoutedEventArgs e)
        {
            using (ClientServiceClient cs = new ClientServiceClient())
            {
                reservations = cs.GetReservations(loggedUser.ClientID).ToList();
                List<ReservationData> daneORezewacjach = new List<ReservationData>();

                if (RezerwacjeOd.SelectedDate.HasValue && RezerwacjeDo.SelectedDate.HasValue)
                {

                    if (RezerwacjeOd.SelectedDate.Value > RezerwacjeOd.SelectedDate.Value)
                    {
                        RezerwacjeDo.SelectedDate = RezerwacjeOd.SelectedDate.Value.AddDays(1);
                    }
                    foreach (ReservationModel model in reservations.Where(x => x.ReservationDate > RezerwacjeOd.SelectedDate.Value && x.ReservationDate < RezerwacjeDo.SelectedDate.Value).ToList())
                    {
                        daneORezewacjach.Add(new ReservationData(model));
                    }
                }
                else
                {
                    foreach (ReservationModel model in reservations)
                    {
                        daneORezewacjach.Add(new ReservationData(model));
                    }
                }
                RezerwacjeDataGrid.ItemsSource = daneORezewacjach;
            }
        }



        private void AnulujButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult a = MessageBox.Show("Czy chcesz usunąć zaznaczoną pozycję na rezerwacji", "Usuwanie rezerwacji", MessageBoxButton.YesNo);
                if (a == MessageBoxResult.Yes)
                {
                    ReservationPositionData r = (ReservationPositionData)RezerwacjeAktywneDataGrid.SelectedItem;
                    if (r != null)
                    {
                        cs.RemoveReservation(r.ReservationID, r.Row, r.Position);
                    }
                    else
                    {
                        MessageBox.Show("Nie zaznaczyłeś rezerwacji do usunięcia");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("NIepowodzenie podczas usuwania rezerwacji");
            }
        }

        private void EdytujButton_Click(object sender, RoutedEventArgs e)
        {
            ReservationPositionData r = (ReservationPositionData)RezerwacjeAktywneDataGrid.SelectedItem;
            if (r != null)
            {
                AktualizujRezerwacje res = new AktualizujRezerwacje(r);
                res.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                res.ShowDialog();
                RezerwacjeAktywneDataGrid.UpdateLayout();
            }
        }

        private void OdswiezButton_Click(object sender, RoutedEventArgs e)
        {

            foreach (ReservationModel m in cs.GetReservations(loggedUser.ClientID))
            {
                reservationData = new List<ReservationPositionData>();
                if (m.SeanceInfo.SeanceDate > DateTime.Now)
                {
                    foreach (ReservationPositionModel p in m.ReservationInfo)
                    {
                        reservationData.Add(new ReservationPositionData(m, p.Row, p.Position));
                    }
                }
            }
            RezerwacjeAktywneDataGrid.ItemsSource = reservationData;
            AktywneRezerwacjeComboBox.ItemsSource = reservationData.Select(x => x.ReservationID).Distinct().ToList();
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

        private void RezerwacjeDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            System.ComponentModel.PropertyDescriptor pd = (System.ComponentModel.PropertyDescriptor)e.PropertyDescriptor;
            e.Column.IsReadOnly = true;
            e.Column.Header = pd.DisplayName;
            if (pd.DisplayName == "Numer seansu")
            {
                e.Column.Visibility = Visibility.Hidden;
            }
        }

        private void RezerwacjeAktywneDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            System.ComponentModel.PropertyDescriptor pd = (System.ComponentModel.PropertyDescriptor)e.PropertyDescriptor;
            e.Column.IsReadOnly = true;
            e.Column.Header = pd.DisplayName;
            if (pd.DisplayName == "Numer seansu")
            {
                e.Column.Visibility = Visibility.Hidden;
            }
        }

        private void AktywneRezerwacje_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var p = AktywneRezerwacjeComboBox.SelectedItem;
            if (p != null)
            {
                RezerwacjeAktywneDataGrid.ItemsSource = reservationData.Where(x => x.ReservationID == (int)p).ToList();
            }
            else
            {
                RezerwacjeAktywneDataGrid.ItemsSource = reservationData;
            }
        }

        private void AnulujCaleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult a = MessageBox.Show("Czy chcesz usunąć całą rezerwacje", "Usuwanie rezerwacji", MessageBoxButton.YesNo);
                if (a == MessageBoxResult.Yes)
                {
                    ReservationPositionData r = (ReservationPositionData)RezerwacjeAktywneDataGrid.SelectedItem;
                    if (r != null)
                    {
                        cs.RemoveReservation(r.ReservationID, null, null);
                    }
                    else
                    {
                        MessageBox.Show("Nie zaznaczyłeś rezerwacji do usunięcia");
                    }
                }
            }
            catch(Exception)
            {
                MessageBox.Show("NIepowodzenie podczas usuwania rezerwacji");
            }

        }

        private void RezerwacjeAktywneDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ReservationPositionData pd = (ReservationPositionData)RezerwacjeAktywneDataGrid.SelectedItem;
                if (pd != null)
                {
                    using (ClientServiceClient cs = new ClientServiceClient())
                    {
                        MovieModel q = cs.GetMovie(pd.MovieID);
                        SzczegolyTytulFilmu.Content = q.Title;
                        SzczegolyRezyser.Content = q.Regisseur;
                        SzczegolyRokWydania.Content = q.PublicationDate;
                        SzczegolyOcena.Content = q.Note;
                        SzczegolyZdjecie.DataContext = q.ImageContent;
                    }
                }
                else
                {
                    SzczegolyTytulFilmu.Content = "";
                    SzczegolyRezyser.Content = "";
                    SzczegolyRokWydania.Content = "";
                    SzczegolyOcena.Content = "";
                    SzczegolyZdjecie.DataContext = null;
                }
            }
            catch (Exception) { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Wyloguj_Click(object sender, RoutedEventArgs e)
        {
            loggedUser = null;
            this.Hide();
            LoginWindow window = new LoginWindow(this);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ShowDialog();
            if(loggedUser == null)
            {
                this.Close();
            }
        }
    }
}
