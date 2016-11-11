(function (app) {

    'use strict';

    TextFieldTypeFilterEditorDirective.$inject = [];

    function TextFieldTypeFilterEditorDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var textFieldTypeFilterEditor = new TextFieldTypeFilterEditor(ctrl, $scope, $attrs);
                textFieldTypeFilterEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function TextFieldTypeFilterEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var directiveAPI;

            function initializeController() {
                ctrl.onDirectiveReady = function (api) {
                    directiveAPI = api;

                    if (ctrl.onReady != undefined) {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};
                api.load = function (payload) {
                    return directiveAPI.load(payload);
                };
                api.getValuesAsArray = function () {
                    var values = [];
                    values.push(directiveAPI.getData());
                    return values;
                };
                api.getData = function () {
                    return directiveAPI.getData();
                };
                return api;
            }
        }

        function getDirectiveTemplate(attrs) {
            return '<vr-genericdata-fieldtype-text-runtimeeditor on-ready="ctrl.onDirectiveReady" selectionmode="single" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" />';
        }
    }

    app.directive('vrGenericdataFieldtypeTextFiltereditor', TextFieldTypeFilterEditorDirective);

})(app);