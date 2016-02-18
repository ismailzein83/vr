(function (app) {

    'use strict';

    BusinessEntityFieldTypeFilterEditorDirective.$inject = [];

    function BusinessEntityFieldTypeFilterEditorDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var businessEntityFieldTypeFilterEditor = new BusinessEntityFieldTypeFilterEditor(ctrl, $scope, $attrs);
                businessEntityFieldTypeFilterEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function BusinessEntityFieldTypeFilterEditor(ctrl, $scope, $attrs) {
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
                        $type: 'Vanrise.GenericData.MainExtensions.DataRecordFields.BusinessEntityFieldTypeFilter, Vanrise.GenericData.MainExtensions',
                        BusinessEntityIds: directiveAPI.getData()
                    };
                };
                return api;
            }
        }

        function getDirectiveTemplate(attrs) {
            return '<vr-genericdata-businessentity-runtimeeditor on-ready="ctrl.onDirectiveReady" selectionmode="multiple" normal-col-num="ctrl.normalColNum" />';
        }
    }

    app.directive('vrGenericdataFieldtypeBusinessentityFiltereditor', BusinessEntityFieldTypeFilterEditorDirective);

})(app);