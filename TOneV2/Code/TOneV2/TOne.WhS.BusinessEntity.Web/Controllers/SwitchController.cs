using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
	[JSONWithTypeAttribute]
	[RoutePrefix(Constants.ROUTE_PREFIX + "Switch")]
	public class WhSBE_SwitchController : BaseAPIController
	{
		SwitchManager _manager = new SwitchManager();


		[HttpPost]
		[Route("GetFilteredSwitches")]
		public object GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input)
		{
            return GetWebResponse(input, _manager.GetFilteredSwitches(input), "Switches");
		}

		[HttpGet]
		[Route("GetSwitch")]
		public Switch GetSwitch(int switchId)
		{
			return _manager.GetSwitch(switchId);
		}

		[HttpGet]
		[Route("GetSwitchesInfo")]
		public IEnumerable<SwitchInfo> GetSwitchesInfo(string serializedFilter = null)
		{
			var deserializedFilter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<SwitchFilter>(serializedFilter) : null;
			return _manager.GetSwitchesInfo(deserializedFilter);
		}

		[HttpPost]
		[Route("AddSwitch")]
		public InsertOperationOutput<SwitchDetail> AddSwitch(Switch whsSwitch)
		{
			return _manager.AddSwitch(whsSwitch);
		}

		[HttpPost]
		[Route("UpdateSwitch")]
		public UpdateOperationOutput<SwitchDetail> UpdateSwitch(SwitchToEdit whsSwitch)
		{
			return _manager.UpdateSwitch(whsSwitch);
		}


		[HttpGet]
		[Route("DeleteSwitch")]
		public DeleteOperationOutput<SwitchDetail> DeleteSwitch(int switchId)
		{
			return _manager.DeleteSwitch(switchId);
		}

		[HttpGet]
		[Route("ResetSwitchSyncData")]
		public void ResetSwitchSyncData(string switchId)
		{
			_manager.ResetSwitchSyncData(switchId);
		}
	}
}