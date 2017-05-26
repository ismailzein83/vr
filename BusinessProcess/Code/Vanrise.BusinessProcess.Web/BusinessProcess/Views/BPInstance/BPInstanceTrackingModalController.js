(function (appControllers) {

    "use strict";

    bpTrackingModalController.$inject = ['$scope', 'VRNavigationService', 'BusinessProcess_BPInstanceAPIService', 'VRUIUtilsService', 'BusinessProcess_BPDefinitionAPIService', 'BusinessProcess_BPInstanceService', 'VRTimerService', 'UtilsService', 'BPInstanceStatusEnum', 'BusinessProcess_BPDefinitionService'];

    function bpTrackingModalController($scope, VRNavigationService, BusinessProcess_BPInstanceAPIService, VRUIUtilsService, BusinessProcess_BPDefinitionAPIService, BusinessProcess_BPInstanceService, VRTimerService, UtilsService, BPInstanceStatusEnum, BusinessProcess_BPDefinitionService) {

        var bpInstanceID;
        var bpDefinitionID;

        var filter;
        var instanceTrackingFilter;

        var instanceMonitorGridAPI;
        var instanceTrackingHistoryGridAPI;
        var instanceTrackingMonitorGridAPI;
        var taskTrackingMonitorGridAPI;
        var validationMessageMonitorGridAPI;
        var validationMessageHistoryGridAPI;

        var bpInstance;
        var context;

        var completionViewURL;
        var defaultCompletionViewLinkText = 'View Result';

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

            $scope.onInstanceMonitorGridReady = function (api) {
                if ($scope.process.HasChildProcesses) {
                    instanceMonitorGridAPI = api;
                    getFilterObject();
                    instanceMonitorGridAPI.loadGrid(filter);
                }
            };

            $scope.onInstanceTrackingHistoryGridReady = function (api) {
                instanceTrackingHistoryGridAPI = api;
                getFilterObject();
                instanceTrackingHistoryGridAPI.loadGrid(filter);
            };

            $scope.onInstanceTrackingMonitorGridReady = function (api) {
                instanceTrackingMonitorGridAPI = api;
                getInstanceTrackingFilter();
                instanceTrackingMonitorGridAPI.loadGrid(instanceTrackingFilter);
            };

            $scope.onTaskMonitorGridReady = function (api) {
                taskTrackingMonitorGridAPI = api;
                getFilterObject();
                taskTrackingMonitorGridAPI.loadGrid(filter);
            };

            $scope.onValidationMessageMonitorGridReady = function (api) {
                validationMessageMonitorGridAPI = api;
                getFilterObject();
                validationMessageMonitorGridAPI.loadGrid(filter);
            };

            $scope.onValidationMessageHistoryGridReady = function (api) {
                validationMessageHistoryGridAPI = api;
                getFilterObject();
                validationMessageHistoryGridAPI.load(filter);
            };

            $scope.modalContext.onModalHide = function () {
                instanceTrackingMonitorGridAPI.clearTimer();

                if ($scope.process.HasChildProcesses) {
                    instanceMonitorGridAPI.clearTimer();
                }

                taskTrackingMonitorGridAPI.clearTimer();

                if ($scope.process.HasBusinessRules) {
                    validationMessageMonitorGridAPI.clearTimer();
                }

                if ($scope.job) {
                    VRTimerService.unregisterJob($scope.job);
                }

                if (context != undefined) {
                    if (context.onClose != undefined)
                        context.onClose();
                }
            };
            $scope.getStatusColor = function () {
                if (bpInstance) {
                    return 'control-label vr-control-label ' + BusinessProcess_BPInstanceService.getStatusColor(bpInstance.Status);
                }
                return "";
            };

            $scope.openCompletionView = function () {
                var onCompletionViewClosed = function () { };
                BusinessProcess_BPDefinitionService.openCompletionView(completionViewURL, bpInstanceID, onCompletionViewClosed);
            };
        }
        function load() {
            var promises = [];
            $scope.isLoading = true;

            var getBPInstancePromise = getBPInstance();
            promises.push(getBPInstancePromise);

            var getBPDefinitionDeferred = UtilsService.createPromiseDeferred();
            promises.push(getBPDefinitionDeferred.promise);

            getBPInstancePromise.then(function () {
                getBPDefinition().then(function () {
                    getBPDefinitionDeferred.resolve();
                }).catch(function (error) {
                    getBPDefinitionDeferred.reject(error);
                });
            });

            createTimer();

            return UtilsService.waitMultiplePromises(promises).finally(function () {
                $scope.isLoading = false;
            });
        }

        function getBPInstance() {
            return BusinessProcess_BPInstanceAPIService.GetBPInstance(bpInstanceID).then(function (response) {
                bpDefinitionID = response.DefinitionID;
                bpInstance = response;
                $scope.process = {
                    InstanceStatus: response.InstanceStatus,
                    Title: response.Title,
                    CreatedTime: response.CreatedTime,
                    StatusUpdatedTime: response.StatusUpdatedTime,
                    Status: response.StatusDescription,
                    HasChildProcesses: false,
                    HasBusinessRules: false
                };
                $scope.title += $scope.process.Title;
            });
        }
        function getBPDefinition() {
            return BusinessProcess_BPDefinitionAPIService.GetBPDefintion(bpDefinitionID).then(function (response) {
                $scope.process.HasChildProcesses = response.Configuration.HasChildProcesses;
                $scope.process.HasBusinessRules = response.Configuration.HasBusinessRules;

                completionViewURL = response.Configuration.CompletionViewURL;
                $scope.completionViewLinkText = (response.Configuration.CompletionViewLinkText != null) ? response.Configuration.CompletionViewLinkText : defaultCompletionViewLinkText;
            });
        }

        function createTimer() {
            if ($scope.job) {
                VRTimerService.unregisterJob($scope.job);
            }
            VRTimerService.registerJob(onTimerElapsed, $scope);
        }
        function onTimerElapsed() {
            return BusinessProcess_BPInstanceAPIService.GetBPInstance(bpInstanceID).then(function (response) {
                $scope.process.Status = response.StatusDescription;

                if (response.Status == BPInstanceStatusEnum.Completed.value && completionViewURL != undefined)
                    $scope.showCompletionViewLink = true;

                $scope.process.StatusUpdatedTime = response.StatusUpdatedTime;
                bpInstance = response;
            },
             function (exception) {
                 $scope.isLoading = false;
             });
        }

        function getFilterObject() {
            filter = {
                BPInstanceID: bpInstanceID,
            };
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
        }
    }

    appControllers.controller("BusinessProcess_BP_TrackingModalController", bpTrackingModalController);

})(appControllers);