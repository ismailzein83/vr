'use strict';

app.directive('vInput', function () {
    return {
        restrict: 'E',
        scope: {
            holder: '@placeholder'
        },
        templateUrl: function(elem, attr){
            return '../../Client/Templates/Directives/VInput' + attr.type + '.html';
        }
    };
});