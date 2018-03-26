(function (appControllers) {

    "use strict";

    universityEditorController.$inject = ['$scope', 'Demo_Module_UniversityAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function universityEditorController($scope, Demo_Module_UniversityAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var universityId;
        var universityEntity;
        var context;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                universityId = parameters.universityId;
                context = parameters.context;
            }
            isEditMode = (universityId != undefined);
        }

        function defineScope() {

            $scope.saveUniversity = function () {
                if (isEditMode)
                    return updateUniversity();
                else
                    return insertUniversity();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {

            $scope.isLoading = true;

            if (isEditMode) {
                getUniversity().then(function () {
                    loadAllControls()
                        .finally(function () {
                            universityEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getUniversity() {
            return Demo_Module_UniversityAPIService.GetUniversityById(universityId).then(function (universityObject) {
                universityEntity = universityObject;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && universityEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(universityEntity.Name, "University");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("University");
        }

        function loadStaticData() {
            if (universityEntity != undefined)
                $scope.name = universityEntity.Name;
        }

        function buildUniversityObjFromScope() {
            return {
                UniversityId: universityId,
                Name: $scope.name,
            };
        }

        function insertUniversity() {
            $scope.isLoading = true;

            var universityObject = buildUniversityObjFromScope();
            return Demo_Module_UniversityAPIService.AddUniversity(universityObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("University", response, "Name")) {
                    if ($scope.onUniversityAdded != undefined) {
                        $scope.onUniversityAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateUniversity() {
            $scope.isLoading = true;

            var universityObject = buildUniversityObjFromScope();
            Demo_Module_UniversityAPIService.UpdateUniversity(universityObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("University", response, "Name")) {
                    if ($scope.onUniversityUpdated != undefined)
                        $scope.onUniversityUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }
    appControllers.controller('Demo_Module_UniversityEditorController', universityEditorController);
})(appControllers);
