(function(appControllers) {
    
    "use strict";

    appControllers.controller('Queueing_QueueItemManagementController', queueItemManagementController);

    queueItemManagementController.$inject = ['$scope', 'QueueingAPIService'];

    function queueItemManagementController($scope, QueueingAPIService) {
        
        function defineScope() {
            $scope.queueItemTypes = [];
            $scope.selectedQueueItemTypes = [];
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