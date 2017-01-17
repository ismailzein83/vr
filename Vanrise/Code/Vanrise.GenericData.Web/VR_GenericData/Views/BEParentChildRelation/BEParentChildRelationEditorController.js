(function (appControllers) {

    "use strict";

    beParentChildRelationEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VRValidationService', 'VR_GenericData_BEParentChildRelationAPIService', 'VR_GenericData_BEParentChildRelationDefinitionAPIService'];

    function beParentChildRelationEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VRValidationService, VR_GenericData_BEParentChildRelationAPIService, VR_GenericData_BEParentChildRelationDefinitionAPIService) {

        var isEditMode;

        var beParentChildRelationId;
        var beParentChildRelationEntity;
        var beParentChildRelationDefinitionId;
        var beParentChildRelationDefinitionEntity;
        var parentId;
        var childId;
        var childFilter;

        var parentBESelectorAPI;
        var parentBESelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var childBESelectorAPI;
        var childBESelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                beParentChildRelationDefinitionId = parameters.beParentChildRelationDefinitionId;
                parentId = parameters.parentId;
                childId = parameters.childId;
            }
            isEditMode = (beParentChildRelationId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.isParentBESelectorDisabled = parentId != undefined ? true : false;
            $scope.scopeModel.isChildBESelectorDisabled = childId != undefined ? true : false;

            $scope.scopeModel.onParentBESelectorReady = function (api) {
                parentBESelectorAPI = api;
                parentBESelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onChildBESelectorReady = function (api) {
                childBESelectorAPI = api;
                childBESelectorReadyDeferred.resolve();
            };

            $scope.validateEffectiveDate = function () {
                return VRValidationService.validateTimeEqualorGreaterthanToday($scope.beginEffectiveDate);
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return updateBEParentChildRelation();
                }
                else {
                    return insertBEParentChildRelation();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            //$scope.scopeModel.hasSaveBEParentChildRelationPermission = function () {
            //    if ($scope.scopeModel.isEditMode)
            //        return VR_GenericData_BEParentChildRelationAPIService.HasUpdateBEParentChildRelationPermission();
            //    else
            //        return VR_GenericData_BEParentChildRelationAPIService.HasAddBEParentChildRelationPermission();
            //};
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                UtilsService.waitMultiplePromises([getBEParentChildRelationDefinition(), getBEParentChildRelation()]).then(function () {
                    loadAllControls()
                        .finally(function () {
                            beParentChildRelationEntity = undefined;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                UtilsService.waitMultiplePromises([getBEParentChildRelationDefinition()]).then(function () {
                    loadAllControls()
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
        }

        function getBEParentChildRelationDefinition() {
            return VR_GenericData_BEParentChildRelationDefinitionAPIService.GetBEParentChildRelationDefinition(beParentChildRelationDefinitionId).then(function (response) {
                beParentChildRelationDefinitionEntity = response;
            });
        }
        function getBEParentChildRelation() {
            return VR_GenericData_BEParentChildRelationAPIService.GetBEParentChildRelation(beParentChildRelationDefinitionId, beParentChildRelationId).then(function (response) {
                beParentChildRelationEntity = response;
            });
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadParentBESelector, loadChildBESelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        }
        function setTitle() {
            $scope.title =
                isEditMode ? UtilsService.buildTitleForUpdateEditor(beParentChildRelationEntity ? beParentChildRelationEntity.Name : undefined, 'BEParentChildRelation') : UtilsService.buildTitleForAddEditor('BEParentChildRelation');
        }
        function loadStaticData() {
            if (beParentChildRelationEntity == undefined)
                return;

            $scope.scopeModel.beginEffectiveDate = beParentChildRelationEntity.BED;
        }
        function loadParentBESelector() {
            var parentBESelectorLoadDeferred = UtilsService.createPromiseDeferred();

            parentBESelectorReadyDeferred.promise.then(function () {

                var parentBESelectorPayload = {
                    businessEntityDefinitionId: beParentChildRelationDefinitionEntity.Settings.ParentBEDefinitionId,
                    selectedIds: parentId
                };
                VRUIUtilsService.callDirectiveLoad(parentBESelectorAPI, parentBESelectorPayload, parentBESelectorLoadDeferred);
            });

            return parentBESelectorLoadDeferred.promise;
        }
        function loadChildBESelector() {
            var childBESelectorLoadDeferred = UtilsService.createPromiseDeferred();

            childBESelectorReadyDeferred.promise.then(function () {

                var childBESelectorPayload = {
                    filter: {
                        Filters: [{
                            $type: beParentChildRelationDefinitionEntity.Settings.ChildFilterFQTN,
                            ParentChildRelationDefinition: beParentChildRelationDefinitionId
                        }]
                    },
                    businessEntityDefinitionId: beParentChildRelationDefinitionEntity.Settings.ChildBEDefinitionId,
                    selectedIds: childId
                };
                VRUIUtilsService.callDirectiveLoad(childBESelectorAPI, childBESelectorPayload, childBESelectorLoadDeferred);
            });

            return childBESelectorLoadDeferred.promise;
        }

        function insertBEParentChildRelation() {
            $scope.scopeModel.isLoading = true;

            return VR_GenericData_BEParentChildRelationAPIService.AddBEParentChildRelation(buildBEParentChildRelationObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("BEParentChildRelation", response, "Name")) {
                        if ($scope.onBEParentChildRelationAdded != undefined)
                            $scope.onBEParentChildRelationAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }
        function updateBEParentChildRelation() {
            $scope.scopeModel.isLoading = true;

            return VR_GenericData_BEParentChildRelationAPIService.UpdateBEParentChildRelation(buildBEParentChildRelationObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("BEParentChildRelation", response, "Name")) {
                        if ($scope.onBEParentChildRelationUpdated != undefined)
                            $scope.onBEParentChildRelationUpdated(response.UpdatedObject);

                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function buildBEParentChildRelationObjFromScope() {
            var obj = {
                BEParentChildRelationId: beParentChildRelationId,
                RelationDefinitionId: beParentChildRelationDefinitionId,
                ParentBEId: parentBESelectorAPI.getSelectedIds(),
                ChildBEId: childBESelectorAPI.getSelectedIds(),
                BED: $scope.scopeModel.beginEffectiveDate
            };
            return obj;
        }
    }

    appControllers.controller('VR_GenericData_BEParentChildRelationEditorController', beParentChildRelationEditorController);

})(appControllers);
