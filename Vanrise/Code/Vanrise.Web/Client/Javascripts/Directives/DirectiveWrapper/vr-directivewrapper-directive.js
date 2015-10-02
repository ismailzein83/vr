'use strict';


app.directive('vrDirectivewrapper', ['$compile', function ($compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;            

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            
            

            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                   
                    $scope.$watch(iAttrs.directive, function () {
                        var directive = $scope.$eval(iAttrs.directive);
                        var newElement = "";
                        if (directive != "")
                        {
                            newElement = '<' + directive;
                            for (var prop in iAttrs.$attr) {
                                if (iAttrs.$attr[prop] != "directive")
                                    newElement += ' ' + iAttrs.$attr[prop] + '="' + iAttrs[prop] + '"';
                            }
                            newElement += ' ></' + directive + '>';                          
                        }
                        iElem.html(newElement);
                        $compile(iElem.contents())($scope);
                    });
                   
                   
                }
            }
        }

    };

    return directiveDefinitionObject;
}]);

