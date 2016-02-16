"use strict";

app.directive("vrDemoCarrierprofileGrid", ["UtilsService", "VRNotificationService", "Demo_CarrierProfileAPIService", "Demo_CarrierAccountService", "Demo_CarrierProfileService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, Demo_CarrierProfileAPIService, Demo_CarrierAccountService, Demo_CarrierProfileService, VRUIUtilsService) {

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
        templateUrl: "/Client/Modules/Demo_Module/Directives/CarrierAccount/Templates/CarrierProfileGridTemplate.html"

    };

    function CarrierProfileGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.carrierProfiles = [];
            defineMenuActions();
            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = [];
                var drillDownDefinition = {};

                drillDownDefinition.title = "Carrier Account";
                drillDownDefinition.directive = "vr-demo-carrieraccount-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, carrierProfileItem) {
                    carrierProfileItem.carrierAccountGridAPI = directiveAPI;
                    var payload = {
                        query: {
                            CarrierProfilesIds: [carrierProfileItem.Entity.CarrierProfileId]
                        },
                        hideProfileColumn: true
                    };
                    return carrierProfileItem.carrierAccountGridAPI.loadGrid(payload);
                };
                drillDownDefinitions.push(drillDownDefinition);
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onCarrierProfileAdded = function (carrierProfileObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(carrierProfileObject);
                        gridAPI.itemAdded(carrierProfileObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Demo_CarrierProfileAPIService.GetFilteredCarrierProfiles(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

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
                gridDrillDownTabsObj.setDrillDownExtensionObject(carrierProfile);
                gridAPI.itemUpdated(carrierProfile);

            }
            Demo_CarrierProfileService.editCarrierProfile(carrierProfileObj, onCarrierProfileUpdated);
        }

        function addCarrierAccount(dataItem) {
            gridAPI.expandRow(dataItem);
            var query = {
                CarrierProfilesIds: [dataItem.CarrierProfileId],
            }

            var onCarrierAccountAdded = function (carrierAccountObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(carrierAccountObj);
                if (dataItem.carrierAccountGridAPI != undefined)
                    dataItem.carrierAccountGridAPI.onCarrierAccountAdded(carrierAccountObj);
            };
            Demo_CarrierAccountService.addCarrierAccount(onCarrierAccountAdded, dataItem.Entity);
        }

        function deleteCarrierProfile(carrierProfileObj) {
            var onCarrierProfileDeleted = function () {
                //TODO: This is to refresh the Grid after delete, should be removed when centralized
                retrieveData();
            };

            // Demo_MainService.deleteCarrierAccount(carrierProfileObj, onCarrierProfileDeleted); to be added in CarrierAccountService
        }
    }

    return directiveDefinitionObject;

}]);