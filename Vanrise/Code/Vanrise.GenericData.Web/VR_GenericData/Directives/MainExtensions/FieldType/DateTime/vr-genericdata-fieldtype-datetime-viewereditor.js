﻿'use strict';
app.directive('vrGenericdataFieldtypeDatetimeViewereditor', ['UtilsService',
    function (UtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            var ctor = new datetimeTypeViewerEditorCtor(ctrl, $scope);
            ctor.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: function (element, attrs) {
            return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Datetime/Templates/DatetimeFieldTypeViewerEditorTemplate.html';
        }
    };

    function datetimeTypeViewerEditorCtor(ctrl, $scope) {

        function initializeController() {
            $scope.scopeModel = {};
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var changeInfo;
                if (payload != undefined) {
                    changeInfo = payload.changeInfo;
                    if(changeInfo != undefined)
                    {
                        $scope.scopeModel.fieldValueDescription = payload.fieldValueDescription;

                    }
                }
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                ctrl.onReady(api);
            }
        }

        this.initializeController = initializeController;
    }
 }]);