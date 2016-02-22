(function (appControllers) {

    "use strict";

    operatorDeclaredInformationManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'Demo_OperatorDeclaredInformationService', 'VRValidationService'];

    function operatorDeclaredInformationManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, Demo_OperatorDeclaredInformationService, VRValidationService) {
        var gridAPI;
        var operatorDeclaredInformationDirectiveAPI;
        var operatorProfileDirectiveAPI;
        var operatorProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var zoneDirectiveAPI;
        var zoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var serviceTypeDirectiveAPI;
        var serviceTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            $scope.onServiceTypeReady = function (api) {
                serviceTypeDirectiveAPI = api;
                serviceTypeReadyPromiseDeferred.resolve();
            }

            $scope.onZoneDirectiveReady = function (api) {
                zoneDirectiveAPI = api;
                zoneReadyPromiseDeferred.resolve();
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
                    OperatorIds: operatorProfileDirectiveAPI.getSelectedIds(),
                    ZoneIds: zoneDirectiveAPI.getSelectedIds(),
                    ServiceTypeIds: serviceTypeDirectiveAPI.getSelectedIds(),
                };

                return data;
            }
        }

        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadOperatorProfiles, loadZones, loadServiceTypes])
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

        function loadZones() {
            var loadZonePromiseDeferred = UtilsService.createPromiseDeferred();
            zoneReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = undefined;
                    VRUIUtilsService.callDirectiveLoad(zoneDirectiveAPI, directivePayload, loadZonePromiseDeferred);
                });

            return loadZonePromiseDeferred.promise;
        }

        function loadServiceTypes() {
            var loadServiceTypePromiseDeferred = UtilsService.createPromiseDeferred();
            serviceTypeReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(serviceTypeDirectiveAPI, undefined, loadServiceTypePromiseDeferred)
            })
            return loadServiceTypePromiseDeferred.promise;
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