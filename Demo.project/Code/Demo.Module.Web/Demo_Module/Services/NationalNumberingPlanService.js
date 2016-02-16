(function (appControllers) {

    'use stict';

    NationalNumberingPlanService.$inject = ['VRModalService'];

    function NationalNumberingPlanService(VRModalService) {
        return ({
            addNationalNumberingPlan: addNationalNumberingPlan,
            editNationalNumberingPlan: editNationalNumberingPlan
        });

        function addNationalNumberingPlan(onNationalNumberingPlanAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {

                modalScope.onNationalNumberingPlanAdded = onNationalNumberingPlanAdded;
            };

            VRModalService.showModal('/Client/Modules/Demo_Module/Views/NationalNumberingPlan/NationalNumberingPlanEditor.html', null, settings);
        }

        function editNationalNumberingPlan(nationalNumberingPlanObj, onNationalNumberingPlanUpdated) {
            var modalSettings = {
            };

            var parameters = {
                NationalNumberingPlanId: nationalNumberingPlanObj.Entity.NationalNumberingPlanId,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onNationalNumberingPlanUpdated = onNationalNumberingPlanUpdated;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Views/NationalNumberingPlan/NationalNumberingPlanEditor.html', parameters, modalSettings);
        }
    }

    appControllers.service('Demo_NationalNumberingPlanService', NationalNumberingPlanService);

})(appControllers);
