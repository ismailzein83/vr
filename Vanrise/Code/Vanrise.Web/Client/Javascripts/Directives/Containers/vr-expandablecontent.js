(function (app) {

    "use strict";

    app.directive('vrExpandablecontent', ['$timeout', function ($timeout) {
        var defOption = {
            height: 35
        };
        var directiveDefinitionObject = {
            restrict: 'E',
            transclude: true,
            scope: {},
            template: function(){
                return '<div class="showless"><div ng-transclude class="content" ng-style="style"></div><section class="trigger" ng-if="showSection"> \
                          <span ng-click="toggleMore()">{{more ? "See More":"Less"}}</span> \
                        </section></div>'
            },
            link: function (scope, elm, attrs) {
                var origHeight, options = angular.extend({}, angular.copy(defOption), scope.$eval(attrs.options));

                initialize();


                function initialize() {
                    scope.toggleMore = toggleMore;
                    scope.toggleSection = toggleSection;
                    scope.toggleSection(false);
                    $timeout(_initShowLess);
                }

                function _initShowLess() {
                    origHeight = elm.find('.content').height();
                    scope.style = { height: origHeight };
                    if (origHeight > options.height + 17) {
                        scope.toggleSection(true);
                        scope.style.height = options.height;
                        scope.toggleMore();
                    }
                }

                function toggleMore() {
                    expandCollapse(scope.more);
                    scope.more = !scope.more;
                }

                function toggleSection(show) {
                    scope.showSection = show;
                }

                function expandCollapse(expand) {
                    var height = expand ? origHeight : options.height;
                    scope.style.height = height;
                }


            }
        };

        return directiveDefinitionObject;

    }]);

})(app);




