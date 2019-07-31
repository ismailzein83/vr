(function (appControllers) {

    "use strict";

    ExpressionBuilderEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function ExpressionBuilderEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var textAreaAPI;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            $scope.scopeModel = {};
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                $scope.scopeModel.expressionValue = parameters.ExpressionValue;

                $scope.scopeModel.arguments = parameters.Arguments;
                if ($scope.scopeModel.arguments != undefined && $scope.scopeModel.arguments.length > 0) {
                    $scope.scopeModel.arguments.sort(sortByName);
                }

                $scope.scopeModel.variables = parameters.Variables;
                if ($scope.scopeModel.variables != undefined && $scope.scopeModel.variables.length > 0) {
                    $scope.scopeModel.variables.sort(sortByName);
                }
            }
        }

        function defineScope() {

            $scope.scopeModel.onTextAreaReady = function (api) {
                textAreaAPI = api;
            };

            $scope.scopeModel.saveExpressionValue = function () {
                return setExpressionValue();
            };

            $scope.scopeModel.insertText = function (item) {
                if (textAreaAPI != undefined) {
                    textAreaAPI.appendAtCursorPosition(item.Name);
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.filterValueChanged = function (value) {
                if (value == undefined || value.length == 0) {
                    setHideItemsFalse($scope.scopeModel.variables);
                    setHideItemsFalse($scope.scopeModel.arguments);
                    return;
                }

                var filter = value.toLowerCase();

                for (var i = 0; i < $scope.scopeModel.variables.length; i++) {
                    var variable = $scope.scopeModel.variables[i].Name.toLowerCase();
                    if (variable.indexOf(filter) == -1)
                        $scope.scopeModel.variables[i].hideItem = true;
                    else
                        $scope.scopeModel.variables[i].hideItem = false;
                }

                for (var j = 0; j < $scope.scopeModel.arguments.length; j++) {
                    var argument = $scope.scopeModel.arguments[j].Name.toLowerCase();
                    if (argument.indexOf(filter) == -1)
                        $scope.scopeModel.arguments[j].hideItem = true;
                    else
                        $scope.scopeModel.arguments[j].hideItem = false;
                }
            };
        }

        function load() {
            $scope.title = UtilsService.buildTitleForUpdateEditor('Expression Builder');
        }

        function setExpressionValue() {
            if ($scope.onSetExpressionBuilder != undefined)
                $scope.onSetExpressionBuilder($scope.scopeModel.expressionValue);
            $scope.modalContext.closeModal();
        }

        function setHideItemsFalse(objectsList) {

            if (objectsList == undefined || objectsList.length == 0)
                return;

            for (var i = 0; i < objectsList.length; i++)
                objectsList[i].hideItem = false;
        }

        function sortByName(a, b) {
            var nameA = a.Name.toLowerCase();
            var nameB = b.Name.toLowerCase();
            if (nameA < nameB) {
                return -1;
            }
            if (nameA > nameB) {
                return 1;
            }
            return 0;
        }
    }

    appControllers.controller('BusinessProcess_VRWorkflow_ExpressionBuilderController', ExpressionBuilderEditorController);
})(appControllers);