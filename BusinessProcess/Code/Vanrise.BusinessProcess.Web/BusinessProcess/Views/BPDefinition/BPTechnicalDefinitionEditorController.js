BPTechnicalDefinitionEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'BusinessProcess_BPDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

function BPTechnicalDefinitionEditorController($scope, VRNavigationService, VRNotificationService, BusinessProcess_BPDefinitionAPIService, UtilsService, VRUIUtilsService) {

    var businessProcessDefinitionEntity;
    var businessProcessDefinitionId;

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

        if (parameters != undefined) {
            businessProcessDefinitionId = parameters.businessProcessDefinitionId;
        }
    }

    function defineScope() {
        $scope.scopeModel = {};

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
            return update();
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
    }

    function load() {
        $scope.scopeModel.isLoading = true;

        getBusinessProcessDefinition().then(function () {
            loadAllControls()
        }).catch(function () {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
            $scope.scopeModel.isLoading = false;
        });
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadViewRequiredPermission, loadStartNewInstanceRequiredPermission, loadScheduleTaskRequiredPermission]).then(function () {

        }).finally(function () {
            $scope.scopeModel.isLoading = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });

        function setTitle() {
            $scope.title = UtilsService.buildTitleForUpdateEditor(businessProcessDefinitionEntity.Title, 'Business Process Editor');
        }

        function loadStaticData() {
            $scope.scopeModel.title = businessProcessDefinitionEntity.Title;
            $scope.scopeModel.MaxConcurrentWorkflows = businessProcessDefinitionEntity.Configuration.MaxConcurrentWorkflows;
            $scope.scopeModel.NotVisibleInManagementScreen = businessProcessDefinitionEntity.Configuration.NotVisibleInManagementScreen;
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
    }

    function getBusinessProcessDefinition() {
        return BusinessProcess_BPDefinitionAPIService.GetBPDefintion(businessProcessDefinitionId).then(function (response) {
            businessProcessDefinitionEntity = response;
        });
    }

    function buildBPEntityObjFromScope() {
        var obj = businessProcessDefinitionEntity;
        obj.Title = $scope.scopeModel.title;
        obj.Configuration.MaxConcurrentWorkflows = $scope.scopeModel.MaxConcurrentWorkflows;
        obj.Configuration.NotVisibleInManagementScreen = $scope.scopeModel.NotVisibleInManagementScreen;
        obj.Configuration.Security = {
            View: viewPermissionAPI.getData(),
            StartNewInstance: startNewInstancePermissionAPI.getData(),
            ScheduleTask: scheduleTaskPermissionAPI.getData()
        };
        return obj;
    }

    function update() {
        $scope.scopeModel.isLoading = true;

        return BusinessProcess_BPDefinitionAPIService.UpdateBPDefinition(buildBPEntityObjFromScope()).then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated(businessProcessDefinitionEntity.Title, response, 'Title')) {
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
}

appControllers.controller('BusinessProcess_BP_TechnicalDefinitionEditorController', BPTechnicalDefinitionEditorController);
