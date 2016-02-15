(function (appControllers) {

    'use strict';

    TrunkService.$inject = ['VRModalService', 'VRNotificationService','CDRAnalysis_PSTN_TrunkAPIService','CDRAnalysis_PSTN_SwitchService'];

    function TrunkService(VRModalService, VRNotificationService, CDRAnalysis_PSTN_TrunkAPIService, CDRAnalysis_PSTN_SwitchService) {
        var drillDownDefinitions = [];
        return ({
            editTrunk: editTrunk,
            addTrunk: addTrunk,
            deleteTrunk:deleteTrunk,
            registerDrillDownToSwitch: registerDrillDownToSwitch
        });
        function editTrunk(trunkId, onTrunkUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onTrunkUpdated = onTrunkUpdated;
            };
            var parameters = {
                TrunkId: trunkId
            };

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/NetworkInfrastructure/TrunkEditor.html", parameters, settings);
        }
        function addTrunk(onTrunkAdded, switchId) {

            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onTrunkAdded = onTrunkAdded;
            };
            var parameters = {};
            if (switchId != undefined) {
                parameters.SwitchId = switchId;
            }

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/NetworkInfrastructure/TrunkEditor.html", parameters, settings);
        }
        function deleteTrunk(trunkObj, onTrunkDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response == true) {

                        trunkObj.LinkedToTrunkId = (trunkObj.LinkedToTrunkId != null) ? trunkObj.LinkedToTrunkId : -1;

                        return CDRAnalysis_PSTN_TrunkAPIService.DeleteTrunk(trunkObj.Entity.TrunkId, trunkObj.LinkedToTrunkId)
                            .then(function (deletionResponse) {
                                if (VRNotificationService.notifyOnItemDeleted("Trunk", deletionResponse))
                                    onTrunkDeleted(trunkObj, trunkObj.LinkedToTrunkId);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            });
                    }
                });
        }
        function registerDrillDownToSwitch() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Truks";
            drillDownDefinition.directive = "cdranalysis-pstn-trunk-grid";
            drillDownDefinition.parentMenuActions = [{
                name: "New Trunk",
                clicked: function (switchItem) {
                    if (drillDownDefinition.setTabSelected != undefined)
                        drillDownDefinition.setTabSelected(switchItem);

                    var onTrunkAdded = function (trunkObj) {
                        if (switchItem.trunkGridAPI != undefined) {
                            switchItem.trunkGridAPI.onTrunkAdded(trunkObj);
                        }
                    };
                    addTrunk(onTrunkAdded, switchItem.Entity.SwitchId);
                }
            }];
            drillDownDefinition.loadDirective = function (directiveAPI, switchItem) {
               switchItem.trunkGridAPI = directiveAPI;
                var query = {
                    SelectedSwitchIds: [switchItem.Entity.SwitchId],
                };

                return switchItem.trunkGridAPI.loadGrid(query);
            };
            CDRAnalysis_PSTN_SwitchService.addDrillDownDefinition(drillDownDefinition);
        }
      }
      appControllers.service('CDRAnalysis_PSTN_TrunkService', TrunkService);

})(appControllers);