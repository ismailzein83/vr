(function (appControllers) {

    "use strict";

    operatorConfigurationManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'Demo_OperatorConfigurationService', 'VRValidationService'];

    function operatorConfigurationManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, Demo_OperatorConfigurationService, VRValidationService) {
        var gridAPI;
        var operatorConfigurationDirectiveAPI;
        var operatorProfileDirectiveAPI;
        var operatorProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var cdrDirectionDirectiveAPI;
        var cdrDirectionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.validateDateRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            }

            $scope.onOperatorProfileDirectiveReady = function (api) {
                operatorProfileDirectiveAPI = api;
                operatorProfileReadyPromiseDeferred.resolve();
            }

            $scope.onCDRDirectionReady = function (api) {
                cdrDirectionDirectiveAPI = api;
                cdrDirectionReadyPromiseDeferred.resolve();
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            }

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewOperatorConfiguration = AddNewOperatorConfiguration;

            function getFilterObject() {
                var data = {
                    OperatorIds: operatorProfileDirectiveAPI.getSelectedIds(),
                    CDRDirectionIds: cdrDirectionDirectiveAPI.getSelectedIds(),
                    FromDate: $scope.fromDate,
                    ToDate:$scope.toDate
                };

                return data;
            }
        }

        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadOperatorProfiles, loadCDRDirections])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }

        function loadOperatorProfiles() {
            var loadOperatorProfilePromiseDeferred = UtilsService.createPromiseDeferred();

            operatorProfileReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(operatorProfileDirectiveAPI, undefined, loadOperatorProfilePromiseDeferred);
                });

            return loadOperatorProfilePromiseDeferred.promise;
        }

        function loadCDRDirections() {
            var loadCDRDirectionsPromiseDeferred = UtilsService.createPromiseDeferred();
            cdrDirectionReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(cdrDirectionDirectiveAPI, undefined, loadCDRDirectionsPromiseDeferred);
            });
            return loadCDRDirectionsPromiseDeferred.promise;
        }


        function AddNewOperatorConfiguration() {
            var onOperatorConfigurationAdded = function (operatorConfigurationObj) {
                gridAPI.onOperatorConfigurationAdded(operatorConfigurationObj);
            };

            Demo_OperatorConfigurationService.addOperatorConfiguration(onOperatorConfigurationAdded);
        }

    }

    appControllers.controller('Demo_OperatorConfigurationManagementController', operatorConfigurationManagementController);
})(appControllers);