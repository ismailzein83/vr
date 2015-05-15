'use strict';


app.directive('vrDatagridrows', [function () {
    var directiveDefinitionObject = {

        restrict: 'E',
        require: '^vrDatagrid',
        scope: {},
        controller: function ($scope, $element) {            

        },
        compile: function (element, attrs) {
            var rowTemplateElement = element.find('#rowTemplate');
            var rowTemplate = rowTemplateElement.html();
            rowTemplateElement.html('');
            element.find('#rowSection1').html(rowTemplate);
            element.find('#rowSection2').html(rowTemplate);

            return {
                pre: function (scope, elem, attrs, dataGridCtrl) {
                    scope.isGridScope = true;
                    scope.ctrl = dataGridCtrl;
                    scope.gridParentScope = scope.$parent;
                    scope.viewScope = scope.$parent;
                    while (scope.viewScope.isGridScope)
                        scope.viewScope = scope.viewScope.$parent;

                    var lastScrollTop;
                    var gridBodyElement = elem.find("#gridBody");
                    elem.find("#gridBodyContainer").scroll(function () {

                        var scrollTop = $(this).scrollTop();
                        var scrollPercentage = 100 * scrollTop / (gridBodyElement.height() - $(this).height());

                        if (scrollTop > lastScrollTop) {
                            if (scrollPercentage > 80)
                                dataGridCtrl.onScrolling();
                        }
                        lastScrollTop = scrollTop;
                    });
                }
            }
        },
        templateUrl: function (element, attrs) {
            return "/Client/Javascripts/Directives/DataGrid/vr-datagrid.html";
        }
    };

    return directiveDefinitionObject;

}]);

