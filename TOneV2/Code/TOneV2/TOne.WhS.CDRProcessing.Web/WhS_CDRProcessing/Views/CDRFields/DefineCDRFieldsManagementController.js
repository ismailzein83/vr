(function (appControllers) {

    "use strict";

    defineCDRFieldsManagementController.$inject = ['$scope', 'WhS_CDRProcessing_MainService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function defineCDRFieldsManagementController($scope, WhS_CDRProcessing_MainService, UtilsService, VRNotificationService, VRUIUtilsService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

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
           
        }

        function getFilterObject() {
            var data = {
                Description: $scope.description,
            };
            return data;
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