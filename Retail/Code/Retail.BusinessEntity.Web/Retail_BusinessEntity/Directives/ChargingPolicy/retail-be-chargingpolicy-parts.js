'use strict';

app.directive('retailBeChargingpolicyParts', [function ()
{
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var chargingPolicyParts = new ChargingPolicyParts($scope, ctrl, $attrs);
            chargingPolicyParts.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/ChargingPolicy/Templates/ChargingPolicyPartsTemplate.html'
    };

    function ChargingPolicyParts($scope, ctrl, $attrs)
    {
        this.initializeController = initializeController;

        function initializeController()
        {
            $scope.scopeModel = {};
            defineAPI();
        }

        function defineAPI()
        {
            var api = {};

            api.load = function (payload) {
                
            };

            api.getData = function () {
                return [];
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
