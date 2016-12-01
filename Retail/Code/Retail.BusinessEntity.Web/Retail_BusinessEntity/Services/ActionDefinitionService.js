(function (appControllers) {

    'use stict';

    ActionDefinitionService.$inject = ['VRModalService','Retail_BE_EntityTypeEnum'];

    function ActionDefinitionService(VRModalService, Retail_BE_EntityTypeEnum) {

        function addActionDefinition(onActionDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onActionDefinitionAdded = onActionDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Action/Definition/ActionDefinitionEditor.html', null, settings);
        };

        function editActionDefinition(actionDefinitionId, onActionDefinitionUpdated) {
            var modalSettings = {
            };

            var parameters = {
                actionDefinitionId: actionDefinitionId,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onActionDefinitionUpdated = onActionDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Action/Definition/ActionDefinitionEditor.html', parameters, modalSettings);
        }

        function getEntityId(entityType,actionEntityId)
        {
            var entityId = "Retail_BE";
            switch(entityType)
            {
                case Retail_BE_EntityTypeEnum.Account.value:
                    entityId += "_" + Retail_BE_EntityTypeEnum.Account.name + "_" + actionEntityId;
                    break;
                case Retail_BE_EntityTypeEnum.AccountService.value:
                    entityId += "_" + Retail_BE_EntityTypeEnum.AccountService.name + "_" + actionEntityId;
                    break;
            }
            return entityId;
        }

        return {
            addActionDefinition: addActionDefinition,
            editActionDefinition: editActionDefinition,
            getEntityId: getEntityId
        };
    }

    appControllers.service('Retail_BE_ActionDefinitionService', ActionDefinitionService);

})(appControllers);