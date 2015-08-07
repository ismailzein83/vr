(function (appControllers) {

    "use strict";

    bpTrackingModalController.$inject = ['$scope', 'UtilsService', 'VRNavigationService', '$interval', 'BusinessProcessAPIService', 'DataRetrievalResultTypeEnum', 'VRNotificationService'];

    function bpTrackingModalController($scope, UtilsService, VRNavigationService, $interval, BusinessProcessAPIService, DataRetrievalResultTypeEnum, VRNotificationService) {

        var mainGridApi, nonClosedStatuses, interval, startTimer = true, lockGetData = false;
        var trackingResponse ;
        function getTrackingsFrom() {
            return BusinessProcessAPIService.GetTrackingsFrom($scope.BPInstanceID, $scope.lastTrackingId).then(function (response) {
                trackingResponse = response;
            });
        }

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

                UtilsService.waitMultipleAsyncOperations([getTrackingsFrom, getInstance]).then(function () {

                    $scope.lastTrackingId = UtilsService.getPropMaxValueFromArray(trackingResponse, "Id");

                    var isNonClosed = false;

                    

                    for (var i = 0, len = nonClosedStatuses.length; i < len; i++) {

                        if ($scope.process.InstanceStatus === nonClosedStatuses[i]) {
                            isNonClosed = true;
                            break;
                        }
                    }
                    if ( ! isNonClosed) stopGetData();
                    
                    for (var i = 0, len = trackingResponse.length; i < len; i++) {
                        trackingResponse[i].SeverityDescription = UtilsService.getLogEntryTypeDescription(trackingResponse[i].Severity);
                    }
                    mainGridApi.addItemsToBegin(trackingResponse);
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
                FromTrackingId: $scope.lastTrackingId,
                Message: $scope.message,
                Severities: UtilsService.getPropValuesFromArray($scope.selectedTrackingSeverity, "value")
            });
        }

        function defineScope() {
            $scope.lastTrackingId = 0;
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


                return BusinessProcessAPIService.GetFilteredTrackings(dataRetrievalInput).then(function (response) {

                    $scope.lastTrackingId = UtilsService.getPropMaxValueFromArray(response.Data, "Id");
                    if ($scope.lastTrackingId != undefined && startTimer) {
                        startTimer = false;
                        startGetData();
                    }

                    if (dataRetrievalInput.DataRetrievalResultType === DataRetrievalResultTypeEnum.Normal.value) {
                        
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
            defineGrid();
            loadFilters();
            loadNonClosedStatuses();
        }

        load();

    }

    appControllers.controller("BusinessProcess_BPTrackingModalController", bpTrackingModalController);

})(appControllers);