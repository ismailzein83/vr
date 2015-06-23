VrChartDirectiveTemplateController.$inject = ['$scope','BITimeDimensionTypeEnum', 'BIConfigurationAPIService', 'ChartSeriesTypeEnum', 'DynamicPagesManagementAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function VrChartDirectiveTemplateController($scope,BITimeDimensionTypeEnum, BIConfigurationAPIService, ChartSeriesTypeEnum, DynamicPagesManagementAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {
    //var mainGridAPI;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
    }
    function defineScope() {
        $scope.Measures = [];
        $scope.Entities = [];
        $scope.fromDate = "2015-04-01";
        $scope.toDate = "2015-04-30";
        $scope.selectedEntityType;
        $scope.selectedMeasureTypes = [];
        defineTimeDimensionTypes();
        $scope.subViewValue.getValue = function () {
            return getSubViewValue();
        }



    }
    function getSubViewValue() {
        return {
            operationType: $scope.selectedOperationType,
            entityType: $scope.selectedEntityType,
            measureTypes: $scope.selectedMeasureTypes,
            timedimensiontype: $scope.selectedTimeDimensionType,
            fromdate:   $scope.fromDate ,
            todate:$scope.toDate 
        };
    }
    function defineTimeDimensionTypes() {
        $scope.timeDimensionTypes = [];
        for (var td in BITimeDimensionTypeEnum)
            $scope.timeDimensionTypes.push(BITimeDimensionTypeEnum[td]);

        $scope.selectedTimeDimensionType = $.grep($scope.timeDimensionTypes, function (t) {
            return t == BITimeDimensionTypeEnum.Daily;
        })[0];
    }
    function load() {
        defineNumberOfColumns();
        defineOperationTypes();
        defineChartSeriesTypes();
        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadMeasures, loadEntities]).finally(function () {
            $scope.isInitializing = false;
            $scope.isGettingData = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });

    }

    function defineNumberOfColumns() {
        $scope.numberOfColumns = [
            {
                value: "6",
                description: "Half Row"
            },
            {
                value: "12",
                description: "Full Row"
            }
        ];

        $scope.selectedNumberOfColumns = $scope.numberOfColumns[0];
    }
    function defineOperationTypes() {
        $scope.operationTypes = [{
            value: "TopEntities",
            description: "Top X"
        }, {
            value: "MeasuresGroupedByTime",
            description: "Time Variation Data"
        }
        ];
        $scope.selectedOperationType = $scope.operationTypes[0];
    }

    function defineChartSeriesTypes() {
        $scope.chartSeriesTypes = [];
        for (var m in ChartSeriesTypeEnum) {
            $scope.chartSeriesTypes.push(ChartSeriesTypeEnum[m]);
        }
    }

    function loadMeasures() {
        return BIConfigurationAPIService.GetMeasures().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.Measures.push(itm);
                console.log(itm);
            });
        });
    }
    function loadEntities() {
        return BIConfigurationAPIService.GetEntities().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.Entities.push(itm);
                console.log($scope.Entities[0].Id);
            });
        });
    }

}
appControllers.controller('BI_VrChartDirectiveTemplateController', VrChartDirectiveTemplateController);
