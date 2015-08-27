SupplierInvoiceManagementController.$inject = ['$scope', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'SupplierInvoiceMeasureEnum', 'SupplierInvoiceAPIService', 'VRNotificationService'];

function SupplierInvoiceManagementController($scope, CarrierAccountAPIService, CarrierTypeEnum, SupplierInvoiceMeasureEnum, SupplierInvoiceAPIService, VRNotificationService) {

    var gridApi = undefined;

    defineScope();
    load();

    function defineScope() {
        $scope.suppliers = [];
        $scope.selectedSupplier = undefined;
        $scope.from = Date.now();
        $scope.to = Date.now();

        $scope.measures = [];
        $scope.invoices = [];
        $scope.showGrid = false;

        $scope.searchClicked = function () {
            $scope.showGrid = true;
            return retrieveData();
        }

        $scope.gridReady = function (api) {
            gridApi = api;
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return SupplierInvoiceAPIService.GetFilteredSupplierInvoices(dataRetrievalInput)
                .then(function (response) {
                    console.log(response);
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope); // notifyException instead?
                });
        }
    }

    function load() {
        $scope.isInitializing = true;

        loadMeasures();

        // load the suppliers
        CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Supplier.value, false)
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.suppliers.push(item);
                });
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isInitializing = false;
            });
    }

    function retrieveData() {
        var query = {
            selectedSupplierID: ($scope.selectedSupplier != undefined) ? $scope.selectedSupplier.CarrierAccountID : null,
            from: $scope.from,
            to: $scope.to
        };

        console.log(query);
        return gridApi.retrieveData(query);
    }

    function loadMeasures() {
        for (var property in SupplierInvoiceMeasureEnum) {
            $scope.measures.push(SupplierInvoiceMeasureEnum[property]);
        }
    }
}

appControllers.controller('Billing_SupplierInvoiceManagementController', SupplierInvoiceManagementController);
