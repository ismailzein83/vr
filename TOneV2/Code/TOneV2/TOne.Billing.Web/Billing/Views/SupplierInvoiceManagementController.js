SupplierInvoiceManagementController.$inject = ['$scope', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'SupplierInvoiceAPIService', 'AnalyticsService', 'VRNotificationService'];

function SupplierInvoiceManagementController($scope, CarrierAccountAPIService, CarrierTypeEnum, SupplierInvoiceAPIService, AnalyticsService, VRNotificationService) {

    var gridApi = undefined;

    defineScope();
    load();

    function defineScope() {
        $scope.suppliers = [];
        $scope.selectedSupplier = undefined;

        $scope.periods = AnalyticsService.getPeriods();
        $scope.selectedPeriod = $scope.periods[0];
        $scope.from = Date.now();
        $scope.to = Date.now();

        $scope.invoices = [];
        $scope.showGrid = false;
        defineMenuActions();

        $scope.periodSelectionChanged = function (selectedOption) {

            if ($scope.selectedPeriod != undefined && $scope.selectedPeriod.value != -1) {
                var date = $scope.selectedPeriod.getInterval();
                $scope.from = date.from;
                $scope.to = date.to;
            }
        }

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

                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope); // notifyException instead?
                });
        }
    }

    function load() {
        $scope.isInitializing = true;

        // load the suppliers
        CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Supplier.value, true)
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
            SelectedSupplierID: ($scope.selectedSupplier != undefined) ? $scope.selectedSupplier.CarrierAccountID : null,
            From: $scope.from,
            To: $scope.to
        };

        return gridApi.retrieveData(query);
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [
            {
                name: 'Get Supplier Amount',
                clicked: getSupplierAmount
            },
            {
                name: 'Toggle Is Locked',
                clicked: toggleIsLocked
            },
            {
                name: 'Toggle Is Paid',
                clicked: toggleIsPaid
            },
            {
                name: 'Edit Notes',
                clicked: editNotes
            },
            {
                name: 'Export As Excel',
                clicked: exportAsExcel
            },
            {
                name: 'Export As PDF',
                clicked: exportAsPDF
            },
            {
                name: 'Download Attachment',
                clicked: downloadAttachment
            },
            {
                name: 'Email',
                clicked: emailInvoice
            },
            {
                name: 'Compare',
                clicked: compareInvoice
            },
            {
                name: 'Delete',
                clicked: deleteInvoice
            }
        ];
    }

    function getSupplierAmount(invoiceObject) { }

    function toggleIsLocked(invoiceObject) {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    return SupplierInvoiceAPIService.ToggleInvoiceLock(invoiceObject.InvoiceID)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Invoice", deletionResponse);
                            return retrieveData();
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }

    function toggleIsPaid(invoiceObject) { }

    function editNotes(invoiceObject) { }

    function exportAsExcel(invoiceObject) { }

    function exportAsPDF(invoiceObject) { }

    function downloadAttachment(invoiceObject) { }

    function emailInvoice(invoiceObject) { }

    function compareInvoice(invoiceObject) { }

    function deleteInvoice(invoiceObject) {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    return SupplierInvoiceAPIService.DeleteInvoice(invoiceObject.InvoiceID)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Invoice", deletionResponse);
                            return retrieveData();
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}

appControllers.controller('Billing_SupplierInvoiceManagementController', SupplierInvoiceManagementController);
