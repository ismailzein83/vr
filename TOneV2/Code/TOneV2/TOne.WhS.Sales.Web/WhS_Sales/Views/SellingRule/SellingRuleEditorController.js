(function (appControllers) {

    "use strict";

    sellingRuleEditorController.$inject = ["$scope", "WhS_Sales_SellingRuleAPIService", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService", "VRDateTimeService"];

    function sellingRuleEditorController($scope, WhS_Sales_SellingRuleAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VRDateTimeService) {

        var isEditMode;

        var sellingRuleId;
        var sellingProductId;
        var sellingNumberPlanId;

        var saleZoneGroupSettingsAPI;
        var saleZoneGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var customerGroupSettingsAPI;
        var customerGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingRuleSettingsAPI;
        var sellingRuleSettingsReadyPromiseDeferred;

        var sellingRuleEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {

                sellingRuleId = parameters.sellingRuleId;
                sellingProductId = parameters.sellingProductId;
                sellingNumberPlanId = parameters.sellingNumberPlanId;
            }
            isEditMode = (sellingRuleId != undefined);
        }

        function defineScope() {

            $scope.hasSaveRulePermission = function () {
                if (isEditMode)
                    return WhS_Sales_SellingRuleAPIService.HasUpdateRulePermission();
                else
                    return WhS_Sales_SellingRuleAPIService.HasAddRulePermission();
            };

            $scope.scopeModel = {};
            $scope.scopeModel.saleZoneGroupTemplates = [];
            $scope.scopeModel.customerGroupTemplates = [];
            $scope.scopeModel.sellingRuleSettingsTemplates = [];

            $scope.scopeModel.beginEffectiveDate = VRDateTimeService.getNowDateTime();
            $scope.scopeModel.endEffectiveDate = undefined;


            $scope.scopeModel.onSaleZoneGroupSettingsDirectiveReady = function (api) {
                saleZoneGroupSettingsAPI = api;
                saleZoneGroupSettingsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onCustomerGroupSettingsDirectiveReady = function (api) {
                customerGroupSettingsAPI = api;
                customerGroupSettingsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onSellingRuleSettingsDirectiveReady = function (api) {
                sellingRuleSettingsAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingSellingRuleSettings = value };

                var sellingRuleSettingsPayload = {
                    FilterSettings: { SellingProductId: sellingProductId }
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sellingRuleSettingsAPI, sellingRuleSettingsPayload, setLoader, sellingRuleSettingsReadyPromiseDeferred);
            };



            $scope.scopeModel.SaveSellingRule = function () {
                if (isEditMode) {
                    return updateSellingRule();
                }
                else {
                    return insertSellingRule();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                $scope.title = "Edit Selling Rule";
                getSellingRule().then(function () {
                    loadAllControls()
                        .finally(function () {
                            sellingRuleEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                $scope.title = "New Selling Rule";
                loadAllControls();
            }
        }

        function getSellingRule() {
            return WhS_Sales_SellingRuleAPIService.GetRule(sellingRuleId).then(function (sellingRule) {
                sellingRuleEntity = sellingRule;
                sellingProductId = sellingRuleEntity.Criteria != null ? sellingRuleEntity.Criteria.SellingProductId : undefined;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([editScopeTitle, loadSaleZoneGroupSection, loadCustomerGroupSection, loadSellingRuleSettingsSection, loadStaticSection])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        }
        function editScopeTitle() {
            if (sellingProductId == undefined)
                return;

            return WhS_BE_SellingProductAPIService.GetSellingProduct(sellingProductId).then(function (response) {
                $scope.title += " of " + response.Name;
            });
        }

        function loadFilterBySection() {

            $scope.scopeModel.sellingRuleCriteriaTypes = UtilsService.getArrayEnum(WhS_Sales_SellingRuleCriteriaTypeEnum);
            $scope.scopeModel.selectedSellingRuleCriteriaType = UtilsService.getEnum(WhS_Sales_SellingRuleCriteriaTypeEnum, "value", WhS_Sales_SellingRuleCriteriaTypeEnum.SaleZone.value);
        }

        function loadSaleZoneGroupSection() {
            var promises = [];

            if (saleZoneGroupSettingsReadyPromiseDeferred == undefined)
                saleZoneGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleZoneGroupSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            promises.push(saleZoneGroupSettingsLoadPromiseDeferred.promise);

            saleZoneGroupSettingsReadyPromiseDeferred.promise.then(function () {
                var saleZoneGroupPayload = {
                    sellingNumberPlanId: sellingNumberPlanId != undefined ? sellingNumberPlanId : undefined,
                    saleZoneFilterSettings: { SellingProductId: sellingProductId },
                };

                var saleZoneGroupPayload;

                if (sellingRuleEntity != undefined) {
                    saleZoneGroupPayload.sellingNumberPlanId = sellingRuleEntity.Criteria.SaleZoneGroupSettings != undefined ? sellingRuleEntity.Criteria.SaleZoneGroupSettings.SellingNumberPlanId : undefined;
                    saleZoneGroupPayload.saleZoneGroupSettings = sellingRuleEntity.Criteria.SaleZoneGroupSettings
                }

                saleZoneGroupSettingsReadyPromiseDeferred = undefined;
                VRUIUtilsService.callDirectiveLoad(saleZoneGroupSettingsAPI, saleZoneGroupPayload, saleZoneGroupSettingsLoadPromiseDeferred);
            });
            return UtilsService.waitMultiplePromises(promises);
        }


        function loadCustomerGroupSection() {
            var promises = [];
            if (customerGroupSettingsReadyPromiseDeferred == undefined)
                customerGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var customerGroupSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            promises.push(customerGroupSettingsLoadPromiseDeferred.promise);

            customerGroupSettingsReadyPromiseDeferred.promise.then(function () {
                var customerGroupPayload = {};
                if (sellingRuleEntity != undefined && sellingRuleEntity.Criteria.CustomerGroupSettings != null)
                    customerGroupPayload.customerGroupSettings = sellingRuleEntity.Criteria.CustomerGroupSettings;

                customerGroupSettingsReadyPromiseDeferred = undefined;
                VRUIUtilsService.callDirectiveLoad(customerGroupSettingsAPI, customerGroupPayload, customerGroupSettingsLoadPromiseDeferred);
            });


            return UtilsService.waitMultiplePromises(promises);
        }

        function loadSellingRuleSettingsSection() {
            var promises = [];
            var sellingRuleSettingsPayload;

            if (sellingRuleEntity != undefined && sellingRuleEntity.Settings != null) {
                sellingRuleSettingsPayload = {
                    //SupplierFilterSettings: { SellingProductId: 1 },
                    SellingRuleSettings: sellingRuleEntity.Settings
                }
            }

            var loadSellingRuleSettingsTemplatesPromise = WhS_Sales_SellingRuleAPIService.GetSellingRuleSettingsTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.scopeModel.sellingRuleSettingsTemplates.push(item);
                });

                if (sellingRuleSettingsPayload != undefined)
                    $scope.scopeModel.selectedSellingRuleSettingsTemplate = UtilsService.getItemByVal($scope.scopeModel.sellingRuleSettingsTemplates, sellingRuleSettingsPayload.SellingRuleSettings.ConfigId, "ExtensionConfigurationId");
            });

            promises.push(loadSellingRuleSettingsTemplatesPromise);

            if (sellingRuleSettingsPayload != undefined) {
                sellingRuleSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                var sellingRuleSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(sellingRuleSettingsLoadPromiseDeferred.promise);

                sellingRuleSettingsReadyPromiseDeferred.promise.then(function () {
                    sellingRuleSettingsReadyPromiseDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(sellingRuleSettingsAPI, sellingRuleSettingsPayload, sellingRuleSettingsLoadPromiseDeferred);
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function loadStaticSection() {
            if (sellingRuleEntity == undefined)
                return;

            $scope.scopeModel.beginEffectiveDate = sellingRuleEntity.BeginEffectiveTime;
            $scope.scopeModel.endEffectiveDate = sellingRuleEntity.EndEffectiveTime;

        }

        function buildSellingRuleObjFromScope() {

            var sellingRule = {
                RuleId: (sellingRuleId != null) ? sellingRuleId : 0,
                Criteria: {
                    SellingProductId: sellingProductId,
                    SaleZoneGroupSettings: saleZoneGroupSettingsAPI.getData(),
                    CustomerGroupSettings: customerGroupSettingsAPI.getData()
                },
                Settings: VRUIUtilsService.getSettingsFromDirective($scope.scopeModel, sellingRuleSettingsAPI, "selectedSellingRuleSettingsTemplate"),
                BeginEffectiveTime: $scope.scopeModel.beginEffectiveDate,
                EndEffectiveTime: $scope.scopeModel.endEffectiveDate
            };

            return sellingRule;
        }

        function insertSellingRule() {
            var sellingRuleObject = buildSellingRuleObjFromScope();
            return WhS_Sales_SellingRuleAPIService.AddRule(sellingRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Selling Rule", response)) {
                    if ($scope.onSellingRuleAdded != undefined)
                        $scope.onSellingRuleAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateSellingRule() {
            var sellingRuleObject = buildSellingRuleObjFromScope();
            WhS_Sales_SellingRuleAPIService.UpdateRule(sellingRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Selling Rule", response)) {
                    if ($scope.onSellingRuleUpdated != undefined)
                        $scope.onSellingRuleUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }


    }
    appControllers.controller("WhS_Sales_SellingRuleEditorController", sellingRuleEditorController);
})(appControllers);