(function (appControllers) {
    "use strict";
    automatedReportHandlerEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRCommon_VRComponentTypeAPIService', 'VR_Analytic_AdvancedExcelFileGeneratorAPIService'];
    function automatedReportHandlerEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VRCommon_VRComponentTypeAPIService, VR_Analytic_AdvancedExcelFileGeneratorAPIService) {


        var isEditMode;
        var attachementGeneratorEntity;

        var fileNamePatternAPI;
        var fileNamePatternReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onFileNamePatternReady = function (api) {
                fileNamePatternAPI = api;
                fileNamePatternReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onFileGeneratorSelectorReady = function (api) {
                fileGeneratorAPI = api;
                fileGeneratorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.downloadAttachmentGenerator = function () {
                var input =
                {
                    FileGenerator: buildObjFromScope(),
                    Queries: context.getQueryInfo()
                };

                return VR_Analytic_AdvancedExcelFileGeneratorAPIService.DownloadAttachmentGenerator(input).then(function (response) {
                    if (response != undefined)
                        UtilsService.downloadFile(response.data, response.headers);
                });
            };

            $scope.scopeModel.saveAttachmentGenerator = function () {
                if (isEditMode)
                    return updateAttachementGenerator();
                else
                    return insertAttachementGenerator();
            };

            $scope.scopeModel.close = function () {
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
            }

            function loadFileNamePattern() {
                var fileNamePatternDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                fileNamePatternReadyPromiseDeferred.promise.then(function () {
                    var fileNamePatternDirectivePayload =
                       {
                           fileNamePattern: attachementGeneratorEntity!=undefined ? attachementGeneratorEntity.Name : undefined
                       };
                    VRUIUtilsService.callDirectiveLoad(fileNamePatternAPI, fileNamePatternDirectivePayload, fileNamePatternDeferredLoadPromiseDeferred);
                });
                return fileNamePatternDeferredLoadPromiseDeferred.promise;
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

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFileGeneratorSelector, loadFileNamePattern])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function buildObjFromScope() {
            var obj = {
                //$type: "Vanrise.Analytic.Entities.VRAutomatedReportFileGenerator, Vanrise.Analytic.Entities",
                VRAutomatedReportFileGeneratorId: attachementGeneratorEntity != undefined ? attachementGeneratorEntity.VRAutomatedReportFileGeneratorId: UtilsService.guid() ,
                Name: fileNamePatternAPI.getData(),
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