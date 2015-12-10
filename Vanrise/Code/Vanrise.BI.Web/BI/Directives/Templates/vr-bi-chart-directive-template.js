VrChartDirectiveTemplateController.$inject = ['$scope', 'TimeDimensionTypeEnum', 'ChartDefinitionTypeEnum', 'BIConfigurationAPIService', 'ChartSeriesTypeEnum', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function VrChartDirectiveTemplateController($scope, TimeDimensionTypeEnum, ChartDefinitionTypeEnum, BIConfigurationAPIService, ChartSeriesTypeEnum, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {
    var lastTopMeasureValue;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
    }
    function defineScope() {
        $scope.Measures = [];
        $scope.Entities = [];
        
        $scope.selectedEntitiesType=[];
        $scope.definitionTypes = [];
        $scope.selectedDefinitionType;
        $scope.selectedMeasureTypes = [];
        $scope.selectedMeasureType;
        $scope.selectedTopMeasure;
        $scope.singleMeasureRequired = false;
        $scope.topMeasureRequired = false;
        $scope.multipleMeasureRequired = false;
        $scope.entityRequired = false;
        $scope.isPieChart = true;
        $scope.onSwitchValueChanged = function () {
            if ($scope.isPieChart)
            {
                $scope.selectedTopMeasure = undefined;
                $scope.singleMeasureRequired = true;
                $scope.multipleMeasureRequired = false;
                $scope.topMeasureRequired = false;
                $scope.selectedTopMeasure = lastTopMeasureValue;
            }
            else {
                $scope.singleMeasureRequired = false;
                $scope.multipleMeasureRequired = true;
                $scope.topMeasureRequired = true;
                $scope.selectedTopMeasure = $scope.selectedMeasureTypes.length > 0 ? $scope.selectedMeasureTypes[0] : undefined;
            }

        }
        $scope.onSelectionOperationChanged = function () {
            if ($scope.selectedOperationType.value == "MeasuresGroupedByTime")
            {
                $scope.singleMeasureRequired = false;
                $scope.topMeasureRequired = false;
                $scope.multipleMeasureRequired = true;
                $scope.entityRequired = false;
            }
            else {
                $scope.singleMeasureRequired = true;
                $scope.topMeasureRequired = false;
                $scope.multipleMeasureRequired = false;
                $scope.entityRequired = true;
            }
        }

        $scope.onSelectionChanged = function () {
            if ($scope.selectedTopMeasure == undefined)
            {
                $scope.selectedMeasureType != undefined && $scope.isPieChart ? $scope.selectedTopMeasure = $scope.selectedMeasureType : $scope.selectedTopMeasure = $scope.selectedMeasureTypes[0];
            }
            else {
                if ($scope.selectedMeasureTypes.length > 0 && !UtilsService.contains($scope.selectedMeasureTypes, $scope.selectedTopMeasure))
                {
                    $scope.selectedTopMeasure = $scope.selectedMeasureTypes[0];

                }
                else if(!$scope.isPieChart && $scope.selectedMeasureTypes.length== 0 )

                    $scope.selectedTopMeasure = undefined;
                else if ($scope.selectedMeasureType != undefined && $scope.isPieChart)
                    $scope.selectedTopMeasure = $scope.selectedMeasureType;
            }
            lastTopMeasureValue = $scope.selectedTopMeasure;
        }
        $scope.topRecords = 10;
       
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
        if ($scope.selectedOperationType.value=="TopEntities" && $scope.isPieChart){
            if ($scope.selectedEntitiesType.length==0 || $scope.selectedMeasureType == undefined)
                return false;
            else {
                measureTypes.push($scope.selectedMeasureType.Name);
            }
        }
        else if($scope.selectedOperationType.value== "MeasuresGroupedByTime" ||  !$scope.isPieChart){
            if ($scope.selectedMeasureTypes == undefined || $scope.selectedMeasureTypes.length == 0)
                return false;
            else
            {
                for (var i = 0; i < $scope.selectedMeasureTypes.length; i++)
                {
                    measureTypes.push($scope.selectedMeasureTypes[i].Name);
                    if($scope.selectedMeasureTypes[i].Name== $scope.selectedTopMeasure.Name)
                    {
                        var swap = measureTypes[0];
                        measureTypes[0] = $scope.selectedMeasureTypes[i].Name;
                        measureTypes[i] = swap;
                    }
                }
                   
            }
        }
        var topMeasure = null;
        if ($scope.selectedTopMeasure != undefined)
            topMeasure = $scope.selectedTopMeasure.Name;
        var entityType = [];
       
        if ($scope.selectedEntitiesType.length > 0 && $scope.selectedOperationType.value != "MeasuresGroupedByTime")
        {
            for (var i = 0; i < $scope.selectedEntitiesType.length; i++)
                entityType.push($scope.selectedEntitiesType[i].Name);
        }

        return {
            $type: "Vanrise.BI.Entities.ChartDirectiveSetting, Vanrise.BI.Entities",
            OperationType: $scope.selectedOperationType.value,
            EntityType: entityType,
            MeasureTypes: measureTypes,
            TopMeasure: topMeasure,
            DefinitionType: $scope.selectedDefinitionType.value,
            IsPieChart: $scope.isPieChart,
            TopRecords: $scope.topRecords
        };
    }
    function setSubViewValue(settings) {
       
        if (settings == undefined)
            return;
        if (settings.DefinitionType != undefined) {
            for (var i = 0; i < $scope.definitionTypes.length; i++) {
                if ($scope.definitionTypes[i].value == settings.DefinitionType)
                    $scope.selectedDefinitionType = $scope.definitionTypes[i];
            }
        }
        $scope.isPieChart = settings.IsPieChart;
       
        if (settings.EntityType != undefined) {
            $scope.selectedEntitiesType.length = 0;
            for (j = 0; j < settings.EntityType.length; j++) {
                for (i = 0; i < $scope.Entities.length; i++) {
                    if ($scope.Entities[i].Name == settings.EntityType[j] && !UtilsService.contains($scope.selectedEntitiesType, $scope.Entities[i])) {
                        $scope.selectedEntitiesType.push($scope.Entities[i]);
                    }
                }
            }
        }
       
        $scope.topRecords = settings.TopRecords;
        if (settings.operationTypes != undefined) {
            for (var i = 0; i < $scope.operationTypes.length; i++) {

                if ($scope.operationTypes[i].value == settings.OperationType)
                    $scope.selectedOperationType = $scope.operationTypes[i];
            }
        }
        for (var i = 0; i < settings.MeasureTypes.length; i++) {
            var measureType = settings.MeasureTypes[i];
            for (j = 0; j < $scope.Measures.length; j++) {

                if (measureType == $scope.Measures[j].Name)
                {
                    if ($scope.selectedOperationType.value == "TopEntities" && settings.IsPieChart)
                    {
                        $scope.singleMeasureRequired = true;
                        $scope.multipleMeasureRequired = false;
                        $scope.topMeasureRequired = false;
                        $scope.selectedMeasureType = $scope.Measures[j];
                    }
                        
                    else
                    {  
                        $scope.selectedMeasureTypes.push($scope.Measures[j]);
                        $scope.multipleMeasureRequired = true;
                        $scope.singleMeasureRequired = false;
                        if ($scope.selectedOperationType.value == "MeasuresGroupedByTime") {
                            $scope.topMeasureRequired = false;
                            $scope.entityRequired = false;
                        }
                        else {
                            $scope.topMeasureRequired = true;
                            $scope.entityRequired = true;
                        }
                    }
                       
                }
                
                

            }
        }
        lastTopMeasureValue = $scope.selectedTopMeasure;
       


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
        for (var m in ChartDefinitionTypeEnum) {
            $scope.definitionTypes.push(ChartDefinitionTypeEnum[m]);
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
          //  $scope.selectedEntitiesType.push($scope.Entities[0])  ;
        });
    }

}
appControllers.controller('BI_VrChartDirectiveTemplateController', VrChartDirectiveTemplateController);
