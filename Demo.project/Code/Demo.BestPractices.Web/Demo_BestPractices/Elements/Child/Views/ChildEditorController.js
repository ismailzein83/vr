(function (appControllers) {

    "use strict";
    childEditorController.$inject = ['$scope', 'Demo_BestPractices_ChildAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function childEditorController($scope, Demo_BestPractices_ChildAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var childId;
        var childEntity;

        var parentIdItem;

        var parentDirectiveApi;
        var parentReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var childShapeDirectiveApi;
        var childShapeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.disableParent = parentIdItem != undefined;
            $scope.scopeModel.onParentDirectiveReady = function (api) {
                parentDirectiveApi = api;
                parentReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onChildShapeDirectiveReady = function (api) {
                childShapeDirectiveApi = api;
                childShapeReadyPromiseDeferred.resolve();
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
            else
                loadAllControls();
        };

        function getChild() {
            return Demo_BestPractices_ChildAPIService.GetChildById(childId).then(function (response) {
                childEntity = response;
            });
        };

        function loadAllControls() {

            function loadChildShapeDirective() {
                var childShapeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                childShapeReadyPromiseDeferred.promise.then(function () {
                    var childShapePayload;
                    if (childEntity != undefined && childEntity.Settings != undefined)
                        childShapePayload = {
                            childShapeEntity: childEntity.Settings.ChildShape
                        };
                    VRUIUtilsService.callDirectiveLoad(childShapeDirectiveApi, childShapePayload, childShapeLoadPromiseDeferred);
                });
                return childShapeLoadPromiseDeferred.promise;
            }

            function loadParentSelector() {
                var parentLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                parentReadyPromiseDeferred.promise.then(function () {
                    var parentPayload = {};
                    if (parentIdItem != undefined)
                        parentPayload.selectedIds = parentIdItem.ParentId;
                   
                    if (childEntity != undefined)
                        parentPayload.selectedIds = childEntity.ParentId;

                    VRUIUtilsService.callDirectiveLoad(parentDirectiveApi, parentPayload, parentLoadPromiseDeferred);
                });
                return parentLoadPromiseDeferred.promise;

            }

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

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadParentSelector, loadChildShapeDirective])
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        };

        function buildChildObjectFromScope() {
            var object = {
                ChildId: (childId != undefined) ? childId : undefined,
                Name: $scope.scopeModel.name,
                ParentId: parentDirectiveApi.getSelectedIds(),
                Settings: {
                    ChildShape: childShapeDirectiveApi.getData()
                }
            };
            return object;
        };

        function insertChild() {

            $scope.scopeModel.isLoading = true;
            var childObject = buildChildObjectFromScope();
            return Demo_BestPractices_ChildAPIService.AddChild(childObject)
            .then(function (response) {
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

    };
    appControllers.controller('Demo_BestPractices_ChildEditorController', childEditorController);
})(appControllers);