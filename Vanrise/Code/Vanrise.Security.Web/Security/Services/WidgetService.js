(function (appControllers) {

    'use strict';

    WidgetService.$inject = ['VR_Sec_WidgetAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function WidgetService(VR_Sec_WidgetAPIService, UtilsService, VRModalService, VRNotificationService) {
        return ({
            updateWidget: updateWidget,
            deleteWidget: deleteWidget,
            addWidget: addWidget
        });

        function addWidget(onWidgetAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onWidgetAdded = onWidgetAdded;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/WidgetsPages/WidgetEditor.html', null, modalSettings);
        }

        function updateWidget(widgetId, onWidgetUpdated) {
            var modalParameters = {
                Id: widgetId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onWidgetUpdated = onWidgetUpdated;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/WidgetsPages/WidgetEditor.html', modalParameters, modalSettings);
        }

        function deleteWidget(scope, dataItem, onWidgetDeleted) {
            VRNotificationService.showConfirmation().then(function (confirmed) {
                if (confirmed) {
                    return VR_Sec_WidgetAPIService.DeleteWidget(dataItem.Entity.Id).then(function (responseObject) {
                        var deleted = VRNotificationService.notifyOnItemDeleted('Widget', responseObject, 'dynamic pages');

                        if (deleted && onWidgetDeleted && typeof onWidgetDeleted == 'function') {
                            onWidgetDeleted(dataItem);
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    });
                }
            });
        }
    }

    appControllers.service('VR_Sec_WidgetService', WidgetService);

})(appControllers);
