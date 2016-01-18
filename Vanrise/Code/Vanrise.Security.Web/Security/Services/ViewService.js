(function (appControllers) {

    "use strict";
    viewService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService', 'ViewAPIService', 'DeleteOperationResultEnum'];

    function viewService(VRModalService, VRNotificationService, UtilsService, ViewAPIService, DeleteOperationResultEnum) {

        function deleteDynamicPage($scope, dataItem, onDynamicPageDeleted) {

            VRNotificationService.showConfirmation().then(function (response) {

                if (response == true) {
                    return ViewAPIService.DeleteView(dataItem.ViewId).then(function (responseObject) {
                        if (responseObject.Result == DeleteOperationResultEnum.Succeeded.value)
                            onDynamicPageDeleted(dataItem);

                        VRNotificationService.notifyOnItemDeleted("View", responseObject);
                        $scope.isGettingData = false
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    }).finally(function () {
                    });
                }

            });
        }
 
        function updateDynamicPage(pageId, onDynamicPageUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForUpdateEditor(dataItem.Name, "Dynamic Page");
                modalScope.onPageUpdated = onDynamicPageUpdated
            };
            VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPageEditor.html', dataItem, settings);

        }
  
        function addDynamicPage(onDynamicPageAdded) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForAddEditor("Dynamic Page");
                modalScope.onDynamicPageAdded = onDynamicPageAdded;
            };
            VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPageEditor.html', null, settings);
        }

        return ({
            updateDynamicPage: updateDynamicPage,
            deleteDynamicPage: deleteDynamicPage,
            addDynamicPage: addDynamicPage
        });
    }
   
    appControllers.service('VR_ViewService', viewService);
})(appControllers);