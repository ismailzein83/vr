app.directive('vrDatagridcolumn', ['$parse', function ($parse) {
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
                    var widthFactor = iAttrs.widthfactor != undefined ? $scope.$eval(iAttrs.widthfactor) : undefined;
                    var enableHiding = iAttrs.enablehiding != undefined ? $scope.$eval(iAttrs.enablehiding) : undefined;
                    var isClickable = iAttrs.isclickable != undefined ? $scope.$eval(iAttrs.isclickable) : undefined;
                    var onClicked = iAttrs.onclicked != undefined ? $scope.$eval(iAttrs.onclicked) : undefined;
                    var type = iAttrs.type != undefined ? $scope.$eval(iAttrs.type) : undefined;
                    var tag = iAttrs.tag != undefined ? $scope.$eval(iAttrs.tag) : undefined;
                    var onSortChanged = iAttrs.onsortchanged != undefined ? $scope.$eval(iAttrs.onsortchanged) : undefined;
                    var columnIndex = iAttrs.columnindex != undefined ? $scope.$eval(iAttrs.columnindex) : undefined;
                    var col = {
                        headerText: headerText,
                        field: field,
                        widthFactor: widthFactor,
                        enableHiding: enableHiding,
                        isClickable: isClickable,
                        onClicked: onClicked,
                        type: type,
                        tag: tag,
                        onSortChanged: onSortChanged,
                        cellTemplate: cellTemplate
                    };

                    var show = iAttrs.ngShow != undefined ? $scope.$eval(iAttrs.ngShow) : true;

                    var colDef;
                    if (show)
                        colDef = dataGridCtrl.addColumn(col, columnIndex);

                    iElem.bind("$destroy", function () {
                        if (colDef != undefined) {
                            dataGridCtrl.removeColumn(colDef);
                            colDef = undefined;
                        }
                    });

                    iAttrs.$observe('ngShow', function (expr) {
                        $scope.$watch(function () {
                            return $parse(expr)($scope);
                        }, function (value) {
                            if (value == false) {
                                if (colDef != undefined) {
                                    dataGridCtrl.removeColumn(colDef);
                                    colDef = undefined;
                                }
                            }
                            else if (value == true)
                            {
                                if (colDef == undefined)
                                    colDef = dataGridCtrl.addColumn(col, columnIndex);
                            }
                        })
                    });
                }
            }
        }
    }
}]);