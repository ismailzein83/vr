'use strict';
app.directive('vrHtmlcompiler', ['$compile', function ($compile) {
    var directiveDefinitionObject = {

        restrict: 'A',
        replace: true,
        link: function ($scope, $element, $attrs) {
            $scope.$on("$destroy", function () {
                vrHtmlcompilerWatch();
                $element.remove();
            });
            var htmlData = $attrs.vrHtmlcompiler;
            var vrHtmlcompilerWatch =  $scope.$watch($attrs.vrHtmlcompiler, function (html) {
                if (html != undefined) {
                    $element.html(html);
                    $compile($element.contents())($scope);
                }                                   
            });
        },

    };

    return directiveDefinitionObject;
}]);