RepricingController.$inject = ['$scope', '$http', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService','UtilsService'];

function RepricingController($scope, $http, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    var pageLoaded = false;
    defineScope();
    function defineScope() {

        $scope.createRepricingInputObjects = [];
        $scope.repricingProcess = [];
        $scope.selectedSuppliers = [];
        $scope.selectedCustomers = [];
        $scope.generateTrafficStatistic=true;
        $scope.createProcessInput.getData = function () {

            var runningDate = new Date($scope.fromDate);

            $scope.createRepricingInputObjects.length = 0;
            if (runningDate.toLocaleString() === $scope.toDate.toLocaleString()) {//Single day
                    $scope.createRepricingInputObjects.push({
                        InputArguments: {
                            $type: "TOne.CDRProcess.Arguments.DailyRepricingProcessInput, TOne.CDRProcess.Arguments",
                            RepricingDay: $scope.fromDate,
                            GenerateTrafficStatistic: $scope.generateTrafficStatistic,
                            CustomersIds: UtilsService.getPropValuesFromArray($scope.selectedCustomers, 'CarrierAccountID'),
                            SupplierIds: UtilsService.getPropValuesFromArray($scope.selectedSuppliers, 'CarrierAccountID')
                        }
                    });
                }

                else if (runningDate.toLocaleString() < $scope.toDate.toLocaleString()) //More than one day
                {
                while (runningDate < $scope.toDate) {
                    $scope.createRepricingInputObjects.push({
                        InputArguments: {
                            $type: "TOne.CDRProcess.Arguments.DailyRepricingProcessInput, TOne.CDRProcess.Arguments",
                            RepricingDay: new Date(runningDate),
                            GenerateTrafficStatistic: $scope.generateTrafficStatistic,
                            CustomersIds: UtilsService.getPropValuesFromArray($scope.selectedCustomers, 'CarrierAccountID'),
                            SupplierIds: UtilsService.getPropValuesFromArray($scope.selectedSuppliers, 'CarrierAccountID')
                        }
                    });
                    runningDate = new Date(runningDate.setHours(runningDate.getHours() + 24));
                }
            }
            return $scope.createRepricingInputObjects;

        };


    }



}
appControllers.controller('CDR_RepricingController', RepricingController)



