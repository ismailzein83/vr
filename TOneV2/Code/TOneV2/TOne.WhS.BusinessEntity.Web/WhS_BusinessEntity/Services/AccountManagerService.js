
app.service('WhS_BE_AccountManagerService', ['VRModalService', 'VRNotificationService', 'UtilsService',
    function (VRModalService, VRNotificationService, UtilsService) {

        function openOrgChartsModal(onOrgChartAssigned, assignedOrgChartId) {
            var settings = {};
            var parameters = null;

            if (assignedOrgChartId != 0) {
                parameters = {
                    assignedOrgChartId: assignedOrgChartId
                };
            }
            settings.onScopeReady = function (modalScope) {
                modalScope.title = 'Assign Org Chart';
                modalScope.onOrgChartAssigned = onOrgChartAssigned;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/AccountManager/OrgChartAssignmentEditor.html', parameters, settings);
        };
        function assignCarriers(onCarriersAssigned, nodeId) {
            var settings = {};

            var parameters = {
                selectedAccountManagerId: nodeId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = 'Assign Carriers';
                modalScope.onCarriersAssigned = onCarriersAssigned
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/AccountManager/CarrierAssignmentEditor.html', parameters, settings);
        };

        return ({
            openOrgChartsModal: openOrgChartsModal,
            assignCarriers: assignCarriers
        });


    }]);