SupplierInvoiceDetailGridController.$inject = ['$scope', 'SupplierInvoiceAPIService', "RateTypeEnum", "UtilsService", 'VRNotificationService'];

function SupplierInvoiceDetailGridController($scope, SupplierInvoiceAPIService, RateTypeEnum, UtilsService, VRNotificationService) {

    var gridApi = undefined;

    defineScope();
    load();

    function defineScope() {

        $scope.details = [];
        $scope.dayGroupedDetails = [];
        
        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            if ($scope.viewScope.groupByDay) {

                return SupplierInvoiceAPIService.GetFilteredSupplierInvoiceDetailsGroupedByDay(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            }
            else {

                return SupplierInvoiceAPIService.GetFilteredSupplierInvoiceDetails(dataRetrievalInput)
                .then(function (response) {

                    angular.forEach(response.Data, function (item) {
                        var rateType = UtilsService.getEnum(RateTypeEnum, "value", item.RateType);
                        item.RateTypeDescription = rateType.description;
                    });

                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            }
        }
    }

    function load() {
    }

    function retrieveData() {
        var query = ($scope.viewScope.groupByDay) ?
            {
                SupplierID: $scope.viewScope.selectedSupplier.CarrierAccountID,
                From: $scope.dataItem.BeginDate,
                To: $scope.dataItem.EndDate
            } :
            { InvoiceID: $scope.dataItem.InvoiceID };

        return gridApi.retrieveData(query);
    }
}

appControllers.controller('Billing_SupplierInvoiceDetailGridController', SupplierInvoiceDetailGridController);