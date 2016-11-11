'use strict';
app.directive('vrWhsBePurchasepricingrulecriteria', ['UtilsService', '$compile', 'WhS_BE_CarrierAccountAPIService', 'VRUIUtilsService', 'VRNotificationService',
function (UtilsService, $compile, WhS_BE_CarrierAccountAPIService, VRUIUtilsService, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new bePurchasePricingRuleCriteria(ctrl, $scope, $attrs);
            ctor.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SupplierZone/Templates/PurchasePricingRuleCriteriaTemplate.html"
    };


    function bePurchasePricingRuleCriteria(ctrl, $scope, $attrs) {

        var supplierSelectorAPI;
        var directiveReadyAPI;
        var directiveReadyPromiseDeferred;

        function initializeController() {
            $scope.suppliersWithZonesStettingsTemplates = [];

            $scope.onSupplierSelectorReady = function (api) {
                supplierSelectorAPI = api;
                defineAPI();
            };

            $scope.onDirectiveReady = function (api) {
                directiveReadyAPI = api;
                var setLoader = function (value) { $scope.isLoadingDirective = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveReadyAPI, undefined, setLoader, directiveReadyPromiseDeferred);
            };

            $scope.onSelectionChanged = function () {
                if (carrierAccountDirectiveAPI != undefined)
                    $scope.suppliers = carrierAccountDirectiveAPI.getData();
            };
        }
        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {};
                if ($scope.selectedSuppliersWithZonesStettingsTemplate != undefined) {
                    obj = {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.SuppliersWithZonesGroups.SelectiveSuppliersWithZonesGroup,TOne.WhS.BusinessEntity.MainExtensions",
                        SuppliersWithZones: directiveReadyAPI.getData(),
                        ConfigId: $scope.selectedSuppliersWithZonesStettingsTemplate.ExtensionConfigurationId
                    }
                }
                return obj;
            };

            api.load = function (payload) {
                supplierSelectorAPI.clearDataSource();

                var suppliersWithZones;
                var configId;

                if (payload != undefined && payload.SuppliersWithZonesGroupSettings != null) {
                    suppliersWithZones = payload.SuppliersWithZonesGroupSettings.SuppliersWithZones;
                    configId = payload.SuppliersWithZonesGroupSettings.ConfigId;
                }
                var promises = [];
                var loadTemplatesPromise = WhS_BE_CarrierAccountAPIService.GetSuppliersWithZonesGroupsTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.suppliersWithZonesStettingsTemplates.push(item);
                    });

                    if (configId != undefined)
                        $scope.selectedSuppliersWithZonesStettingsTemplate = UtilsService.getItemByVal($scope.suppliersWithZonesStettingsTemplates, configId, "ExtensionConfigurationId");
                });
                promises.push(loadTemplatesPromise);
                if (suppliersWithZones != undefined) {
                    directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(directiveLoadPromiseDeferred.promise);

                    directiveReadyPromiseDeferred.promise.then(function () {
                        directiveReadyPromiseDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(directiveReadyAPI, suppliersWithZones, directiveLoadPromiseDeferred);
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);