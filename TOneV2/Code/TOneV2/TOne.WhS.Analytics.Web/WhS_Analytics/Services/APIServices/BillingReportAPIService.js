(function (appControllers) {

    'use strict';

    BillingReportAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Analytics_ModuleConfig'];

    function BillingReportAPIService(BaseAPIService, UtilsService, WhS_Analytics_ModuleConfig) {

        var controllerName = 'BillingReport';

        return {
            ExportCarrierProfile: ExportCarrierProfile
        };

        function ExportCarrierProfile(fromDate, toDate, topDestination, customerId, currencyId, currencySymbol, currencyName) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Analytics_ModuleConfig.moduleName, controllerName, 'ExportCarrierProfile'),
                {
                    FromDate: fromDate,
                    ToDate: toDate,
                    TopDestination: topDestination,
                    CustomerId: customerId,
                    CurrencyID: currencyId,
                    CurrencySymbol: currencySymbol,
                    CurrencyName: currencyName
                },
                {
                    responseTypeAsBufferArray: true,
                    returnAllResponseParameters: true
                }
            );
        }
    }

    appControllers.service('WhS_Analytics_BillingReportAPIService', BillingReportAPIService);

})(appControllers);