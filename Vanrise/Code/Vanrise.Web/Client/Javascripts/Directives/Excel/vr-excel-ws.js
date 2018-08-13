'use strict';
app.directive('vrExcelWs', ["VR_ExcelConversion_ExcelAPIService", function (VR_ExcelConversion_ExcelAPIService) {

    return {
        restrict: 'E',
        scope: {
            data: '=',
            onReady: '=',
            fileid: "=",
            index: "="
        },
        replace: true,
        template: "<div></div>",
        link: function (scope, elem, attrs) {
            var data = [];


            $(elem).handsontable({
                rowHeaders: true,
                colHeaders: true,
                outsideClickDeselects: false,
                readOnly: attrs.allowedit == undefined,
                colWidths: 100,
                mergeCells: scope.data.MergedCells,
                height: 320,
                fillHandle: false


            });

            var api = $(elem).handsontable('getInstance');
            scope.$parent[loaderkey] = false;

            for (var i = 0; i < scope.data.Rows.length ; i++) {
                var row = scope.data.Rows[i];
                data[i] = row.Cells;
                if (scope.data.NumberOfColumns > 0 && row.Cells.length > 0) {
                    for (var j = 0; j < row.Cells.length; j++) {
                        data[i][j] = row.Cells[j].Value;
                    }
                }                    
                else
                    for (var j = 0; j < 20 ; j++) {
                        data[i][j] = "";
                    }

            }

            var pagesize = 100;
            var lastpage = scope.data.Rows.length < 100;
            function getPageSize() {
                return 100;
            }
            var loaderkey = "isloadingdatatab" + scope.index;
            function getPageInfo(startFromBeginning) {
                var currentdatalength = api.getData().length;
                var fromRow = startFromBeginning ? 1 : currentdatalength;

                return {
                    fromRow: fromRow,
                    toRow: fromRow + getPageSize()
                };
            }
            function fetchDataPageFromServer() {
                scope.$parent[loaderkey] = true;
                var info = getPageInfo(false);
                var query = {
                    FileId: scope.fileid,
                    From: info.fromRow,
                    To: info.toRow,
                    SheetIndex: scope.index
                };
                return VR_ExcelConversion_ExcelAPIService.ReadExcelFilePage(query).then(function (response) {
                    return response;

                }).finally(function () {
                    scope.$parent[loaderkey] = false;
                });
            }

            function getData() {
                var alldata = data;
                var items = alldata.slice(page * 100, (page + 1) * 100);
                var res = [];
                for (var i = 0 ; i < items.length; i++) {
                    res.push(items[i]);
                }
                return res;
            }
            var lastScrollTop;
            var gridBodyElement = $(elem.find(".wtHider"));
            $(elem).find('.wtHolder').on('scroll', function () {
                if (scope.$parent[loaderkey] || lastpage || api.getData().length > scope.data.MaxDataRow)
                    return;
                var scrollTop = $(this).scrollTop();
                var scrollPercentage = 100 * scrollTop / (gridBodyElement.height() - $(this).height());
                if (scrollTop > lastScrollTop) {
                    if (scrollPercentage > 80) {
                        fetchDataPageFromServer().then(function (response) {
                            var newdata = [];
                            lastpage = response.Rows.length < 100;
                            for (var i = 0; i < response.Rows.length ; i++) {
                                var row = response.Rows[i];
                                newdata[i] = row.Cells;
                                for (var j = 0; j < row.Cells.length; j++) {
                                    newdata[i][j] = row.Cells[j].Value;
                                }

                            }
                            if (newdata.length > 0) {
                                var mergeddata = api.getData();
                                mergeddata = mergeddata.concat(newdata);

                                api.loadData(mergeddata);
                            }

                        });
                    }
                }

                lastScrollTop = scrollTop;

            });
            if (data.length > 0) {
                api.loadData(data);

            }
            else {
                api.loadData(buildFakeData());
            }



            function buildFakeData() {
                var fakedata = [];
                for (var i = 0; i < 20 ; i++) {
                    fakedata[i] = [];
                    for (var j = 0; j < 20; j++) {
                        fakedata[i][j] = "";
                    }

                }
                return fakedata;
            }
            var Menu = null;
            if (attrs.enbalerowinsert != undefined) {
                if (Menu == null)
                    Menu = {};
                Menu["row_above"] = {
                    disabled: function () {
                        //if first row, disable this option
                        return api.getData().length < scope.data.MaxDataRow;
                    }
                };
                Menu["row_below"] = {
                    disabled: function () {
                        //if first row, disable this option
                        return api.getData().length < scope.data.MaxDataRow;
                    }
                };
                /*Menu["remove_row"] = true;*/
            }

            if (attrs.enbalecolinsert != undefined) {
                if (Menu == null)
                    Menu = {};
                Menu["col_left"] = true;
                Menu["col_right"] = true;

            }
            if (Menu != null)
                api.updateSettings({
                    contextMenu: {
                        items: Menu
                    }
                });
            var inter;
            scope.i = 0;
            api.reLoadRefresh = function () {
                scope.i = 0;
                var inter = setInterval(function () {
                    scope.i++;
                    if (scope.i === 1000) {
                        clearInterval(inter);
                        api.isrendered = true;
                    }
                    if (scope.i <= 1000) {
                        api.render();
                    }

                }, 1);
            };
            if (scope.onReady != undefined && typeof (scope.onReady) == 'function') {
                scope.onReady(api);
            }


        }
    };
}]);