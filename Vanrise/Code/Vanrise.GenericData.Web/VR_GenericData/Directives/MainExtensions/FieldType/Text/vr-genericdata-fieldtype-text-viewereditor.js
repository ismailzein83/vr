'use strict';
app.directive('vrGenericdataFieldtypeTextViewereditor', ['UtilsService',
    function (UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new textTypeViewerEditorCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Text/Templates/TextFieldTypeViewerEditorTemplate.html';
            }
        };

        function textTypeViewerEditorCtor(ctrl, $scope) {

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var changeInfo;
                    if (payload != undefined) {
                        changeInfo = payload.changeInfo;
                        if (changeInfo != undefined) {
                            $scope.scopeModel.oldValue = changeInfo.OldValue;
                            $scope.scopeModel.newValue = changeInfo.NewValue;
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