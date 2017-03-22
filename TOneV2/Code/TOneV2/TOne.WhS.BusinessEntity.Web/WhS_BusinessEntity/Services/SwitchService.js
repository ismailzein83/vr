(function (appControllers) {

    'use strict';

    SwitchService.$inject = ['WhS_BE_SwitchAPIService', 'VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService'];

    function SwitchService(WhS_BE_SwitchAPIService, VRModalService, VRNotificationService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addSwitch: addSwitch,
            editSwitch: editSwitch,
            deleteSwitch: deleteSwitch,
            registerObjectTrackingDrillDownToSwitch: registerObjectTrackingDrillDownToSwitch,
            getDrillDownDefinition: getDrillDownDefinition
        });

        function addSwitch(onSwitchAdded) {
            var settings = {
            };

            var parameters = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwitchAdded = onSwitchAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/Switch/SwitchEditor.html', parameters, settings);
        }

        function editSwitch(switchId, onSwitchUpdated) {
            var modalSettings = {
            };
            var parameters = {
                switchId: switchId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSwitchUpdated = onSwitchUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/Switch/SwitchEditor.html', parameters, modalSettings);
        }

        function deleteSwitch(scope, switchId, onSwitchDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                   
                    if (response) {
                        return WhS_BE_SwitchAPIService.DeleteSwitch(switchId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Switch", deletionResponse);
                                onSwitchDeleted();
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, scope);
                            });
                    }
                });
        }
        function getEntityUniqueName() {
            return "WhS_BusinessEntity_Switch";
        }

        function registerObjectTrackingDrillDownToSwitch() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, switchItem) {
                switchItem.objectTrackingGridAPI = directiveAPI;

                var query = {
                    ObjectId: switchItem.Entity.SwitchId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return switchItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

    }

    appControllers.service('WhS_BE_SwitchService', SwitchService);

})(appControllers);
