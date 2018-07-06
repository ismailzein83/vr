//(function (app) {

//    'use strict';

//    QueueActivatorAccumulateData.$inject = ['UtilsService', 'VRUIUtilsService'];

//    function QueueActivatorAccumulateData(UtilsService, VRUIUtilsService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '='
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var accumulateDataActivatorCtor = new AccumulateDataActivatorCtor(ctrl, $scope);
//                accumulateDataActivatorCtor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: function (element, attrs) {
//                return '/Client/Modules/Retail_Data/Directives/MainExtensions/QueueActivators/Templates/QueueActivatorAccumulateDataTemplate.html';
//            }
//        };

//        function AccumulateDataActivatorCtor(ctrl, $scope) {
//            this.initializeController = initializeController;

//            function initializeController() {
//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];
//                    return UtilsService.waitMultiplePromises(promises);
//                };

//                api.getData = function () {
//                    return {
//                        $type: 'Retail.Data.MainExtensions.QueueActivators.AccumulateDataActivator, Retail.Data.MainExtensions'
//                    };
//                };

//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') 
//                    ctrl.onReady(api);
//            }
//        }
//    }
//    app.directive('retailDataQueueactivatorAccumulatedata', QueueActivatorAccumulateData);
//})(app);