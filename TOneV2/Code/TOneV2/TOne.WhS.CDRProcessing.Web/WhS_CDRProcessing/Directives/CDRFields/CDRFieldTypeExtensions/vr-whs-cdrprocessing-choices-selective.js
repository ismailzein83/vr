'use strict';
app.directive('vrWhsCdrprocessingChoicesSelective', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

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
                return '/Client/Modules/WhS_CDRProcessing/Directives/CDRFields/CDRFieldTypeExtensions/Templates/SelectiveChoicesDirectiveTemplate.html';
            }

        };

        function choicesTypeCtor(ctrl, $scope) {

            function initializeController() {
                ctrl.values = [];
                ctrl.disableAddButton = true;
                ctrl.addValue = function () {
                    ctrl.values.push(AddChoice(ctrl.value));
                    ctrl.value = undefined;
                    ctrl.disableAddButton = true;
                }
                ctrl.onValueChange = function (value) {
                    ctrl.disableAddButton = value == undefined || UtilsService.getItemIndexByVal(ctrl.values, ctrl.value, "Choice")!=-1;
                }
                defineAPI();
            }
            function AddChoice(choice) {
                var obj = {
                    Choice: choice
                }
                return obj;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.values = payload.Choices;
                    }
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.CDRProcessing.Entities.CDRFieldChoicesType, TOne.WhS.CDRProcessing.Entities",
                        Choices: $scope.values
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);