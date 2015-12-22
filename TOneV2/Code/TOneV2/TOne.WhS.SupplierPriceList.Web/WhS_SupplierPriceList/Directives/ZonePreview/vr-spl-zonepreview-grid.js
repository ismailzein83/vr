"use strict";

app.directive("vrSplZonepreviewGrid", ["WhS_SupPL_SupplierPriceListPreviewPIService", "VRNotificationService", "WhS_SupPL_ZoneChangeTypeEnum",
function (WhS_SupPL_SupplierPriceListPreviewPIService, VRNotificationService, WhS_SupPL_ZoneChangeTypeEnum) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var zonePreviewGrid = new ZonePreviewGrid($scope, ctrl, $attrs);
                zonePreviewGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/ZonePreview/Templates/SupplierPriceListZonePreviewGridTemplate.html"

        };

        function ZonePreviewGrid($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {
              
                $scope.changedZones = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());
                    function getDirectiveAPI() {

                        var directiveAPI = {};
                        directiveAPI.load = function (query) {

                            return gridAPI.retrieveData(query);
                        }
                        
                        return directiveAPI;
                    }
                };
                function getValueType(enumname, value) {
                    switch (value) {                        
                        case enumname.New.value:
                            return enumname.New.description;
                            break;
                        case enumname.Closed.value:
                            return enumname.Closed.description;
                            break;
                        case enumname.Deleted.value:
                            return enumname.Deleted.description;
                            break;
                        default:
                            return undefined;
                    }
                }
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_SupPL_SupplierPriceListPreviewPIService.GetFilteredZonePreview(dataRetrievalInput)
                        .then(function (response) {
                            for (var i = 0 ; i < response.Data.length ; i++) {
                                response.Data[i].ChangeTypeText = getValueType(WhS_SupPL_ZoneChangeTypeEnum, response.Data[i].ChangeType)
                            }
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };
            }
        }

        return directiveDefinitionObject;
        
}]);
