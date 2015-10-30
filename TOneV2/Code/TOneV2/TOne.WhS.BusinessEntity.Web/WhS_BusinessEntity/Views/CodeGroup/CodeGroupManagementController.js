(function (appControllers) {

    "use strict";

    codeGroupManagementController.$inject = ['$scope', 'WhS_BE_MainService', 'UtilsService', 'VRNotificationService'];

    function codeGroupManagementController($scope, WhS_BE_MainService, UtilsService, VRNotificationService) {
        var gridAPI;
        var countryDirectiveApi;
        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined) {
                   
                    return gridAPI.loadGrid(getFilterObject());
                }
                    
            };
            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                api.load();
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;            
                api.loadGrid({});
            }

            $scope.AddNewCodeGroup = AddNewCodeGroup;
        }

        function load() {

           

        }

        function getFilterObject() {
            var data = {
                Code: $scope.code,
                CountriesIds: countryDirectiveApi.getIdsData()
            };
            return data;
        }

        function AddNewCodeGroup() {
            var onCodeGroupAdded = function (codeGroupObj) {
                if (gridAPI != undefined)
                    gridAPI.onCodeGroupAdded(codeGroupObj);
            };
            WhS_BE_MainService.addCodeGroup(onCodeGroupAdded);
        }

    }

    appControllers.controller('WhS_BE_CodeGroupManagementController', codeGroupManagementController);
})(appControllers);