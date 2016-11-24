(function (appControllers) {

    "use strict";

    codeGroupEditorController.$inject = ['$scope', 'Vr_NP_CodeGroupAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function codeGroupEditorController($scope, Vr_NP_CodeGroupAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

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

            $scope.hasSaveCodeGroupPermission = function () {
                if (editMode)
                    return Vr_NP_CodeGroupAPIService.HasUpdateCodeGroupPermission();
                else
                    return Vr_NP_CodeGroupAPIService.HasAddCodeGroupPermission();
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
                $scope.modalContext.closeModal()
            };



        }

        function load() {
            $scope.isGettingData = true;
            if (editMode) {
                getCodeGroup().then(function () {
                    loadAllControls()
                        .finally(function () {
                            codeGroupEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                loadAllControls();
            }

        }

        function getCodeGroup() {
            return Vr_NP_CodeGroupAPIService.GetCodeGroup(codeGroupId).then(function (codeGroupeObj) {
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
        }

        function insertCodeGroup() {
            $scope.isGettingData = true;
            return Vr_NP_CodeGroupAPIService.AddCodeGroup(buildCodeGroupObjFromScope())
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
            return Vr_NP_CodeGroupAPIService.UpdateCodeGroup(buildCodeGroupObjFromScope())
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
                CountryId: countryDirectiveApi.getSelectedIds()
            };
            return obj;
        }
    }

    appControllers.controller('Vr_NP_CodeGroupEditorController', codeGroupEditorController);
})(appControllers);
