﻿(function (app) {

    'use strict';

    NumericFieldTypeFilterEditorDirective.$inject = [];

    function NumericFieldTypeFilterEditorDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
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
            return '<vr-genericdata-numeric-runtimeeditor on-ready="ctrl.onDirectiveReady" selectionmode="single" />';
        }
    }

    app.directive('vrGenericdataFieldtypeNumericFiltereditor', NumericFieldTypeFilterEditorDirective);

})(app);