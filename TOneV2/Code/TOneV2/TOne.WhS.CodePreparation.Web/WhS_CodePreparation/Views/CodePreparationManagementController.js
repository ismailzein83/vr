CodePreparationManagementController.$inject = ['$scope', 'WhS_CodePrep_CodePrepAPIService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcessService', 'VRUIUtilsService', 'UtilsService', 'VRCommon_CountryAPIService', 'WhS_BE_SaleZoneAPIService', 'VRModalService', 'VRNotificationService'];

function CodePreparationManagementController($scope, WhS_CodePrep_CodePrepAPIService, WhS_BP_CreateProcessResultEnum, BusinessProcessService, VRUIUtilsService, UtilsService, VRCommon_CountryAPIService, WhS_BE_SaleZoneAPIService, VRModalService, VRNotificationService) {
    var sellingNumberPlanDirectiveAPI;
    var sellingNumberPlanReadyPromiseDeferred;
    var treeAPI;
    var codesGridAPI;
    var currencyDirectiveAPI;
    var currencyReadyPromiseDeferred;
    var countries = [];
    var filter;
    var codesFilter;
    defineScope();
    loadParameters();
    load();
    function loadParameters() {
    }
    function defineScope() {
        $scope.nodes = [];
        $scope.sellingNumberPlans = [];
        $scope.selectedSellingNumberPlan;
        $scope.zoneList;
        $scope.currentNode;

        $scope.onSellingNumberPlanSelectorReady = function (api) {
            sellingNumberPlanDirectiveAPI = api;
            var setLoader = function (value) { $scope.isLoadingSellingNumberPlan = value };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sellingNumberPlanDirectiveAPI, undefined, setLoader, sellingNumberPlanReadyPromiseDeferred);
        }

        $scope.countriesTreeReady = function (api) {
            treeAPI = api;
        }

        $scope.countriesTreeValueChanged = function () {
            if ($scope.currentNode != undefined) {
                if ($scope.currentNode.type == 'Zone') {
                    console.log($scope.currentNode);
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
            treeAPI.refreshTree($scope.nodes);
        }

        $scope.searchClicked = function () {
            filter = getFilter();
            treeAPI.refreshTree($scope.nodes);
        };

        $scope.saleCodesGridReady = function (api) {
            codesGridAPI = api;
        }

        $scope.newZoneClicked = function () {
            console.log('New Zone Button Clicked');
            addNewZone();
        }
        $scope.newCodeClicked = function () {
            console.log('New Code Button Clicked');
            addNewCode();
        }
    }
    function onZoneAdded(addedZone) {
        if (addedZone != undefined) {
            console.log(addedZone);
            addZoneNode(addedZone);
        }
    }

    function onCodeAdded(addedCode) {
        if (addedCode != undefined) {
            console.log(addedCode);
            codesGridAPI.onCodeAdded(addedCode);
        }
    }

    function addNewZone() {
        var parameters = {
            CountryId: $scope.currentNode.nodeId,
            ZoneId: undefined,
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
            ZoneStatus: $scope.currentNode.status
        };
        var settings = {};
        settings.onScopeReady = function (modalScope) {
            modalScope.onCodeAdded = onCodeAdded;
        };

        VRModalService.showModal("/Client/Modules/WhS_CodePreparation/Views/Dialogs/NewCodeDialog.html", parameters, settings);
    }
    function load() {
        $scope.isLoading = true;

        UtilsService.waitMultipleAsyncOperations([getCountries]).then(function () {
            buildCountriesTree();
        }).catch(function (error) {
            $scope.isLoading = false;
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
    }
    function getFilter() {
        return {
            sellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds()
        };
    }
    function getCountries() {
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
        $scope.isLoading = false;
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
            status: zoneInfo.Status
        };
    }

    function addZoneNode(zoneItem) {
        var node = {
            nodeId: 0,
            nodeName: zoneItem.Name,
            hasRemoteChildren: false,
            effectiveZones: [],
            type: 'Zone',
            status: zoneItem.Status
        };
        treeAPI.createNode(node);
        //treeAPI.refreshTree($scope.nodes);
    }

    function setCodesFilterObject() {
        //var zones = [];
        //zones.push($scope.currentNode.nodeId);
        codesFilter = {
            SellingNumberPlanId: filter.sellingNumberPlanId,
            ZoneId: $scope.currentNode.nodeId,
            ZoneName: $scope.currentNode.nodeName,
            ZoneItemStatus: $scope.currentNode.status
        };
    }
};



appControllers.controller('WhS_CodePreparation_CodePreparationManagementController', CodePreparationManagementController);