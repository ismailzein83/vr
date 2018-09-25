'use strict';
app.directive('vrGenericdataGenericrulesearchCompositecondition', ['UtilsService', 'VR_GenericData_DataRecordFieldAPIService', 'VRUIUtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VR_GenericData_GenericUIService',
    function (UtilsService, VR_GenericData_DataRecordFieldAPIService, VRUIUtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VR_GenericData_GenericUIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                advancedselected: '=',
                basicselected: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new searchCompositeConditionCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'compositeConditionCtrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/GenericRule/Search/Templates/SearchCompositeConditionTemplate.html';
            }
        };

        function searchCompositeConditionCtor(ctrl, $scope) {
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

                api.addGenericRule = function () {
                    console.log('Not yet implemented');
                };


                api.uploadGenericRules = function (uploadRulesObj) {
                    console.log('Not yet implemented');
                };

                api.getSearchObject = function () {
                    console.log('Not yet implemented');
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }]);