VrDatagridDirectiveTemplateController.$inject = ['$scope','TimeDimensionTypeEnum', 'BIConfigurationAPIService', 'ChartSeriesTypeEnum', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function VrDatagridDirectiveTemplateController($scope, TimeDimensionTypeEnum,BIConfigurationAPIService, ChartSeriesTypeEnum, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
    }

    function defineScope() {
        $scope.Measures = [];
        $scope.Entities = [];
        $scope.selectedEntitiesType = [];
        $scope.topRecords = 10;
        $scope.selectedMeasureTypes = [];

        $scope.selectedTopMeasure;
        defineTimeDimensionTypes();
        $scope.onSelectionChanged = function () {
            if ($scope.selectedTopMeasure==undefined)
                $scope.selectedTopMeasure = $scope.selectedMeasureTypes[0];
            else {
                if (!UtilsService.contains($scope.selectedMeasureTypes, $scope.selectedTopMeasure))
                    $scope.selectedTopMeasure = $scope.selectedMeasureTypes[0];
            }
        }
        $scope.subViewConnector.getValue = function () {
            return getSubViewValue();
        }

        $scope.onSelectionOperationChanged = function () {
            if ($scope.selectedOperationType.value == "MeasuresGroupedByTime") {
                $scope.entityRequired = false;
            }
            else {
                $scope.entityRequired = true;
            }
        }
    }

    function getSubViewValue() {

        switch ($scope.selectedOperationType.value) {
            case "TopEntities": if ($scope.selectedEntitiesType.length == 0  || $scope.selectedMeasureTypes == undefined || $scope.selectedMeasureTypes.length == 0) return false;
            case "MeasuresGroupedByTime": if ($scope.selectedMeasureTypes == undefined ||$scope.selectedMeasureTypes.length == 0) return false;
        }
        
        var topMeasure = null;
        if ($scope.selectedTopMeasure != undefined)
            topMeasure = $scope.selectedTopMeasure.Name;
        var measureTypes = [];
        for (var i = 0; i < $scope.selectedMeasureTypes.length; i++) {
            measureTypes.push($scope.selectedMeasureTypes[i].Name);
            if ($scope.selectedMeasureTypes[i].Name == $scope.selectedTopMeasure.Name) {
                var swap = measureTypes[0];
                measureTypes[0] = $scope.selectedMeasureTypes[i].Name;
                measureTypes[i] = swap;
            }
        }    
        var entityType = [];
        if ($scope.selectedEntitiesType.length > 0 && $scope.selectedOperationType.value != "MeasuresGroupedByTime")
        {
            for (var i = 0; i < $scope.selectedEntitiesType.length; i++)
                entityType.push($scope.selectedEntitiesType[i].Name);
        
        }
           
        return {
            $type: "Vanrise.BI.Entities.DataGridDirectiveSetting, Vanrise.BI.Entities",
            OperationType: $scope.selectedOperationType.value,
            EntityType: entityType,
            MeasureTypes: measureTypes,
            TopMeasure: topMeasure,
            TopRecords: $scope.topRecords
        };
    }

    function setSubViewValue(settings) {
        if (settings.EntityType != undefined)
        {
            $scope.selectedEntitiesType.length = 0;
            for (j = 0; j < settings.EntityType.length; j++) {
                for (var i = 0; i < $scope.Entities.length; i++) {

                    if ($scope.Entities[i].Name == settings.EntityType[j] && !UtilsService.contains($scope.selectedEntitiesType, $scope.Entities[i])) {
                        $scope.selectedEntitiesType.push($scope.Entities[i]);

                    }
                }
            }
        }

        $scope.topRecords = settings.TopRecords;
        for (var i = 0; i < settings.MeasureTypes.length; i++) {
            var measureType=settings.MeasureTypes[i];
            for (j = 0; j < $scope.Measures.length; j++) {
                if (measureType == $scope.Measures[j].Name)
                    $scope.selectedMeasureTypes.push($scope.Measures[j]);
                if ($scope.Measures[j].Name == settings.TopMeasure)
                    $scope.selectedTopMeasure = $scope.Measures[j];
            }
        }

        if (settings.OperationType != undefined) {
            for (var i = 0; i < $scope.operationTypes.length; i++) {

                if ($scope.operationTypes[i].value == settings.OperationType)
                    $scope.selectedOperationType = $scope.operationTypes[i];
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
            });
        });
    }

    function loadEntities() {
        return BIConfigurationAPIService.GetEntities().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.Entities.push(itm);
            });
           // $scope.selectedEntitiesType.push($scope.Entities[0]);
        });
    }

}
appControllers.controller('BI_VrDatagridDirectiveTemplateController', VrDatagridDirectiveTemplateController);
