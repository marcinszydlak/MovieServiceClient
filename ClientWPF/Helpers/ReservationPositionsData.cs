using MovieServiceClient.MovieService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace ClientWPF.Helpers
{
    public class ReservationPositionData
    {
        [DisplayName("Numer rezerwacji")]
        public int ReservationID { get; set; }
        [DisplayName("Numer pozycji na rezerwacji")]
        public int ReservationPositionID { get; set; }
        [DisplayName("Numer seansu")]
        public int SeanceID { get; set; }
        [DisplayName("Numer filmu")]
        public int MovieID { get; set; }
        [DisplayName("Numer sali")]
        public int HallID { get; set; }
        [DisplayName("Tytuł filmu")]
        public string Title { get; set; }
        [DisplayName("Termin seansu")]
        public DateTime SeanceTime { get; set; }
        [DisplayName("Rząd")]
        public int Row { get; set; }
        [DisplayName("Miejsce")]
        public int Position { get; set; }
        public ReservationPositionData(ReservationModel model,int row,int position)
        {
            HallID = model.SeanceInfo.CinemaHallID;
            ReservationID = model.ReservationID;
            SeanceID = model.SeanceID;
            Title = model.SeanceInfo.MovieInfo.Title;
            MovieID = model.SeanceInfo.MovieID; 
            SeanceTime = model.SeanceInfo.SeanceDate;
            Row = row;
            Position = position;
            ReservationPositionID = model.ReservationInfo.Where(x => x.Row == row && x.Position == position).Select(x => x.ReservationPositionID).FirstOrDefault();
            
        }
    }
}
