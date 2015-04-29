app.directive('vrDatagridcolumn', function () {
    return {
        restrict: 'E',
        require: '^vrDatagrid',
        link: function (scope, elem, attrs, dataGridCtrl) {
            var headerText = attrs.headertext != undefined ? scope.$eval(attrs.headertext) : undefined;
            var field = attrs.field != undefined ? scope.$eval(attrs.field) : undefined;
            var enableHiding = attrs.enablehiding != undefined ? scope.$eval(attrs.enablehiding) : undefined;
            var isClickable = attrs.isclickable != undefined ? scope.$eval(attrs.isclickable) : undefined;
            var onClicked = attrs.onclicked != undefined ? scope.$eval(attrs.onclicked) : undefined;
            var type = attrs.type != undefined ? scope.$eval(attrs.type) : undefined;
            var tag = attrs.tag != undefined ? scope.$eval(attrs.tag) : undefined;
            var onSortChanged = attrs.onsortchanged != undefined ? scope.$eval(attrs.onsortchanged) : undefined;

            dataGridCtrl.addColumn({
                headerText: headerText,
                field: field,
                enableHiding: enableHiding,
                isClickable: isClickable,
                onClicked: onClicked,
                type: type,
                tag: tag,
                onSortChanged: onSortChanged,
                cellTemplate: elem.context.innerHTML.trim()
            });
        }
    }
});