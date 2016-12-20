(function (app) {

    'use strict';

    AccountGridDefinitionManagementDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_AccountGridDefinitionService'];

    function AccountGridDefinitionManagementDirective(UtilsService, VRNotificationService, Retail_BE_AccountGridDefinitionService) {
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Settings/Templates/AccountGridDefinitionTemplate.html'
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
                        $scope.scopeModel.columnDefinitions.push(addedColumnDefinition);
                    };

                    Retail_BE_AccountGridDefinitionService.addColumnDefinition(onColumnDefinitionAdded);
                };
                $scope.scopeModel.onDeleteColumnDefinition = function (columnDefinition) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.columnDefinitions, columnDefinition.FieldName, 'FieldName');
                            $scope.scopeModel.columnDefinitions.splice(index, 1);
                        }
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var columnDefinitions;

                    if (payload != undefined) {
                        var accountGridDefinition = payload.accountGridDefinition;

                        if (accountGridDefinition != undefined) {
                            columnDefinitions = accountGridDefinition.ColumnDefinitions
                        }
                    }

                    //Loading ColumnDefinitions Grid
                    if (columnDefinitions != undefined)
                        $scope.scopeModel.columnDefinitions = columnDefinitions;
                };

                api.getData = function () {

                    var obj = {
                        ColumnDefinitions: $scope.scopeModel.columnDefinitions
                    };

                    return obj;
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
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.columnDefinitions, columnDefinition.FieldName, 'FieldName');
                    $scope.scopeModel.columnDefinitions[index] = updatedColumnDefinition;
                };

                Retail_BE_AccountGridDefinitionService.editColumnDefinition(columnDefinition, onColumnDefinitionUpdated);
            }
        }
    }

    app.directive('retailBeAccountgriddefinition', AccountGridDefinitionManagementDirective);

})(app);