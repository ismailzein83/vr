(function (app) {

    'use strict';

    NumericFieldTypeFilterEditorDirective.$inject = [];

    function NumericFieldTypeFilterEditorDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var numericFieldTypeFilterEditor = new NumericFieldTypeFilterEditor(ctrl, $scope, $attrs);
                numericFieldTypeFilterEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function NumericFieldTypeFilterEditor(ctrl, $scope, $attrs) {
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
                    var data = directiveAPI.getData();
                    if (data != undefined)
                        return data.Values;
                };
                api.getData = function () {

                    return directiveAPI.getData();
                };
                return api;
            }
        }

        function getDirectiveTemplate(attrs) {
            return '<vr-genericdata-fieldtype-number-runtimeeditor on-ready="ctrl.onDirectiveReady" selectionmode="single" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" />';
        }
    }

    app.directive('vrGenericdataFieldtypeNumberFiltereditor', NumericFieldTypeFilterEditorDirective);

})(app);