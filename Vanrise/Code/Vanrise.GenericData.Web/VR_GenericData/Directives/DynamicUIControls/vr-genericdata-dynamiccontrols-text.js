﻿'use strict';
app.directive('vrGenericdataDynamiccontrolsText', ['UtilsService', function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            selectionmode: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.values = [];

            var ctor = new textCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: "/Client/Modules/VR_GenericData/Directives/DynamicUIControls/Templates/TextEditorTemplate.html"
    };
    
    function textCtor(ctrl, $scope, $attrs) {

        function initializeController() {
            ctrl.showInMultipleMode = ($attrs.selectionmode == "multiple" || $attrs.selectionmode == "dynamic");

            ctrl.isValid = function () {
                if (ctrl.values != undefined && ctrl.values.length > 0)
                    return null;
                return "You should add at least one choice."
            }

            ctrl.disableAddButton = true;
            ctrl.addValue = function () {
                ctrl.values.push(ctrl.value);
                ctrl.value = undefined;
                ctrl.disableAddButton = true;
            }

            ctrl.onValueChange = function (value) {
                ctrl.disableAddButton = (value == undefined);
            }

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var fieldType;
                if (payload != undefined) {
                    fieldType = payload.fieldType;
                }
            }

            api.getData = function()
            {
                return {
                    $type: "Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions",
                    Values: ctrl.values
                }
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);

