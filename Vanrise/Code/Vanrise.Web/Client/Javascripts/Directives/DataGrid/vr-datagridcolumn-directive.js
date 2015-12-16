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
                    var disableSorting = iAttrs.disablesorting != undefined ? $scope.$eval(iAttrs.disablesorting) : false;

                    var headerDescription = iAttrs.headerdescription != undefined ? $scope.$eval(iAttrs.headerdescription) : undefined;
                    var field = iAttrs.field != undefined ? $scope.$eval(iAttrs.field) : undefined;
                    var isFieldDynamic = iAttrs.isfielddynamic != undefined ? $scope.$eval(iAttrs.isfielddynamic) : undefined;
                    var summaryField = iAttrs.summaryfield != undefined ? $scope.$eval(iAttrs.summaryfield) : undefined;
                    var tooltipField = iAttrs.tooltipfield != undefined ? $scope.$eval(iAttrs.tooltipfield) : undefined;
                    var widthFactor = iAttrs.widthfactor != undefined ? $scope.$eval(iAttrs.widthfactor) : undefined;
                    var enableHiding = iAttrs.enablehiding != undefined ? $scope.$eval(iAttrs.enablehiding) : undefined;
                    var isClickable = iAttrs.isclickable != undefined ? $scope.$eval(iAttrs.isclickable) : undefined;
                    var onClicked = iAttrs.onclicked != undefined ? $scope.$eval(iAttrs.onclicked) : undefined;
                    var type = iAttrs.type != undefined ? $scope.$eval(iAttrs.type) : undefined;
                    var numberPrecision = iAttrs.numberprecision != undefined ? $scope.$eval(iAttrs.numberprecision) : undefined;
                    var tag = iAttrs.tag != undefined ? $scope.$eval(iAttrs.tag) : undefined;
                    var onSortChanged = iAttrs.onsortchanged != undefined ? $scope.$eval(iAttrs.onsortchanged) : undefined;
                    var columnIndex = iAttrs.columnindex != undefined ? $scope.$eval(iAttrs.columnindex) : undefined;
                    var getcolor = iAttrs.getcolor != undefined ? $scope.$eval(iAttrs.getcolor) : undefined;
                    var col = {
                        headerText: headerText,
                        disableSorting:disableSorting,
                        headerDescription:headerDescription,
                        field: field,
                        isFieldDynamic: isFieldDynamic,
                        summaryField: summaryField,
                        tooltipField: tooltipField,
                        widthFactor: widthFactor,
                        enableHiding: enableHiding,
                        isClickable: isClickable,
                        onClicked: onClicked,
                        type: type,
                        numberPrecision: numberPrecision,
                        tag: tag,
                        onSortChanged: onSortChanged,
                        cellTemplate: cellTemplate,
                        getcolor: getcolor
                    };

                    var show = iAttrs.ngShow != undefined ? $scope.$eval(iAttrs.ngShow) : true;

                    var colDef = dataGridCtrl.addColumn(col, columnIndex);
                    if (iAttrs.headertext != undefined)
                        $scope.$watch(iAttrs.headertext, function (val) {                            
                            if (colDef != undefined && val != undefined)
                                dataGridCtrl.updateColumnHeader(colDef, val);
                        });
                    if (!show)
                        dataGridCtrl.hideColumn(colDef);

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
                            if (colDef != undefined) {
                                if (value) 
                                    dataGridCtrl.showColumn(colDef);
                                else
                                    dataGridCtrl.hideColumn(colDef);
                                    
                            }                            
                        })
                    });
                }
            }
        }
    }
}]);