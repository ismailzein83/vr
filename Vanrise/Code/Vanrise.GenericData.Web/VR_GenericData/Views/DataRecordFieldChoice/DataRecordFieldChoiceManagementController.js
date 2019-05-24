(function (appControllers) {
    'use strict';

    DataRecordFieldChoiceManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldChoiceService', 'VR_GenericData_DataRecordFieldChoiceAPIService','VRNotificationService'];

    function DataRecordFieldChoiceManagementController($scope, UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldChoiceService, VR_GenericData_DataRecordFieldChoiceAPIService, VRNotificationService) {

        var dataRecordFieldChoiceGridAPI;
        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.onGridReady = function (api) {
                dataRecordFieldChoiceGridAPI = api;
                var filter = {};
                dataRecordFieldChoiceGridAPI.loadGrid(filter);
            };

            $scope.search = function () {
                return dataRecordFieldChoiceGridAPI.loadGrid(getFilterObject());
            };
            $scope.hasAddDataRecordFieldChoice = function () {
                return VR_GenericData_DataRecordFieldChoiceAPIService.HasAddDataRecordFieldChoice();
            };
            $scope.addDataRecordFieldChoice = function () {
                var onDataRecordFieldChoiceAdded = function (dataRecordFieldChoiceObj) {
                    dataRecordFieldChoiceGridAPI.onDataRecordFieldChoiceAdded(dataRecordFieldChoiceObj);
                };

                VR_GenericData_DataRecordFieldChoiceService.addDataRecordFieldChoice(onDataRecordFieldChoiceAdded);
            };
            $scope.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadDevProjectSelector])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }
        function loadDevProjectSelector() {
            var devProjectPromiseLoadDeferred = UtilsService.createPromiseDeferred();
            devProjectPromiseReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(devProjectDirectiveApi, undefined, devProjectPromiseLoadDeferred);
            });
            return devProjectPromiseLoadDeferred.promise;
        }
        function getFilterObject() {
          var  filter = {
              Name: $scope.name,
              DevProjectIds: devProjectDirectiveApi.getSelectedIds()
            };
            return filter;
        }
    }

    appControllers.controller('VR_GenericData_DataRecordFieldChoiceManagementController', DataRecordFieldChoiceManagementController);

})(appControllers);
