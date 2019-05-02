﻿(function (appControllers) {

    "use strict";

    bpTrackingModalController.$inject = ['$scope', 'VRNavigationService', 'BusinessProcess_BPInstanceAPIService', 'VRUIUtilsService', 'BusinessProcess_BPDefinitionAPIService', 'BusinessProcess_BPInstanceService', 'VRTimerService', 'UtilsService', 'BPInstanceStatusEnum', 'BusinessProcess_BPDefinitionService', 'VRNotificationService'];

    function bpTrackingModalController($scope, VRNavigationService, BusinessProcess_BPInstanceAPIService, VRUIUtilsService, BusinessProcess_BPDefinitionAPIService, BusinessProcess_BPInstanceService, VRTimerService, UtilsService, BPInstanceStatusEnum, BusinessProcess_BPDefinitionService, VRNotificationService) {

        var filter;
        var instanceTrackingFilter;

        var bpInstanceID;
        var bpInstance;
        var bpDefinitionID;
        var context;
        var bpInstanceStatusValue;
        var serviceInstanceId;

        var completionViewURL;
        var defaultCompletionViewLinkText = 'View Result';

        $scope.isNew = false;
        $scope.isPostponed = false;
        $scope.isRunning = false;
        $scope.isWaiting = false;
        $scope.isCancelling = false;
        $scope.isCompleted = false;
        $scope.isSuspended = false;
        $scope.isAborted = false;
        $scope.isTerminated = false;
        $scope.isCancelled = false;


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

            $scope.scopeModel.cancelActionClicked = false;
            $scope.scopeModel.allowCancel = false;
            $scope.scopeModel.showCloseMessage = false;
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

            $scope.scopeModel.showCancelAction = function () {
                var statusEnumValue = UtilsService.getEnum(BPInstanceStatusEnum, 'value', bpInstanceStatusValue);
                if (statusEnumValue != undefined && statusEnumValue.isOpened && bpInstanceStatusValue.value != BPInstanceStatusEnum.Cancelling.value)
                    return true;
                return false;
            };

            $scope.scopeModel.showCancelRequestLabel = function () {
                var statusEnumValue = UtilsService.getEnum(BPInstanceStatusEnum, 'value', bpInstanceStatusValue);
                if (statusEnumValue != undefined && statusEnumValue.isOpened && bpInstanceStatusValue.value != BPInstanceStatusEnum.Cancelling.value)
                    return true;
                return false;
            };

            $scope.scopeModel.cancelProcess = function () {
                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        $scope.scopeModel.isLoading = true;
                        $scope.scopeModel.cancelActionClicked = true;
                        return BusinessProcess_BPInstanceAPIService.CancelProcess({ BPInstanceId: bpInstanceID }).then(function (response) {
                        }).finally(function () {
                            $scope.scopeModel.isLoading = false;
                        });
                    }
                });
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

                if ($scope.jobIds) {
                    VRTimerService.unregisterJobByIds($scope.jobIds);
                    $scope.jobIds.length = 0;
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
                getBPInstanceDefinitionDetail().then(function () {
                    var promises = [];
                    EvaluateStatus();
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
        function setStatusVariables(isNew, isPostponed, isRunning, isWaiting, isCancelling, isCompleted, isAborted, isSuspended, isTerminated, isCancelled) {
            $scope.scopeModel.isNew = isNew;
            $scope.scopeModel.isPostponed = isPostponed;
            $scope.scopeModel.isRunning = isRunning;
            $scope.scopeModel.isWaiting = isWaiting;
            $scope.scopeModel.isCancelling = isCancelling;
            $scope.scopeModel.isCompleted = isCompleted;
            $scope.scopeModel.isAborted = isAborted;
            $scope.scopeModel.isSuspended = isSuspended;
            $scope.scopeModel.isTerminated = isTerminated;
            $scope.scopeModel.isCancelled = isCancelled;
        }
        function EvaluateStatus() {
            switch (bpInstanceStatusValue) {
                case BPInstanceStatusEnum.New.value:
                    setStatusVariables(true, false, false, false, false, false, false, false, false, false);
                    break;
                case BPInstanceStatusEnum.Postponed.value:
                    setStatusVariables(false, true, false, false, false, false, false, false, false, false);
                    break;
                case BPInstanceStatusEnum.Running.value:
                    setStatusVariables(false, false, true, false, false, false, false, false, false, false);
                    break;
                case BPInstanceStatusEnum.Waiting.value:
                    setStatusVariables(false, false, false, true, false, false, false, false, false, false);
                    break;
                case BPInstanceStatusEnum.Cancelling.value:
                    setStatusVariables(false, false, false, false, true, false, false, false, false, false);
                    break;
                case BPInstanceStatusEnum.Completed.value:
                    setStatusVariables(false, false, false, false, false, true, false, false, false, false);
                    break;
                case BPInstanceStatusEnum.Aborted.value:
                    setStatusVariables(false, false, false, false, false, false, true, false, false, false);
                    break;
                case BPInstanceStatusEnum.Suspended.value:
                    setStatusVariables(false, false, false, false, false, false, false, true, false, false);
                    break;
                case BPInstanceStatusEnum.Terminated.value:
                    setStatusVariables(false, false, false, false, false, false, false, false, true, false);
                    break;
                case BPInstanceStatusEnum.Cancelled.value:
                    setStatusVariables(false, false, false, false, false, false, false, false, false, true);
                    break;
            }
        }
        function getBPInstance() {
            return BusinessProcess_BPInstanceAPIService.GetBPInstance(bpInstanceID).then(function (response) {
                bpInstance = response;
                bpDefinitionID = response.DefinitionID;
                bpInstanceStatusValue = response.Status;
                serviceInstanceId = response.ServiceInstanceId;
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
        function getBPInstanceDefinitionDetail() {
            return BusinessProcess_BPInstanceAPIService.GetBPInstanceDefinitionDetail(bpDefinitionID, bpInstanceID).then(function (response) {
                var configuration = response.Entity.Configuration;
                $scope.scopeModel.allowCancel = response.AllowCancel;
                $scope.scopeModel.process.HasChildProcesses = configuration.HasChildProcesses;
                $scope.scopeModel.process.HasBusinessRules = configuration.HasBusinessRules;

                completionViewURL = configuration.CompletionViewURL;
                $scope.scopeModel.completionViewLinkText = (configuration.CompletionViewLinkText != null) ? configuration.CompletionViewLinkText : defaultCompletionViewLinkText;
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
            if ($scope.jobIds) {
                VRTimerService.unregisterJobByIds($scope.jobIds);
                $scope.jobIds.length = 0;
            }
            VRTimerService.registerJob(onTimerElapsed, $scope);
        }

        function onTimerElapsed() {

            return BusinessProcess_BPInstanceAPIService.GetBPInstance(bpInstanceID).then(function (response) {
                $scope.scopeModel.process.Status = response.StatusDescription;
                bpInstanceStatusValue = response.Status;

                if (bpInstanceStatusValue == BPInstanceStatusEnum.Cancelling.value)
                    $scope.scopeModel.allowCancel = false;
                if (bpInstanceStatusValue == BPInstanceStatusEnum.Completed.value)
                    $scope.scopeModel.showCloseMessage = true;
                if (context != undefined && context.automaticCloseWhenCompleted && response.Status == BPInstanceStatusEnum.Completed.value) {
                    $scope.modalContext.closeModal();
                }
                EvaluateStatus();
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

