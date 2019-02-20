(function (app) {

    'use strict';

    recordAnalysisSwitchinterconnectionSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function recordAnalysisSwitchinterconnectionSettingsDirective(UtilsService, VRUIUtilsService, VRNotificationService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RecordAnalysisSwitchinterconnectionSettings(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/RecordAnalysis/Directives/C4SwitchInterconnection/Templates/C4SwitchInterconnectionSettingsTemplate.html"
        };

        function RecordAnalysisSwitchinterconnectionSettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var trunkGridAPI;
            var trunkGridReadyDeferred = UtilsService.createPromiseDeferred();

            var trunkGroupGridAPI;
            var trunkGroupGridReadyDeferred = UtilsService.createPromiseDeferred();

            var firstLoad;
            var rowIndex = 0;

            var notSelectedTrunks = [];

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.trunks = [];
                $scope.scopeModel.trunkGroups = [];
                $scope.scopeModel.trunksInfo = [];
                $scope.scopeModel.isTrunkGroupGridLoading = false;
                firstLoad = true;

                $scope.scopeModel.onTrunkGridReady = function (api) {
                    trunkGridAPI = api;
                    trunkGridReadyDeferred.resolve();
                };

                $scope.scopeModel.onTrunkAdded = function () {
                    var trunk = {
                        TrunkId: UtilsService.guid(),
                        TrunkName: undefined
                    };
                    notSelectedTrunks.push(trunk);
                    $scope.scopeModel.trunks.push(trunk);
                };

                $scope.scopeModel.onBeforeTrunkDeleted = function (deletedItem) {
                    return VRNotificationService.showConfirmation("Are you sure you want to delete this trunk; this trunk will be removed from all trunk groups").then(function (result) {
                        return result;
                    });
                };

                $scope.scopeModel.onTrunkDeleted = function (deletedItem) {
                    var trunkIndex = UtilsService.getItemIndexByVal($scope.scopeModel.trunks, deletedItem.TrunkId, "TrunkId");
                    if (trunkIndex >= 0)
                        $scope.scopeModel.trunks.splice(trunkIndex, 1);

                    deleteTrunkFromNotselected(deletedItem);

                    deleteTrunkFromTrunkGroups(deletedItem);

                };


                $scope.scopeModel.onTrunkNameValueBlur = function (value) {
                    for (var i = 0; i < $scope.scopeModel.trunks.length; i++) {
                        var currentTrunk = $scope.scopeModel.trunks[i];
                        if (currentTrunk.TrunkName == undefined || currentTrunk.TrunkName == "")
                            continue;

                        var currentNotSelectedTrunk = UtilsService.getItemByVal(notSelectedTrunks, currentTrunk.TrunkId, "TrunkId");

                        if (currentNotSelectedTrunk != undefined) {
                            currentNotSelectedTrunk.description = currentTrunk.TrunkName;
                        }

                        for (var j = 0; j < $scope.scopeModel.trunkGroups.length; j++) {
                            var currentTrunkGroup = $scope.scopeModel.trunkGroups[j];

                            var trunksInfo = currentTrunkGroup.trunksInfo;
                            if (trunksInfo == undefined)
                                continue;

                            var currentTrunkInfo = UtilsService.getItemByVal(trunksInfo, currentTrunk.TrunkId, "value");

                            if (currentTrunkInfo != undefined) {
                                currentTrunkInfo.description = currentTrunk.TrunkName;
                            } else {
                                if (currentNotSelectedTrunk != undefined) {
                                    var key = trunksInfo.length;
                                    trunksInfo.push({
                                        id: currentTrunkGroup.TrunkGroupNb,
                                        value: currentTrunk.TrunkId,
                                        description: currentTrunk.TrunkName
                                    });
                                }
                            }
                        }
                    }
                };

                $scope.scopeModel.isTrunksValid = function () {
                    var trunks = $scope.scopeModel.trunks;
                    if (trunks.length == 0) {
                        return 'You should have at Least 1 Trunk.';
                    }
                    else {
                        var trunkNames = [];
                        for (var index = 0; index < trunks.length; index++) {
                            var currentTrunk = trunks[index];
                            if (trunkNames.includes(currentTrunk.TrunkName))
                                return "Trunk Name is unique";

                            if (currentTrunk.TrunkName != undefined && currentTrunk.TrunkName != "")
                                trunkNames.push(currentTrunk.TrunkName);
                        }
                    }
                    return null;
                };

                $scope.scopeModel.onSelectTrunk = function (trunk) {

                    var notSelectedTrunkIndex = UtilsService.getItemIndexByVal(notSelectedTrunks, trunk.value, "TrunkId");
                    if (notSelectedTrunkIndex >= 0)
                        notSelectedTrunks.splice(notSelectedTrunkIndex, 1);

                    for (var index = 0; index < $scope.scopeModel.trunkGroups.length; index++) {
                        var currentTrunkGroup = $scope.scopeModel.trunkGroups[index];

                        var trunksInfo = currentTrunkGroup.trunksInfo;
                        if (trunksInfo == undefined)
                            continue;
                        if (trunksInfo.length <= 0 || trunk.id != trunksInfo[0].id) {
                            var currentTrunkIndex = UtilsService.getItemIndexByVal(trunksInfo, trunk.value, "value");
                            if (currentTrunkIndex >= 0)
                                trunksInfo.splice(currentTrunkIndex, 1);
                        }
                    }
                };

                $scope.scopeModel.onDeselectTrunk = function (item) {
                    addTrunksToNotSelected([item]);
                };

                $scope.scopeModel.onTrunkGroupGridReady = function (api) {
                    trunkGroupGridAPI = api;
                    trunkGroupGridReadyDeferred.resolve();
                };

                $scope.scopeModel.onTrunkGroupAdded = function () {
                    $scope.scopeModel.isTrunkGroupGridLoading = true;

                    var addedTrunkGroup = {
                        TrunkGroupNb: rowIndex++,
                        selectedTrunksInfo: []
                    };

                    var trunkGroupLoadDirectivesDeferred = UtilsService.createPromiseDeferred();
                    extendTrunkGroupEntity(addedTrunkGroup, trunkGroupLoadDirectivesDeferred);
                    $scope.scopeModel.trunkGroups.push(addedTrunkGroup);

                    UtilsService.waitMultiplePromises([trunkGroupLoadDirectivesDeferred.promise]).then(function () {

                        $scope.scopeModel.isTrunkGroupGridLoading = false;
                    });
                };

                $scope.scopeModel.onTrunkGroupDeleted = function (deletedItem) {

                    var trunks = deletedItem.selectedTrunksInfo;
                    addTrunksToNotSelected(trunks, deletedItem.TrunkGroupNb);

                    var trunkGroupsIndex = UtilsService.getItemIndexByVal($scope.scopeModel.trunkGroups, deletedItem.TrunkGroupNb, "TrunkGroupNb");
                    if (trunkGroupsIndex >= 0)
                        $scope.scopeModel.trunkGroups.splice(trunkGroupsIndex, 1);
                };

                $scope.scopeModel.isTrunkGroupsValid = function () {

                    var trunkGroups = $scope.scopeModel.trunkGroups;
                    if (trunkGroups.length > 0) {
                        var trunkGroupsNames = [];

                        if ($scope.scopeModel.isTrunkGroupGridLoading)
                            return null;

                        for (var index = 0; index < trunkGroups.length; index++) {
                            var currentTrunkGroup = trunkGroups[index];

                            if (trunkGroupsNames.includes(currentTrunkGroup.Name))
                                return "Trunk Group Name is unique";

                            if (currentTrunkGroup.Name != undefined && currentTrunkGroup.Name != "")
                                trunkGroupsNames.push(currentTrunkGroup.Name);
                        }
                    }
                    return null;
                };


                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var settings;

                    if (payload != undefined && payload.selectedValues != undefined) {
                        settings = payload.selectedValues.Settings;
                    }

                    if (settings != undefined) {
                        if (settings.Trunks != undefined) {

                            var trunkGridLoadPromise = getTrunkGridLoadPromise(settings.Trunks);
                            promises.push(trunkGridLoadPromise);
                        }
                        if (settings.TrunkGroups) {
                            var trunkGroupGridLoadPromise = getTrunkGroupGridLoadPromise(settings.TrunkGroups);
                            promises.push(trunkGroupGridLoadPromise);
                        }
                    }

                    function getTrunkGridLoadPromise(trunks) {
                        var trunkGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        trunkGridReadyDeferred.promise.then(function () {

                            for (var index = 0; index < trunks.length; index++) {
                                var currentTrunk = trunks[index];
                                notSelectedTrunks.push(currentTrunk);
                                $scope.scopeModel.trunks.push(currentTrunk);
                            }
                            trunkGridLoadPromiseDeferred.resolve();
                        });
                        return trunkGridLoadPromiseDeferred.promise;
                    }

                    function getTrunkGroupGridLoadPromise(trunkGroups) {
                        var trunkGroupGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        trunkGroupGridReadyDeferred.promise.then(function () {
                            trunkGridLoadPromise.then(function () {

                                if (trunkGroups != undefined) {

                                    for (var index = 0; index < trunkGroups.length; index++) {
                                        removeSelectedFromNotselected(trunkGroups[index].Trunks);
                                    }

                                    var _promises = [];
                                    for (var index = 0; index < trunkGroups.length; index++) {
                                        var currentTrunkGroup = trunkGroups[index];
                                        currentTrunkGroup.TrunkGroupNb = rowIndex;

                                        currentTrunkGroup.selectedTrunksInfo = [];

                                        var trunkGroupLoadDirectivesDeferred = UtilsService.createPromiseDeferred();
                                        _promises.push(trunkGroupLoadDirectivesDeferred.promise);
                                        extendTrunkGroupEntity(currentTrunkGroup, trunkGroupLoadDirectivesDeferred);
                                        $scope.scopeModel.trunkGroups.push(currentTrunkGroup);
                                    }
                                }

                                UtilsService.waitMultiplePromises(_promises).then(function () {
                                    trunkGroupGridLoadPromiseDeferred.resolve();
                                });
                            });
                        });

                        return trunkGroupGridLoadPromiseDeferred.promise;
                    }


                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        firstLoad = false;
                    });
                };


                api.setData = function (switchInterconnectionItem) {

                    switchInterconnectionItem.Settings = getSwitchInterconnectionEntity();;
                };

                api.getData = function () {
                    return getSupplierMappingEntity();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function extendTrunkGroupEntity(trunkGroup, trunkGroupLoadDirectivesDeferred) {

                loadTrunkSelector(trunkGroup);

                trunkGroupLoadDirectivesDeferred.resolve();
            }

            function loadTrunkSelector(trunkGroup) {
                trunkGroup.trunksInfo = [];
                if (notSelectedTrunks != undefined && notSelectedTrunks.length > 0) {
                    for (var index = 0; index < notSelectedTrunks.length; index++) {
                        var currentNotSelectedTrunk = notSelectedTrunks[index];
                        trunkGroup.trunksInfo.push({
                            id: trunkGroup.TrunkGroupNb,
                            value: currentNotSelectedTrunk.TrunkId,
                            description: currentNotSelectedTrunk.TrunkName
                        });
                    }
                }

                if (trunkGroup.Trunks != undefined) {
                    for (var index = 0; index < trunkGroup.Trunks.length; index++) {
                        var currentSelectedTrunk = trunkGroup.Trunks[index];
                        trunkGroup.trunksInfo.push({
                            id: trunkGroup.TrunkGroupNb,
                            value: currentSelectedTrunk.TrunkId,
                            description: currentSelectedTrunk.TrunkName
                        });

                        var selectedValue = UtilsService.getItemByVal(trunkGroup.trunksInfo, currentSelectedTrunk.TrunkId, "value");
                        if (selectedValue != null) {
                            trunkGroup.selectedTrunksInfo.push(selectedValue);
                        }
                    }
                }
            }
            function removeSelectedFromNotselected(trunks) {
                if (trunks != undefined && trunks.length > 0) {

                    for (var index = 0; index < trunks.length; index++) {

                        var currentTrunk = trunks[index];
                        var notSelectedTrunkIndex = UtilsService.getItemIndexByVal(notSelectedTrunks, currentTrunk.TrunkId, "TrunkId");
                        if (notSelectedTrunkIndex >= 0)
                            notSelectedTrunks.splice(notSelectedTrunkIndex, 1);
                    }
                }
            }

            function deleteTrunkFromTrunkGroups(trunk) {
                for (var index = 0; index < $scope.scopeModel.trunkGroups.length; index++) {
                    var currentTrunkGroup = $scope.scopeModel.trunkGroups[index];

                    var trunksInfo = currentTrunkGroup.trunksInfo;
                    if (trunksInfo == undefined)
                        continue;

                    var currentTrunkIndex = UtilsService.getItemIndexByVal(trunksInfo, trunk.TrunkId, "value");
                    if (currentTrunkIndex >= 0)
                        trunksInfo.splice(currentTrunkIndex, 1);

                    var selectedTrunkInfoIndex = UtilsService.getItemIndexByVal(currentTrunkGroup.selectedTrunksInfo, trunk.TrunkId, "value");
                    if (selectedTrunkInfoIndex >= 0) {
                        currentTrunkGroup.selectedTrunksInfo.splice(selectedTrunkInfoIndex, 1);
                    }
                }
            }

            function deleteTrunkFromNotselected(trunk) {
                var notSelectedTrunkIndex = UtilsService.getItemIndexByVal(notSelectedTrunks, trunk.TrunkId, "TrunkId");
                if (notSelectedTrunkIndex >= 0)
                    notSelectedTrunks.splice(notSelectedTrunkIndex, 1);
            }


            function addTrunksToNotSelected(trunks, trunkGroupNb) {
                if (trunks != undefined && trunks.length > 0) {
                    for (var index = 0; index < trunks.length; index++) {
                        var currentTrunk = trunks[index];

                        notSelectedTrunks.push({
                            TrunkId: currentTrunk.value,
                            TrunkName: currentTrunk.description,
                        });
                    }

                    for (var trunkGroupsIndex = 0; trunkGroupsIndex < $scope.scopeModel.trunkGroups.length; trunkGroupsIndex++) {
                        var currentTrunkGroup = $scope.scopeModel.trunkGroups[trunkGroupsIndex];
                        if (trunkGroupNb == undefined || currentTrunkGroup.TrunkGroupNb != trunkGroupNb) {
                            for (var trunksIndex = 0; trunksIndex < trunks.length; trunksIndex++) {
                                var currentTrunk = trunks[trunksIndex];
                                if (UtilsService.getItemIndexByVal(currentTrunkGroup.trunksInfo, currentTrunk.value, "value") < 0)
                                    currentTrunkGroup.trunksInfo.push({
                                        id: currentTrunkGroup.TrunkGroupNb,
                                        value: currentTrunk.value,
                                        description: currentTrunk.description
                                    });
                            }
                        }
                    }
                }
            }

            function getSwitchInterconnectionEntity() {

                function getTrunks() {
                    var trunks = [];
                    for (var index = 0; index < $scope.scopeModel.trunks.length; index++) {
                        var currentTrunk = $scope.scopeModel.trunks[index];
                        trunks.push({
                            TrunkId: currentTrunk.TrunkId,
                            TrunkName: currentTrunk.TrunkName
                        });
                    }
                    return trunks.length > 0 ? trunks : undefined;
                }
                function getTrunkGroups() {
                    var trunkGroups = [];
                    for (var index = 0; index < $scope.scopeModel.trunkGroups.length; index++) {
                        var currentTrunkGroup = $scope.scopeModel.trunkGroups[index];
                        trunkGroups.push({
                            Name: currentTrunkGroup.Name,
                            Trunks: getTrunks(currentTrunkGroup),
                        });
                    }

                    function getTrunks(trunkGroup) {
                        var Trunks = [];
                        var selectedTrunksInfo = trunkGroup.selectedTrunksInfo;
                        if (selectedTrunksInfo != undefined && selectedTrunksInfo.length > 0) {

                            for (var index = 0; index < selectedTrunksInfo.length; index++) {
                                var trunk = selectedTrunksInfo[index];
                                Trunks.push({
                                    TrunkId: trunk.value,
                                    TrunkName: trunk.description
                                });
                            }
                        }
                        return Trunks.length > 0 ? Trunks : undefined;
                    }
                    function getCodeGroupTrunkGroups(trunkGroup) {
                        var codeGroupTrunkGroups = [];
                        if (trunkGroup.trunkGroupCodeGroupSelectorAPI != undefined) {
                            var codeGroupIds = trunkGroup.trunkGroupCodeGroupSelectorAPI.getSelectedIds();
                            if (codeGroupIds != undefined) {
                                for (var index = 0; index < codeGroupIds.length; index++) {
                                    var currentCodeGroupId = codeGroupIds[index];
                                    codeGroupTrunkGroups.push({ CodeGroupId: currentCodeGroupId });
                                }
                            }
                        }
                        return codeGroupTrunkGroups.length > 0 ? codeGroupTrunkGroups : undefined;
                    }

                    return trunkGroups.length > 0 ? trunkGroups : undefined;
                }

                var trunks = getTrunks();
                var trunkGroups = getTrunkGroups();
                if (trunks == undefined && trunkGroups == undefined)
                    return null;

                return {
                    $type: "RecordAnalysis.Entities.C4SwitchInterconnection,RecordAnalysis.Entities",
                    Trunks: trunks,
                    TrunkGroups: trunkGroups
                };
            };
        }
    }
    app.directive('recAnalC4switchinterconnectionSettings', recordAnalysisSwitchinterconnectionSettingsDirective);

})(app);