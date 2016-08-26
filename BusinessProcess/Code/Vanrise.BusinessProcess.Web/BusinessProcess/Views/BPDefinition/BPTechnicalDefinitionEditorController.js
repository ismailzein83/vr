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
        if (parameters != undefined && parameters != null) {
            businessProcessDefinitionId = parameters.businessProcessDefinitionId;
        }
    }

    function defineScope() {
        $scope.scopeModal = {};

        $scope.scopeModal.onViewRequiredPermissionReady = function (api) {
            viewPermissionAPI = api;
            viewPermissionReadyDeferred.resolve();
        }
        $scope.scopeModal.onStartNewInstanceRequiredPermissionReady = function (api) {
            startNewInstancePermissionAPI = api;
            startNewInstancePermissionReadyDeferred.resolve();
        }
        $scope.scopeModal.onScheduleTaskRequiredPermissionReady = function (api) {
            scheduleTaskPermissionAPI = api;
            scheduleTaskPermissionReadyDeferred.resolve();
        }
        $scope.saveBPDefinition = function () {
            return update();
        };
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

    }

    function load() {
        $scope.scopeModal.isLoading = true;
        getBusinessProcessDefinition().then(function () {
            loadAllControls()
        }).catch(function () {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
            $scope.scopeModal.isLoading = false;
        });
    }


    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadViewRequiredPermission, loadStartNewInstanceRequiredPermission, loadScheduleTaskRequiredPermission]).then(function () {

        }).finally(function () {
            $scope.scopeModal.isLoading = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });


        function setTitle() {
            $scope.title = UtilsService.buildTitleForUpdateEditor(businessProcessDefinitionEntity.Title, 'Business Process Editor');
        }
        function loadStaticData() {

            $scope.scopeModal.title = businessProcessDefinitionEntity.Title;
            $scope.scopeModal.MaxConcurrentWorkflows = businessProcessDefinitionEntity.Configuration.MaxConcurrentWorkflows;
            $scope.scopeModal.NotVisibleInManagementScreen = businessProcessDefinitionEntity.Configuration.NotVisibleInManagementScreen;
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
        obj.Title = $scope.scopeModal.title;
        obj.Configuration.MaxConcurrentWorkflows = $scope.scopeModal.MaxConcurrentWorkflows;
        obj.Configuration.NotVisibleInManagementScreen = $scope.scopeModal.NotVisibleInManagementScreen;
        obj.Configuration.Security = {
            View: viewPermissionAPI.getData(),
            StartNewInstance: startNewInstancePermissionAPI.getData(),
            ScheduleTask: scheduleTaskPermissionAPI.getData()
        }
        
        return obj;
    }

   

    function update() {
        $scope.scopeModal.isLoading = true;

        return BusinessProcess_BPDefinitionAPIService.UpdateBPDefinition(buildBPEntityObjFromScope()).then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated(businessProcessDefinitionEntity.Title, response, 'Title')) {
                if ($scope.onBPDefenitionUpdated != undefined)
                    $scope.onBPDefenitionUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        }).finally(function () {
            $scope.scopeModal.isLoading = false;
        });
    }
       
}
appControllers.controller('BusinessProcess_BP_TechnicalDefinitionEditorController', BPTechnicalDefinitionEditorController);
