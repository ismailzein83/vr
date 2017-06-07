
(function (appControllers) {

    "use strict";

    SwitchDashboardService.$inject = ['VRModalService', 'UtilsService'];

    function SwitchDashboardService(VRModalService, UtilsService) {

        function viewLiveCdrs(dataItem) {
            var settings = {
                useModalTemplate: true,
               title:"Live CDR"
            };
            var parameters = {
                CustomerId: (dataItem.CustomerId!=undefined)?dataItem.CustomerId:undefined,
                SupplierId: (dataItem.SupplierId != undefined) ? dataItem.SupplierId : undefined
            };
            VRModalService.showModal('/Client/Modules/NP_IVSwitch/Views/LiveCdr/LiveCdrManagement.html', parameters, settings);
        };

        return {
            viewLiveCdrs: viewLiveCdrs
        };
    }

    appControllers.service('NP_IVSwitch_SwitchDashboardService', SwitchDashboardService);

})(appControllers);