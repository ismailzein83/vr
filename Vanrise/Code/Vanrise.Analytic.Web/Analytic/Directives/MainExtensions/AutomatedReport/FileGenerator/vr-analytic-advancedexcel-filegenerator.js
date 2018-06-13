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

        var mappedTables = [];
        var tableIndex = 0;

        var queries;

        var querySelected;
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
                        var listNameSelectorReadyDeferred = context.getQueryListNames(query.value);
                        listNameSelectorReadyDeferred.then(function (listNames) {
                            if (listNames != undefined) {
                                for (var i = 0; i < listNames.length ; i++) {
                                    $scope.scopeModel.listNames.push({
                                        description: listNames[i],
                                        value: listNames[i]
                                    });
                                }
                            }
                            listNameSelectorAPI.selectIfSingleItem();

                        });
                    }
                };
                excelWorkbookReadyDeferred.promise.then(function () {
                    defineAPI();
                });
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    {
                        var tableDefinitions = payload.fileGenerator != undefined ? payload.fileGenerator.TableDefinitions : undefined;
                        var fileId = payload.fileGenerator != undefined ? payload.fileGenerator.FileTemplateId : undefined;
                        context = payload.context;
                    }
                    if (context != undefined) {
                        queries = context.getQueryInfo();
                        queryNameSelectorReadyDeferred.promise.then(function () {
                            if (queries != undefined) {
                                for (var i = 0; i < queries.length ; i++) {
                                    $scope.scopeModel.queries.push({
                                        description: queries[i].QueryTitle,
                                        value: queries[i].VRAutomatedReportQueryId
                                    });
                                }
                                queryNameSelectorAPI.selectIfSingleItem();

                            }
                        });
                    }
                    if (tableDefinitions != undefined) {
                        mappedTables = tableDefinitions;
                    }
                    if (mappedTables != undefined && mappedTables.length != 0) {
                        var mappedTable = mappedTables[0];
                        listNameSelected = mappedTable.ListName;
                        querySelected = UtilsService.getItemByVal($scope.scopeModel.queries, mappedTable.VRAutomatedReportQueryId, "value");
                    }
                    if (fileId != undefined)
                        $scope.scopeModel.file = { fileId: fileId };

                    promises.push(loadMappedTables());
                }
                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
              var obj = {
                   $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators.AdvancedExcelFileGenerator,Vanrise.Analytic.MainExtensions",
                   FileTemplateId:$scope.scopeModel.file!=undefined ? $scope.scopeModel.file.fileId : undefined,
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

       
        function loadMappedTables() {
            var promises = [];

            for (var i = 0 ; i < mappedTables.length; i++) {
                var mappedTableItem = {
                    payload: mappedTables[i],
                    readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                    loadPromiseDeferred: UtilsService.createPromiseDeferred()
                };
                promises.push(mappedTableItem.loadPromiseDeferred.promise);

                addMappedTableTab(mappedTableItem);
            }
            return UtilsService.waitMultiplePromises(promises);
        }

        function addMappedTableTab(mappedTableItem) {
            var directiveLoadDeferred = UtilsService.createPromiseDeferred();
            var mappedTableTab = {
                header: querySelected.description + " - "+listNameSelected,
                tableTabIndex: ++tableIndex,
                Editor: "vr-analytic-advancedexcel-filegenerator-mappedtable",
                onDirectiveReady: function (api) {
                    mappedTableTab.directiveAPI = api;
                    mappedTableItem.readyPromiseDeferred.resolve();
                }
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


        function getContext() {
            var currentContext = context;
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