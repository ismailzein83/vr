(function (appControllers) {

    "use strict";

    VRDynamicCodeEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'VRCommon_VRNamespaceService', 'VRCommon_VRNamespaceItemAPIService', 'VRCommon_VRNamespaceItemService'];

    function VRDynamicCodeEditorController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService, VRCommon_VRNamespaceService,
        VRCommon_VRNamespaceItemAPIService, VRCommon_VRNamespaceItemService) {

        var isEditMode;
        var vrNamespaceItemId;
        var vrNamespaceItem
        var vrNamespaceId;
        var vrDynamicCodeSettingsDirectiveApi;

        var vrDynamicCodeSettingsDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                vrNamespaceItemId = parameters.vrNameSpaceItemId; //edit mode
                vrNamespaceId = parameters.vrNamespaceId;  // add mode
            }

            isEditMode = (vrNamespaceItemId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onVRDynamicCodeSettingsReady = function (api) {
                vrDynamicCodeSettingsDirectiveApi = api;
                vrDynamicCodeSettingsDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.tryCompileNamespace = function () {
                return tryCompileNamespaceItem();
            };

            $scope.scopeModel.save = function () {

                var promiseDeferred = UtilsService.createPromiseDeferred();

                tryCompileNamespaceItem().then(function (response) {
                    if (response) { // success compile
                        var savePromise;
                        if (isEditMode)
                            savePromise = update();
                        else
                            savePromise = insert();

                        savePromise.then(function () { // success update/insert
                            promiseDeferred.resolve();
                        }).catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                            promiseDeferred.reject();
                        });
                    }
                    else { // compilation wrong
                        promiseDeferred.resolve();
                    }
                }).catch(function (error) { // compilation error
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
                getVRNamespaceItem().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else
                loadAllControls();
        }

        function getVRNamespaceItem() {
            return VRCommon_VRNamespaceItemAPIService.GetVRNamespaceItem(vrNamespaceItemId).then(function (response) {
                vrNamespaceItem = response;
                vrNamespaceId = vrNamespaceItem.VRNamespaceId;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadVRDynamicCodeSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var vrDynamicCodeTitle = (vrNamespaceItem != undefined) ? vrNamespaceItem.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrDynamicCodeTitle, 'Dynamic Code');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Dynamic Code');
                }
            }
            function loadStaticData() {
                if (vrNamespaceItem == undefined)
                    return;
                $scope.scopeModel.title = vrNamespaceItem.Name;
            }

            function loadVRDynamicCodeSettingsDirective() {

                var promises = [];
                var vrDynamicCodeSettingsDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                promises.push(vrDynamicCodeSettingsDirectiveReadyPromiseDeferred.promise);
                UtilsService.waitMultiplePromises(promises).then(function (response) {
                    if (isEditMode) {
                        if (vrNamespaceItem.Settings != undefined) {
                            if (vrNamespaceItemId != undefined)
                                var settingsPayload = { Settings: vrNamespaceItem.Settings.Code };
                            else
                                var settingsPayload = { Settings: vrNamespaceItem.Settings };
                        }
                    }
                    VRUIUtilsService.callDirectiveLoad(vrDynamicCodeSettingsDirectiveApi, settingsPayload, vrDynamicCodeSettingsDirectiveLoadPromiseDeferred);
                });
                $scope.scopeModel.isLoading = false;
                return vrDynamicCodeSettingsDirectiveLoadPromiseDeferred.promise;
            }

        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return VRCommon_VRNamespaceItemAPIService.AddVRNamespaceItem(buildVRNamespaceItemObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Namespace Item', response, 'Name')) {
                    if ($scope.onVRNameSpaceItemAdded != undefined) {
                        $scope.onVRNameSpaceItemAdded(response.InsertedObject);
                    }
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
            return VRCommon_VRNamespaceItemAPIService.UpdateVRNamespaceItem(buildVRNamespaceItemObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Namespace Item', response, 'Name')) {
                    if ($scope.onVRNameSpaceItemUpdated != undefined) {
                        $scope.onVRNameSpaceItemUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildVRNamespaceItemObjFromScope() {
            return {
                VRNamespaceItemId: vrNamespaceItemId,
                VRNamespaceId: vrNamespaceId,
                Name: $scope.scopeModel.title,
                Settings: {
                    Code: vrDynamicCodeSettingsDirectiveApi.getData()
                }
            };
        }

        function tryCompileNamespaceItem() {
            var promiseDeferred = UtilsService.createPromiseDeferred();
            $scope.scopeModel.isLoading = true;
            var namespaceItemObj = buildVRNamespaceItemObjFromScope();
            VRCommon_VRNamespaceItemAPIService.TryCompileNamespaceItem(namespaceItemObj).then(function (response) {
                if (response.Result) { // success compilation 
                    VRNotificationService.showSuccess("Namespace compiled successfully.");
                    promiseDeferred.resolve(true);
                }
                else { // wrong compilation 
                    VRCommon_VRNamespaceItemService.tryCompilationResult(response.ErrorMessages, namespaceItemObj);
                    promiseDeferred.resolve(false);
                }
            }).catch(function (error) { // error in compilation 
                VRNotificationService.notifyException(error, $scope);
                promiseDeferred.reject(error);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
            return promiseDeferred.promise;
        }
    }

    appControllers.controller('VRCommon_VRDynamicCodeEditorController', VRDynamicCodeEditorController);

})(appControllers);