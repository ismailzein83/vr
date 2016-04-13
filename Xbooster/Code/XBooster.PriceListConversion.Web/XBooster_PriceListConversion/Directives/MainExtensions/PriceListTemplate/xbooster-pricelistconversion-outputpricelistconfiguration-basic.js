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
            templateUrl: "/Client/Modules/XBooster_PriceListConversion/Directives/MainExtensions/PriceListTemplate/Templates/BasicOutputPriceListConfiguration.html"
        };

        function Outputpricelistconfiguration($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var outPutWorkBookAPI;
            $scope.outPutFieldMappings;
            function initializeController() {
                $scope.onOutPutReadyWoorkBook = function (api) {
                    outPutWorkBookAPI = api;
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.outPutFieldMappings = payload.fieldMappings;
                        if (payload.configDetails !=undefined)
                        {
                            $scope.outPutFile = {
                                fileId: payload.configDetails.TemplateFileId
                            }
                        }
                        return loadOutputMappingFields(payload);
                    }


                    function loadOutputMappingFields(payload) {
                        var promises = [];
                        for (var i = 0; i < $scope.outPutFieldMappings.length; i++) {
                            var item = $scope.outPutFieldMappings[i];
                            item.readyPromiseDeferred = UtilsService.createPromiseDeferred(),
                            item.loadPromiseDeferred = UtilsService.createPromiseDeferred()
                            promises.push(item.loadPromiseDeferred.promise);
                            setOutputFieldMappingAPI(item, payload);
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }
                    function setOutputFieldMappingAPI(dataItem, payloadData) {
                        var payload = {
                            context: buildOutputContext(),
                        };
                        if (payloadData != undefined && payloadData.configDetails != undefined)
                        {
                       
                            payload.cellFieldData = {
                                sheetIndex: payloadData.configDetails.SheetIndex,
                                rowIndex: payloadData.configDetails.FirstRowIndex,
                                cellIndex: getCellIndex(dataItem, payloadData.configDetails)
                            }
                        }
                        dataItem.onFieldReady = function (api) {
                            dataItem.fieldMappingAPI = api;

                            dataItem.readyPromiseDeferred.resolve();
                        }

                        dataItem.readyPromiseDeferred.promise
                              .then(function () {
                                  VRUIUtilsService.callDirectiveLoad(dataItem.fieldMappingAPI, payload, dataItem.loadPromiseDeferred);
                              });
                    }
                    function getCellIndex(dataItem,configDetails)
                    {
                        switch(dataItem.fieldName)
                        {
                            case "FirstRow": return configDetails.FirstRowIndex;
                            case "Code": return configDetails.CodeCellIndex;
                            case "Zone": return configDetails.ZoneCellIndex;
                            case "Rate": return configDetails.RateCellIndex;
                            case "EffectiveDate": return configDetails.EffectiveDateCellIndex;

                        }
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                        var data = {
                            $type: "XBooster.PriceListConversion.MainExtensions.OutputPriceListSettings.BasicOutputPriceListSettings,XBooster.PriceListConversion.MainExtensions",
                            TemplateFileId: $scope.outPutFile.fileId,
                            SheetIndex: $scope.outPutFieldMappings[0].fieldMappingAPI.getData().SheetIndex,
                            FirstRowIndex: $scope.outPutFieldMappings[0].fieldMappingAPI.getData().RowIndex,
                            CodeCellIndex: $scope.outPutFieldMappings[1].fieldMappingAPI.getData().CellIndex,
                            ZoneCellIndex: $scope.outPutFieldMappings[2].fieldMappingAPI.getData().CellIndex,
                            RateCellIndex: $scope.outPutFieldMappings[3].fieldMappingAPI.getData().CellIndex,
                            EffectiveDateCellIndex: $scope.outPutFieldMappings[4].fieldMappingAPI.getData().CellIndex,
                        }

                    return data;
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
                        var firstRow = $scope.outPutFieldMappings[0];
                        if (firstRow.fieldMappingAPI != undefined) {
                            var obj = firstRow.fieldMappingAPI.getData();
                            if (obj != undefined)
                                return { row: obj.RowIndex }

                        }
                    }
                    return context;
                }
            }
        }
    }

    app.directive('xboosterPricelistconversionOutputpricelistconfigurationBasic', OutputpricelistconfigurationBasic);

})(app);