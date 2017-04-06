(function (appControllers) {

    'use strict';

    GroupService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService', 'UtilsService'];

    function GroupService(VRModalService, VRCommon_ObjectTrackingService, UtilsService) {
        var drillDownDefinitions = [];
        return ({
            addGroup: addGroup,
            editGroup: editGroup,
            registerObjectTrackingDrillDownToGroup: registerObjectTrackingDrillDownToGroup,
            getDrillDownDefinition: getDrillDownDefinition,
            registerHistoryViewAction: registerHistoryViewAction
        });



        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "VR_Security_Group_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistoryGroup(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }

        function viewHistoryGroup(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/Security/Views/Group/GroupEditor.html', modalParameters, modalSettings);
        };

        function addGroup(onGroupAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGroupAdded = onGroupAdded;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/Group/GroupEditor.html', null, modalSettings);
        }

        function editGroup(groupId, onGroupUpdated) {
            var modalParameters = {
                groupId: groupId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGroupUpdated = onGroupUpdated;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/Group/GroupEditor.html', modalParameters, modalSettings);
        }
        function getEntityUniqueName()
        {
            return "VR_Security_Group";
        }

        function registerObjectTrackingDrillDownToGroup() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";
            

            drillDownDefinition.loadDirective = function (directiveAPI, groupItem) {
                groupItem.objectTrackingGridAPI = directiveAPI;
               
                var query = {
                    ObjectId: groupItem.Entity.GroupId,
                    EntityUniqueName: getEntityUniqueName(),
                    
                };
                return groupItem.objectTrackingGridAPI.load(query);
            };
            
            addDrillDownDefinition(drillDownDefinition);
           
        }
        function addDrillDownDefinition(drillDownDefinition) {
          
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
    };
    

    appControllers.service('VR_Sec_GroupService', GroupService);

})(appControllers);
