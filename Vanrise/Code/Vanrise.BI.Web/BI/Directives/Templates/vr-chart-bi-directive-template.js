VrChartDirectiveTemplateController.$inject = ['$scope', 'TimeDimensionTypeEnum', 'BIChartDefinitionTypeEnum', 'BIConfigurationAPIService', 'ChartSeriesTypeEnum', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function VrChartDirectiveTemplateController($scope, TimeDimensionTypeEnum, BIChartDefinitionTypeEnum, BIConfigurationAPIService, ChartSeriesTypeEnum, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {
    //var mainGridAPI;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
    }
    function defineScope() {
        $scope.Measures = [];
        $scope.Entities = [];
        $scope.selectedEntityType ;
        $scope.definitionTypes = [];
        $scope.selectedDefinitionType;
        $scope.selectedMeasureTypes = [];
        $scope.selectedMeasureType;
        defineTimeDimensionTypes();
        $scope.subViewConnector.getValue = function () {
            return getSubViewValue();
        }
        $scope.subViewConnector.setValue = function (value) {
            $scope.subViewConnector.value = value;
        }


    }
    function getSubViewValue() {
        var measureTypes = [];
       
        switch ($scope.selectedOperationType.value) {
            case "TopEntities":
                if ($scope.selectedEntityType == undefined || $scope.selectedEntityType == null || $scope.selectedMeasureType == undefined)
                    return false;
                else {
                    measureTypes.push($scope.selectedMeasureType.Name);
                    break;
                }
               

            case "MeasuresGroupedByTime":
                if ($scope.selectedMeasureTypes == undefined || $scope.selectedMeasureTypes.length == 0)
                    return false;
                else
                {
                 for (var i = 0; i < $scope.selectedMeasureTypes.length; i++) 
                     measureTypes.push($scope.selectedMeasureTypes[i].Name);
                 break;
                    }
        }

        var entityType = null;
        if ($scope.selectedEntityType != undefined)
            entityType = $scope.selectedEntityType.Name;
        return {
            $type: "TOne.BI.Entities.ChartDirectiveSetting, TOne.BI.Entities",
            OperationType: $scope.selectedOperationType.value,
            EntityType: entityType,
            MeasureTypes: measureTypes,
            TopMeasure: measureTypes[0],
            DefinitionType: $scope.selectedDefinitionType.value,
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
       
        for (var i = 0; i < $scope.operationTypes.length; i++) {
          
                if($scope.operationTypes[i].value==settings.OperationType)
                    $scope.selectedOperationType=$scope.operationTypes[i];
        }
        for (var i = 0; i < settings.MeasureTypes.length; i++) {
            var measureType = settings.MeasureTypes[i];
            for (j = 0; j < $scope.Measures.length; j++) {

                if (measureType == $scope.Measures[j].Name)
                {
                    if ($scope.selectedOperationType.value == "TopEntities")
                        $scope.selectedMeasureType = $scope.Measures[j];
                    else
                        $scope.selectedMeasureTypes.push($scope.Measures[j]);
                }
                   

            }
        }
       


    }
    function defineTimeDimensionTypes() {
        $scope.timeDimensionTypes = [];
        for (var td in TimeDimensionTypeEnum)
            $scope.timeDimensionTypes.push(TimeDimensionTypeEnum[td]);

        $scope.selectedTimeDimensionType = $.grep($scope.timeDimensionTypes, function (t) {
            return t == TimeDimensionTypeEnum.Daily;
        })[0];
    }
    function load() {
        defineChartDefinitionTypes();
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
    function defineChartDefinitionTypes() {
        $scope.definitionTypes = [];
        for (var m in BIChartDefinitionTypeEnum) {
            $scope.definitionTypes.push(BIChartDefinitionTypeEnum[m]);
        }
        $scope.selectedDefinitionType = $scope.definitionTypes[0];
    }

    function loadMeasures() {
        return BIConfigurationAPIService.GetMeasures().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.Measures.push(itm);
              
            });
        });
    }
    function loadEntities() {
        return BIConfigurationAPIService.GetEntities().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.Entities.push(itm);
            });
            $scope.selectedEntityType = $scope.Entities[0];
        });
    }

}
appControllers.controller('BI_VrChartDirectiveTemplateController', VrChartDirectiveTemplateController);
