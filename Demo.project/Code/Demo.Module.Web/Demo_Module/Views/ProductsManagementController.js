/// <reference path="../Services/APIServices/SettingConfigsAPIService.js" />
(function (appControllers) {

    "use strict";

productsManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_ProductService'];

function productsManagementController($scope,VRNotificationService,Demo_Module_ProductService) {

    var gridAPI;
    defineScope();
    load();

    function defineScope(){

        $scope.searchClicked = function () {

            gridAPI.loadGrid(getGridQuery());
        }

        $scope.onGridReady = function (api) {

            gridAPI = api;
            api.loadGrid(getGridQuery());

        }

        $scope.addNewProduct = function () 
        {
            var onProductAdded = function (product) {

                if (gridAPI != undefined)
                    gridAPI.onProductAdded(product)
            };

            Demo_Module_ProductService.addProduct(onProductAdded);
        }

    }//end defineScope()


    function load() {
    }

    function getGridQuery() {

        return { Name: $scope.name };
    }



}//end controller function

appControllers.controller("Demo_Module_ProductsManagementController",productsManagementController);

})(appControllers);