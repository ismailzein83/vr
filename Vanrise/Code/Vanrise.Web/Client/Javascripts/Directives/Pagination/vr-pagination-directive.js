'use strict';


app.directive('vrPagination', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            pagersettings: '='
        },
        controller: function ($scope, $element, $attrs) {
            var pagerCtrl = this;
            pagerCtrl.topCounts = [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 100];
            if (pagerCtrl.pagersettings != undefined) {
                pagerCtrl.pagersettings.itemsPerPage = pagerCtrl.topCounts[1];
                pagerCtrl.pagersettings.getPageInfo = function () {
                    var fromRow = (pagerCtrl.pagersettings.currentPage - 1) * pagerCtrl.pagersettings.itemsPerPage + 1;
                    return {
                        fromRow: fromRow,
                        toRow: fromRow + pagerCtrl.pagersettings.itemsPerPage - 1
                    };
                };

                pagerCtrl.pageChanged = function () {
                    if (pagerCtrl.pagersettings.pageChanged != undefined)
                        pagerCtrl.pagersettings.pageChanged();
                };

                pagerCtrl.pageSizeChanged = function () {
                    if (pagerCtrl.pagersettings.pageChanged != undefined)
                        pagerCtrl.pagersettings.pageChanged();
                };
            }


        },
        controllerAs: 'pagerCtrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                    //var ctrl = $scope.ctrl;
                }
            };
        },
        templateUrl: function (element, attrs) {
            return "/Client/Javascripts/Directives/Pagination/vr-pagination.html";
        }

    };

    return directiveDefinitionObject;



}]);

