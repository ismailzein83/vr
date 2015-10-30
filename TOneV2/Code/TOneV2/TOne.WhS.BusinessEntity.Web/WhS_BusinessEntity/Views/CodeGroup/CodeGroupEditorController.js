(function (appControllers) {

    "use strict";

    codeGroupEditorController.$inject = ['$scope', 'WhS_BE_CodeGroupAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function codeGroupEditorController($scope, WhS_BE_CodeGroupAPIService, UtilsService, VRNotificationService, VRNavigationService) {

        
        var codeGroupId;
        var countryId;
        var countryDirectiveApi;
        var editMode;
        var disableCountry;
        defineScope();
        loadParameters();
        load();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                codeGroupId = parameters.CodeGroupId;
                countryId = parameters.CountryId;
                disableCountry = parameters.disableCountry

            }
            editMode = (codeGroupId != undefined);
            $scope.disableCountry = ((countryId != undefined) && !editMode) || disableCountry == true;
        }
        function defineScope() {
            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                load();
            }
            $scope.SaveCodeGroup = function () {
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
             if (countryDirectiveApi == undefined)
                return;
            countryDirectiveApi.load().then(function () {
                if ($scope.disableCountry && countryId != undefined)
                {
                    countryDirectiveApi.setData(countryId);
                    $scope.isGettingData = false;
                }   
                else if (editMode) {
                    getCodeGroup();
                }
                else {
                    $scope.isGettingData = false;
                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });

        }
        function getCodeGroup() {
            return WhS_BE_CodeGroupAPIService.GetCodeGroup(codeGroupId).then(function (codeGroupe) {
                fillScopeFromCodeGroupObj(codeGroupe);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }

        function fillScopeFromCodeGroupObj(codeGroupe) {
            $scope.code = codeGroupe.Code;
            countryDirectiveApi.setData(codeGroupe.CountryId)
        }

        function buildCodeGroupObjFromScope() {
            var obj = {
                Code: $scope.code,
                CountryId: countryDirectiveApi.getDataId()

            }
            if (codeGroupId != null)
                obj.CodeGroupId = codeGroupId;
            return obj;
        }
        function insertCodeGroup() {
            var codeGroupObject = buildCodeGroupObjFromScope();
            return WhS_BE_CodeGroupAPIService.AddCodeGroup(codeGroupObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Code Group", response)) {
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
                if (VRNotificationService.notifyOnItemUpdated("Code Group", response)) {
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
