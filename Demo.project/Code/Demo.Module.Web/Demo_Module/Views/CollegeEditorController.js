(function (appControllers) {

    "use strict";
  
    collegeEditorController.$inject = ['$scope', 'Demo_Module_CollegeAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function collegeEditorController($scope, Demo_Module_CollegeAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var collegeId;
        var collegeEntity;
        var context;

        var universityDirectiveApi;
        var universityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var infoDirectiveAPI;
        var infoReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var infoEntity;

        var descriptionDirectiveAPI;
        var descriptionReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var descriptionEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                collegeId = parameters.collegeId;
                context = parameters.context;
            }
            isEditMode = (collegeId != undefined);

        }

        function defineScope() {

            $scope.scopeModel = {};
            
            $scope.saveCollege = function () {
                if (isEditMode)
                    return updateCollege();
                else
                    return insertCollege();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.onUniversityDirectiveReady = function (api) {
                universityDirectiveApi = api;
                universityReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onCollegeInfoReady = function (api) {
                infoDirectiveAPI = api;
                infoReadyPromiseDeferred.resolve();
            };

            $scope.onDescriptionReady = function (api) {
                descriptionDirectiveAPI = api;
                descriptionReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            $scope.isLoading = true;
            if (isEditMode) {
                getCollege().then(function () {
                      loadAllControls()
                        .finally(function () {
                            collegeEntity = undefined;
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

        function getCollege() {
            return Demo_Module_CollegeAPIService.GetCollegeById(collegeId).then(function (collegeObject) {
                collegeEntity = collegeObject;
                infoEntity = collegeEntity.CollegeInfo;
                descriptionEntity = collegeEntity.DescriptionString;
            });
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadUniversitySelector, loadInfoDirective, loadDescriptionDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && collegeEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(collegeEntity.Name, "College");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("College");
        }

        function loadStaticData() {
            if (collegeEntity != undefined) {
                $scope.scopeModel.name = collegeEntity.Name;
            }
        }

        function loadUniversitySelector() {
            var universityLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            universityReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {
                    selectedIds: collegeEntity != undefined ? collegeEntity.UniversityId : undefined,
                };

                VRUIUtilsService.callDirectiveLoad(universityDirectiveApi, directivePayload, universityLoadPromiseDeferred);

            });
            return universityLoadPromiseDeferred.promise;
        }

        function loadInfoDirective() {
            var infoDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            infoReadyPromiseDeferred.promise.then(function () {
                var infoDirectivePayload;
                if (infoEntity != undefined) {
                    infoDirectivePayload = infoEntity;
                }
            
                VRUIUtilsService.callDirectiveLoad(infoDirectiveAPI, infoDirectivePayload, infoDeferredLoadPromiseDeferred);

            });
            return infoDeferredLoadPromiseDeferred.promise;
        }

        function loadDescriptionDirective() {
            var descriptionDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
           descriptionReadyPromiseDeferred.promise.then(function () {
               var descriptionDirectivePayload;
                if (descriptionEntity != undefined) {
                    descriptionDirectivePayload = descriptionEntity;
                }
                VRUIUtilsService.callDirectiveLoad(descriptionDirectiveAPI, descriptionDirectivePayload, descriptionDeferredLoadPromiseDeferred);

            });
            return descriptionDeferredLoadPromiseDeferred.promise;
        }

        function buildCollegeObjFromScope() {
            return{
                CollegeId: (collegeId != null) ? collegeId : 0,
                Name: $scope.scopeModel.name,
                UniversityId: $scope.scopeModel.selector.UniversityId,
                UniversityName: $scope.scopeModel.selector.Name,
                CollegeInfo: infoDirectiveAPI.getData(),
                DescriptionString: descriptionDirectiveAPI.getData()
            };
        }

        function insertCollege() {
            $scope.isLoading = true;

            var collegeObject = buildCollegeObjFromScope();
            return Demo_Module_CollegeAPIService.AddCollege(collegeObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("College", response, "Name")) {
                    if ($scope.onCollegeAdded != undefined) {
                        $scope.onCollegeAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function updateCollege() {
            $scope.isLoading = true;

            var collegeObject = buildCollegeObjFromScope();
            Demo_Module_CollegeAPIService.UpdateCollege(collegeObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("College", response, "Name")) {
                    if ($scope.onCollegeUpdated != undefined)
                        $scope.onCollegeUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }
    appControllers.controller('Demo_Module_CollegeEditorController', collegeEditorController);
})(appControllers);
