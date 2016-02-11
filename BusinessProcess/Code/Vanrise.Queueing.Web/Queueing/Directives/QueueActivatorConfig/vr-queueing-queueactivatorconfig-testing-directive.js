'use strict';
app.directive('vrQueueingQueueactivatorconfigTestingDirective', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new choicesTypeCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Queueing/Directives/QueueActivatorConfig/Templates/QueueActivatorconfigTestingDirectiveTemplate.html';
            }

        };

        function choicesTypeCtor(ctrl, $scope) {

            function initializeController() {

                defineAPI();
            }

             
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.DummyProp != undefined) {
                        ctrl.Name = payload.DummyProp;
                    }
                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.Queueing.Entities.TestActivator, Vanrise.Queueing.Entities",
                        DummyProp: ctrl.Name
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);