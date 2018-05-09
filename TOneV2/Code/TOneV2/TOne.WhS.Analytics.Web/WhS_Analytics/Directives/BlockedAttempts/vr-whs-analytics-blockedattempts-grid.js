"use strict";

app.directive("vrWhsAnalyticsBlockedattemptsGrid", ["UtilsService", "VRNotificationService", "WhS_Analytics_BlockedAttemptsAPIService", "WhS_BE_SwitchReleaseCauseService","VR_Analytic_AnalyticItemActionService",
function (UtilsService, VRNotificationService, WhS_Analytics_BlockedAttemptsAPIService, WhS_BE_SwitchReleaseCauseService, VR_Analytic_AnalyticItemActionService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
            shownumber: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new Grid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Analytics/Directives/BlockedAttempts/Templates/BlockedAttemptsGridTemplate.html"

    };

    function Grid($scope, ctrl, $attrs) {

        var gridAPI;
        var queryObject;
        var fromDate;
        var toDate;
        var gridAPI;
        var switchIds;
        var groupByNumber;
        var payloadPeriod;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.blockedAttempts = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        queryObject = query;
                        fromDate = query.From;
                        toDate = query.To;
                        payloadPeriod = query.Period;
                        if (query.Filter != undefined) {
                            switchIds = query.Filter.SwitchIds;
                            groupByNumber = query.Filter.GroupByNumber;
                        }

                        return gridAPI.retrieveData(query);
                    };

                    return directiveAPI;
                }
            };
            $scope.onClickReleaseCode = function (dataItem) {
                if (queryObject != undefined && queryObject.Filter != undefined && queryObject.Filter.SwitchIds != undefined)
                    switchIds = queryObject.Filter.SwitchIds;
                WhS_BE_SwitchReleaseCauseService.openReleaseCodeDescriptions(dataItem.Entity.ReleaseCode, switchIds);
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                ctrl.showGrid = true;
                return WhS_Analytics_BlockedAttemptsAPIService.GetBlockedAttemptsData(dataRetrievalInput)
                                 .then(function (response) {
                                     $scope.shownumber = ctrl.shownumber;
                                     onResponseReady(response);
                                 })
                                 .catch(function (error) {
                                     VRNotificationService.notifyException(error, $scope);
                                 });
            };
            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "CDRs",
                clicked: openRecordSearch,
            }];
        }
        function openRecordSearch(dataItem) {
            var reportId = "c82daa2a-3fd8-432d-8811-7ba3e4cb3c58";
            var sourceName = "Invalid CDRs";
            var title = "CDRs";
            var period = payloadPeriod;
            var fieldFilters = [];

            if (groupByNumber==true) {
                fieldFilters.push({
                    FieldName: "CDPN",
                    FilterValues: [dataItem.Entity.CDPN]
                });
                fieldFilters.push({
                    FieldName: "CGPN",
                    FilterValues: [dataItem.Entity.CGPN]
                });
                
            } 
            if (switchIds != undefined) {
                fieldFilters.push({
                    FieldName: "SwitchId",
                    FilterValues: switchIds
                });
            }
            fieldFilters.push({
                FieldName: "CustomerId",
                FilterValues: dataItem.Entity.CustomerId != null ? [dataItem.Entity.CustomerId] : []
            });
            var salezoneId = dataItem.Entity.SaleZoneId == 0 ? undefined : dataItem.Entity.SaleZoneId;
            fieldFilters.push({
                FieldName: "SaleZoneId",
                FilterValues: [salezoneId]
            });
            fieldFilters.push({
                FieldName: "SupplierId",
                FilterValues: null
            });
            fieldFilters.push({
                FieldName: "DurationInSeconds",
                FilterValues: [0]
            });
            fieldFilters.push({
                FieldName: "ReleaseCode",
                FilterValues: [dataItem.Entity.ReleaseCode]
            });
            fieldFilters.push({
                FieldName: "ReleaseSource",
                FilterValues: [dataItem.Entity.ReleaseSource]
            });
            return VR_Analytic_AnalyticItemActionService.openRecordSearch(reportId, title, sourceName, fromDate, toDate, period, fieldFilters);
        }
    }
    return directiveDefinitionObject;

}]);