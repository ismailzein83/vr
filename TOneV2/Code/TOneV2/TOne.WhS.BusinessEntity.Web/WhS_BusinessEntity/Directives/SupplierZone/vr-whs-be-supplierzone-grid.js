"use strict";

app.directive("vrWhsBeSupplierzoneGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SupplierZoneAPIService", "WhS_BE_SupplierZoneService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, WhS_BE_SupplierZoneAPIService, WhS_BE_SupplierZoneService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new SupplierZoneGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SupplierZone/Templates/SupplierZoneGridTemplate.html"

    };

    function SupplierZoneGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.supplierzones = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = WhS_BE_SupplierZoneService.getDrillDownDefinition();
                
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        return gridAPI.retrieveData(query);
                    };

                    return directiveAPI;
                }
            };


            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SupplierZoneAPIService.GetFilteredSupplierZones(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                response.Data[i].EffectiveOn = dataRetrievalInput.Query.EffectiveOn;
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

    }

    return directiveDefinitionObject;

}]);
