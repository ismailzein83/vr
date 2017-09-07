(function (app) {

    'use strict';

    DictionaryFieldTypeFilterEditorDirective.$inject = [];

    function DictionaryFieldTypeFilterEditorDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DictionaryFieldTypeFilterEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function DictionaryFieldTypeFilterEditor(ctrl, $scope, $attrs) {
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
            return '<vr-genericdata-fieldtype-dictionary-runtimeeditor on-ready="ctrl.onDirectiveReady" selectionmode="single" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" />';
        }
    }
    
    app.directive('vrGenericdataFieldtypeDictionaryFiltereditor', DictionaryFieldTypeFilterEditorDirective);

})(app);