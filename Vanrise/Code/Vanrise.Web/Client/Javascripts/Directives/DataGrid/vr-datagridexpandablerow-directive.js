'use strict';
app.directive('vrDatagridexpandablerow', [function () {
    return {
        restrict: 'E',
        require: '^vrDatagrid',
        compile: function (element, attrs) {
            var htmlTemplate = element.html();
            element.html('');
            return {
                pre: function ($scope, iElem, iAttrs, dataGridCtrl) {
                    dataGridCtrl.setExpandableRowTemplate(htmlTemplate);
                    $scope.$on('$destroy', function () {
                        iElem.remove();
                    });
                }
            };
        }
    };
}]);