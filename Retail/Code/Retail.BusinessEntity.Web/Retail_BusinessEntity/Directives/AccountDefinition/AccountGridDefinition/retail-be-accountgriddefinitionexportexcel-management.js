(function (app) {

    'use strict';

    AccountGridDefinitionExportExcelManagementDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_AccountBEDefinitionService', 'Retail_BE_AccountTypeAPIService'];

    function AccountGridDefinitionExportExcelManagementDirective(UtilsService, VRNotificationService, Retail_BE_AccountBEDefinitionService, Retail_BE_AccountTypeAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountGridDefinitionManagementExportExcelCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountGridDefinition/Templates/AccountGridDefinitionExportExcelManagementTemplate.html'
        };

        function AccountGridDefinitionManagementExportExcelCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var accountFields;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.exportColumnDefinitions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onAddColumnDefinition = function () {
                    var onColumnDefinitionAdded = function (addedColumnDefinition) {
                        extendColumnDefinitionObj(addedColumnDefinition);
                        $scope.scopeModel.exportColumnDefinitions.push({ Entity: addedColumnDefinition });
                    };

                    Retail_BE_AccountBEDefinitionService.addGridColumnExportExcelDefinition(accountBEDefinitionId, onColumnDefinitionAdded);
                };
                $scope.scopeModel.onDeleteColumnDefinition = function (columnDefinition) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.exportColumnDefinitions, columnDefinition.Entity.FieldName, 'Entity.FieldName');
                            $scope.scopeModel.exportColumnDefinitions.splice(index, 1);
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
                        accountGridDefinition = payload.accountGridDefinitionExportExcel;
                    }

                    var loadAccountFieldsPromise = loadAccountFields();
                    promises.push(loadAccountFieldsPromise);

                    loadAccountFieldsPromise.then(function () {

                        //Loading ColumnDefinitions Grid
                        if (accountGridDefinition != undefined) {
                            for (var index in accountGridDefinition) {
                                if (index != "$type") {
                                    var columnDefinition = accountGridDefinition[index];
                                    extendColumnDefinitionObj(columnDefinition);
                                    $scope.scopeModel.exportColumnDefinitions.push({ Entity: columnDefinition });
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

                    var exportColumnDefinitions;
                    if ($scope.scopeModel.exportColumnDefinitions.length > 0) {
                        exportColumnDefinitions = [];
                        for (var i = 0; i < $scope.scopeModel.exportColumnDefinitions.length; i++) {
                            var columnDefinition = $scope.scopeModel.exportColumnDefinitions[i].Entity;
                            exportColumnDefinitions.push(columnDefinition);
                        }
                    }


                    return exportColumnDefinitions;
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
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.exportColumnDefinitions, columnDefinition.Entity.FieldName, 'Entity.FieldName');
                    extendColumnDefinitionObj(updatedColumnDefinition);
                    $scope.scopeModel.exportColumnDefinitions[index] = { Entity: updatedColumnDefinition };
                };

                Retail_BE_AccountBEDefinitionService.editGridColumnExportExcelDefinition(columnDefinition.Entity, accountBEDefinitionId, onColumnDefinitionUpdated);
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

    app.directive('retailBeAccountgriddefinitionexportexcelManagement', AccountGridDefinitionExportExcelManagementDirective);

})(app);