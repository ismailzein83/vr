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
                    return directiveAPI.getData();

                };
                api.getData = function () {
                    return directiveAPI.getData();
                };
                return api;
            }
        }

        function getDirectiveTemplate(attrs) {
            return '<vr-genericdata-fieldtype-boolean-runtimeeditor on-ready="ctrl.onDirectiveReady" selectionmode="single" normal-col-num="{{ctrl.normalColNum}}" />';
        }
    }

    app.directive('vrGenericdataFieldtypeBooleanFiltereditor', BooleanFieldTypeFilterEditorDirective);

})(app);