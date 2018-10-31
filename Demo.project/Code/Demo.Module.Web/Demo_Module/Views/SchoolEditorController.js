(function (appControllers) {

    "use strict";
    schoolEditorController.$inject = ['$scope', 'Demo_Module_SchoolAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function schoolEditorController($scope, Demo_Module_SchoolAPIService, VRNotificationService, VRNavigationService, UtilsService) {

        var isEditMode;
        var schoolId;
        var schoolEntity;

        
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                schoolId = parameters.schoolId;
            }
            isEditMode = (schoolId != undefined);
        };

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.saveSchool = function () {
                if (isEditMode)
                    return updateSchool();
                else
                    return insertSchool();

            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        };

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getSchool().then(function () {
                    loadAllControls().finally(function () {
                        schoolEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else
                loadAllControls();
        };

        function getSchool() {
            return Demo_Module_SchoolAPIService.GetSchoolById(schoolId).then(function (response) {
                schoolEntity = response;
            });
        };

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && schoolEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(schoolEntity.Name, "School");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("School");
            };

            function loadStaticData() {
                if (schoolEntity != undefined) {
                    $scope.scopeModel.name = schoolEntity.Name;
                }

            };

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        };

        function buildSchoolObjectFromScope() {
            var object = {
                SchoolId: (schoolId != undefined) ? schoolId : undefined,
                Name: $scope.scopeModel.name
            };
            return object;
        };

        function insertSchool() {

            $scope.scopeModel.isLoading = true;
            var schoolObject = buildSchoolObjectFromScope();
            return Demo_Module_SchoolAPIService.AddSchool(schoolObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("School", response, "Name")) {
                    if ($scope.onSchoolAdded != undefined) {
                        $scope.onSchoolAdded(response.InsertedObject);
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

        function updateSchool() {
            $scope.scopeModel.isLoading = true;
            var schoolObject = buildSchoolObjectFromScope();
            Demo_Module_SchoolAPIService.UpdateSchool(schoolObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("School", response, "Name")) {
                    if ($scope.onSchoolUpdated != undefined) {
                        $scope.onSchoolUpdated(response.UpdatedObject);
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
    appControllers.controller('Demo_Module_SchoolEditorController', schoolEditorController);
})(appControllers);