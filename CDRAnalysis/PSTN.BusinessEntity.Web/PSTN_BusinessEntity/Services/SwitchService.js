(function (appControllers) {

    'use strict';

    SwitchService.$inject = ['VRModalService', 'VRNotificationService', 'CDRAnalysis_PSTN_SwitchAPIService'];

    function SwitchService(VRModalService, VRNotificationService, CDRAnalysis_PSTN_SwitchAPIService) {
        var drillDownDefinitions = [];
        function editSwitch(switchId, onSwitchUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwitchUpdated = onSwitchUpdated;
            };
            var parameters = {
                SwitchId: switchId
            };

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/NetworkInfrastructure/SwitchEditor.html", parameters, settings);
        }

        function addSwitch(onSwitchAdded) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSwitchAdded = onSwitchAdded;
            };
            var parameters = {};

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/NetworkInfrastructure/SwitchEditor.html", parameters, settings);
        }
        
        function deleteSwitch(switchObj, onSwitchDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response == true) {
                        return CDRAnalysis_PSTN_SwitchAPIService.DeleteSwitch(switchObj.Entity.SwitchId)
                        .then(function (deletionResponse) {
                            if (VRNotificationService.notifyOnItemDeleted("Switch", deletionResponse))
                                onSwitchDeleted(switchObj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
            });
        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
        return ({
            editSwitch: editSwitch,
            addSwitch: addSwitch,
            deleteSwitch:deleteSwitch,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition
        });
    }

    appControllers.service('CDRAnalysis_PSTN_SwitchService', SwitchService);

})(appControllers);
