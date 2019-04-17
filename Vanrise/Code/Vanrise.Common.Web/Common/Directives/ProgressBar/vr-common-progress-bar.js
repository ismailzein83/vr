'use strict';

app.directive('vrCommonProgressBar', [ 'UtilsService', 'VRUIUtilsService',

    function ( UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                items: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                $(function () {
                    $('[data-toggle="tooltip"]').tooltip()
                }); 

            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/ProgressBar/Templates/ProgressBarTemplate.html'
        };

        
    }]); 