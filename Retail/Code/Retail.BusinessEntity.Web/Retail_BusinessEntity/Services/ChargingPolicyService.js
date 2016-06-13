(function (appControllers) {

    'use stict';

    ChargingPolicyService.$inject = ['VRModalService', 'VRNotificationService'];

    function ChargingPolicyService(VRModalService, VRNotificationService)
    {
        function addChargingPolicy(onChargingPolicyAdded)
        {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onChargingPolicyAdded = onChargingPolicyAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/ChargingPolicy/ChargingPolicyEditor.html', null, settings);
        };

        function editChargingPolicy(chargingPolicyId, onChargingPolicyUpdated)
        {
            var parameters = {
                chargingPolicyId: chargingPolicyId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onChargingPolicyUpdated = onChargingPolicyUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/ChargingPolicy/ChargingPolicyEditor.html', parameters, settings);
        };

        return {
            addChargingPolicy: addChargingPolicy,
            editChargingPolicy: editChargingPolicy
        };
    }

    appControllers.service('Retail_BE_ChargingPolicyService', ChargingPolicyService);

})(appControllers);