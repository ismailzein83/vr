(function (appControllers) {
    "use strict";
    sendEmailHandlerAutomatedReportEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRCommon_VRComponentTypeAPIService'];
    function sendEmailHandlerAutomatedReportEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VRCommon_VRComponentTypeAPIService) {


        var isEditMode;
        var attachementGeneratorEntity;

        var fileGeneratorAPI;
        var fileGeneratorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {

            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                attachementGeneratorEntity = parameters.Entity;
            }
            isEditMode = (attachementGeneratorEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onFileGeneratorSelectorReady = function (api) {
                fileGeneratorAPI = api;
                fileGeneratorReadyPromiseDeferred.resolve();
            }

            $scope.saveAttachementGenerator = function () {
                if (isEditMode)
                    return updateAttachementGenerator();
                else
                    return insertAttachementGenerator();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            //function getContext() {
            //    var currentContext = context;

            //    if (currentContext == undefined) {
            //        currentContext = {};
            //    }
            //    return currentContext
            //}

        }
        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && attachementGeneratorEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(attachementGeneratorEntity.Name, "Attachement Generator");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Attachement Generator");
            }

            function loadStaticData() {

                if (attachementGeneratorEntity == undefined)
                    return;
                $scope.scopeModel.Name = attachementGeneratorEntity.Name;
            }

            function loadFileGeneratorSelector() {
                var fileGeneratorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                fileGeneratorReadyPromiseDeferred.promise.then(function () {

                    var fileGeneratorPayload = {
                        fileGenerator: attachementGeneratorEntity != undefined && attachementGeneratorEntity.Settings != undefined ? attachementGeneratorEntity.Settings : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(fileGeneratorAPI, fileGeneratorPayload, fileGeneratorLoadPromiseDeferred);
                });
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFileGeneratorSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function buildObjFromScope() {
            var obj = {
                VRAutomatedReportFileGeneratorId: attachementGeneratorEntity != undefined ? attachementGeneratorEntity.VRAutomatedReportFileGeneratorId: UtilsService.guid() ,
                Name: $scope.scopeModel.Name,
                Settings: fileGeneratorAPI.getData(),
            };
            return obj;
        }

        function insertAttachementGenerator() {
            var Object = buildObjFromScope();
            if ($scope.onAttachementGeneratorAdded != undefined && typeof ($scope.onAttachementGeneratorAdded) == 'function')
                $scope.onAttachementGeneratorAdded(Object);
            $scope.modalContext.closeModal();
        }

        function updateAttachementGenerator() {
            var Object = buildObjFromScope();
            if ($scope.onAttachementGeneratorUpdated != undefined && typeof ($scope.onAttachementGeneratorUpdated) == 'function')
                $scope.onAttachementGeneratorUpdated(Object);
            $scope.modalContext.closeModal();
        }
    }
    appControllers.controller('VRAnalytic_SendEmailHandlerAutomatedReportEditorController', sendEmailHandlerAutomatedReportEditorController);
})(appControllers);