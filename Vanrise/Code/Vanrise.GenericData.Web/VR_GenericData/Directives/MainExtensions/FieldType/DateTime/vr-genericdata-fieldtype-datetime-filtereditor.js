(function (app) {

    'use strict';

    DateTimeFieldTypeFilterEditorDirective.$inject = [];

    function DateTimeFieldTypeFilterEditorDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dateTimeFieldTypeFilterEditor = new DateTimeFieldTypeFilterEditor(ctrl, $scope, $attrs);
                dateTimeFieldTypeFilterEditor.initializeController();
            },
            controllerAs: 'filterEditorCtrl',
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
            return '<vr-genericdata-fieldtype-datetime-runtimeeditor on-ready="filterEditorCtrl.onDirectiveReady" selectionmode="single" normal-col-num="{{filterEditorCtrl.normalColNum}}" isrequired="filterEditorCtrl.isrequired" />';
        }
    }

    app.directive('vrGenericdataFieldtypeDatetimeFiltereditor', DateTimeFieldTypeFilterEditorDirective);

})(app);