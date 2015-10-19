(function (appControllers) {

    "use strict";

    pricingRuleEditorController.$inject = ['$scope', 'WhS_BE_SalePricingRuleAPIService',  'UtilsService', 'VRNotificationService', 'VRNavigationService','WhS_Be_PricingRuleTypeEnum','WhS_BE_CarrierAccountAPIService','WhS_BE_SaleZoneAPIService','WhS_Be_PricingTypeEnum'];

    function pricingRuleEditorController($scope, WhS_BE_SalePricingRuleAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_Be_PricingRuleTypeEnum, WhS_BE_CarrierAccountAPIService, WhS_BE_SaleZoneAPIService, WhS_Be_PricingTypeEnum) {

        var editMode;
        var pricingType;

        var pricingRuleTypeDirectiveAPI;
        var criteriaDirectiveAPI;
        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                pricingType = parameters.PricingType;
            }
            console.log(parameters);
            editMode = (pricingType == undefined);
        }

        function defineScope() {
            $scope.onCriteriaDirectiveReady = function (api) {
                console.log(api);
                criteriaDirectiveAPI = api;
                load();
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
                api.load();
            }
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.selectedPricingRuleType;

           

            $scope.isCustomerShown = function () {
                if (pricingType.value == WhS_Be_PricingTypeEnum.Sale.value)
                    return true;
                return false;
            }

        }

        function load() {
            $scope.isGettingData = true;
            if (criteriaDirectiveAPI == undefined)
                return;
            //criteriaDirectiveAPI.load().catch(function (error) {
            //    VRNotificationService.notifyException(error, $scope);
            //    $scope.isGettingData = false;
            //}).finally(function () {

            //    $scope.isGettingData = false;
            //});
          //  $scope.isGettingData = true;
            definePricingRuleTypes();
            $scope.isGettingData = false;
        }

        function definePricingRuleTypes() {
            $scope.pricingRuleTypes = [];
            for (var p in WhS_Be_PricingRuleTypeEnum)
                $scope.pricingRuleTypes.push(WhS_Be_PricingRuleTypeEnum[p]);
        }
        function getPricingRule() {
            return WhS_BE_SalePricingRuleAPIService.GetRule(pricingRuleId).then(function (pricingRule) {
                fillScopeFromPricingRuleObj(pricingRule);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });
        }


        function buildPricingRuleObjFromScope() {
            var settings = pricingRuleTypeDirectiveAPI.getData();
            settings.RuleType = $scope.selectedPricingRuleType.value;
            var criteria=criteriaDirectiveAPI.getData();
            criteria.CriteriaType=pricingType.value
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
        }

        function insertPricingRule() {

            var pricingRuleObject = buildPricingRuleObjFromScope();
            return WhS_BE_SalePricingRuleAPIService.AddRule(pricingRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Pricing Rule", response)) {
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
            WhS_BE_SalePricingRuleAPIService.UpdateRule(routeRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Pricing Rule", response)) {
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
