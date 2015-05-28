EntityReportResultController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'BITimeDimensionTypeEnum', 'BIEntityTypeEnum', 'BIMeasureTypeEnum', 'BIAPIService', 'BIUtilitiesService'];

function EntityReportResultController($scope, VRNavigationService, UtilsService, BITimeDimensionTypeEnum, BIEntityTypeEnum, BIMeasureTypeEnum, BIAPIService, BIUtilitiesService) {
        var gridAPI;
        var entityType;
        var entityId;
        defineScope();       
        load();   
        function defineScope() {
            $scope.data = [];
            $scope.isGettingData = false;
            $scope.gridReady = function (api) {
                gridAPI = api;
                var entityReportResultReadyAPI = {
                    loadData: function (fromDate, toDate, timeDimensionValue) {
                        return loadData(fromDate, toDate, timeDimensionValue);
                    }
                };
                if ($scope.onEntityReportResultReady != undefined)
                    $scope.onEntityReportResultReady(entityReportResultReadyAPI);
            };
        }

        function load() {
            defineMeasureTypes();
            if ($scope.viewScope != undefined) {
                entityType = $scope.viewScope.entityType;
                entityId = $scope.viewScope.entityId;
            }
            else {
                entityType = $scope.entityType;
                entityId = $scope.entityId;
            }
            if ($scope.dataItem != undefined) {
                var fromDate = $scope.dataItem.Time;
                var parentTimeDimensionValue = $scope.dataItem.timeDimensionType;
                var toDate = BIUtilitiesService.getNextDate(parentTimeDimensionValue, $scope.dataItem);
                var timeDimensionValue = parentTimeDimensionValue + 1;
                loadData(fromDate, toDate, timeDimensionValue);
            }
        }
       
        function defineMeasureTypes() {
            $scope.measureTypes = [];
            for (var m in BIMeasureTypeEnum) {
                $scope.measureTypes.push(BIMeasureTypeEnum[m]);
            }

        }

        function loadData(fromDate, toDate, timeDimensionValue) {
            $scope.timeDimensionValue = timeDimensionValue;
            $scope.isGettingData = true;
            var measureTypes = [];
            angular.forEach($scope.measureTypes, function (measureType) {
                measureTypes.push(measureType.value);
            });
            
            return BIAPIService.GetEntityMeasuresValues(entityType, entityId, timeDimensionValue, fromDate, toDate, measureTypes)
             .then(function (response) {

                 $scope.data.length = 0;
                 BIUtilitiesService.fillDateTimeProperties(response, timeDimensionValue, fromDate, toDate, true);
                 angular.forEach(response, function (itm) {
                     itm.timeDimensionType = timeDimensionValue;
                     $scope.data.push(itm);
                 });
                 $scope.isGettingData = false;

             }).catch(function (error) {
                 $scope.isGettingData = false;
             });

        }
}

appControllers.controller('BI_EntityReportResultController', EntityReportResultController);