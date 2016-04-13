(function (appControllers) {

    'use strict';

    CodePreparationManagementController.$inject = ['$scope', 'WhS_CodePrep_CodePrepAPIService', 'WhS_BP_CreateProcessResultEnum', 'VRUIUtilsService', 'UtilsService', 'VRCommon_CountryAPIService', 'WhS_BE_SaleZoneAPIService', 'VRModalService', 'VRNotificationService', 'WhS_CP_NewCPOutputResultEnum', 'WhS_CP_ZoneItemDraftStatusEnum', 'WhS_CP_ZoneItemStatusEnum'];

    function CodePreparationManagementController($scope, WhS_CodePrep_CodePrepAPIService, WhS_BP_CreateProcessResultEnum, VRUIUtilsService, UtilsService, VRCommon_CountryAPIService, WhS_BE_SaleZoneAPIService, VRModalService, VRNotificationService, WhS_CP_NewCPOutputResultEnum, WhS_CP_ZoneItemDraftStatusEnum, WhS_CP_ZoneItemStatusEnum) {

        //#region Global Variables

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var treeAPI;
        var codesGridAPI;
        var currencyDirectiveAPI;
        var currencyReadyPromiseDeferred;
        var countries = [];
        var filter;
        var codesFilter;
        var incrementalNodeId = 0;

        //#endregion

        //#region Load

        defineScope();
        loadParameters();
        load();

        //#endregion

        //#region Functions

        function defineScope() {
            $scope.nodes = [];
            $scope.sellingNumberPlans = [];
            $scope.selectedSellingNumberPlan;
            $scope.currentNode;
            $scope.hasState = false;
            $scope.showGrid = false;

            $scope.selectedCodes = [];

            $scope.applyCodePreparationState = function () {
                var onCodePreparationStateApplied = function () {
                };
                WhS_CodePrep_CodePrepAPIService.ApplyCodePreparationState(filter.sellingNumberPlanId, onCodePreparationStateApplied);
            }

            $scope.uploadCodePreparation = function () {
                var onCodePreparationUpdated = function () {
                };
                WhS_CodePrep_CodePrepAPIService.UploadCodePreparationSheet(filter.sellingNumberPlanId, onCodePreparationUpdated);
            }

            $scope.onSellingNumberPlanSelectorReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            }

            $scope.countriesTreeReady = function (api) {
                treeAPI = api;
            }

            $scope.countriesTreeValueChanged = function () {

                if ($scope.currentNode != undefined) {
                    $scope.selectedCodes.length = 0;
                    if ($scope.currentNode.type == 'Zone') {
                        checkState();
                        codesGridAPI.clearUpdatedItems();
                        setCodesFilterObject();
                        $scope.showAddNewCode = $scope.currentNode.status != WhS_CP_ZoneItemStatusEnum.PendingClosed.value;
                        $scope.showGrid = true;
                        $scope.showAddNewZone = false;
                        $scope.showRenameZone = true;
                        $scope.showEnd = true;

                        if ($scope.currentNode.status != null) {
                            $scope.showAddNewCode = $scope.currentNode.status != WhS_CP_ZoneItemStatusEnum.PendingClosed.value;
                            $scope.showEnd = false;
                            $scope.showRenameZone = false;
                        }
                        else if ($scope.hasState) {
                            $scope.showAddNewCode = $scope.currentNode.DraftStatus != WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.value;
                            $scope.showEnd = $scope.currentNode.DraftStatus != WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.value && $scope.currentNode.DraftStatus != WhS_CP_ZoneItemDraftStatusEnum.New.value;
                            $scope.showRenameZone = $scope.currentNode.DraftStatus != WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.value;
                        }

                        return codesGridAPI.loadGrid(codesFilter);
                    }

                    clear(false);
                    $scope.showAddNewZone = true;
                }

            }

            $scope.loadEffectiveSaleZones = function (countryNode) {
                var effectiveZonesPromiseDeffered = UtilsService.createPromiseDeferred();
                WhS_CodePrep_CodePrepAPIService.GetZoneItems(filter.sellingNumberPlanId, countryNode.nodeId).then(function (response) {
                    var effectiveZones = [];
                    angular.forEach(response, function (itm) {
                        effectiveZones.push(mapZoneToNode(itm));
                    });
                    var countryIndex = UtilsService.getItemIndexByVal($scope.nodes, countryNode.nodeId, 'nodeId');
                    var parentNode = $scope.nodes[countryIndex];
                    parentNode.effectiveZones = effectiveZones;
                    $scope.nodes[countryIndex] = parentNode;
                    effectiveZonesPromiseDeffered.resolve(effectiveZones);
                });
                return effectiveZonesPromiseDeffered.promise;
            }

            $scope.onSellingNumberPlanSelectorChanged = function () {
                var selectedSellingNumberPlanId = sellingNumberPlanDirectiveAPI.getSelectedIds();
                if (selectedSellingNumberPlanId != undefined) {
                    countries.length = 0;
                    clear(false);
                    filter = getFilter();
                    $scope.isLoadingCountries = true;
                    UtilsService.waitMultipleAsyncOperations([getCountries, checkState]).then(function () {
                        buildCountriesTree();
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    }).finally(function () {
                        $scope.isLoadingCountries = false;
                    });
                }
                else {
                    clear(false);
                    $scope.selectedCodes.length = 0;
                }
            }

            $scope.saleCodesGridReady = function (api) {

                codesGridAPI = api;
                $scope.selectedCodes = codesGridAPI.getSelectedCodes();
            }

            $scope.newZoneClicked = function () {
                addNewZone();
            }

            $scope.newCodeClicked = function () {
                addNewCode();
            }

            $scope.moveCodesClicked = function () {
                moveCodes();
            }

            $scope.endClicked = function () {
                ($scope.selectedCodes.length > 0) ? closeCodes() : closeZone();
            }

            $scope.cancelState = function () {
                return VRNotificationService.showConfirmation().then(function (result) {
                    if (result) {
                        countries.length = 0;
                        return WhS_CodePrep_CodePrepAPIService.CancelCodePreparationState(filter.sellingNumberPlanId).then(function (response) {
                            $scope.hasState = !response;
                            $scope.onSellingNumberPlanSelectorChanged();
                        });
                    }
                });

            }

            $scope.renameZoneClicked = function () {
                var parameters = {
                    ZoneId: $scope.currentNode.nodeId,
                    ZoneName: $scope.currentNode.nodeName,
                    SellingNumberPlanId: filter.sellingNumberPlanId,
                    CountryId: $scope.currentNode.countryId
                };
                var settings = {};
                settings.onScopeReady = function (modalScope) {
                    modalScope.onZoneRenamed = onZoneRenamed;
                };

                VRModalService.showModal("/Client/Modules/WhS_CodePreparation/Views/Dialogs/RenameZoneDialog.html", parameters, settings);

            }

        }


            function loadParameters() {
            }

            function load() {
                $scope.isLoadingFilter = true;

                UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlans]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.isLoadingFilter = false;
                });
            }

            function clear(action) {
                $scope.showGrid = action;
                $scope.showAddNewCode = action;
                $scope.showAddNewZone = action;
                $scope.showRenameZone = action;
                $scope.showEnd = action;
            }

            function loadSellingNumberPlans() {
                var loadSNPPromiseDeferred = UtilsService.createPromiseDeferred();
                sellingNumberPlanReadyPromiseDeferred.promise.then(function () {

                    VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, undefined, loadSNPPromiseDeferred);
                });

                return loadSNPPromiseDeferred.promise;
            }

            function onZoneAdded(addedZones) {
                if (addedZones != undefined) {
                    $scope.hasState = true;
                    var countryIndex = UtilsService.getItemIndexByVal($scope.nodes, $scope.currentNode.nodeId, 'nodeId');
                    var countryNode = $scope.nodes[countryIndex];
                    for (var i = 0; i < addedZones.length; i++) {
                        var node = mapZoneToNode(addedZones[i]);
                        node.isOpened = true;
                        treeAPI.createNode(node);
                        countryNode.effectiveZones.push(node);
                        treeAPI.refreshTree($scope.nodes);
                    }

                }
            }

            function onZoneClosed(closedZone) {
                $scope.hasState = true;
                $scope.showRenameZone = false;

                var zoneNode = getCurrentZoneNode();

                zoneNode.DraftStatus = WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.value;

                var node = mapClosedZoneToNode(closedZone);
                treeAPI.createNode(node);

                setCodesFilterObject();
                return codesGridAPI.loadGrid(codesFilter);
            }

            function onZoneRenamed(renamedZone) {
                $scope.hasState = true;
                
                var zoneNode = getCurrentZoneNode();

                zoneNode.nodeName = renamedZone.NewZoneName;
                zoneNode.renamedZone = renamedZone.OldZoneName;

                var node = mapRenamedZoneToNode(renamedZone);
                treeAPI.createNode(node);

                $scope.currentNode.nodeName = renamedZone.NewZoneName;
                $scope.currentNode.renamedZone = renamedZone.OldZoneName;
                treeAPI.refreshTree($scope.nodes);
                setCodesFilterObject();

                return codesGridAPI.loadGrid(codesFilter);
            }


            function onCodeAdded(addedCodes) {
                if (addedCodes != undefined) {
                    $scope.showGrid = true;
                    $scope.hasState = true;
                    for (var i = 0; i < addedCodes.length; i++)
                        codesGridAPI.onCodeAdded(addedCodes[i]);

                    if ($scope.currentNode != undefined) {
                        if ($scope.currentNode.type == 'Zone') {
                            setCodesFilterObject();
                            return codesGridAPI.loadGrid(codesFilter);
                        }
                    }
                }

            }

            function onCodesMoved(movedCodes) {

                if (movedCodes != undefined) {
                    $scope.selectedCodes.length = 0;
                    $scope.showGrid = true;
                    $scope.hasState = true;
                    for (var i = 0; i < movedCodes.length; i++)
                        codesGridAPI.onCodeClosed(movedCodes[i]);

                    if ($scope.currentNode != undefined) {
                        if ($scope.currentNode.type == 'Zone') {
                            setCodesFilterObject();
                            return codesGridAPI.loadGrid(codesFilter);
                        }
                    }
                }

            }

            function onCodesClosed(closedCodes) {
                if (closedCodes != undefined) {
                    $scope.selectedCodes.length = 0;
                    $scope.showGrid = true;
                    $scope.hasState = true;
                    for (var i = 0; i < closedCodes.length; i++)
                        codesGridAPI.onCodeClosed(closedCodes[i]);

                    if ($scope.currentNode != undefined) {
                        if ($scope.currentNode.type == 'Zone') {
                            setCodesFilterObject();
                            return codesGridAPI.loadGrid(codesFilter);
                        }
                    }
                }
            }

            function addNewZone() {
                var parameters = {
                    CountryId: $scope.currentNode.nodeId,
                    CountryName: $scope.currentNode.nodeName,
                    SellingNumberPlanId: filter.sellingNumberPlanId
                };
                var settings = {};
                settings.onScopeReady = function (modalScope) {
                    modalScope.onZoneAdded = onZoneAdded;
                };

                VRModalService.showModal("/Client/Modules/WhS_CodePreparation/Views/Dialogs/NewZoneDialog.html", parameters, settings);
            }

            function addNewCode() {

                var parameters = {
                    ZoneId: $scope.currentNode.nodeId,
                    ZoneName: $scope.currentNode.renamedZone != undefined ? $scope.currentNode.renamedZone : $scope.currentNode.nodeName,
                    SellingNumberPlanId: filter.sellingNumberPlanId,
                    CountryId: $scope.currentNode.countryId,
                    ZoneStatus: $scope.currentNode.status
                };
                var settings = {};
                settings.onScopeReady = function (modalScope) {
                    modalScope.onCodeAdded = onCodeAdded;
                };

                VRModalService.showModal("/Client/Modules/WhS_CodePreparation/Views/Dialogs/NewCodeDialog.html", parameters, settings);
            }

            function moveCodes() {

                var codes = codesGridAPI.getSelectedCodes();
                var parameters = {
                    ZoneId: $scope.currentNode.nodeId,
                    ZoneName: $scope.currentNode.renamedZone != undefined ? $scope.currentNode.renamedZone : $scope.currentNode.nodeName,
                    currentZoneName: $scope.currentNode.nodeName,
                    SellingNumberPlanId: filter.sellingNumberPlanId,
                    CountryId: $scope.currentNode.countryId,
                    ZoneDataSource: GetCurrentCountryNodeZones(),
                    Codes: UtilsService.getPropValuesFromArray(codes, 'Code')
                };
                var settings = {};
                settings.onScopeReady = function (modalScope) {
                    modalScope.onCodesMoved = onCodesMoved;
                };

                VRModalService.showModal("/Client/Modules/WhS_CodePreparation/Views/Dialogs/MoveCodeDialog.html", parameters, settings);
            }

            function closeCodes() {

                var codes = codesGridAPI.getSelectedCodes();
                var parameters = {
                    ZoneId: $scope.currentNode.nodeId,
                    ZoneName: $scope.currentNode.renamedZone != undefined ? $scope.currentNode.renamedZone : $scope.currentNode.nodeName,
                    SellingNumberPlanId: filter.sellingNumberPlanId,
                    Codes: UtilsService.getPropValuesFromArray(codes, 'Code')
                };
                var settings = {};
                settings.onScopeReady = function (modalScope) {
                    modalScope.onCodesClosed = onCodesClosed;
                };

                VRModalService.showModal("/Client/Modules/WhS_CodePreparation/Views/Dialogs/CloseCodeDialog.html", parameters, settings);
            }


            function closeZone() {
                return VRNotificationService.showConfirmation("Are you sure you want to close " + $scope.currentNode.nodeName + " zone").then(function (result) {
                    if (result) {
                        var zoneInput = {
                            SellingNumberPlanId: filter.sellingNumberPlanId,
                            CountryId: $scope.currentNode.countryId,
                            ZoneId: $scope.currentNode.nodeId,
                            ZoneName: $scope.currentNode.renamedZone != undefined ? $scope.currentNode.renamedZone : $scope.currentNode.nodeName,
                        };
                        return WhS_CodePrep_CodePrepAPIService.CloseZone(zoneInput)
                         .then(function (response) {
                             if (response.Result == WhS_CP_NewCPOutputResultEnum.Existing.value) {
                                 VRNotificationService.showWarning(response.Message);
                             }
                             else if (response.Result == WhS_CP_NewCPOutputResultEnum.Inserted.value) {
                                 onZoneClosed(response.ClosedZone);
                                 VRNotificationService.showSuccess(response.Message);
                             }
                             else if (response.Result == WhS_CP_NewCPOutputResultEnum.Failed.value) {
                                 VRNotificationService.showError(response.Message);
                             }
                         }).catch(function (error) {
                             VRNotificationService.notifyException(error, $scope);
                             //});
                         }).finally(function () {

                         });

                    }
                });
            }

            function getCurrentZoneNode() {
                var countryIndex = UtilsService.getItemIndexByVal($scope.nodes, $scope.currentNode.countryId, 'nodeId');
                var countryNode = $scope.nodes[countryIndex];

                var zoneIndex = UtilsService.getItemIndexByVal(countryNode.effectiveZones, $scope.currentNode.nodeName, 'nodeName');

                return countryNode.effectiveZones[zoneIndex];
            }

            function GetCurrentCountryNodeZones() {
                var countryIndex = UtilsService.getItemIndexByVal($scope.nodes, $scope.currentNode.countryId, 'nodeId');
                var countryNode = $scope.nodes[countryIndex];
                return countryNode.effectiveZones;
            }

            function getZoneInfos(zoneNodes) {
                var zoneInfos = [];
                angular.forEach(zoneNodes, function (itm) {
                    zoneInfos.push(mapZoneInfoFromNode(itm));
                });

                return zoneInfos;
            }

            function mapZoneInfoFromNode(zoneItem) {
                return {
                    // SaleZoneId: zoneItem.nodeId,
                    Name: zoneItem.nodeName
                };
            }

            function getCountries() {
                countries.length = 0;
                return VRCommon_CountryAPIService.GetCountriesInfo().then(function (response) {
                    angular.forEach(response, function (itm) {
                        countries.push(itm);
                    });
                });
            }

            function checkState() {
                countries.length = 0;
                return WhS_CodePrep_CodePrepAPIService.CheckCodePreparationState(filter.sellingNumberPlanId).then(function (response) {
                    $scope.hasState = response;
                });
            }

            function buildCountriesTree() {
                $scope.nodes.length = 0;

                for (var i = 0; i < countries.length; i++) {
                    var node = mapCountryToNode(countries[i]);
                    $scope.nodes.push(node);
                }
                treeAPI.refreshTree($scope.nodes);

            }

            function mapCountryToNode(country) {
                return {
                    nodeId: country.CountryId,
                    nodeName: country.Name,
                    effectiveZones: [],
                    hasRemoteChildren: true,
                    type: 'Country'
                };
            }

            function mapZoneToNode(zoneInfo) {
                var icon = null;
                if ($scope.hasState) {
                    switch (zoneInfo.DraftStatus) {
                        case WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.value:
                            icon = WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.icon;
                            break;
                        case WhS_CP_ZoneItemDraftStatusEnum.New.value:
                            icon = WhS_CP_ZoneItemDraftStatusEnum.New.icon;
                            break;
                        case WhS_CP_ZoneItemDraftStatusEnum.Renamed.value:
                            icon = WhS_CP_ZoneItemDraftStatusEnum.Renamed.icon;
                            break;
                    }
                }

                if (zoneInfo.Status != null) {
                    switch (zoneInfo.Status) {
                        case WhS_CP_ZoneItemStatusEnum.PendingClosed.value:
                            icon = WhS_CP_ZoneItemStatusEnum.PendingClosed.icon;
                            break;
                        case WhS_CP_ZoneItemStatusEnum.PendingEffective.value:
                            icon = WhS_CP_ZoneItemStatusEnum.PendingEffective.icon;
                            break;
                    }
                }

                return {
                    nodeId: zoneInfo.ZoneId,
                    nodeName: zoneInfo.Name,
                    renamedZone: zoneInfo.RenamedZone,
                    hasRemoteChildren: false,
                    effectiveZones: [],
                    type: 'Zone',
                    status: zoneInfo.Status,
                    DraftStatus: zoneInfo.DraftStatus,
                    countryId: zoneInfo.CountryId,
                    icon: icon
                };
            }



            function mapRenamedZoneToNode(renamedZone) {

                return {
                    nodeId: renamedZone.ZoneId != null ? renamedZone.ZoneId : "generatedId_" + incrementalNodeId++,
                    nodeName: renamedZone.NewZoneName,
                    renamedZone: renamedZone.OldZoneName,
                    hasRemoteChildren: false,
                    effectiveZones: [],
                    type: 'Zone',
                    DraftStatus: renamedZone.ZoneId != null ? WhS_CP_ZoneItemDraftStatusEnum.Renamed.value : WhS_CP_ZoneItemDraftStatusEnum.New.value,
                    countryId: renamedZone.CountryId,
                    icon: renamedZone.ZoneId != null ? WhS_CP_ZoneItemDraftStatusEnum.Renamed.icon : WhS_CP_ZoneItemDraftStatusEnum.New.icon
                };
            }


            function mapClosedZoneToNode(closedZone) {
                return {
                    nodeId: closedZone.ZoneId,
                    nodeName: $scope.currentNode.nodeName,
                    hasRemoteChildren: false,
                    effectiveZones: [],
                    type: 'Zone',
                    DraftStatus: WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.value,
                    countryId: closedZone.CountryId,
                    icon: WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.icon
                };

            }

            //#endregion

            //#region Filters
            function setCodesFilterObject() {
                codesFilter = {
                    SellingNumberPlanId: filter.sellingNumberPlanId,
                    ZoneId: $scope.currentNode.nodeId,
                    ZoneName: $scope.currentNode.renamedZone != undefined ? $scope.currentNode.renamedZone : $scope.currentNode.nodeName,
                    RenamedZone: $scope.currentNode.renamedZone,
                    ZoneItemStatus: $scope.currentNode.status,
                    CountryId: $scope.currentNode.countryId,
                    ShowDraftStatus: $scope.hasState,
                    ShowSelectCode: $scope.currentNode.status != WhS_CP_ZoneItemStatusEnum.PendingClosed.value ? true : false
                };
            }

            function getFilter() {
                return {
                    sellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds()
                };
            }


            //#endregion

        };

        appControllers.controller('WhS_CP_CodePreparationManagementController', CodePreparationManagementController);

    })(appControllers);
