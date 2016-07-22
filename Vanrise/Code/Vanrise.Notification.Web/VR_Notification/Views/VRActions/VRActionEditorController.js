﻿(function (appControllers) {

    "use strict";

    VRActionEditorController.$inject = ['$scope',  'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function VRActionEditorController($scope,  VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;
        var actionEntity;

        var actionDirectiveApi;
        var actionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                actionEntity = parameters.actionEntity;
            }

            isEditMode = (actionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onVRActionDirectiveReady = function (api) {
                actionDirectiveApi = api;
                actionReadyPromiseDeferred.resolve();
            }


            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
     
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadActionDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            if (isEditMode) {
                var actionName = (actionEntity != undefined) ? actionEntity.Name : null;
                $scope.title = UtilsService.buildTitleForUpdateEditor(actionName, 'Action');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Action');
            }
        }

        function loadStaticData() {
            if (actionEntity == undefined)
                return;
        }

        function loadActionDirective() {
            var actionLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            actionReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload;
                    VRUIUtilsService.callDirectiveLoad(actionDirectiveApi, directivePayload, actionLoadPromiseDeferred);
                });
            return actionLoadPromiseDeferred.promise;
        }

        function insert() {
            if ($scope.onVRActionAdded != undefined)
                $scope.onVRActionAdded(buildActionObjFromScope());
            $scope.modalContext.closeModal();
        }

        function update() {
            if ($scope.onVRActionUpdated != undefined) {
                $scope.onVRActionUpdated(buildActionObjFromScope());
            }
            $scope.modalContext.closeModal();
        }

        function buildActionObjFromScope() {
            return actionDirectiveApi.getData();
        }
    }

    appControllers.controller('VRNotification_VRActionEditorController', VRActionEditorController);

})(appControllers);