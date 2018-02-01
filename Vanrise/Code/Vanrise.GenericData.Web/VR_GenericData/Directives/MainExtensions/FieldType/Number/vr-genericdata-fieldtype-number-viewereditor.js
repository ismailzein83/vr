﻿'use strict';
app.directive('vrGenericdataFieldtypeNumberViewereditor', ['UtilsService',
    function (UtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            var ctor = new numberTypeViewerEditorCtor(ctrl, $scope);
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
            return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Number/Templates/NumberFieldTypeViewerEditorTemplate.html';
        }
    };

    function numberTypeViewerEditorCtor(ctrl, $scope) {

        function initializeController() {
            $scope.scopeModel = {};
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.scopeModel.fieldValueDescription = payload.fieldValueDescription;
                }
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                ctrl.onReady(api);
            }
        }

        this.initializeController = initializeController;
    }
 }]);