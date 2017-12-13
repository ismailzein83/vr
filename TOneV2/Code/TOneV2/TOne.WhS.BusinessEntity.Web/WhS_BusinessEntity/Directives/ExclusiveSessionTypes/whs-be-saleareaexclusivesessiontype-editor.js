//'use strict';

//app.directive('whsBeSaleareaexclusivesessiontypeEditor', ['UtilsService', 'VRUIUtilsService',
//    function (UtilsService, VRUIUtilsService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new SaleAreaExclusiveSessionTypeCtor($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/ExclusiveSessionTypes/Templates/SaleAreaExclusiveSessionTypeEditorTemplate.html'
//        };

//        function SaleAreaExclusiveSessionTypeCtor($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            function initializeController() {
//                $scope.scopeModel = {};

//                defineAPI();
//            }
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];

//                    if (payload != undefined) {

//                    }

//                    return UtilsService.waitMultiplePromises(promises);
//                };

//                api.getData = function () {

//                    return {
//                        $type: "TOne.WhS.BusinessEntity.MainExtensions.SaleAreaExclusiveSessionType, TOne.WhS.BusinessEntity.MainExtensions"
//                    };
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }
//        }
//    }]);
