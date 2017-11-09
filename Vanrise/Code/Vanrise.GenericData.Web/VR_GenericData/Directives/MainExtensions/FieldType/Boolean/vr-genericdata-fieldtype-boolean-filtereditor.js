(function (app) {

    'use strict';

    BooleanFieldTypeFilterEditorDirective.$inject = [];

    function BooleanFieldTypeFilterEditorDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var booleanFieldTypeFilterEditor = new BooleanFieldTypeFilterEditor(ctrl, $scope, $attrs);
                booleanFieldTypeFilterEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function BooleanFieldTypeFilterEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [{ Text: "True", Value: "True" }, { Text: "False", Value: "False" }];

                ctrl.onSelectorReady = function (api) {
                    if (ctrl.onReady != undefined) {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.label = payload != undefined ? payload.fieldTitle : "";
                };

                api.getData = function () {
                    var data = ctrl.selectedvalues;
                    if (data == undefined)
                        return undefined;
                    return data.Value;
                };

                api.getValuesAsArray = function () {
                    var value = api.getData();
                    if (value == undefined)
                        return undefined;
                    return [value];
                };

                return api;
            }
        }

        function getDirectiveTemplate(attrs) {
            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                       '<vr-select datatextfield="Text" datavaluefield="Value" isrequired="ctrl.isrequired" label="{{ctrl.label}}"' +
                           '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged"' +
                           '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" customvalidate="ctrl.customvalidate">' +
                       '</vr-select>' +
                   '</vr-columns>';
        }
    }

    app.directive('vrGenericdataFieldtypeBooleanFiltereditor', BooleanFieldTypeFilterEditorDirective);

})(app);