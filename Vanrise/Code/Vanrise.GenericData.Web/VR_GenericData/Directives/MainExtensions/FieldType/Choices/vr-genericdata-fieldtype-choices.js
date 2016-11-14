'use strict';
app.directive('vrGenericdataFieldtypeChoices', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.values = [];
                var ctor = new choicesTypeCtor(ctrl, $scope);
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
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Choices/Templates/ChoicesFieldTypeTemplate.html';
            }

        };

        function choicesTypeCtor(ctrl, $scope) {

            function initializeController() {
                
                ctrl.Id = ctrl.values.length+1;
                ctrl.isValid = function () {
                    if (ctrl.values != undefined && ctrl.values.length > 0)
                        return null;
                    return "You Should Add At Least One Choice.";
                };
                ctrl.disableAddButton = true;
                ctrl.addValue = function () {
                    ctrl.values.push(AddChoice(ctrl.value));
                    ctrl.Id = ctrl.values.length + 1;
                    ctrl.value = undefined;
                    ctrl.disableAddButton = true;
                };
                ctrl.onValueChange = function (value) {
                    ctrl.disableAddButton = value == undefined || (UtilsService.getItemIndexByVal(ctrl.values, ctrl.value, "Text") != -1 || UtilsService.getItemIndexByVal(ctrl.values, ctrl.Id, "Value") != -1);
                };

                defineAPI();
            }

            function AddChoice(choice) {
                var obj = {
                    Value: ctrl.Id,
                    Text: choice
                };
                return obj;
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.Choices != undefined && payload.Choices.length > 0) {
                        ctrl.values = payload.Choices;
                        ctrl.isNullable = payload.IsNullable;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldChoicesType, Vanrise.GenericData.MainExtensions",
                        Choices: ctrl.values,
                        IsNullable: ctrl.isNullable
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);