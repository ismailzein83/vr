(function (app) {

    'use strict';

    ChoicesFieldTypeFilterEditorDirective.$inject = [];

    function ChoicesFieldTypeFilterEditorDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
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
                    var returnValue;
                    var directiveData = directiveAPI.getData();
                    
                    if (directiveData != undefined) {
                        returnValue = {
                            $type: 'Vanrise.GenericData.MainExtensions.DataRecordFields.Filters.ChoicesFieldTypeFilter, Vanrise.GenericData.MainExtensions',
                            ChoiceIds: directiveData
                        };
                    }

                    return returnValue;
                };
                return api;
            }
        }

        function getDirectiveTemplate(attrs) {
            return '<vr-genericdata-fieldtype-choices-runtimeeditor on-ready="ctrl.onDirectiveReady" selectionmode="multiple" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" />';
        }
    }

    app.directive('vrGenericdataFieldtypeChoicesFiltereditor', ChoicesFieldTypeFilterEditorDirective);

})(app);