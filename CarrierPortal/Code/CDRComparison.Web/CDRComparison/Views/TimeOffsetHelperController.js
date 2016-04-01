(function (appControllers) {

    "use strict";

    TimeOffsetHelperController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'CDRComparison_CDRComparisonAPIService','CDRComparison_CDRAPIService'];

    function TimeOffsetHelperController($scope, BusinessProcess_BPTaskAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, CDRComparison_CDRComparisonAPIService, CDRComparison_CDRAPIService) {
        var bpTaskId;
        var processInstanceId;

        var systemCDRGridAPI;
        var partnerCDRGridAPI;
        var selectedSystemCDR;
        var selectedPartnerCDR;
        loadParameters();

        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters !== undefined && parameters !== null) {
                bpTaskId = parameters.TaskId;
            }
        }

        function defineScope() {

            $scope.scopeModal = {};

            $scope.systemCDRs = [];
            $scope.partnerCDRs = [];
            $scope.onSystemGridReady = function (api) {
                systemCDRGridAPI = api;
                var query = {
                    IsPartnerCDRs: false
                };
                systemCDRGridAPI.retrieveData(query);
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return CDRComparison_CDRAPIService.GetFilteredCDRs(dataRetrievalInput).then(function (response) {
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };
            
            $scope.onPartnerGridReady = function (api) {
                partnerCDRGridAPI = api;
                if (!$scope.showPartnerGrid)
                {
                    var query = {
                        IsPartnerCDRs: true,
                        CDPN: selectedSystemCDR != undefined ? selectedSystemCDR.CDPN : undefined,
                    };
                    partnerCDRGridAPI.retrieveData(query);
                }

            };

            $scope.onClickedCDPN = function(dataItem)
            {
                selectedSystemCDR = dataItem;
                if (partnerCDRGridAPI != undefined)
                {
                    var query = {
                        IsPartnerCDRs: true,
                        CDPN: dataItem.CDPN,
                    };
                    partnerCDRGridAPI.retrieveData(query);
                }
                $scope.showPartnerGrid = true;
            }

            $scope.onSelectedRow = function (dataItem)
            {
                selectedPartnerCDR = dataItem;
            }

            $scope.close = function () {
                $scope.modalContext.closeModal()
            }

            $scope.scopeModal.onPartnerCDRDirectiveReady = function (api) {
                partnerCDRGridAPI = api;
                var payload = {
                    IsPartnerCDRs: true
                };
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingPartnerCDRDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, partnerCDRGridAPI, payload, setLoader);
            }
           
            $scope.scopeModal.save = function () {
                return save();
            }

        }

        function load() {
            $scope.scopeModal.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([setTitle])
                          .catch(function (error) {
                              VRNotificationService.notifyException(error);
                          })
                          .finally(function () {
                              $scope.scopeModal.isLoading = false;
                          });

        }

        function setTitle()
        {
            $scope.title = "Time Offset Helper";
        }

        function save()
        {
            if(selectedSystemCDR != undefined && selectedPartnerCDR!=undefined)
            {
                var systemDate = new Date(selectedSystemCDR.Time);
                var partnerDate = new Date(selectedPartnerCDR.Time);
                var timeOffset = getTimeOffset(systemDate, partnerDate);
                $scope.onTimeOffsetSelected(timeOffset);
                $scope.modalContext.closeModal();
            }
        }

        function getTimeOffset(firstDate,SecondDate)
        {
            var hours = firstDate.getHours() - SecondDate.getHours();
            if (hours == 0 || hours < 10)
                hours = '0' + hours;
            var seconds = firstDate.getSeconds() - SecondDate.getSeconds();
            if (seconds == 0 || seconds < 10)
                seconds = '0' + seconds;
            var milliseconds = firstDate.getMilliseconds() - SecondDate.getMilliseconds();
            if (milliseconds == 0 || milliseconds < 10)
                milliseconds = '0' + milliseconds;
            return hours + ":" + seconds + ":" + milliseconds;
        }
    }

    appControllers.controller('CDRComparison_TimeOffsetHelperController', TimeOffsetHelperController);
})(appControllers);
