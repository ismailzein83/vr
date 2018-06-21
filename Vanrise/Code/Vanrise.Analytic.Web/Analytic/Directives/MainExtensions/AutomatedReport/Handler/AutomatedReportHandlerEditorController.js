(function (appControllers) {
    "use strict";
    automatedReportHandlerEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRCommon_VRComponentTypeAPIService'];
    function automatedReportHandlerEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VRCommon_VRComponentTypeAPIService) {


        var isEditMode;
        var attachementGeneratorEntity;

        var fileGeneratorAPI;
        var fileGeneratorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var context;

        loadParameters();
        defineScope(); 
        load();

        function loadParameters() {

            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                attachementGeneratorEntity = parameters.Entity;
                context = parameters.context;
            }
            isEditMode = (attachementGeneratorEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onFileGeneratorSelectorReady = function (api) {
                fileGeneratorAPI = api;
                fileGeneratorReadyPromiseDeferred.resolve();
            };

            $scope.saveAttachementGenerator = function () {
                if (isEditMode)
                    return updateAttachementGenerator();
                else
                    return insertAttachementGenerator();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };



        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && attachementGeneratorEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(attachementGeneratorEntity.Name, "Attachment");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Attachment");
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
                        fileGenerator: attachementGeneratorEntity != undefined && attachementGeneratorEntity.Settings != undefined ? attachementGeneratorEntity.Settings : undefined,
                        context : getContext()
                    };
                    VRUIUtilsService.callDirectiveLoad(fileGeneratorAPI, fileGeneratorPayload, fileGeneratorLoadPromiseDeferred);
                });
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFileGeneratorSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
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
            var object = buildObjFromScope();
            if ($scope.onAttachementGeneratorAdded != undefined && typeof ($scope.onAttachementGeneratorAdded) == 'function')
                $scope.onAttachementGeneratorAdded(object);
            $scope.modalContext.closeModal();
        }

        function updateAttachementGenerator() {
            var object = buildObjFromScope();
            if ($scope.onAttachementGeneratorUpdated != undefined && typeof ($scope.onAttachementGeneratorUpdated) == 'function')
                $scope.onAttachementGeneratorUpdated(object);
            $scope.modalContext.closeModal();
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
    }
    appControllers.controller('VRAnalytic_AutomatedReportHandlerEditorController', automatedReportHandlerEditorController);
})(appControllers);