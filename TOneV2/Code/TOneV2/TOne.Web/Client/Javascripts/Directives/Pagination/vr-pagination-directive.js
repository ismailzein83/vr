'use strict';


app.directive('vrPagination', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            pagersettings: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.topCounts = [5, 10, 15];
            if (ctrl.pagersettings != undefined) {
                ctrl.pagersettings.itemsPerPage = ctrl.topCounts[1];
            }

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                    var ctrl = $scope.ctrl;
                }
            }
        },
        templateUrl: function (element, attrs) {
            return "/Client/Javascripts/Directives/Pagination/vr-pagination.html";
        }

    };

    return directiveDefinitionObject;



}]);

