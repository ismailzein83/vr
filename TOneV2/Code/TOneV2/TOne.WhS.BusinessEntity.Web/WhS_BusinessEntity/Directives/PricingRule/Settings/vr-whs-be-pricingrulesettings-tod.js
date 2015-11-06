'use strict';
app.directive('vrWhsBePricingrulesettingsTod', ['$compile', 'WhS_BE_PricingRuleAPIService',
function ($compil, WhS_BE_PricingRuleAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            $scope.pricingRuleTODSettings = [];
            var bePricingRuleTODSettingObject = new bePricingRuleTODSetting(ctrl, $scope, $attrs);
            bePricingRuleTODSettingObject.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Settings/Templates/PricingRuleTODSettings.html"

    };


    function bePricingRuleTODSetting(ctrl, $scope, $attrs) {
        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
            }

            api.setData = function (selectedIds) {


            }
            api.load = function (payload) {

                if(payload!=undefined)
                {

                }
                var loadTODTemplatesPromiseDeferred = WhS_BE_PricingRuleAPIService.GetPricingRuleTODTemplates().then(function (response) {
                    angular.forEach(response, function (itm) {
                        $scope.pricingRuleTODSettings.push(itm);
                    });
                });
                loadTODTemplatesPromiseDeferred.promise;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);