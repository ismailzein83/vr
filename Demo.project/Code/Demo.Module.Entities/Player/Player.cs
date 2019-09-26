using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Player
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public int Number { get; set; }
        public PlayerType Type { get; set; }
    }

    public abstract class PlayerType
    {
        public abstract Guid ConfigID { get; }
        public string Nationality { get; set; }
    }
}