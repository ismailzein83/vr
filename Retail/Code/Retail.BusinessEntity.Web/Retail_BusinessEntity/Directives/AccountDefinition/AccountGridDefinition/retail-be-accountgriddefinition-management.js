(function (app) {

    'use strict';

    AccountGridDefinitionManagementDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_AccountBEDefinitionService', 'Retail_BE_AccountTypeAPIService'];

    function AccountGridDefinitionManagementDirective(UtilsService, VRNotificationService, Retail_BE_AccountBEDefinitionService, Retail_BE_AccountTypeAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountGridDefinitionManagementCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountGridDefinition/Templates/AccountGridDefinitionManagementTemplate.html'
        };

        function AccountGridDefinitionManagementCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var accountFields;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.columnDefinitions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onAddColumnDefinition = function () {
                    var onColumnDefinitionAdded = function (addedColumnDefinition) {
                        extendColumnDefinitionObj(addedColumnDefinition);
                        $scope.scopeModel.columnDefinitions.push({ Entity: addedColumnDefinition });
                    };

                    Retail_BE_AccountBEDefinitionService.addGridColumnDefinition(accountBEDefinitionId, onColumnDefinitionAdded);
                };
                $scope.scopeModel.onDeleteColumnDefinition = function (columnDefinition) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.columnDefinitions, columnDefinition.Entity.FieldName, 'Entity.FieldName');
                            $scope.scopeModel.columnDefinitions.splice(index, 1);
                        }
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountGridDefinition;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        accountGridDefinition = payload.accountGridDefinition;
                    }

                    var loadAccountFieldsPromise = loadAccountFields();
                    promises.push(loadAccountFieldsPromise);

                    loadAccountFieldsPromise.then(function () {

                        //Loading ColumnDefinitions Grid
                        if (accountGridDefinition != undefined && accountGridDefinition.ColumnDefinitions != undefined) {
                            for (var index in accountGridDefinition.ColumnDefinitions) {
                                if (index != "$type") {
                                    var columnDefinition = accountGridDefinition.ColumnDefinitions[index];
                                    extendColumnDefinitionObj(columnDefinition);
                                    $scope.scopeModel.columnDefinitions.push({ Entity: columnDefinition });
                                }
                            }
                        }
                    });

                    function loadAccountFields() {
                        return Retail_BE_AccountTypeAPIService.GetGenericFieldDefinitionsInfo(accountBEDefinitionId).then(function (response) {
                            accountFields = response;
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var columnDefinitions;
                    if ($scope.scopeModel.columnDefinitions.length > 0) {
                        columnDefinitions = [];
                        for (var i = 0; i < $scope.scopeModel.columnDefinitions.length; i++) {
                            var columnDefinition = $scope.scopeModel.columnDefinitions[i].Entity;
                            columnDefinitions.push(columnDefinition);
                        }
                    }

                    return {
                        ColumnDefinitions: columnDefinitions
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editColumnDefinitionDefinition
                }];
            }
            function editColumnDefinitionDefinition(columnDefinition) {
                var onColumnDefinitionUpdated = function (updatedColumnDefinition) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.columnDefinitions, columnDefinition.Entity.FieldName, 'Entity.FieldName');
                    extendColumnDefinitionObj(updatedColumnDefinition);
                    $scope.scopeModel.columnDefinitions[index] = { Entity: updatedColumnDefinition };
                };

                Retail_BE_AccountBEDefinitionService.editGridColumnDefinition(columnDefinition.Entity, accountBEDefinitionId, onColumnDefinitionUpdated);
            }

            function extendColumnDefinitionObj(columnDefinition) {
                if (accountFields == undefined)
                    return;

                for (var index = 0; index < accountFields.length; index++) {
                    var currentAccountField = accountFields[index];
                    if (columnDefinition.FieldName == currentAccountField.Name) {
                        columnDefinition.FieldTitle = currentAccountField.Title;
                        return;
                    }
                }
            }
        }
    }

    app.directive('retailBeAccountgriddefinitionManagement', AccountGridDefinitionManagementDirective);

})(app);