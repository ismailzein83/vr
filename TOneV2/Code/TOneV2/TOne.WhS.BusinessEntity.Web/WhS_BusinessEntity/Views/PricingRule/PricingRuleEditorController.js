(function (appControllers) {

    "use strict";

    pricingRuleEditorController.$inject = ['$scope', 'WhS_BE_SalePricingRuleAPIService',  'UtilsService', 'VRNotificationService', 'VRNavigationService','WhS_Be_PricingRuleTypeEnum','WhS_Be_PricingTypeEnum','VRUIUtilsService','WhS_BE_PurchasePricingRuleAPIService'];

    function pricingRuleEditorController($scope, WhS_BE_SalePricingRuleAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_Be_PricingRuleTypeEnum, WhS_Be_PricingTypeEnum, VRUIUtilsService, WhS_BE_PurchasePricingRuleAPIService) {

        var editMode;
        var pricingRuleType;
        var pricingType;
        var ruleId;
        var pricingRuleTypeDirectiveAPI;
        var criteriaDirectiveAPI;
        var directiveAppendixData;
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

            editMode = (ruleId != undefined);
        }
        function defineScope() {
            $scope.onCriteriaDirectiveReady = function (api) {
                criteriaDirectiveAPI = api;
                if (directiveAppendixData != undefined) {
                    tryLoadAppendixDirectives();
                }
                else {
                    $scope.criteriaAppendixLoader;
                    VRUIUtilsService.loadDirective($scope, criteriaDirectiveAPI, $scope.criteriaAppendixLoader);
                }
            }

            $scope.SavePricingRule = function () {
                if (editMode) {
                    return updatePricingRule();
                }
                else {
                    return insertPricingRule();
                }
            };
            $scope.onPricingRuleTypeDirectiveReady = function (api) {
                pricingRuleTypeDirectiveAPI = api;
                if (directiveAppendixData != undefined) {
                    tryLoadAppendixDirectives();
                }
                else {
                    $scope.pricingRuleTypeAppendixLoader;
                    VRUIUtilsService.loadDirective($scope, pricingRuleTypeDirectiveAPI, $scope.pricingRuleTypeAppendixLoader);
                }
            }
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.selectedPricingRuleType;
            $scope.selectedPricingType;
        }

        function load() {
            $scope.isGettingData = true;
            definePricingRuleTypes();
            if (editMode)
            {
                getPricingRule();
            }
            else
            {
                $scope.isGettingData = false;
                setDefaultValues();
            }
           
        }
        function setDefaultValues() {
            for (var p in WhS_Be_PricingRuleTypeEnum) {
                if (WhS_Be_PricingRuleTypeEnum[p].value == pricingRuleType) {
                    $scope.selectedPricingRuleType = WhS_Be_PricingRuleTypeEnum[p];
                }
            }
            $scope.title = UtilsService.buildTitleForAddEditor($scope.selectedPricingRuleType.title);
            if (pricingType != undefined)
                for (var p in WhS_Be_PricingTypeEnum)
                    if (WhS_Be_PricingTypeEnum[p].value == pricingType)
                        $scope.selectedPricingType = WhS_Be_PricingTypeEnum[p];
        }

        function definePricingRuleTypes() {
            $scope.pricingRuleTypes = [];
            for (var p in WhS_Be_PricingRuleTypeEnum)
                $scope.pricingRuleTypes.push(WhS_Be_PricingRuleTypeEnum[p]);
        }

        function getPricingRule() {
            return WhS_BE_SalePricingRuleAPIService.GetRule(ruleId).then(function (pricingRule) {
                directiveAppendixData = pricingRule;
                fillScopeFromPricingRuleObj(pricingRule);
                tryLoadAppendixDirectives();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });
        }

        function tryLoadAppendixDirectives() {
            var loadOperations = [];
            var setDirectivesDataOperations = [];
            if ($scope.selectedPricingRuleType != undefined) {
                if (pricingRuleTypeDirectiveAPI == undefined)
                    return;
                loadOperations.push(pricingRuleTypeDirectiveAPI.load);

                setDirectivesDataOperations.push(setPricingRuleTypeDirective);
            }
            if ($scope.selectedPricingType != undefined) {
                if (criteriaDirectiveAPI == undefined)
                    return;

                loadOperations.push(criteriaDirectiveAPI.load);

                setDirectivesDataOperations.push(setCriteriaDirective);
            }
            UtilsService.waitMultipleAsyncOperations(loadOperations).then(function () {

                setAppendixDirectives();

            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });

            function setAppendixDirectives() {
                UtilsService.waitMultipleAsyncOperations(setDirectivesDataOperations).then(function () {

                    directiveAppendixData = undefined;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.isGettingData = false;
                });
            }

            function setPricingRuleTypeDirective() {
                return pricingRuleTypeDirectiveAPI.setData(directiveAppendixData.Settings);
            }

            function setCriteriaDirective() {
                return criteriaDirectiveAPI.setData(directiveAppendixData.Criteria);
            }
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

        function fillScopeFromPricingRuleObj(pricingRuleObj) {
           
            $scope.beginEffectiveDate = pricingRuleObj.BeginEffectiveTime;
            $scope.endEffectiveDate = pricingRuleObj.EndEffectiveTime;
            $scope.description = pricingRuleObj.Description;
            for (var p in WhS_Be_PricingRuleTypeEnum) {
                if (WhS_Be_PricingRuleTypeEnum[p].value == pricingRuleObj.Settings.RuleType) {
                    $scope.selectedPricingRuleType = WhS_Be_PricingRuleTypeEnum[p];
                }
            }
         
           for (var p in WhS_Be_PricingTypeEnum)
               if (WhS_Be_PricingTypeEnum[p].value == pricingRuleObj.Criteria.CriteriaType)
               {
                   
                   $scope.selectedPricingType = WhS_Be_PricingTypeEnum[p];
               }
           $scope.title = UtilsService.buildTitleForUpdateEditor($scope.selectedPricingRuleType.title);
        }

        function insertPricingRule() {

            var pricingRuleObject = buildPricingRuleObjFromScope();
            if ($scope.selectedPricingType.value == WhS_Be_PricingTypeEnum.Sale.value) {
                return WhS_BE_SalePricingRuleAPIService.AddRule(pricingRuleObject)
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
            else if ($scope.selectedPricingType.value == WhS_Be_PricingTypeEnum.Purchase.value) {
                return WhS_BE_PurchasePricingRuleAPIService.AddRule(pricingRuleObject)
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
            

        }

        function updatePricingRule() {
            var pricingRuleObject = buildPricingRuleObjFromScope();
            pricingRuleObject.RuleId = ruleId;
            WhS_BE_SalePricingRuleAPIService.UpdateRule(pricingRuleObject)
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
