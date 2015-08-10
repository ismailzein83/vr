BlockedAttemptsController.$inject = ['$scope', 'UtilsService', 'AnalyticsAPIService', 'uiGridConstants', '$q', 'BusinessEntityAPIService_temp', 'BlockedAttemptsAPIService',
        'VRNotificationService', 'DataRetrievalResultTypeEnum', 'PeriodEnum','UtilsService','BlockedAttemptsMeasureEnum','CarrierAccountAPIService','CarrierTypeEnum','ZonesService'];

function BlockedAttemptsController($scope, UtilsService, AnalyticsAPIService, uiGridConstants, $q, BusinessEntityAPIService, BlockedAttemptsAPIService,
        VRNotificationService, DataRetrievalResultTypeEnum, PeriodEnum, UtilsService, BlockedAttemptsMeasureEnum, CarrierAccountAPIService, CarrierTypeEnum, ZonesService) {

    var mainGridAPI;
    var measures = [];
    var selectedPeriod;
    defineScope();
    load();

    function defineScope() {
        definePeriods();
        $scope.onValueChanged = function () {
            console.log($scope.selectedPeriod);
            if ($scope.selectedPeriod != selectedPeriod) {
                var customize = {
                    value: -1,
                    description: "Customize"
                }
                selectedPeriod = $scope.selectedPeriod;
                $scope.selectedPeriod = customize;
            }

        }
        $scope.periodSelectionChanged = function () {
            if ($scope.selectedPeriod != undefined && $scope.selectedPeriod.value != -1) {
                var date = UtilsService.getPeriod($scope.selectedPeriod.value);
                $scope.fromDate = date.from;
                $scope.toDate = date.to;
            }

        }
        $scope.data = [];
        $scope.switches = [];
        $scope.zones = [];
        $scope.selectedZones = [];
        $scope.selectedSwitches = [];
        $scope.customers = [];
        $scope.selectedCustomers = [];
        $scope.getColor = function (dataItem, coldef) {
            if (coldef.tag.value == TrafficStatisticsMeasureEnum.ACD.value)
                return getACDColor(dataItem.ACD, dataItem.Attempts);
        }
        $scope.measures = measures;
        defineMenuActions();
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return BlockedAttemptsAPIService.GetBlockedAttempts(dataRetrievalInput).then(function (response) {
                onResponseReady(response);
            })
        };

       
        $scope.searchClicked = function () {
            return retrieveData();
        };
        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }
    }

    function retrieveData() {


        var query = {
            Filter: filter,
            From: $scope.fromDate,
            To: $scope.toDate,
        };
        return mainGridAPI.retrieveData(query);
    }
    function load() {
        loadMeasures();
        $scope.isInitializing = true;
        UtilsService.waitMultipleAsyncOperations([loadSwitches, loadCustomers]).finally(function () {
            $scope.isInitializing = false;
            if (mainGridAPI != undefined) {
                retrieveData();
            }
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
    }

    function getACDColor(acdValue, attemptsValue) {
        if (attemptsValue > $scope.attampts && acdValue < $scope.acd)
            return LabelColorsEnum.Warning.Color;
        //if (status === BPInstanceStatusEnum.Running.value) return LabelColorsEnum.Info.Color;
        //if (status === BPInstanceStatusEnum.ProcessFailed.value) return LabelColorsEnum.Error.Color;
        //if (status === BPInstanceStatusEnum.Completed.value) return LabelColorsEnum.Success.Color;
        //if (status === BPInstanceStatusEnum.Aborted.value) return LabelColorsEnum.Warning.Color;
        //if (status === BPInstanceStatusEnum.Suspended.value) return LabelColorsEnum.Warning.Color;
        //if (status === BPInstanceStatusEnum.Terminated.value) return LabelColorsEnum.Error.Color;

        // return LabelColorsEnum.Info.Color;
    };

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "CDRs",
            clicked: function (dataItem) {
                var modalSettings = {
                    useModalTemplate: true,
                    width: "80%"//,
                    //maxHeight: "800px"
                };
                var parameters = {
                    fromDate: $scope.filter.fromDate,
                    toDate: $scope.filter.toDate

                    ///[dataItem.GroupKeyValues[0].Id]
                };
                loadCDRParameters(parameters, dataItem);



                VRModalService.showModal('/Client/Modules/Analytics/Views/CDR/CDRLog.html', parameters, modalSettings);
            }
        },
        {
            name: "Show Confirmation",
            clicked: function (dataItem) {
                VRNotificationService.showConfirmation('Are you sure you want to delete?')
                .then(function (result) {
                    if (result)
                        console.log('Confirmed');
                    else
                        console.log('not confirmed');
                });
            }
        },
        {
            name: "Show Error",
            clicked: function (dataItem) {
                VRNotificationService.showError('Error Message');
            }
        }];
    }

    function loadMeasures() {
        for (var prop in BlockedAttemptsMeasureEnum) {
            measures.push(BlockedAttemptsMeasureEnum[prop]);
        }
    }

    function buildFilter() {
        var filter = {};
        filter.SwitchIds = getFilterIds($scope.selectedSwitches, "SwitchId");
        filter.CustomerIds = getFilterIds($scope.selectedCustomers, "CarrierAccountID");
        filter.Zones = getFilterIds($scope.selectedZones, "Zone");
        return filter;
    }

    function getFilterIds(values, idProp) {
        var filterIds = [];
        if (values.length > 0) {
            angular.forEach(values, function (val) {
                filterIds.push(val[idProp]);
            });
        }
        return filterIds;
    }

    function loadSwitches() {
        return BusinessEntityAPIService.GetSwitches().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.switches.push(itm);
            });
        });
    }


    function loadCustomers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Customer.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
            });
        });
    }



    function definePeriods() {
        $scope.periods = [];
        for (var p in PeriodEnum)
            $scope.periods.push(PeriodEnum[p]);
        $scope.selectedPeriod = $scope.periods[0];
    }

};

appControllers.controller('Analytics_BlockedAttemptsController', BlockedAttemptsController);