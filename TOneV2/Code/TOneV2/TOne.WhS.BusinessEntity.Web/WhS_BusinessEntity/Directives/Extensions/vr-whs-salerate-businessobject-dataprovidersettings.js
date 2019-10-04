"use strict";
app.directive("vrWhsSalerateBusinessobjectDataprovidersettings", [
function () {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var businessObject = new BusinessObject($scope, ctrl, $attrs);
            businessObject.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/Extensions/Templates/SaleRateBusinessObjectDataProviderSettingsTemplate.html'
    };


    function BusinessObject($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.BusinessEntity.Business.SaleRateDataProviderSettings, TOne.WhS.BusinessEntity.Business"
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}
]);