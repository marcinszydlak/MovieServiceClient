using MovieServiceClient.MovieService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ClientWPF.Helpers
{
    public class ReservationData
    {
        [DisplayName("Numer rezerweacji")]
        public int ReservationID { get; set; }
        [DisplayName("Numer seansu")]
        public int SeanceID { get; set; }
        [DisplayName("Godzina")]
        public DateTime Time { get; set; }
        [DisplayName("Film")]
        public string Movie { get; set; }
        [DisplayName("Miejsca")]
        public string Posiion { get; set; }
        public ReservationData(ReservationModel model)
        {
            ReservationID = model.ReservationID;
            SeanceID = model.SeanceID;
            Time = model.SeanceInfo.SeanceDate;
            Movie = model.SeanceInfo.MovieInfo.Title;
            StringBuilder builder = new StringBuilder();

            foreach (ReservationPositionModel r in model.ReservationInfo)
            {
                builder.Append($"({r.Row},{r.Position})");
            }
            Posiion = builder.ToString();

        }
    }
}
