using MovieServiceClient.MovieService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ClientWPF.Helpers
{
    public class SeanceData
    {
        [DisplayName("Tytuł")]
        public string Title { get; set; }
        [DisplayName("Godzina seansu")]
        public DateTime SeanceDate { get; set; }
        [DisplayName("Czy są wolne miejsca")]
        public bool AreFreePosition { get; set; }
        [DisplayName("Identyfikator filmu")]
        public int MovieID { get; set; }
        [DisplayName("Numer sali")]
        public int SalaID { get; set; }
        public SeanceData(SeanceModel model)
        {
            MovieID = model.MovieID;
            Title = model.MovieInfo.Title;
            SeanceDate = model.SeanceDate;
            SalaID = model.CinemaHallID;
            AreFreePosition = ComplexQueries.AreFreePositions(model.SeanceID, model.CinemaHallID);
        }
    }
}
