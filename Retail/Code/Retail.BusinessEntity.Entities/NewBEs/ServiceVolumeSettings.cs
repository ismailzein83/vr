//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Retail.BusinessEntity.Entities
//{
//    public abstract class VolumeSettings
//    {
//        public abstract VolumeBalance CreateBalance(IVolumeCreateBalanceContext context);
//        public abstract void UpdateVolume(IVolumeUpdateBalanceContext context);
//    }

//    public abstract class VolumeBalance
//    {

//    }

//    public interface IVolumeCreateBalanceContext
//    {

//    }

//    public interface IVolumeUpdateBalanceContext
//    {
//        VolumeBalance Balance { get; }

//        Decimal CallDuration { get; }
//    }
//}
