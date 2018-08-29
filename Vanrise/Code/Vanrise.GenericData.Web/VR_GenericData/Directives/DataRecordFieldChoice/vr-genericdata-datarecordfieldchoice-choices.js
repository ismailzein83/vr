'use strict';
app.directive('vrGenericdataDatarecordfieldchoiceChoices', ['UtilsService', '$compile', 'VRUIUtilsService',
function (UtilsService, $compile, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.extraChargeTemplates = [];
            var ctor = new ChoicesGrid(ctrl, $scope, $attrs);
            ctor.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_GenericData/Directives/DataRecordFieldChoice/Templates/ChoicesGridTemplate.html"

    };


    function ChoicesGrid(ctrl, $scope, $attrs) {

        function initializeController() {

            ctrl.datasource = [];

            ctrl.isValid = function () {
                if (ctrl.datasource.length > 0)
                    return null;
                return "You Should Add At Least One Choice.";
            };
            ctrl.addChoice = function () {
                var dataItem = {
                    validate: function (item) {
                        return validation(item);
                    }
                };
                ctrl.datasource.push({ Entity: dataItem });
            };
            ctrl.removeChoice = function (dataItem) {
                var index = ctrl.datasource.indexOf(dataItem);
                ctrl.datasource.splice(index, 1);
            };

            defineAPI();

        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var choices = [];
                for (var i = 0, length = ctrl.datasource.length; i < length ; i++) {
                    var choice = ctrl.datasource[i];
                    choices.push(choice.Entity);
                }
                return choices;
            };

            api.load = function (payload) {
                var promises = [];

                if (payload != undefined) {
                    var choices = payload.choices;
                    if (choices != undefined) {
                        for (var i = 0, length = choices.length; i < length; i++) {
                            var dataItem = choices[i];
                            dataItem.validate = function (item) {
                                return validation(item);
                            };
                            ctrl.datasource.push({ Entity: dataItem });
                        }
                    }
                }
                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        function validation(item)
        {
            for (var i = 0, length = ctrl.datasource.length; i < length; i++) {
                var currentChoice = ctrl.datasource[i];
                for (var j = i + 1, sourcelength = ctrl.datasource.length; j < sourcelength; j++) {
                    var choice = ctrl.datasource[j];
                    if (item.Entity.Value == choice.Entity.Value && choice.Entity.Value == currentChoice.Entity.Value) {
                        return "Same value exists.";
                    }
                }
            }
            return null;
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);