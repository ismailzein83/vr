
app.service('BusinessProcess_TaskTypeActionService', ['VRNotificationService', 'BusinessProcess_BPTaskAPIService', 'WhS_BP_ExecuteBPTaskResultEnum', 'VRCommon_VRTempPayloadAPIService', 'InsertOperationResultEnum',
	function (VRNotificationService, BusinessProcess_BPTaskAPIService, WhS_BP_ExecuteBPTaskResultEnum, VRCommon_VRTempPayloadAPIService, InsertOperationResultEnum) {

		var actionTypes = [];

		function registerActionType(actionType) {
			actionTypes.push(actionType);
		}

		function getActionTypeIfExist(actionTypeName) {
			for (var i = 0; i < actionTypes.length; i++) {
				var actionType = actionTypes[i];
				if (actionType.ActionTypeName == actionTypeName)
					return actionType;
			}
		}

		function registerExecuteAction() {
			var actionType = {
				ActionTypeName: "Execute",
				actionMethod: function (payload) {
					var input = {
						$type: "Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput, Vanrise.BusinessProcess.Entities",
						TaskId: payload.taskId,
						TaskData: {
							$type: "Vanrise.BusinessProcess.MainExtensions.BPTaskTypes.BPGenericTaskData, Vanrise.BusinessProcess.MainExtensions",
							FieldValues: payload.fieldValues
						},
						Notes: payload.notes,
						Decision: payload.decision
					};
					return BusinessProcess_BPTaskAPIService.ExecuteTask(input).then(function (response) {
						if (response != undefined && response.Result == WhS_BP_ExecuteBPTaskResultEnum.Failed.value) {
							VRNotificationService.showError(response.OutputMessage);
						}
						else
							payload.context.closeModal();
					});
				}
			};
			registerActionType(actionType);
		}
		function registerOpenRDLCReportAction() {
			var actionType = {
				ActionTypeName: "OpenRDLCReport",
				actionMethod: function (payload) {
					var openRDLCTempPayload = {
						Settings: {
							$type: "Vanrise.BusinessProcess.Entities.OpenRDLCReportVRTempPayload,Vanrise.BusinessProcess.Entities"
						}
					};
					openRDLCTempPayload.Settings.CurrencyId = payload.fieldValues.Currency;
					openRDLCTempPayload.Settings.Amount = payload.fieldValues.Amount;
					openRDLCTempPayload.Settings.ReceivedTime = payload.fieldValues.ReceivedTime;
					openRDLCTempPayload.Settings.ReceivedBy = payload.fieldValues.ReceivedBy;
					openRDLCTempPayload.Settings.Customerid = payload.fieldValues.Customer;
					openRDLCTempPayload.Settings.CheckNumber = payload.fieldValues.CheckNumber;
					openRDLCTempPayload.Settings.PaymentType = payload.fieldValues.PaymentType;
					return VRCommon_VRTempPayloadAPIService.AddVRTempPayload(openRDLCTempPayload).then(function (response) {
						if (response.Result == InsertOperationResultEnum.Succeeded.value) {
							var tempPayloadId = response.InsertedObject;
							var paramsurl = "";
							paramsurl += "tempPayloadId=" + tempPayloadId;
							var screenWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
							var left = ((screenWidth / 2) - (1000 / 2));
							window.open("Client/Modules/BusinessProcess/Reports/OpenRDLCReport.aspx?" + paramsurl, "_blank", "toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=yes, copyhistory=no,width=1000, height=600,scrollbars=1 , top = 40, left = " + left + "");
						}
					}).catch(function (error) {
					});
				}
			};
			registerActionType(actionType);
		}

		return ({
			registerActionType: registerActionType,
			getActionTypeIfExist: getActionTypeIfExist,
			registerExecuteAction: registerExecuteAction,
			registerOpenRDLCReportAction: registerOpenRDLCReportAction
		});
	}]);
