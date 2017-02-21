(function (app) {

    'use strict';

    VisibilityAccountDefinitionsManagementDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_VisibilityAccountDefinitionService', 'Retail_BE_AccountTypeAPIService'];

    function VisibilityAccountDefinitionsManagementDirective(UtilsService, VRNotificationService, Retail_BE_VisibilityAccountDefinitionService, Retail_BE_AccountTypeAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VisibilityAccountDefinitionsCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/Templates/VisibilityAccountDefinitionsManagementTemplate.html'
        };

        function VisibilityAccountDefinitionsCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var retailBEVisibilityEditorRuntime;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.visibilityAccountDefinitions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onAddVisibilityAccountDefinition = function () {
                    var onVisibilityAccountDefinitionAdded = function (addedVisibilityAccountDefinition) {
                        $scope.scopeModel.visibilityAccountDefinitions.push({ Entity: addedVisibilityAccountDefinition });
                    };

                    Retail_BE_VisibilityAccountDefinitionService.addVisibilityAccountDefinition(getExcludedAccountBEDefinitionIds(), onVisibilityAccountDefinitionAdded);
                };
                $scope.scopeModel.onDeleteVisibilityAccountDefinitions = function (visibilityAccountDefinition) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.visibilityAccountDefinitions, visibilityAccountDefinition.Entity.AccountBEDefinitionName, 'Entity.AccountBEDefinitionName');
                            $scope.scopeModel.visibilityAccountDefinitions.splice(index, 1);
                        }
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountDefinitions;
                    var accountDefinitionNamesById;

                    if (payload != undefined) {
                        retailBEVisibilityEditorRuntime = payload.vrModuleVisibilityEditorRuntime;

                        if (payload.vrModuleVisibility != undefined) {
                            accountDefinitions = payload.vrModuleVisibility.AccountDefinitions;
                        }
                        if (retailBEVisibilityEditorRuntime != undefined) {
                            accountDefinitionNamesById = retailBEVisibilityEditorRuntime.AccountBEDefinitionNamesById;
                        }
                    }

                    if (accountDefinitions != undefined) {
                        for (var index in accountDefinitions) {
                            if (index != "$type") {
                                var visibilityAccountDefinition = accountDefinitions[index];
                                extendVisibilityAccountDefinition(visibilityAccountDefinition);
                                $scope.scopeModel.visibilityAccountDefinitions.push({ Entity: visibilityAccountDefinition });
                            }
                        }
                    }

                    function extendVisibilityAccountDefinition(visibilityAccountDefinition) {
                        if (accountDefinitionNamesById == undefined || visibilityAccountDefinition.AccountBEDefinitionName != undefined)
                            return;

                        visibilityAccountDefinition.AccountBEDefinitionName = accountDefinitionNamesById[visibilityAccountDefinition.AccountBEDefinitionId];
                    }
                };

                api.getData = function () {

                    var visibilityAccountDefinitions;
                    if ($scope.scopeModel.visibilityAccountDefinitions.length > 0) {
                        visibilityAccountDefinitions = {};
                        for (var i = 0; i < $scope.scopeModel.visibilityAccountDefinitions.length; i++) {
                            var visibilityAccountDefinition = $scope.scopeModel.visibilityAccountDefinitions[i].Entity;
                            visibilityAccountDefinitions[visibilityAccountDefinition.AccountBEDefinitionId] = visibilityAccountDefinition;
                        }
                    }

                    return {
                        $type: "Retail.BusinessEntity.Business.VRRetailBEVisibility, Retail.BusinessEntity.Business",
                        AccountDefinitions: visibilityAccountDefinitions
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editVisibilityAccountDefinition
                }];
            }
            function editVisibilityAccountDefinition(visibilityAccountDefinition) {
                var onVisibilityAccountDefinitionUpdated = function (updatedVisibilityAccountDefinition) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.visibilityAccountDefinitions, visibilityAccountDefinition.Entity.AccountBEDefinitionName, 'Entity.AccountBEDefinitionName');
                    $scope.scopeModel.visibilityAccountDefinitions[index] = { Entity: updatedVisibilityAccountDefinition };
                };

                Retail_BE_VisibilityAccountDefinitionService.editVisibilityAccountDefinition(visibilityAccountDefinition.Entity, retailBEVisibilityEditorRuntime, getExcludedAccountBEDefinitionIds(), onVisibilityAccountDefinitionUpdated);
            }
            function getExcludedAccountBEDefinitionIds() {
                if ($scope.scopeModel.visibilityAccountDefinitions == undefined)
                    return;

                var accountBEDefinitionIds = [];
                for (var i = 0; i < $scope.scopeModel.visibilityAccountDefinitions.length; i++) {
                    var entity = $scope.scopeModel.visibilityAccountDefinitions[i].Entity;
                    if (entity != undefined) {
                        accountBEDefinitionIds.push(entity.AccountBEDefinitionId);
                    }
                }
                return accountBEDefinitionIds;
            }
        }
    }

    app.directive('retailBeVisibilityaccountdefinitionsManagement', VisibilityAccountDefinitionsManagementDirective);

})(app);