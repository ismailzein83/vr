SupplierInvoiceDetailGridController.$inject = ['$scope', 'SupplierInvoiceDetailGridMeasureEnum', 'SupplierInvoiceAPIService', 'VRNotificationService'];

function SupplierInvoiceDetailGridController($scope, SupplierInvoiceDetailGridMeasureEnum, SupplierInvoiceAPIService, VRNotificationService) {

    var gridApi = undefined;

    defineScope();
    load();

    function defineScope() {

        $scope.detailMeasures = [];
        $scope.details = [];
        
        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return SupplierInvoiceAPIService.GetFilteredSupplierInvoiceDetails(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }
    }

    function load() {
        loadMeasures();
    }

    function retrieveData() {

        var invoiceID = $scope.dataItem.InvoiceID;
        return gridApi.retrieveData(invoiceID);
    }

    function loadMeasures() {
        for (var property in SupplierInvoiceDetailGridMeasureEnum) {
            $scope.detailMeasures.push(SupplierInvoiceDetailGridMeasureEnum[property]);
        }
    }
}

appControllers.controller('Billing_SupplierInvoiceDetailGridController', SupplierInvoiceDetailGridController);