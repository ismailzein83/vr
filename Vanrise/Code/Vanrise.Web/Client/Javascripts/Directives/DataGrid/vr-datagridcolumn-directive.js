'use strict';
app.directive('vrDatagridcolumn', ['$parse', 'VR_GridColCSSClassEnum', 'UtilsService','VRLocalizationService', function ($parse, VR_GridColCSSClassEnum, UtilsService, VRLocalizationService) {
    return {
        restrict: 'E',
        require: '^vrDatagrid',
        compile: function (element, attrs) {
            var cellTemplate = element.html().trim();
            element.html('');
            return {
                pre: function ($scope, iElem, iAttrs, dataGridCtrl) {
                    
                    var gridColumnAttribute = iAttrs.columnattributes != undefined ? $scope.$eval(iAttrs.columnattributes) : undefined;

                    var col = {};
                    if (gridColumnAttribute != undefined) {
                        col = {
                            headerText: VRLocalizationService.getResourceValue(gridColumnAttribute.LocalizedHeaderText, gridColumnAttribute.HeaderText),
                            disableSorting: gridColumnAttribute.DisableSorting,
                            headerDescription: VRLocalizationService.getResourceValue(gridColumnAttribute.LocalizedHeaderDescription, gridColumnAttribute.HeaderDescription),
                            field: gridColumnAttribute.Field,
                            isFieldDynamic: gridColumnAttribute.IsFieldDynamic,
                            widthFactor: gridColumnAttribute.WidthFactor,
                            fixedWidth: gridColumnAttribute.FixedWidth,
                            isClickable: gridColumnAttribute.IsClickable,
                            onClicked: gridColumnAttribute.OnClicked,
                            type: gridColumnAttribute.Type,
                            numberPrecision: gridColumnAttribute.NumberPrecision,
                            tag: gridColumnAttribute.Tag,

                        };
                        if (gridColumnAttribute.GridColCSSClassValue != undefined)
                        {
                            var cssClassObject = UtilsService.getEnum(VR_GridColCSSClassEnum, "value", gridColumnAttribute.GridColCSSClassValue);
                            if (cssClassObject != undefined)
                                col.cssclass = cssClassObject.cssClass;
                        }
                        
                    }
                    if (iAttrs.headertext != undefined)
                        col.headerText = $scope.$eval(iAttrs.headertext);
                    if (iAttrs.localizedheadertext != undefined)
                        col.headerText = VRLocalizationService.getResourceValue($scope.$eval(iAttrs.localizedheadertext), col.headerText);
                    if (iAttrs.field != undefined)
                        col.field = $scope.$eval(iAttrs.field);
                    if (iAttrs.disablesorting != undefined)
                        col.disableSorting = $scope.$eval(iAttrs.disablesorting);
                    else if (col.disableSorting == undefined)
                        col.disableSorting = false;
                    if (iAttrs.headerdescription != undefined)
                        col.headerDescription =  $scope.$eval(iAttrs.headerdescription);
                    if (iAttrs.localizedheaderdescription != undefined)
                        col.headerDescription = VRLocalizationService.getResourceValue($scope.$eval(iAttrs.localizedheaderdescription), $scope.$eval(iAttrs.headerdescription));
                    if (iAttrs.isfielddynamic != undefined)
                        col.isFieldDynamic = $scope.$eval(iAttrs.isfielddynamic);
                    if (iAttrs.summaryfield != undefined)
                        col.summaryField = $scope.$eval(iAttrs.summaryfield);
                    if (iAttrs.tooltipfield != undefined)
                        col.tooltipField = $scope.$eval(iAttrs.tooltipfield);
                    if (iAttrs.widthfactor != undefined)
                        col.widthFactor = $scope.$eval(iAttrs.widthfactor);
                    if (iAttrs.enablehiding != undefined)
                        col.enableHiding = $scope.$eval(iAttrs.enablehiding);
                    if (iAttrs.isclickable != undefined)
                        col.isClickable = $scope.$eval(iAttrs.isclickable);
                    if (iAttrs.expendablecolumn != undefined)
                        col.expendableColumn = true;

                    if(iAttrs.invisibleheader != undefined)
                        col.invisibleheader = true;
                   
                    if (iAttrs.expendablecolumntitle != undefined)
                        col.expendableColumnTitle = $scope.$eval(iAttrs.expendablecolumntitle);
                    if (iAttrs.expendablecolumndescription != undefined)
                        col.expendableColumnDescription = $scope.$eval(iAttrs.expendablecolumndescription);
                    if (iAttrs.onclicked != undefined)
                        col.onClicked = $scope.$eval(iAttrs.onclicked);
                    if (iAttrs.type != undefined)
                        col.type = $scope.$eval(iAttrs.type);
                    if (iAttrs.numberprecision != undefined)
                        col.numberPrecision = $scope.$eval(iAttrs.numberprecision);
                    if (iAttrs.tag != undefined)
                        col.tag = $scope.$eval(iAttrs.tag);
                    if (iAttrs.onsortchanged != undefined)
                        col.onSortChanged = $scope.$eval(iAttrs.onsortchanged);
                    //fixedWidth
                    if (iAttrs.fixedwidth != undefined)
                        col.fixedWidth = $scope.$eval(iAttrs.fixedwidth);

                    if (iAttrs.getcolor != undefined)
                        col.getcolor = $scope.$eval(iAttrs.getcolor);
                    col.cellTemplate = cellTemplate;
                    if (iAttrs.cssclass != undefined)
                        col.cssclass = $scope.$eval(iAttrs.cssclass);


                    var columnIndex = iAttrs.columnindex != undefined ? $scope.$eval(iAttrs.columnindex) : undefined;

                    var show = (iAttrs.initiallyhidden != undefined) != undefined ? !$scope.$eval(iAttrs.initiallyhidden) : true;

                    if(iAttrs.ngShow != undefined)
                    show =  $scope.$eval(iAttrs.ngShow);
                   
                    var colDef = dataGridCtrl.addColumn(col, columnIndex);

                    var headertextWatch;
                    var ngshowWatch;

                    if (col.headerText != undefined && (iAttrs.localizedheadertext == undefined || !VRLocalizationService.isLocalizationEnabled())) {
                        headertextWatch = $scope.$watch(iAttrs.headertext, function (val) {
                            if (colDef != undefined && val != undefined)
                                dataGridCtrl.updateColumnHeader(colDef, val);
                        });
                        if (col.headerText !=undefined &&  col.headerText.toUpperCase() === 'ID') {
                            dataGridCtrl.hideColumn(colDef);
                        }
                    }

                    if (!show)
                        dataGridCtrl.hideColumn(colDef);

                    iElem.bind("$destroy", function () {
                        removeColumnFromGridIfExists();
                    });
                    $scope.$on("$destroy", function () {
                        removeColumnFromGridIfExists();
                        headertextWatch != undefined && headertextWatch();
                        ngshowWatch != undefined && ngshowWatch();
                    });

                    function removeColumnFromGridIfExists() {
                        if (colDef != undefined) {
                            dataGridCtrl.removeColumn(colDef);
                            colDef = undefined;
                        }
                    }

                    iAttrs.$observe('ngShow', function (expr) {
                        ngshowWatch = $scope.$watch(function () {
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
            };
        }
    };
}]);