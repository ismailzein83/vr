using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Business
{
    public class InteractionManager
    {

        #region Public Methods
        public List<Interaction> GetMessages()
        {
            IInteractionDataManager interactionDataManager = DemoModuleFactory.GetDataManager<IInteractionDataManager>();
            return interactionDataManager.GetMessages();
        }
        #endregion
    }
}
