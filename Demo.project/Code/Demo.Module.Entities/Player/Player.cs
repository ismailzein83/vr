using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Player
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public PlayerType Type { get; set; }
    }

    public abstract class PlayerType
    {
        public abstract Guid ConfigID { get; }

        public string Nationality { get; set; }
    }
}