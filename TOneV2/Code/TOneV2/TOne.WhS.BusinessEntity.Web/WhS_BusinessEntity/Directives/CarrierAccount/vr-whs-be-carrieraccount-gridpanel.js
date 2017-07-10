"use strict";

app.directive("vrWhsBeCarrieraccountGridpanel", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'WhS_BE_CarrierAccountService', 'WhS_BE_CarrierAccountAPIService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, WhS_BE_CarrierAccountService, WhS_BE_CarrierAccountAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var panel = new CarrierGridPanel($scope, ctrl, $attrs);
            panel.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CarrierAccount/Templates/CarrierAccountGridPanelTemplate.html"

    };

    function CarrierGridPanel($scope, ctrl, $attrs) {

        var gridAPI;
        var carrierProfileId;
        var openAddOnLoad;
        this.initializeController = initializeController;
        var context;
        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.addCarrierAccount = function () {
                var carrierProfileItem = { CarrierProfileId: carrierProfileId };
                var onCarrierAccountAdded = function (obj) {
                    gridAPI.onCarrierAccountAdded(obj);
                };
                WhS_BE_CarrierAccountService.addCarrierAccount(onCarrierAccountAdded, carrierProfileItem);
            };

            $scope.scopeModel.hadAddCarrierAccountPermission = function () {
                return WhS_BE_CarrierAccountAPIService.HasAddCarrierAccountPermission();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.loadPanel = function (payload) {
                if (payload != undefined) {
                    carrierProfileId = payload.carrierProfileId;
                    openAddOnLoad = payload.openAddOnLoad;
                } 
                return gridAPI.loadGrid(getGridQuery());
              

            };
            api.onCarrierAccountAdded = function (carrierAccountObject) {
                gridAPI.onCarrierAccountAdded(carrierAccountObject);
            };


            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }
       
        function getGridQuery() {

            var query = {
                query: {
                    CarrierProfilesIds: [carrierProfileId]
                },
                hideProfileColumn: true
            };
            return query;
        }
    }

    return directiveDefinitionObject;

}]);
