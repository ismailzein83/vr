'use strict';

var dSearchPlaceholder = "Search ...";
var dSearchIcon = "search";


app.directive('vTextbox', function () {
    return {
        restrict: 'E',
        scope: {
            holder: '@placeholder',
            icon: '@icon',
        },
        compile: function (element, attrs) {
            if (attrs.type == 'search') {
                if (attrs.icon == undefined)
                    attrs.$set("icon", dIconPlaceholder);
                if (attrs.placeholder == undefined)
                    attrs.$set("placeholder", dSearchPlaceholder);
            }
        },
        templateUrl: function (elem, attrs) {
            if (attrs.type == 'search') return '../../Client/Templates/Directives/VTextboxIcon.html';
            return '../../Client/Templates/Directives/VTextbox' + attrs.type + '.html';
        }
    };
});