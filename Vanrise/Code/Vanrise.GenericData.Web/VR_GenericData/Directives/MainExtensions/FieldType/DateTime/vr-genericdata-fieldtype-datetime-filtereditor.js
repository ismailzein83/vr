(function (app) {

    'use strict';

    DateTimeFieldTypeFilterEditorDirective.$inject = [];

    function DateTimeFieldTypeFilterEditorDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dateTimeFieldTypeFilterEditor = new DateTimeFieldTypeFilterEditor(ctrl, $scope, $attrs);
                dateTimeFieldTypeFilterEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function DateTimeFieldTypeFilterEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var directiveAPI;

            function initializeController() {
                ctrl.onDirectiveReady = function (api) {
                    directiveAPI = api;

                    if (ctrl.onReady != undefined) {
                        ctrl.onReady(getDirectiveAPI());
                    }
                }
            }

            function getDirectiveAPI() {
                var api = {};
                api.load = function (payload) {
                    return directiveAPI.load(payload);
                };
                api.getData = function () {
                    return directiveAPI.getData();
                };
                return api;
            }
        }

        function getDirectiveTemplate(attrs) {
            return '<vr-genericdata-fieldtype-datetime-runtimeeditor on-ready="ctrl.onDirectiveReady" selectionmode="single" normal-col-num="{{ctrl.normalColNum}}" />';
        }
    }

    app.directive('vrGenericdataFieldtypeDatetimeFiltereditor', DateTimeFieldTypeFilterEditorDirective);

})(app);