'use strict';

app.directive('cpWhsRepeatednumbersGrid', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'CP_WhS_RepeatedNumbersAPIService', 'VR_Analytic_AnalyticItemActionService', 'CP_WhS_AccountViewTypeEnum', 'CP_WhS_PhoneNumberEnum','CP_WhS_BillingCDROptionMeasureEnum',
    function (UtilsService, VRUIUtilsService, VRNotificationService, CP_WhS_RepeatedNumbersAPIService, VR_Analytic_AnalyticItemActionService, CP_WhS_AccountViewTypeEnum, CP_WhS_PhoneNumberEnum, CP_WhS_BillingCDROptionMeasureEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                accountviewtype:'='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var grid = new RepeatedNumbersGrid($scope, ctrl, $attrs);
                grid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/CP_WhS/Elements/RepeatedNumbers/Directives/Templates/RepeatedNumbersGridTemplate.html'
        };

        function RepeatedNumbersGrid($scope, ctrl, $attrs) {
            var fromDate;
            var toDate;
            var gridAPI;
            var PhoneNumberType;
            var payloadPeriod;
            var sourceName;

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.repeatedNumbers = [];
                $scope.scopeModel.gridMenuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return CP_WhS_RepeatedNumbersAPIService.GetFilteredRepeatedNumbers(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };
                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    if (query != undefined) {
                        if (query.CDRType != undefined) {
                            var cdrOption = UtilsService.getEnum(CP_WhS_BillingCDROptionMeasureEnum, "value", query.CDRType);
                            if (cdrOption != undefined) {
                                sourceName = cdrOption.cdrSourceName;
                            }
                        }
                        PhoneNumberType = query.PhoneNumberType;
                        fromDate = query.From;
                        toDate = query.To;
                        payloadPeriod = query.Period;
                    }
                    $scope.showCustomer = query.Filter.CustomerIds != undefined;
                    return gridAPI.retrieveData(query);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "CDRs",
                    clicked: openRecordSearch
                }];
            }

            function openRecordSearch(dataItem) {
                var reportId = ctrl.accountviewtype == CP_WhS_AccountViewTypeEnum.Customer.value ? "4D263C2E-B1A2-45DE-BF48-D2C949DA1069" : "474F182C-8998-46F1-A75D-BA5672D0AFDB";
                if (sourceName == undefined)
                    sourceName = "AllCDRs";
                var title = "CDRs";
                var period = payloadPeriod;

                var fieldFilters = [];

                var phoneNumberFieldName;
                if (PhoneNumberType == CP_WhS_PhoneNumberEnum.CDPN.propertyName) {
                    phoneNumberFieldName = "CDPN";
                } else {
                    phoneNumberFieldName = "CGPN";
                }
                fieldFilters.push({
                    FieldName: phoneNumberFieldName,
                    FilterValues: [dataItem.Entity.PhoneNumber]
                });

                if (ctrl.accountviewtype == CP_WhS_AccountViewTypeEnum.Customer.value) {
                    fieldFilters.push({
                        FieldName: "CustomerId",
                        FilterValues: dataItem.Entity.CustomerId != null ? [dataItem.Entity.CustomerId] : null
                    });
                }
                else {
                    fieldFilters.push({
                        FieldName: "SupplierId",
                        FilterValues: dataItem.Entity.SupplierId != null ? [dataItem.Entity.SupplierId] : null
                    });
                }
                if (ctrl.accountviewtype == CP_WhS_AccountViewTypeEnum.Customer.value) {
                    var salezoneId = dataItem.Entity.SaleZoneId == 0 ? undefined : dataItem.Entity.SaleZoneId;
                    fieldFilters.push({
                        FieldName: "MasterPlanZoneId",
                        FilterValues: [salezoneId]
                    });
                }
                return VR_Analytic_AnalyticItemActionService.openRecordSearch(reportId, title, sourceName, fromDate, toDate, period, fieldFilters);
            }
        }
    }]);
