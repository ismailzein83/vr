//(function (app) {

//    'use strict';

//    ChangeStatusGenericBEDefinitionActionDirective.$inject = ['UtilsService', 'VRNotificationService'];

//    function ChangeStatusGenericBEDefinitionActionDirective(UtilsService, VRNotificationService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new ChangeStatusGenericBEDefinitionActionCtor($scope, ctrl);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEActionDefinition/Templates/ChangeStatusGenericBEDefinitionActionTemplate.html'
//        };

//        function ChangeStatusGenericBEDefinitionActionCtor($scope, ctrl) {
//            this.initializeController = initializeController;
//            function initializeController() {
//                $scope.scopeModel = {};
//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var context = payload.context;
//                    if (context != undefined && context.showSecurityGridCallBack != undefined && typeof (context.showSecurityGridCallBack) == 'function')
//                        context.showSecurityGridCallBack(true);
//                };


//                api.getData = function () {
//                    return {
//                        $type: "TestCallAnalysis.Business.CaseCDRChangeStatusAction, TestCallAnalysis.Business"
//                    };
//                };
//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }
//        }
//    }

//    app.directive('vrGenericdataGenericbeChangestatusgenericactionDefinition', ChangeStatusGenericBEDefinitionActionDirective);

//})(app);