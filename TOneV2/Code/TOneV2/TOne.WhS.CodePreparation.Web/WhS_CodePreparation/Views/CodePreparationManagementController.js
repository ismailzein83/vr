(function (appControllers) {

    'use strict';

    CodePreparationManagementController.$inject = ['$scope', 'WhS_CodePrep_CodePrepAPIService', 'WhS_BP_CreateProcessResultEnum', 'VRUIUtilsService', 'UtilsService', 'VRCommon_CountryAPIService', 'WhS_BE_SaleZoneAPIService', 'VRModalService', 'VRNotificationService'];

    function CodePreparationManagementController($scope, WhS_CodePrep_CodePrepAPIService, WhS_BP_CreateProcessResultEnum, VRUIUtilsService, UtilsService, VRCommon_CountryAPIService, WhS_BE_SaleZoneAPIService, VRModalService, VRNotificationService) {

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
                    if ($scope.currentNode.type == 'Zone') {
                        checkState();
                        codesGridAPI.clearUpdatedItems();
                        setCodesFilterObject();
                        $scope.showGrid = true;
                        // codesGridAPI.clearUpdatedItems();
                        return codesGridAPI.loadGrid(codesFilter);
                    }
                    $scope.showGrid = false;
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

            $scope.onSellingNumberPlanSelectorChanged = function (selectedPlan) {

                if (selectedPlan != undefined && !$scope.isLoading) {
                    countries.length = 0;

                    filter = getFilter();
                    $scope.isLoading = true;

                    UtilsService.waitMultipleAsyncOperations([getCountries, checkState]).then(function () {
                        buildCountriesTree();
                        $scope.currentNode = undefined;
                        $scope.isLoading = false;
                    }).catch(function (error) {
                        $scope.isLoading = false;
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
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

            $scope.closeCodesClicked = function () {
                closeCodes();
            }

            $scope.cancelState = function () {
                return VRNotificationService.showConfirmation().then(function (result) {
                    if (result) {
                        countries.length = 0;
                        return WhS_CodePrep_CodePrepAPIService.CancelCodePreparationState(filter.sellingNumberPlanId).then(function (response) {
                            $scope.hasState = !response;
                            treeAPI.refreshTree($scope.nodes);
                            $scope.currentNode = undefined;
                        });
                    }
                });

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
                $scope.hasState = true;
                for (var i = 0; i < addedZones.length; i++) {
                    var node = mapZoneToNode(addedZones[i]);
                    treeAPI.createNode(node);
                    for (var i = 0; i < $scope.nodes.length; i++) {
                        if ($scope.nodes[i].nodeId == $scope.currentNode.nodeId) {
                            $scope.nodes[i].effectiveZones.push(node);
                        }

                    }
                }


            }
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
                    codesGridAPI.onCodeAdded(movedCodes[i]);

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
                    codesGridAPI.onCodeAdded(closedCodes[i]);

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
                ZoneName: $scope.currentNode.nodeName,
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
                ZoneName: $scope.currentNode.nodeName,
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
                ZoneName: $scope.currentNode.nodeName,
                SellingNumberPlanId: filter.sellingNumberPlanId,
                Codes: UtilsService.getPropValuesFromArray(codes, 'Code')
            };
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onCodesClosed = onCodesClosed;
            };

            VRModalService.showModal("/Client/Modules/WhS_CodePreparation/Views/Dialogs/CloseCodeDialog.html", parameters, settings);
        }

        function GetCurrentCountryNodeZones() {
            var countryIndex = UtilsService.getItemIndexByVal($scope.nodes, $scope.currentNode.countryId, 'nodeId');
            var countryNode = $scope.nodes[countryIndex];
            return getZoneInfos(countryNode.effectiveZones);
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

            return {
                nodeId: zoneInfo.ZoneId,
                nodeName: zoneInfo.Name,
                hasRemoteChildren: false,
                effectiveZones: [],
                type: 'Zone',
                status: zoneInfo.Status,
                countryId: zoneInfo.CountryId
            };
        }

        //#endregion

        //#region Filters
        function setCodesFilterObject() {
            codesFilter = {
                SellingNumberPlanId: filter.sellingNumberPlanId,
                ZoneId: $scope.currentNode.nodeId,
                ZoneName: $scope.currentNode.nodeName,
                ZoneItemStatus: $scope.currentNode.status,
                CountryId: $scope.currentNode.countryId
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
