'use strict';

app.directive('retailBeVisibilityactionManagement', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VisibilityAction(ctrl, $scope);
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
                return '/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/AccountDefinitionAction/Templates/VisibilityActionManagementTemplate.html';
            }
        };

        function VisibilityAction(ctrl, $scope) {

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.actionDefinitions = [];
                $scope.scopeModel.selectedActionDefinitions = [];
                $scope.scopeModel.actions = [];

                $scope.scopeModel.onSelectActionDefinition = function (selectedItem) {

                    $scope.scopeModel.actions.push({
                        AccountActionDefinitionId: selectedItem.AccountActionDefinitionId,
                        Name: selectedItem.Name
                    });
                };
                $scope.scopeModel.onDeselectActionDefinition = function (deselectedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.actions, deselectedItem.AccountActionDefinitionId, 'AccountActionDefinitionId');
                    $scope.scopeModel.actions.splice(index, 1);
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedActionDefinitions, deletedItem.AccountActionDefinitionId, 'AccountActionDefinitionId');
                    $scope.scopeModel.selectedActionDefinitions.splice(index, 1);
                    $scope.scopeModel.onDeselectActionDefinition(deletedItem);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var actions;
                    var actionDefinitions;

                    if (payload != undefined) {
                        actions = payload.actions;
                        actionDefinitions = payload.actionDefinitions;
                    }

                    //Loading Selector
                    if (actionDefinitions != undefined) {
                        for (var i = 0; i < actionDefinitions.length; i++) {
                            $scope.scopeModel.actionDefinitions.push(actionDefinitions[i]);
                        }
                        if (actions != undefined) {
                            for (var i = 0; i < actions.length; i++) {
                                var currentAction = actions[i];
                                for (var j = 0; j < actionDefinitions.length; j++) {
                                    var currentActionDefinition = actionDefinitions[j];
                                    if (currentActionDefinition.AccountActionDefinitionId == currentAction.ActionId)
                                        $scope.scopeModel.selectedActionDefinitions.push(currentActionDefinition);
                                }
                            }
                        }
                    }

                    //Loading Grid
                    if ($scope.scopeModel.selectedActionDefinitions != undefined) {
                        for (var i = 0; i < $scope.scopeModel.selectedActionDefinitions.length; i++) {
                            var actionDefinition = $scope.scopeModel.selectedActionDefinitions[i];
                            var action = actions[i];

                            $scope.scopeModel.actions.push({
                                AccountActionDefinitionId: actionDefinition.AccountActionDefinitionId,
                                Name: actionDefinition.Name,
                                Title: action.Title
                            });
                        }
                    }
                };

                api.getData = function () {

                    var _actions;
                    if ($scope.scopeModel.actions.length > 0) {
                        _actions = [];
                        for (var i = 0; i < $scope.scopeModel.actions.length; i++) {
                            var currentAction = $scope.scopeModel.actions[i];
                            _actions.push({
                                ActionId: currentAction.AccountActionDefinitionId,
                                Title: currentAction.Title
                            });
                        }
                    }
                    return _actions
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);