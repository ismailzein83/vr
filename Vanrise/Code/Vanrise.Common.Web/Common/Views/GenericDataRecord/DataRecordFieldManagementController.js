(function (appControllers) {

    "use strict";

    dataRecordFieldManagementController.$inject = ['$scope', 'VRCommon_DataRecordFieldService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VRCommon_DataRecordFieldAPIService'];

    function dataRecordFieldManagementController($scope, VRCommon_DataRecordFieldService, UtilsService, VRNotificationService, VRUIUtilsService, VRCommon_DataRecordFieldAPIService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };
            $scope.selectedDataRecordFieldTypeTemplate = [];
            $scope.dataRecordFieldTypeTemplates = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {};
                api.loadGrid(filter);
            }
            $scope.name;
            $scope.AddDataRecordField = AddDataRecordField;
        }

        function load() {
            $scope.isLoadingFilterData = true;
            loadDataRecordFieldTypeTemplates()
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoadingFilterData = false;
                })
                .finally(function () {
                    $scope.isLoadingFilterData = false;
                });
        }

        function getFilterObject() {
            var data = {
                Name: $scope.name,
                TypeIds: UtilsService.getPropValuesFromArray($scope.selectedDataRecordFieldTypeTemplate, "TemplateConfigID")
            };
            return data;
        }

        function loadDataRecordFieldTypeTemplates() {
            return VRCommon_DataRecordFieldAPIService.GetDataRecordFieldTypeTemplates()
                .then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.dataRecordFieldTypeTemplates.push(item);
                    });
                });
        }

        function AddDataRecordField() {
            var onDataRecordFieldAdded = function (dataRecordFieldObj) {
                if (gridAPI != undefined)
                    gridAPI.onDataRecordFieldAdded(dataRecordFieldObj);
            };

            VRCommon_DataRecordFieldService.addDataRecordField(onDataRecordFieldAdded);
        }
    }

    appControllers.controller('VRCommon__DataRecordFieldManagementController', dataRecordFieldManagementController);
})(appControllers);