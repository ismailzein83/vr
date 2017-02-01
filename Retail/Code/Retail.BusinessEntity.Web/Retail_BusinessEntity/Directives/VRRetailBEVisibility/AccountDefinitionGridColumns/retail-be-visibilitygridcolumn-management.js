'use strict';

app.directive('retailBeVisibilitygridcolumnManagement', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VisibilityGridColumn(ctrl, $scope);
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
                return '/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/AccountDefinitionGridColumns/Templates/VisibilityGridColumnManagementTemplate.html';
            }
        };

        function VisibilityGridColumn(ctrl, $scope) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.gridColumnDefinitions = [];
                $scope.scopeModel.selectedGridColumnDefinitions = [];
                $scope.scopeModel.gridColumns = [];

                $scope.scopeModel.onSelectGridColumnDefinition = function (selectedItem) {

                    $scope.scopeModel.gridColumns.push({
                        FieldName: selectedItem.FieldName,
                        Header: selectedItem.Header
                    });
                };
                $scope.scopeModel.onDeselectGridColumnDefinition = function (deselectedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.gridColumns, deselectedItem.FieldName, 'FieldName');
                    $scope.scopeModel.gridColumns.splice(index, 1);
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedGridColumnDefinitions, deletedItem.FieldName, 'FieldName');
                    $scope.scopeModel.selectedGridColumnDefinitions.splice(index, 1);
                    $scope.scopeModel.onDeselectGridColumnDefinition(deletedItem);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var gridColumns;
                    var gridColumnDefinitions;

                    if (payload != undefined) {
                        gridColumns = payload.gridColumns;
                        gridColumnDefinitions = payload.gridColumnDefinitions;
                    }

                    //Loading Selector
                    if (gridColumnDefinitions != undefined) {
                        for (var i = 0; i < gridColumnDefinitions.length; i++) {
                            $scope.scopeModel.gridColumnDefinitions.push(gridColumnDefinitions[i]);
                        }
                        if (gridColumns != undefined) {
                            for (var i = 0; i < gridColumns.length; i++) {
                                var currentGridColumn = gridColumns[i];
                                for (var j = 0; j < gridColumnDefinitions.length; j++) {
                                    var currentGridColumnDefinition = gridColumnDefinitions[j];
                                    if (currentGridColumnDefinition.FieldName == currentGridColumn.FieldName)
                                        $scope.scopeModel.selectedGridColumnDefinitions.push(currentGridColumnDefinition);
                                }
                            }
                        }
                    }

                    //Loading Grid
                    if ($scope.scopeModel.selectedGridColumnDefinitions != undefined) {
                        for (var i = 0; i < $scope.scopeModel.selectedGridColumnDefinitions.length; i++) {
                            var gridColumnDefinition = $scope.scopeModel.selectedGridColumnDefinitions[i];

                            $scope.scopeModel.gridColumns.push({
                                FieldName: gridColumnDefinition.FieldName,
                                Header: gridColumnDefinition.Header
                            });
                        }
                    }
                };

                api.getData = function () {

                    var _gridColumns;
                    if ($scope.scopeModel.gridColumns.length > 0) {
                        _gridColumns = [];
                        for (var i = 0; i < $scope.scopeModel.gridColumns.length; i++) {
                            var currentGridColumn = $scope.scopeModel.gridColumns[i];
                            _gridColumns.push({
                                FieldName: currentGridColumn.FieldName
                            });
                        }
                    }
                    return _gridColumns
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);

