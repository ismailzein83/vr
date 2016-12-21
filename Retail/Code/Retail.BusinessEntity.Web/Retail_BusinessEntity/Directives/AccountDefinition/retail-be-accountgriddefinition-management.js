(function (app) {

    'use strict';

    AccountGridDefinitionManagementDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_AccountDefinitionService'];

    function AccountGridDefinitionManagementDirective(UtilsService, VRNotificationService, Retail_BE_AccountDefinitionService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/Templates/AccountGridDefinitionManagementTemplate.html'
        };

        function AccountGridDefinitionManagementCtor($scope, ctrl) {
            this.initializeController = initializeController;

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
                        $scope.scopeModel.columnDefinitions.push({ Entity: addedColumnDefinition });
                    };

                    Retail_BE_AccountDefinitionService.addGridColumnDefinition(onColumnDefinitionAdded);
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

                    var accountGridDefinition;

                    if (payload != undefined) {
                        accountGridDefinition = payload.accountGridDefinition;
                    }

                    //Loading ColumnDefinitions Grid
                    if (accountGridDefinition != undefined && accountGridDefinition.ColumnDefinitions != undefined) {
                        for (var index in accountGridDefinition.ColumnDefinitions) {
                            if (index != "$type") {
                                var columnDefinition = accountGridDefinition.ColumnDefinitions[index];
                                $scope.scopeModel.columnDefinitions.push({ Entity: columnDefinition });
                            }
                        }
                    }
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
                    $scope.scopeModel.columnDefinitions[index] = { Entity: updatedColumnDefinition };
                };

                Retail_BE_AccountDefinitionService.editGridColumnDefinition(columnDefinition.Entity, onColumnDefinitionUpdated);
            }
        }
    }

    app.directive('retailBeAccountgriddefinitionManagement', AccountGridDefinitionManagementDirective);

})(app);