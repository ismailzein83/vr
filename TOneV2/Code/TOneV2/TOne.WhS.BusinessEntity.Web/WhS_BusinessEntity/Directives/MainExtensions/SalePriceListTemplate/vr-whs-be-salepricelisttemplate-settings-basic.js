﻿'use strict';

app.directive('vrWhsBeSalepricelisttemplateSettingsBasic', ['UtilsService', 'VRUIUtilsService', 'WhS_BE_SalePriceListTemplateAPIService', function (UtilsService, VRUIUtilsService, WhS_BE_SalePriceListTemplateAPIService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var basicSalePriceListTemplateSettings = new BasicSalePriceListTemplateSettings($scope, ctrl, $attrs);
            basicSalePriceListTemplateSettings.initializeController();
        },
        controllerAs: "basicSettingsCtrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/SalePriceListTemplate/Templates/BasicSalePriceListTemplateSettingsTemplate.html'
    };

    function BasicSalePriceListTemplateSettings($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var tabsAPI;
        var excelWorkbookAPI;
        var excelWorkbookReadyDeferred = UtilsService.createPromiseDeferred();

        var mappedTableDirectiveAPI;
        var mappedTableDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var mappedCellsAPI;
        var mappedCellsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var mappedTables = [];
        var mappedCells = [];
        var mappedTablesSelective;

        var tableIndex = 0;

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.mappedCols = [];
            $scope.scopeModel.tables = [];
            $scope.scopeModel.dateTimeFormat = 'd'; // Short date pattern

            $scope.scopeModel.onExcelWorkbookReady = function (api) {
                excelWorkbookAPI = api;
                excelWorkbookReadyDeferred.resolve();
            };


            $scope.scopeModel.onMappedTableDirectiveReady = function (api) {
                mappedTableDirectiveAPI = api;
                mappedTableDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.addMappedTable = function () {
                var mappedTableItem = {
                    readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                    loadPromiseDeferred: UtilsService.createPromiseDeferred()
                };
                addMappedTableTab(mappedTableItem);
                mappedTableDirectiveAPI.clearSelectedValue();
                tabsAPI.setLastTabSelected();

                
            };

            $scope.scopeModel.disableAddMappedTable = function () {
                if (mappedTableDirectiveAPI == undefined)
                    return true;
                return mappedTableDirectiveAPI.getData() == undefined;
            };

            $scope.scopeModel.onTabsReady = function (api) {
                tabsAPI = api;
            };

            $scope.scopeModel.onMappedCellsReady = function (api) {
                mappedCellsAPI = api;
                mappedCellsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.removeTable = function (obj) {
                var index = $scope.scopeModel.tables.indexOf(obj);
                $scope.scopeModel.tables.splice(index, 1);
            };

            UtilsService.waitMultiplePromises([excelWorkbookReadyDeferred.promise, mappedTableDirectiveReadyPromiseDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {

                var promises = [];
                var settings;

                if (payload != undefined) {
                    settings = payload.settings;
                }


                if (settings != undefined) {

                    $scope.scopeModel.file = { fileId: settings.TemplateFileId };
                    $scope.scopeModel.dateTimeFormat = settings.DateTimeFormat;

                    if (settings.MappedTables != null && settings.MappedTables.length > 0) {
                        mappedTables = settings.MappedTables;
                       // tableIndex = mappedTables.length -1 ;
                    }
                    if (settings.MappedCells != null && settings.MappedCells.length > 0) {
                        mappedCells = settings.MappedCells;
                    }
                }

                var promiseDeffered = UtilsService.createPromiseDeferred();
                //promises.push(promiseDeffered.promise);


                WhS_BE_SalePriceListTemplateAPIService.GetMappedTablesExtensionConfigs().then(function (response) {
                    mappedTablesSelective = response;
                    loadMappedTables().finally(function () {
                        promiseDeffered.resolve();
                    }).catch(function (error) {
                        promiseDeffered.reject(error);
                    });
                }).catch(function (error) {
                    promiseDeffered.reject(error);
                });

                var loadMappedTableDirectivePromise = loadMappedTableDirective();
                promises.push(loadMappedTableDirectivePromise);

                var loadMappedCellsPromise = loadMappedCells();
                promises.push(loadMappedCellsPromise);

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function getData() {

                var data = {
                    $type: 'TOne.WhS.BusinessEntity.MainExtensions.BasicSalePriceListTemplateSettings, TOne.WhS.BusinessEntity.MainExtensions',
                    TemplateFileId: $scope.scopeModel.file.fileId,
                    DateTimeFormat: $scope.scopeModel.dateTimeFormat
                };

                var mappedTables = [];
                if ($scope.scopeModel.tables != undefined) {
                    for (var i = 0; i < $scope.scopeModel.tables.length; i++) {
                        var table = $scope.scopeModel.tables[i];
                        mappedTables.push(table.directiveAPI.getData());
                    }

                }

                data.MappedTables = mappedTables;

                data.mappedCells = mappedCellsAPI.getData();

                return data;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadMappedCells()
        {
            var mappedCellsLoadDeferred = UtilsService.createPromiseDeferred();
            var mappedCellsPayload = {
                mappedCells: mappedCells,
                context: getContext(),
            };
            mappedCellsReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(mappedCellsAPI, mappedCellsPayload, mappedCellsLoadDeferred);
            });

            return mappedCellsLoadDeferred.promise;
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
                header: "Table " + parseInt(tableIndex + 1 ),
                tableTabIndex: ++tableIndex,
                Editor: mappedTableItem.payload != undefined ? getEditorByConfigId(mappedTableItem.payload.ConfigId) : getEditorByConfigId(mappedTableDirectiveAPI.getData().ExtensionConfigurationId),
                onDirectiveReady: function (api) {
                    mappedTableTab.directiveAPI = api;
                    mappedTableItem.readyPromiseDeferred.resolve();
                }
            };
            var directivePayload = {
                context: getContext(),
                mappedTable: mappedTableItem.payload,
                showEditButton : false,
                priceListType: mappedTableItem.payload != undefined ? getConfigTypeByConfigId(mappedTableItem.payload.ConfigId) : getConfigTypeByConfigId(mappedTableDirectiveAPI.getData().ExtensionConfigurationId)
            };

            mappedTableItem.readyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(mappedTableTab.directiveAPI, directivePayload, mappedTableTab.loadPromiseDeferred);
            });

            $scope.scopeModel.tables.push(mappedTableTab);
        }


        function loadMappedTableDirective() {

            var mappedTableDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            mappedTableDirectiveReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(mappedTableDirectiveAPI, undefined, mappedTableDirectiveLoadDeferred);
            });

            return mappedTableDirectiveLoadDeferred.promise;
        }

        function getEditorByConfigId(configId) {
            for (var i = 0 ; i < mappedTablesSelective.length; i++) {
                var currentMappedTableSelective = mappedTablesSelective[i];
                if (currentMappedTableSelective.ExtensionConfigurationId == configId)
                    return currentMappedTableSelective.Editor;
            }
        }

        function getConfigTypeByConfigId(configId) {
            for (var i = 0 ; i < mappedTablesSelective.length; i++) {
                var currentMappedTableSelective = mappedTablesSelective[i];
                if (currentMappedTableSelective.ExtensionConfigurationId == configId)
                    return currentMappedTableSelective.PriceListType;
            }
        }
        

        function getContext() {
            var context = {
                getSelectedSheetApi: function () {
                    return excelWorkbookAPI.getSelectedSheetApi();
                },
                selectCellAtSheet: function (rowIndex, columnIndex, sheetIndex) {
                    return excelWorkbookAPI.selectCellAtSheet(rowIndex, columnIndex, sheetIndex);
                },
                getSelectedSheet: function () {
                    return excelWorkbookAPI.getSelectedSheet();
                }
            };
            return context;
        }

    }
}]);