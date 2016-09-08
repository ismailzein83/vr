'use strict';

app.directive('retailBeTargetconvertorDirective', ['VRNotificationService',
    function (vrNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var targetConvertor = new targetConvertorDirective($scope, ctrl, $attrs);
                targetConvertor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/RingoExtensions/TargetConvertor/Templates/RingoAccountConvertorTemplate.html'
        };

        function targetConvertorDirective($scope, ctrl, $attrs) {
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
                        $type: "Retail.BusinessEntity.RingoExtensions.RingoFileAccountConvertor, Retail.BusinessEntity.RingoExtensions"

                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
