'use strict';

app.directive('networkprovisionHandlertypeExtendedsettingsCustomcode', ['UtilsService',
    function (UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CustomCodeRuntime($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/NetworkProvision/Elements/HandlerType/Directives/MainExtensions/CustomCode/Templates/CustomCodeHandlerTypeTemplate.html'
        };

        function CustomCodeRuntime($scope, ctrl, $attrs) {
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
                        $type: "NetworkProvision.Business.CustomCodeNetworkProvisionHandlerType, NetworkProvision.Business"
                    };
                };

                if (ctrl.onReady != undefined)
                    ctrl.onReady(api);
            }
        }
    }
]);