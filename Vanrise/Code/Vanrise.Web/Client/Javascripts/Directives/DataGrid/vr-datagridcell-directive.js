'use strict';
app.directive('vrDatagridcell', ['$compile', function ($compile) {
    var directiveDefinitionObject = {

        restrict: 'E',
        controller: function ($scope, $element) {
            var ctrl = this;


        },
        controllerAs: 'ctrl',
        bindToController: true,
        link: function ($scope, $element, $attrs, $tabsCtrl) {
            var html = $scope.$eval($attrs.htmltemplate);

            $element.html(html);
            $compile($element.contents())($scope);
        }

    };

    return directiveDefinitionObject;
}]);