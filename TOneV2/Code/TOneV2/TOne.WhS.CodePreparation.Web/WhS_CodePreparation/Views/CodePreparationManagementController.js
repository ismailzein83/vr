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
        $scope.zoneList;
        $scope.currentNode;

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

    function onZoneAdded(addedZone) {
        if (addedZone != undefined) {
            var node = mapZoneToNode(addedZone);
            treeAPI.createNode(node);
        }
    }

    function onCodeAdded(addedCode) {
        if (addedCode != undefined) {
            codesGridAPI.onCodeAdded(addedCode);
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