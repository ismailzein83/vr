CustomerInvoiceController.$inject = ['$scope', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'CustomerInvoiceAPIService', 'PeriodRangeEnum', 'VRNotificationService'];

function CustomerInvoiceController($scope, CarrierAccountAPIService, CarrierTypeEnum, CustomerInvoiceAPIService, PeriodRangeEnum, VRNotificationService) {

    var gridApi = undefined;

    defineScope();
    load();

    function defineScope() {
        $scope.customers = [];
        $scope.selectedCustomer = undefined;
        
      
        $scope.customerInvoices = [];
        $scope.showGrid = false;

        $scope.searchClicked = function () {
            $scope.showGrid = true;
            $scope.mainGridPagerSettings.currentPage = 1;
            return retrieveData();
        }

        $scope.periodRangeSelectionChanged = function () {
            var currentdate = new Date();
            var newFromDate = new Date();
            var newToDate = new Date();
            switch ($scope.selectedPeriodRange) {
                case PeriodRangeEnum.LastYear:
                    newFromDate.setYear((new Date()).getFullYear() - 1);
                    newFromDate.setMonth(0);
                    newFromDate.setDate(01);
                    $scope.fromDate = newFromDate;
                    newToDate.setYear((new Date()).getFullYear() - 1);
                    newToDate.setMonth(11);
                    newToDate.setDate(31);
                    $scope.toDate = newToDate;
                    break;
                case PeriodRangeEnum.LastMonth:
                    newFromDate.setYear((new Date()).getFullYear());
                    newFromDate.setMonth((new Date()).getMonth() -1);
                    newFromDate.setDate(01);
                    $scope.fromDate = newFromDate;
                    newToDate.setYear((new Date()).getFullYear());
                    newToDate.setMonth((new Date()).getMonth());
                    newToDate.setDate(0);
                    $scope.toDate = newToDate;
                    break;
                case PeriodRangeEnum.LastWeek:
                    newFromDate.setYear((new Date()).getFullYear());
                    newFromDate.setMonth((new Date()).getMonth());
                    newFromDate.setDate((new Date()).getDate() -7);
                    $scope.fromDate = newFromDate;
                    $scope.toDate = newToDate;
                    break;
                case PeriodRangeEnum.Yesterday:
                    newFromDate.setYear((new Date()).getFullYear());
                    newFromDate.setMonth((new Date()).getMonth());
                    newFromDate.setDate((new Date()).getDate() - 1);
                    $scope.fromDate = newFromDate;
                    $scope.toDate = newFromDate;
                    break;
                case PeriodRangeEnum.Today:
                    $scope.fromDate = newFromDate;
                    $scope.toDate = newFromDate;
                    break;
                case PeriodRangeEnum.CurrentWeek:
                    $scope.fromDate = newFromDate;
                    newToDate.setYear((new Date()).getFullYear());
                    newToDate.setMonth((new Date()).getMonth());
                    newToDate.setDate((new Date()).getDate()+7);
                    $scope.toDate = newToDate;
                    break;
                case PeriodRangeEnum.CurrentMonth:
                    newFromDate.setYear((new Date()).getFullYear());
                    newFromDate.setMonth((new Date()).getMonth());
                    newFromDate.setDate(01);
                    $scope.fromDate = newFromDate;
                    newToDate.setYear((new Date()).getFullYear());
                    newToDate.setMonth((new Date()).getMonth()+1);
                    newToDate.setDate(0);
                    $scope.toDate = newToDate;
                    break;
                case PeriodRangeEnum.CurrentYear:
                    newFromDate.setYear((new Date()).getFullYear() );
                    newFromDate.setMonth(0);
                    newFromDate.setDate(01);
                    $scope.fromDate = newFromDate;
                    newToDate.setYear((new Date()).getFullYear());
                    newToDate.setMonth(11);
                    newToDate.setDate(31);
                    $scope.toDate = newToDate;
                    break;
                case PeriodRangeEnum.Customize:
                    $scope.fromDate = "";
                    $scope.toDate = "";
                    break;

            }
         
        }

        $scope.dateTimeSelectionChange = function () {
             $scope.selectedPeriodRange = PeriodRangeEnum.Customize;
        }

        $scope.mainGridPagerSettings = {
            currentPage: 1,
            totalDataCount: 0,
            pageChanged: function () {
                return retrieveData();
            }
        };
        $scope.gridReady = function (api) {
            gridApi = api;
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return CustomerInvoiceAPIService.GetFilteredCustomerInvoices(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope); 
                });
        }
    }

    function load() {
        loadCustomers();
        loadTimePeriods();
    }

    function loadCustomers() {
        $scope.isInitializing = true;
        CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Customer.value, true)
            .then(function(response) {
                angular.forEach(response, function(item) {
                    $scope.customers.push(item);
                });
            })
            .catch(function(error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function() {
                $scope.isInitializing = false;
            });
    }

    function loadTimePeriods() {
        $scope.periodRanges = [];
        for (var prop in PeriodRangeEnum) {
            $scope.periodRanges.push(PeriodRangeEnum[prop]);
        }
        $scope.selectedPeriodRange = $scope.periodRanges[4];
    }


    function retrieveData() {
        var pageInfo = $scope.mainGridPagerSettings.getPageInfo();
        var query = {
            SelectedCustomerID: ($scope.selectedCustomer != undefined) ? $scope.selectedCustomer.CarrierAccountID : null,
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate
        };
        return gridApi.retrieveData(query);
    }

}

appControllers.controller('Billing_CustomerInvoiceController', CustomerInvoiceController);
