
app.service('WhS_Sales_SellingRuleService', ['WhS_Sales_SellingRuleAPIService', 'VRModalService', 'VRNotificationService',
    function (WhS_Sales_SellingRuleAPIService, VRModalService, VRNotificationService) {

        return ({
            addSellingRule: addSellingRule,
            editSellingRule: editSellingRule,
            deleteSellingRule: deleteSellingRule
        });

        function addSellingRule(onSellingRuleAdded, routingProductId, sellingNumberPlanId) {
            var settings = {
            };

            var parameters = {
                routingProductId: routingProductId,
                sellingNumberPlanId: sellingNumberPlanId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSellingRuleAdded = onSellingRuleAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_Sales/Views/SellingRule/SellingRuleEditor.html', parameters, settings);
        }

        function editSellingRule(sellingRuleId, onSellingRuleUpdated) {
            var modalSettings = {
            };
            var parameters = {
                sellingRuleId: sellingRuleId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSellingRuleUpdated = onSellingRuleUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_Sales/Views/SellingRule/SellingRuleEditor.html', parameters, modalSettings);
        }

        function deleteSellingRule(scope, sellingRuleObj, onSellingRuleDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        return WhS_Sales_SellingRuleAPIService.DeleteRule(sellingRuleObj.Entity.RuleId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Selling Rule", deletionResponse);
                                onSellingRuleDeleted(sellingRuleObj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, scope);
                            });
                    }
                });
        }

    }]);
