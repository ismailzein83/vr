EntityReportController.$inject = ['$scope', 'UtilsService', 'BITimeDimensionTypeEnum', 'BIEntityTypeEnum', 'BIMeasureTypeEnum', 'BIAPIService', 'BIUtilitiesService', 'VRNavigationService'];

function EntityReportController($scope, UtilsService, BITimeDimensionTypeEnum, BIEntityTypeEnum, BIMeasureTypeEnum, BIAPIService, BIUtilitiesService, VRNavigationService) {

        defineScope();
        load();
        loadParametresValue();       
        function defineScope() {
            $scope.data = [];
            $scope.fromDate = "2015-04-01";           
            $scope.toDate = "2015-04-30";
            $scope.isGettingData = false;
            

            defineTimeDimensionTypes();
            defineMeasureTypes()

            $scope.onchangeEntityType = function () {                
                loadEntityData()

            }
            $scope.search = function () {
                $scope.isGettingData = true;
                var measureTypes = [];
                angular.forEach($scope.measureTypes, function (measureType) {
                    measureTypes.push(measureType.value);
                });
                return BIAPIService.GetEntityMeasuresValues($scope.entityType, $scope.entityId, $scope.selectedTimeDimensionType.value, $scope.fromDate, $scope.toDate, measureTypes)
                 .then(function (response) {

                     $scope.data.length = 0;
                     BIUtilitiesService.fillDateTimeProperties(response, $scope.selectedTimeDimensionType.value, $scope.fromDate, $scope.toDate, true);
                     angular.forEach(response, function (itm) {
                         itm.timeDimensionType = $scope.selectedTimeDimensionType.value;
                         $scope.data.push(itm);
                     });
                     $scope.isGettingData = false;

                 }).catch(function (error) {
                     $scope.isGettingData = false;
                 });

            }
        }

        function load() {
            
        }
        function loadParametresValue() {
             
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != null && parameters != undefined) {
                $scope.entityType = parameters.EntityType;
                $scope.entityId = parameters.EntityId;
                $scope.title = parameters.EntityName;
            }
            else {
                $scope.entityType = 0;
                $scope.entityId = 27708;
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
        function defineMeasureTypes() {
            $scope.measureTypes = [];
            for (var m in BIMeasureTypeEnum) {
                $scope.measureTypes.push(BIMeasureTypeEnum[m]);
            }

        }

}

appControllers.controller('BI_EntityReportController', EntityReportController);