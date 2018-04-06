'use strict';

app.directive('whsRoutesyncEricssonSuppliermapping', ['VRNotificationService', 'VRUIUtilsService', 'UtilsService', 'WhS_RouteSync_TrunkTypeEnum',
    function (VRNotificationService, VRUIUtilsService, UtilsService, WhS_RouteSync_TrunkTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new EricssonSupplierMappingDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/Templates/EricssonSupplierMappingTemplate.html'
        };

        function EricssonSupplierMappingDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var isFirstLoad = true;
            var context;

            var trunkGridAPI;
            var trunkGridReadyDeferred = UtilsService.createPromiseDeferred();

            var trunkGroupGridAPI;
            var trunkGroupGridReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.trunks = [];
                $scope.scopeModel.trunkGroups = [];
                $scope.scopeModel.supplierMappingExists = false;
                $scope.scopeModel.isTrunkGroupGridLoading = false;
                $scope.scopeModel.trunkTypes = UtilsService.getArrayEnum(WhS_RouteSync_TrunkTypeEnum);

                $scope.scopeModel.onTrunkGridReady = function (api) {
                    trunkGridAPI = api;
                    trunkGridReadyDeferred.resolve();
                };

                $scope.scopeModel.onTrunkAdded = function () {
                    $scope.scopeModel.trunks.push({
                        TrunkId: UtilsService.guid(),
                        TrunkName: undefined,
                        NationalCountryCode: undefined,
                        selectedTrunkType: UtilsService.getEnum(WhS_RouteSync_TrunkTypeEnum, 'value', 0),
                        IsRouting: true
                    });

                    $scope.scopeModel.updateSupplierDescriptions();
                };

                $scope.scopeModel.onBeforeTrunkDeleted = function (deletedItem) {
                    return VRNotificationService.showConfirmation("Are you sure you want to delete this trunk; this trunk will be removed from all trunk groups").then(function (result) {
                        return result;
                    });
                };

                $scope.scopeModel.onTrunkDeleted = function (deletedItem) {
                    var trunkIndex = UtilsService.getItemIndexByVal($scope.scopeModel.trunks, deletedItem.TrunkId, "TrunkId");
                    $scope.scopeModel.trunks.splice(trunkIndex, 1);

                    deleteTrunkFromTrunkGroups(deletedItem);
                    $scope.scopeModel.updateSupplierDescriptions();
                };

                $scope.scopeModel.onTrunkNameValueChanged = function (value) {
                    updateErrorDescription();
                    updateSupplierMappingDescription();
                };

                $scope.scopeModel.onTrunkNameValueBlur = function (value) {

                    for (var i = 0 ; i < $scope.scopeModel.trunks.length; i++) {
                        var currentTrunk = $scope.scopeModel.trunks[i];
                        if (currentTrunk.TrunkName == undefined || currentTrunk.TrunkName == "")
                            continue;

                        for (var j = 0 ; j < $scope.scopeModel.trunkGroups.length; j++) {
                            var currentTrunkGroup = $scope.scopeModel.trunkGroups[j];
                            if (currentTrunkGroup.trunkTrunkGroupGridAPI == undefined)
                                continue;

                            var trunksInfo = currentTrunkGroup.trunkTrunkGroupGridAPI.getTrunksInfo();
                            if (trunksInfo == undefined)
                                continue;

                            var currentTrunkInfo = UtilsService.getItemByVal(trunksInfo, currentTrunk.TrunkId, "value");

                            if (currentTrunkInfo != undefined) {
                                currentTrunkInfo.description = currentTrunk.TrunkName;
                            } else {
                                trunksInfo.push({
                                    value: currentTrunk.TrunkId,
                                    description: currentTrunk.TrunkName
                                });
                            }
                        }
                    }
                };

                $scope.scopeModel.isTrunksValid = function () {
                    if (!isFirstLoad) {
                        var trunks = $scope.scopeModel.trunks;
                        if (trunks.length > 0) {
                            $scope.scopeModel.supplierMappingExists = true;

                            var trunkNames = [];
                            for (var index = 0; index < trunks.length; index++) {
                                var currentTrunk = trunks[index];
                                if (trunkNames.includes(currentTrunk.TrunkName))
                                    return "Trunk Name is unique";

                                if (currentTrunk.TrunkName != undefined && currentTrunk.TrunkName != "")
                                    trunkNames.push(currentTrunk.TrunkName);
                            }
                        } else {
                            $scope.scopeModel.supplierMappingExists = false;
                        }
                    }
                    return null;
                };

                $scope.scopeModel.onTrunkGroupGridReady = function (api) {
                    trunkGroupGridAPI = api;
                    trunkGroupGridReadyDeferred.resolve();
                };

                $scope.scopeModel.onTrunkGroupAdded = function () {
                    $scope.scopeModel.isTrunkGroupGridLoading = true;

                    var addedTrunkGroup = {
                        TrunkGroupNb: $scope.scopeModel.trunkGroups.length + 1,
                        IsBackup: true,
                        selectedTrunksInfo: []
                    };

                    var trunkGroupLoadDirectivesDeferred = UtilsService.createPromiseDeferred();
                    extendTrunkGroupEntity(addedTrunkGroup, trunkGroupLoadDirectivesDeferred);
                    defineTrunkTrunkGroupTabs(addedTrunkGroup, $scope.scopeModel.trunks);

                    trunkGroupGridAPI.expandRow(addedTrunkGroup);
                    //$scope.scopeModel.updateSupplierDescriptions();
                    $scope.scopeModel.trunkGroups.push(addedTrunkGroup);

                    UtilsService.waitMultiplePromises([trunkGroupLoadDirectivesDeferred.promise]).then(function () {

                        $scope.scopeModel.isTrunkGroupGridLoading = false;
                    });
                };

                $scope.scopeModel.onTrunkGroupDeleted = function (deletedItem) {
                    var trunkGroupsIndex = UtilsService.getItemIndexByVal($scope.scopeModel.trunkGroups, deletedItem.TrunkGroupNb, "TrunkGroupNb");
                    $scope.scopeModel.trunkGroups.splice(trunkGroupsIndex, 1);

                    $scope.scopeModel.updateSupplierDescriptions();
                };

                $scope.scopeModel.isTrunkGroupsValid = function () {
                    if (!isFirstLoad) {
                        var trunkGroups = $scope.scopeModel.trunkGroups;
                        if (trunkGroups.length > 0) {
                            var customerCodeGroupCombinations = [];

                            if ($scope.scopeModel.isTrunkGroupGridLoading)
                                return null;

                            for (var index = 0; index < trunkGroups.length; index++) {
                                var currentTrunkGroup = trunkGroups[index];
                                var customerIds = currentTrunkGroup.trunkGroupCustomerSelectorAPI != undefined ? currentTrunkGroup.trunkGroupCustomerSelectorAPI.getSelectedIds() : undefined;
                                var codeGroups = currentTrunkGroup.trunkGroupCodeGroupSelectorAPI != undefined ? currentTrunkGroup.trunkGroupCodeGroupSelectorAPI.getSelectedIds() : undefined;

                                if (customerIds == undefined && codeGroups == undefined) {
                                    var customerCodeGroupCombination = "-1,-1";
                                    if (customerCodeGroupCombinations.includes(customerCodeGroupCombination)) {
                                        return "you cannot define same (customer, code group) combination at different trunk groups";
                                    } else {
                                        customerCodeGroupCombinations.push(customerCodeGroupCombination);
                                    }
                                }
                                else if (customerIds != undefined && codeGroups != undefined) {
                                    for (var i = 0; i < customerIds.length; i++) {
                                        var currentCustomerId = customerIds[i];

                                        for (var j = 0; j < codeGroups.length; j++) {
                                            var currentCodeGroup = codeGroups[j];

                                            var customerCodeGroupCombination = currentCustomerId + "," + currentCodeGroup;
                                            if (customerCodeGroupCombinations.includes(customerCodeGroupCombination)) {
                                                return "you cannot define same (customer, code group) combination at different trunk groups";
                                            } else {
                                                customerCodeGroupCombinations.push(customerCodeGroupCombination);
                                            }
                                        }
                                    }
                                } else if (customerIds != undefined && codeGroups == undefined) {
                                    for (var i = 0; i < customerIds.length; i++) {
                                        var currentCustomerId = customerIds[i];

                                        var customerCodeGroupCombination = currentCustomerId + ",-1";
                                        if (customerCodeGroupCombinations.includes(customerCodeGroupCombination)) {
                                            return "you cannot define same (customer, code group) combination at different trunk groups";
                                        } else {
                                            customerCodeGroupCombinations.push(customerCodeGroupCombination);
                                        }
                                    }
                                } else if (customerIds == undefined && codeGroups != undefined) {
                                    for (var i = 0; i < codeGroups.length; i++) {
                                        var currentCodeGroup = codeGroups[i];

                                        var customerCodeGroupCombination = "-1," + currentCodeGroup;
                                        if (customerCodeGroupCombinations.includes(customerCodeGroupCombination)) {
                                            return "you cannot define same (customer, code group) combination at different trunk groups";
                                        } else {
                                            customerCodeGroupCombinations.push(customerCodeGroupCombination);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return null;
                };

                $scope.scopeModel.updateSupplierDescriptions = function () {
                    setTimeout(function () {
                        $scope.$apply(function () {
                            updateErrorDescription();
                            updateSupplierMappingDescription();
                        });
                    }, 0);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var supplierMapping;

                    if (payload != undefined) {
                        context = payload.context;
                        supplierMapping = payload.supplierMapping;
                    }

                    if (supplierMapping != undefined && supplierMapping.OutTrunks != undefined) {
                        var trunkGridLoadPromise = getTrunkGridLoadPromise(supplierMapping.OutTrunks);
                        promises.push(trunkGridLoadPromise);

                        var trunkGroupGridLoadPromise = getTrunkGroupGridLoadPromise(supplierMapping.OutTrunks, supplierMapping.TrunkGroups);
                        promises.push(trunkGroupGridLoadPromise);
                    }

                    function getTrunkGridLoadPromise(trunks) {
                        var trunkGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        trunkGridReadyDeferred.promise.then(function () {
                            var _promises = [];

                            for (var index = 0; index < trunks.length; index++) {
                                var currentTrunk = trunks[index];

                                var trunkTypeLoadSelectorDeferred = UtilsService.createPromiseDeferred();
                                _promises.push(trunkTypeLoadSelectorDeferred.promise);
                                extendTrunkEntity(currentTrunk, trunkTypeLoadSelectorDeferred);
                                $scope.scopeModel.trunks.push(currentTrunk);
                            }

                            UtilsService.waitMultiplePromises(_promises).then(function () {
                                trunkGridLoadPromiseDeferred.resolve();
                            });
                        });

                        return trunkGridLoadPromiseDeferred.promise;
                    }
                    function getTrunkGroupGridLoadPromise(trunks, trunkGroups) {
                        var trunkGroupGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        trunkGroupGridReadyDeferred.promise.then(function () {
                            var _promises = [];

                            if (trunkGroups != undefined) {
                                for (var index = 0; index < trunkGroups.length; index++) {
                                    var currentTrunkGroup = trunkGroups[index];
                                    currentTrunkGroup.TrunkGroupNb = index + 1;

                                    var trunkGroupLoadDirectivesDeferred = UtilsService.createPromiseDeferred();
                                    _promises.push(trunkGroupLoadDirectivesDeferred.promise);
                                    extendTrunkGroupEntity(currentTrunkGroup, trunkGroupLoadDirectivesDeferred);
                                    defineTrunkTrunkGroupTabs(currentTrunkGroup, trunks);
                                    $scope.scopeModel.trunkGroups.push(currentTrunkGroup);
                                }
                            }

                            UtilsService.waitMultiplePromises(_promises).then(function () {
                                trunkGroupGridLoadPromiseDeferred.resolve();
                            });
                        });

                        return trunkGroupGridLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        isFirstLoad = false;
                    });
                };

                api.getData = function () {
                    return getSupplierMappingEntity();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function extendTrunkEntity(trunk, trunkTypeLoadSelectorDeferred) {
                trunk.onTrunkTypeSelectorReady = function (api) {
                    //trunk.trunkTypeSelectorGridAPI = api;
                    trunk.selectedTrunkType = UtilsService.getEnum(WhS_RouteSync_TrunkTypeEnum, 'value', trunk.TrunkType);
                    trunkTypeLoadSelectorDeferred.resolve();
                };
            }

            function extendTrunkGroupEntity(trunkGroup, trunkGroupLoadDirectivesDeferred) {

                var trunkGroupCustomerSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                trunkGroup.onCustomerSelectorReady = function (api) {
                    trunkGroup.trunkGroupCustomerSelectorAPI = api;

                    var customerSelectorPayload;
                    if (trunkGroup.CustomerTrunkGroups != undefined) {
                        customerSelectorPayload = { selectedIds: UtilsService.getPropValuesFromArray(trunkGroup.CustomerTrunkGroups, "CustomerId") };
                    }
                    VRUIUtilsService.callDirectiveLoad(trunkGroup.trunkGroupCustomerSelectorAPI, customerSelectorPayload, trunkGroupCustomerSelectorLoadDeferred);
                };

                var trunkGroupCodeGroupSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                trunkGroup.onCodeGroupSelectorReady = function (api) {
                    trunkGroup.trunkGroupCodeGroupSelectorAPI = api;

                    var codeGroupSelectorPayload;
                    if (trunkGroup.CodeGroupTrunkGroups != undefined) {
                        codeGroupSelectorPayload = { selectedIds: UtilsService.getPropValuesFromArray(trunkGroup.CodeGroupTrunkGroups, "CodeGroup") };
                    }
                    VRUIUtilsService.callDirectiveLoad(trunkGroup.trunkGroupCodeGroupSelectorAPI, codeGroupSelectorPayload, trunkGroupCodeGroupSelectorLoadDeferred);
                };

                UtilsService.waitMultiplePromises([trunkGroupCustomerSelectorLoadDeferred.promise, trunkGroupCodeGroupSelectorLoadDeferred.promise]).then(function () {
                    trunkGroupLoadDirectivesDeferred.resolve();
                });
            }

            function defineTrunkTrunkGroupTabs(trunkGroup) {

                var drillDownTabs = [];
                drillDownTabs.push(buildTrunkTrunkGroupDrillDownTab());

                setDrillDownTabs();

                function buildTrunkTrunkGroupDrillDownTab() {
                    var drillDownTab = {};
                    drillDownTab.title = "Trunks";
                    drillDownTab.directive = "whs-routesync-ericsson-trunktrunkgroup";

                    drillDownTab.loadDirective = function (trunkTrunkGroupGridAPI, trunkGroup) {
                        trunkGroup.trunkTrunkGroupGridAPI = trunkTrunkGroupGridAPI;

                        return trunkGroup.trunkTrunkGroupGridAPI.load(buildTrunkTrunkGroupPayload(trunkGroup)).then(function () {
                            $scope.scopeModel.updateSupplierDescriptions();
                        });
                    };

                    function buildTrunkTrunkGroupPayload(trunkGroup) {
                        var trunkTrunkGroupPayload = {};
                        trunkTrunkGroupPayload.trunks = $scope.scopeModel.trunks;
                        trunkTrunkGroupPayload.trunkTrunkGroups = trunkGroup.TrunkTrunkGroups;
                        trunkTrunkGroupPayload.context = buildTrunkTrunkGroupDirectiveContext();
                        return trunkTrunkGroupPayload;
                    }
                    function buildTrunkTrunkGroupDirectiveContext() {
                        var context = {
                            updateSupplierDescriptions: function () {
                                $scope.scopeModel.updateSupplierDescriptions();
                            }
                        };
                        return context;
                    }

                    return drillDownTab;
                }
                function setDrillDownTabs() {
                    var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, trunkGroupGridAPI);
                    drillDownManager.setDrillDownExtensionObject(trunkGroup);
                }
            }

            function updateErrorDescription() {
                if (isFirstLoad || context == undefined)
                    return;

                var validatationMessage = $scope.validationContext.validate();
                var isValid = validatationMessage == null;
                context.updateErrorDescription(isValid, false);
            }

            function updateSupplierMappingDescription() {
                if (context == undefined)
                    return;

                if ($scope.scopeModel.supplierMappingExists) {
                    context.updateSupplierMappingDescription(getSupplierMappingEntity());
                } else {
                    context.updateSupplierMappingDescription(null);
                }
            }

            function deleteTrunkFromTrunkGroups(trunk) {
                for (var index = 0; index < $scope.scopeModel.trunkGroups.length; index++) {
                    var currentTrunkGroup = $scope.scopeModel.trunkGroups[index];

                    if (currentTrunkGroup.trunkTrunkGroupGridAPI != undefined) {
                        trunkGroupGridAPI.expandRow(currentTrunkGroup);
                        currentTrunkGroup.trunkTrunkGroupGridAPI.deleteTrunkTrunkGroup(trunk);
                    } else {
                        trunkGroupGridAPI.expandRow(currentTrunkGroup);
                    }
                }
            }

            function getSupplierMappingEntity() {

                function getTrunks() {
                    var trunks = [];
                    for (var index = 0; index < $scope.scopeModel.trunks.length; index++) {
                        var currentTrunk = $scope.scopeModel.trunks[index];
                        trunks.push({
                            TrunkId: currentTrunk.TrunkId,
                            TrunkName: currentTrunk.TrunkName,
                            NationalCountryCode: currentTrunk.NationalCountryCode,
                            TrunkType: currentTrunk.selectedTrunkType.value,
                            IsRouting: currentTrunk.IsRouting
                        });
                    }
                    return trunks.length > 0 ? trunks : undefined;
                }
                function getTrunkGroups() {
                    var trunkGroups = [];
                    for (var index = 0; index < $scope.scopeModel.trunkGroups.length; index++) {
                        var currentTrunkGroup = $scope.scopeModel.trunkGroups[index];
                        trunkGroups.push({
                            TrunkTrunkGroups: currentTrunkGroup.trunkTrunkGroupGridAPI != undefined ? currentTrunkGroup.trunkTrunkGroupGridAPI.getData() : undefined,
                            CustomerTrunkGroups: getCustomerTrunkGroups(currentTrunkGroup),
                            CodeGroupTrunkGroups: getCodeGroupTrunkGroups(currentTrunkGroup),
                            IsBackup: currentTrunkGroup.IsBackup
                        });
                    }

                    function getCustomerTrunkGroups(trunkGroup) {
                        var customerTrunkGroups = [];
                        if (trunkGroup.trunkGroupCustomerSelectorAPI != undefined) {
                            var customerIds = trunkGroup.trunkGroupCustomerSelectorAPI.getSelectedIds();
                            if (customerIds != undefined) {
                                for (var index = 0; index < customerIds.length; index++) {
                                    var currentCustomerId = customerIds[index];
                                    customerTrunkGroups.push({ CustomerId: currentCustomerId });
                                }
                            }
                        }
                        return customerTrunkGroups;
                    }
                    function getCodeGroupTrunkGroups(trunkGroup) {
                        var codeGroupTrunkGroups = [];
                        if (trunkGroup.trunkGroupCodeGroupSelectorAPI != undefined) {
                            var codeGroups = trunkGroup.trunkGroupCodeGroupSelectorAPI.getSelectedIds();
                            if (codeGroups != undefined) {
                                for (var index = 0; index < codeGroups.length; index++) {
                                    var currentCodeGroup = codeGroups[index];
                                    codeGroupTrunkGroups.push({ CodeGroup: currentCodeGroup });
                                }
                            }
                        }
                        return codeGroupTrunkGroups;
                    }

                    return trunkGroups.length > 0 ? trunkGroups : undefined;
                }

                return {
                    OutTrunks: getTrunks(),
                    TrunkGroups: getTrunkGroups()
                };
            }
        }
    }]);