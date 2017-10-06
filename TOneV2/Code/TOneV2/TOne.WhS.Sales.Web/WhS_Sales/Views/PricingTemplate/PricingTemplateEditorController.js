(function (appControllers) {

    "use strict";

    PricingTemplateEditorController.$inject = ['$scope', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'WhS_Sales_PricingTemplateAPIService'];

    function PricingTemplateEditorController($scope, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService, WhS_Sales_PricingTemplateAPIService) {

        var isEditMode;

        var pricingTemplateId;
        var pricingTemplateEntity;
        var pricingTemplateRulesEditorRuntime;

        var currencyDirectiveAPI;
        var currencyDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingNumberPlanSelectorAPI;
        var sellingNumberPlanSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var sellingNumberPlanSelectionChangedDeferred = UtilsService.createPromiseDeferred();

        var pricingTemplateRuleManagementDirectiveAPI;
        var pricingTemplateRuleManagementDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                pricingTemplateId = parameters.pricingTemplateId;
            }

            isEditMode = (pricingTemplateId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onCurrencySelectorReady = function (api) {
                currencyDirectiveAPI = api;
                currencyDirectiveReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onSellingNumberPlanSelectorReady = function (api) {
                sellingNumberPlanSelectorAPI = api;
                sellingNumberPlanSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onPricingTemplateRuleManagementDirectiveReady = function (api) {
                pricingTemplateRuleManagementDirectiveAPI = api;
                pricingTemplateRuleManagementDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onBeforeSellingNumberPlanSelectionChanged = function () {
                if ($scope.scopeModel.selectedSellingNumberPlan == undefined)
                    return true;

                if (pricingTemplateRuleManagementDirectiveAPI == undefined || pricingTemplateRuleManagementDirectiveAPI.getData() == undefined)
                    return true;

                return VRNotificationService.showConfirmation("Settings Data will be deleted. Are you sure you want to continue?").then(function (response) {
                    return response;
                });
            };

            $scope.scopeModel.onSellingNumberPlanSelectionChanged = function (selectdedSNP) {

                if (selectdedSNP != undefined) {
                    var pricingTemplateRuleManagementDirectivePayload = { context: buildPricingTemplateRulesContext() };

                    var setLoader = function (value) {
                        setTimeout(function () { $scope.scopeModel.isPricingTemplateRuleGridLoading = value; });
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, pricingTemplateRuleManagementDirectiveAPI, pricingTemplateRuleManagementDirectivePayload, setLoader, sellingNumberPlanSelectionChangedDeferred);
                }
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getPricingTemplate().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getPricingTemplate() {
            return WhS_Sales_PricingTemplateAPIService.GetPricingTemplateEditorRuntime(pricingTemplateId).then(function (response) {
                if (response != undefined) {
                    pricingTemplateEntity = response.Entity;
                    pricingTemplateRulesEditorRuntime = response.RulesEditorRuntime;
                }
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCurrencySelector, loadSellingNumberPlanSelector, loadPricingTemplateRuleManagementDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            if (isEditMode) {
                var pricingTemplateName = (pricingTemplateEntity != undefined) ? pricingTemplateEntity.Name : null;
                $scope.title = UtilsService.buildTitleForUpdateEditor(pricingTemplateName, 'PricingTemplate');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('PricingTemplate');
            }
        }
        function loadStaticData() {
            if (pricingTemplateEntity == undefined)
                return;

            $scope.scopeModel.name = pricingTemplateEntity.Name;
            $scope.scopeModel.description = pricingTemplateEntity.Settings != undefined ? pricingTemplateEntity.Settings.Description : undefined;
        }
        function loadCurrencySelector() {
            var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var currencyPayload;
            if (pricingTemplateEntity != undefined && pricingTemplateEntity.Settings != undefined) {
                currencyPayload = { selectedIds: pricingTemplateEntity.Settings.CurrencyId };
            }
            else {
                currencyPayload = { selectSystemCurrency: true };
            }

            currencyDirectiveReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, currencyPayload, loadCurrencySelectorPromiseDeferred);

            });

            return loadCurrencySelectorPromiseDeferred.promise;
        }
        function loadSellingNumberPlanSelector() {
            var sellingNumberPlanSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            sellingNumberPlanSelectorReadyPromiseDeferred.promise.then(function () {

                var sellingNumberPlanSelectorPayload;
                if (pricingTemplateEntity != undefined) {
                    sellingNumberPlanSelectorPayload = { selectedIds: pricingTemplateEntity.SellingNumberPlanId };
                }
                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanSelectorAPI, sellingNumberPlanSelectorPayload, sellingNumberPlanSelectorLoadPromiseDeferred);
            });

            return sellingNumberPlanSelectorLoadPromiseDeferred.promise;
        }
        function loadPricingTemplateRuleManagementDirective() {
            if (!isEditMode)
                sellingNumberPlanSelectionChangedDeferred.resolve();

            var pricingTemplateRuleManagementDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultiplePromises([pricingTemplateRuleManagementDirectiveReadyDeferred.promise, sellingNumberPlanSelectionChangedDeferred.promise]).then(function () {
                sellingNumberPlanSelectionChangedDeferred = undefined;

                var pricingTemplateRuleManagementDirectivePayload = { context: buildPricingTemplateRulesContext() };
                if (pricingTemplateEntity != undefined) {
                    pricingTemplateRuleManagementDirectivePayload.pricingTemplateRules = pricingTemplateEntity.Settings.Rules;
                    pricingTemplateRuleManagementDirectivePayload.pricingTemplateRulesEditorRuntime = pricingTemplateRulesEditorRuntime;
                }
                VRUIUtilsService.callDirectiveLoad(pricingTemplateRuleManagementDirectiveAPI, pricingTemplateRuleManagementDirectivePayload, pricingTemplateRuleManagementDirectiveLoadDeferred);
            });

            return pricingTemplateRuleManagementDirectiveLoadDeferred.promise;
        }

        function insert() {
            $scope.scopeModel.isLoading = true;

            return WhS_Sales_PricingTemplateAPIService.AddPricingTemplate(buildPricingTemplateObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('PricingTemplate', response, 'Name')) {
                    if ($scope.onPricingTemplateAdded != undefined)
                        $scope.onPricingTemplateAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;

            return WhS_Sales_PricingTemplateAPIService.UpdatePricingTemplate(buildPricingTemplateObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('PricingTemplate', response, 'Name')) {
                    if ($scope.onPricingTemplateUpdated != undefined) {
                        $scope.onPricingTemplateUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildPricingTemplateObjFromScope() {

            return {
                PricingTemplateId: pricingTemplateEntity != undefined ? pricingTemplateEntity.PricingTemplateId : undefined,
                Name: $scope.scopeModel.name,
                SellingNumberPlanId: sellingNumberPlanSelectorAPI.getSelectedIds(),
                Settings: {
                    CurrencyId: currencyDirectiveAPI.getSelectedIds(),
                    Description: $scope.scopeModel.description,
                    Rules: pricingTemplateRuleManagementDirectiveAPI.getData()
                }
            };
        }
        function buildPricingTemplateRulesContext() {
            return {
                getSellingNumberPlan: function () {
                    return $scope.scopeModel.selectedSellingNumberPlan ? $scope.scopeModel.selectedSellingNumberPlan.SellingNumberPlanId : undefined;
                }
            };
        }
    }

    appControllers.controller('WhS_Sales_PricingTemplateEditorController', PricingTemplateEditorController);

})(appControllers);