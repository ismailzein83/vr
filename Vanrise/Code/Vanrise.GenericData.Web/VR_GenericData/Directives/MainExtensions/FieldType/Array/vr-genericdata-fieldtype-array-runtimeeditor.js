(function (app) {

    'use strict';

    ArrayFieldTypeRuntimeEditorDirective.$inject = ['UtilsService'];

    function ArrayFieldTypeRuntimeEditorDirective(UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectionmode: '@',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var arrayFieldTypeRuntimeEditor = new ArrayFieldTypeRuntimeEditor(ctrl, $scope, $attrs);
                arrayFieldTypeRuntimeEditor.initializeController();
            },
            controllerAs: 'runtimeCtrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function ArrayFieldTypeRuntimeEditor(ctrl, $scope, $attrs) {

            var fieldTypeDirectiveAPI;

            function initializeController() {
                ctrl.onFieldTypeDirectiveReady = function (api) {
                    fieldTypeDirectiveAPI;

                    if (ctrl.onReady != undefined) {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var fieldType;
                    var fieldValue;

                    if (payload != undefined) {
                        $scope.scopeModel.label = payload.fieldTitle;
                        fieldType = payload.fieldType;
                        fieldValue = payload.fieldValue;
                    }

                    if (fieldValue != undefined) {
                        if (ctrl.selectionmode == "dynamic") {
                            angular.forEach(fieldValue.Values, function (val) {
                                $scope.scopeModel.values.push(val);
                            });
                        }
                        else {
                            $scope.scopeModel.value = fieldValue;
                        }
                    }
                }

                api.getData = function () {
                    var retVal;

                    if (ctrl.selectionmode == "dynamic") {
                        if ($scope.scopeModel.values.length > 0) {
                            retVal = {
                                $type: "Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions",
                                Values: $scope.scopeModel.values
                            };
                        }
                    }
                    else {
                        retVal = $scope.scopeModel.value;
                    }

                    return retVal;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }

        function getDirectiveTemplate(attrs) {

            var selectionMode = (attrs.selectionmode == 'single') ? 'single' : 'multiple';

            return '<vr-columns colnum="{{runtimeCtrl.normalColNum}}">'
                        + '<vr-directivewrapper directive="runtimeCtrl.fieldTypeDirectiveEditor" on-ready="runtimeCtrl.onFieldTypeDirectiveReady" selectionmode="' + selectionMode + '" isrequired="runtimeCtrl.isrequired" />'
                + '</vr-columns>';
        }
    }

    app.directive('vrGenericdataFieldtypeArrayRuntimeeditor', ArrayFieldTypeRuntimeEditorDirective);

})(app);