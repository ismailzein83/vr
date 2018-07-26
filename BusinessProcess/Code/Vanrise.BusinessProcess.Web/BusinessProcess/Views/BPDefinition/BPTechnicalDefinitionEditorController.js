(function (appControllers) {

    "use strict";

    BPTechnicalDefinitionEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'BusinessProcess_BPDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function BPTechnicalDefinitionEditorController($scope, VRNavigationService, VRNotificationService, BusinessProcess_BPDefinitionAPIService, UtilsService, VRUIUtilsService) {

        var isEditMode;

        var businessProcessDefinitionId;
        var businessProcessDefinitionEntity;

        var vrWorkflowSelectorAPI;
        var vrWorkflowSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var viewPermissionAPI;
        var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var startNewInstancePermissionAPI;
        var startNewInstancePermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var scheduleTaskPermissionAPI;
        var scheduleTaskPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                businessProcessDefinitionId = parameters.businessProcessDefinitionId;
            }

            isEditMode = (businessProcessDefinitionId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.loadVRWorklowSelector = isEditMode ? false : true;

            $scope.scopeModel.onVRWorkflowSelectorReady = function (api) {
                vrWorkflowSelectorAPI = api;
                vrWorkflowSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onViewRequiredPermissionReady = function (api) {
                viewPermissionAPI = api;
                viewPermissionReadyDeferred.resolve();
            };

            $scope.scopeModel.onStartNewInstanceRequiredPermissionReady = function (api) {
                startNewInstancePermissionAPI = api;
                startNewInstancePermissionReadyDeferred.resolve();
            };

            $scope.scopeModel.onScheduleTaskRequiredPermissionReady = function (api) {
                scheduleTaskPermissionAPI = api;
                scheduleTaskPermissionReadyDeferred.resolve();
            };

            $scope.saveBPDefinition = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getBusinessProcessDefinition().then(function () {
                    if (businessProcessDefinitionEntity.VRWorkflowId != undefined) {
                        $scope.scopeModel.loadVRWorklowSelector = true;
                    }
                    loadAllControls();
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getBusinessProcessDefinition() {
            return BusinessProcess_BPDefinitionAPIService.GetBPDefintion(businessProcessDefinitionId).then(function (response) {
                businessProcessDefinitionEntity = response;
            });
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode) {
                    $scope.title = UtilsService.buildTitleForUpdateEditor(businessProcessDefinitionEntity.Title, 'Business Process Editor');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Business Process Editor');
                }
            }
            function loadStaticData() {
                if (!isEditMode)
                    return;

                $scope.scopeModel.title = businessProcessDefinitionEntity.Title;
                $scope.scopeModel.MaxConcurrentWorkflows = businessProcessDefinitionEntity.Configuration.MaxConcurrentWorkflows;
                $scope.scopeModel.NotVisibleInManagementScreen = businessProcessDefinitionEntity.Configuration.NotVisibleInManagementScreen;
            }
            function loadVRWorkflowSelector() {
                if (!$scope.scopeModel.loadVRWorklowSelector)
                    return;

                var vrWorkflowLoadDeferred = UtilsService.createPromiseDeferred();

                vrWorkflowSelectorReadyDeferred.promise.then(function () {

                    var vrWorkflowSelectorPayload;
                    if (businessProcessDefinitionEntity != undefined && businessProcessDefinitionEntity.VRWorkflowId) {
                        vrWorkflowSelectorPayload = { selectedIds: businessProcessDefinitionEntity.VRWorkflowId };
                    }
                    VRUIUtilsService.callDirectiveLoad(vrWorkflowSelectorAPI, vrWorkflowSelectorPayload, vrWorkflowLoadDeferred);
                });

                return vrWorkflowLoadDeferred.promise;
            }
            function loadViewRequiredPermission() {
                var viewPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                viewPermissionReadyDeferred.promise.then(function () {
                    var payload;

                    if (businessProcessDefinitionEntity != undefined && businessProcessDefinitionEntity.Configuration != undefined && businessProcessDefinitionEntity.Configuration.Security != undefined && businessProcessDefinitionEntity.Configuration.Security.View != null) {
                        payload = {
                            data: businessProcessDefinitionEntity.Configuration.Security.View
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(viewPermissionAPI, payload, viewPermissionLoadDeferred);
                });

                return viewPermissionLoadDeferred.promise;
            }
            function loadStartNewInstanceRequiredPermission() {
                var startNewInstancePermissionLoadDeferred = UtilsService.createPromiseDeferred();

                startNewInstancePermissionReadyDeferred.promise.then(function () {
                    var payload;

                    if (businessProcessDefinitionEntity != undefined && businessProcessDefinitionEntity.Configuration != undefined && businessProcessDefinitionEntity.Configuration.Security != undefined && businessProcessDefinitionEntity.Configuration.Security.StartNewInstance != null) {
                        payload = {
                            data: businessProcessDefinitionEntity.Configuration.Security.StartNewInstance
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(startNewInstancePermissionAPI, payload, startNewInstancePermissionLoadDeferred);
                });

                return startNewInstancePermissionLoadDeferred.promise;
            }
            function loadScheduleTaskRequiredPermission() {
                var scheduleTaskPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                scheduleTaskPermissionReadyDeferred.promise.then(function () {
                    var payload;

                    if (businessProcessDefinitionEntity != undefined && businessProcessDefinitionEntity.Configuration != undefined && businessProcessDefinitionEntity.Configuration.Security != undefined && businessProcessDefinitionEntity.Configuration.Security.ScheduleTask != null) {
                        payload = {
                            data: businessProcessDefinitionEntity.Configuration.Security.ScheduleTask
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(scheduleTaskPermissionAPI, payload, scheduleTaskPermissionLoadDeferred);
                });

                return scheduleTaskPermissionLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadVRWorkflowSelector, loadViewRequiredPermission, loadStartNewInstanceRequiredPermission, loadScheduleTaskRequiredPermission]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function insert() {
            $scope.scopeModel.isLoading = true;

            return BusinessProcess_BPDefinitionAPIService.AddBPDefinition(buildBPEntityObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('BP Definition', response, 'Title')) {
                    if ($scope.onBPDefenitionAdded != undefined)
                        $scope.onBPDefenitionAdded(response.InsertedObject);
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

            return BusinessProcess_BPDefinitionAPIService.UpdateBPDefinition(buildBPEntityObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('BP Definition', response, 'Title')) {
                    if ($scope.onBPDefenitionUpdated != undefined)
                        $scope.onBPDefenitionUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildBPEntityObjFromScope() {
            var obj;

            if (isEditMode) {
                obj = businessProcessDefinitionEntity;
            } else {
                obj = { Configuration: {} };
            };

            obj.Title = $scope.scopeModel.title;
            obj.VRWorkflowId = $scope.scopeModel.loadVRWorklowSelector ? vrWorkflowSelectorAPI.getSelectedIds() : null;
            obj.Configuration.MaxConcurrentWorkflows = $scope.scopeModel.MaxConcurrentWorkflows;
            obj.Configuration.NotVisibleInManagementScreen = $scope.scopeModel.NotVisibleInManagementScreen;
            obj.Configuration.Security = {
                View: viewPermissionAPI.getData(),
                StartNewInstance: startNewInstancePermissionAPI.getData(),
                ScheduleTask: scheduleTaskPermissionAPI.getData()
            };
            return obj;
        }
    }

    appControllers.controller('BusinessProcess_BP_TechnicalDefinitionEditorController', BPTechnicalDefinitionEditorController);
})(appControllers);
