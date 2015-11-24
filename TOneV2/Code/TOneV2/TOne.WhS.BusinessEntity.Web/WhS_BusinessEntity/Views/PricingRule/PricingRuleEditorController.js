(function (appControllers) {

    "use strict";

    pricingRuleEditorController.$inject = ['$scope', 'WhS_BE_SalePricingRuleAPIService',  'UtilsService', 'VRNotificationService', 'VRNavigationService','WhS_Be_PricingRuleTypeEnum','WhS_Be_PricingTypeEnum','VRUIUtilsService','WhS_BE_PurchasePricingRuleAPIService','VRValidationService'];

    function pricingRuleEditorController($scope, WhS_BE_SalePricingRuleAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_Be_PricingRuleTypeEnum, WhS_Be_PricingTypeEnum, VRUIUtilsService, WhS_BE_PurchasePricingRuleAPIService, VRValidationService) {

        var isEditMode;
        var pricingRuleType;
        var pricingType;
        var ruleId;

        var pricingRuleTypeDirectiveAPI;
        var pricingRuleTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var criteriaDirectiveAPI;
        var criteriaReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var pricingRuleEntity;
        var service;
        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                pricingRuleType = parameters.PricingRuleType;
                pricingType = parameters.PricingType;
                ruleId = parameters.RuleId
            }
            isEditMode = (ruleId != undefined);
        }
        function defineScope() {
            $scope.scopeModal = {}
            $scope.scopeModal.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.scopeModal.beginEffectiveDate, $scope.scopeModal.endEffectiveDate);
            }
            $scope.scopeModal.onCriteriaDirectiveReady = function (api) {
                criteriaDirectiveAPI = api;
                criteriaReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.SavePricingRule = function () {
                if (isEditMode) {
                    return updatePricingRule();
                }
                else {
                    return insertPricingRule();
                }
            };
            $scope.scopeModal.onPricingRuleTypeDirectiveReady = function (api) {
                pricingRuleTypeDirectiveAPI = api;
                pricingRuleTypeReadyPromiseDeferred.resolve();
            }
            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.scopeModal.beginEffectiveDate = new Date();
            $scope.scopeModal.selectedPricingRuleType;
            $scope.scopeModal.selectedPricingType;
        }

        function load() {
            $scope.scopeModal.isLoading = true;

            definePricingRuleTypes();
            if (pricingType != undefined)
                for (var p in WhS_Be_PricingTypeEnum)
                    if (WhS_Be_PricingTypeEnum[p].value == pricingType)
                        $scope.scopeModal.selectedPricingType = WhS_Be_PricingTypeEnum[p];

            if ($scope.scopeModal.selectedPricingType.value == WhS_Be_PricingTypeEnum.Sale.value)
                service = WhS_BE_SalePricingRuleAPIService;
            else if ($scope.scopeModal.selectedPricingType.value == WhS_Be_PricingTypeEnum.Purchase.value)
                service = WhS_BE_PurchasePricingRuleAPIService;


            if (isEditMode) {
                getPricingRule().then(function () {
                    loadAllControls()
                        .finally(function () {
                            pricingRuleEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            else {
                loadAllControls();
                setDefaultValues();
            }

           
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection,loadPricingRuleTypeDirective, loadCriteriaDirective])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.scopeModal.isLoading = false;
               });
        }

        function loadPricingRuleTypeDirective() {

            var loadPricingRuleTypePromiseDeferred = UtilsService.createPromiseDeferred();

            pricingRuleTypeReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = (pricingRuleEntity != undefined) ? pricingRuleEntity.Settings : undefined

                    VRUIUtilsService.callDirectiveLoad(pricingRuleTypeDirectiveAPI, directivePayload, loadPricingRuleTypePromiseDeferred);
                });

            return loadPricingRuleTypePromiseDeferred.promise;
        }
        function loadCriteriaDirective() {

            var loadCriteriaPromiseDeferred = UtilsService.createPromiseDeferred();

            criteriaReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = (pricingRuleEntity != undefined) ? pricingRuleEntity.Criteria : undefined

                    VRUIUtilsService.callDirectiveLoad(criteriaDirectiveAPI, directivePayload, loadCriteriaPromiseDeferred);
                });

            return loadCriteriaPromiseDeferred.promise;
        }
        function setDefaultValues() {
            for (var p in WhS_Be_PricingRuleTypeEnum) {
                if (WhS_Be_PricingRuleTypeEnum[p].value == pricingRuleType) {
                    $scope.scopeModal.selectedPricingRuleType = WhS_Be_PricingRuleTypeEnum[p];
                }
            }
            $scope.title = UtilsService.buildTitleForAddEditor($scope.scopeModal.selectedPricingRuleType.title);
            
        }

        function definePricingRuleTypes() {
            $scope.scopeModal.pricingRuleTypes = [];
            for (var p in WhS_Be_PricingRuleTypeEnum)
                $scope.scopeModal.pricingRuleTypes.push(WhS_Be_PricingRuleTypeEnum[p]);
        }
        function getPricingRule() {
            return service.GetRule(ruleId).then(function (pricingRule) {
                pricingRuleEntity = pricingRule;
            });
        }



        function buildPricingRuleObjFromScope() {
             
            var settings = pricingRuleTypeDirectiveAPI.getData();

            settings.RuleType = $scope.scopeModal.selectedPricingRuleType.value;
            var criteria = criteriaDirectiveAPI.getData();
           
            criteria.CriteriaType = pricingType
            var pricingRule = {
                Settings: settings,
                Description: $scope.scopeModal.description,
                Criteria: criteria,
                BeginEffectiveTime: $scope.scopeModal.beginEffectiveDate,
                EndEffectiveTime: $scope.scopeModal.endEffectiveDate
            }
            return pricingRule;
        }

        function loadFilterBySection() {
            if (pricingRuleEntity != undefined)
            {
                $scope.scopeModal.beginEffectiveDate = pricingRuleEntity.BeginEffectiveTime;
                $scope.scopeModal.endEffectiveDate = pricingRuleEntity.EndEffectiveTime;
                $scope.scopeModal.description = pricingRuleEntity.Description;
                for (var p in WhS_Be_PricingRuleTypeEnum) {
                    if (WhS_Be_PricingRuleTypeEnum[p].value == pricingRuleEntity.Settings.RuleType) {
                        $scope.scopeModal.selectedPricingRuleType = WhS_Be_PricingRuleTypeEnum[p];
                    }
                }
                $scope.title = UtilsService.buildTitleForUpdateEditor($scope.scopeModal.selectedPricingRuleType.title);
            }
        }
        function insertPricingRule() {

            var pricingRuleObject = buildPricingRuleObjFromScope();
            return service.AddRule(pricingRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded($scope.scopeModal.selectedPricingRuleType.title, response)) {
                    if ($scope.onPricingRuleAdded != undefined)
                        $scope.onPricingRuleAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updatePricingRule() {
            var pricingRuleObject = buildPricingRuleObjFromScope();
            pricingRuleObject.RuleId = ruleId;
            service.UpdateRule(pricingRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated($scope.scopeModal.selectedPricingRuleType.title, response)) {
                    if ($scope.onPricingRuleUpdated != undefined)
                        $scope.onPricingRuleUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }


       
        
       

      


    }

    appControllers.controller('WhS_BE_PricingRuleEditorController', pricingRuleEditorController);
})(appControllers);
