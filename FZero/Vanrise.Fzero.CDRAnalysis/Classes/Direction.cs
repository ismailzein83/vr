
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.CommonLibrary;





namespace Vanrise.Fzero.CDRAnalysis
{
    public partial class Direction
    {
        public static List<Direction> GetAll()
        {
            List<Direction> directions = new List<Direction>();
            try 
            {
                using(Entities context = new Entities())
                {
                    directions = context.Directions.ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Direction.GetAll()", err);
            }
            return directions;
        }
    }
}
