//(function (appControllers) {

//    "use strict";

//    ratePlanSellingZonesWithDefaultratesController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'VRNotificationService', 'VRNavigationService', 'VRCommon_CountryAPIService'];

//    function ratePlanSellingZonesWithDefaultratesController($scope, BusinessProcess_BPTaskAPIService, VRNotificationService, VRNavigationService, VRCommon_CountryAPIService) {

//        var taskId;
//        var countryIds;
//        var countries;
//        loadParameters();
//        defineScope();
//        load();

//        function loadParameters() {
//            var parameters = VRNavigationService.getParameters($scope);

//            if (parameters != undefined && parameters != null) {
//                taskId = parameters.TaskId;
//            }
//        }

//        function defineScope() {
//            $scope.scopeModel = {};

//            $scope.scopeModel.continueTask = function () {
//                return executeTask(true);
//            };
//            $scope.scopeModel.stopTask = function () {
//                return executeTask(false);
//            };
//        }

//        function load() {
           
//            $scope.scopeModel.isLoading = true;

//            getTaskData().then(function () {
//                getCountriesByCountryIds().then(function () {
//                    for (var i = 0; i < countries.length; i++)
//                    {
//                        $scope.scopeModel.msg = $scope.scopeModel.msg + ' , ' + countries[i].Name;
//                    }
//                }).catch(function (error) {
//                    VRNotificationService.notifyExceptionWithClose(error, $scope);
//                    $scope.scopeModel.isLoading = false;
//                });
//            }).catch(function (error) {
//                VRNotificationService.notifyExceptionWithClose(error, $scope);
//                $scope.scopeModel.isLoading = false;
//            });
//        }
//        function getTaskData() {
//            return BusinessProcess_BPTaskAPIService.GetTask(taskId).then(function (response) {
//                if (response == null)
//                    return;
//                if (response.TaskData == null)
//                    return;
//                countryIds = response.TaskData.countryIdsWithDefaultRates;
//            });
//        }
//        function getCountriesByCountryIds()
//        {
//           return VRCommon_CountryAPIService.GetCountriesByCountryIds(countryIds).then(function (response) {
//                if (response == null)
//                    return;
//                countries = response;
//            })
//        }
//        function executeTask(decision) {
//            $scope.scopeModel.isLoading = true;

//            var executionInformation = {
//                $type: "TOne.WhS.Sales.BP.Arguments.Tasks.SellingZonesWithDefaultRatesTaskExecutionInformation, TOne.WhS.Sales.BP.Arguments",
//                Decision: decision
//            };

//            var input = {
//                $type: "Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput, Vanrise.BusinessProcess.Entities",
//                TaskId: taskId,
//                ExecutionInformation: executionInformation
//            };

//            return BusinessProcess_BPTaskAPIService.ExecuteTask(input).then(function (response) {
//                $scope.modalContext.closeModal();
//            }).catch(function (error) {
//                VRNotificationService.notifyException(error);
//            }).finally(function () {
//                $scope.scopeModel.isLoading = false;
//            });
//        }
      
//    }

//    appControllers.controller('WhS_Sales_RatePlanSellingzonesWithDefaultRatesController', ratePlanSellingZonesWithDefaultratesController);
//})(appControllers);
