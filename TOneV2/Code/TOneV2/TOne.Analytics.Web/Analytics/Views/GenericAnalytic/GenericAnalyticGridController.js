(function (appControllers) {

    "use strict";

    genericAnalyticGridController.$inject = ['$scope', 'GenericAnalyticAPIService', 'GenericAnalyticDimensionEnum', 'VRModalService', 'GenericAnalyticService'];
    function genericAnalyticGridController($scope, GenericAnalyticAPIService, GenericAnalyticDimensionEnum, vrModalService, GenericAnalyticService) {
        var gridApi;
        var measureFieldsValues = [];
        var parameters = {
            asr: 50,
            acd: 20,
            attempts: 2
        };
        var isSummary = false;
        defineScope();
        function defineScope() {
            
            $scope.datasource = [];
            $scope.params = parameters;
            defineMenuActions();
            $scope.sortField = $scope.measures[0].name;

            if ($scope.periods.length > 0 || $scope.groupKeys.length > 0)
                $scope.sortField = 'DimensionValues[0].Name';
            else
                $scope.sortField = 'MeasureValues.' + $scope.measures[0].name;

            $scope.gridReady = function (api) {
                gridApi = api;
            };

            $scope.currentSearchCriteria = {
                groupKeys:[]
            }

            var fixedDimensions = [];
            var selectedGroupKeys = [];

            $scope.subViewConnector.getValue = function () {
                return "GetValue";
            };
            
            $scope.subViewConnector.retrieveData = function (value) {
                isSummary = value.WithSummary;
                $scope.subViewConnector.value = value;
                measureFieldsValues.length = 0;
                if (gridApi == undefined)
                    return;

                var groupKeys = [];
                fixedDimensions.length = 0;

                value.DimensionFields.forEach(function (group) {
                    groupKeys.push(group.value);
                });
                
                if (value.FixedDimensionFields == undefined)
                    value.FixedDimensionFields = [];
                else {
                    groupKeys.push(value.FixedDimensionFields.value);
                    fixedDimensions.push(value.FixedDimensionFields);

                    if (value.FixedDimensionFields == GenericAnalyticDimensionEnum.Hour) {
                        groupKeys.push(GenericAnalyticDimensionEnum.Date.value);
                        fixedDimensions.push(GenericAnalyticDimensionEnum.Date);
                    }
                 }
                
                selectedGroupKeys = value.DimensionFields;
                
                for (var i = 0, len = value.MeasureFields.length; i < len; i++) {
                    measureFieldsValues.push(value.MeasureFields[i].value);
                }

                var query = {
                    Filters: value.Filters,
                    DimensionFields: groupKeys,
                    MeasureFields: measureFieldsValues,
                    FromTime: value.FromTime,
                    ToTime: value.ToTime,
                    Currency: value.Currency,
                    WithSummary: value.WithSummary
                }
                
                $scope.selectedMeasures = value.MeasureFields;
                $scope.fromDate = value.FromTime;
                $scope.toDate = value.ToTime;
                $scope.Currency = value.Currency;
                $scope.selectedfilters = value.Filters;
                return gridApi.retrieveData(query);
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return GenericAnalyticAPIService.GetFiltered(dataRetrievalInput)
                .then(function (response) {
                    $scope.currentSearchCriteria.groupKeys.length = 0;
                    
                    selectedGroupKeys.forEach(function (group) {
                        $scope.currentSearchCriteria.groupKeys.push(group);
                    });

                    fixedDimensions.forEach(function (group) {
                        $scope.currentSearchCriteria.groupKeys.push(group);
                    });

                    if (isSummary)
                        gridApi.setSummary(response.Summary);

                    onResponseReady(response);
                });
            };

            $scope.checkExpandablerow = function (groupKeys) {
                return groupKeys.length !== $scope.groupKeys.length;
            };

            $scope.getColor = function (dataItem, coldef) {

                return GenericAnalyticService.getMeasureColor(dataItem, coldef, parameters);
            };
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Settings",
                clicked: function () {
                    showGenericAnalyticGridSettings();
                }
            }];
        }

        function showGenericAnalyticGridSettings() {

            vrModalService.showModal('/Client/Modules/Analytics/Views/GenericAnalytic/GenericAnalyticGridSettings.html', parameters, {
                useModalTemplate: true,
                width: "40%",
                maxHeight: "190px",
                title: "Settings",
                onScopeReady: function (modalScope) {
                    modalScope.onSaveSettings = function (settings) {
                        parameters = settings;
                    };
                }
            });
        }
    }
    appControllers.controller('GenericAnalyticGridController', genericAnalyticGridController);

})(appControllers);