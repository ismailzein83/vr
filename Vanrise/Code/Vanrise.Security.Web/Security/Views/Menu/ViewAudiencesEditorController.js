(function (appControllers) {

    'use strict';

    ViewAudiencesEditorController.$inject = ['$scope', 'VR_Sec_ViewAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function ViewAudiencesEditorController($scope, VR_Sec_ViewAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {
        var isEditMode;
        var viewId;
        var viewAudiencesInfo;

        var userDirectiveAPI;
        var userReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var groupDirectiveAPI;
        var groupReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                viewId = parameters.viewId;

            isEditMode = (viewId != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {};

            $scope.scopeModal.save = function () {
                if (isEditMode)
                    return updateViewAudiences();
            };


            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModal.onUserDirectiveReady = function (api) {
                userDirectiveAPI = api;
                userReadyPromiseDeferred.resolve();
            };

            $scope.scopeModal.onGroupDirectiveReady = function (api) {
                groupDirectiveAPI = api;
                groupReadyPromiseDeferred.resolve();
            };



        }

        function load() {
            $scope.scopeModal.isLoading = true;

            if (isEditMode) {
                getViewAudiencesInfo().then(function () {
                    loadAllControls().finally(function () {
                            viewAudiencesInfo = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            function getViewAudiencesInfo() {
                return VR_Sec_ViewAPIService.GetViewAudiencesInfo(viewId).then(function (response) {
                    viewAudiencesInfo = response;
                });
            }
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadUsers, loadGroups])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModal.isLoading = false;
              });
        }

        function loadUsers() {
            var userLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            userReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {
                    selectedIds: (viewAudiencesInfo != undefined && viewAudiencesInfo.Audience != null && viewAudiencesInfo.Audience.Users != undefined) ? viewAudiencesInfo.Audience.Users : undefined
                };
                VRUIUtilsService.callDirectiveLoad(userDirectiveAPI, directivePayload, userLoadPromiseDeferred);
            });
            return userLoadPromiseDeferred.promise;

        }

        function loadGroups() {
            var groupLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            groupReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {
                    selectedIds: (viewAudiencesInfo != undefined && viewAudiencesInfo.Audience != null && viewAudiencesInfo.Audience.Groups != undefined) ? viewAudiencesInfo.Audience.Groups : undefined
                };
                VRUIUtilsService.callDirectiveLoad(groupDirectiveAPI, directivePayload, groupLoadPromiseDeferred);
            });
            return groupLoadPromiseDeferred.promise;
        }

        function setTitle() {
            if (isEditMode && viewAudiencesInfo != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(viewAudiencesInfo.Name, 'View Audiences');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('View Audiences');
        }

     

        function buildViewAudiencesObjFromScope() {
            var audiences = {
                Users: userDirectiveAPI.getSelectedIds(),
                Groups: groupDirectiveAPI.getSelectedIds()
            };
            var viewAudiencesObject = {
                ViewId: viewId,               
                Audience: audiences
            };
            return viewAudiencesObject;
        }

        function updateViewAudiences() {
            $scope.scopeModal.isLoading = true;

            var viewAudiencesObject = buildViewAudiencesObjFromScope();

            return VR_Sec_ViewAPIService.UpdateViewAudiences(viewAudiencesObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('View Audiences', response, 'Name')) {
                    if ($scope.onViewAudiencesUpdated != undefined)
                        $scope.onViewAudiencesUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });
        }
    }

    appControllers.controller('VR_Sec_ViewAudiencesEditorController', ViewAudiencesEditorController);

})(appControllers);
