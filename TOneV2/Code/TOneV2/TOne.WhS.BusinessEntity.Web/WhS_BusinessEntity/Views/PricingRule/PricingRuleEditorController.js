(function (appControllers) {

    "use strict";

    pricingRuleEditorController.$inject = ['$scope', 'WhS_BE_SalePricingRuleAPIService',  'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function pricingRuleEditorController($scope, WhS_BE_SalePricingRuleAPIService, UtilsService, VRNotificationService, VRNavigationService) {

        var editMode;  
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
            }
            editMode = (parameters != undefined);
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
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
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
