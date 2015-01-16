using System;
using System.Collections.Generic;
using System.Text;

namespace TABS.Components
{
    [Serializable]
    public abstract class FlaggedServicesEntity : BaseEntity
    {
        protected short _ServicesFlag;
        protected NotifierSet<FlaggedService> _FlaggedServices = new NotifierSet<FlaggedService>();
        public event EventHandler ServicesFlagChanged;

        public virtual short ServicesFlag
        {
            get
            {
                return _ServicesFlag;
            }
            set
            {
                //if (_ServicesFlag != value)
                //{
                    _ServicesFlag = value;
                    FlaggedService.UpdateServicesFromFlag(_ServicesFlag, _FlaggedServices);
                    if (ServicesFlagChanged != null) ServicesFlagChanged(this, EventArgs.Empty);
                //}
            }
        }

        public FlaggedServicesEntity()
        {
            // When the related Services Are updated, Recalculate the Flag
            _FlaggedServices.CollectionChanged += new Components.NotifierSet<FlaggedService>.CollectionChangedDelegate(_FlaggedServices_CollectionChanged);
        }
        ~FlaggedServicesEntity()
        {
            //Huseinali
            //System.Diagnostics.Debug.WriteLine("Publisher FlaggedServicesEntity is disposed ~FlaggedServicesEntity");
            _FlaggedServices.CollectionChanged -= _FlaggedServices_CollectionChanged;
        }
        protected virtual void _FlaggedServices_CollectionChanged(FlaggedService item, ItemChangeType changeType)
        {
            /*
            if (changeType == ItemChangeType.ItemAdded)
            {
                if (item.Equals(FlaggedService.Direct) || item.Equals(item == FlaggedService.Video))
                {
                    FlaggedServices.Add(FlaggedService.CLI);
                    FlaggedServices.Add(FlaggedService.Premium);
                    FlaggedServices.Add(FlaggedService.Retail);
                }
                else if (item.Equals(FlaggedService.CLI))
                {
                    FlaggedServices.Add(FlaggedService.Premium);
                    FlaggedServices.Add(FlaggedService.Retail);
                }
                else if (item.Equals(FlaggedService.Premium))
                {
                    FlaggedServices.Add(FlaggedService.Retail);
                }
            }
             */
            short value = FlaggedService.UpdateServicesFlag(FlaggedServices);
            if (_ServicesFlag != value && ServicesFlagChanged != null) ServicesFlagChanged(this, EventArgs.Empty);
            _ServicesFlag = value;
        }

        /// <summary>
        /// Gets the services this Account has for his zones (and rates) by default.
        /// </summary>
        public virtual Iesi.Collections.Generic.ISet<FlaggedService> FlaggedServices
        {
            get
            {
                return _FlaggedServices;
            }
        }

        public static string GetServicesDisplayList(short serviceFlag)
        {
            if (serviceFlag == 0) return FlaggedService.All[0].Symbol;
            StringBuilder sb = new StringBuilder();
            foreach (FlaggedService service in FlaggedService.All.Values)
                if ((serviceFlag & service.FlaggedServiceID) == service.FlaggedServiceID)
                {
                    if (sb.Length > 0) sb.Append(", ");
                    sb.Append(service.Symbol);
                }
            return sb.ToString();
        }

        public string Services
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (FlaggedService service in FlaggedServices)//if (service.FlaggedServiceID > 0)
                    {
                        if (sb.Length > 0) sb.Append(", ");
                        sb.Append(service.Symbol);
                    }
                return sb.ToString();
            }
        }

        public string DominantServices
        {
            get
            {
                List<FlaggedService> childServices = new List<FlaggedService>();
                List<FlaggedService> dominantServices = new List<FlaggedService>(this.FlaggedServices);

                foreach (FlaggedService possibleChild in dominantServices)
                    foreach (FlaggedService parent in dominantServices)
                        if (parent.FlaggedServiceID > possibleChild.FlaggedServiceID)
                            if ((parent.FlaggedServiceID & possibleChild.FlaggedServiceID) == possibleChild.FlaggedServiceID)
                                childServices.Add(possibleChild);

                foreach (FlaggedService orphanService in childServices)
                    dominantServices.Remove(orphanService);

                StringBuilder sb = new StringBuilder();
                foreach (FlaggedService service in dominantServices)
                    if (service.FlaggedServiceID > 0)
                    {
                        if (sb.Length > 0) sb.Append(", ");
                        sb.Append(service.Symbol);
                    }

                return sb.ToString() == string.Empty ? "WHS" : sb.ToString();
            }
        }

    }
}