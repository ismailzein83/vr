﻿'use strict';


app.directive('vrDirectivewrapper', ['$compile', '$injector', function ($compile, $injector) {

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
                    var cloneScope = null;
                    $scope.$on("$destroy", function () {
                        iElem.off();
                        directiveWatch();
                    });
                    var valueToWatch = iAttrs.valuetowatch != undefined ? iAttrs.valuetowatch : iAttrs.directive;

                    var directiveWatch = $scope.$watch(valueToWatch, function () {
                        var directive = $scope.$eval(iAttrs.directive);
                        if (directive != undefined) {
                            if (!$injector.has(attrs.$normalize(directive) + 'Directive'))
                                console.log('directive: ' + directive + ' not exists');
                        }
                        var newElement = "";
                        if (directive != undefined && directive != "") {
                            newElement = '<' + directive;
                            for (var prop in iAttrs.$attr) {
                                if (iAttrs.$attr[prop] != "directive")
                                    newElement += ' ' + iAttrs.$attr[prop] + '="' + iAttrs[prop] + '"';
                            }
                            newElement += ' ></' + directive + '>';
                        }

                        if (cloneScope) {
                            // ***************************************************
                            // NOTE: We are removing the element BEFORE we are
                            // destroying the scope associated with the element.
                            // ***************************************************

                            cloneScope.$destroy();
                            cloneScope = null;
                        }
                        setTimeout(function () {
                            cloneScope = $scope.$new();
                            iElem.html(newElement);
                            $compile(iElem.contents())(cloneScope);
                        });
                    });


                }
            };
        }

    };

    return directiveDefinitionObject;
}]);

