using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionSalles
{
    internal class Room
    {
        string Label;
        int Capacity;

        public Room(string label, int capacity)
        {
            this.Label = label;
            this.Capacity = capacity;
        }
    }
}
