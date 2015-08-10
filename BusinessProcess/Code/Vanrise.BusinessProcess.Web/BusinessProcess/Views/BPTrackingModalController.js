(function (appControllers) {

    "use strict";

    bpTrackingModalController.$inject = ['$scope', 'UtilsService', 'VRNavigationService', '$interval', 'BusinessProcessAPIService', 'DataRetrievalResultTypeEnum', 'VRNotificationService'];

    function bpTrackingModalController($scope, UtilsService, VRNavigationService, $interval, BusinessProcessAPIService, DataRetrievalResultTypeEnum, VRNotificationService) {

        var mainGridApi, nonClosedStatuses, interval, startTimer = true, lockGetData = false ,minTrackingId = 0,lastTrackingId = 0;
        
        function getInstance() {

            return BusinessProcessAPIService.GetBPInstance($scope.BPInstanceID).then(function (response) {

                $scope.process = {
                    InstanceStatus : response.InstanceStatus,
                    Title: response.Title,
                    Date: response.CreatedTime,
                    Status: response.StatusDescription
                };
            });
        }

        function stopGetData() {
            if (angular.isDefined(interval)) {
                $interval.cancel(interval);
                interval = undefined;
            }
        }

        function startGetData() {
            if (angular.isDefined(interval)) return;
            interval = $interval(function callAtInterval() {
                getTracking();
            }, 1000);
        }

        function getTracking() {

            if (!lockGetData) {

                lockGetData = true;

                BusinessProcessAPIService.GetTrackingsFrom($scope.BPInstanceID, lastTrackingId).then(function (trackingResponse) {
                    var isNonClosed = false;

                    for (var i = 0, len = nonClosedStatuses.length; i < len; i++) {

                        if (trackingResponse.InstanceStatus === nonClosedStatuses[i]) {
                            isNonClosed = true;
                            break;
                        }
                    }
                    if (!isNonClosed) {
                        stopGetData();
                        if (lastTrackingId === 0) return;
                    }

                    
                    if (lastTrackingId !== 0) {
                        
                        for (var i = 0, len = trackingResponse.Tracking.length; i < len; i++) {
                            trackingResponse.Tracking[i].SeverityDescription = UtilsService.getLogEntryTypeDescription(trackingResponse.Tracking[i].Severity);
                        }
                        mainGridApi.addItemsToBegin(trackingResponse.Tracking);

                    }
                    
                    if (trackingResponse.Tracking !== undefined && trackingResponse.Tracking.length > 0)
                        lastTrackingId = UtilsService.getPropMaxValueFromArray(trackingResponse.Tracking, "Id");

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    lockGetData = false;
                });
            }
        }

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            $scope.BPInstanceID = undefined;
            if (parameters !== undefined && parameters !== null) {
                $scope.BPInstanceID = parameters.BPInstanceID;
            }
        }

        function loadFilters() {
            $scope.trackingSeverity = UtilsService.getLogEntryType();
        }

        function loadNonClosedStatuses() {
            nonClosedStatuses = [];
            BusinessProcessAPIService.GetNonClosedStatuses().then(function (response) {
                for (var i = 0 ; i < response.length ; i++) {
                    nonClosedStatuses.push(response[i]);
                }
            });
        }

        function retrieveData() {

            return mainGridApi.retrieveData({
                ProcessInstanceId: $scope.BPInstanceID,
                FromTrackingId: 0,
                Message: $scope.message,
                Severities: UtilsService.getPropValuesFromArray($scope.selectedTrackingSeverity, "value")
            });
        }

        function defineScope() {
            $scope.message = '';
            $scope.trackingSeverity = [];
            $scope.selectedTrackingSeverity = [];
            $scope.close = function () {
                stopGetData();
                $scope.modalContext.closeModal();
            };

            $scope.$on('$destroy', function () {
                stopGetData();
            });

            $scope.getSeverityColor = function (dataItem, colDef) {
                return UtilsService.getLogEntryTypeColor(dataItem.Severity);
            };

            $scope.searchClicked = function () {
                stopGetData();
                return retrieveData();
            };

        }

        function defineGrid() {

            $scope.datasource = [];

            $scope.onGridReady = function (api) {
                mainGridApi = api;
                return retrieveData();
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                dataRetrievalInput.Query.FromTrackingId = minTrackingId;

                return BusinessProcessAPIService.GetFilteredTrackings(dataRetrievalInput).then(function (response) {

                    
                    if (dataRetrievalInput.DataRetrievalResultType === DataRetrievalResultTypeEnum.Normal.value) {
                        
                        minTrackingId =  UtilsService.getPropMinValueFromArray(response.Data, "Id");
                        if (minTrackingId != undefined && startTimer) {
                            startTimer = false;
                            startGetData();
                        }
                        
                        for (var i = 0, len = response.Data.length; i < len; i++) {
                            response.Data[i].SeverityDescription = UtilsService.getLogEntryTypeDescription(response.Data[i].Severity);
                        }
                    }
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };

        }

        function load() {
            defineScope();
            loadParameters();
            getInstance();
            defineGrid();
            loadFilters();
            loadNonClosedStatuses();
        }

        load();

    }

    appControllers.controller("BusinessProcess_BPTrackingModalController", bpTrackingModalController);

})(appControllers);