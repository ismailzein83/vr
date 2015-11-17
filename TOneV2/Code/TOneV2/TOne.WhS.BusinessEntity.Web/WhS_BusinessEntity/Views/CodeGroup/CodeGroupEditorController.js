(function (appControllers) {

    "use strict";

    codeGroupEditorController.$inject = ['$scope', 'WhS_BE_CodeGroupAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService','VRUIUtilsService'];

    function codeGroupEditorController($scope, WhS_BE_CodeGroupAPIService, VRNotificationService, VRNavigationService, UtilsService,VRUIUtilsService) {

        
        var codeGroupId;
        var codeGroupEntity;
        var countryId;
        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var editMode;
        var disableCountry;
        defineScope();
        loadParameters();
       
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                codeGroupId = parameters.CodeGroupId;
                countryId = parameters.CountryId;
                disableCountry = parameters.disableCountry
            }
            editMode = (codeGroupId != undefined);
            $scope.disableCountry = ((countryId != undefined) && !editMode) || disableCountry == true;
            load();
        }
        function defineScope() {

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            }
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
            if (countryId != undefined) {
                $scope.title = UtilsService.buildTitleForAddEditor("Code Group");
                loadAllControls();
            }
            else if (editMode) {
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
                $scope.title = UtilsService.buildTitleForAddEditor("Code Group");
            }
           
        }
        function getCodeGroup() {
            return WhS_BE_CodeGroupAPIService.GetCodeGroup(codeGroupId).then(function (codeGroupeObj) {
                codeGroupEntity = codeGroupeObj;
                $scope.code = codeGroupEntity.Code;
                $scope.title = UtilsService.buildTitleForUpdateEditor($scope.code, "Code Group");
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCountrySelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isGettingData = false;
              });
        }
        function loadCountrySelector() {
            var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            countryReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: codeGroupEntity != undefined ? codeGroupEntity.CountryId :(countryId != undefined) ?countryId: undefined
                    }

                    VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, directivePayload, countryLoadPromiseDeferred);
                });
            return countryLoadPromiseDeferred.promise;
        }

        function fillScopeFromCodeGroupObj(codeGroupe) {
            $scope.code = codeGroupe.Code;
            $scope.title = UtilsService.buildTitleForUpdateEditor($scope.code, "Code Group");
        }

        function buildCodeGroupObjFromScope() {
            var obj = {
                Code: $scope.code,
                CountryId: countryDirectiveApi.getSelectedIds()

            }
            if (codeGroupId != null)
                obj.CodeGroupId = codeGroupId;
            return obj;
        }
        function insertCodeGroup() {
            var codeGroupObject = buildCodeGroupObjFromScope();
            return WhS_BE_CodeGroupAPIService.AddCodeGroup(codeGroupObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Code Group", response,"Code")) {
                    if ($scope.onCodeGroupAdded != undefined)
                        $scope.onCodeGroupAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }
        function updateCodeGroup() {
            var codeGroupObject = buildCodeGroupObjFromScope();
            WhS_BE_CodeGroupAPIService.UpdateCodeGroup(codeGroupObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Code Group", response, "Code")) {
                    if ($scope.onCodeGroupUpdated != undefined)
                        $scope.onCodeGroupUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('WhS_BE_CodeGroupEditorController', codeGroupEditorController);
})(appControllers);
