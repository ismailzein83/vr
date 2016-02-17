(function (appControllers) {

    "use strict";

    operatorDeclaredInformationManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'Demo_OperatorDeclaredInformationService', 'VRValidationService'];

    function operatorDeclaredInformationManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, Demo_OperatorDeclaredInformationService, VRValidationService) {
        var gridAPI;
        var operatorDeclaredInformationDirectiveAPI;
        var operatorProfileDirectiveAPI;
        var operatorProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        defineScope();
        load();

        function defineScope() {

            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            }


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

            $scope.AddNewOperatorDeclaredInformation = AddNewOperatorDeclaredInformation;

            function getFilterObject() {
                var data = {
                    FromDate: $scope.fromDate,
                    ToDate: $scope.toDate,
                    OperatorIds: operatorProfileDirectiveAPI.getSelectedIds()
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

        function AddNewOperatorDeclaredInformation() {
            var onOperatorDeclaredInformationAdded = function (operatorDeclaredInformationObj) {
                gridAPI.onOperatorDeclaredInformationAdded(operatorDeclaredInformationObj);
            };

            Demo_OperatorDeclaredInformationService.addOperatorDeclaredInformation(onOperatorDeclaredInformationAdded);
        }

    }

    appControllers.controller('Demo_OperatorDeclaredInformationManagementController', operatorDeclaredInformationManagementController);
})(appControllers);