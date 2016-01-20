(function (appControllers) {

    "use strict";
    widgetService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService','WidgetAPIService','DeleteOperationResultEnum'];

    function widgetService(VRModalService, VRNotificationService, UtilsService, WidgetAPIService, DeleteOperationResultEnum) {

        function deleteWidget($scope, dataItem, onWidgetDeleted) {
            VRNotificationService.showConfirmation().then(function (response) {
                if (response) {
                    return WidgetAPIService.DeleteWidget(dataItem.Entity.Id).then(function (responseObject) {
                        if (responseObject.Result == DeleteOperationResultEnum.Succeeded.value)
                            onWidgetDeleted(dataItem);
                        VRNotificationService.notifyOnItemDeleted("Widget", responseObject, "dynamic pages");
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    }).finally(function () {


                    });
                }

            });


        }

        function updateWidget(widgetId, onWidgetUpdated) {
            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onWidgetUpdated = onWidgetUpdated;
            };
            var parameters = {
                Id: widgetId
            };

            VRModalService.showModal('/Client/Modules/Security/Views/WidgetsPages/WidgetEditor.html', parameters, modalSettings);
        }

        function addWidget(onWidgetAdded) {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForAddEditor("Widget");
                modalScope.onWidgetAdded = onWidgetAdded
            };
            VRModalService.showModal('/Client/Modules/Security/Views/WidgetsPages/WidgetEditor.html', null, settings);
        }

        return ({
            updateWidget: updateWidget,
            deleteWidget: deleteWidget,
            addWidget: addWidget
        });
    }

    appControllers.service('VR_WidgetService', widgetService);
})(appControllers);