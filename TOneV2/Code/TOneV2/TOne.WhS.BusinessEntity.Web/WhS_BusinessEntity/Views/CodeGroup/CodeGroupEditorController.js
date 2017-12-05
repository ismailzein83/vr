(function (appControllers) {

    "use strict";

    codeGroupEditorController.$inject = ['$scope', 'WhS_BE_CodeGroupAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function codeGroupEditorController($scope, WhS_BE_CodeGroupAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var codeGroupId;
        var codeGroupEntity;
        var countryId;
        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var editMode;

        defineScope();
        loadParameters();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                codeGroupId = parameters.CodeGroupId;
                countryId = parameters.CountryId;
            }
            editMode = (codeGroupId != undefined);
            $scope.disableCountry = (codeGroupId != undefined);
            load();
        }

        function defineScope() {
            $scope.disableEditorController = false;
            $scope.disabledSaveButton = false;
            $scope.hasSaveCodeGroupPermission = function () {
                if (editMode)
                    return WhS_BE_CodeGroupAPIService.HasUpdateCodeGroupPermission();
                else
                    return WhS_BE_CodeGroupAPIService.HasAddCodeGroupPermission();
            };

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };
            $scope.saveCodeGroup = function () {
                if (editMode) {
                    return updateCodeGroup();
                }
                else {
                    return insertCodeGroup();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };



        }

        function load() {
            $scope.isGettingData = true;
            if (editMode) {
                checkIfCodeGroupHasRelatedCodes().then(function () {
                    getCodeGroup().then(function () {
                        loadAllControls()
                            .finally(function () {
                                codeGroupEntity = undefined;
                            });
                    }).catch(function () {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                });
            }
            else {
                loadAllControls();
            }

        }
        function checkIfCodeGroupHasRelatedCodes() {
            return WhS_BE_CodeGroupAPIService.CheckIfCodeGroupHasRelatedCodes(codeGroupId).then(function (disableEditorController) {
                $scope.disableEditorController = disableEditorController;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {

            });
        }
        function getCodeGroup() {
            return WhS_BE_CodeGroupAPIService.GetCodeGroup(codeGroupId).then(function (codeGroupeObj) {
                codeGroupEntity = codeGroupeObj;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCountrySelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isGettingData = false;
              });
        }

        function loadCountrySelector() {
            var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            countryReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {
                    selectedIds: codeGroupEntity != undefined ? codeGroupEntity.CountryId : (countryId != undefined) ? countryId : undefined
                };
                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, directivePayload, countryLoadPromiseDeferred);
            });
            return countryLoadPromiseDeferred.promise;
        }

        function setTitle() {
            if (editMode && codeGroupEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(codeGroupEntity.Code, "Code Group");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Code Group");
        }

        function loadStaticData() {

            if (codeGroupEntity == undefined)
                return;

            $scope.code = codeGroupEntity.Code;
            $scope.name = codeGroupEntity.Name;
        }

        function insertCodeGroup() {
            $scope.isGettingData = true;
            return WhS_BE_CodeGroupAPIService.AddCodeGroup(buildCodeGroupObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Code Group", response, "value")) {
                    if ($scope.onCodeGroupAdded != undefined)
                        $scope.onCodeGroupAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }

        function updateCodeGroup() {
            $scope.isGettingData = true;
            return WhS_BE_CodeGroupAPIService.UpdateCodeGroup(buildCodeGroupObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Code Group", response, "value")) {
                    if ($scope.onCodeGroupUpdated != undefined)
                        $scope.onCodeGroupUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }

        function buildCodeGroupObjFromScope() {
            var obj = {
                CodeGroupId: (codeGroupId != null) ? codeGroupId : 0,
                Code: $scope.code,
                CountryId: countryDirectiveApi.getSelectedIds(),
                Name: $scope.name
            };
            return obj;
        }
    }

    appControllers.controller('WhS_BE_CodeGroupEditorController', codeGroupEditorController);
})(appControllers);
