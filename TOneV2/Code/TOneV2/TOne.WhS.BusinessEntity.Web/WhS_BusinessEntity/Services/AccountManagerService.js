(function (appControllers) {

    'use strict';

    AccountManagerService.$inject = ['VRModalService'];

    function AccountManagerService(VRModalService) {
        return ({
            assignOrgChart: assignOrgChart,
            assignCarriers: assignCarriers
        });

        function assignOrgChart(assignedOrgChartId, onOrgChartAssigned) {
            var modalParameters = {
                assignedOrgChartId: assignedOrgChartId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onOrgChartAssigned = onOrgChartAssigned;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/AccountManager/OrgChartAssignmentEditor.html', modalParameters, modalSettings);
        }

        function assignCarriers(accountManagerId, onCarriersAssigned) {
            var modalParameters = {
                selectedAccountManagerId: accountManagerId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCarriersAssigned = onCarriersAssigned;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/AccountManager/CarrierAssignmentEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('WhS_BE_AccountManagerService', AccountManagerService);

})(appControllers);
