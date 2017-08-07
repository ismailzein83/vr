(function (appControllers) {

    "use strict";

    beParentChildRelationEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VRValidationService', 'VR_GenericData_BEParentChildRelationAPIService', 'VR_GenericData_BEParentChildRelationDefinitionAPIService'];

    function beParentChildRelationEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VRValidationService, VR_GenericData_BEParentChildRelationAPIService, VR_GenericData_BEParentChildRelationDefinitionAPIService) {

        var isEditMode;

        var beParentChildRelationDefinitionId;
        var beParentChildRelationDefinitionEntity;
        var beParentChildRelationId;
        var beParentChildRelationEntity;
        var parentId;
        var childIds = [];

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
                beParentChildRelationId = parameters.beParentChildRelationId;
                parentId = parameters.parentId;

                if (parameters.childId != undefined)
                    childIds.push(parameters.childId);
            }

            isEditMode = (beParentChildRelationId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.isParentBESelectorDisabled = parentId != undefined ? true : false;
            $scope.scopeModel.isChildBESelectorDisabled = childIds.length > 0 ? true : false;
            $scope.scopeModel.errorMessage = "";

            $scope.scopeModel.onParentBESelectorReady = function (api) {
                parentBESelectorAPI = api;
                parentBESelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onChildBESelectorReady = function (api) {
                childBESelectorAPI = api;
                childBESelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onChildBESelectionChanged = function (selectedItem) {
                //var selectedChildBusinessEntityId = childBESelectorAPI.getSelectedIds();
                //if (!isEditMode && selectedChildBusinessEntityId != undefined) {
                //    $scope.scopeModel.isBeginEffectiveDateLoading = true;

                //    VR_GenericData_BEParentChildRelationAPIService.GetLastAssignedEED(beParentChildRelationDefinitionId, selectedChildBusinessEntityId).then(function (response) {
                //        $scope.scopeModel.beginEffectiveDate = response;
                //    }).catch(function (error) {
                //        VRNotificationService.notifyExceptionWithClose(error, $scope);
                //    }).finally(function (error) {
                //        $scope.scopeModel.isBeginEffectiveDateLoading = false;
                //    });
                //}
            };

            $scope.scopeModel.validateEffectiveDate = function () {
                return VRValidationService.validateTimeRange($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
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
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                UtilsService.waitMultiplePromises([getBEParentChildRelationDefinition(), getBEParentChildRelation()]).then(function () {
                    loadAllControls().finally(function () {
                        beParentChildRelationEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                UtilsService.waitMultiplePromises([getBEParentChildRelationDefinition()]).then(function () {
                    loadAllControls();
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
            var relationName = beParentChildRelationDefinitionEntity.Name;

            $scope.title =
                isEditMode ? UtilsService.buildTitleForUpdateEditor(beParentChildRelationEntity ? beParentChildRelationEntity.Name : undefined, relationName) : UtilsService.buildTitleForAddEditor(relationName);
        }
        function loadStaticData() {
            if (beParentChildRelationEntity == undefined)
                return;

            $scope.scopeModel.beginEffectiveDate = beParentChildRelationEntity.BED;
            $scope.scopeModel.endEffectiveDate = beParentChildRelationEntity.EED;
        }
        function loadParentBESelector() {
            var parentBESelectorLoadDeferred = UtilsService.createPromiseDeferred();

            parentBESelectorReadyDeferred.promise.then(function () {

                var parentBESelectorPayload = {
                    businessEntityDefinitionId: beParentChildRelationDefinitionEntity.Settings.ParentBEDefinitionId,
                    selectedIds: parentId,
                    beRuntimeSelectorFilter: beParentChildRelationDefinitionEntity.Settings.ParentBERuntimeSelectorFilter
                };
                VRUIUtilsService.callDirectiveLoad(parentBESelectorAPI, parentBESelectorPayload, parentBESelectorLoadDeferred);
            });

            return parentBESelectorLoadDeferred.promise;
        }
        function loadChildBESelector() {
            var childBESelectorLoadDeferred = UtilsService.createPromiseDeferred();

            childBESelectorReadyDeferred.promise.then(function () {

                var childBESelectorPayload = {
                    businessEntityDefinitionId: beParentChildRelationDefinitionEntity.Settings.ChildBEDefinitionId,
                    beRuntimeSelectorFilter: beParentChildRelationDefinitionEntity.Settings.ChildBERuntimeSelectorFilter
                };

                if (childIds.length == 0) {
                    childBESelectorPayload.filter = {
                        Filters: [{
                            $type: beParentChildRelationDefinitionEntity.Settings.ChildFilterFQTN,
                            ParentChildRelationDefinitionId: beParentChildRelationDefinitionId
                        }]
                    };
                } else {
                    childBESelectorPayload.selectedIds = childIds;
                }
                VRUIUtilsService.callDirectiveLoad(childBESelectorAPI, childBESelectorPayload, childBESelectorLoadDeferred);
            });

            return childBESelectorLoadDeferred.promise;
        }

        function insertBEParentChildRelation() {
            $scope.scopeModel.isLoading = true;

            var beParentChildRelationObjToAdd = buildBEParentChildrenRelationObjFromScope();

            return VR_GenericData_BEParentChildRelationAPIService.AddBEParentChildrenRelation(beParentChildRelationObjToAdd).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded(beParentChildRelationDefinitionEntity.Name, response, "Name")) {
                    if ($scope.onBEParentChildRelationAdded != undefined && beParentChildRelationObjToAdd.ChildBEIds.length == 1)
                        $scope.onBEParentChildRelationAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
                else if (beParentChildRelationObjToAdd.ChildBEIds.length == 1) {
                    $scope.scopeModel.errorMessage = "";
                }
                else if ((beParentChildRelationObjToAdd.ChildBEIds.length > 1)) {
                    $scope.scopeModel.errorMessage = "* " + response.Message;
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function updateBEParentChildRelation() {
            $scope.scopeModel.isLoading = true;

            buildBEParentChildRelationObjFromScope()

            return VR_GenericData_BEParentChildRelationAPIService.UpdateBEParentChildRelation(buildBEParentChildRelationObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated(beParentChildRelationDefinitionEntity.Name, response, "Name")) {
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

        function buildBEParentChildrenRelationObjFromScope() {
            var obj = {
                BEParentChildRelationId: beParentChildRelationId,
                RelationDefinitionId: beParentChildRelationDefinitionId,
                ParentBEId: parentBESelectorAPI.getSelectedIds(),
                ChildBEIds: childBESelectorAPI.getSelectedIds(),
                BED: $scope.scopeModel.beginEffectiveDate,
                EED: $scope.scopeModel.endEffectiveDate
            };
            return obj;
        }
        function buildBEParentChildRelationObjFromScope() {

            var childBEIds = childBESelectorAPI.getSelectedIds();

            var obj = {
                BEParentChildRelationId: beParentChildRelationId,
                RelationDefinitionId: beParentChildRelationDefinitionId,
                ParentBEId: parentBESelectorAPI.getSelectedIds(),
                ChildBEId: childBEIds[0],
                BED: $scope.scopeModel.beginEffectiveDate,
                EED: $scope.scopeModel.endEffectiveDate
            };
            return obj;
        }
    }

    appControllers.controller('VR_GenericData_BEParentChildRelationEditorController', beParentChildRelationEditorController);

})(appControllers);
