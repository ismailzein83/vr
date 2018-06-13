(function (appControllers) {

	'use strict';

	CustomerRoutesService.$inject = ['VRModalService', 'VR_Analytic_AnalyticItemActionService'];

	function CustomerRoutesService(VRModalService, VR_Analytic_AnalyticItemActionService) {

		function registerOpenCustomerRoutes() {
			var actionType = {
				ActionTypeName: "Routes",
				ConfigId: "2aedd1d5-26d1-4c0e-a49c-ce5e8355c300",
				ExecuteAction: function (payload) {
					if (payload == undefined || payload.ItemAction == undefined || payload.Settings == undefined)
						return;
					openCustomerRoutes(payload)
				}
			};
			VR_Analytic_AnalyticItemActionService.registerActionType(actionType);
		}

		function openCustomerRoutes(payload) {
			if (payload.Settings.DimensionFilters == null)
				return;
			var customersIds;
			var zoneIds;

			for (var i = 0; i < payload.Settings.DimensionFilters.length; i++) {
				var item = payload.Settings.DimensionFilters[i];

				if (item.Dimension == 'MasterZone')
					zoneIds = item.FilterValues;

				if (item.Dimension == 'Customer')
					customersIds = item.FilterValues;
			}

			var modalParameters = {
				CustomersIds: customersIds,
				ZoneIds: zoneIds
			};
			var modalSettings = {
				useModalTemplate: true,
				width: "80%",
				title: payload.ItemAction.Title
			};
			modalSettings.onScopeReady = function (modalScope) {
			};

			VRModalService.showModal('/Client/Modules/WhS_Routing/Views/CustomerRoute/CustomerRouteManagement.html', modalParameters, modalSettings);
		}

		return ({
			registerOpenCustomerRoutes: registerOpenCustomerRoutes
		});
	}

	appControllers.service('WhS_Routing_CustomerRoutesService', CustomerRoutesService);

})(appControllers);