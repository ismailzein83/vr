//(function (app) {

//    'use strict';

//    GenericBEAccountViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_ActionDefinitionService', 'Retail_BE_EntityTypeEnum'];

//    function GenericBEAccountViewDirective(UtilsService, VRNotificationService, Retail_BE_ActionDefinitionService, Retail_BE_EntityTypeEnum) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new ActionsViewCtor($scope, ctrl);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            compile: function (element, attrs) {
//                return {
//                    pre: function ($scope, iElem, iAttrs, ctrl) {

//                    }
//                };
//            },
//            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountViews/MainExtensions/Templates/GenericBEAccountViewTemplate.html'
//        };

//        function ActionsViewCtor($scope, ctrl) {
//            this.initializeController = initializeController;


//            var gridAPI;

//            function initializeController() {
//                $scope.scopeModel = {};

//            }
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {

//                    if (payload != undefined) {
//                        accountBEDefinitionId = payload.accountBEDefinitionId;
//                        parentAccountId = payload.parentAccountId;
//                    }

//                };

//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }

//        }
//    }

//    app.directive('retailBeGenericbeaccountView', GenericBEAccountViewDirective);

//})(app);