CodePreparationManagementController.$inject = ['$scope', 'WhS_CodePrep_CodePrepAPIService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcessService', 'VRUIUtilsService', 'UtilsService', 'VRCommon_CountryAPIService', 'WhS_BE_SaleZoneAPIService', 'VRModalService', 'VRNotificationService'];

function CodePreparationManagementController($scope, WhS_CodePrep_CodePrepAPIService, WhS_BP_CreateProcessResultEnum, BusinessProcessService, VRUIUtilsService, UtilsService, VRCommon_CountryAPIService, WhS_BE_SaleZoneAPIService, VRModalService, VRNotificationService) {

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
        $scope.effectiveDate = new Date();
        $scope.zoneList;
        $scope.currentNode;

        $scope.selectedCodes = [];

        $scope.applyCodePreparationForEntities = function()
        {
            return WhS_CodePrep_CodePrepAPIService.ApplyCodePreparationForEntities($scope.selectedSellingNumberPlan.SellingNumberPlanId, {}, {}, false).then(function (response) {
                if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value)
                    return BusinessProcessService.openProcessTracking(response.ProcessInstanceId);
            });
        }

        $scope.uploadCodePreparationEntities = function () {
            return WhS_CodePrep_CodePrepAPIService.ApplyCodePreparationForEntities($scope.selectedSellingNumberPlan.SellingNumberPlanId, $scope.zoneList.fileId, $scope.effectiveDate, true).then(function (response) {
                if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value)
                    return BusinessProcessService.openProcessTracking(response.ProcessInstanceId);
            });
        }

        $scope.downloadTemplate = function () {
            return WhS_CodePrep_CodePrepAPIService.DownloadImportCodePreparationTemplate().then(function (response) {
                UtilsService.downloadFile(response.data, response.headers);
            });
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
                    setCodesFilterObject();
                    return codesGridAPI.loadGrid(codesFilter);
                }
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

            countries.length = 0;

            $scope.isLoading = true;

            UtilsService.waitMultipleAsyncOperations([getCountries], [buildCountriesTree]).then(function () {
                $scope.isLoading = false;
            }).catch(function (error) {
                $scope.isLoading = false;
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        $scope.searchClicked = function () {
            filter = getFilter();
            $scope.isLoading = true;

            UtilsService.waitMultipleAsyncOperations([getCountries]).then(function () {
                buildCountriesTree();
                $scope.isLoading = false;
            }).catch(function (error) {
                $scope.isLoading = false;
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
            treeAPI.refreshTree($scope.nodes);
        };

        $scope.saleCodesGridReady = function (api) {
            codesGridAPI = api;
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
    }

    function loadParameters() {
    }

    function load() {
        $scope.isLoading = true;

        UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlans]).then(function () {
            $scope.isLoading = false;
        }).catch(function (error) {
            $scope.isLoading = false;
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
    }

    function loadSellingNumberPlans() {
        var loadSNPPromiseDeferred = UtilsService.createPromiseDeferred();
        sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
            var payload = {};
            VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, payload, loadSNPPromiseDeferred);
        });

        return loadSNPPromiseDeferred.promise;
    }

    function onZoneAdded(addedZones) {
        if (addedZones != undefined) {
            for (var i = 0; i < addedZones.length; i++)
            {
                var node = mapZoneToNode(addedZones[i]);
                treeAPI.createNode(node);
            }
            
        }
    }

    function onCodeAdded(addedCodes) {

        if (addedCodes != undefined) {
            for (var i = 0; i < addedCodes.length;i++)
                codesGridAPI.onCodeAdded(addedCodes[i]);

            if ($scope.currentNode != undefined) {
                if ($scope.currentNode.type == 'Zone') {
                    setCodesFilterObject();
                    return codesGridAPI.loadGrid(codesFilter);
                }
            }
        }

    }

    function onCodesMoved(response) {
        console.log(response);
    }

    function onCodesClosed(response) {
        console.log(response);
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
            SaleZoneId: zoneItem.nodeId,
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

    function buildCountriesTree() {
        $scope.nodes = [];

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
appControllers.controller('WhS_CodePreparation_CodePreparationManagementController', CodePreparationManagementController);