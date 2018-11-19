(function (appControllers) {

    "use strict";
    childEditorController.$inject = ['$scope', 'Demo_BestPractices_ChildAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function childEditorController($scope, Demo_BestPractices_ChildAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var childId;
        var childEntity;
        var parentIdItem;

        var parentDirectiveAPI;
        var parentDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var childShapeDirectiveAPI;
        var childShapeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                childId = parameters.childId;
                parentIdItem = parameters.parentIdItem;
            }

            isEditMode = (childId != undefined);
        };

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.disableParent = (parentIdItem != undefined);

            $scope.scopeModel.onParentDirectiveReady = function (api) {
                parentDirectiveAPI = api;
                parentDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onChildShapeDirectiveReady = function (api) {
                childShapeDirectiveAPI = api;
                childShapeDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.saveChild = function () {
                if (isEditMode)
                    return updateChild();
                else
                    return insertChild();

            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        };

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getChild().then(function () {
                    loadAllControls().finally(function () {
                        childEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                loadAllControls();
            }
        };

        function getChild() {
            return Demo_BestPractices_ChildAPIService.GetChildById(childId).then(function (response) {
                childEntity = response;
            });
        };

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && childEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(childEntity.Name, "Child");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Child");
            };

            function loadStaticData() {
                if (childEntity != undefined)
                    $scope.scopeModel.name = childEntity.Name;
            };

            function loadChildShapeDirective() {
                var childShapeLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                childShapeDirectiveReadyDeferred.promise.then(function () {

                    var childShapePayload;
                    if (childEntity != undefined && childEntity.Settings != undefined) {
                        childShapePayload = {
                            childShapeEntity: childEntity.Settings.ChildShape
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(childShapeDirectiveAPI, childShapePayload, childShapeLoadPromiseDeferred);
                });

                return childShapeLoadPromiseDeferred.promise;
            }

            function loadParentSelector() {
                var parentLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                parentDirectiveReadyDeferred.promise.then(function () {

                    var parentPayload = {};
                    if (parentIdItem != undefined) {
                        parentPayload.selectedIds = parentIdItem.ParentId;
                    }
                    if (childEntity != undefined) {
                        parentPayload.selectedIds = childEntity.ParentId;
                    }
                    VRUIUtilsService.callDirectiveLoad(parentDirectiveAPI, parentPayload, parentLoadPromiseDeferred);
                });

                return parentLoadPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadParentSelector, loadChildShapeDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        };


        function insertChild() {
            $scope.scopeModel.isLoading = true;

            var childObject = buildChildObjectFromScope();
            return Demo_BestPractices_ChildAPIService.AddChild(childObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Child", response, "Name")) {
                    if ($scope.onChildAdded != undefined) {
                        $scope.onChildAdded(response.InsertedObject);
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

        function updateChild() {
            $scope.scopeModel.isLoading = true;

            var childObject = buildChildObjectFromScope();
            Demo_BestPractices_ChildAPIService.UpdateChild(childObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Child", response, "Name")) {
                    if ($scope.onChildUpdated != undefined) {
                        $scope.onChildUpdated(response.UpdatedObject);
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

        function buildChildObjectFromScope() {
            var object = {
                ChildId: (childId != undefined) ? childId : undefined,
                Name: $scope.scopeModel.name,
                ParentId: parentDirectiveAPI.getSelectedIds(),
                Settings: {
                    ChildShape: childShapeDirectiveAPI.getData()
                }
            };
            return object;
        };


    };
    appControllers.controller('Demo_BestPractices_ChildEditorController', childEditorController);
})(appControllers);