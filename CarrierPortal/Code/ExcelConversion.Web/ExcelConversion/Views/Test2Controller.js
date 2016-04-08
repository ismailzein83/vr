(
    function (appControllers) {
        "use strict";

        function testController($scope, UtilsService, VRNotificationService, VRUIUtilsService, excelAPIService) {

            var inputWorkBookApi;
            var outPutWorkBookAPI;
            var codeListAPI;
            var rateListAPI;
            defineScope();
            load();
            function defineScope() {
                
                $scope.scopeModel = {};
                $scope.scopeModel.onReadyWoorkBook = function (api) {
                    inputWorkBookApi = api;
                }
                $scope.scopeModel.onFieldMappingReady = function (api)
                {
                    var payload = {
                        context: buildContext(),
                      
                    };
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, payload, setLoader);
                }
                $scope.scopeModel.onCodeListMappingReady = function (api) {
                    codeListAPI = api;
                    var payload = {
                        context: buildContext(),
                        fieldMappings: [{ FieldName: "Code", isRequired: true, type: "cell" }, { FieldName: "Zone", isRequired: true, type: "cell" }, { FieldName: "BED", isRequired: true, type: "cell" }, { FieldName: "EED", type: "cell" }],
                        listName:"CodeList"
                    };
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, payload, setLoader);
                }
                $scope.scopeModel.onRateListMappingReady = function (api) {
                    rateListAPI = api;
                    var payload = {
                        context: buildContext(),
                        fieldMappings: [{ FieldName: "Rate", isRequired: true, type: "cell" }, { FieldName: "Zone", isRequired: true, type: "cell" }, { FieldName: "BED", isRequired: true, type: "cell" }, { FieldName: "EED", type: "cell" }],
                        listName: "RateList"
                    };
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, payload, setLoader);
                }

                $scope.scopeModel.convertAndDownload = function()
                {
                    var listMappings = [];
                    listMappings.push(codeListAPI.getData());
                    listMappings.push(rateListAPI.getData());
                    var obj = {
                        ListMappings:listMappings,
                        FieldMappings:null,
                        DateTimeFormat:"yyyy/MM/dd"
                    }
                    var excelToConvert = {
                        ExcelConversionSettings: obj,
                        FileId: $scope.scopeModel.inPutFile.fileId
                    }
                    return excelAPIService.ConvertAndDownload(excelToConvert).then(function (response) {
                        UtilsService.downloadFile(response.data, response.headers);
                    });;
                }

                $scope.scopeModel.showInputExcelToMap = true;
                $scope.scopeModel.showOutputExcelToMap = false;
                $scope.scopeModel.next = function()
                {
                    $scope.scopeModel.showInputExcelToMap = false;
                    $scope.scopeModel.showOutputExcelToMap = true;
                }
                $scope.scopeModel.back = function () {
                    $scope.scopeModel.showInputExcelToMap = true;
                    $scope.scopeModel.showOutputExcelToMap = false;
                }
                $scope.scopeModel.outPutFieldMappings = [{ fieldName: "First Row", isRequired: true, type: "row" }, { fieldName: "Code", isRequired: true, type: "cell" }, { fieldName: "Zone", isRequired: true, type: "cell" }, { fieldName: "Rate", isRequired: true, type: "cell" }, { fieldName: "Effective Date", isRequired: true, type: "cell" }]

                $scope.scopeModel.onOutPutReadyWoorkBook = function(api)
                {
                    outPutWorkBookAPI = api;
                }
                loadOutputMappingFields();

            }

            function loadOutputMappingFields()
            {
                for(var i=0;i<$scope.scopeModel.outPutFieldMappings.length;i++)
                {
                    setOutputFieldMappingAPI($scope.scopeModel.outPutFieldMappings[i]);
                    
                }
            }
            function setOutputFieldMappingAPI(dataItem)
            {
                dataItem.onFieldReady = function (api) {
                    dataItem.fieldMappingAPI = api;
                    var payload = {
                        context: buildOutputContext(),
                    };
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, payload, setLoader);
                }
            }

            function load() {

            }
            function buildContext()
            {
                var context = {
                    getSelectedCell: getSelectedCell,
                    setSelectedCell: setSelectedCell,
                    getSelectedSheet: getSelectedSheet
                }
                function setSelectedCell(row, col) {
                    var a = parseInt(row);
                    var b = parseInt(col);
                    if (inputWorkBookApi != undefined && inputWorkBookApi.getSelectedSheetApi() != undefined)
                        inputWorkBookApi.getSelectedSheetApi().selectCell(a, b, a, b);
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
                    setSelectedCell: setSelectedCell,
                    getSelectedSheet: getSelectedSheet
                }
                function setSelectedCell(row, col) {
                    var a = parseInt(row);
                    var b = parseInt(col);
                    if (outPutWorkBookAPI != undefined && outPutWorkBookAPI.getSelectedSheetApi() != undefined)
                        outPutWorkBookAPI.getSelectedSheetApi().selectCell(a, b, a, b);
                }
                function getSelectedCell() {
                    if (outPutWorkBookAPI != undefined && outPutWorkBookAPI.getSelectedSheetApi() != undefined)
                        return outPutWorkBookAPI.getSelectedSheetApi().getSelected();
                }
                function getSelectedSheet() {
                    if (outPutWorkBookAPI != undefined)
                        return outPutWorkBookAPI.getSelectedSheet();
                }
                return context;
            }
           
        }

        testController.$inject = ['$scope','UtilsService','VRNotificationService','VRUIUtilsService','ExcelConversion_ExcelAPIService'];
        appControllers.controller('Test2Controller', testController);
    }
)(appControllers);