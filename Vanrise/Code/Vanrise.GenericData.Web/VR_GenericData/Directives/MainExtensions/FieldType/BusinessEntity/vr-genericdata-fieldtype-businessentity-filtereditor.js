﻿(function (app) {

    'use strict';

    BusinessEntityFieldTypeFilterEditorDirective.$inject = [];

    function BusinessEntityFieldTypeFilterEditorDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var businessEntityFieldTypeFilterEditor = new BusinessEntityFieldTypeFilterEditor(ctrl, $scope, $attrs);
                businessEntityFieldTypeFilterEditor.initializeController();
            },
            controllerAs: 'filterEditorCtrl',
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
                    var returnValue;
                    var directiveData = directiveAPI.getData();

                    if (directiveData != undefined) {
                        returnValue = {
                            $type: 'Vanrise.GenericData.MainExtensions.DataRecordFields.Filters.BusinessEntityFieldTypeFilter, Vanrise.GenericData.MainExtensions',
                            BusinessEntityIds: directiveData
                        };
                    }

                    return returnValue;
                };
                return api;
            }
        }

        function getDirectiveTemplate(attrs) {
            return '<vr-genericdata-fieldtype-businessentity-runtimeeditor on-ready="filterEditorCtrl.onDirectiveReady" selectionmode="multiple" normal-col-num="{{filterEditorCtrl.normalColNum}}" isrequired="filterEditorCtrl.isrequired" />';
        }
    }

    app.directive('vrGenericdataFieldtypeBusinessentityFiltereditor', BusinessEntityFieldTypeFilterEditorDirective);

})(app);