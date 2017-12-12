(function (app) {

    "use strict";

    VRLocalizationTextResourceTranslationEditorController.$inject = ['$scope', 'VRCommon_VRLocalizationTextResourceAPIService', 'VRCommon_VRLocalizationTextResourceService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'VRCommon_VRLocalizationTextResourceTranslationAPIService'];

    function VRLocalizationTextResourceTranslationEditorController($scope, VRCommon_VRLocalizationTextResourceAPIService, VRCommon_VRLocalizationTextResourceService, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService, VRCommon_VRLocalizationTextResourceTranslationAPIService) {

        var gridAPI;

        var textResourceSelectorAPI;
        var textResourceSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var languageSelectorAPI;
        var languageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var textResourceTranslationId;

        var textResourceTranslationEntity;

        var textResourceId;

        var isEditMode;

        loadParameters();

        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                textResourceTranslationId = parameters.vrLocalizationTextResourceTranslationId;
                textResourceId=parameters.textResourceId;
            }
            isEditMode = (textResourceTranslationId != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.onTextResourceSelectorReady = function (api) {
                textResourceSelectorAPI = api;
                textResourceSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onLanguageSelectorReady = function (api) {
                languageSelectorAPI = api;
                languageSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (!isEditMode)
                    addTextResourceTranslation();
                else
                    updateTextResourceTranslation();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                if (textResourceId != undefined) {
                    $scope.scopeModel.textResourceSelectorDisabled = true;
                }
                getVRLocalizationTextResourceTranslation().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
          else  loadAllControls();
        }
        function loadAllControls() {
            function setTitle() {
                if (!isEditMode)
                    $scope.title = UtilsService.buildTitleForAddEditor('Text Resource Translation');
                else
                    $scope.title = UtilsService.buildTitleForUpdateEditor('Text Resource Translation');

            }
            function loadStaticData()
            {
                if (textResourceTranslationEntity != undefined)
                    $scope.scopeModel.value = textResourceTranslationEntity.Settings.Value;
            }
            function loadTextResourceSelector() {
                var textResourceSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                textResourceSelectorReadyDeferred.promise.then(function () {
                    var payload = {};
                    if (textResourceTranslationEntity != undefined)
                        payload.selectedIds = textResourceTranslationEntity.ResourceId;
                    if (textResourceId != undefined) {
                        payload.selectedIds = textResourceId;
                        $scope.scopeModel.textResourceSelectorDisabled = true;          
                    }
                    VRUIUtilsService.callDirectiveLoad(textResourceSelectorAPI, payload, textResourceSelectorLoadDeferred);
                });
                return textResourceSelectorLoadDeferred.promise;
            }

            function loadLanguageSelector() {
                var languageSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                languageSelectorReadyDeferred.promise.then(function () {
                    var payload = {};
                    if (textResourceTranslationEntity != undefined)
                        payload.selectedIds = textResourceTranslationEntity.LanguageId;
                    VRUIUtilsService.callDirectiveLoad(languageSelectorAPI, payload, languageSelectorLoadDeferred);
                });
                return languageSelectorLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([loadLanguageSelector, loadTextResourceSelector, setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function buildObjectFromScope() {
            var obj = {
                ResourceId: textResourceSelectorAPI.getSelectedIds(),
                LanguageId: languageSelectorAPI.getSelectedIds(),
                Settings: {
                    Value:$scope.scopeModel.value
                }
            };
            if (isEditMode)
                obj.VRLocalizationTextResourceTranslationId = textResourceTranslationId;
            return obj;
        }
        
        function addTextResourceTranslation() {
            var textResourceTranslation = buildObjectFromScope();
            VRCommon_VRLocalizationTextResourceTranslationAPIService.AddVRLocalizationTextResourceTranslation(textResourceTranslation).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Resource Translation", response)) {
                    if ($scope.onVRLocalizationTextResourceTranslationAdded != undefined) {
                        $scope.onVRLocalizationTextResourceTranslationAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function getVRLocalizationTextResourceTranslation() {
          return  VRCommon_VRLocalizationTextResourceTranslationAPIService.GetVRLocalizationTextResourceTranslation(textResourceTranslationId).then(function (response) {
                textResourceTranslationEntity = response;
            });
        }
        function updateTextResourceTranslation() {
            var textResourceTranslation = buildObjectFromScope();
            VRCommon_VRLocalizationTextResourceTranslationAPIService.UpdateVRLocalizationTextResourceTranslation(textResourceTranslation).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Resource Translation", response)) {
                    if ($scope.onVRLocalizationTextResourceTranslationUpdated != undefined) {
                        $scope.onVRLocalizationTextResourceTranslationUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });


        }

    }
    app.controller('VRCommon_VRLocalizationTextResourceTranslationEditorController', VRLocalizationTextResourceTranslationEditorController);

})(app);