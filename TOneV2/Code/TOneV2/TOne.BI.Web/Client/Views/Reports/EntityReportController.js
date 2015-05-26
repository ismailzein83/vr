EntityReportController.$inject = ['$scope', 'UtilsService', 'BITimeDimensionTypeEnum', 'BIEntityTypeEnum', 'BIMeasureTypeEnum'];

function EntityReportController($scope, UtilsService, BITimeDimensionTypeEnum, BIEntityTypeEnum, BIMeasureTypeEnum) {

        defineScope();
        load();

        function defineScope() {
            defineEntityTypes()
            defineEntity()

            $scope.onchangeEntityType = function () {
                
                console.log($scope.selectedEntityType)

            }
        }

        function load() {
           
        }

        
        function defineEntityTypes() {
            $scope.entityTypes = [];
            for (var e in BIEntityTypeEnum) {
                $scope.entityTypes.push(BIEntityTypeEnum[e]);
            }

            $scope.selectedEntityType = $scope.entityTypes[0];
        }
        function defineEntity() {
            $scope.entitiesData = [
            {value:1 , label: "test"}
            ];
            $scope.selectedEntity = $scope.entitiesData[0];
        }

}

appControllers.controller('BI_EntityReportController', EntityReportController);