(function (appControllers) {

    'use strict';

    PricingTemplateRuleEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function PricingTemplateRuleEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var pricingTemplateRuleEntity;
        var context;

        var zoneFilterDirectiveAPI;
        var zoneFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var ratesDirectiveAPI;
        var ratesDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                pricingTemplateRuleEntity = parameters.pricingTemplateRule;
                context = parameters.context;
            }

            isEditMode = (pricingTemplateRuleEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onPricingTemplateZoneFilterDirectiveReady = function (api) {
                zoneFilterDirectiveAPI = api;
                zoneFilterDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onPricingTemplateRatesDirectiveReady = function (api) {
                ratesDirectiveAPI = api;
                ratesDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadZoneFilterDirective, loadRatesDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = (isEditMode) ? UtilsService.buildTitleForUpdateEditor('Princing Template Rule') : UtilsService.buildTitleForAddEditor('Princing Template Rule');
        }
        function loadZoneFilterDirective() {
            var zoneFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            zoneFilterDirectiveReadyDeferred.promise.then(function () {

                var zoneFilterDirectivePayload = { context: context };
                if (pricingTemplateRuleEntity != undefined) {
                    zoneFilterDirectivePayload.pricingTemplateRuleIndex = pricingTemplateRuleEntity.PricingTemplateRuleIndex;
                    zoneFilterDirectivePayload.countries = pricingTemplateRuleEntity.Countries;
                    zoneFilterDirectivePayload.countriesName = pricingTemplateRuleEntity.CountriesName;
                    zoneFilterDirectivePayload.zones = pricingTemplateRuleEntity.Zones;
                    zoneFilterDirectivePayload.zonesName = pricingTemplateRuleEntity.ZonesName;
                }
                VRUIUtilsService.callDirectiveLoad(zoneFilterDirectiveAPI, zoneFilterDirectivePayload, zoneFilterDirectiveLoadDeferred);
            });

            return zoneFilterDirectiveLoadDeferred.promise;
        }
        function loadRatesDirective() {
            var ratesDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            ratesDirectiveReadyDeferred.promise.then(function () {

                var ratesDirectivePayload;
                if (pricingTemplateRuleEntity != undefined) {
                    ratesDirectivePayload = { rates: pricingTemplateRuleEntity.Rates };
                }
                VRUIUtilsService.callDirectiveLoad(ratesDirectiveAPI, ratesDirectivePayload, ratesDirectiveLoadDeferred);
            });

            return ratesDirectiveLoadDeferred.promise;
        }

        function insert() {
            var pricingTemplateRuleObject = buildPricingTemplateRuleObjectFromScope();

            if ($scope.onPricingTemplateRuleAdded != undefined && typeof ($scope.onPricingTemplateRuleAdded) == 'function') {
                $scope.onPricingTemplateRuleAdded(pricingTemplateRuleObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var pricingTemplateRuleObject = buildPricingTemplateRuleObjectFromScope();

            if ($scope.onPricingTemplateRuleUpdated != undefined && typeof ($scope.onPricingTemplateRuleUpdated) == 'function') {
                $scope.onPricingTemplateRuleUpdated(pricingTemplateRuleObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildPricingTemplateRuleObjectFromScope() {

            var pricingTemplateZoneFilterObj = zoneFilterDirectiveAPI.getData();
            if (pricingTemplateZoneFilterObj == undefined)
                return;

            return {
                PricingTemplateRuleIndex: pricingTemplateZoneFilterObj.pricingTemplateRuleIndex,
                Countries: pricingTemplateZoneFilterObj.countries,
                CountriesName: pricingTemplateZoneFilterObj.countriesName,
                Zones: pricingTemplateZoneFilterObj.zones,
                ZonesName: pricingTemplateZoneFilterObj.zonesName,
                Rates: ratesDirectiveAPI.getData()
            };
        }
    }

    appControllers.controller('WhS_Sales_PricingTemplateRuleEditorController', PricingTemplateRuleEditorController);

})(appControllers);