(function (appControllers) {

    'use strict';

    CustomerRoutesService.$inject = ['VRModalService', 'VR_Analytic_AnalyticItemActionService', 'UtilsService'];

    function CustomerRoutesService(VRModalService, VR_Analytic_AnalyticItemActionService, UtilsService) {

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
            var suppliersIds;
            var zoneIds;
            var saleCode;
            var hasSaleZoneDimension;

            if (UtilsService.getItemByVal(payload.Settings.DimensionFilters, 'SaleZone', 'Dimension') != undefined)
                hasSaleZoneDimension = true;

            for (var i = 0; i < payload.Settings.DimensionFilters.length; i++) {
                var item = payload.Settings.DimensionFilters[i];

                if (item.Dimension == 'MasterZone' && !hasSaleZoneDimension)
                    zoneIds = item.FilterValues;

                if (item.Dimension == 'SaleZone')
                    zoneIds = item.FilterValues;

                if (item.Dimension == 'Customer')
                    customersIds = item.FilterValues;

                if (item.Dimension == 'Supplier')
                    suppliersIds = item.FilterValues;

                if (item.Dimension == 'SaleCode')
                    saleCode = item.FilterValues != undefined && item.FilterValues.length > 0 ? item.FilterValues[0] : undefined;
            }

            var modalParameters = {
                CustomersIds: customersIds,
                SuppliersIds: suppliersIds,
                ZoneIds: zoneIds,
                SaleCode: saleCode
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