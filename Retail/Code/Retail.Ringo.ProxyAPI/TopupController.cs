using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Ringo.ProxyAPI
{
    public class TopupController
    {
        TopupManager _topupManager = new TopupManager();
        public AddTopupOutput AddTopup(AddTopupInput input)
        {
            return _topupManager.AddTopup(input);
        }
    }
}
