"use strict";

app.directive("retailBeChargeableentitysettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var chargeableentitydefinitionsettings = new Chargeableentitydefinitionsettings($scope, ctrl);
            chargeableentitydefinitionsettings.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/ChargeableEntity/Templates/ChargeableEntitySettings.html"
    };

    function Chargeableentitydefinitionsettings($scope, ctrl) {

        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: "Retail.BusinessEntity.Entities.ChargeableEntitySettings ,Retail.BusinessEntity.Entities",
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
    return directiveDefinitionObject;

}]);