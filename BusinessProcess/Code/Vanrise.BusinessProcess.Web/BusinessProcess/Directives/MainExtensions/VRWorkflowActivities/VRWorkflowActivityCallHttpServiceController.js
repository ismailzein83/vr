(function (appControllers) {

    "use strict";

    CallHttpServiceEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowService', 'VRWorkflowCallHttpServiceMethodEnum', 'VRWorkflowCallHttpServiceMessageFormatEnum', 'VRWorkflowCallHttpRetrySettingsEnum'];

    function CallHttpServiceEditorController($scope, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowService, VRWorkflowCallHttpServiceMethodEnum, VRWorkflowCallHttpServiceMessageFormatEnum, VRWorkflowCallHttpRetrySettingsEnum) {

        var serviceName;
        var connectionId;
        var callHttpServiceMethod;
        var actionPath;
        var buildBodyLogic;
        var callHttpServiceMessageFormat;
        var callHttpRetrySettings;
        var responseLogic;
        var errorLogic;
        var isSucceeded;
        var continueWorkflowIfCallFailed;
        var classMembersCode;
        var enableVisualization;
        var headers;
        var urlParameters;
        var context;
        var isNew;

        var connectionSelectorAPI;
        var connectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var headersGridAPI;
        var headersGridReadyDeferred = UtilsService.createPromiseDeferred();

        var urlParametersGridAPI;
        var urlParametersGridReadyDeferred = UtilsService.createPromiseDeferred();

        var isSucceededExpressionBuilderDirectiveAPI;
        var isSucceededExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();

        var actionPathExpressionBuilderDirectiveAPI;
        var actionPathExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();

        var responseLogicExpressionBuilderDirectiveAPI;
        var responseLogicExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();

        var bodyLogicExpressionBuilderDirectiveAPI;
        var bodyLogicExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();

        var errorLogicExpressionBuilderDirectiveAPI;
        var errorLogicExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();
                       

        loadParameters();
        defineScope();
        load();



        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters.obj != undefined) {
                serviceName = parameters.obj.ServiceName;
                connectionId = parameters.obj.ConnectionId;
                callHttpServiceMethod = parameters.obj.Method;
                actionPath = parameters.obj.ActionPath;
                buildBodyLogic = parameters.obj.BuildBodyLogic;
                callHttpServiceMessageFormat = parameters.obj.MessageFormat;
                callHttpRetrySettings = parameters.obj.RetrySettings;
                responseLogic = parameters.obj.ResponseLogic;
                errorLogic = parameters.obj.ErrorLogic;
                isSucceeded = parameters.obj.IsSucceeded;
                continueWorkflowIfCallFailed = parameters.obj.ContinueWorkflowIfCallFailed;
                classMembersCode = parameters.obj.ClassMembersCode;
                enableVisualization = parameters.obj.EnableVisualization;
                headers = parameters.obj.Headers;
                urlParameters = parameters.obj.URLParameters;
                context = parameters.context;
                isNew = parameters.isNew;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.enableVisualization = true;

            $scope.modalContext.onModalHide = function () {
                if ($scope.remove != undefined && isNew == true) {
                    $scope.remove();
                }
            };


            $scope.scopeModel.onIsSucceededExpressionBuilderDirectiveReady = function (api) {
                isSucceededExpressionBuilderDirectiveAPI = api;
                isSucceededExpressionBuilderPromiseReadyDeffered.resolve();
            };

            $scope.scopeModel.onActionPathExpressionBuilderDirectiveReady = function (api) {
                actionPathExpressionBuilderDirectiveAPI = api;
                actionPathExpressionBuilderPromiseReadyDeffered.resolve();
            };

            $scope.scopeModel.onBodyLogicExpressionBuilderDirectiveReady = function (api) {
                bodyLogicExpressionBuilderDirectiveAPI = api;
                bodyLogicExpressionBuilderPromiseReadyDeffered.resolve();
            };

            $scope.scopeModel.onResponseLogicExpressionBuilderDirectiveReady = function (api) {
                responseLogicExpressionBuilderDirectiveAPI = api;
                responseLogicExpressionBuilderPromiseReadyDeffered.resolve();
            };

            $scope.scopeModel.onErrorLogicExpressionBuilderDirectiveReady = function (api) {
                errorLogicExpressionBuilderDirectiveAPI = api;
                errorLogicExpressionBuilderPromiseReadyDeffered.resolve();
            };


            $scope.scopeModel.onConnectionSelectorReady = function (api) {
                connectionSelectorAPI = api;
                connectionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onHeadersGridReady = function (api) {
                headersGridAPI = api;
                headersGridReadyDeferred.resolve();
            };

            $scope.scopeModel.onURLParametersGridReady = function (api) {
                urlParametersGridAPI = api;
                urlParametersGridReadyDeferred.resolve();
            };

            $scope.scopeModel.saveActivity = function () {
                return updateActivity();
            };

            $scope.scopeModel.close = function () {
                if ($scope.remove != undefined && isNew == true) {
                    $scope.remove();
                }
                $scope.modalContext.closeModal();
            };

        }


        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                $scope.title = "Edit Call Http Service";
            }

            function loadStaticData() {
                $scope.scopeModel.serviceName = serviceName;
                $scope.scopeModel.connectionId = connectionId;
                $scope.scopeModel.continueWorkflowIfCallFailed = continueWorkflowIfCallFailed;
                $scope.scopeModel.context = context;
                $scope.scopeModel.classMembersCode = classMembersCode;
                $scope.scopeModel.enableVisualization = enableVisualization;
            }

            function loadCallHttpServiceMethodSelector() {
                $scope.scopeModel.callHttpServiceMethodEnums = UtilsService.getArrayEnum(VRWorkflowCallHttpServiceMethodEnum);

                if (callHttpServiceMethod != undefined)
                    $scope.scopeModel.selectedCallHttpServiceMethod = UtilsService.getItemByVal($scope.scopeModel.callHttpServiceMethodEnums, callHttpServiceMethod, "value");
                else
                    $scope.scopeModel.selectedCallHttpServiceMethod = $scope.scopeModel.callHttpServiceMethodEnums[0];
            }

            function loadCallHttpServiceMessageFormatSelector() {
                $scope.scopeModel.callHttpServiceMessageFormatEnums = UtilsService.getArrayEnum(VRWorkflowCallHttpServiceMessageFormatEnum);

                if (callHttpServiceMessageFormat != undefined)
                    $scope.scopeModel.selectedCallHttpServiceMessageFormat = UtilsService.getItemByVal($scope.scopeModel.callHttpServiceMessageFormatEnums, callHttpServiceMessageFormat, "value");
                else
                    $scope.scopeModel.selectedCallHttpServiceMessageFormat = $scope.scopeModel.callHttpServiceMessageFormatEnums[0];
            }

            function loadCallHttpRetrySettingsSelector() {
                $scope.scopeModel.callHttpRetrySettingsEnums = UtilsService.getArrayEnum(VRWorkflowCallHttpRetrySettingsEnum);

                if (callHttpRetrySettings != undefined)
                    $scope.scopeModel.selectedCallHttpRetrySettings = UtilsService.getItemByVal($scope.scopeModel.callHttpRetrySettingsEnums, callHttpRetrySettings, "value");
                else
                    $scope.scopeModel.selectedCallHttpRetrySettings = $scope.scopeModel.callHttpRetrySettingsEnums[0];
            }

            function loadHeadersGrid() {
                var headersGridLoadDeferred = UtilsService.createPromiseDeferred();
                headersGridReadyDeferred.promise.then(function () {
                    var headersGridPayload = {
                        headers: headers,
                        getWorkflowArguments: context.getWorkflowArguments,
                        getParentVariables: context.getParentVariables
                    };
                    VRUIUtilsService.callDirectiveLoad(headersGridAPI, headersGridPayload, headersGridLoadDeferred);
                });
                return headersGridLoadDeferred.promise;
            }

            function loadURLParametersGrid() {
                var urlParametersGridLoadDeferred = UtilsService.createPromiseDeferred();
                urlParametersGridReadyDeferred.promise.then(function () {
                    var urlParametersGridPayload = {
                        urlParameters: urlParameters,
                        getWorkflowArguments: context.getWorkflowArguments,
                        getParentVariables: context.getParentVariables
                    };
                    VRUIUtilsService.callDirectiveLoad(urlParametersGridAPI, urlParametersGridPayload, urlParametersGridLoadDeferred);
                });
                return urlParametersGridLoadDeferred.promise;
            }

            function loadConnectionSelector() {
                var connectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                connectionSelectorReadyDeferred.promise.then(function () {
                    var selectorPayload = {
                        filter: {
                            Filters: [{
                                $type: "Vanrise.Common.Business.VRHttpConnectionFilter, Vanrise.Common.Business"
                            }]
                        },
                        selectedIds: connectionId
                    };
                    VRUIUtilsService.callDirectiveLoad(connectionSelectorAPI, selectorPayload, connectionSelectorLoadDeferred);
                });
                return connectionSelectorLoadDeferred.promise;
            }

            function loadIsSucceededExpressionBuilder() {
                var isSucceededExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                isSucceededExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: isSucceeded
                    };
                    VRUIUtilsService.callDirectiveLoad(isSucceededExpressionBuilderDirectiveAPI, payload, isSucceededExpressionBuilderPromiseLoadDeffered);
                });
                return isSucceededExpressionBuilderPromiseLoadDeffered.promise;
            }
            function loadActionPathExpressionBuilder() {
                var actionPathExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                actionPathExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: actionPath
                    };
                    VRUIUtilsService.callDirectiveLoad(actionPathExpressionBuilderDirectiveAPI, payload, actionPathExpressionBuilderPromiseLoadDeffered);
                });
                return actionPathExpressionBuilderPromiseLoadDeffered.promise;
            }
            function loadBodyLogicExpressionBuilder() {
                var bodyLogicExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                bodyLogicExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: buildBodyLogic
                    };
                    VRUIUtilsService.callDirectiveLoad(bodyLogicExpressionBuilderDirectiveAPI, payload, bodyLogicExpressionBuilderPromiseLoadDeffered);
                });
                return bodyLogicExpressionBuilderPromiseLoadDeffered.promise;
            }
            function loadResponseLogicExpressionBuilder() {
                var responseLogicExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                responseLogicExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: responseLogic
                    };
                    VRUIUtilsService.callDirectiveLoad(responseLogicExpressionBuilderDirectiveAPI, payload, responseLogicExpressionBuilderPromiseLoadDeffered);
                });
                return responseLogicExpressionBuilderPromiseLoadDeffered.promise;
            }
            function loadErrorLogicExpressionBuilder() {
                var errorLogicExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                errorLogicExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: errorLogic
                    };
                    VRUIUtilsService.callDirectiveLoad(errorLogicExpressionBuilderDirectiveAPI, payload, errorLogicExpressionBuilderPromiseLoadDeffered);
                });
                return errorLogicExpressionBuilderPromiseLoadDeffered.promise;
            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadHeadersGrid, loadURLParametersGrid, loadCallHttpServiceMethodSelector, loadCallHttpServiceMessageFormatSelector, loadCallHttpRetrySettingsSelector, loadConnectionSelector, loadIsSucceededExpressionBuilder, loadActionPathExpressionBuilder, loadBodyLogicExpressionBuilder, loadResponseLogicExpressionBuilder, loadErrorLogicExpressionBuilder]).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateActivity() {
            $scope.scopeModel.isLoading = true;
            var updatedObject = {
                connectionId: connectionSelectorAPI.getSelectedIds(),
                callHttpServiceMethod: $scope.scopeModel.selectedCallHttpServiceMethod.value,
                actionPath: actionPathExpressionBuilderDirectiveAPI != undefined ? actionPathExpressionBuilderDirectiveAPI.getData() : undefined,
                buildBodyLogic: bodyLogicExpressionBuilderDirectiveAPI != undefined ? bodyLogicExpressionBuilderDirectiveAPI.getData() : undefined,
                callHttpServiceMessageFormat: $scope.scopeModel.selectedCallHttpServiceMessageFormat.value,
                callHttpRetrySettings: $scope.scopeModel.selectedCallHttpRetrySettings.value,
                responseLogic: responseLogicExpressionBuilderDirectiveAPI != undefined ? responseLogicExpressionBuilderDirectiveAPI.getData() : undefined,
                errorLogic: errorLogicExpressionBuilderDirectiveAPI != undefined ? errorLogicExpressionBuilderDirectiveAPI.getData() : undefined,
                isSucceeded: isSucceededExpressionBuilderDirectiveAPI != undefined ? isSucceededExpressionBuilderDirectiveAPI.getData() : undefined,
                continueWorkflowIfCallFailed: $scope.scopeModel.continueWorkflowIfCallFailed,
                serviceName: $scope.scopeModel.serviceName,
                headers: (headersGridAPI != undefined) ? headersGridAPI.getData() : undefined,
                urlParameters: (urlParametersGridAPI != undefined) ? urlParametersGridAPI.getData() : undefined,
                classMembersCode: $scope.scopeModel.classMembersCode,
                enableVisualization: $scope.scopeModel.enableVisualization
            };
            if ($scope.onActivityUpdated != undefined) {
                $scope.onActivityUpdated(updatedObject);
            }
            $scope.scopeModel.isLoading = false;
            isNew = false;
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('BusinessProcess_VR_WorkflowActivityCallHttpServiceController', CallHttpServiceEditorController);
})(appControllers);