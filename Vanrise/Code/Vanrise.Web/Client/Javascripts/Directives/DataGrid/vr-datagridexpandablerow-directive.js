'use strict';
app.directive('vrDatagridexpandablerow', [function () {
    return {
        restrict: 'E',
        require: '^vrDatagrid',
        compile: function (element, attrs) {
            var htmlTemplate = element.context.innerHTML;
            element.html('');
            return {
                pre: function ($scope, iElem, iAttrs, dataGridCtrl) {
                    dataGridCtrl.setExpandableRowTemplate(htmlTemplate);
                }
            };
        }
    };
}]);