"use strict";

app.directive("vrWhsAnalyticsReleasecodeGrid", ["UtilsService", "VRNotificationService", "WhS_Analytics_ReleaseCodeAPIService", "VR_Analytic_AnalyticItemActionService", "WhS_Analytics_GenericAnalyticReleaseCodeDimensionsEnum", "WhS_BE_SwitchReleaseCauseService",
function (UtilsService, VRNotificationService, WhS_Analytics_ReleaseCodeAPIService, VR_Analytic_AnalyticItemActionService, WhS_Analytics_GenericAnalyticReleaseCodeDimensionsEnum, WhS_BE_SwitchReleaseCauseService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
            dimenssion: "="
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
        templateUrl: "/Client/Modules/WhS_Analytics/Directives/ReleaseCodeStatistic/Templates/ReleaseCodeGridTemplate.html"

    };

    function Grid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;
       
        var fromDate;
        var toDate;
        var dimensionIds;
        var customerIds;
        var countryIds;
        var masterSaleZoneIds;
        var supplierIds;
        var switchIds;
        function initializeController() {
            ctrl.showDimessionCol = function (d) {

                return ctrl.dimenssion != undefined && ctrl.dimenssion.indexOf(d) > -1;
            };
            $scope.blockedAttempts = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        if (query != undefined)
                        {
                            fromDate = query.From;
                            toDate = query.To;
                            if (query.Filter != undefined)
                            {
                                dimensionIds = query.Filter.Dimession;
                                customerIds = query.Filter.CustomerIds;
                                countryIds = query.Filter.CountryIds;
                                masterSaleZoneIds = query.Filter.MasterSaleZoneIds;
                                supplierIds = query.Filter.SupplierIds;
                                switchIds = query.Filter.SwitchIds;
                            }
                        }
                        return gridAPI.retrieveData(query);
                    };

                    return directiveAPI;
                }
            };
            $scope.onClickReleaseCode = function (dataItem) {
                var switchIds = [dataItem.Entity.SwitchId];
                WhS_BE_SwitchReleaseCauseService.openReleaseCodeDescriptions(dataItem.Entity.ReleaseCode, switchIds);
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                 ctrl.showGrid = true;            
                return WhS_Analytics_ReleaseCodeAPIService.GetAllFilteredReleaseCodes(dataRetrievalInput)
                .then(function (response) {
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
        function openRecordSearch(dataItem)
        {
            var reportId = "c82daa2a-3fd8-432d-8811-7ba3e4cb3c58";
            var sourceName = "AllCDRs";
            var title= "CDRs";
            var period = -1;
          
            var fieldFilters = [];

            fieldFilters.push({
                FieldName: "ReleaseCode",
                FilterValues: [dataItem.Entity.ReleaseCode]
            }, {
                FieldName: "ReleaseSource",
                FilterValues: [dataItem.Entity.ReleaseSource]
            });

            var switchValues = switchIds;
            if (switchValues == undefined)
                switchValues = [];
            if (!UtilsService.contains(switchValues, dataItem.Entity.SwitchId))
                switchValues.push(dataItem.Entity.SwitchId);
            fieldFilters.push({
                FieldName: "SwitchId",
                FilterValues: switchValues
            });

            
            if (countryIds != undefined) {
                fieldFilters.push({
                    FieldName: "CountryId",
                    FilterValues: countryIds
                });
            }
            if (customerIds != undefined) {
                fieldFilters.push({
                    FieldName: "CustomerId",
                    FilterValues: customerIds
                });
            }
            if (masterSaleZoneIds != undefined) {
                fieldFilters.push({
                    FieldName: "MasterPlanZoneId",
                    FilterValues: masterSaleZoneIds
                });
            }
            if (supplierIds != undefined) {


                fieldFilters.push({
                    FieldName: "SupplierId",
                    FilterValues: supplierIds
                });
            }

            if (dimensionIds != undefined)
            {
                for(var i=0;i<dimensionIds.length;i++)
                {
                    var dimension = UtilsService.getEnum(WhS_Analytics_GenericAnalyticReleaseCodeDimensionsEnum, "value", dimensionIds[i]);
                    var entityValue = dataItem.Entity[dimension.entityValueName];
                    if (entityValue == "" || entityValue == " ")
                        entityValue = undefined;
                  
                    var fieldFilter = UtilsService.getItemByVal(fieldFilters, dimension.fieldName, "FieldName");
                    if (fieldFilter == undefined) {
                        fieldFilters.push({
                            FieldName: dimension.fieldName,
                            FilterValues: [entityValue]
                        });
                    }
                    
                }
            }
          
            return VR_Analytic_AnalyticItemActionService.openRecordSearch(reportId, title, sourceName, fromDate, toDate, period, fieldFilters);
        }
    }
    return directiveDefinitionObject;

}]);