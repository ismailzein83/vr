'use strict'
dynamicPageManagementController.$inject = ['$scope', 'VR_ViewService'];
function dynamicPageManagementController($scope, VR_ViewService) {
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
            if (mainGridAPI != undefined)
                return mainGridAPI.loadGrid(getFilterObject());
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
        VR_ViewService.addDynamicPage(onDynamicPageAdded);
    }

    function load() {
     
    }



};

appControllers.controller('VR_Sec_DynamicPageManagementController', dynamicPageManagementController);