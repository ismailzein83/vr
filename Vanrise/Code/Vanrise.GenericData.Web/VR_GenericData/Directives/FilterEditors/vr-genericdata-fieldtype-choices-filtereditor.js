(function (app) {

    'use strict';

    ChoicesFieldTypeFilterEditorDirective.$inject = [];

    function ChoicesFieldTypeFilterEditorDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var choicesFieldTypeFilterEditor = new ChoicesFieldTypeFilterEditor(ctrl, $scope, $attrs);
                choicesFieldTypeFilterEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function ChoicesFieldTypeFilterEditor(ctrl, $scope, $attrs) {
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
                    return {
                        $type: 'Vanrise.GenericData.MainExtensions.DataRecordFields.ChoicesFieldTypeFilter, Vanrise.GenericData.MainExtensions',
                        ChoiceIds: directiveAPI.getData()
                    };
                };
                return api;
            }
        }

        function getDirectiveTemplate(attrs) {
            return '<vr-genericdata-choices-runtimeeditor on-ready="ctrl.onDirectiveReady" selectionmode="multiple" normal-col-num="ctrl.normalColNum" />';
        }
    }

    app.directive('vrGenericdataFieldtypeChoicesFiltereditor', ChoicesFieldTypeFilterEditorDirective);

})(app);