'use strict'
dynamicPageManagementController.$inject = ['$scope', 'VR_Sec_ViewService', 'VR_Sec_ViewAPIService'];

function dynamicPageManagementController($scope, VR_Sec_ViewService, VR_Sec_ViewAPIService) {
    var mainGridAPI;
    defineScope();
    load();

    function defineScope() {

        $scope.filterValue;

        $scope.onGridReady = function (api) {
            mainGridAPI = api;
            var filter = {};
            api.loadGrid(filter);
        };

        $scope.Add = addDynamicPage

        $scope.searchClicked = function () {
            return mainGridAPI.loadGrid(getFilterObject());
        };

        $scope.hasAddViewPermission = function () {
            return VR_Sec_ViewAPIService.HasAddViewPermission();
        };
    }

    function getFilterObject() {
        return $scope.filterValue;
    }

    function addDynamicPage() {
        var onDynamicPageAdded = function (dynamicPageObj) {
            if (mainGridAPI != undefined)
                mainGridAPI.onDynamicPageAdded(dynamicPageObj);
        };
        VR_Sec_ViewService.addDynamicPage(onDynamicPageAdded);
    }

    function load() {

    }

};

appControllers.controller('VR_Sec_DynamicPageManagementController', dynamicPageManagementController);