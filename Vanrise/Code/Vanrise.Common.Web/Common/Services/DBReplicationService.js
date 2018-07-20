(function (appControllers) {

    "use strict";

    DBReplicationService.$inject = ['VRModalService'];

    function DBReplicationService(VRModalService) {

        function addDBReplicationSetting(onDBReplicationSettingAdded, dbReplicationDefinitionId, selectedDBDefinitions) {

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onDBReplicationSettingAdded = onDBReplicationSettingAdded;
            };

            var parameters = {
                dbReplicationDefinitionId: dbReplicationDefinitionId,
                selectedDBDefinitions: selectedDBDefinitions
            };

            VRModalService.showModal('/Client/Modules/Common/Directives/DBReplication/DBReplicationSettings/Templates/DBReplicationSettingsTemplate.html', parameters, settings);
        }

        function editDBReplicationSetting(onDBReplicationSettingUpdated, dbReplicationSettingEntity, dbReplicationDefinitionId, selectedDBDefinitions) {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onDBReplicationSettingUpdated = onDBReplicationSettingUpdated;
            };

            var parameters = {
                dbReplicationSettingEntity: dbReplicationSettingEntity,
                dbReplicationDefinitionId: dbReplicationDefinitionId,
                selectedDBDefinitions: selectedDBDefinitions
            };

            VRModalService.showModal('/Client/Modules/Common/Directives/DBReplication/DBReplicationSettings/Templates/DBReplicationSettingsTemplate.html', parameters, settings);
        }

        return {
            addDBReplicationSetting: addDBReplicationSetting,
            editDBReplicationSetting: editDBReplicationSetting
        };
    }

    appControllers.service('VRCommon_DBReplicationService', DBReplicationService);

})(appControllers);