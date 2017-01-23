'use strict';
app.directive('vrInvoicetypeInvoicesettingDefinitionSection', ['UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceTypeService',
    function (UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeService) {

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
                return '/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSettingDefinition/Templates/DefinitionSectionTemplate.html';
            }

        };

        function SectionCtor(ctrl, $scope) {
            var selectedValues;
            var gridAPI;
            var context;
            var fieldSelectedText = " Parts.";
            function initializeController() {
                ctrl.rows = [];
                ctrl.datasource = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                };
                ctrl.removeRow = function (dataItem)
                {
                    var index = ctrl.rows.indexOf(dataItem);
                    ctrl.rows.splice(index, 1);
                }
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

                api.onAddRow = function (part) {
                    var rowObj = {
                        parts: [part],
                        numberOfParts: 1 + fieldSelectedText,
                        onRowDirectiveReady: function (api) {
                            rowObj.rowAPI = api;
                            var payload = { context :rowObj.context, parts: rowObj.parts };
                            var setLoader = function (value) { $scope.isLoading = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, rowObj.rowAPI, payload, setLoader);
                        },
                        context: {
                            removePart:function(part)
                            {
                                rowObj.parts = rowObj.rowAPI.getData().Parts;
                                rowObj.numberOfParts = rowObj.parts.length + fieldSelectedText;
                            }
                        }
                    };
                    var dataItem = { Entity: rowObj };
                    ctrl.rows.push(dataItem);
                    gridAPI.expandRow(dataItem);
                };

                api.getData = function () {
                    var rows = [];
                    for (var i = 0; i < ctrl.rows.length; i++) {
                        var row = ctrl.rows[i];
                        if (row.Entity.rowAPI != undefined)
                            rows.push(row.Entity.rowAPI.getData());
                        else
                            rows.push({ Parts: row.Entity.parts });
                    }
                    return { Rows: rows };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function prepareRowObject(row) {
                var rowObj = {
                    parts: row.Parts,
                    numberOfParts: row.Parts.length + fieldSelectedText,
                    onRowDirectiveReady: function (api) {
                        rowObj.rowAPI = api;
                        var payload = { context: rowObj.context, parts: row.Parts };
                        var setLoader = function (value) { $scope.isLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, rowObj.rowAPI, payload, setLoader);
                    },
                    context: {
                        removePart: function (part) {
                            rowObj.parts = rowObj.rowAPI.getData().Parts;
                            rowObj.numberOfParts = rowObj.parts.length + fieldSelectedText;
                        }
                    }
                };
                ctrl.rows.push({ Entity: rowObj });
            }

            function defineGridRowsMenuAction() {
                $scope.gridRowsMenuActions = [{
                    name: "Add Part",
                    clicked: addPart,
                }];
            }
            function addPart(dataItem){
                var onRowAdded = function (part) {
                    dataItem.Entity.rowAPI.applyChanges(part);
                    dataItem.Entity.numberOfParts = dataItem.Entity.parts.length + 1 + fieldSelectedText;
                };
                VR_Invoice_InvoiceTypeService.addInvoiceSettingPart(onRowAdded);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);