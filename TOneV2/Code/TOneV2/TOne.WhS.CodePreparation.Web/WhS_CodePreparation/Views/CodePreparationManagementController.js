(function (appControllers) {

    'use strict';

    CodePreparationManagementController.$inject = ['$scope', 'WhS_CodePrep_CodePrepAPIService', 'WhS_BP_CreateProcessResultEnum', 'VRUIUtilsService', 'UtilsService', 'VRCommon_CountryAPIService', 'WhS_BE_SaleZoneAPIService', 'VRModalService', 'VRNotificationService', 'WhS_CP_NewCPOutputResultEnum', 'WhS_CP_ZoneItemDraftStatusEnum', 'WhS_CP_ZoneItemStatusEnum', 'WhS_CP_ValidationOutput', 'WhS_CodePrep_CodePrepService'];

    function CodePreparationManagementController($scope, WhS_CodePrep_CodePrepAPIService, WhS_BP_CreateProcessResultEnum, VRUIUtilsService, UtilsService, VRCommon_CountryAPIService, WhS_BE_SaleZoneAPIService, VRModalService, VRNotificationService, WhS_CP_NewCPOutputResultEnum, WhS_CP_ZoneItemDraftStatusEnum, WhS_CP_ZoneItemStatusEnum, WhS_CP_ValidationOutput, WhS_CodePrep_CodePrepService) {

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

            showState(false);
            showSaleCodes(false);

            $scope.selectedCodes = [];

            $scope.applyCodePreparationState = function () {
                var onCodePreparationStateApplied = function () {
                };
                return WhS_CodePrep_CodePrepAPIService.ApplyCodePreparationState(filter.sellingNumberPlanId, onCodePreparationStateApplied);
            }

            $scope.uploadCodePreparation = function () {
                var onCodePreparationUpdated = function () {
                };
                return WhS_CodePrep_CodePrepAPIService.UploadCodePreparationSheet(filter.sellingNumberPlanId, onCodePreparationUpdated);
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

                        showRenameZone($scope.currentNode.DraftStatus, $scope.currentNode.status);
                        showEnd($scope.currentNode.DraftStatus, $scope.currentNode.status);
                        showAddCode($scope.currentNode.DraftStatus, $scope.currentNode.status);

                        showSaleCodes(true);
                        showAddZone(false);

                        return codesGridAPI.loadGrid(codesFilter);
                    }

                    clear(false);
                    showAddZone(true);
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
                return ($scope.selectedCodes.length > 0) ? closeCodes() : closeZone();
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

      

        function loadSellingNumberPlans() {
            var loadSNPPromiseDeferred = UtilsService.createPromiseDeferred();
            sellingNumberPlanReadyPromiseDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, undefined, loadSNPPromiseDeferred);
            });

            return loadSNPPromiseDeferred.promise;
        }

        function onZoneAdded(addedZones) {
            if (addedZones != undefined) {
                showState(true);
                var countryIndex = UtilsService.getItemIndexByVal($scope.nodes, $scope.currentNode.nodeId, 'nodeId');
                var countryNode = $scope.nodes[countryIndex];
                for (var i = 0; i < addedZones.length; i++) {
                    var node = mapZoneToNode(addedZones[i]);
                    node.isOpened = true;
                    treeAPI.createNode(node);
                    countryNode.effectiveZones.push(node);
                }
                treeAPI.refreshTree($scope.nodes);

            }
        }

        function onZoneClosed() {
            showState(true);

            var zoneNode = getCurrentZoneNode();

            zoneNode.DraftStatus = WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.value;

            showRenameZone(zoneNode.DraftStatus, zoneNode.Status);
            showEnd(zoneNode.DraftStatus, zoneNode.Status);
            showAddCode(zoneNode.DraftStatus, zoneNode.Status);

            var node = mapClosedZoneToNode();
            treeAPI.createNode(node);

            setCodesFilterObject();
            return codesGridAPI.loadGrid(codesFilter);
        }

        function onZoneRenamed(renamedZone) {
            showState(true);

            var zoneNode = getCurrentZoneNode();

            zoneNode.nodeName = renamedZone.NewZoneName;
            zoneNode.originalZoneName = renamedZone.OriginalZoneName;
            zoneNode.DraftStatus = renamedZone.ZoneId != null ? WhS_CP_ZoneItemDraftStatusEnum.Renamed.value : WhS_CP_ZoneItemDraftStatusEnum.New.value;
            zoneNode.icon = renamedZone.ZoneId != null ? WhS_CP_ZoneItemDraftStatusEnum.Renamed.icon : WhS_CP_ZoneItemDraftStatusEnum.New.icon;

            showRenameZone(zoneNode.DraftStatus, zoneNode.Status);
            showEnd(zoneNode.DraftStatus, zoneNode.Status);
            showAddCode(zoneNode.DraftStatus, zoneNode.Status);

            var node = mapRenamedZoneToNode(renamedZone);
            treeAPI.createNode(node);

            $scope.currentNode.nodeName = renamedZone.NewZoneName;
            $scope.currentNode.originalZoneName = renamedZone.OriginalZoneName;
            treeAPI.refreshTree($scope.nodes);
            setCodesFilterObject();

            return codesGridAPI.loadGrid(codesFilter);
        }


        function onCodeAdded(addedCodes) {
            if (addedCodes != undefined) {
                showSaleCodes(true);
                showState(true);
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
                showSaleCodes(true);
                showState(true);
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
                showSaleCodes(true);
                showState(true);
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
                ZoneName: $scope.currentNode.originalZoneName != undefined ? $scope.currentNode.originalZoneName : $scope.currentNode.nodeName,
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
                ZoneName: $scope.currentNode.originalZoneName != undefined ? $scope.currentNode.originalZoneName : $scope.currentNode.nodeName,
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
                ZoneName: $scope.currentNode.originalZoneName != undefined ? $scope.currentNode.originalZoneName : $scope.currentNode.nodeName,
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
            VRNotificationService.showConfirmation("Are you sure you want to close " + $scope.currentNode.nodeName + " zone").then(function (result) {
                if (result) {
                    $scope.isLoadingFilter = true;
                    var zoneInput = {
                        SellingNumberPlanId: filter.sellingNumberPlanId,
                        CountryId: $scope.currentNode.countryId,
                        ZoneId: $scope.currentNode.nodeId,
                        ZoneName: $scope.currentNode.originalZoneName != undefined ? $scope.currentNode.originalZoneName : $scope.currentNode.nodeName,
                    };
                    return WhS_CodePrep_CodePrepAPIService.CloseZone(zoneInput)
                     .then(function (response) {
                         if (response.Result == WhS_CP_ValidationOutput.Success.value) {
                             onZoneClosed();
                             VRNotificationService.showSuccess(response.Message);
                         }
                         else if (response.Result == WhS_CP_ValidationOutput.ValidationError.value) {
                             WhS_CodePrep_CodePrepService.NotifyValidationWarning(response.Message);
                         }
                     }).catch(function (error) {
                         VRNotificationService.notifyException(error, $scope);
                     })
                    .finally(function () {
                        $scope.isLoadingFilter = false;
                    });

                }
            });
        }

        function clear(action) {
            showSaleCodes(action);
            $scope.showAddNewCode = action;
            showAddZone(action);
            $scope.showRenameZone = action;
            $scope.showEnd = action;
        }

        function showRenameZone(draftStatus, status) {
            if (status != null)
                $scope.showRenameZone = false;
            else
                $scope.showRenameZone = draftStatus != WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.value;
        }

        function showEnd(draftStatus, status) {
            if (status != null)
                $scope.showEnd = false;
            else
                $scope.showEnd = draftStatus != WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.value && draftStatus != WhS_CP_ZoneItemDraftStatusEnum.New.value && (draftStatus != WhS_CP_ZoneItemDraftStatusEnum.Renamed.value || $scope.selectedCodes.length > 0);
        }

        function showAddCode(draftStatus, status) {
            if (status != null)
                $scope.showAddNewCode = status != WhS_CP_ZoneItemStatusEnum.PendingClosed.value;
            else
                $scope.showAddNewCode = draftStatus != WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.value;
        }

        function showAddZone(action) {
            $scope.showAddNewZone = action;
        }

        function showSaleCodes(action) {
            $scope.showGrid = action;
        }

        function showState(action) {
            $scope.hasState = action;
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
                originalZoneName: zoneInfo.OriginalZoneName,
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
                originalZoneName: renamedZone.OriginalZoneName,
                hasRemoteChildren: false,
                effectiveZones: [],
                type: 'Zone',
                DraftStatus: renamedZone.ZoneId != null ? WhS_CP_ZoneItemDraftStatusEnum.Renamed.value : WhS_CP_ZoneItemDraftStatusEnum.New.value,
                countryId: renamedZone.CountryId,
                icon: renamedZone.ZoneId != null ? WhS_CP_ZoneItemDraftStatusEnum.Renamed.icon : WhS_CP_ZoneItemDraftStatusEnum.New.icon
            };
        }

        function mapClosedZoneToNode() {
            return {
                nodeId: $scope.currentNode.nodeId,
                nodeName: $scope.currentNode.nodeName,
                hasRemoteChildren: false,
                effectiveZones: [],
                type: 'Zone',
                DraftStatus: WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.value,
                countryId: $scope.currentNode.countryId,
                icon: WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.icon
            };

        }

        //#endregion

        //#region Filters
        function setCodesFilterObject() {
            codesFilter = {
                SellingNumberPlanId: filter.sellingNumberPlanId,
                ZoneId: $scope.currentNode.nodeId,
                ZoneName: $scope.currentNode.originalZoneName != undefined ? $scope.currentNode.originalZoneName : $scope.currentNode.nodeName,
                OriginalZoneName: $scope.currentNode.originalZoneName,
                ZoneItemStatus: $scope.currentNode.status,
                CountryId: $scope.currentNode.countryId,
                ShowDraftStatus: $scope.hasState,
                ShowSelectCode: $scope.currentNode.status != WhS_CP_ZoneItemStatusEnum.PendingClosed.value
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
