(function (appControllers) {

    "use strict";
    familyEditorController.$inject = ['$scope', 'Demo_Module_FamilyAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function familyEditorController($scope, Demo_Module_FamilyAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var familyId;
        var familyEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                familyId = parameters.familyId;
            }
            isEditMode = (familyId != undefined);
        };

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.saveFamily = function () {
                if (isEditMode)
                    return updateFamily();
                else
                    return insertFamily();

            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        };

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getFamily().then(function () {
                    loadAllControls().finally(function () {
                        familyEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else
                loadAllControls();
        };

        function getFamily() {
            return Demo_Module_FamilyAPIService.GetFamilyById(familyId).then(function (response) {
                familyEntity = response;
            });
        };

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && familyEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(familyEntity.Name, "Family");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Family");
            };

            function loadStaticData() {
                if (familyEntity != undefined)
                    $scope.scopeModel.name = familyEntity.Name;
            };

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        };

        function buildFamilyObjectFromScope() {
            var object = {
                FamilyId: (familyId != undefined) ? familyId : undefined,
                Name: $scope.scopeModel.name,
            };
            return object;
        };

        function insertFamily() {

            $scope.scopeModel.isLoading = true;
            var familyObject = buildFamilyObjectFromScope();
            return Demo_Module_FamilyAPIService.AddFamily(familyObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Family", response, "Name")) {
                    if ($scope.onFamilyAdded != undefined) {
                        $scope.onFamilyAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        };

        function updateFamily() {
            $scope.scopeModel.isLoading = true;
            var familyObject = buildFamilyObjectFromScope();
            Demo_Module_FamilyAPIService.UpdateFamily(familyObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Family", response, "Name")) {
                    if ($scope.onFamilyUpdated != undefined) {
                        $scope.onFamilyUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;

            });
        };

    };
    appControllers.controller('Demo_Module_FamilyEditorController', familyEditorController);
})(appControllers);