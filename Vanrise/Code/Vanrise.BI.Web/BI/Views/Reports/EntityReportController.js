EntityReportController.$inject = ['$scope', 'UtilsService', 'TimeDimensionTypeEnum', 'BIEntityTypeEnum', 'BIMeasureTypeEnum', 'VR_BI_BIAPIService', 'BIUtilitiesService', 'VRNavigationService'];

function EntityReportController($scope, UtilsService, TimeDimensionTypeEnum, BIEntityTypeEnum, BIMeasureTypeEnum, VR_BI_BIAPIService, BIUtilitiesService, VRNavigationService) {
        var resultAPI;
        defineScope();
        load();
        loadParametresValue();       
        function defineScope() {
            $scope.data = [];
            $scope.fromDate = "2015-04-01";           
            $scope.toDate = "2015-04-30";
            $scope.isGettingData = false;
            

            defineTimeDimensionTypes();
            defineMeasureTypes();

            $scope.onchangeEntityType = function () {
                loadEntityData();

            };
            $scope.search = function () {

                return resultAPI.loadData($scope.fromDate, $scope.toDate, $scope.selectedTimeDimensionType.value);

            };

            $scope.onEntityReportResultReady = function (api) {
                resultAPI = api;
                resultAPI.loadData($scope.fromDate, $scope.toDate, $scope.selectedTimeDimensionType.value);
            };
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
            for (var td in TimeDimensionTypeEnum)
                $scope.timeDimensionTypes.push(TimeDimensionTypeEnum[td]);

            $scope.selectedTimeDimensionType = $.grep($scope.timeDimensionTypes, function (t) {
                return t == TimeDimensionTypeEnum.Daily;
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