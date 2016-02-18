(function (appControllers) {

    "use strict";

    operatorConfigurationManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'Demo_OperatorConfigurationService', 'VRValidationService'];

    function operatorConfigurationManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, Demo_OperatorConfigurationService, VRValidationService) {
        var gridAPI;
        var operatorConfigurationDirectiveAPI;
        var operatorProfileDirectiveAPI;
        var operatorProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {


            $scope.onOperatorProfileDirectiveReady = function (api) {
                operatorProfileDirectiveAPI = api;
                operatorProfileReadyPromiseDeferred.resolve();
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
                };

                return data;
            }
        }

        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadOperatorProfiles])
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
                    var directivePayload = undefined;

                    VRUIUtilsService.callDirectiveLoad(operatorProfileDirectiveAPI, directivePayload, loadOperatorProfilePromiseDeferred);
                });

            return loadOperatorProfilePromiseDeferred.promise;
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