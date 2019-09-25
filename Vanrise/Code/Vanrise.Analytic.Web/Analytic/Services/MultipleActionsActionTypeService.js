'use strict';

app.service('VR_Analytic_MultipleActionsActionTypeService', ['VRModalService',
    function (VRModalService) {

        function addAction(actionTypeTemplateConfig, onActionAdded) {

            var parameters = {
                actionTypeTemplateConfig: actionTypeTemplateConfig
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onActionAdded = onActionAdded;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/ActionType/ActionTypeEditor.html', parameters, settings);
        };

        function editAction(actionTypeEntity, actionTypeTemplateConfig, onActionUpdated) {

            var parameters = {
                actionTypeTemplateConfig: actionTypeTemplateConfig,
                actionTypeEntity: actionTypeEntity
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onActionUpdated = onActionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/ActionType/ActionTypeEditor.html', parameters, settings);
        };

        return {
            addAction: addAction,
            editAction: editAction
        };
    }]);