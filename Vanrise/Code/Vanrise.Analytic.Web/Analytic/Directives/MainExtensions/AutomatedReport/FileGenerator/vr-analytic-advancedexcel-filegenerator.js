"use strict";
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

        var excelFileUploaderAPI;
        var excelFileUploaderReadyDeferred = UtilsService.createPromiseDeferred();

        var mappedTables = [];
        var tableIndex = 0;

        var queries;

        var querySelected;

        var isEditMode;

        function initializeController() {
            var promises = [excelFileUploaderReadyDeferred.promise, queryNameSelectorReadyDeferred.promise, listNameSelectorReadyDeferred.promise];

                $scope.scopeModel = {};
                
                $scope.scopeModel.tables = [];
                $scope.scopeModel.queries = [];
                $scope.scopeModel.listNames = [];

                $scope.scopeModel.onExcelWorkbookReady = function (api) {
                    excelWorkbookAPI = api;
                    excelWorkbookReadyDeferred.resolve();
                };


                $scope.scopeModel.addMappedTable = function () {
                    var mappedTableItem = {
                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                        loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                        VRAutomatedReportQueryId: $scope.scopeModel.querySelected.value,
                        ListName: $scope.scopeModel.listNameSelected.value
                    };

                    addMappedTableTab(mappedTableItem);
                    tabsAPI.setLastTabSelected();
                    $scope.scopeModel.querySelected = undefined;
                    $scope.scopeModel.listNameSelected = undefined;
                    $scope.scopeModel.listNames.length = 0;
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
                    if (context != undefined && context.disableTestGenerateButton != undefined && typeof (context.disableTestGenerateButton) == 'function') {
                        if ($scope.scopeModel.tables.length == 0) {
                            context.disableTestGenerateButton(true);
                        }
                    }
                };
                
                $scope.scopeModel.disableAddMappedTable = function () {
                    return ($scope.scopeModel.querySelected == undefined || $scope.scopeModel.listNameSelected==undefined || $scope.scopeModel.file == undefined);
                };

                $scope.scopeModel.onQuerySelectionChanged = function (query) {
                    if (query != undefined) {
                        $scope.scopeModel.listNames.length = 0;
                        $scope.scopeModel.isLoadingListNames = true;
                        var listNameSelectorDataPromise = context.getQueryListNames(query.value);
                        promises.push(listNameSelectorDataPromise);
                        listNameSelectorDataPromise.then(function (listNames) {
                            if (listNames != undefined) {
                                for (var i = 0; i < listNames.length; i++) {
                                    var listName = listNames[i];
                                    $scope.scopeModel.listNames.push({
                                        description: listName,
                                        value: listName
                                    });
                                }
                            }
                            listNameSelectorAPI.selectIfSingleItem();
                        }).then(function () {
                            $scope.scopeModel.isLoadingListNames = false;
                        });
                    }
                };

                
                $scope.scopeModel.onExcelFileUploaderReady = function (api) {
                    excelFileUploaderAPI = api;
                    excelFileUploaderReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises(promises).then(function () {
                    defineAPI();
                });
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                var queryArrayPromise = UtilsService.createPromiseDeferred();
                var tableDefinitions;
                if (payload != undefined) {
                    context = payload.context;
                    if (context != undefined && context.disableTestGenerateButton != undefined && typeof (context.disableTestGenerateButton) == 'function') {
                        if ($scope.scopeModel.tables.length > 0) {
                            context.disableTestGenerateButton(false);
                        }
                    }
                    var fileGenerator = payload.fileGenerator;
                    if (fileGenerator != undefined) {
                        tableDefinitions = fileGenerator.TableDefinitions;
                        promises.push(excelWorkbookReadyDeferred.promise);
                    }

                }

                if (context != undefined) {
                    promises.push(queryArrayPromise.promise);
                    queries = context.getQueryInfo();
                    queryNameSelectorReadyDeferred.promise.then(function () {
                        if (queries != undefined) {
                            for (var i = 0; i < queries.length; i++) {
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



                promises.push(loadMappedTables());
                promises.push(loadExcelFileUploader());
                function loadMappedTables() {
                    var promises = [];
                    $scope.scopeModel.isLoadingTabs = true;
                    for (var i = 0; i < mappedTables.length; i++) {
                        var mappedTable = mappedTables[i];
                        var mappedTableItem = {
                            payload: mappedTable,
                            readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                            loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                            VRAutomatedReportQueryId: mappedTable.VRAutomatedReportQueryId,
                            ListName: mappedTable.ListName
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
                function loadExcelFileUploader() {
                    var excelFileUploaderLoadDeferred = UtilsService.createPromiseDeferred();
                    excelFileUploaderReadyDeferred.promise.then(function () {
                        var excelFileGeneratorPayload = {
                            fileUniqueId: (payload != undefined && payload.fileGenerator != undefined) ? payload.fileGenerator.FileUniqueId : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(excelFileUploaderAPI, excelFileGeneratorPayload, excelFileUploaderLoadDeferred);
                    });
                    return excelFileUploaderLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };
            api.reload = function (newQueries) {
                if (queries != undefined && newQueries != undefined) {
                    if (queries.length != newQueries.length) {
                        if (queries.length < newQueries.length) {
                            var queryAdded = newQueries[newQueries.length-1];
                            queries.push(queryAdded);
                            $scope.scopeModel.queries.push({
                                description: queryAdded.QueryTitle,
                                value: queryAdded.VRAutomatedReportQueryId
                            });
                        }
                        else
                        {
                            var queryDeleted;
                            for (var i = 0; i < queries.length; i++) {
                                var oldQuery = queries[i];
                                var oldQueryIndex = UtilsService.getItemIndexByVal(newQueries, oldQuery.VRAutomatedReportQueryId, 'VRAutomatedReportQueryId');
                                if (oldQueryIndex==-1) {
                                    queryDeleted = oldQuery;
                                }
                            }
                            if (queryDeleted !=undefined) {
                                let index = UtilsService.getItemIndexByVal($scope.scopeModel.tables, queryDeleted.VRAutomatedReportQueryId, 'VRAutomatedReportQueryId');
                                while(index > -1) {
                                    $scope.scopeModel.tables.splice(index, 1);
                                    index = UtilsService.getItemIndexByVal($scope.scopeModel.tables, queryDeleted.VRAutomatedReportQueryId, 'VRAutomatedReportQueryId');
                                    tabsAPI.setTabSelected($scope.scopeModel.tables.length -1);
                                }
                                if (context != undefined && context.disableTestGenerateButton != undefined && typeof (context.disableTestGenerateButton) == 'function') {
                                    if ($scope.scopeModel.tables.length == 0) {
                                        context.disableTestGenerateButton(true);
                                    }
                                }
                                if ($scope.scopeModel.tables.length != 0) {
                                    tabsAPI.setLastTabSelected();
                                }

                                var matchingDeletedQueryIndex = UtilsService.getItemIndexByVal($scope.scopeModel.queries, queryDeleted.VRAutomatedReportQueryId, 'value');
                                $scope.scopeModel.queries.splice(matchingDeletedQueryIndex, 1);
                                var matchingDeletedQueryArrayIndex = UtilsService.getItemIndexByVal(queries, queryDeleted.VRAutomatedReportQueryId, 'VRAutomatedReportQueryId');
                                queries.splice(matchingDeletedQueryArrayIndex, 1);
                                if ($scope.scopeModel.queries.length==0) {
                                    $scope.scopeModel.querySelected = undefined;
                                    $scope.scopeModel.listNameSelected = undefined;
                                    $scope.scopeModel.listNames.length = 0;
                                }
                            }
                        }
                    }
                    else {
                        var queryUpdated;
                        for (var i = 0; i < queries.length; i++) {
                            var oldQuery = queries[i];
                            var newQuery = newQueries[i];
                            if (oldQuery.Settings != newQuery.Settings) {
                                queryUpdated = newQuery;
                                break;
                            }
                            }
                            if (queryUpdated !=undefined) {
                                   var matchingQuery = UtilsService.getItemByVal($scope.scopeModel.queries, queryUpdated.VRAutomatedReportQueryId, 'value');
                                   if (matchingQuery != undefined) {
                                       if (matchingQuery.description != queryUpdated.QueryTitle) {
                                           var matchingUpdatedQueryIndex = $scope.scopeModel.queries.indexOf(matchingQuery);
                                           var matchingUpdatedQueryArrayIndex = UtilsService.getItemIndexByVal(queries, matchingQuery.value, 'VRAutomatedReportQueryId');
                                           queries[matchingUpdatedQueryArrayIndex].QueryTitle = queryUpdated.QueryTitle;
                                           $scope.scopeModel.queries[matchingUpdatedQueryIndex].description = queryUpdated.QueryTitle;
                                           if ($scope.scopeModel.queries.length == 1 && $scope.scopeModel.queries[0].value == queryUpdated.VRAutomatedReportQueryId) {
                                               $scope.scopeModel.querySelected.description = queryUpdated.QueryTitle;
                                       }
                                   }
                                    for (var i = 0; i < $scope.scopeModel.tables.length; i++) {
                                        var tab = $scope.scopeModel.tables[i];
                                        if (tab.VRAutomatedReportQueryId==queryUpdated.VRAutomatedReportQueryId) {
                                            let index = $scope.scopeModel.tables.indexOf(tab);
                                            $scope.scopeModel.tables[index].header = queryUpdated.QueryTitle + " - " +tab.ListName + ' (' +tab.tableTabIndex + ')';
                                            if ($scope.scopeModel.tables[index].directiveAPI != undefined && $scope.scopeModel.tables[index].directiveAPI.reload != undefined && typeof ($scope.scopeModel.tables[index].directiveAPI.reload == 'function')) {
                                                $scope.scopeModel.tables[index].directiveAPI.reload(queryUpdated.VRAutomatedReportQueryId);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    queries = newQueries;
                }
            };
            api.getData = function () {
              var obj = {
                   $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators.AdvancedExcelFileGenerator,Vanrise.Analytic.MainExtensions",
                   FileUniqueId: $scope.scopeModel.file != undefined ? $scope.scopeModel.file.fileUniqueId : undefined,
                   TableDefinitions: getTables()
              };
              return obj;

                function getTables() {
                    var mappedTables = [];
                    if ($scope.scopeModel.tables != undefined) {
                        for (var i = 0; i < $scope.scopeModel.tables.length; i++) {
                            var table = $scope.scopeModel.tables[i];
                            var tableDefinition = table.directiveAPI.getData();
                            mappedTables.push(tableDefinition);
                        }
                    }
                    return mappedTables;
                }
            };

            api.doesGeneratorHaveTables = function () {
                return $scope.scopeModel.tables.length==0;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

       
        function addMappedTableTab(mappedTableItem) {
            querySelected = UtilsService.getItemByVal(queries, mappedTableItem.VRAutomatedReportQueryId, "VRAutomatedReportQueryId");
            if (querySelected != undefined) {
                if ($scope.scopeModel.tables.length > 0) {
                    var lastItem = $scope.scopeModel.tables[$scope.scopeModel.tables.length-1];
                    tableIndex = lastItem.tableTabIndex + 1;
                }
                var mappedTableTab = {
                    VRAutomatedReportQueryId: querySelected.VRAutomatedReportQueryId,
                    ListName: mappedTableItem.ListName,
                    header: querySelected.QueryTitle + " - " + mappedTableItem.ListName + ' (' + tableIndex + ')',
                    tableTabIndex: tableIndex,
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
                    VRAutomatedReportQueryId: mappedTableItem.VRAutomatedReportQueryId,
                    ListName: mappedTableItem.ListName
                };
                mappedTableItem.readyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(mappedTableTab.directiveAPI, directivePayload, mappedTableTab.loadPromiseDeferred);
                });
                $scope.scopeModel.tables.push(mappedTableTab);
                if (context != undefined && context.disableTestGenerateButton != undefined && typeof (context.disableTestGenerateButton) == 'function') {
                    if ($scope.scopeModel.tables.length > 0) {
                        context.disableTestGenerateButton(false);
                    }
                }
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

            currentContext.getSelectedQueryId = function () {
                var selectedQuery = querySelected;
                return selectedQuery.VRAutomatedReportQueryId;
            };
            return currentContext;
        }
    }

    return directiveDefinitionObject;
}
]);