(function (app) {

    'use strict';

    VisibilityAccountDefinitionGridColumnsDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_AccountBEDefinitionService', 'Retail_BE_AccountTypeAPIService'];

    function VisibilityAccountDefinitionGridColumnsDirective(UtilsService, VRNotificationService, Retail_BE_AccountBEDefinitionService, Retail_BE_AccountTypeAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VisibilityAccountDefinitionGridColumnsCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/AccountDefinitionGridColumns/Templates/VisibilityAccountDefinitionGridColumnsTemplate.html'
        };

        function VisibilityAccountDefinitionGridColumnsCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.gridColumns = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onAddGridColumn = function () {
                    var onGridColumnAdded = function (addedGridColumn) {
                        extendGridColumnObj(addedGridColumn);
                        $scope.scopeModel.gridColumns.push({ Entity: addedGridColumn });
                    };

                    Retail_BE_AccountBEDefinitionService.addGridGridColumn(accountBEDefinitionId, onGridColumnAdded);
                };
                $scope.scopeModel.onDeleteGridColumn = function (columnDefinition) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.gridColumns, columnDefinition.Entity.FieldName, 'Entity.FieldName');
                            $scope.scopeModel.gridColumns.splice(index, 1);
                        }
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var gridColumns;

                    if (payload != undefined) {
                        gridColumns = payload.GridColumns;
                    }

                    //var loadAccountFieldsPromise = loadAccountFields();
                    //promises.push(loadAccountFieldsPromise);

                    //loadAccountFieldsPromise.then(function () {

                    //    //Loading GridColumns Grid
                    //    if (accountGridDefinition != undefined && accountGridDefinition.GridColumns != undefined) {
                    //        for (var index in accountGridDefinition.GridColumns) {
                    //            if (index != "$type") {
                    //                var columnDefinition = accountGridDefinition.GridColumns[index];
                    //                extendGridColumnObj(columnDefinition);
                    //                $scope.scopeModel.gridColumns.push({ Entity: columnDefinition });
                    //            }
                    //        }
                    //    }
                    //});

                    //function loadAccountFields() {
                    //    return Retail_BE_AccountTypeAPIService.GetGenericFieldDefinitionsInfo(accountBEDefinitionId).then(function (response) {
                    //        accountFields = response;
                    //    });
                    //}

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var gridColumns;
                    if ($scope.scopeModel.gridColumns.length > 0) {
                        gridColumns = [];
                        for (var i = 0; i < $scope.scopeModel.gridColumns.length; i++) {
                            var columnDefinition = $scope.scopeModel.gridColumns[i].Entity;
                            gridColumns.push(columnDefinition);
                        }
                    }

                    return {
                        GridColumns: gridColumns
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editGridColumn
                }];
            }
            function editGridColumn(columnDefinition) {
                var onGridColumnUpdated = function (updatedGridColumn) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.gridColumns, columnDefinition.Entity.FieldName, 'Entity.FieldName');
                    //extendGridColumnObj(updatedGridColumn);
                    $scope.scopeModel.gridColumns[index] = { Entity: updatedGridColumn };
                };

                Retail_BE_AccountBEDefinitionService.editGridGridColumn(columnDefinition.Entity, accountBEDefinitionId, onGridColumnUpdated);
            }

            //function extendGridColumnObj(columnDefinition) {
            //    if (accountFields == undefined)
            //        return;

            //    for (var index = 0; index < accountFields.length; index++) {
            //        var currentAccountField = accountFields[index];
            //        if (columnDefinition.FieldName == currentAccountField.Name) {
            //            columnDefinition.FieldTitle = currentAccountField.Title;
            //            return;
            //        }
            //    }
            //}
        }
    }

    app.directive('retailBeVisibilityGridcolumns', VisibilityAccountDefinitionGridColumnsDirective);

})(app);