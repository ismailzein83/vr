"use strict";

app.directive("retailBeAccountextrafielddefinitionsManagement", ["UtilsService", "VRNotificationService", "Retail_BE_AccountBEDefinitionService",
    function (UtilsService, VRNotificationService, Retail_BE_AccountBEDefinitionService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountExtraFieldDefinitionManagementCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountExtraFieldDefinition/Templates/AccountExtraFieldDefinitionsTemplate.html"

        };

        function AccountExtraFieldDefinitionManagementCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;

            var gridAPI;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.extraFieldDefinitions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
                $scope.scopeModel.onAddExtraFieldDefinition = function () {
                    var onAccountExtraFieldDefinitionAdded = function (addedExtraFieldDefinition) {
                        $scope.scopeModel.extraFieldDefinitions.push({ Entity: addedExtraFieldDefinition });
                    };

                    Retail_BE_AccountBEDefinitionService.addAccountExtraFieldDefinition(accountBEDefinitionId, onAccountExtraFieldDefinitionAdded);
                };
                $scope.scopeModel.onDeleteExtraFieldDefinition = function (extraFieldDefinition) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = $scope.scopeModel.extraFieldDefinitions.indexOf(extraFieldDefinition);
                            $scope.scopeModel.extraFieldDefinitions.splice(index, 1);
                        }
                    });
                };
                defineMenuActions();

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;

                        if (payload.accountExtraFieldDefinitions != undefined) {
                            for (var i = 0; i < payload.accountExtraFieldDefinitions.length; i++) {
                                var accountExtraFieldDefinition = payload.accountExtraFieldDefinitions[i];
                                $scope.scopeModel.extraFieldDefinitions.push({ Entity: accountExtraFieldDefinition });
                            }
                        }
                    }
                };

                api.getData = function () {
                    var accountExtraFieldDefinitions;
                    if ($scope.scopeModel.extraFieldDefinitions != undefined) {
                        accountExtraFieldDefinitions = [];
                        for (var i = 0; i < $scope.scopeModel.extraFieldDefinitions.length; i++) {
                            var currentItem = $scope.scopeModel.extraFieldDefinitions[i];
                            accountExtraFieldDefinitions.push(currentItem.Entity);
                        }
                    }
                    return accountExtraFieldDefinitions;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editAccountExtraFieldDefinition,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editAccountExtraFieldDefinition(accountExtraFieldDefinitionObj) {
                var onAccountExtraFieldDefinitionUpdated = function (accountExtraFieldDefinition) {
                    var index = $scope.scopeModel.extraFieldDefinitions.indexOf(accountExtraFieldDefinitionObj);
                    $scope.scopeModel.extraFieldDefinitions[index] = { Entity: accountExtraFieldDefinition };
                };
                Retail_BE_AccountBEDefinitionService.editAccountExtraFieldDefinition(accountExtraFieldDefinitionObj.Entity, accountBEDefinitionId, onAccountExtraFieldDefinitionUpdated);
            }
        }

        return directiveDefinitionObject;

    }
]);