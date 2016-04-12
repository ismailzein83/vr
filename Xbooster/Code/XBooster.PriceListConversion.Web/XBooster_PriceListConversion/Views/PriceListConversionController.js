(
    function (appControllers) {
        "use strict";
        priceListConversionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService','ExcelConversion_PriceListConversionAPIService'];
        function priceListConversionController($scope, UtilsService, VRNotificationService, VRUIUtilsService, ExcelConversion_PriceListConversionAPIService) {

            var inputWorkBookApi;
            var outPutWorkBookAPI;
            var rateListAPI;
            var rateListMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var codeListAPI;
            var codeListMappingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            defineScope();
            load();

            function defineScope() {

                $scope.scopeModel = {};
                $scope.scopeModel.showInputExcelToMap = true;
                $scope.scopeModel.showOutputExcelToMap = false;

                $scope.scopeModel.onReadyWoorkBook = function (api) {
                    inputWorkBookApi = api;
                }

                $scope.scopeModel.onCodeListMappingReady = function (api) {
                    codeListAPI = api;
                    codeListMappingReadyPromiseDeferred.resolve();
                }

                $scope.scopeModel.onRateListMappingReady = function (api) {
                    rateListAPI = api;
                    rateListMappingReadyPromiseDeferred.resolve();
                }

                $scope.scopeModel.convertAndDownload = function () {
                    var listMappings = [];
                    listMappings.push(codeListAPI.getData());
                    listMappings.push(rateListAPI.getData());
                    var obj = {
                        ListMappings: listMappings,
                        FieldMappings: null,
                        DateTimeFormat: "yyyy/MM/dd"
                    }
                    var excelToConvert = {
                        ExcelConversionSettings: obj,
                        FileId: $scope.scopeModel.inPutFile.fileId
                    }
                    return ExcelConversion_PriceListConversionAPIService.ConvertAndDownload(excelToConvert).then(function (response) {
                        UtilsService.downloadFile(response.data, response.headers);
                    });;
                }

                $scope.scopeModel.startConvertAndDownload = function () {
                    var listMappings = [];
                    listMappings.push(codeListAPI.getData());
                    listMappings.push(rateListAPI.getData());
                    var obj = {
                        ListMappings: listMappings,
                        FieldMappings: null,
                        DateTimeFormat: "yyyy/MM/dd"
                    }
                    var inputExcel = {
                        ExcelConversionSettings: obj,
                        FileId: $scope.scopeModel.inPutFile.fileId
                    }

                    var outputFieldMappings = {
                        SheetIndex:$scope.scopeModel.outPutFieldMappings[0].fieldMappingAPI.getData().SheetIndex,
                        FirstRowIndex: $scope.scopeModel.outPutFieldMappings[0].fieldMappingAPI.getData().RowIndex,
                        CodeCellIndex: $scope.scopeModel.outPutFieldMappings[1].fieldMappingAPI.getData().CellIndex,
                        ZoneCellIndex: $scope.scopeModel.outPutFieldMappings[2].fieldMappingAPI.getData().CellIndex,
                        RateCellIndex: $scope.scopeModel.outPutFieldMappings[3].fieldMappingAPI.getData().CellIndex,
                        EffectiveDateCellIndex: $scope.scopeModel.outPutFieldMappings[4].fieldMappingAPI.getData().CellIndex,
                    };

                    var outPutExcel = {
                        FileId: $scope.scopeModel.outPutFile.fileId,
                        OutputPriceListFields: outputFieldMappings
                    }
                    var priceListConversion = {
                            InputPriceListSettings: inputExcel,
                            OutputPriceListSettings:outPutExcel
                        }
                    return ExcelConversion_PriceListConversionAPIService.PriceListConvertAndDownload(priceListConversion).then(function (response) {
                        UtilsService.downloadFile(response.data, response.headers);
                    });;
                }

                $scope.scopeModel.next = function () {
                    $scope.scopeModel.showInputExcelToMap = false;
                    $scope.scopeModel.showOutputExcelToMap = true;
                }

                $scope.scopeModel.back = function () {
                    $scope.scopeModel.showInputExcelToMap = true;
                    $scope.scopeModel.showOutputExcelToMap = false;
                }

                $scope.scopeModel.outPutFieldMappings = [{ fieldName: "First Row", isRequired: true, type: "row" }, { fieldName: "Code", isRequired: true, type: "cell" }, { fieldName: "Zone", isRequired: true, type: "cell" }, { fieldName: "Rate", isRequired: true, type: "cell" }, { fieldName: "Effective Date", isRequired: true, type: "cell" }]

                $scope.scopeModel.onOutPutReadyWoorkBook = function (api) {
                    outPutWorkBookAPI = api;
                }

            }

            function load() {
                $scope.scopeModel.isLoading = true;
                loadAllControls();
            }

            function loadAllControls()
            {
                return UtilsService.waitMultipleAsyncOperations([loadRateListMapping, loadCodeListMapping, loadOutputMappingFields])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
            }

            function loadOutputMappingFields() {
                var promises = [];
                for (var i = 0; i < $scope.scopeModel.outPutFieldMappings.length; i++) {
                    var item = $scope.scopeModel.outPutFieldMappings[i];
                    item.readyPromiseDeferred = UtilsService.createPromiseDeferred(),
                    item.loadPromiseDeferred = UtilsService.createPromiseDeferred()
                    promises.push(item.loadPromiseDeferred.promise);
                    setOutputFieldMappingAPI(item);
                }
                return UtilsService.waitMultiplePromises(promises);
            }

            function setOutputFieldMappingAPI(dataItem) {
                var payload = {
                    context: buildOutputContext(),
                };
                dataItem.onFieldReady = function (api) {
                    dataItem.fieldMappingAPI = api;
                  
                    dataItem.readyPromiseDeferred.resolve();
                }
                //dataItem.validate = function () {
                //    if (dataItem.fieldMappingAPI != undefined && $scope.scopeModel.outPutFieldMappings[0].fieldMappingAPI != undefined) {
                //        var rowIndex = $scope.scopeModel.outPutFieldMappings[0].fieldMappingAPI.getData();
                //        var obj = dataItem.fieldMappingAPI.getData();
                //        if (obj != undefined) {
                //            if (rowIndex == undefined || obj.RowIndex != rowIndex.RowIndex)
                //                return "Error row index.";
                //        }
                //    }
                //    return null;
                //}
                dataItem.readyPromiseDeferred.promise
                      .then(function () {
                          VRUIUtilsService.callDirectiveLoad(dataItem.fieldMappingAPI, payload, dataItem.loadPromiseDeferred);
                      });
            }

            function loadRateListMapping()
            {
                var loadRateListMappingPromiseDeferred = UtilsService.createPromiseDeferred();
                rateListMappingReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        context: buildContext(),
                        fieldMappings: [{ FieldName: "Rate", FieldTitle: "Rate", isRequired: true, type: "cell" }, { FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell" }, { FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: true, type: "cell" }],
                        listName: "RateList"
                    };
                    VRUIUtilsService.callDirectiveLoad(rateListAPI, payload, loadRateListMappingPromiseDeferred);
                });

                return loadRateListMappingPromiseDeferred.promise;
            }

            function loadCodeListMapping() {
                var loadCodeListMappingPromiseDeferred = UtilsService.createPromiseDeferred();
                codeListMappingReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        context: buildContext(),
                        fieldMappings: [{ FieldName: "Code", FieldTitle: "Code", isRequired: true, type: "cell" }, { FieldName: "Zone", FieldTitle: "Zone", isRequired: true, type: "cell" }, { FieldName: "EffectiveDate", FieldTitle: "Effective Date", isRequired: true, type: "cell" }],
                        listName: "CodeList"
                    };
                    VRUIUtilsService.callDirectiveLoad(codeListAPI, payload, loadCodeListMappingPromiseDeferred);
                });

                return loadCodeListMappingPromiseDeferred.promise;
            }

            function buildContext() {
                var context = {
                    getSelectedCell: getSelectedCell,
                    setSelectedCell: selectCellAtSheet,
                    getSelectedSheet: getSelectedSheet
                }
                function selectCellAtSheet(row, col, s) {
                    var a = parseInt(row);
                    var b = parseInt(col);
                    if (inputWorkBookApi != undefined && inputWorkBookApi.getSelectedSheetApi() != undefined)
                        inputWorkBookApi.selectCellAtSheet(a, b, s);
                }
                function getSelectedCell() {
                    if (inputWorkBookApi != undefined && inputWorkBookApi.getSelectedSheetApi() != undefined)
                        return inputWorkBookApi.getSelectedSheetApi().getSelected();
                }
                function getSelectedSheet() {
                    if (inputWorkBookApi != undefined)
                        return inputWorkBookApi.getSelectedSheet();
                }
                return context;
            }

            function buildOutputContext() {
                var context = {
                    getSelectedCell: getSelectedCell,
                    setSelectedCell: selectCellAtSheet,
                    getSelectedSheet: getSelectedSheet,
                    getFirstRowIndex: getFirstRowIndex
                }
                function selectCellAtSheet(row, col,s) {
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
                function getFirstRowIndex()
                {
                    var firstRow = $scope.scopeModel.outPutFieldMappings[0];
                    if (firstRow.fieldMappingAPI != undefined)
                    {
                        var obj = firstRow.fieldMappingAPI.getData();
                        if(obj !=undefined)
                            return { row: obj.RowIndex }

                    }
                }
                return context;
            }

        }

        appControllers.controller('ExcelConversion_PriceListConversionController', priceListConversionController);
    }
)(appControllers);