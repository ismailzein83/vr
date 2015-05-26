DynamicDashboardController.$inject = ['$scope', 'UtilsService', 'BITimeDimensionTypeEnum', 'BIEntityTypeEnum', 'BIMeasureTypeEnum', 'VRModalService'];

function DynamicDashboardController($scope, UtilsService, BITimeDimensionTypeEnum, BIEntityTypeEnum, BIMeasureTypeEnum, VRModalService) {

        defineScope();
        load();

        function defineScope() {

            $scope.fromDate = "2015-04-01";
            $scope.toDate = "2015-04-30";

            defineTimeDimensionTypes();
                    

            $scope.updateDashboard = function () {
                var refreshDataOperations = [];
                angular.forEach($scope.visualElements, function (visualElement) {
                    refreshDataOperations.push(visualElement.API.retrieveData);
                });
                $scope.isGettingData = true;
                return UtilsService.waitMultipleAsyncOperations(refreshDataOperations)
                    .finally(function () {
                        $scope.isGettingData = false;
                });
            };

            if ($scope.$root.dynamicDashboardSettings != undefined)
                $scope.visualElements = JSON.parse($scope.$root.dynamicDashboardSettings);
            else
                $scope.visualElements = [];

            $scope.addVisualElement = function () {
                var modalSettings = {};
                modalSettings.onScopeReady = function (modalScope) {
                    modalScope.onAddElement = function (element) {
                        addVisualElement(element.directive, element.settings, element.numberOfColumns);
                    }
                };

                VRModalService.showModal('/Client/Modules/BI/Views/Dashboards/DynamicDashboadTemplates/VisualElementEditor.html', null, modalSettings);
            };

            $scope.savePage = function () {
                $scope.$root.dynamicDashboardSettings = JSON.stringify($scope.visualElements);
            };

            $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };

            $scope.removeVisualElement = function (visualElement) {
                $scope.visualElements.splice($scope.visualElements.indexOf(visualElement), 1);
            };
        }

        function load() {
           
        }

        function defineTimeDimensionTypes() {
            $scope.timeDimensionTypes = [];
            for (var td in BITimeDimensionTypeEnum)
                $scope.timeDimensionTypes.push(BITimeDimensionTypeEnum[td]);

            $scope.selectedTimeDimensionType = $.grep($scope.timeDimensionTypes, function (t) {
                return t == BITimeDimensionTypeEnum.Daily;
            })[0];
        }

        function addVisualElement(directive, settings, numberOfColumns) {
            var visualElement = {
                directive: directive,
                settings: settings,
                numberOfColumns: numberOfColumns
            };
            visualElement.onElementReady = function (api) {                
                visualElement.API = api;
            };
            $scope.visualElements.push(visualElement);
        }

}

appControllers.controller('BI_DynamicDashboardController', DynamicDashboardController);