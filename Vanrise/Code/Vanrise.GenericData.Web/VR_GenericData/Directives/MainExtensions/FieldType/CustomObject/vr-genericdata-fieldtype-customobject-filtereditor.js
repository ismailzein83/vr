(function (app) {

    'use strict';

    CustomobjectFieldTypeFilterEditorDirective.$inject = [];

	function CustomobjectFieldTypeFilterEditorDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var customobjectFieldTypeFilterEditor = new CustomobjectFieldTypeFilterEditor(ctrl, $scope, $attrs);
				customobjectFieldTypeFilterEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
				return getDirectiveTemplate(attrs);
            }
        };

		function CustomobjectFieldTypeFilterEditor(ctrl, $scope, $attrs) {
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
            return '<vr-genericdata-fieldtype-customobject-runtimeeditor on-ready="ctrl.onDirectiveReady" selectionmode="single" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" />';
        }
    }

	app.directive('vrGenericdataFieldtypeCustomobjectFiltereditor', CustomobjectFieldTypeFilterEditorDirective);

})(app);