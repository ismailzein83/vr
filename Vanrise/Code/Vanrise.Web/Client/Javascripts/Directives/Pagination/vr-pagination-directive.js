'use strict';


app.directive('vrPagination', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            pagersettings: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.topCounts = [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 100, 200];
            if (ctrl.pagersettings != undefined) {
                ctrl.pagersettings.itemsPerPage = ctrl.topCounts[1];
                ctrl.pagersettings.getPageInfo = function () {
                    var fromRow = (ctrl.pagersettings.currentPage - 1) * ctrl.pagersettings.itemsPerPage + 1;
                    return {
                        fromRow: fromRow,
                        toRow: fromRow + ctrl.pagersettings.itemsPerPage - 1
                    };
                };

                ctrl.pageChanged = function () {
                    if (ctrl.pagersettings.pageChanged != undefined)
                        ctrl.pagersettings.pageChanged();
                };

                ctrl.pageSizeChanged = function () {
                    if (ctrl.pagersettings.pageChanged != undefined)
                        ctrl.pagersettings.pageChanged();
                };
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

