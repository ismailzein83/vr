SupplierInvoiceDetailGridController.$inject = ['$scope', 'SupplierInvoiceAPIService', 'VRNotificationService'];

function SupplierInvoiceDetailGridController($scope, SupplierInvoiceAPIService, VRNotificationService) {

    var gridApi = undefined;

    defineScope();
    load();

    function defineScope() {

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
    }

    function retrieveData() {
        var query = {
            InvoiceID: $scope.dataItem.InvoiceID
        };
        
        return gridApi.retrieveData(query);
    }
}

appControllers.controller('Billing_SupplierInvoiceDetailGridController', SupplierInvoiceDetailGridController);