"use strict";

app.directive("vrWhsBeCarrierprofilegrid", ["UtilsService", "VRNotificationService", "WhS_BE_CarrierProfileAPIService", "WhS_BE_MainService",
function (UtilsService, VRNotificationService, WhS_BE_CarrierProfileAPIService, WhS_BE_MainService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var carrierProfileGrid = new CarrierProfileGrid($scope, ctrl, $attrs);
            carrierProfileGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Templates/CarrierProfileGridTemplate.html"

    };

    function CarrierProfileGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.carrierProfiles = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onCarrierProfileAdded = function (carrierProfileObject) {
                        gridAPI.itemAdded(carrierProfileObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_CarrierProfileAPIService.GetFilteredCarrierProfiles(dataRetrievalInput)
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
                CarrierProfilesIds: [dataItem.CarrierProfileId],
            }
            extensionObject.onGridReady = function (api) {
                extensionObject.carrierAccountGridAPI = api;
                extensionObject.carrierAccountGridAPI.loadGrid(query);
                extensionObject.onGridReady = undefined;
            };
            dataItem.extensionObject = extensionObject;

        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editCarrierProfile,
            },
           {
               name: "New Carrier Account",
               clicked: addCarrierAccount
           }
            ];
        }

        function editCarrierProfile(carrierProfileObj) {
            var onCarrierProfileUpdated = function (carrierProfile) {
                gridAPI.itemUpdated(carrierProfile);
            }

            WhS_BE_MainService.editCarrierProfile(carrierProfileObj, onCarrierProfileUpdated);
        }
        function addCarrierAccount(dataItem) {
            gridAPI.expandRow(dataItem);
            var query = {
                CarrierProfilesIds: [dataItem.CarrierProfileId],
            }
            if (dataItem.extensionObject.carrierAccountGridAPI != undefined)
                dataItem.extensionObject.carrierAccountGridAPI.loadGrid(query);
            var onCarrierAccountAdded = function (carrierAccountObj) {
                if (dataItem.extensionObject.carrierAccountGridAPI != undefined)
                    dataItem.extensionObject.carrierAccountGridAPI.onCarrierAccountAdded(carrierAccountObj);
            };
            WhS_BE_MainService.addCarrierAccount(onCarrierAccountAdded, dataItem);
        }
        function deleteCarrierProfile(carrierProfileObj) {
            var onCarrierProfileDeleted = function () {
                //TODO: This is to refresh the Grid after delete, should be removed when centralized
                retrieveData();
            };

            WhS_BE_MainService.deleteCarrierAccount(carrierProfileObj, onCarrierProfileDeleted);
        }
    }

    return directiveDefinitionObject;

}]);
