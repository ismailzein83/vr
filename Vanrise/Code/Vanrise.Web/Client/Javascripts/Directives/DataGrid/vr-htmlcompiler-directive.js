
app.directive('vrHtmlcompiler', ['$compile', function ($compile) {
    var directiveDefinitionObject = {

        restrict: 'A',
        replace: true,
        link: function ($scope, $element, $attrs) {            

            $scope.$watch($attrs.vrHtmlcompiler, function (html) {
                $element.html(html);
                $compile($element.contents())($scope);
            });
        },

    };

    return directiveDefinitionObject;
}]);