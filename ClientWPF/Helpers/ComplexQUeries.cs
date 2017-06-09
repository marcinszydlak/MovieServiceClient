using MovieServiceClient.MovieService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF.Helpers
{
    public static class ComplexQueries
    {
        public static ClientServiceClient cs = new ClientServiceClient();
        public static bool AreFreePositions(int seanceID,int hallID)
        {
            int count = cs.GetActualCinemaHallState(seanceID).Count();
            CinemaHallModel ch = cs.GetCinemaHall(hallID);
            return count < ch.Rows * ch.Positions;
        }
    }
}
