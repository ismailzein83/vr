(function (appControllers) {

    "use strict";

    codeGroupManagementController.$inject = ['$scope', 'WhS_BE_MainService', 'UtilsService', 'VRNotificationService'];

    function codeGroupManagementController($scope, WhS_BE_MainService, UtilsService, VRNotificationService) {
        var gridAPI;
        var filter = {};
        var countryDirectiveApi;
        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined) {
                    setFilterObject();
                    return gridAPI.loadGrid(filter);
                }
                    
            };
            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                api.load();
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;            
                api.loadGrid(filter);
            }

            $scope.addNewCodeGroup = addNewCodeGroup;
        }

        function load() {

           

        }

        function setFilterObject() {
            filter = {
                Code: $scope.code,
                CountriesIds: countryDirectiveApi.getIdsData()
            };
        }

        function addNewCodeGroup() {
            var onCodeGroupAdded = function (codeGroupObj) {
                if (gridAPI != undefined)
                    gridAPI.onCodeGroupAdded(codeGroupObj);
            };
            WhS_BE_MainService.addCodeGroup(onCodeGroupAdded);
        }

    }

    appControllers.controller('WhS_BE_CodeGroupManagementController', codeGroupManagementController);
})(appControllers);