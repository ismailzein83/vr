EntityReportController.$inject = ['$scope', 'UtilsService', 'BITimeDimensionTypeEnum', 'BIEntityTypeEnum', 'BIMeasureTypeEnum',  'BIAPIService', 'BIUtilitiesService'];

function EntityReportController($scope, UtilsService, BITimeDimensionTypeEnum, BIEntityTypeEnum, BIMeasureTypeEnum, BIAPIService, BIUtilitiesService) {

        defineScope();
        var entityType;
        var entityId; 
        load();
        loadParametresValue();       
        function defineScope() {
            $scope.data = [];
            $scope.fromDate = "2015-04-01";           
            $scope.toDate = "2015-04-30";
            
            

            defineTimeDimensionTypes();
            defineMeasureTypes()

            $scope.onchangeEntityType = function () {                
                loadEntityData()

            }
            $scope.search = function () {
                var measureTypes = [];
                for (var prop in BIMeasureTypeEnum) {
                    measureTypes.push(BIMeasureTypeEnum.value);
                }
                return BIAPIService.GetEntityMeasuresValues(entityType, entityId, $scope.selectedTimeDimensionType.value, $scope.fromDate, $scope.toDate, measureTypes)
                 .then(function (response) {

                     $scope.data.length = 0;
                     BIUtilitiesService.fillDateTimeProperties(response, $scope.selectedTimeDimensionType.value, $scope.fromDate, $scope.toDate, true);
                     angular.forEach(response, function (itm) {
                         $scope.data.push(itm);
                     });

                 }).catch(function (error) {

                 });

            }
        }

        function load() {
            
        }
        function loadParametresValue() {
            entityType = 0;
            entityId = 27708;
        }

        function defineTimeDimensionTypes() {
            $scope.timeDimensionTypes = [];
            for (var td in BITimeDimensionTypeEnum)
                $scope.timeDimensionTypes.push(BITimeDimensionTypeEnum[td]);

            $scope.selectedTimeDimensionType = $.grep($scope.timeDimensionTypes, function (t) {
                return t == BITimeDimensionTypeEnum.Daily;
            })[0];
        }
        function defineMeasureTypes() {
            $scope.measureTypes = [];
            for (var m in BIMeasureTypeEnum) {
                $scope.measureTypes.push(BIMeasureTypeEnum[m]);
            }

        }

}

appControllers.controller('BI_EntityReportController', EntityReportController);