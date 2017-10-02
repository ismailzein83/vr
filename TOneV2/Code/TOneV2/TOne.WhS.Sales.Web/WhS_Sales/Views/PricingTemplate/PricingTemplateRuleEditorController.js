(function (appControllers) {

    'use strict';

    PricingTemplateRuleEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function PricingTemplateRuleEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var pricingTemplateRuleEntity;

        var pricingTemplateZoneFilterDirectiveAPI;
        var pricingTemplateZoneFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                pricingTemplateRuleEntity = parameters.pricingTemplateRule;
            }

            isEditMode = (pricingTemplateRuleEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onPricingTemplateZoneFilterDirectiveReady = function (api) {
                pricingTemplateZoneFilterDirectiveAPI = api;
                pricingTemplateZoneFilterDirectiveReadyDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadPricingTemplateZoneFilterDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((pricingTemplateRuleEntity != undefined) ? pricingTemplateRuleEntity.FieldName : null, 'Column') :
                UtilsService.buildTitleForAddEditor('Column');
        }
        function loadStaticData() {

            if (pricingTemplateRuleEntity == undefined)
                return;

            $scope.scopeModel.header = pricingTemplateRuleEntity.Header;
            $scope.scopeModel.IsAvailableInRoot = pricingTemplateRuleEntity.IsAvailableInRoot;
            $scope.scopeModel.IsAvailableInSubAccounts = pricingTemplateRuleEntity.IsAvailableInSubAccounts;
        }
        function loadPricingTemplateZoneFilterDirective() {
            var pricingTemplateZoneFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            pricingTemplateZoneFilterDirectiveReadyDeferred.promise.then(function () {

                var pricingTemplateZoneFilterPayload;
                if (pricingTemplateRuleEntity != undefined) {
                    pricingTemplateZoneFilterPayload = {
                        countries: pricingTemplateRuleEntity.Countries,
                        zones: pricingTemplateRuleEntity.Zones
                    };
                };
                VRUIUtilsService.callDirectiveLoad(pricingTemplateZoneFilterDirectiveAPI, pricingTemplateZoneFilterPayload, pricingTemplateZoneFilterDirectiveLoadDeferred);
            });

            return pricingTemplateZoneFilterDirectiveLoadDeferred.promise;
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

            var accountGenericFieldDefinitionSelectorObj = accountGenericFieldDefinitionSelectorAPI.getData();

            return {
                FieldName: accountGenericFieldDefinitionSelectorObj != undefined ? accountGenericFieldDefinitionSelectorObj.Name : undefined,
                Header: $scope.scopeModel.header,
                IsAvailableInRoot: $scope.scopeModel.IsAvailableInRoot,
                IsAvailableInSubAccounts: $scope.scopeModel.IsAvailableInSubAccounts,
                SubAccountsAvailabilityCondition: $scope.scopeModel.IsAvailableInSubAccounts == true ? accountConditionSelectiveAPI.getData() : null
            };
        }
    }

    appControllers.controller('WhS_Sales_PricingTemplateRuleEditorController', PricingTemplateRuleEditorController);

})(appControllers);