//"use strict";

//app.directive("vrAnalyticAdvancedexcelFilegeneratorFixedmappedfieldstable", ["VRUIUtilsService", "UtilsService", "VRNotificationService", "VRAnalytic_AutomatedReportHandlerAPIService",
//    function (VRUIUtilsService, UtilsService, VRNotificationService, VRAnalytic_AutomatedReportHandlerAPIService) {

//        var directiveDefinitionObject = {

//            restrict: "E",
//            scope: {
//                onReady: "="
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var grid = new AdvancedExcelMappedFixedFieldsTableGrid($scope, ctrl, $attrs);
//                grid.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: '/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/FileGenerator/Templates/AdvancedExcelFileGeneratorFixedFieldsMappedTable.html'

//        };

//        function AdvancedExcelMappedFixedFieldsTableGrid($scope, ctrl, $attrs) {

//            var gridAPI;
//            var context;
//            var fixedFieldsEntity;

//            var fixedFields = [];

//            this.initializeController = initializeController;

//            function initializeController() {
//                $scope.scopeModel = {};

//                $scope.scopeModel.onGridReady = function (api) {
//                    gridAPI = api;
//                    defineAPI();
//                };

       
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var initialPromises = [];

//                    context = payload.context;
//                    fixedFieldsEntity = payload.fixedFields;

//                    initialPromises.push(getAdvancedExcelFileGeneratorFixedFieldSettingsConfigs());

//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            var dataItemsLoadPromises = [];

//                            if (fixedFieldsEntity != undefined && fixedFieldsEntity.length>0) {
//                                $scope.scopeModel.isGridLoading = true;
//                                for (var i = 0; i < fixedFieldsEntity.length; i++) {
//                                    var dataItem = {
//                                        payload: fixedFieldsEntity[i],
//                                        readyCellPromiseDeferred: UtilsService.createPromiseDeferred(),
//                                        loadCellPromiseDeferred: UtilsService.createPromiseDeferred(),
//                                        readyFixedFieldPromiseDeferred: UtilsService.createPromiseDeferred(),
//                                        loadFixedFieldPromiseDeferred: UtilsService.createPromiseDeferred(),
//                                        readyFixedFieldValuePromiseDeferred: UtilsService.createPromiseDeferred(),
//                                        loadFixedFieldValuePromiseDeferred: UtilsService.createPromiseDeferred()
//                                    };
//                                    dataItemsLoadPromises.push(dataItem.loadCellPromiseDeferred.promise);
//                                    dataItemsLoadPromises.push(dataItem.loadFixedFieldPromiseDeferred.promise);
//                                    dataItemsLoadPromises.push(dataItem.loadFixedFieldValuePromiseDeferred.promise);
//                                    addItemToGrid(dataItem);
//                                }
//                                $scope.scopeModel.isGridLoading = false;
//                            }

//                            return {
//                                promises: dataItemsLoadPromises
//                            };
//                        }
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };

//                api.getData = function () {

//                };

//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
//                    ctrl.onReady(api);
//            }

//            function getAdvancedExcelFileGeneratorFixedFieldSettingsConfigs() {
//                return VRAnalytic_AutomatedReportHandlerAPIService.GetAdvancedExcelFileGeneratorFixedFieldSettingsConfigs().then(function (response) {
//                    if (response != undefined) {
//                        for (var i = 0; i < response.length; i++) {
//                            fixedFields.push(response[i]);
//                        }
//                        if (settings != undefined) {
//                            $scope.scopeModel.selectedTemplateConfig =
//                                UtilsService.getItemByVal($scope.scopeModel.templateConfigs, vrRestAPIAnalyticQueryInterceptor.ConfigId, 'ExtensionConfigurationId');
//                        }
//                    }
//                });
//            }

//            function addItemToGrid(gridItem) {
//                var dataItem = {
//                    id: $scope.scopeModel.mappedCols.length + 1,
//                };

//                var payload = gridItem.payload;

//                if (inputArgumentsMapping != undefined && inputArgumentsMapping.length > 0) {
//                    var item = UtilsService.getItemByVal(inputArgumentsMapping, dataItem.fieldName, "InputArgumentName");
//                    if (item != undefined) {
//                        gridItem.mappedFieldName = item.MappedFieldName;
//                    }
//                }

//                dataItem.onDataRecordTypeFieldsSelectorReady = function (api) {
//                    dataItem.dataRecordTypeFieldsAPI = api;
//                    gridItem.readyPromiseDeferred.resolve();
//                };

//                gridItem.readyPromiseDeferred.promise.then(function () {
//                    var dataRecordTypeFieldSelectorPayload = {
//                        dataRecordTypeId: context.getDataRecordTypeId(),
//                        selectedIds: gridItem.mappedFieldName
//                    };
//                    VRUIUtilsService.callDirectiveLoad(dataItem.dataRecordTypeFieldsAPI, dataRecordTypeFieldSelectorPayload, gridItem.loadPromiseDeferred);
//                });

//                $scope.scopeModel.mappedCols.push(dataItem);
//            }



//            return directiveDefinitionObject;
//        }

//    }]);
