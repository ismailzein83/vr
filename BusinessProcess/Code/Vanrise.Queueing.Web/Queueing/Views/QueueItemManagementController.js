(function(appControllers) {
    
    "use strict";

    appControllers.controller('Queueing_QueueItemManagementController', queueItemManagementController);

    queueItemManagementController.$inject = ['$scope', 'QueueingAPIService', 'UtilsService'];

    function queueItemManagementController($scope, QueueingAPIService, UtilsService) {
        
        function defineScope() {
            $scope.queueItemTypes = [];
            $scope.selectedQueueItemTypes = [];
            $scope.queueInstances = [];
            $scope.selectedQueueInstances = [];
            $scope.onchangeQueueItemTypes = function () {
                QueueingAPIService.GetQueueInstances(UtilsService.getPropValuesFromArray($scope.selectedQueueItemTypes, "Id")).then(function (response) {
                    $scope.queueInstances = [];
                    for (var i = 0, len = response.length; i < len; i++) {
                        $scope.queueInstances.push(response[i]);
                    }
                });
            };
        }

        function loadFilters() {

            QueueingAPIService.GetQueueItemTypes().then(function (response) {

                for (var i = 0, len = response.length; i < len; i++) {
                    $scope.queueItemTypes.push(response[i]);
                }
            });
        }

        defineScope();
        loadFilters();
    }

})(appControllers);