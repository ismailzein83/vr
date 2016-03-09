(function (appControllers) {

    'use strict';

    ViewService.$inject = ['VR_Sec_ViewAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function ViewService(VR_Sec_ViewAPIService, UtilsService, VRModalService, VRNotificationService) {
        return ({
            addDynamicPage: addDynamicPage,
            updateDynamicPage: updateDynamicPage,
            deleteDynamicPage: deleteDynamicPage,
            editView: editView
        });
 
        function addDynamicPage(onDynamicPageAdded) {
            var modalSettings = {
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDynamicPageAdded = onDynamicPageAdded;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPageEditor.html', null, modalSettings);
        }

        function updateDynamicPage(viewId, onDynamicPageUpdated) {
            var modalParameters = {
                ViewId: viewId
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
    }
   
    appControllers.service('VR_Sec_ViewService', ViewService);

})(appControllers);
