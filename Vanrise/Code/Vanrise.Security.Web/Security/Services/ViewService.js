(function (appControllers) {

    'use strict';

    ViewService.$inject = ['VR_Sec_ViewAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService'];

    function ViewService(VR_Sec_ViewAPIService, UtilsService, VRModalService, VRNotificationService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addDynamicPage: addDynamicPage,
            updateDynamicPage: updateDynamicPage,
            deleteDynamicPage: deleteDynamicPage,
            editView: editView,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToView: registerObjectTrackingDrillDownToView
        });
 
        function addDynamicPage(onViewAdded) {
            var modalSettings = {
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onViewAdded = onViewAdded;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPageEditor.html', null, modalSettings);
        }

        function updateDynamicPage(viewId, onDynamicPageUpdated) {
            var modalParameters = {
                viewId: viewId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onPageUpdated = onDynamicPageUpdated
            };

            VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPageEditor.html', modalParameters, modalSettings);
        }

        function deleteDynamicPage(scope, dataItem, onDynamicPageDeleted) {
            VRNotificationService.showConfirmation().then(function (confirmed) {
                if (confirmed) {
                    return VR_Sec_ViewAPIService.DeleteView(dataItem.Entity.ViewId).then(function (responseObject) {
                        var deleted = VRNotificationService.notifyOnItemDeleted('View', responseObject);

                        if (deleted && onDynamicPageDeleted && typeof onDynamicPageDeleted == 'function') {
                            onDynamicPageDeleted(dataItem);
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    })
                }
            });
        }

        function editView(viewId, onViewUpdated)
        {
            var modalParameters = {
                viewId: viewId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onViewUpdated = onViewUpdated
            };

            VRModalService.showModal('/Client/Modules/Security/Views/Menu/ViewEditor.html', modalParameters, modalSettings);

        }

        function getEntityUniqueName() {
            return "VR_Security_View";
        }

        function registerObjectTrackingDrillDownToView() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, viewItem) {
                viewItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: viewItem.Entity.ViewId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return viewItem.objectTrackingGridAPI.load(query);
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
   
    appControllers.service('VR_Sec_ViewService', ViewService);

})(appControllers);
