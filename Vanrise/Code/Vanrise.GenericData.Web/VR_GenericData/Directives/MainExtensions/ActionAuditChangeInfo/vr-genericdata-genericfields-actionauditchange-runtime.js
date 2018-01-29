'use strict';
app.directive('vrGenericdataGenericfieldsActionauditchangeRuntime', ['UtilsService',
    function (UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new genericFieldsActionAuditChangeInfoRuntimeCtor(ctrl, $scope);
                ctor.initializeController();

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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/ActionAuditChangeInfo/Templates/GenericFieldsActionAuditChangeInfoRuntime.html';
            }
        };

        function genericFieldsActionAuditChangeInfoRuntimeCtor(ctrl, $scope) {

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    console.log(payload);
                    var vrActionAuditChangeInfoDefinition;
                    var vrActionAuditChangeInfo
                    if (payload != undefined) {
                        vrActionAuditChangeInfoDefinition = payload.vrActionAuditChangeInfoDefinition;
                        vrActionAuditChangeInfo = payload.vrActionAuditChangeInfo;

                        if (vrActionAuditChangeInfoDefinition != undefined) {
                            if (vrActionAuditChangeInfoDefinition.FieldTypes != undefined && vrActionAuditChangeInfo != undefined)
                            {

                            }
                           
                        }
                    }
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
    }]);