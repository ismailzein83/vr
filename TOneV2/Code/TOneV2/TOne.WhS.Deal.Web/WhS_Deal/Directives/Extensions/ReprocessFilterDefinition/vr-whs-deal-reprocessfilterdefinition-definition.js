(function (app) {

    'use strict';

    DealReprocessFilterDefinitionDefinition.$inject = ['UtilsService'];

    function DealReprocessFilterDefinitionDefinition(UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DealReprocessFilterDefinitionDefinitionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Deal/Directives/Extensions/ReprocessFilterDefinition/Templates/DealReprocessFilterDefinitionTemplate.html'
        };

        function DealReprocessFilterDefinitionDefinitionCtor($scope, ctrl) {
            this.initializeController = initializeController;

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        var context = payload.context;

                        if (payload.filterDefinition == undefined) {
                            context.onFieldAdded("Customer");
                            context.onFieldAdded("Supplier");
                            context.onFieldAdded("Type");
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'TOne.WhS.Deal.MainExtensions.ReprocessFilterDefinition.DealReprocessFilterDefinition, TOne.WhS.Deal.MainExtensions'
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsDealReprocessfilterdefinitionDefinition', DealReprocessFilterDefinitionDefinition);
})(app);