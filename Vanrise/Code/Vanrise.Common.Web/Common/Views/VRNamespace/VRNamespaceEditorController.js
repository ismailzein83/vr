(function (appControllers) {

    "use strict";

    VRNamespaceEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'VRCommon_VRNamespaceAPIService', 'VRCommon_VRNamespaceService'];

    function VRNamespaceEditorController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService, VRCommon_VRNamespaceAPIService, VRCommon_VRNamespaceService) {

        var isEditMode;

        var vrNamespaceId;
        var vrNamespaceEntity;
        
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                vrNamespaceId = parameters.vrNamespaceId;
            }

            isEditMode = (vrNamespaceId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.tryCompileNamespace = function () {
                return tryCompileNamespace();
            };

            $scope.scopeModel.save = function () {

                var promiseDeferred = UtilsService.createPromiseDeferred();
                tryCompileNamespace().then(function (response) {
                    if (response) {
                        var savePromise;
                        if (isEditMode)
                            savePromise = update();
                        else
                            savePromise = insert();

                        savePromise.then(function () {
                            promiseDeferred.resolve();
                        }).catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                            promiseDeferred.reject();
                        });
                    }
                    else {
                        promiseDeferred.resolve();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    promiseDeferred.reject();
                });

                return promiseDeferred.promise;
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getVRNamespace().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getVRNamespace() {
            return VRCommon_VRNamespaceAPIService.GetVRNamespace(vrNamespaceId).then(function (response) {
                vrNamespaceEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var vrNamespaceName = (vrNamespaceEntity != undefined) ? vrNamespaceEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrNamespaceName, 'Namespace');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Namespace');
                }
            }
            function loadStaticData() {
                if (vrNamespaceEntity == undefined)
                    return;
                $scope.scopeModel.namespace = vrNamespaceEntity.Name;
                $scope.scopeModel.code = vrNamespaceEntity.Settings.Code;
            }
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return VRCommon_VRNamespaceAPIService.AddVRNamespace(buildVRNamespaceObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('VRNamespace', response, 'Name')) {
                    if ($scope.onVRNamespaceAdded != undefined)
                        $scope.onVRNamespaceAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            return VRCommon_VRNamespaceAPIService.UpdateVRNamespace(buildVRNamespaceObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('VRNamespace', response, 'Name')) {
                    if ($scope.onVRNamespaceUpdated != undefined) {
                        $scope.onVRNamespaceUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildVRNamespaceObjFromScope() {
            return {
                VRNamespaceId: vrNamespaceEntity != undefined ? vrNamespaceEntity.VRNamespaceId : undefined,
                Name: $scope.scopeModel.namespace,
                Settings:{
                    Code: $scope.scopeModel.code
                } 
            };
        }

        function tryCompileNamespace() {
            var promiseDeferred = UtilsService.createPromiseDeferred();
            $scope.scopeModel.isLoading = true;
            var namespaceObj = buildVRNamespaceObjFromScope();
            VRCommon_VRNamespaceAPIService.TryCompileNamespace(namespaceObj).then(function (response) {
                if (response.Result) {
                    VRNotificationService.showSuccess("Namespace compiled successfully.");
                    promiseDeferred.resolve(true);
                }
                else {
                    VRCommon_VRNamespaceService.tryCompilationResult(response.ErrorMessages, namespaceObj);
                    promiseDeferred.resolve(false);
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
                promiseDeferred.reject(error);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
            return promiseDeferred.promise;
        }
    }

    appControllers.controller('VRCommon_VRNamespaceEditorController', VRNamespaceEditorController);

})(appControllers);