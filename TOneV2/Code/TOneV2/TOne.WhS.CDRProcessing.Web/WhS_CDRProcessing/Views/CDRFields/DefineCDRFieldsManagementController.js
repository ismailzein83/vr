(function (appControllers) {

    "use strict";

    defineCDRFieldsManagementController.$inject = ['$scope', 'WhS_CDRProcessing_MainService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService','WhS_CDRProcessing_DefineCDRFieldsAPIService'];

    function defineCDRFieldsManagementController($scope, WhS_CDRProcessing_MainService, UtilsService, VRNotificationService, VRUIUtilsService, WhS_CDRProcessing_DefineCDRFieldsAPIService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };
            $scope.selectedCDRFieldTypeTemplate=[];
            $scope.cdrFieldTypeTemplates = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {};
                api.loadGrid(filter);
            }
            $scope.name;
            $scope.AddNewCDRField = AddNewCDRField;
        }

        function load() {
            $scope.isLoadingFilterData = true;
            loadCDRFieldTypeTemplates().catch(function (error) {
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
                TypeIds: UtilsService.getPropValuesFromArray($scope.selectedCDRFieldTypeTemplate, "TemplateConfigID")
            };
            return data;
        }
        function loadCDRFieldTypeTemplates() {
            return WhS_CDRProcessing_DefineCDRFieldsAPIService.GetCDRFieldTypeTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.cdrFieldTypeTemplates.push(item);
                });
            });
        }

        function AddNewCDRField() {
            var onCDRFieldAdded = function (cdrFieldObj) {
                if (gridAPI != undefined)
                    gridAPI.onCDRFieldAdded(cdrFieldObj);
            };

            WhS_CDRProcessing_MainService.addNewCDRField(onCDRFieldAdded);
        }
    }

    appControllers.controller('WhS_CDRProcessing_DefineCDRFieldsManagementController', defineCDRFieldsManagementController);
})(appControllers);