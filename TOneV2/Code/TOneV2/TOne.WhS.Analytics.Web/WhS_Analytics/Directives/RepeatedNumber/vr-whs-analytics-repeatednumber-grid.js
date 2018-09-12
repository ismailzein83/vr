"use strict";

app.directive("vrWhsAnalyticsRepeatednumberGrid", ["UtilsService", "VRNotificationService", "WhS_Analytics_RepeatedNumberAPIService", "WhS_Analytics_GenericAnalyticService", "VR_Analytic_AnalyticItemActionService", "WhS_Analytics_PhoneNumberEnum",'WhS_Analytics_BillingCDROptionMeasureEnum',
function (UtilsService, VRNotificationService, WhS_Analytics_RepeatedNumberAPIService, WhS_Analytics_GenericAnalyticService, VR_Analytic_AnalyticItemActionService, WhS_Analytics_PhoneNumberEnum, WhS_Analytics_BillingCDROptionMeasureEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new RepeatedNumberGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Analytics/Directives/RepeatedNumber/Templates/RepeatedNumberGridTemplate.html"

    };

    function RepeatedNumberGrid($scope, ctrl, $attrs) {
        var fromDate;
        var toDate;
        var gridAPI;
        var switchIds;
        var PhoneNumberType;
        var payloadPeriod;
        var sourceName;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.repeatedNumber = [];


            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        if (query != undefined) {

                            if (query.CDRType != undefined)
                            {
                                var cdrOption = UtilsService.getEnum(WhS_Analytics_BillingCDROptionMeasureEnum,"value",query.CDRType);
                                if(cdrOption != undefined)
                                {
                                    sourceName = cdrOption.cdrSourceName;
                                }
                            }
                            PhoneNumberType = query.PhoneNumberType;
                            fromDate = query.From;
                            toDate = query.To;
                            payloadPeriod = query.Period;
                            if (query.Filter != undefined) {
                                switchIds = query.Filter.SwitchIds;
                            }
                        }

                        ctrl.parameters = query;
                        return gridAPI.retrieveData(query);
                    };

                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                ctrl.showGrid = true;
                return WhS_Analytics_RepeatedNumberAPIService.GetAllFilteredRepeatedNumbers(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .finally(function () {
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

            //$scope.gridMenuActions = [{
            //    name: "CDRs",
            //    clicked: function () {
            //        var parameters = {
            //            fromDate: UtilsService.cloneDateTime(ctrl.parameters.From),
            //            toDate: UtilsService.cloneDateTime(ctrl.parameters.To),
            //            customerIds: [],
            //            saleZoneIds: [],
            //            supplierIds: [],
            //            switchIds: ctrl.parameters.Filter.SwitchIds,
            //            supplierZoneIds: []
            //        };
            //        WhS_Analytics_GenericAnalyticService.showCdrLog(parameters);

            //    }
            //}];

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
            if (sourceName == undefined)
                sourceName = "AllCDRs";
            var title = "CDRs";
            var period = payloadPeriod;

            var fieldFilters = [];

            var phoneNumberFieldName;
            if (PhoneNumberType == WhS_Analytics_PhoneNumberEnum.CDPN.propertyName) {
                phoneNumberFieldName = "CDPN";
            } else {
                phoneNumberFieldName = "CGPN";
            }
            fieldFilters.push({
                FieldName: phoneNumberFieldName,
                FilterValues: [dataItem.Entity.PhoneNumber]
            });


            if (switchIds != undefined) {
                fieldFilters.push({
                    FieldName: "SwitchId",
                    FilterValues: switchIds
                });
            }

            fieldFilters.push({
                FieldName: "CustomerId",
                FilterValues: dataItem.Entity.CustomerId != null ? [dataItem.Entity.CustomerId] : null
            }, {
                FieldName: "SupplierId",
                FilterValues: dataItem.Entity.SupplierId != null ? [dataItem.Entity.SupplierId] : null
            });
            var salezoneId = dataItem.Entity.SaleZoneId == 0 ? undefined : dataItem.Entity.SaleZoneId;
            fieldFilters.push({
                FieldName: "SaleZoneId",
                FilterValues: [salezoneId]
            });

            return VR_Analytic_AnalyticItemActionService.openRecordSearch(reportId, title, sourceName, fromDate, toDate, period, fieldFilters);
        }
    }

    return directiveDefinitionObject;

}]);