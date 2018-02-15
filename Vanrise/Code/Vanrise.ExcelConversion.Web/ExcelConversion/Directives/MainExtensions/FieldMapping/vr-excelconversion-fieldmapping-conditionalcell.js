(function (app) {

    'use strict';

    fieldmappingConditionalCellDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_ExcelConversion_ExcelAPIService'];

    function fieldmappingConditionalCellDirective(UtilsService, VRUIUtilsService, VR_ExcelConversion_ExcelAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                type: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var conditionalCell = new ConditionalCell($scope, ctrl, $attrs);
                conditionalCell.initializeController();
            },
            controllerAs: "conditionalCellMappingCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/ExcelConversion/Directives/MainExtensions/FieldMapping/Templates/ConditionalCellFieldMappingTemplate.html"
        };

        function ConditionalCell($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var context;
            var selectorAPI;
            var gridAPI;
            var gridReadyDeferred = UtilsService.createPromiseDeferred();
            var rowFieldSelectionDeferred;

            var cellFieldMappingAPI;
            var cellFieldMappingReadyDeferred;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.conditionalTypes = [{
                    value:0,
                    description:"Cell Value"
                }, {
                    value:1,
                    description:"Row Field"
                }];

                $scope.scopeModel.onCellFieldMappingReady = function (api) {
                    cellFieldMappingAPI = api;
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    var directivePayload = { context: getContext() };

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, cellFieldMappingAPI, directivePayload, setLoader, cellFieldMappingReadyDeferred);
                };
                $scope.scopeModel.datasource = [];
                $scope.scopeModel.datasourcemapping = [];
                $scope.scopeModel.removeField = function (dataItem) {
                    var index = $scope.scopeModel.datasourcemapping.indexOf(dataItem);
                    $scope.scopeModel.datasourcemapping.splice(index, 1);
                };

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    gridReadyDeferred.resolve();
                };

                $scope.scopeModel.addConditionalCell = function () {
                    var dataItem = {
                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                        loadPromiseDeferred: UtilsService.createPromiseDeferred()
                    };
                    addConditionalCellFieldMapping(dataItem);
                };
                $scope.scopeModel.onSelectCellChanged = function () {
                        var cellField = cellFieldMappingAPI != undefined ? cellFieldMappingAPI.getData() : undefined;
                        if (cellField != undefined)
                            getDefaultConditions(cellField);
                        else
                            $scope.scopeModel.datasourcemapping.length = 0;
                };
                $scope.scopeModel.onRowFieldSelectionChanged = function () {
                    if (rowFieldSelectionDeferred != undefined)
                        rowFieldSelectionDeferred.resolve();
                    else {
                        if ($scope.scopeModel.selectedvalues != undefined) {
                            var cellFieldMapping = context.getFilterCellFieldMapping($scope.scopeModel.selectedvalues.FieldName);
                            if (cellFieldMapping != undefined) {
                                getDefaultConditions(cellFieldMapping);
                            }
                        }
                        else
                            $scope.scopeModel.datasourcemapping.length = 0;
                    }
                };
                defineAPI();

            }
            function addConditionalCellFieldMapping(dataItem) {
                var payload = {
                    context: getContext()
                };
                dataItem.normalColNum = $scope.scopeModel.normalColNum;
                dataItem.onFieldMappingReady = function (api) {
                    dataItem.fieldMappingAPI = api;
                    dataItem.readyPromiseDeferred.resolve();
                };
                dataItem.readyPromiseDeferred.promise
              .then(function () {

                  VRUIUtilsService.callDirectiveLoad(dataItem.fieldMappingAPI, payload, dataItem.loadPromiseDeferred);
              });
                $scope.scopeModel.datasourcemapping.push(dataItem);
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var parts;
                    var filterItems;

                    if (payload != undefined) {
                        context = payload.context;
                        $scope.scopeModel.datasource = context.getFilterFieldsMappings();
                        if(payload.fieldMapping != undefined)
                        {
                            if(payload.fieldMapping.RowFieldName != undefined)
                            {
                                $scope.scopeModel.selectedConditionalType = $scope.scopeModel.conditionalTypes[1];
                                $scope.scopeModel.selectedvalues = UtilsService.getItemByVal($scope.scopeModel.datasource, payload.fieldMapping.RowFieldName, 'FieldName');
                            }
                        }
                    }

                    if ($scope.scopeModel.selectedConditionalType == undefined)
                    {
                        $scope.scopeModel.selectedConditionalType = $scope.scopeModel.conditionalTypes[0];
                        cellFieldMappingReadyDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadCellFieldMappingSelector(payload));
                    }

                    if (payload != undefined && payload.fieldMapping && payload.fieldMapping.Choices != undefined)
                    {
                        var loadGridFieldMappingDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadGridFieldMappingDeferred.promise);

                        if (payload.fieldMapping.RowFieldName != undefined) {
                            rowFieldSelectionDeferred = UtilsService.createPromiseDeferred();
                            rowFieldSelectionDeferred.promise.then(function () {
                                loadGridFieldMapping(payload).then(function () {
                                    loadGridFieldMappingDeferred.resolve();
                                    rowFieldSelectionDeferred = undefined;
                                });
                            });
                        } else {
                                loadGridFieldMapping(payload).then(function () {
                                    loadGridFieldMappingDeferred.resolve();
                                });
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            };

            function loadCellFieldMappingSelector(payload)
            {
                var cellFieldMappingLoadDeferred = UtilsService.createPromiseDeferred();
                cellFieldMappingReadyDeferred.promise.then(function () {
                    cellFieldMappingReadyDeferred = undefined;
                    var directivePayload = { context: getContext() };
                    if (payload != undefined && payload.fieldMapping != undefined) {
                        directivePayload.fieldMapping = payload.fieldMapping.CellFieldMapping;
                    }
                    VRUIUtilsService.callDirectiveLoad(cellFieldMappingAPI, directivePayload, cellFieldMappingLoadDeferred);
                });
                return cellFieldMappingLoadDeferred.promise;
            }
            function loadGridFieldMapping(payload) {
                var promises = [];

                for (var i = 0; i < payload.fieldMapping.Choices.length; i++) {

                    var dataItem = payload.fieldMapping.Choices[i];
                    extendDataItem(dataItem);
                    promises.push(dataItem.selectiveLoadDeferred.promise);
                    $scope.scopeModel.datasourcemapping.push(dataItem);
                }

                return UtilsService.waitMultiplePromises(promises);
            }

            function extendDataItem(dataItem) {
                dataItem.selectiveLoadDeferred = UtilsService.createPromiseDeferred();

                dataItem.onFieldMappingReady = function (api) {
                    dataItem.fieldMappingAPI = api;
                    var selectorPayload = {
                        fieldMapping: dataItem.FieldMappingChoice,
                        context: getContext()
                    };
                    VRUIUtilsService.callDirectiveLoad(api, selectorPayload, dataItem.selectiveLoadDeferred);
                };
            }

            function getData() {
                var data;
                if ($scope.scopeModel.datasourcemapping.length > 0) {
                    var choices = [];
                    for (var i = 0; i < $scope.scopeModel.datasourcemapping.length; i++) {
                        var fieldsMapping = $scope.scopeModel.datasourcemapping[i];
                        choices.push({
                            RowFieldValue: fieldsMapping.RowFieldValue,
                            FieldMappingChoice: fieldsMapping.fieldMappingAPI != undefined ? fieldsMapping.fieldMappingAPI.getData() : undefined
                        });
                    }
                    data = {
                        $type: "Vanrise.ExcelConversion.MainExtensions.ConditionalCellFieldMapping, Vanrise.ExcelConversion.MainExtensions",
                        RowFieldName: ($scope.scopeModel.selectedvalues != undefined && $scope.scopeModel.selectedConditionalType.value == 1) ? $scope.scopeModel.selectedvalues.FieldName : undefined,
                        CellFieldMapping:cellFieldMappingAPI != undefined?cellFieldMappingAPI.getData():undefined,
                        Choices: choices
                    };
                }
                return data;
            }

            function getContext() {
                var currentContext;
                if (context != undefined) {
                    currentContext = UtilsService.cloneObject(context);
                }
                if (currentContext == undefined)
                    currentContext = {};

                return currentContext;
            }

            function getDefaultConditions(cellField)
            {
                $scope.scopeModel.datasourcemapping.length = 0;
                if(context != undefined)
                {
                    if(context.getFileId != undefined)
                    {
                        var fileId = context.getFileId();
                        if (fileId != undefined) {
                            var firstRowIndex = context.getFirstRowIndex();
                            var lastRowIndex = context.getLastRowIndex();

                            if (cellField != undefined && firstRowIndex.row == cellField.RowIndex)
                            {
                                cellField.FieldName = "Condition";
                                var input = {
                                    FileId: fileId,
                                    ConversionSettings: {
                                        ListMappings: [{
                                            ListName: "ConditionList",
                                            SheetIndex: firstRowIndex.sheet,
                                            FirstRowIndex: firstRowIndex != undefined ? firstRowIndex.row : undefined,
                                            LastRowIndex: lastRowIndex != undefined ? lastRowIndex.row : undefined,
                                            FieldMappings: [cellField]
                                        }]
                                    }
                                };
                                return VR_ExcelConversion_ExcelAPIService.ReadConditionsFromFile(input).then(function (response) {
                                    if (response != undefined) {
                                        for (var i = 0; i < response.length; i++) {
                                            var item = response[i];
                                            var dataItem = {
                                                readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                                loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                                RowFieldValue: item
                                            };
                                            addConditionalCellFieldMapping(dataItem);
                                        }
                                    }
                                });
                            }

                        }
                    }
                }
            }
        }

    }

    app.directive('vrExcelconversionFieldmappingConditionalcell', fieldmappingConditionalCellDirective);

})(app);