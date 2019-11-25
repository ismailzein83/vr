(function (app) {

    'use strict';

    EricssonTrunkTrunkGroup.$inject = ["UtilsService", 'VRUIUtilsService'];

    function EricssonTrunkTrunkGroup(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new EricssonTrunkTrunkGroupCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/Templates/EricssonTrunkTrunkGroupTemplate.html"
        };

        function EricssonTrunkTrunkGroupCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var drillDownManager;
            var supplierOutTrunksMappings;

            var trunkTrunkGroupGridAPI;
            var trunkTrunkGroupGridReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.trunksInfo = [];
                $scope.scopeModel.selectedTrunksInfo = [];
                $scope.scopeModel.trunkTrunkGroups = [];

                $scope.scopeModel.onTrunkTrunkGroupGridReady = function (api) {
                    trunkTrunkGroupGridAPI = api;
                    //drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(buildTrunkTrunkGroupBackupDrillDownTabs(), trunkTrunkGroupGridAPI);
                    trunkTrunkGroupGridReadyDeferred.resolve();
                };

                $scope.scopeModel.onSelectTrunk = function (addedTrunk) {

                    var trunkTrunkGroup = {
                        TrunkId: addedTrunk.value,
                        TrunkTrunkGroupNb: $scope.scopeModel.trunkTrunkGroups.length + 1
                    };

                    extendTrunkTrunkGroup(trunkTrunkGroup, addedTrunk);
                    //drillDownManager.setDrillDownExtensionObject(trunkTrunkGroup);

                    $scope.scopeModel.trunkTrunkGroups.push(trunkTrunkGroup);
                };

                $scope.scopeModel.onDeselectTrunk = function (deletedTrunk) {

                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.trunkTrunkGroups, deletedTrunk.value, 'TrunkId');
                    $scope.scopeModel.trunkTrunkGroups.splice(index, 1);
                };

                $scope.scopeModel.onDeleteTrunkTrunkGroup = function (deletedTrunk) {

                    var selectorIndex = UtilsService.getItemIndexByVal($scope.scopeModel.selectedTrunksInfo, deletedTrunk.TrunkId, 'value');
                    $scope.scopeModel.selectedTrunksInfo.splice(selectorIndex, 1);

                    var gridIndex = UtilsService.getItemIndexByVal($scope.scopeModel.trunkTrunkGroups, deletedTrunk.TrunkId, 'TrunkId');
                    $scope.scopeModel.trunkTrunkGroups.splice(gridIndex, 1);
                };

                $scope.scopeModel.isTrunkTrunkGroupsValid = function () {
                    var trunkTrunkGroups = $scope.scopeModel.trunkTrunkGroups;
                    if (trunkTrunkGroups.length == 0)
                        return null;

                    var isPercentageExists = false;
                    var sumOfPercentage = 0;

                    for (var i = 0; i < trunkTrunkGroups.length; i++) {
                        var currentTrunkTrunkGroup = trunkTrunkGroups[i];
                        if (currentTrunkTrunkGroup.Percentage != undefined) {
                            isPercentageExists = true;
                            sumOfPercentage += parseInt(currentTrunkTrunkGroup.Percentage);
                        }
                    }

                    //if ($scope.scopeModel.loadSharing && !isPercentageExists)
                    //    return 'At least one trunk must have percentage';

                    if (isPercentageExists && sumOfPercentage != 100) {
                        return "Percentages summation should be 100";
                    }
                    return null;
                };

                $scope.scopeModel.isTrunkExapandable = function (trunkGroup) {
                    return trunkGroup.Percentage != undefined && trunkGroup.Percentage != '';
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var trunks;
                    var trunkTrunkGroups;

                    if (payload != undefined) {
                        trunks = payload.trunks;
                        trunkTrunkGroups = payload.trunkTrunkGroups;
                        supplierOutTrunksMappings = payload.supplierOutTrunksMappings;

                        //$scope.scopeModel.loadSharing = payload.LoadSharing;
                    }

                    loadTrunkSelector();

                    if (trunkTrunkGroups != undefined && trunkTrunkGroups.length > 0) {
                        var loadTrunkTrunkGroupGridPromise = loadTrunkTrunkGroupGrid();
                        promises.push(loadTrunkTrunkGroupGridPromise);
                    }

                    function loadTrunkSelector() {
                        if (trunks != undefined) {
                            for (var index = 0; index < trunks.length; index++) {
                                var currentTrunk = trunks[index];
                                $scope.scopeModel.trunksInfo.push({
                                    value: currentTrunk.TrunkId,
                                    description: currentTrunk.TrunkName
                                });
                            }

                            if (trunkTrunkGroups != undefined) {
                                var trunkIds = UtilsService.getPropValuesFromArray(trunkTrunkGroups, "TrunkId");
                                for (var i = 0; i < trunkIds.length; i++) {
                                    var selectedValue = UtilsService.getItemByVal($scope.scopeModel.trunksInfo, trunkIds[i], "value");
                                    if (selectedValue != null) {
                                        $scope.scopeModel.selectedTrunksInfo.push(selectedValue);
                                    }
                                }
                            }
                        }
                    }
                    function loadTrunkTrunkGroupGrid() {
                        var loadTrunkTrunkGroupGridDeferred = UtilsService.createPromiseDeferred();

                        trunkTrunkGroupGridReadyDeferred.promise.then(function () {
                            for (var i = 0; i < trunkTrunkGroups.length; i++) {
                                var currentTrunkTrunkGroup = trunkTrunkGroups[i];
                                currentTrunkTrunkGroup.TrunkTrunkGroupNb = i + 1;

                                var currentTrunkInfo = UtilsService.getItemByVal($scope.scopeModel.trunksInfo, currentTrunkTrunkGroup.TrunkId, "value");
                                if (currentTrunkInfo == undefined)
                                    continue;

                                extendTrunkTrunkGroup(currentTrunkTrunkGroup, currentTrunkInfo);
                                //drillDownManager.setDrillDownExtensionObject(currentTrunkTrunkGroup);

                                $scope.scopeModel.trunkTrunkGroups.push(currentTrunkTrunkGroup);
                            }

                            loadTrunkTrunkGroupGridDeferred.resolve();
                        });

                        return loadTrunkTrunkGroupGridDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var trunkTrunkGroups = [];
                    for (var i = 0; i < $scope.scopeModel.trunkTrunkGroups.length; i++) {
                        var currentTrunkTrunkGroup = $scope.scopeModel.trunkTrunkGroups[i];

                        //var currentTrunkTrunkGroupBackups;
                        //if (currentTrunkTrunkGroup.trunkTrunkGroupBackupDirectiveAPI != undefined) {
                        //    currentTrunkTrunkGroupBackups = currentTrunkTrunkGroup.trunkTrunkGroupBackupDirectiveAPI.getData();
                        //} else {
                        //    var loadedTrunkTrunkGroup = UtilsService.getItemByVal($scope.scopeModel.trunkTrunkGroups, currentTrunkTrunkGroup.TrunkTrunkGroupNb, 'TrunkTrunkGroupNb');
                        //    currentTrunkTrunkGroupBackups = loadedTrunkTrunkGroup.Backups;
                        //}

                        trunkTrunkGroups.push({
                            TrunkId: currentTrunkTrunkGroup.TrunkId,
                            Percentage: currentTrunkTrunkGroup.Percentage,
                            //Backups: currentTrunkTrunkGroup.Percentage != undefined ? currentTrunkTrunkGroupBackups : undefined
                        });
                    }

                    return {
                        TrunkTrunkGroups: trunkTrunkGroups,
                        //LoadSharing: $scope.scopeModel.loadSharing
                    };
                };

                api.getTrunksInfo = function () {
                    return $scope.scopeModel.trunksInfo;
                };

                api.deleteTrunkTrunkGroup = function (trunk) {

                    var selectedTrunkInfoIndex = UtilsService.getItemIndexByVal($scope.scopeModel.selectedTrunksInfo, trunk.TrunkId, "value");
                    if (selectedTrunkInfoIndex >= 0) {
                        $scope.scopeModel.selectedTrunksInfo.splice(selectedTrunkInfoIndex, 1);
                    }

                    var trunksInfoIndex = UtilsService.getItemIndexByVal($scope.scopeModel.trunksInfo, trunk.TrunkId, "value");
                    $scope.scopeModel.trunksInfo.splice(trunksInfoIndex, 1);

                    var trunkTrunkGroupsIndex = UtilsService.getItemIndexByVal($scope.scopeModel.trunkTrunkGroups, trunk.TrunkId, "TrunkId");
                    if (trunkTrunkGroupsIndex >= 0) {
                        $scope.scopeModel.trunkTrunkGroups.splice(trunkTrunkGroupsIndex, 1);
                    }
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function extendTrunkTrunkGroup(trunkTrunkGroup, trunkInfo) {
                trunkTrunkGroup.TrunkName = trunkInfo.description;

                trunkTrunkGroup.onPercentageChanged = function (value) {
                    if (value == undefined) {
                        trunkTrunkGroupGridAPI.collapseRow(trunkTrunkGroup);
                    } else {
                        trunkTrunkGroupGridAPI.expandRow(trunkTrunkGroup);
                    }
                };
            }

            function buildTrunkTrunkGroupBackupDrillDownTabs() {
                var drillDownTabs = [];
                drillDownTabs.push(buildTrunkTrunkGroupBackupDrillDownTab());

                function buildTrunkTrunkGroupBackupDrillDownTab() {
                    var drillDownTab = {};
                    drillDownTab.title = "Backup";
                    drillDownTab.directive = "whs-routesync-ericsson-trunktrunkgroupbackup";

                    drillDownTab.loadDirective = function (trunkTrunkGroupBackupDirectiveAPI, trunkTrunkGroup) {
                        trunkTrunkGroup.trunkTrunkGroupBackupDirectiveAPI = trunkTrunkGroupBackupDirectiveAPI;
                        return trunkTrunkGroup.trunkTrunkGroupBackupDirectiveAPI.load(buildTrunkTrunkGroupPayload(trunkTrunkGroup));
                    };

                    function buildTrunkTrunkGroupPayload(trunkTrunkGroup) {
                        var trunkTrunkGroupBackupPayload = {};
                        trunkTrunkGroupBackupPayload.Backups = trunkTrunkGroup.Backups;
                        trunkTrunkGroupBackupPayload.supplierOutTrunksMappings = supplierOutTrunksMappings;
                        return trunkTrunkGroupBackupPayload;
                    }

                    return drillDownTab;
                }

                return drillDownTabs;
            }
        }
    }

    app.directive('whsRoutesyncEricssonTrunktrunkgroup', EricssonTrunkTrunkGroup);
})(app);