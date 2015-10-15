(function (appControllers) {

    "use strict";

    pricingRuleEditorController.$inject = ['$scope', 'WhS_BE_SalePricingRuleAPIService',  'UtilsService', 'VRNotificationService', 'VRNavigationService','WhS_Be_PricingRuleTypeEnum'];

    function pricingRuleEditorController($scope, WhS_BE_SalePricingRuleAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_Be_PricingRuleTypeEnum) {

        var editMode;
        var pricingType;
        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                pricingType = parameters.PricingType;
            }
            console.log(parameters);
            editMode = (pricingType != undefined);
        }

        function defineScope() {

            $scope.PricingRule = function () {
                if (editMode) {
                    return updatePricingRule();
                }
                else {
                    return insertPricingRule();
                }
            };
            $scope.onPricingRuleTypeDirectiveReady = function (api) {
                console.log(api);
                api.load();
            }
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.selectedPricingRuleType;
        }

        function load() {
            definePricingRuleTypes();
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
            var pricingRule = {
            };

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
