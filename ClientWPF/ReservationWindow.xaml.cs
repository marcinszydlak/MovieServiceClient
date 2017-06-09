using ClientWPF.Helpers;
using MovieServiceClient.MovieService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for OknoRezerwacji.xaml
    /// </summary>
    public partial class OknoRezerwacji : Window
    {
        SeanceData DaneSeansu { get; set; }
        CinemaHallModel DaneSali { get; set; }
        ObservableCollection<PositionData> MiejscaDoZarezerwowania { get; set; }
        MainWindow parent;
        int ostatni = 1;
        public OknoRezerwacji()
        {
            InitializeComponent();
        }

        public OknoRezerwacji(MainWindow parent, SeanceData dane) : this()
        {
            this.parent = parent;
            DaneSeansu = dane;
            TytulLabel.Content = DaneSeansu.Title;
            DataLabel.Content = DaneSeansu.SeanceDate;
            SalaLabel.Content = "Sala nr " + DaneSeansu.SalaID;
            using (ClientServiceClient cs = new ClientServiceClient())
            {
                DaneSali = cs.GetCinemaHall(DaneSeansu.SalaID);
            }
            List<int> numeryRzedow = new List<int>();
            for (int i = 1; i <= DaneSali.Rows; i++)
            {
                numeryRzedow.Add(i);
            }
            List<int> numeryMiejsc = new List<int>();
            for (int i = 1; i <= DaneSali.Positions; i++)
            {
                numeryMiejsc.Add(i);
            }
            RządComboBox.ItemsSource = numeryRzedow;
            MiejsceComboBox.ItemsSource = numeryMiejsc;
            MiejscaDoZarezerwowania = new ObservableCollection<PositionData>();
            Rezerwacje.ItemsSource = MiejscaDoZarezerwowania;

        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            bool daneOK = true;

            int count = MiejscaDoZarezerwowania.Count();
            int countDistinct = MiejscaDoZarezerwowania.Distinct().Count();
            if (count == countDistinct)
            {
                try
                {
                    using (ClientServiceClient cs = new ClientServiceClient())
                    {
                        List<ReservationPositionModel> res = cs.GetActualCinemaHallState(DaneSeansu.SalaID).ToList();

                        foreach (PositionData pos in MiejscaDoZarezerwowania)
                        {
                            if (res.Where(x => x.Position == pos.Position && x.Row == pos.Row).FirstOrDefault() == null)
                            {
                                cs.AddReservationSinglePosition(DaneSeansu.SalaID, parent.loggedUser.ClientID, pos.Row, pos.Position);
                            }
                        }
                    }
                    this.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            
        }

        private void DodajDoRezerwacji_Click(object sender, RoutedEventArgs e)
        {
            if (RządComboBox.SelectedItem != null && MiejsceComboBox.SelectedItem != null)
            {
                int r = (int)RządComboBox.SelectedItem;
                int p = (int)MiejsceComboBox.SelectedItem;
                if (MiejscaDoZarezerwowania.Where(x => x.Row == r && x.Position == p).FirstOrDefault() == null)
                {
                    MiejscaDoZarezerwowania.Add(new PositionData()
                    {
                        Row = r,
                        Position = p,
                    });
                }
                RządComboBox.SelectedItem = null;
                MiejsceComboBox.SelectedItem = null;
                Rezerwacje.UpdateLayout();
            }
        }
    }
}
