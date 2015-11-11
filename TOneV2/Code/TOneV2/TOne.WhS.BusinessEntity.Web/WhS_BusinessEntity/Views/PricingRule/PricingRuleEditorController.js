(function (appControllers) {

    "use strict";

    pricingRuleEditorController.$inject = ['$scope', 'WhS_BE_SalePricingRuleAPIService',  'UtilsService', 'VRNotificationService', 'VRNavigationService','WhS_Be_PricingRuleTypeEnum','WhS_Be_PricingTypeEnum','VRUIUtilsService','WhS_BE_PurchasePricingRuleAPIService'];

    function pricingRuleEditorController($scope, WhS_BE_SalePricingRuleAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_Be_PricingRuleTypeEnum, WhS_Be_PricingTypeEnum, VRUIUtilsService, WhS_BE_PurchasePricingRuleAPIService) {

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
            $scope.onCriteriaDirectiveReady = function (api) {
                criteriaDirectiveAPI = api;
                criteriaReadyPromiseDeferred.resolve();
            }

            $scope.SavePricingRule = function () {
                if (isEditMode) {
                    return updatePricingRule();
                }
                else {
                    return insertPricingRule();
                }
            };
            $scope.onPricingRuleTypeDirectiveReady = function (api) {
                pricingRuleTypeDirectiveAPI = api;
                pricingRuleTypeReadyPromiseDeferred.resolve();
            }
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.beginEffectiveDate = new Date();
            $scope.selectedPricingRuleType;
            $scope.selectedPricingType;
        }

        function load() {
            $scope.isLoading = true;

            definePricingRuleTypes();
            if (pricingType != undefined)
                for (var p in WhS_Be_PricingTypeEnum)
                    if (WhS_Be_PricingTypeEnum[p].value == pricingType)
                        $scope.selectedPricingType = WhS_Be_PricingTypeEnum[p];

            if ($scope.selectedPricingType.value == WhS_Be_PricingTypeEnum.Sale.value)
                service = WhS_BE_SalePricingRuleAPIService;
            else if ($scope.selectedPricingType.value == WhS_Be_PricingTypeEnum.Purchase.value)
                service = WhS_BE_PurchasePricingRuleAPIService;


            if (isEditMode) {
                getPricingRule().then(function () {
                    loadAllControls()
                        .finally(function () {
                            pricingRuleEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
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
                   $scope.isLoading = false;
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
                    $scope.selectedPricingRuleType = WhS_Be_PricingRuleTypeEnum[p];
                }
            }
            $scope.title = UtilsService.buildTitleForAddEditor($scope.selectedPricingRuleType.title);
            
        }

        function definePricingRuleTypes() {
            $scope.pricingRuleTypes = [];
            for (var p in WhS_Be_PricingRuleTypeEnum)
                $scope.pricingRuleTypes.push(WhS_Be_PricingRuleTypeEnum[p]);
        }
        function getPricingRule() {
            return service.GetRule(ruleId).then(function (pricingRule) {
                pricingRuleEntity = pricingRule;
            });
        }



        function buildPricingRuleObjFromScope() {
             
            var settings = pricingRuleTypeDirectiveAPI.getData();

            settings.RuleType = $scope.selectedPricingRuleType.value;
            var criteria = criteriaDirectiveAPI.getData();
           
            criteria.CriteriaType = pricingType
            var pricingRule = {
                Settings: settings,
                Description: $scope.description,
                Criteria: criteria,
                BeginEffectiveTime: $scope.beginEffectiveDate,
                EndEffectiveTime: $scope.endEffectiveDate
            }
            return pricingRule;
        }

        function loadFilterBySection() {
            if (pricingRuleEntity != undefined)
            {
                $scope.beginEffectiveDate = pricingRuleEntity.BeginEffectiveTime;
                $scope.endEffectiveDate = pricingRuleEntity.EndEffectiveTime;
                $scope.description = pricingRuleEntity.Description;
                for (var p in WhS_Be_PricingRuleTypeEnum) {
                    if (WhS_Be_PricingRuleTypeEnum[p].value == pricingRuleEntity.Settings.RuleType) {
                        $scope.selectedPricingRuleType = WhS_Be_PricingRuleTypeEnum[p];
                    }
                }
                $scope.title = UtilsService.buildTitleForUpdateEditor($scope.selectedPricingRuleType.title);
            }
        }
        function insertPricingRule() {

            var pricingRuleObject = buildPricingRuleObjFromScope();
            return service.AddRule(pricingRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded($scope.selectedPricingRuleType.title, response)) {
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
                if (VRNotificationService.notifyOnItemUpdated($scope.selectedPricingRuleType.title, response)) {
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
