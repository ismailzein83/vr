'use strict';
app.directive('vrGenericdataGenericbusinessentityDefinitionSection', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_ExtensibleBEItemService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_ExtensibleBEItemService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new SectionCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/Editor/Templates/DefinitionSectionTemplate.html';
            }

        };

        function SectionCtor(ctrl, $scope) {
            var selectedValues;
            var gridAPI;
            var context;
            var fieldSelectedText = " Fields.";
            function initializeController() {
                ctrl.rows = [];
                ctrl.datasource = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                };


                defineGridRowsMenuAction();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.rows != undefined) {
                            for (var i = 0; i < payload.rows.length; i++) {
                                var row = payload.rows[i];
                                prepareRowObject(row);
                            }

                        }
                    }
                };

                api.onAddRow = function (row) {
                    var rowObj = {
                        numberOfFields: row.length + fieldSelectedText,
                        row: row,
                        onRowDirectiveReady: function (api) {
                            rowObj.rowAPI = api;
                            var payload = { fields: rowObj.row };
                            var setLoader = function (value) { $scope.isLoading = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, rowObj.rowAPI, payload, setLoader);
                        }
                    };
                    ctrl.rows.push(rowObj);
                    gridAPI.expandRow(rowObj);
                };

                api.getData = function () {
                    var rows = [];
                    for (var i = 0; i < ctrl.rows.length; i++) {
                        var row = ctrl.rows[i];
                        if (row.rowAPI != undefined)
                            rows.push(row.rowAPI.getData());
                        else
                            rows.push({ Fields: row.row });
                    }
                    return { Rows: rows };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function prepareRowObject(row) {
                var rowObj = {
                    numberOfFields: row.Fields.length + fieldSelectedText,
                    row:  row.Fields,
                    onRowDirectiveReady: function (api) {
                        rowObj.rowAPI = api;
                        var payload = { fields: rowObj.row };
                        var setLoader = function (value) { $scope.isLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, rowObj.rowAPI, payload, setLoader);
                    }
                };
                ctrl.rows.push(rowObj);
            }

            function defineGridRowsMenuAction() {
                $scope.gridRowsMenuActions = [{
                    name: "Edit Row",
                    clicked: editRow,
                },
                 {
                     name: "Delete",
                     clicked: deleteRow,
                 }];
            }

            function editRow(dataItem) {
                gridAPI.expandRow(dataItem);
                var onRowUpdated = function (row) {
                    var rowObj = UtilsService.getItemByVal(ctrl.rows, dataItem.row, 'row');

                    if (rowObj != undefined)
                    {
                        rowObj.numberOfFields = row.length + fieldSelectedText;
                        rowObj.row = row;
                        rowObj.rowAPI.applyChanges(row);
                    }
                };
                VR_GenericData_ExtensibleBEItemService.editRow(onRowUpdated, context.getFilteredFields(dataItem.row), dataItem.row);
            }

            function deleteRow(dataItem) {
                var onRowDeleted = function (rowObj) {
                    var index = ctrl.rows.indexOf(rowObj);
                    if (index != -1);
                    ctrl.rows.splice(index, 1);
                };

                VR_GenericData_ExtensibleBEItemService.deleteRow($scope, dataItem, onRowDeleted);
            }
            

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);