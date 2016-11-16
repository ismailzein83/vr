(function (app) {

    'use strict';

    QueueActivatorDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function QueueActivatorDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var queueActivatorCtor = new QueueActivatorCtor(ctrl, $scope);
                queueActivatorCtor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Queueing/Directives/QueueActivator/Templates/QueueActivatorCustomActivatorTemplate.html';
            }
        };

        function QueueActivatorCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var dataRecordStorageSelectorAPI;

            function initializeController() {
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(getDirectiveAPI());
                }
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.QueueActivator != undefined) {
                        ctrl.fqtn = payload.QueueActivator.FQTN;
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.Queueing.Entities.CustomQueueActivator, Vanrise.Queueing.Entities',
                        FQTN: ctrl.fqtn
                    };
                };

                return api;
            }
        }
    }

    app.directive('vrQueueingQueueactivatorCustomactivator', QueueActivatorDirective);

})(app);