app.directive('vrDatagridcolumn', function () {
    return {
        restrict: 'E',
        require: '^vrDatagrid',
        compile: function (element, attrs) {
            var cellTemplate = element.context.innerHTML.trim();
            element.html('');
            return {
                pre: function ($scope, iElem, iAttrs, dataGridCtrl) {
                    var headerText = iAttrs.headertext != undefined ? $scope.$eval(iAttrs.headertext) : undefined;
                    var field = iAttrs.field != undefined ? $scope.$eval(iAttrs.field) : undefined;
                    var enableHiding = iAttrs.enablehiding != undefined ? $scope.$eval(iAttrs.enablehiding) : undefined;
                    var isClickable = iAttrs.isclickable != undefined ? $scope.$eval(iAttrs.isclickable) : undefined;
                    var onClicked = iAttrs.onclicked != undefined ? $scope.$eval(iAttrs.onclicked) : undefined;
                    var type = iAttrs.type != undefined ? $scope.$eval(iAttrs.type) : undefined;
                    var tag = iAttrs.tag != undefined ? $scope.$eval(iAttrs.tag) : undefined;
                    var onSortChanged = iAttrs.onsortchanged != undefined ? $scope.$eval(iAttrs.onsortchanged) : undefined;

                    dataGridCtrl.addColumn({
                        headerText: headerText,
                        field: field,
                        enableHiding: enableHiding,
                        isClickable: isClickable,
                        onClicked: onClicked,
                        type: type,
                        tag: tag,
                        onSortChanged: onSortChanged,
                        cellTemplate: cellTemplate
                    });
                }
            }
        }
    }
});