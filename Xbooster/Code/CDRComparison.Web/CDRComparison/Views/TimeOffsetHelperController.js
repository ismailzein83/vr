(function (appControllers) {

    "use strict";

    TimeOffsetHelperController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService','CDRComparison_CDRAPIService'];

    function TimeOffsetHelperController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, CDRComparison_CDRAPIService) {
        var tableKey;
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
                tableKey = parameters.tableKey;
            }
        }

        function defineScope() {

            $scope.scopeModal = {};

            $scope.systemCDRs = [];
            $scope.partnerCDRs = [];
            $scope.onSystemGridReady = function (api) {
                systemCDRGridAPI = api;
                if (!$scope.scopeModal.isLoading)
                {
                    var query = {
                        IsPartnerCDRs: false,
                        TableKey: tableKey
                    };
                    systemCDRGridAPI.retrieveData(query);
                }

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
                if ($scope.showPartnerGrid)
                {
                    var query = {
                        IsPartnerCDRs: true,
                        CDPN: selectedSystemCDR != undefined ? selectedSystemCDR.CDPN : undefined,
                        TableKey: tableKey
                    };
                    partnerCDRGridAPI.retrieveData(query);
                }

            };

            $scope.onClickedCDPN = function (dataItem) {
                selectedSystemCDR = dataItem;
                if (partnerCDRGridAPI != undefined) {
                    var query = {
                        IsPartnerCDRs: true,
                        CDPN: dataItem.CDPN,
                        TableKey: tableKey
                    };
                    partnerCDRGridAPI.retrieveData(query);
                }
                $scope.showPartnerGrid = true;
            };

            $scope.onSelectedRow = function (dataItem) {
                if (selectedPartnerCDR != undefined)
                    selectedPartnerCDR.IsSelected = false;
                selectedPartnerCDR = dataItem;
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModal.onPartnerCDRDirectiveReady = function (api) {
                partnerCDRGridAPI = api;
                var payload = {
                    IsPartnerCDRs: true
                };
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingPartnerCDRDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, partnerCDRGridAPI, payload, setLoader);
            };
           
            $scope.scopeModal.save = function () {
                return save();
            };

        }

        function load() {
            $scope.scopeModal.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadSystemGrid])
                          .catch(function (error) {
                              VRNotificationService.notifyException(error);
                          })
                          .finally(function () {
                              $scope.scopeModal.isLoading = false;
                          });

        }
        function loadSystemGrid()
        {
            if (systemCDRGridAPI !=undefined) {
                var query = {
                    IsPartnerCDRs: false,
                    TableKey: tableKey
                };
                systemCDRGridAPI.retrieveData(query);
            }
        }
        function setTitle()
        {
            $scope.title = "Time Offset Selection";
        }

        function save()
        {
            if(selectedSystemCDR != undefined && selectedPartnerCDR!=undefined)
            {
                var systemDate = new Date(selectedSystemCDR.Time);
                var partnerDate = new Date(selectedPartnerCDR.Time);

                var timeOffset = UtilsService.getTimeOffset(partnerDate, systemDate);
                $scope.onTimeOffsetSelected(timeOffset);
                $scope.modalContext.closeModal();
            }
        }
    }

    appControllers.controller('CDRComparison_TimeOffsetHelperController', TimeOffsetHelperController);
})(appControllers);
