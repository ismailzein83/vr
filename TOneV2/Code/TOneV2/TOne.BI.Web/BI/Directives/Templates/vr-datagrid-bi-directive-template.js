VrDatagridDirectiveTemplateController.$inject = ['$scope','BITimeDimensionTypeEnum', 'BIConfigurationAPIService', 'ChartSeriesTypeEnum', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function VrDatagridDirectiveTemplateController($scope, BITimeDimensionTypeEnum,BIConfigurationAPIService, ChartSeriesTypeEnum, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {
    //var mainGridAPI;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
    }
    function defineScope() {
        $scope.Measures = [];
        $scope.Entities = [];
        $scope.selectedEntityType;
        $scope.selectedMeasureTypes = [];
        defineTimeDimensionTypes();
        $scope.subViewValue.getValue = function () {
            return getSubViewValue();
        }


    }
    function getSubViewValue() {
        var measureTypes = [];
       
        for (var i = 0; i < $scope.selectedMeasureTypes.length; i++) {
           
            measureTypes.push($scope.selectedMeasureTypes[i].Name);
        }
       
        var entityType = null;
        if ($scope.selectedEntityType != undefined)
            entityType = $scope.selectedEntityType.Name;
        return {
            $type: "TOne.BI.Entities.DataGridDirectiveObject, TOne.BI.Entities",
            OperationType: $scope.selectedOperationType.value,
            EntityType: entityType,
            MeasureTypes: measureTypes,
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
              //  console.log(itm);
            });
        });
    }
    function loadEntities() {
        return BIConfigurationAPIService.GetEntities().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.Entities.push(itm);
               // console.log($scope.Entities[0].Id);
            });
        });
    }

}
appControllers.controller('BI_VrDatagridDirectiveTemplateController', VrDatagridDirectiveTemplateController);
