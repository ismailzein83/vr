app.directive('vrRecompilerReady', ['$compile', '$parse', function ($compile, $parse) {

    var directiveDefinitionObject = {
        restrict: 'A',
        scope: false,
        link: function preLink(scope, iElement, iAttrs) {
            var api = {
                recompile: function () {
                    var originalHtml = iElement[0].innerHTML;
                    //console.log('before');
                    //console.log(iElement.html());
                    //iElement.html('');
                    //console.log('after empty');
                    //console.log(iElement.html());
                    ////$compile(iElement)(scope);
                    //setTimeout(function () {
                    //    iElement.html(originalHtml);
                    //    console.log('after refill');
                    //    console.log(iElement.html());
                    //    $compile(iElement)(scope);
                    //});
                    //console.log(scope);
                    //var parentScope = scope.$parent;
                    //scope.$destroy();
                    //var newScope = parentScope.$new();
                    //$compile(iElement)(newScope);
                    iElement.attr('ng-if', 'false');
                    $compile(iElement)(scope);
                    setTimeout(function () {
                        iElement.attr('ng-if', 'true');
                        iElement[0].innerHTML = originalHtml;
                        console.log(iElement[0].innerHTML);
                        $compile(iElement[0])(scope);
                    });                    
                }
            }; //console.log(iElement);
            //iElement.attr('ng-if', 'true');
            var onReadyMethod = $parse(iAttrs.vrRecompilerReady);
            onReadyMethod(scope, { api: api });
        }

    };
    return directiveDefinitionObject;

}]);