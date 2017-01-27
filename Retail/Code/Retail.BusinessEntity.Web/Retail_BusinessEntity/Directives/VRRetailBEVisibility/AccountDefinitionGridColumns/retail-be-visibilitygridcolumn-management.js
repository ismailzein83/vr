(function (app) {

    'use strict';

    VisibilityGridColumnManagementDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_VisibilityAccountDefinitionService'];

    function VisibilityGridColumnManagementDirective(UtilsService, VRNotificationService, Retail_BE_VisibilityAccountDefinitionService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VisibilityGridColumnsCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/AccountDefinitionGridColumns/Templates/VisibilityGridColumnManagementTemplate.html'
        };

        function VisibilityGridColumnsCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var columnDefinitions;

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
                        $scope.scopeModel.gridColumns.push({ Entity: addedGridColumn });
                    };

                    Retail_BE_VisibilityAccountDefinitionService.addVisibilityGridColumn(columnDefinitions, onGridColumnAdded);
                };
                $scope.scopeModel.onDeleteGridColumn = function (gridColumn) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.gridColumns, gridColumn.Entity.GridColumnTitle, 'Entity.GridColumnTitle');
                            $scope.scopeModel.gridColumns.splice(index, 1);
                        }
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var gridColumns;

                    console.log(payload);

                    if (payload != undefined) {
                        gridColumns = payload.gridColumns;
                        columnDefinitions = payload.columnDefinitions;
                    }

                    //Loading GridColumns Grid
                    if (gridColumns != undefined) {
                        for (var index = 0 ; index < gridColumns.length; index++) {
                            if (index != "$type") {
                                var gridColumn = gridColumns[index];
                                extendGridColumnObj(gridColumn);
                                $scope.scopeModel.gridColumns.push({ Entity: gridColumn });
                            }
                        }
                    }

                    function extendGridColumnObj(gridColumn) {
                        if (columnDefinitions == undefined || gridColumn.Header != undefined)
                            return;

                        for (var index = 0; index < columnDefinitions.length; index++)
                        {
                            var currentColumnDefinition = columnDefinitions[index];
                            if(currentColumnDefinition.FieldName == gridColumn.FieldName)
                                gridColumn.Header = currentColumnDefinition.Header;
                        }
                    }
                };

                api.getData = function () {

                    var gridColumns;
                    if ($scope.scopeModel.gridColumns.length > 0) {
                        gridColumns = [];
                        for (var i = 0; i < $scope.scopeModel.gridColumns.length; i++) {
                            var gridColumn = $scope.scopeModel.gridColumns[i].Entity;
                            gridColumns.push(gridColumn);
                        }
                    }

                    return gridColumns;
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
            function editGridColumn(gridColumn) {
                var onGridColumnUpdated = function (updatedGridColumn) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.gridColumns, gridColumn.Entity.GridColumnTitle, 'Entity.GridColumnTitle');
                    $scope.scopeModel.gridColumns[index] = { Entity: updatedGridColumn };
                };

                Retail_BE_VisibilityAccountDefinitionService.editVisibilityGridColumn(gridColumn.Entity, columnDefinitions, onGridColumnUpdated);
            }


        }
    }

    app.directive('retailBeVisibilitygridcolumnManagement', VisibilityGridColumnManagementDirective);

})(app);