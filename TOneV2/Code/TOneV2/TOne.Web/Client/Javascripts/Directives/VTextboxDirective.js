'use strict';

var dSearchPlaceholder = "Search ...";
var dSearchIcon = "search";

app.directive('vTextbox', function () {
    return {
        restrict: 'E',
        scope: {
            holder: '@placeholder',
            icon: '@icon',
            text:'=text'
        },
        compile: function (element, attrs) {

            if (attrs.type.toLowerCase() == 'search') {
                if (attrs.icon == undefined) attrs.$set("icon", dSearchIcon);
                if (attrs.placeholder == undefined) attrs.$set("placeholder", dSearchPlaceholder);
            }
        },
        templateUrl: function (elem, attrs) {
            if (attrs.type.toLowerCase() == 'search') return '../../Client/Templates/Directives/VTextboxIcon.html';
            return '../../Client/Templates/Directives/VTextbox' + attrs.type + '.html';
        }
    };
});