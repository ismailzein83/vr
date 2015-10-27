﻿(function (appControllers) {

    "use strict";

    codeGroupManagementController.$inject = ['$scope', 'WhS_BE_MainService', 'UtilsService', 'VRNotificationService'];

    function codeGroupManagementController($scope, WhS_BE_MainService, UtilsService, VRNotificationService) {
        var gridAPI;
        var countryDirectiveApi;
        defineScope();
        load();

        function defineScope() {
            //$scope.searchClicked = function () {
            //    if (!$scope.isGettingData && gridAPI != undefined) {
                   
            //        return gridAPI.loadGrid(getFilterObject());
            //    }
                    
            //};
            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                api.load();
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;            
                api.loadGrid({});
            }

            ////$scope.name;
            ////$scope.onCarrierProfileDirectiveReady = function (api) {
            ////    carrierProfileDirectiveAPI = api;
            ////    load();
            ////}

            ////$scope.onGridReady = function (api) {
            ////    gridAPI = api;
            ////    api.loadGrid({});
            ////}

            //$scope.AddNewCountry = AddNewCountry;
            ////$scope.country = "lebanon";
        }

        function load() {

            //$scope.isGettingData = true;

            //if (carrierProfileDirectiveAPI == undefined)
            //    return;

            //carrierProfileDirectiveAPI.load().then(function () {
            //}).catch(function (error) {
            //    VRNotificationService.notifyExceptionWithClose(error, $scope);
            //    $scope.isGettingData = false;
            //}).finally(function () {
            //    $scope.isGettingData = false;
            //});

        }

        //function getFilterObject() {
        //    var data = {
        //        Name: $scope.name,
        //    };
        //    return data;
        //}

        //function AddNewCountry() {
        //    var onCountryAdded = function (countryObj) {
        //        if (gridAPI != undefined)
        //            gridAPI.onCountryAdded(countryObj);
        //    };
        //    WhS_BE_MainService.addCountry(onCountryAdded);
        //}

    }

    appControllers.controller('WhS_BE_CodeGroupManagementController', codeGroupManagementController);
})(appControllers);