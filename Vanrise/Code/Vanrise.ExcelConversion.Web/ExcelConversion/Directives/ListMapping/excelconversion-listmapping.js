(function (app) {

    'use strict';

    excelconversionListmapping.$inject = ['UtilsService', 'VRUIUtilsService'];

    function excelconversionListmapping(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var listmapping = new Listmapping($scope, ctrl, $attrs);
                listmapping.initializeController();
            },
            controllerAs: "listMappingCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/ExcelConversion/Directives/ListMapping/Templates/ListMappingTemplate.html"
        };

        function Listmapping($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            var listName;
            var listMappingData;
            function initializeController() {
                ctrl.fieldMappings = [];
                ctrl.updateLastRowIndexRange = function () {
                    if (context != undefined) {
                        var range = context.getSelectedCell();
                        if (range != undefined) {
                            ctrl.lastRowIndex = {
                                row: range[0],
                                col: range[1],
                                sheet: context.getSelectedSheet()
                            }
                        }

                    }

                }
                ctrl.selectLastRowIndex = function () {
                    if (context != undefined) {
                        context.setSelectedCell(ctrl.lastRowIndex.row, ctrl.lastRowIndex.col, ctrl.lastRowIndex.sheet);
                    }
                }
                ctrl.updateFirstRowIndexRange = function () {
                    if (context != undefined) {
                        var range = context.getSelectedCell();
                        if (range != undefined) {
                            ctrl.firstRowIndex = {
                                row: range[0],
                                col: range[1],
                                sheet: context.getSelectedSheet(),
                            }
                        }

                    }

                }
                ctrl.selectFirstRowIndex = function () {
                    if (context != undefined) {
                        context.setSelectedCell(ctrl.firstRowIndex.row, ctrl.firstRowIndex.col, ctrl.firstRowIndex.sheet);
                    }
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        var promises = [];
                        context = payload.context;
                        listName = payload.listName;
                        listMappingData = payload.listMappingData;
                        if (listMappingData != undefined)
                        {
                           
                            if (listMappingData.FirstRowIndex !=undefined)
                            {
                                ctrl.firstRowIndex = {
                                    row: listMappingData.FirstRowIndex,
                                    col: listMappingData.FirstRowIndex,
                                    sheet: listMappingData.SheetIndex
                                }
                            }
                            if (listMappingData.LastRowIndex != undefined)
                            {
                                ctrl.lastRowIndex = {
                                    row: listMappingData.LastRowIndex,
                                    col: listMappingData.LastRowIndex,
                                    sheet: listMappingData.SheetIndex
                                }
                            }

                        }
                        ctrl.fieldMappings = payload.fieldMappings;
                        for (var i = 0; i < ctrl.fieldMappings.length; i++) {
                            var item = ctrl.fieldMappings[i];
                            item.readyPromiseDeferred = UtilsService.createPromiseDeferred(),
                            item.loadPromiseDeferred = UtilsService.createPromiseDeferred()
                            promises.push(item.loadPromiseDeferred.promise);
                            addAPIExtension(ctrl.fieldMappings[i]);
                        }


                    }
                    function addAPIExtension(dataItem) {
                        var payload = {
                            context: getContext()
                        };

                        dataItem.normalColNum = ctrl.normalColNum;

                        dataItem.onFieldMappingReady = function (api) {
                            dataItem.fieldMappingAPI = api;
                            dataItem.readyPromiseDeferred.resolve();
                        }
                        dataItem.readyPromiseDeferred.promise
                      .then(function () {
                          if (listMappingData != undefined && listMappingData.FieldMappings != undefined && listMappingData.FieldMappings.length > 0) {
                              var fieldMapping = UtilsService.getItemByVal(listMappingData.FieldMappings, dataItem.FieldName, "FieldName");
                              if (fieldMapping != undefined) {
                                  payload.fieldMapping = fieldMapping;
                                 
                              }

                          }
                          VRUIUtilsService.callDirectiveLoad(dataItem.fieldMappingAPI, payload, dataItem.loadPromiseDeferred);
                      });
                    }

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var fieldMappings;
                    if (ctrl.fieldMappings.length > 0) {
                        fieldMappings = [];
                        for (var i = 0; i < ctrl.fieldMappings.length; i++) {
                            var fieldMapping = ctrl.fieldMappings[i];
                            if (fieldMapping.fieldMappingAPI != undefined) {
                                var fieldMappingData = fieldMapping.fieldMappingAPI.getData();
                                if (fieldMappingData != undefined) {
                                    fieldMappingData.FieldName = fieldMapping.FieldName;
                                    fieldMappingData.FieldType = fieldMapping.FieldType;
                                    fieldMappings.push(fieldMappingData);
                                }

                            }

                        }

                    }
                    var data = {
                        ListName: listName,
                        SheetIndex: ctrl.firstRowIndex != undefined ? ctrl.firstRowIndex.sheet : undefined,
                        FirstRowIndex: ctrl.firstRowIndex != undefined ? ctrl.firstRowIndex.row : undefined,
                        LastRowIndex: ctrl.lastRowIndex != undefined ? ctrl.lastRowIndex.row : undefined,
                        FieldMappings: fieldMappings,
                    };
                    return data;
                }
                function getContext() {

                    if (context != undefined) {
                        var currentContext = UtilsService.cloneObject(context);
                        if (currentContext == undefined)
                            currentContext = {};
                        currentContext.getFirstRowIndex = function () {
                            return ctrl.firstRowIndex;
                        }
                        return currentContext;
                    }
                }
            }
        }
    }

    app.directive('vrExcelconversionListmapping', excelconversionListmapping);

})(app);