(function (app) {

    'use strict';

    OutputpricelistconfigurationBasic.$inject = ["UtilsService", "VRUIUtilsService"];

    function OutputpricelistconfigurationBasic(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                type: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var outputpricelistconfiguration = new Outputpricelistconfiguration($scope, ctrl, $attrs);
                outputpricelistconfiguration.initializeController();
            },
            controllerAs: "outputbasicCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/XBooster_PriceListConversion/Directives/MainExtensions/PriceListTemplate/OutputPriceListConfiguration/Templates/BasicOutputPriceListConfiguration.html"
        };

        function Outputpricelistconfiguration($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var outPutWorkBookAPI;

            var firstRowAPI;
            var firstRowReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            $scope.outPutFieldMappings;
            function initializeController() {
                $scope.outPutFieldMappings = [{ fieldTitle: "Code", isRequired: true, type: "cell", fieldName: "Code", configId: 0, editor: "xbooster-pricelistconversion-outputfieldvalue-pricelistfield" }, { fieldTitle: "Zone", isRequired: true, type: "cell", fieldName: "Zone", configId: 0, editor: "xbooster-pricelistconversion-outputfieldvalue-pricelistfield" }, { fieldTitle: "Rate", isRequired: true, type: "cell", fieldName: "Rate", configId: 0, editor: "xbooster-pricelistconversion-outputfieldvalue-pricelistfield" }, { fieldTitle: "Effective Date", isRequired: true, type: "cell", fieldName: "EffectiveDate", configId: 0, editor: "xbooster-pricelistconversion-outputfieldvalue-pricelistfield" }, { fieldTitle: "Custom", isRequired: true, fieldName: "Custom", configId: 1, editor: "xbooster-pricelistconversion-outputfieldvalue-constant" }]

                ctrl.datasource = [];
                ctrl.isValid = function () {

                    if (ctrl.datasource.length > 0)
                        return null;
                    return "At least one field type should be selected.";
                }
                ctrl.disableAddButton = true;
                ctrl.addFilter = function () {
                    var dataItem = {
                        id: ctrl.datasource.length + 1,
                        editor: $scope.selectedoutputFieldMapping.editor,
                        name: $scope.selectedoutputFieldMapping.fieldName,
                        configId: $scope.selectedoutputFieldMapping.configId,
                    };
                    dataItem.onFieldReady = function (api) {
                        dataItem.fieldAPI = api;
                        var payload = {
                            context: buildOutputContext(),
                        };
                        var setLoader = function (value) { ctrl.isLoadingDirective = value };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.fieldAPI, payload, setLoader);
                    };
                    var directivePayload = {
                        FieldName: $scope.selectedoutputFieldMapping.fieldName,
                        FieldTitle: $scope.selectedoutputFieldMapping.fieldTitle
                    }
                    dataItem.onDirectiveReady = function (api) {
                        dataItem.directiveAPI = api;
                        
                        var setLoader = function (value) { ctrl.isLoadingDirective = value };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, directivePayload, setLoader);
                    };
                    ctrl.datasource.push(dataItem);


                    $scope.selectedoutputFieldMapping = undefined;
                };
                ctrl.onActionTemplateChanged = function () {
                    ctrl.disableAddButton = ($scope.selectedoutputFieldMapping == undefined);
                };
                ctrl.removeFilter = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.id, 'id');
                    ctrl.datasource.splice(index, 1);
                };

                $scope.onFirstRowReady = function(api)
                {
                    firstRowAPI = api;
                    firstRowReadyPromiseDeferred.resolve();
                }
                $scope.onOutPutReadyWoorkBook = function (api) {
                    outPutWorkBookAPI = api;
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var table;
                    if (payload != undefined) {
                        if (payload.configDetails !=undefined)
                        {
                            $scope.outPutFile = {
                                fileId: payload.configDetails.TemplateFileId
                            }
                            if (payload.configDetails.Tables != undefined && payload.configDetails.Tables.length > 0)
                                table = payload.configDetails.Tables[0];
                        }
                       
                    }


                    var loadFirstRowPromiseDeferred = UtilsService.createPromiseDeferred();
                    firstRowReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            context: buildOutputContext(),
                           
                        };
                        if (table != undefined) {
                            payload.fieldMapping = {
                                RowIndex: table.RowIndex,
                                SheetIndex: table.SheetIndex,
                                CellIndex:0
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(firstRowAPI, payload, loadFirstRowPromiseDeferred);
                    });
                    promises.push(loadFirstRowPromiseDeferred.promise);
                    promises.push(loadOutputMappingFields(table));
                    return UtilsService.waitMultiplePromises(promises);
                   
                    function loadOutputMappingFields(table) {
                        var promises = [];
                        if (table != undefined && table.FieldsMapping != undefined && table.FieldsMapping.length > 0)
                        {
                            for (var i = 0; i < table.FieldsMapping.length; i++) {
                                var item = table.FieldsMapping[i];
                                var configItem;
                                if (item.FieldValue != undefined)
                                    configItem = UtilsService.getItemByVal($scope.outPutFieldMappings, item.FieldValue.ConfigId, "configId");
                                var dataItem = {
                                    id: ctrl.datasource.length + 1,
                                    editor: configItem.editor,
                                    configId: configItem.configId,
                                };
                                dataItem.readyPromiseDeferred = UtilsService.createPromiseDeferred();
                                dataItem.loadPromiseDeferred = UtilsService.createPromiseDeferred();
                                dataItem.fieldReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                                dataItem.loadFieldPromiseDeferred = UtilsService.createPromiseDeferred();
                                promises.push(dataItem.loadPromiseDeferred.promise);
                                promises.push(dataItem.loadFieldPromiseDeferred.promise);
                                setOutputFieldMappingAPI(dataItem,item, table);
                            }
                           
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }

                    function setOutputFieldMappingAPI(dataItem,item, table) {
                        var fieldPayload = {
                            context: buildOutputContext(),
                        };
                        var directivePayload;
                        if (item != undefined && table !=undefined)
                        {
                           
                            fieldPayload.fieldMapping = {
                                SheetIndex: table.SheetIndex,
                                RowIndex: table.RowIndex,
                                CellIndex: item.CellIndex
                            }
                           
                            if(item.FieldValue !=undefined)
                            {
                                directivePayload = {
                                    outputFieldValue: item.FieldValue
                                }
                            }
                        }
                        dataItem.onFieldReady = function (api) {
                            
                            dataItem.fieldAPI = api;

                            dataItem.fieldReadyPromiseDeferred.resolve();
                        }

                        dataItem.fieldReadyPromiseDeferred.promise
                              .then(function () {
                                  VRUIUtilsService.callDirectiveLoad(dataItem.fieldAPI, fieldPayload, dataItem.loadFieldPromiseDeferred);
                              });

         
                        dataItem.onDirectiveReady = function (api) {

                            dataItem.directiveAPI = api;

                            dataItem.readyPromiseDeferred.resolve();
                        }

                        dataItem.readyPromiseDeferred.promise
                              .then(function () {
                                  VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, directivePayload, dataItem.loadPromiseDeferred);
                              });
                        ctrl.datasource.push(dataItem);
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var firstRow;
                    if(firstRowAPI !=undefined)
                    {
                        firstRow = firstRowAPI.getData();
                    }

                    var fieldsMapping;
                    if(ctrl.datasource != undefined && ctrl.datasource.length > 0)
                    {
                        fieldsMapping = [];
                        for(var i=0;i<ctrl.datasource.length;i++)
                        {
                            var item = ctrl.datasource[i];

                            var cellIndex;
                            if(item.fieldAPI != undefined && item.fieldAPI.getData() !=undefined)
                                cellIndex = item.fieldAPI.getData().CellIndex;
                            var fieldValueData;
                            if(item.directiveAPI !=undefined)
                            {
                                fieldValueData = item.directiveAPI.getData();
                                if(fieldValueData != undefined)
                                {
                                    fieldValueData.ConfigId = item.configId;
                                }
                            }
                            fieldsMapping.push({
                                CellIndex:cellIndex,
                                FieldValue: fieldValueData
                            });
                        }
                    }


                    var tables =[{
                        SheetIndex: firstRow != undefined ? firstRow.SheetIndex : undefined,
                        RowIndex: firstRow != undefined ? firstRow.RowIndex : undefined,
                        FieldsMapping:fieldsMapping
                    }];
                    var data = {
                        $type: "XBooster.PriceListConversion.MainExtensions.OutputPriceListSettings.BasicOutputPriceListSettings,XBooster.PriceListConversion.MainExtensions",
                        TemplateFileId: $scope.outPutFile.fileId,
                        Tables:tables
                    }
                    return data;
                }

     
            }
            function buildOutputContext() {
                var context = {
                    getSelectedCell: getSelectedCell,
                    setSelectedCell: selectCellAtSheet,
                    getSelectedSheet: getSelectedSheet,
                    getFirstRowIndex: getFirstRowIndex
                }
                function selectCellAtSheet(row, col, s) {
                    var a = parseInt(row);
                    var b = parseInt(col);
                    if (outPutWorkBookAPI != undefined && outPutWorkBookAPI.getSelectedSheetApi() != undefined)
                        outPutWorkBookAPI.selectCellAtSheet(a, b, s);
                }
                function getSelectedCell() {
                    if (outPutWorkBookAPI != undefined && outPutWorkBookAPI.getSelectedSheetApi() != undefined)
                        return outPutWorkBookAPI.getSelectedSheetApi().getSelected();
                }
                function getSelectedSheet() {
                    if (outPutWorkBookAPI != undefined)
                        return outPutWorkBookAPI.getSelectedSheet();
                }
                function getFirstRowIndex() {
                    var firstRow = firstRowAPI !=undefined? firstRowAPI.getData():undefined;
                    if (firstRow != undefined)
                        return { row: firstRow.RowIndex, sheet: firstRow.SheetIndex }
               
                }
                return context;
            }
        }
    }

    app.directive('xboosterPricelistconversionOutputpricelistconfigurationBasic', OutputpricelistconfigurationBasic);

})(app);