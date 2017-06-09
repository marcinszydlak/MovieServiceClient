using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF.Helpers
{
    public class PositionData
    {
        public int Row { get; set; }
        public int Position { get; set; }
        public override string ToString()
        {
            return "Rząd : " + Row + ", Miejsce : " + Position;
        }
    }
}
