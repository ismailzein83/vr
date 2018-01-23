'use strict';


app.directive('vrList', [ 'MultiTranscludeService', function ( MultiTranscludeService) {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        scope: {
            maxitemsperrow: '@',
            hideremoveicon: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
        },
        controllerAs: 'VRListCtrl',
        bindToController: true,
        compile: function (element, attrs) {

            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        template: function () {
            
            var template = '<vr-row removeline ng-transclude style="white-space: initial;">'
                            + '<vr-row>';
            return template;
        }


    };

    return directiveDefinitionObject;



}]);

