
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.CommonLibrary;





namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public partial class Direction
    {
        public static List<Direction> GetAll()
        {
            List<Direction> directions = new List<Direction>();
            try 
            {
                using(MobileEntities context = new MobileEntities())
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
