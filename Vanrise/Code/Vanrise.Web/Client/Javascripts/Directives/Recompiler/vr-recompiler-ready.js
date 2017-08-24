app.directive('vrRecompilerReady', ['$compile', '$parse', function ($compile, $parse) {

    var directiveDefinitionObject = {
        restrict: 'A',
        scope: false,
        link: function preLink(scope, iElement, iAttrs) {
            var api = {
                recompile: function () {
                    $compile(iElement)(scope);
                }
            };
            var onReadyMethod = $parse(iAttrs.vrRecompilerReady);
            onReadyMethod(scope, { api: api });
        }

    };
    return directiveDefinitionObject;

}]);