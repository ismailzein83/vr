'use strict';

app.directive('retailBeStatuscharginginfo', ['Retail_BE_StatusChargingSetAPIService', 'VRNotificationService', 'Retail_BE_StatusChargingSetService',
    function (retailBeStatusChargingSetApiService, vrNotificationService, retailBeStatusChargingSetService) {

        function retailBeStatusCharginginfo($scope, ctrl, $attrs) {

        }

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/StatusChargingSet/Templates/StatusChargingInfoTemplate.html'
        };
    }]);
