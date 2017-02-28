"use strict";

app.directive("vrWhsBeCarrierprofileGrid", ["UtilsService", "VRNotificationService", "WhS_BE_CarrierProfileAPIService", "WhS_BE_CarrierAccountService",
    "WhS_BE_CarrierProfileService", "VRUIUtilsService", "WhS_BE_CarrierAccountAPIService",
function (UtilsService, VRNotificationService, WhS_BE_CarrierProfileAPIService, WhS_BE_CarrierAccountService, WhS_BE_CarrierProfileService, VRUIUtilsService, WhS_BE_CarrierAccountAPIService) {

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
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CarrierAccount/Templates/CarrierProfileGridTemplate.html"

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

                var drillDownDefinitions = WhS_BE_CarrierProfileService.getDrillDownDefinition();

                var drillDownDefinition = {};

                drillDownDefinition.title = "Carrier Account";
                drillDownDefinition.directive = "vr-whs-be-carrieraccount-grid";

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
                    };
                    directiveAPI.onCarrierProfileAdded = function (carrierProfileObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(carrierProfileObject);
                        gridAPI.itemAdded(carrierProfileObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_CarrierProfileAPIService.GetFilteredCarrierProfiles(dataRetrievalInput)
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
                haspermission: hasUpdateCarrierProfilePermission
            },
                {
                    name: "New Carrier Account",
                    clicked: addCarrierAccount,
                    haspermission: hasAddCarrierAccountPermission
                }
            ];
        }

        function hasAddCarrierAccountPermission() {
            return WhS_BE_CarrierAccountAPIService.HasAddCarrierAccountPermission();
        }

        function hasUpdateCarrierProfilePermission() {
            return WhS_BE_CarrierProfileAPIService.HasUpdateCarrierProfilePermission();
        }


        function editCarrierProfile(carrierProfileObj) {
            var onCarrierProfileUpdated = function (carrierProfile) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(carrierProfile);
                gridAPI.itemUpdated(carrierProfile);

            };
            WhS_BE_CarrierProfileService.editCarrierProfile(carrierProfileObj, onCarrierProfileUpdated);
        }

        function addCarrierAccount(dataItem) {
            gridAPI.expandRow(dataItem);
            var query = {
                CarrierProfilesIds: [dataItem.CarrierProfileId],
            };

            var onCarrierAccountAdded = function (carrierAccountObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(carrierAccountObj);
                if (dataItem.carrierAccountGridAPI != undefined)
                    dataItem.carrierAccountGridAPI.onCarrierAccountAdded(carrierAccountObj);
            };
            WhS_BE_CarrierAccountService.addCarrierAccount(onCarrierAccountAdded, dataItem.Entity);
        }

        function deleteCarrierProfile(carrierProfileObj) {
            var onCarrierProfileDeleted = function () {
                //TODO: This is to refresh the Grid after delete, should be removed when centralized
                retrieveData();
            };

            // WhS_BE_MainService.deleteCarrierAccount(carrierProfileObj, onCarrierProfileDeleted); to be added in CarrierAccountService
        }
    }

    return directiveDefinitionObject;

}]);