"use strict";

app.directive("vrWhsBeZoneServiceConfigGrid", ["UtilsService", "VRNotificationService", "WhS_BE_ZoneServiceConfigAPIService", "WhS_BE_MainService",
function (UtilsService, VRNotificationService, WhS_BE_ZoneServiceConfigAPIService, WhS_BE_MainService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var zoneServiceConfigGrid = new ZoneServiceConfigGrid($scope, ctrl, $attrs);
            zoneServiceConfigGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/ZoneServiceConfig/Templates/ZoneServiceConfigGridTemplate.html"

    };

    function ZoneServiceConfigGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.zoneServiceConfigs = [];
            $scope.onGridReady = function (api) {

                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onZoneServiceConfigAdded = function (zoneServiceConfigObject) {
                        gridAPI.itemAdded(zoneServiceConfigObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_ZoneServiceConfigAPIService.GetFilteredZoneServiceConfigs(dataRetrievalInput)
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
                ZoneServiceConfigsIds: [dataItem.ServiceFlag],
            }
            extensionObject.onGridReady = function (api) {
                extensionObject.onGridReady = undefined;
            };
            dataItem.extensionObject = extensionObject;

        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editZoneServiceConfig
            }
            ];
        }

        function editZoneServiceConfig(zoneServiceConfigObj) {
            var onZoneServiceConfigUpdated = function (zoneServiceConfigObj) {
                setDataItemExtension(zoneServiceConfigObj);
                gridAPI.itemUpdated(zoneServiceConfigObj);
            }

            WhS_BE_MainService.editZoneServiceConfig(zoneServiceConfigObj, onZoneServiceConfigUpdated);
        }

    }

    return directiveDefinitionObject;

}]);
