app.service('WhS_BE_SalePricingRuleService', ['WhS_BE_SalePricingRuleAPIService', 'WhS_Be_PricingTypeEnum', 'VRModalService', 'VRNotificationService', 'UtilsService',
    function (WhS_BE_SalePricingRuleAPIService,WhS_Be_PricingTypeEnum, VRModalService, VRNotificationService, UtilsService) {

        function addSalePricingRule(onSalePricingRuleAdded, type) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPricingRuleAdded = onSalePricingRuleAdded;
            };
            var parameters = {
                PricingType: WhS_Be_PricingTypeEnum.Sale.value
            };
            if (type != undefined) {
                parameters.PricingRuleType = type;
            }



            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PricingRule/PricingRuleEditor.html', parameters, settings);
        }

        function editSalePricingRule(obj, onSalePricingRuleUpdated) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPricingRuleUpdated = onSalePricingRuleUpdated;
            };
            var parameters = {
                RuleId: obj.RuleId,
                PricingType: obj.PricingType
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PricingRule/PricingRuleEditor.html', parameters, settings);
        }

        function deleteSalePricingRule($scope, salePricingRuleObj, onSalePricingRuleDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        return WhS_BE_SalePricingRuleAPIService.DeleteRule(salePricingRuleObj.Entity.RuleId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Sale Pricing Rule", deletionResponse);
                                onSalePricingRuleDeleted(salePricingRuleObj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            });
                    }
                });
        }

        return ({
            addSalePricingRule: addSalePricingRule,
            editSalePricingRule: editSalePricingRule,
            deleteSalePricingRule: deleteSalePricingRule
        });

    }]);