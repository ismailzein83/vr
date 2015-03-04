'use strict';

app.directive('vTextbox', function () {
    return {
        restrict: 'E',
        scope: {
            holder: '@placeholder',
            icon:'@icon'
        },
        templateUrl: function(elem, attr){
            return '../../Client/Templates/Directives/VTextbox' + attr.type + '.html';
        }
    };
});