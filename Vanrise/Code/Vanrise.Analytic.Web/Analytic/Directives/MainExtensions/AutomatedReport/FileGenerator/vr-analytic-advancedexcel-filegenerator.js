﻿"use strict";
app.directive("vrAnalyticAdvancedexcelFilegenerator", ["UtilsService", "VRAnalytic_AdvancedExcelFileGeneratorService", "VRNotificationService", "VRUIUtilsService","VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService",
function (UtilsService, VRAnalytic_AdvancedExcelFileGeneratorService, VRNotificationService, VRUIUtilsService, VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var advancedExcel = new AdvancedExcel($scope, ctrl, $attrs);
            advancedExcel.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/FileGenerator/Templates/AdvancedExcelFileGeneratorTemplate.html"
    };


    function AdvancedExcel($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var context;

        var tabsAPI;

        var excelWorkbookAPI;
        var excelWorkbookReadyDeferred = UtilsService.createPromiseDeferred();

        var queryNameSelectorAPI;
        var queryNameSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var listNameSelectorAPI;
        var listNameSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var serialNumberPatternAPI;
        var serialNumberPatternReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        var mappedTables = [];
        var tableIndex = 0;

        var queries;

        var querySelected;
        var querySelectedPromise;

        var listNameSelected;

        var isEditMode;

        function initializeController() {

                $scope.scopeModel = {};
                
                $scope.scopeModel.tables = [];
                $scope.scopeModel.queries = [];
                $scope.scopeModel.listNames = [];

                $scope.scopeModel.onExcelWorkbookReady = function (api) {
                    excelWorkbookAPI = api;
                    excelWorkbookReadyDeferred.resolve();
                };

                $scope.scopeModel.onSerialNumberPatternReady = function (api) {
                    serialNumberPatternAPI = api;
                    serialNumberPatternReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.addMappedTable = function () {
                    var mappedTableItem = {
                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                        loadPromiseDeferred: UtilsService.createPromiseDeferred()
                    };
                    querySelected = $scope.scopeModel.querySelected;
                    listNameSelected = $scope.scopeModel.listNameSelected.value;

                    addMappedTableTab(mappedTableItem);
                    tabsAPI.setLastTabSelected();
                    $scope.scopeModel.querySelected = undefined;
                    $scope.scopeModel.listNameSelected = undefined;
                };


                $scope.scopeModel.onTabsReady = function (api) {
                    tabsAPI = api;
                };
                
                $scope.scopeModel.onQueryNameSelectorReady = function (api) {
                    queryNameSelectorAPI = api;
                    queryNameSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onListNameSelectorReady = function (api) {
                    listNameSelectorAPI = api;
                    listNameSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.removeTable = function (obj) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.tables, obj.data.tableTabIndex, 'tableTabIndex');
                    $scope.scopeModel.tables.splice(index, 1);
                };
                
                $scope.scopeModel.disableAddMappedTable = function () {
                    return ($scope.scopeModel.querySelected == undefined || $scope.scopeModel.listNameSelected==undefined);
                };

                $scope.scopeModel.onQuerySelectionChanged = function (query) {
                    if (query != undefined) {
                        $scope.scopeModel.listNames.length = 0;
                        var listNameSelectorDataPromise = context.getQueryListNames(query.value);
                        listNameSelectorDataPromise.then(function (listNames) {
                            if (listNames != undefined) {
                                for (var i = 0; i < listNames.length ; i++) {
                                    var listName = listNames[i];
                                    $scope.scopeModel.listNames.push({
                                        description: listName,
                                        value: listName
                                    });
                                }
                            }
                            listNameSelectorAPI.selectIfSingleItem();
                        });
                    }
                };
                UtilsService.waitMultiplePromises([excelWorkbookReadyDeferred.promise, queryNameSelectorReadyDeferred.promise, listNameSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                var queryArrayPromise = UtilsService.createPromiseDeferred();

                if (payload != undefined) {
                    var tableDefinitions = payload.fileGenerator != undefined ? payload.fileGenerator.TableDefinitions : undefined;
                    var fileId = payload.fileGenerator != undefined ? payload.fileGenerator.FileTemplateId : undefined;
                    context = payload.context;
                }

                if (context != undefined) {
                    promises.push(queryArrayPromise.promise);
                    queries = context.getQueryInfo();
                    queryNameSelectorReadyDeferred.promise.then(function () {
                        if (queries != undefined) {
                            for (var i = 0; i < queries.length ; i++) {
                                var query = queries[i];
                                $scope.scopeModel.queries.push({
                                    description: query.QueryTitle,
                                    value: query.VRAutomatedReportQueryId
                                });
                            }
                            queryNameSelectorAPI.selectIfSingleItem();
                            queryArrayPromise.resolve();
                        }
                    });
                }
                if (tableDefinitions != undefined) {
                    mappedTables = tableDefinitions;
                }
                if (mappedTables != undefined && mappedTables.length > 0) {
                    var mappedTable = mappedTables[0];
                    listNameSelected = mappedTable.ListName;
                    querySelectedPromise = UtilsService.createPromiseDeferred();
                    queryArrayPromise.promise.then(function () {
                        querySelected = UtilsService.getItemByVal($scope.scopeModel.queries, mappedTable.VRAutomatedReportQueryId, "value");
                        querySelectedPromise.resolve();
                    });
                }
                if (fileId != undefined)
                {
                    $scope.scopeModel.file = { fileId: fileId };
                }

                promises.push(loadMappedTables());
                promises.push(loadSerialNumberPattern());
                function loadMappedTables() {
                    var promises = [];
                    $scope.scopeModel.isLoadingTabs = true;
                    for (var i = 0 ; i < mappedTables.length; i++) {
                        var mappedTableItem = {
                            payload: mappedTables[i],
                            readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                            loadPromiseDeferred: UtilsService.createPromiseDeferred()
                        };
                        promises.push(mappedTableItem.loadPromiseDeferred.promise);
                        addMappedTableTab(mappedTableItem);
                    }
                    return UtilsService.waitMultiplePromises(promises).catch(function (error) {
                        VRNotificationService.showError(error);
                    }).finally(function () {
                        $scope.scopeModel.isLoadingTabs = false;
                    });

                }
                function loadSerialNumberPattern() {
                    var serialNumberPatternDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    serialNumberPatternReadyPromiseDeferred.promise.then(function () {
                        var serialNumberPatternDirectivePayload = {};
                        if (payload != undefined && payload.fileGenerator!=undefined)
                            serialNumberPatternDirectivePayload.serialNumberPattern = payload.fileGenerator.SerialNumberPattern;
                        VRUIUtilsService.callDirectiveLoad(serialNumberPatternAPI, serialNumberPatternDirectivePayload, serialNumberPatternDeferredLoadPromiseDeferred);
                    });
                    return serialNumberPatternDeferredLoadPromiseDeferred.promise;
                }
                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
              var obj = {
                   $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators.AdvancedExcelFileGenerator,Vanrise.Analytic.MainExtensions",
                   FileTemplateId: $scope.scopeModel.file != undefined ? $scope.scopeModel.file.fileId : undefined,
                   SerialNumberPattern: serialNumberPatternAPI.getData(),
                   TableDefinitions: getTables()
              };
              return obj;

                function getTables() {
                    var mappedTables = [];
                    if ($scope.scopeModel.tables != undefined) {
                        for (var i = 0; i < $scope.scopeModel.tables.length; i++) {
                            var table = $scope.scopeModel.tables[i];
                            var tableDefinition = table.directiveAPI.getData();
                            tableDefinition.VRAutomatedReportQueryId = querySelected.value;
                            tableDefinition.ListName = listNameSelected;
                            mappedTables.push(tableDefinition);
                        }
                    }
                    return mappedTables;
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

       
        function addMappedTableTab(mappedTableItem) {
            if (querySelectedPromise != undefined) {
                querySelectedPromise.promise.then(function () {
                    if (querySelected != undefined) {
                        var mappedTableTab = {
                            header: querySelected.description + " - " + listNameSelected + ' (' + tableIndex + ')',
                            tableTabIndex: ++tableIndex,
                            Editor: "vr-analytic-advancedexcel-filegenerator-mappedtable",
                            onDirectiveReady: function (api) {
                                mappedTableTab.directiveAPI = api;
                                mappedTableItem.readyPromiseDeferred.resolve();
                            },
                            loadPromiseDeferred: mappedTableItem.loadPromiseDeferred
                        };

                        var directivePayload = {
                            context: getContext(),
                            mappedTable: mappedTableItem.payload,
                            showEditButton: false,
                        };
                        mappedTableItem.readyPromiseDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(mappedTableTab.directiveAPI, directivePayload, mappedTableTab.loadPromiseDeferred);
                        });
                        $scope.scopeModel.tables.push(mappedTableTab);
                    }
                    else {
                        mappedTableItem.loadPromiseDeferred.reject("A query used in this handler has been deleted.");
                    }
                });
            }
            else {
                var mappedTableTab = {
                    header: querySelected.description + " - " + listNameSelected + ' (' + tableIndex + ')',
                    tableTabIndex: ++tableIndex,
                    Editor: "vr-analytic-advancedexcel-filegenerator-mappedtable",
                    onDirectiveReady: function (api) {
                        mappedTableTab.directiveAPI = api;
                        mappedTableItem.readyPromiseDeferred.resolve();
                    },
                    loadPromiseDeferred: mappedTableItem.loadPromiseDeferred
                };

                var directivePayload = {
                    context: getContext(),
                    mappedTable: mappedTableItem.payload,
                    showEditButton: false,
                };
                mappedTableItem.readyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(mappedTableTab.directiveAPI, directivePayload, mappedTableTab.loadPromiseDeferred);
                });
                $scope.scopeModel.tables.push(mappedTableTab);
            }
        }


        function getContext() {
            var currentContext = UtilsService.cloneObject(context);
            if (currentContext == undefined)
                currentContext = {};
            currentContext. getSelectedSheetApi =  function () {
                return excelWorkbookAPI.getSelectedSheetApi();
            };
            currentContext.selectCellAtSheet = function (rowIndex, columnIndex, sheetIndex) {
                return excelWorkbookAPI.selectCellAtSheet(rowIndex, columnIndex, sheetIndex);
            };
            currentContext.getSelectedSheet =  function () {
                return excelWorkbookAPI.getSelectedSheet();
            };

            currentContext.getSelectedQuery = function () {
                return querySelected;
            };
            return currentContext;
        }
    }

    return directiveDefinitionObject;
}
]);