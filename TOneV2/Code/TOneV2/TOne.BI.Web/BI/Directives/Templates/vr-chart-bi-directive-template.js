﻿VrChartDirectiveTemplateController.$inject = ['$scope','BITimeDimensionTypeEnum', 'BIConfigurationAPIService', 'ChartSeriesTypeEnum', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function VrChartDirectiveTemplateController($scope,BITimeDimensionTypeEnum, BIConfigurationAPIService, ChartSeriesTypeEnum, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {
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
        $scope.selectedTopMeasure;
        $scope.selectedMeasureTypes = [];
        defineTimeDimensionTypes();
        $scope.onSelectionChanged = function () {
            $scope.selectedTopMeasure = $scope.selectedMeasureTypes[0];
        }
        $scope.subViewConnector.getValue = function () {
            return getSubViewValue();
        }
        $scope.subViewConnector.setValue = function (value) {
            console.log(value);
            $scope.subViewConnector.value = value;
           // return setSubViewValue(settings);
        }


    }
    function getSubViewValue() {
        var topMeasure = null;
        if ($scope.selectedTopMeasure != undefined)
            topMeasure = $scope.selectedTopMeasure.Name;
        var measureTypes = [];
        for (var i = 0; i < $scope.selectedMeasureTypes.length; i++) {
            measureTypes.push($scope.selectedMeasureTypes[i].Name);
        }
        var entityType = null;
        if ($scope.selectedEntityType != undefined)
            entityType = $scope.selectedEntityType.Name;
        return {
            $type: "TOne.BI.Entities.ChartDirectiveObject, TOne.BI.Entities",
            OperationType: $scope.selectedOperationType.value,
            EntityType: entityType,
            MeasureTypes: measureTypes,
            TopMeasure: topMeasure
        };
    }
    function setSubViewValue(settings) {
        if (settings == undefined)
            return;
        for (i = 0; i < $scope.Entities.length; i++) {
            
            if ($scope.Entities[i].Name == settings.EntityType) {
                $scope.selectedEntityType = $scope.Entities[i];
            
            }
        }
        for (var i = 0; i < settings.MeasureTypes.length; i++) {
            var measureType=settings.MeasureTypes[i];
            for (j = 0; j < $scope.Measures.length; j++)
            {
               
                if (measureType == $scope.Measures[j].Name)
                $scope.selectedMeasureTypes.push($scope.Measures[j]);
                if ($scope.Measures[j].Name == settings.TopMeasure)
                $scope.selectedTopMeasure = $scope.Measures[j];
        }
             }
        for (var i = 0; i < $scope.operationTypes.length; i++) {
          
                if($scope.operationTypes[i].value==settings.OperationType)
                    $scope.selectedOperationType=$scope.operationTypes[i];
            }
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
            if ($scope.subViewConnector.value != null && $scope.subViewConnector.value != undefined) {
                setSubViewValue($scope.subViewConnector.value);
            }
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
               // console.log(itm);
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
appControllers.controller('BI_VrChartDirectiveTemplateController', VrChartDirectiveTemplateController);
