'use strict';

app.directive('whsRoutesyncEricssonSuppliermapping', ['VRUIUtilsService', 'UtilsService', 'WhS_RouteSync_TrunkTypeEnum', 'WhS_RouteSync_RouteSyncDefinitionService', 'WhS_RouteSync_EricssonAPIService',
    function (VRUIUtilsService, UtilsService, WhS_RouteSync_TrunkTypeEnum, WhS_RouteSync_RouteSyncDefinitionService, WhS_RouteSync_EricssonAPIService) {
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
            var supplierOutTrunksMappings;
            var supplierId;
            var drillDownManager;

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
                    var trunk = {
                        TrunkId: UtilsService.guid(),
                        TrunkName: undefined,
                        TrunkType: 0,
                        IsSwitch: false,
                        NationalCountryCode: undefined
                    };
                    extendTrunkEntity(trunk);
                    $scope.scopeModel.trunks.push(trunk);
                };

                $scope.scopeModel.validateTrunkName = function (name) {
                    if (name != undefined && name.includes('~'))
                        return "Trunk name can not include '~'.";
                };

                $scope.scopeModel.onBeforeDeleteTrunk = function (deletedItem) {
                    if (context != undefined && context.getCarrierMappings != undefined && typeof (context.getCarrierMappings) == 'function') {
                        return WhS_RouteSync_EricssonAPIService.IsSupplierTrunkUsed(supplierId, deletedItem.TrunkId, deletedItem.TrunkName, context.getCarrierMappings()).then(function (response) {

                            if (response.Result) {
                                context.clearError();
                            } else {
                                context.displayError(response.ErrorMessages, 'Failed to delete Trunk');
                            }
                            return response.Result;
                        });
                    }
                };

                $scope.scopeModel.onTrunkDeleted = function (deletedItem) {
                    var trunkIndex = UtilsService.getItemIndexByVal($scope.scopeModel.trunks, deletedItem.TrunkId, "TrunkId");
                    $scope.scopeModel.trunks.splice(trunkIndex, 1);

                    var index = UtilsService.getItemIndexByVal(supplierOutTrunksMappings[supplierId], deletedItem.TrunkId, "TrunkId");
                    if (index > -1) {
                        supplierOutTrunksMappings[supplierId].splice(index, 1);
                    }

                    updateSupplierMappingDescription();
                };

                $scope.scopeModel.onTrunkNameValueBlur = function (value) {

                    for (var i = 0; i < $scope.scopeModel.trunks.length; i++) {
                        var currentTrunk = $scope.scopeModel.trunks[i];
                        if (currentTrunk.TrunkName == undefined || currentTrunk.TrunkName == "")
                            continue;

                        for (var j = 0; j < $scope.scopeModel.trunkGroups.length; j++) {
                            var currentTrunkGroup = $scope.scopeModel.trunkGroups[j];
                            if (currentTrunkGroup.trunkTrunkGroupDirectiveAPI == undefined)
                                continue;

                            var trunksInfo = currentTrunkGroup.trunkTrunkGroupDirectiveAPI.getTrunksInfo();
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

                        var index = UtilsService.getItemIndexByVal(supplierOutTrunksMappings[supplierId], currentTrunk.TrunkId, "TrunkId");
                        if (index == -1) {
                            supplierOutTrunksMappings[supplierId].push(currentTrunk);
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
                    drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(buildTrunkTrunkGroupDrillDownTabs(), trunkGroupGridAPI);
                    trunkGroupGridReadyDeferred.resolve();
                };

                $scope.scopeModel.onTrunkGroupAdded = function () {
                    $scope.scopeModel.isTrunkGroupGridLoading = true;

                    var addedTrunkGroup = {
                        TrunkGroupNb: $scope.scopeModel.trunkGroups.length + 1,
                        IsBackup: false,
                        selectedTrunksInfo: []
                    };

                    var trunkGroupLoadDirectivesDeferred = UtilsService.createPromiseDeferred();
                    extendTrunkGroupEntity(addedTrunkGroup, trunkGroupLoadDirectivesDeferred);

                    drillDownManager.setDrillDownExtensionObject(addedTrunkGroup);

                    trunkGroupGridAPI.expandRow(addedTrunkGroup);
                    $scope.scopeModel.trunkGroups.push(addedTrunkGroup);

                    return UtilsService.waitMultiplePromises([trunkGroupLoadDirectivesDeferred.promise]).then(function () {
                        $scope.scopeModel.isTrunkGroupGridLoading = false;
                    });
                };

                $scope.scopeModel.onTrunkGroupDeleted = function (deletedItem) {
                    var trunkGroupsIndex = UtilsService.getItemIndexByVal($scope.scopeModel.trunkGroups, deletedItem.TrunkGroupNb, "TrunkGroupNb");
                    $scope.scopeModel.trunkGroups.splice(trunkGroupsIndex, 1);

                    updateSupplierMappingDescription();
                };

                $scope.scopeModel.isTrunkGroupsValid = function () {
                    if (!isFirstLoad) {
                        var trunkGroups = $scope.scopeModel.trunkGroups;
                        if (trunkGroups.length > 0) {

                            if ($scope.scopeModel.isTrunkGroupGridLoading)
                                return null;

                            var customerCodeGroupCombinations = [];
                            var errorMessage = "you cannot define same combination at different trunk groups"; //(customer, code group, isBackup)

                            for (var index = 0; index < trunkGroups.length; index++) {
                                var currentTrunkGroup = trunkGroups[index];
                                var customerIds = currentTrunkGroup.trunkGroupCustomerSelectorAPI != undefined ? currentTrunkGroup.trunkGroupCustomerSelectorAPI.getSelectedIds() : undefined;
                                var codeGroups = currentTrunkGroup.trunkGroupCodeGroupSelectorAPI != undefined ? currentTrunkGroup.trunkGroupCodeGroupSelectorAPI.getSelectedIds() : undefined;
                                var isBackup = currentTrunkGroup.IsBackup ? "1" : "0";

                                if (customerIds == undefined && codeGroups == undefined) {
                                    var customerCodeGroupCombination = "-1,-1," + isBackup;
                                    if (customerCodeGroupCombinations.includes(customerCodeGroupCombination)) {
                                        return errorMessage;
                                    } else {
                                        customerCodeGroupCombinations.push(customerCodeGroupCombination);
                                    }
                                }
                                else if (customerIds != undefined && codeGroups != undefined) {
                                    for (var i = 0; i < customerIds.length; i++) {
                                        var currentCustomerId = customerIds[i];

                                        for (var j = 0; j < codeGroups.length; j++) {
                                            var currentCodeGroup = codeGroups[j];

                                            var customerCodeGroupCombination = currentCustomerId + "," + currentCodeGroup + "," + isBackup;
                                            if (customerCodeGroupCombinations.includes(customerCodeGroupCombination)) {
                                                return errorMessage;
                                            } else {
                                                customerCodeGroupCombinations.push(customerCodeGroupCombination);
                                            }
                                        }
                                    }
                                } else if (customerIds != undefined && codeGroups == undefined) {
                                    for (var i = 0; i < customerIds.length; i++) {
                                        var currentCustomerId = customerIds[i];

                                        var customerCodeGroupCombination = currentCustomerId + ",-1," + isBackup;
                                        if (customerCodeGroupCombinations.includes(customerCodeGroupCombination)) {
                                            return errorMessage;
                                        } else {
                                            customerCodeGroupCombinations.push(customerCodeGroupCombination);
                                        }
                                    }
                                } else if (customerIds == undefined && codeGroups != undefined) {
                                    for (var i = 0; i < codeGroups.length; i++) {
                                        var currentCodeGroup = codeGroups[i];

                                        var customerCodeGroupCombination = "-1," + currentCodeGroup + "," + isBackup;
                                        if (customerCodeGroupCombinations.includes(customerCodeGroupCombination)) {
                                            return errorMessage;
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
                        supplierOutTrunksMappings = payload.supplierOutTrunksMappings;
                        supplierId = payload.supplierId;
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

                            for (var index = 0; index < trunks.length; index++) {
                                var currentTrunk = trunks[index];
                                extendTrunkEntity(currentTrunk);
                                $scope.scopeModel.trunks.push(currentTrunk);
                            }

                            trunkGridLoadPromiseDeferred.resolve();
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
                                    drillDownManager.setDrillDownExtensionObject(currentTrunkGroup);
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
                        UtilsService.watchFunction($scope, 'validationContext.validate()', updateDescriptions);
                    });
                };

                api.getData = function () {
                    return getSupplierMappingEntity();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function extendTrunkEntity(trunk) {
                trunk.selectedTrunkType = UtilsService.getEnum(WhS_RouteSync_TrunkTypeEnum, 'value', trunk.TrunkType);

                trunk.openNationalCountryCodesEditor = function () {
                    var onNationalCountryCodesUpdated = function (updatedNationalCountryCodes) {
                        trunk.NationalCountryCode = updatedNationalCountryCodes;
                    };

                    WhS_RouteSync_RouteSyncDefinitionService.editNationalCountryCodes(trunk.NationalCountryCode, onNationalCountryCodesUpdated);
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
                        codeGroupSelectorPayload = { selectedIds: UtilsService.getPropValuesFromArray(trunkGroup.CodeGroupTrunkGroups, "CodeGroupId") };
                    }
                    VRUIUtilsService.callDirectiveLoad(trunkGroup.trunkGroupCodeGroupSelectorAPI, codeGroupSelectorPayload, trunkGroupCodeGroupSelectorLoadDeferred);
                };

                UtilsService.waitMultiplePromises([trunkGroupCustomerSelectorLoadDeferred.promise, trunkGroupCodeGroupSelectorLoadDeferred.promise]).then(function () {
                    trunkGroupLoadDirectivesDeferred.resolve();
                });
            }

            function buildTrunkTrunkGroupDrillDownTabs() {

                var drillDownTabs = [];
                drillDownTabs.push(buildTrunkTrunkGroupDrillDownTab());

                function buildTrunkTrunkGroupDrillDownTab() {
                    var drillDownTab = {};
                    drillDownTab.title = "Trunks";
                    drillDownTab.directive = "whs-routesync-ericsson-trunktrunkgroup";

                    drillDownTab.loadDirective = function (trunkTrunkGroupDirectiveAPI, trunkGroup) {
                        trunkGroup.trunkTrunkGroupDirectiveAPI = trunkTrunkGroupDirectiveAPI;

                        return trunkGroup.trunkTrunkGroupDirectiveAPI.load(buildTrunkTrunkGroupPayload(trunkGroup));
                    };

                    function buildTrunkTrunkGroupPayload(trunkGroup) {
                        var trunkTrunkGroupPayload = {};
                        trunkTrunkGroupPayload.trunks = $scope.scopeModel.trunks;
                        trunkTrunkGroupPayload.trunkTrunkGroups = trunkGroup.TrunkTrunkGroups;
                        trunkTrunkGroupPayload.LoadSharing = trunkGroup.LoadSharing;
                        trunkTrunkGroupPayload.supplierOutTrunksMappings = supplierOutTrunksMappings;
                        return trunkTrunkGroupPayload;
                    }

                    return drillDownTab;
                }

                return drillDownTabs;
            }

            function updateDescriptions() {
                updateErrorDescription();
                updateSupplierMappingDescription();
            }

            function updateErrorDescription() {
                if (isFirstLoad || context == undefined)
                    return;

                var validatationMessage = $scope.validationContext.validate();
                var isValid = validatationMessage == null;
                context.updateErrorDescription(isValid, false);
            }

            function updateSupplierMappingDescription() {
                if (isFirstLoad || context == undefined)
                    return;

                if ($scope.scopeModel.supplierMappingExists) {
                    context.updateSupplierMappingDescription(getSupplierMappingEntity());
                } else {
                    context.updateSupplierMappingDescription(null);
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
                            TrunkType: currentTrunk.selectedTrunkType.value,
                            IsSwitch: currentTrunk.IsSwitch,
                            NationalCountryCode: currentTrunk.NationalCountryCode
                        });
                    }
                    return trunks.length > 0 ? trunks : undefined;
                }
                function getTrunkGroups() {
                    var trunkGroups = [];

                    for (var index = 0; index < $scope.scopeModel.trunkGroups.length; index++) {
                        var currentTrunkGroup = $scope.scopeModel.trunkGroups[index];

                        var currentTrunkGroupDirectiveData;
                        if (currentTrunkGroup.trunkTrunkGroupDirectiveAPI != undefined) {
                            currentTrunkGroupDirectiveData = currentTrunkGroup.trunkTrunkGroupDirectiveAPI.getData();
                        } else {
                            currentTrunkGroupDirectiveData = UtilsService.getItemByVal($scope.scopeModel.trunkGroups, currentTrunkGroup.TrunkGroupNb, 'TrunkGroupNb');
                        }

                        trunkGroups.push({
                            CustomerTrunkGroups: getCustomerTrunkGroups(currentTrunkGroup),
                            CodeGroupTrunkGroups: getCodeGroupTrunkGroups(currentTrunkGroup),
                            TrunkTrunkGroups: currentTrunkGroupDirectiveData != undefined ? currentTrunkGroupDirectiveData.TrunkTrunkGroups : undefined,
                            IsBackup: currentTrunkGroup.IsBackup,
                            LoadSharing: currentTrunkGroupDirectiveData != undefined ? currentTrunkGroupDirectiveData.LoadSharing : undefined
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
                        return customerTrunkGroups.length > 0 ? customerTrunkGroups : undefined;
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
                    OutTrunks: trunks,
                    TrunkGroups: trunkGroups
                };
            }
        }
    }]);