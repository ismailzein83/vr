"use strict";

app.directive("vrWhsBeCountryGrid", ["UtilsService", "VRNotificationService", "WhS_BE_CountryAPIService", "WhS_BE_MainService",
function (UtilsService, VRNotificationService, WhS_BE_CountryAPIService, WhS_BE_MainService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var countryGrid = new CountryGrid($scope, ctrl, $attrs);
            countryGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Country/Templates/CountryGridTemplate.html"

    };

    function CountryGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
           
            $scope.countries = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                   
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                       
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onCountryAdded = function (countryObject) {
                        gridAPI.itemAdded(countryObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_CountryAPIService.GetFilteredCountries(dataRetrievalInput)
                    .then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                setDataItemExtension(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }

        function setDataItemExtension(dataItem) {

            var extensionObject = {};
            var query = {
                CountriesIds: [dataItem.CountryId],
            }
            extensionObject.onGridReady = function (api) {
                extensionObject.codeGroupGridAPI = api;
                extensionObject.codeGroupGridAPI.loadGrid(query);
                extensionObject.onGridReady = undefined;
            };
            dataItem.extensionObject = extensionObject;

        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
               clicked: editCountry
            }
           // ,
           //{
           //    name: "New Carrier Account",
           //    clicked: addCarrierAccount
           //}
            ];
        }

        function editCountry(countryObj) {
            var onCountryUpdated = function (countryObj) {
                setDataItemExtension(countryObj);
                gridAPI.itemUpdated(countryObj);
            }

            WhS_BE_MainService.editCountry(countryObj, onCountryUpdated);
        }
        //function addCarrierAccount(dataItem) {
        //    gridAPI.expandRow(dataItem);
        //    var query = {
        //        CarrierProfilesIds: [dataItem.CarrierProfileId],
        //    }
        //    if (dataItem.extensionObject.carrierAccountGridAPI != undefined)
        //        dataItem.extensionObject.carrierAccountGridAPI.loadGrid(query);
        //    var onCarrierAccountAdded = function (carrierAccountObj) {
        //        if (dataItem.extensionObject.carrierAccountGridAPI != undefined)
        //            dataItem.extensionObject.carrierAccountGridAPI.onCarrierAccountAdded(carrierAccountObj);
        //    };
        //    WhS_BE_MainService.addCarrierAccount(onCarrierAccountAdded, dataItem);
        //}
        //function deleteCarrierProfile(carrierProfileObj) {
        //    var onCarrierProfileDeleted = function () {
        //        //TODO: This is to refresh the Grid after delete, should be removed when centralized
        //        retrieveData();
        //    };

        //    WhS_BE_MainService.deleteCarrierAccount(carrierProfileObj, onCarrierProfileDeleted);
        //}
    }

    return directiveDefinitionObject;

}]);
