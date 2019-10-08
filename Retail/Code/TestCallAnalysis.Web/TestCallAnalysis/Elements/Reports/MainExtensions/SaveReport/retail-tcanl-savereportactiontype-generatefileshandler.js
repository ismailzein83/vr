"use strict";
app.directive("retailTcanlSavereportactiontypeGeneratefileshandler", ["UtilsService", "VRUIUtilsService", "Retail_TCAnal_ReportTypeEnum", "Retail_BE_AccountBEAPIService",
    function (UtilsService, VRUIUtilsService, Retail_TCAnal_ReportTypeEnum, Retail_BE_AccountBEAPIService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var saveReportHandler = new SaveReportHandler($scope, ctrl, $attrs);
                saveReportHandler.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/TestCallAnalysis/Elements/Reports/MainExtensions/SaveReport/Templates/SaveReportActionTypeGenerateFilesTemplate.html"
        };


        function SaveReportHandler($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var reportTypeSelectorAPI;
            var reportTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var operatorSelectorAPI;
            var operatorSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var queryNameSelectorAPI;
            var queryNameSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var listNameSelectorAPI;
            var listNameSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var queryFieldsSelectorAPI;
            var queryFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var context;
            var actionType;

            function initializeController() {
                var promises = [reportTypeSelectorReadyPromiseDeferred.promise, operatorSelectorReadyPromiseDeferred.promise, queryNameSelectorReadyDeferred.promise, listNameSelectorReadyDeferred.promise, queryFieldsSelectorReadyDeferred.promise];

                $scope.scopeModel = {};

                $scope.scopeModel.reportTypes = [];
                $scope.scopeModel.operators = [];
                $scope.scopeModel.queries = [];
                $scope.scopeModel.listNames = [];
                $scope.scopeModel.fields = [];

                $scope.scopeModel.onReportTypeSelectorReady = function (api) {
                    reportTypeSelectorAPI = api;
                    reportTypeSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onOperatorSelectorReady = function (api) {
                    operatorSelectorAPI = api;
                    operatorSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onQueryNameSelectorReady = function (api) {
                    queryNameSelectorAPI = api;
                    queryNameSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onListNameSelectorReady = function (api) {
                    listNameSelectorAPI = api;
                    listNameSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onQueryFieldsSelectorReady = function (api) {
                    queryFieldsSelectorAPI = api;
                    queryFieldsSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onQuerySelectionChanged = function (query) {
                    if (query == undefined) {
                        listNameSelectorAPI.clearDataSource();
                        queryFieldsSelectorAPI.clearDataSource();
                    }
                    else {
                        $scope.scopeModel.selectedListName = undefined;
                        $scope.scopeModel.isLoadingListNames = true;
                        loadListNames(query.value).then(function () {
                            if (actionType != undefined && actionType.ListName != undefined) {
                                $scope.scopeModel.selectedListName = UtilsService.getItemByVal($scope.scopeModel.listNames, actionType.ListName, 'value');
                                actionType.ListName = undefined;
                            }
                            else
                                listNameSelectorAPI.selectIfSingleItem();
                        }).finally(function () {
                            $scope.scopeModel.isLoadingListNames = false;
                        });

                        $scope.scopeModel.selectedQueryField = undefined;
                        $scope.scopeModel.isLoadingQueryFields = true;
                        loadQueryFields(query.value).then(function () {
                            if (actionType != undefined && actionType.RecordId != undefined) {
                                $scope.scopeModel.selectedQueryField = UtilsService.getItemByVal($scope.scopeModel.fields, actionType.RecordId, 'value');
                                actionType.RecordId = undefined;
                            }
                            else
                                queryFieldsSelectorAPI.selectIfSingleItem();

                        }).finally(function () {
                            $scope.scopeModel.isLoadingQueryFields = false;
                        });
                    }
                };

                UtilsService.waitMultiplePromises(promises).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        context = payload.context;
                        actionType = payload.actionType;
                    }

                    $scope.scopeModel.reportTypes = UtilsService.getArrayEnum(Retail_TCAnal_ReportTypeEnum);

                    initialPromises.push(loadQueries());
                    initialPromises.push(loadOperatorSelector());
                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];

                            if (actionType != undefined) {
                                $scope.scopeModel.selectedReportType = UtilsService.getItemByVal($scope.scopeModel.reportTypes, actionType.ReportType, 'value');
                                $scope.scopeModel.selectedQuery = UtilsService.getItemByVal($scope.scopeModel.queries, payload.actionType.ReportQueryId, 'value');
                            }
                            else
                                queryNameSelectorAPI.selectIfSingleItem();

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "TestCallAnalysis.Business.SaveReportActionType, TestCallAnalysis.Business",
                        ReportType: $scope.scopeModel.selectedReportType.value,
                        OperatorId: $scope.scopeModel.selectedOperator.AccountId,
                        ReportQueryId: $scope.scopeModel.selectedQuery.value,
                        ListName: $scope.scopeModel.selectedListName.value,
                        RecordId: $scope.scopeModel.selectedQueryField.value
                    };
                };

                api.reload = function () {
                    loadQueries().then(function () {
                        if ($scope.scopeModel.selectedQuery != undefined) {
                            var queryIndex = UtilsService.getItemIndexByVal($scope.scopeModel.queries, $scope.scopeModel.selectedQuery.value, 'value');
                            if (queryIndex == -1) {
                                $scope.scopeModel.selectedQuery = undefined;
                                $scope.scopeModel.selectedListName = undefined;
                                $scope.scopeModel.listNames.length = 0;
                                $scope.scopeModel.selectedQueryField = undefined;
                                $scope.scopeModel.fields.length = 0;
                            }
                            else {
                                loadListNames($scope.scopeModel.selectedQuery.value).then(function () {
                                    if ($scope.scopeModel.selectedListName != undefined) {
                                        var listNameIndex = UtilsService.getItemIndexByVal($scope.scopeModel.listNames, $scope.scopeModel.selectedListName.value, 'value');
                                        if (listNameIndex == -1) {
                                            $scope.scopeModel.selectedListName = undefined;
                                        }
                                    }

                                });
                                loadQueryFields($scope.scopeModel.selectedQuery.value).then(function () {
                                    if ($scope.scopeModel.selectedQueryField != undefined) {
                                        var fieldIndex = UtilsService.getItemIndexByVal($scope.scopeModel.fields, $scope.scopeModel.selectedQueryField.value, 'value');
                                        if (fieldIndex == -1) {
                                            $scope.scopeModel.selectedQueryField = undefined;
                                        }
                                    }
                                });
                            }
                        }
                    });
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadQueries() {
                var queryLoadPromise = UtilsService.createPromiseDeferred();

                queryNameSelectorReadyDeferred.promise.then(function () {
                    var queries = context.getQueryInfo();
                    $scope.scopeModel.queries.length = 0;
                    if (queries != undefined) {
                        for (var i = 0; i < queries.length; i++) {
                            var query = queries[i];
                            $scope.scopeModel.queries.push({
                                description: query.QueryTitle,
                                value: query.VRAutomatedReportQueryId
                            });
                        }
                    }
                    queryLoadPromise.resolve();
                });
                return queryLoadPromise.promise;
            }

            function loadOperatorSelector() {
                var loadOperatorSelectorPromise = UtilsService.createPromiseDeferred();

                operatorSelectorReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        businessEntityDefinitionId: 'd4028716-97aa-4664-8eaa-35b99603b2e7'
                    };

                    if (actionType != undefined)
                        payload.selectedIds = actionType.OperatorId;

                    VRUIUtilsService.callDirectiveLoad(operatorSelectorAPI, payload, loadOperatorSelectorPromise);
                });

                return loadOperatorSelectorPromise.promise;
            }

            function loadListNames(selectedQueryId) {
                var listNamesLoadDataPromise = context.getQueryListNames(selectedQueryId);
                var listNamesLoadPromise = UtilsService.createPromiseDeferred();

                listNamesLoadDataPromise.then(function (listNames) {
                    $scope.scopeModel.listNames.length = 0;
                    if (listNames != undefined) {
                        for (var i = 0; i < listNames.length; i++) {
                            var listName = listNames[i];
                            $scope.scopeModel.listNames.push({
                                description: listName,
                                value: listName
                            });
                        }
                    }
                    listNamesLoadPromise.resolve();
                });
                return listNamesLoadPromise.promise;
            }

            function loadQueryFields(selectedQueryId) {
                var fieldsLoadDataPromise = context.getQueryFields(selectedQueryId);
                var fieldsLoadPromise = UtilsService.createPromiseDeferred();

                fieldsLoadDataPromise.then(function (fields) {
                    $scope.scopeModel.fields.length = 0;

                    if (fields != undefined) {
                        for (var i = 0; i < fields.length; i++) {
                            var field = fields[i];
                            $scope.scopeModel.fields.push({
                                description: field.FieldTitle,
                                value: field.FieldName,
                                source: field.Source
                            });
                        }
                    }
                    fieldsLoadPromise.resolve();
                });
                return fieldsLoadPromise.promise;
            }
        }

        return directiveDefinitionObject;
    }
]);