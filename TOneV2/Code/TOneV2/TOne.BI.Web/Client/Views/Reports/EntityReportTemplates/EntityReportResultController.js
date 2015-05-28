EntityReportResultController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'BITimeDimensionTypeEnum', 'BIEntityTypeEnum', 'BIMeasureTypeEnum', 'BIAPIService', 'BIUtilitiesService'];

function EntityReportResultController($scope, VRNavigationService, UtilsService, BITimeDimensionTypeEnum, BIEntityTypeEnum, BIMeasureTypeEnum, BIAPIService, BIUtilitiesService) {

        var entityType;
        var entityId;
        defineScope();       
        load();   
        function defineScope() {
            $scope.data = [];
            $scope.isGettingData = false;
            defineMeasureTypes()
        }

        function load() {
            console.log($scope.viewScope);
            entityType = $scope.viewScope.entityType;
            entityId = $scope.viewScope.entityId;
            if ($scope.dataItem != undefined) {                
                loadData();
            }
        }
       
        function defineMeasureTypes() {
            $scope.measureTypes = [];
            for (var m in BIMeasureTypeEnum) {
                $scope.measureTypes.push(BIMeasureTypeEnum[m]);
            }

        }

        function loadData() {
            $scope.isGettingData = true;
            var measureTypes = [];
            angular.forEach($scope.measureTypes, function (measureType) {
                measureTypes.push(measureType.value);
            });
            var fromDate = $scope.dataItem.Time;
            var toDate = BIUtilitiesService.getNextDate($scope.dataItem.timeDimensionType, $scope.dataItem);
            var currentTimeDimension = $scope.dataItem.timeDimensionType +1 ;
            return BIAPIService.GetEntityMeasuresValues(entityType, entityId, currentTimeDimension, fromDate, toDate, measureTypes)
             .then(function (response) {

                 $scope.data.length = 0;
                 BIUtilitiesService.fillDateTimeProperties(response, currentTimeDimension, fromDate, toDate, true);
                 angular.forEach(response, function (itm) {
                     itm.timeDimensionType = currentTimeDimension;
                     $scope.data.push(itm);
                 });
                 $scope.isGettingData = false;

             }).catch(function (error) {
                 $scope.isGettingData = false;
             });

        }
}

appControllers.controller('BI_EntityReportResultController', EntityReportResultController);