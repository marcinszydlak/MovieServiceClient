using ClientWPF.Helpers;
using MovieServiceClient.MovieService;
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
using System.Windows.Shapes;

namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for AktualizujRezerwacje.xaml
    /// </summary>
    public partial class AktualizujRezerwacje : Window
    {
        private int salaID { get; set; }
        private List<int> miejsca = new List<int>();
        private List<int> rzedy = new List<int>();
        ReservationPositionData pos;
        public AktualizujRezerwacje()
        {
            InitializeComponent();
        }
        public AktualizujRezerwacje(ReservationPositionData pos):this()
        {
            this.pos = pos;
            using (ClientServiceClient cs = new ClientServiceClient())
            {
                CinemaHallModel temp = cs.GetCinemaHall(pos.HallID);
                for (int i = 0; i < temp.Rows; i++)
                {
                    rzedy.Add(i);
                }
                for (int i = 0; i < temp.Positions; i++)
                {
                    miejsca.Add(i);
                }
                
                RowComboBox.ItemsSource = rzedy;
                PositionComboBox.ItemsSource = miejsca;
                RowComboBox.SelectedItem = pos.Row;
                PositionComboBox.SelectedItem = pos.Position;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (ClientServiceClient cs = new ClientServiceClient())
                {
                    pos.Position = (int)PositionComboBox.SelectedItem;
                    pos.Row = (int)RowComboBox.SelectedItem;
                    cs.UpdateReservation(pos.ReservationPositionID, pos.Row, pos.Position);
                }
            }
            catch(Exception ex)
            {

            }
            finally
            {
                this.Close();
            }
        }
    }
}
