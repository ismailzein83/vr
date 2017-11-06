(function (appControllers) {

    "use strict";

    bpTrackingModalController.$inject = ['$scope', 'VRNavigationService', 'BusinessProcess_BPInstanceAPIService', 'VRUIUtilsService', 'BusinessProcess_BPDefinitionAPIService', 'BusinessProcess_BPInstanceService', 'VRTimerService', 'UtilsService', 'BPInstanceStatusEnum', 'BusinessProcess_BPDefinitionService'];

    function bpTrackingModalController($scope, VRNavigationService, BusinessProcess_BPInstanceAPIService, VRUIUtilsService, BusinessProcess_BPDefinitionAPIService, BusinessProcess_BPInstanceService, VRTimerService, UtilsService, BPInstanceStatusEnum, BusinessProcess_BPDefinitionService) {

        var filter;
        var instanceTrackingFilter;

        var bpInstanceID;
        var bpInstance;
        var bpDefinitionID;
        var context;
        var bpInstanceStatusValue;

        var completionViewURL;
        var defaultCompletionViewLinkText = 'View Result';

        var instanceTrackingMonitorGridAPI;
        var instanceTrackingMonitorGridReadyDeferred = UtilsService.createPromiseDeferred();

        var taskTrackingMonitorGridAPI;
        var taskTrackingMonitorGridReadyDeferred = UtilsService.createPromiseDeferred();

        var instanceMonitorGridAPI;
        var instanceMonitorGridReadyDeferred = UtilsService.createPromiseDeferred();

        var validationMessageMonitorGridAPI;
        var validationMessageMonitorGridReadyDeferrred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters !== undefined && parameters !== null) {
                bpInstanceID = parameters.BPInstanceID;
                context = parameters.context;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onInstanceTrackingMonitorGridReady = function (api) {
                instanceTrackingMonitorGridAPI = api;
                instanceTrackingMonitorGridReadyDeferred.resolve();
            };

            $scope.scopeModel.onTaskMonitorGridReady = function (api) {
                taskTrackingMonitorGridAPI = api;
                taskTrackingMonitorGridReadyDeferred.resolve();
            };

            $scope.scopeModel.onInstanceMonitorGridReady = function (api) {
                instanceMonitorGridAPI = api;
                instanceMonitorGridReadyDeferred.resolve();
            };

            $scope.scopeModel.onValidationMessageMonitorGridReady = function (api) {
                validationMessageMonitorGridAPI = api;
                validationMessageMonitorGridReadyDeferrred.resolve();
            };

            $scope.scopeModel.getStatusColor = function () {
                if (bpInstance) {
                    return 'control-label vr-control-label ' + BusinessProcess_BPInstanceService.getStatusColor(bpInstance.Status);
                }
                return "";
            };

            $scope.scopeModel.openCompletionView = function () {
                var onCompletionViewClosed = function () { };
                var hideSelectedColumn = false;
                BusinessProcess_BPDefinitionService.openCompletionView(completionViewURL, bpInstanceID, hideSelectedColumn, onCompletionViewClosed);
            };

            $scope.modalContext.onModalHide = function () {
                instanceTrackingMonitorGridAPI.clearTimer();
                taskTrackingMonitorGridAPI.clearTimer();

                if ($scope.scopeModel.process.HasChildProcesses)
                    instanceMonitorGridAPI.clearTimer();

                if ($scope.scopeModel.process.HasBusinessRules)
                    validationMessageMonitorGridAPI.clearTimer();

                if ($scope.job) {
                    VRTimerService.unregisterJob($scope.job);
                }

                if (context != undefined && context.onClose != undefined) {
                    var bpInstanceClosureContext = { bpInstanceStatusValue: bpInstanceStatusValue };
                    context.onClose(bpInstanceClosureContext);
                }
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            var loadPromiseDeferred = UtilsService.createPromiseDeferred();

            getBPInstance().then(function () {
                getBPDefinition().then(function () {
                    var promises = [];

                    var instanceTrackingMonitorGridLoadPromise = getInstanceTrackingMonitorGridLoadPromise();
                    promises.push(instanceTrackingMonitorGridLoadPromise);

                    var taskMonitorGridLoadPromise = getTaskMonitorGridLoadPromise();
                    promises.push(taskMonitorGridLoadPromise);

                    if ($scope.scopeModel.process.HasChildProcesses) {
                        var instanceMonitorGridLoadPromise = getInstanceMonitorGridLoadPromise();
                        promises.push(instanceMonitorGridLoadPromise);
                    }

                    if ($scope.scopeModel.process.HasBusinessRules) {
                        var validationMessageMonitorGridLoadPromise = getValidationMessageMonitorGridLoadPromise();
                        promises.push(validationMessageMonitorGridLoadPromise);
                    }

                    UtilsService.waitMultiplePromises(promises).then(function () {
                        loadPromiseDeferred.resolve();
                    }).catch(function (error) {
                        loadPromiseDeferred.reject(error);
                    });

                }).catch(function (error) {
                    loadPromiseDeferred.reject(error);
                });
            }).catch(function (error) {
                loadPromiseDeferred.reject(error);
            });

            createTimer();

            return loadPromiseDeferred.promise.finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function getBPInstance() {
            return BusinessProcess_BPInstanceAPIService.GetBPInstance(bpInstanceID).then(function (response) {
                bpInstance = response;
                bpDefinitionID = response.DefinitionID;

                $scope.scopeModel.process = {
                    InstanceStatus: bpInstance.InstanceStatus,
                    Title: bpInstance.Title,
                    CreatedTime: bpInstance.CreatedTime,
                    StatusUpdatedTime: bpInstance.StatusUpdatedTime,
                    Status: bpInstance.StatusDescription,
                    HasChildProcesses: false,
                    HasBusinessRules: false
                };
                $scope.title += $scope.scopeModel.process.Title;
            });
        }
        function getBPDefinition() {
            return BusinessProcess_BPDefinitionAPIService.GetBPDefintion(bpDefinitionID).then(function (response) {
                $scope.scopeModel.process.HasChildProcesses = response.Configuration.HasChildProcesses;
                $scope.scopeModel.process.HasBusinessRules = response.Configuration.HasBusinessRules;

                completionViewURL = response.Configuration.CompletionViewURL;
                $scope.scopeModel.completionViewLinkText = (response.Configuration.CompletionViewLinkText != null) ? response.Configuration.CompletionViewLinkText : defaultCompletionViewLinkText;
            });
        }

        function getInstanceTrackingMonitorGridLoadPromise() {
            var loadPromiseDeferred = UtilsService.createPromiseDeferred();

            instanceTrackingMonitorGridReadyDeferred.promise.then(function () {

                UtilsService.convertToPromiseIfUndefined(instanceTrackingMonitorGridAPI.loadGrid(getInstanceTrackingFilter())).then(function () {
                    loadPromiseDeferred.resolve();
                }).catch(function (error) {
                    loadPromiseDeferred.reject(error);
                });
            });

            return loadPromiseDeferred.promise;
        }
        function getTaskMonitorGridLoadPromise() {
            var loadPromiseDeferred = UtilsService.createPromiseDeferred();

            taskTrackingMonitorGridReadyDeferred.promise.then(function () {

                UtilsService.convertToPromiseIfUndefined(taskTrackingMonitorGridAPI.loadGrid(getFilterObject())).then(function () {
                    loadPromiseDeferred.resolve();
                })
                .catch(function (error) {
                    loadPromiseDeferred.reject(error);
                });
            });

            return loadPromiseDeferred.promise;
        }
        function getInstanceMonitorGridLoadPromise() {
            var loadPromiseDeferred = UtilsService.createPromiseDeferred();

            instanceMonitorGridReadyDeferred.promise.then(function () {

                UtilsService.convertToPromiseIfUndefined(instanceMonitorGridAPI.loadGrid(getFilterObject())).then(function () {
                    loadPromiseDeferred.resolve();
                }).catch(function (error) {
                    loadPromiseDeferred.reject(error);
                });
            });

            return loadPromiseDeferred.promise;
        }
        function getValidationMessageMonitorGridLoadPromise() {
            var loadPromiseDeferred = UtilsService.createPromiseDeferred();

            validationMessageMonitorGridReadyDeferrred.promise.then(function () {

                UtilsService.convertToPromiseIfUndefined(validationMessageMonitorGridAPI.loadGrid(getFilterObject())).then(function () {
                    loadPromiseDeferred.resolve();
                }).catch(function (error) {
                    loadPromiseDeferred.reject(error);
                });
            });

            return loadPromiseDeferred.promise;
        }

        function getFilterObject() {
            filter = {
                BPInstanceID: bpInstanceID,
            };
            return filter;
        }
        function getInstanceTrackingFilter() {
            var data = UtilsService.getLogEntryType();
            var severities = [];
            for (var x = 0; x < data.length; x++) {
                if (data[x].description != 'Verbose') {
                    severities.push(data[x]);
                }
            }

            instanceTrackingFilter = {
                BPInstanceID: bpInstanceID,
                Severities: severities
            };
            return instanceTrackingFilter;
        }

        function createTimer() {
            if ($scope.job) {
                VRTimerService.unregisterJob($scope.job);
            }
            VRTimerService.registerJob(onTimerElapsed, $scope);
        }
        function onTimerElapsed() {
            return BusinessProcess_BPInstanceAPIService.GetBPInstance(bpInstanceID).then(function (response) {
                $scope.scopeModel.process.Status = response.StatusDescription;
                bpInstanceStatusValue = response.Status;

                if (context != undefined && context.automaticCloseWhenCompleted && response.Status == BPInstanceStatusEnum.Completed.value) {
                    $scope.modalContext.closeModal();
                }

                if (response.Status == BPInstanceStatusEnum.Completed.value && completionViewURL != undefined)
                    $scope.scopeModel.showCompletionViewLink = true;

                $scope.scopeModel.process.StatusUpdatedTime = response.StatusUpdatedTime;
                bpInstance = response;
            }, function (exception) {
                $scope.scopeModel.isLoading = false;
            });
        }
    }

    appControllers.controller("BusinessProcess_BP_TrackingModalController", bpTrackingModalController);

})(appControllers);