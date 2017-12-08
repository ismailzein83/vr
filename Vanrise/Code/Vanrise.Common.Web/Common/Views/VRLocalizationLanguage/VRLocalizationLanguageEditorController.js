(function (appControllers) {

    "use strict";

    VRLocalizationLanguageController.$inject = ['$scope', 'VRCommon_VRLocalizationLanguageAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function VRLocalizationLanguageController($scope, VRCommon_VRLocalizationLanguageAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;

        var vrLocalizationLanguageId;
        var vrLocalizationLanguageEntity;

        var localizationLanguageSelectorAPI;
        var localizationLanguageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                vrLocalizationLanguageId = parameters.vrLocalizationLanguageId;
            }
            isEditMode = (vrLocalizationLanguageId != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.onLocalizationLanguageSelectorReady = function (api) {
                localizationLanguageSelectorAPI = api;
                localizationLanguageSelectorReadyDeferred.resolve();
            };

            $scope.saveVRLocalizationLanguage = function () {
                if (isEditMode)
                    return updateVRLocalizationLanguage();
                else
                    return insertVRLocalizationLanguage();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {

            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getVRLocalizationLanguage().then(function () {
                    loadAllControls()
                        .finally(function () {
                            vrLocalizationLanguageEntity = undefined;
                            $scope.scopeModel.isLoading = false;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                  
                });
            }
            else {
                loadAllControls();
            }
        }

        function getVRLocalizationLanguage() {
            return VRCommon_VRLocalizationLanguageAPIService.GetVRLocalizationLanguage(vrLocalizationLanguageId).then(function (entity) {
                vrLocalizationLanguageEntity = entity;
            });
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrLocalizationLanguageEntity.Name, "Localization Language");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Localization Language");
            }

            function loadStaticData() {
                if (vrLocalizationLanguageEntity != undefined) {
                    $scope.scopeModel.name = vrLocalizationLanguageEntity.Name;
                    $scope.scopeModel.rightToLeft = vrLocalizationLanguageEntity.Settings.IsRTL;
                }
            }

            function loadLocalizationLanguageSelector() {
                var localizationLanguageSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                localizationLanguageSelectorReadyDeferred.promise.then(function () {
                    var localizationLanguageSelectorPayload;
                    if (vrLocalizationLanguageEntity != undefined) {

                        localizationLanguageSelectorPayload = {
                            selectedIds: vrLocalizationLanguageEntity.ParentLanguageId,
                            filter: {
                                ExcludedIds: [vrLocalizationLanguageId]
                            }
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(localizationLanguageSelectorAPI, localizationLanguageSelectorPayload, localizationLanguageSelectorLoadDeferred);
                });
                return localizationLanguageSelectorLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadLocalizationLanguageSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function buildVRLocalizationLanguageObjFromScope() {
            return {
                VRLanguageId: vrLocalizationLanguageId,
                Name: $scope.scopeModel.name,
                ParentLanguageId: localizationLanguageSelectorAPI.getSelectedIds(),
                Settings: {
                    IsRTL: $scope.scopeModel.rightToLeft
                }
            };
        }

        function insertVRLocalizationLanguage() {
            $scope.scopeModel.isLoading = true;

            var VRLocalizationLanguageObject = buildVRLocalizationLanguageObjFromScope();
            return VRCommon_VRLocalizationLanguageAPIService.AddVRLocalizationLanguage(VRLocalizationLanguageObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Localization Language", response, "Name")) {
                    if ($scope.onVRLocalizationLanguageAdded != undefined) {
                        $scope.onVRLocalizationLanguageAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        }

        function updateVRLocalizationLanguage() {
            $scope.scopeModel.isLoading = true;

            var VRLocalizationLanguageObject = buildVRLocalizationLanguageObjFromScope();
            VRCommon_VRLocalizationLanguageAPIService.UpdateVRLocalizationLanguage(VRLocalizationLanguageObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Localization Language", response, "Name")) {
                    if ($scope.onVRLocalizationLanguageUpdated != undefined)
                        $scope.onVRLocalizationLanguageUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }
    appControllers.controller('VRCommon_VRLocalizationLanguageController', VRLocalizationLanguageController);
})(appControllers);
