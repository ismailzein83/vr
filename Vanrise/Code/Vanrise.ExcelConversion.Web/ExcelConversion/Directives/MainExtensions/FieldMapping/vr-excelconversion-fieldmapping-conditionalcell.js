(function (app) {

    'use strict';

    fieldmappingConditionalCellDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function fieldmappingConditionalCellDirective(UtilsService, VRUIUtilsService) {
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
            controllerAs: "ConditionalCellMappingCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/ExcelConversion/Directives/MainExtensions/FieldMapping/Templates/ConditionalCellFieldMappingTemplate.html"
        };

        function ConditionalCell($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var context;

            var selectorAPI;
            var selectorReadyDeferred = UtilsService.createPromiseDeferred();

            var gridAPI;
            var gridReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                ctrl.datasource = [];
                ctrl.datasourcemapping = [];
                ctrl.removeField = function (dataItem) {
                    var index = ctrl.datasourcemapping.indexOf(dataItem);
                    ctrl.datasourcemapping.splice(index, 1);
                };
                ctrl.selectedvalues;
                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    selectorReadyDeferred.resolve();
                };
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    gridReadyDeferred.resolve();
                };

                ctrl.addConditionalCell = function () {
                    var dataItem =
                        {
                            readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                            loadPromiseDeferred: UtilsService.createPromiseDeferred()
                        };
                    addConditionalCellFieldMapping(dataItem);
                };

                UtilsService.waitMultiplePromises([selectorReadyDeferred.promise, gridReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
                
            }
            function addConditionalCellFieldMapping(dataItem) {
                var payload = {
                    context: getContext()
                };
                dataItem.normalColNum = ctrl.normalColNum;
                dataItem.onFieldMappingReady = function (api) {
                    dataItem.fieldMappingAPI = api;
                    dataItem.readyPromiseDeferred.resolve();
                };
                dataItem.readyPromiseDeferred.promise
              .then(function () {

                  VRUIUtilsService.callDirectiveLoad(dataItem.fieldMappingAPI, payload, dataItem.loadPromiseDeferred);
              });
                ctrl.datasourcemapping.push(dataItem);
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                   
                    var parts;
                    var filterItems;

                    if (payload != undefined) {
                        context = payload.context;
                        ctrl.datasource = context.getFilterFieldsMappings();
                    }

                    loadSelectorFieldMapping(payload);

                    var loadGrid = loadGridFieldMapping(payload);
                    promises.push(loadGrid);

                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            };
            function loadSelectorFieldMapping(payload) {
                if (payload != undefined && payload.fieldMapping != undefined && payload.fieldMapping.RowFieldName != undefined) {
                    VRUIUtilsService.setSelectedValues(payload.fieldMapping.RowFieldName, 'FieldName', $attrs, ctrl);
                    selectorloadDeferred.resolve();
                }
            }

            function loadGridFieldMapping(payload) {
                var promises = [];

                if (payload != undefined && payload.fieldMapping != undefined && payload.fieldMapping.Choices != undefined) {

                    for (var i = 0; i < payload.fieldMapping.Choices.length; i++) {
                        
                        var dataItem = payload.fieldMapping.Choices[i];
                        extendDataItem(dataItem);
                        promises.push(dataItem.selectiveLoadDeferred.promise);
                        ctrl.datasourcemapping.push(dataItem);
                    }
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
                if (ctrl.datasourcemapping.length > 0) {
                    var choices = [];
                    for (var i = 0; i < ctrl.datasourcemapping.length; i++) {
                        var fieldsMapping = ctrl.datasourcemapping[i];
                        choices.push({
                            RowFieldValue: fieldsMapping.RowFieldValue,
                            FieldMappingChoice: fieldsMapping.fieldMappingAPI != undefined ? fieldsMapping.fieldMappingAPI.getData() : undefined
                        });
                    }
                    data = {
                        $type: "Vanrise.ExcelConversion.MainExtensions.ConditionalCellFieldMapping, Vanrise.ExcelConversion.MainExtensions",
                        RowFieldName: (ctrl.selectedvalues != undefined) ? ctrl.selectedvalues.FieldName : undefined,
                        Choices: choices
                    };
                }
                return data;
            }

            function getContext() {

                if (context != undefined) {
                    var currentContext = UtilsService.cloneObject(context);
                  
                    return currentContext;
                }
            }
        }

    }

    app.directive('vrExcelconversionFieldmappingConditionalcell', fieldmappingConditionalCellDirective);

})(app);