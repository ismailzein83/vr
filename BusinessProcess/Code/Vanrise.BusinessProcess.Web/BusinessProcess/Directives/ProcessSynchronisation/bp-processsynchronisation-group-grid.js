(function (app) {

    'use strict';

    businessprocessProcesssynchronisationgroupGrid.$inject = ['UtilsService', 'VRUIUtilsService'];

    function businessprocessProcesssynchronisationgroupGrid(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ProcessSynchronisationGroupDirective($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/BusinessProcess/Directives/ProcessSynchronisation/Templates/ProcessSynchronisationGroupGridTemplate.html'
        };

        function ProcessSynchronisationGroupDirective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var shouldAddBPDefinition;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.scopeModel.onAddProcess = function () {
                    var dataItem = {
                        Id: UtilsService.guid(),
                        onBPDefinitionSelectorReady: function (api) {
                            dataItem.bpDefinitionSelectorAPI = api;
                            var setLoader = function (value) { dataItem.isBPDefinitionLoading = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.bpDefinitionSelectorAPI, undefined, setLoader);
                        },
                        onBPTaskSelectorReady: function (api) {
                            dataItem.bpTaskSelectorAPI = api;
                        },
                        onSelectionChanged: function (value) {
                            if (value != undefined) {
                                var selectedBPDefinitionId = value.BPDefinitionID;
                                var dataItemBPTaskSelectorPayload = { filter: buildFilter(selectedBPDefinitionId) };

                                var setLoader = function (value) { dataItem.isBPTaskSelectorLoading = value; };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.bpTaskSelectorAPI, dataItemBPTaskSelectorPayload, setLoader);
                            }
                        }
                    };

                    $scope.scopeModel.datasource.push(dataItem);
                };

                $scope.scopeModel.isValid = function () {
                    if (shouldAddBPDefinition && $scope.scopeModel.datasource.length == 0)
                        return 'At least one BP Definition should be added';
                    return null;
                };

                $scope.scopeModel.removeRow = function (dataItem) {
                    var index = $scope.scopeModel.datasource.indexOf(dataItem);
                    $scope.scopeModel.datasource.splice(index, 1);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined)
                        shouldAddBPDefinition = payload.shouldAddBPDefinition;

                    if (payload != undefined && payload.processSynchronisationItems != undefined) {
                        for (var i = 0; i < payload.processSynchronisationItems.length; i++) {
                            var processSynchronisationItem = payload.processSynchronisationItems[i];

                            var gridItem = {
                                processSynchronisationItem: processSynchronisationItem,
                                readyBPDefinitionSelectorPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadBPDefinitionSelectorPromiseDeferred: UtilsService.createPromiseDeferred(),
                                onBPDefinitionSelectionChangedDeferred: UtilsService.createPromiseDeferred(),
                                readyBPTaskSelectorPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadBPTaskSelectorPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            promises.push(gridItem.loadBPDefinitionSelectorPromiseDeferred.promise);
                            promises.push(gridItem.loadBPTaskSelectorPromiseDeferred.promise);

                            addItemToGrid(gridItem);
                        }
                    }

                    function addItemToGrid(gridItem) {
                        if (gridItem.processSynchronisationItem == undefined)
                            return;

                        var dataItem = {
                            Id: UtilsService.guid(),
                            onBPDefinitionSelectorReady: function (api) {
                                dataItem.bpDefinitionSelectorAPI = api;
                                gridItem.readyBPDefinitionSelectorPromiseDeferred.resolve();
                            },
                            onBPTaskSelectorReady: function (api) {
                                dataItem.bpTaskSelectorAPI = api;
                                gridItem.readyBPTaskSelectorPromiseDeferred.resolve();
                            },
                            onSelectionChanged: function (value) {
                                if (value != undefined) {
                                    if (gridItem.onBPDefinitionSelectionChangedDeferred != undefined) {
                                        gridItem.onBPDefinitionSelectionChangedDeferred.resolve();
                                    }
                                    else {
                                        var selectedBPDefinitionId = value.BPDefinitionID;
                                        var dataItemBPTaskSelectorPayload = { filter: buildFilter(selectedBPDefinitionId) };

                                        var setLoader = function (value) { $scope.scopeModel.isBPTaskSelectorLoading = value; };
                                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.bpTaskSelectorAPI, dataItemBPTaskSelectorPayload, setLoader);
                                    }
                                }
                            }
                        };

                        gridItem.readyBPDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var dataItemBPDefinitionPayload = {
                                selectedIds: gridItem.processSynchronisationItem.BPDefinitionId
                            };
                            VRUIUtilsService.callDirectiveLoad(dataItem.bpDefinitionSelectorAPI, dataItemBPDefinitionPayload, gridItem.loadBPDefinitionSelectorPromiseDeferred);
                        });

                        gridItem.onBPDefinitionSelectionChangedDeferred.promise.then(function () {
                            gridItem.onBPDefinitionSelectionChangedDeferred = undefined;

                            gridItem.readyBPTaskSelectorPromiseDeferred.promise.then(function () {
                                var dataItemBPTaskSelectorPayload = { filter: buildFilter(gridItem.processSynchronisationItem.BPDefinitionId) };
                                if (gridItem.processSynchronisationItem.TaskIds != undefined) {
                                    dataItemBPTaskSelectorPayload.selectedIds = gridItem.processSynchronisationItem.TaskIds;
                                }

                                VRUIUtilsService.callDirectiveLoad(dataItem.bpTaskSelectorAPI, dataItemBPTaskSelectorPayload, gridItem.loadBPTaskSelectorPromiseDeferred);
                            });
                        });

                        $scope.scopeModel.datasource.push(dataItem);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var processes = [];
                    if ($scope.scopeModel.datasource != undefined && $scope.scopeModel.datasource.length > 0) {
                        angular.forEach($scope.scopeModel.datasource, function (item) {
                            if (item.bpDefinitionSelectorAPI != undefined && item.bpTaskSelectorAPI != undefined) {
                                var dataItem = {
                                    BPDefinitionId: item.bpDefinitionSelectorAPI.getSelectedIds(),
                                    TaskIds: item.bpTaskSelectorAPI.getSelectedIds()
                                };
                                processes.push(dataItem);
                            }
                        });
                    }
                    return processes;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };

            function buildFilter(bpDefinitionId) {
                return {
                    Filters: [{
                        $type: "Vanrise.BusinessProcess.MainExtensions.SchedulerTask.BPDefinitionSchedulerTaskFilter,Vanrise.BusinessProcess.MainExtensions",
                        BPDefinitionId: bpDefinitionId
                    }]
                };
            }
        }
    }

    app.directive('businessprocessProcesssynchronisationGroupGrid', businessprocessProcesssynchronisationgroupGrid);

})(app);