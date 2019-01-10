(function (appControllers) {

	'use strict';

	SecurityProviderStatusService.$inject = ['VRNotificationService', 'VR_Sec_SecurityProviderAPIService', 'VR_GenericData_GenericBEActionService', 'UtilsService'];

	function SecurityProviderStatusService(VRNotificationService, VR_Sec_SecurityProviderAPIService, VR_GenericData_GenericBEActionService, UtilsService) {

		function changeSecurityProviderStatus() {

			var actionType = {
				ActionTypeName: "SecurityProviderStatus",
				ExecuteAction: function (payload) {
					var promiseDeffered = UtilsService.createPromiseDeferred();
					if (payload == undefined)
						return;
					var genericBusinessEntityId = payload.genericBusinessEntityId;
					var isEnabled = payload.genericBEAction.Settings.SetEnable;
					var securitActionId = (payload.genericBEAction != undefined) ? payload.genericBEAction.GenericBEActionId : undefined;
					VRNotificationService.showConfirmation().then(function (response) {
						if (response) {
							VR_Sec_SecurityProviderAPIService.ChangeSecurityProviderStatus(genericBusinessEntityId, isEnabled, securitActionId).then(function (response) {
								if (response != undefined) {
									if (VRNotificationService.notifyOnItemUpdated("Authentication Method", response, response.Message)) {
										payload.onItemUpdated(response.UpdatedObject);
									}
								}
								promiseDeffered.resolve(response);
							});
						}
						else promiseDeffered.resolve(response);
					});
					return promiseDeffered.promise;
				}
			};
			VR_GenericData_GenericBEActionService.registerActionType(actionType);
		}

		function registerSetDefaultSecurityProviderAction() {

			var actionType = {
				ActionTypeName: "SetDefaultSecurityProviderAction",
				ExecuteAction: function (payload) {
					var promiseDeffered = UtilsService.createPromiseDeferred();
					if (payload == undefined)
						return;
					var genericBusinessEntityId = payload.genericBusinessEntityId;
					var securitActionId = (payload.genericBEAction != undefined) ? payload.genericBEAction.GenericBEActionId : undefined;
					VRNotificationService.showConfirmation().then(function (response) {
						if (response) {
							VR_Sec_SecurityProviderAPIService.SetDefaultSecurityProvider(genericBusinessEntityId, securitActionId).then(function (response) {
								if (response != undefined) {
									if (VRNotificationService.notifyOnItemUpdated("Authentication Method", response, response.Message)) {
										payload.refreshGrid();
										payload.onItemUpdated(response.UpdatedObject);
									}
								}
								promiseDeffered.resolve(response);
							});
						}
						else promiseDeffered.resolve(response);
					});
					return promiseDeffered.promise;
				}
			};
			VR_GenericData_GenericBEActionService.registerActionType(actionType);
		}

		return ({
			changeSecurityProviderStatus: changeSecurityProviderStatus,
			registerSetDefaultSecurityProviderAction: registerSetDefaultSecurityProviderAction
		});
	}

	appControllers.service('VR_Sec_SecurityProviderStatusService', SecurityProviderStatusService);

})(appControllers);
