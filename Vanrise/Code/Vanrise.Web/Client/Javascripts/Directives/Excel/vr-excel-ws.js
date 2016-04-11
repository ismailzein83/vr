'use strict';
app.directive('vrExcelWs', function () {
   
    return {
        restrict: 'E',
        scope: {
            data: '=',
            onReady: '='
        },
        replace: true,
        template: "<div></div>",
        link: function (scope, elem, attrs) {
            $(elem).handsontable({
                 rowHeaders: true,
                 colHeaders: true,
                 outsideClickDeselects: false,
                 colWidths:100,
                 mergeCells: scope.data.MergedCells,
                 height: 320,
                 fillHandle:false
            })
            var data = [];
            var api = $(elem).handsontable('getInstance');
            
            for (var i = 0; i < scope.data.Rows.length ; i++) {
                var row = scope.data.Rows[i];
                data[i] = row.Cells;
                for (var j = 0; j < row.Cells.length; j++) {
                    data[i][j] = row.Cells[j].Value;
                }

            }
            var page = 0;

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
                var scrollTop = $(this).scrollTop();
                var scrollPercentage = 100 * scrollTop / (gridBodyElement.height() - $(this).height());
                if (scrollTop > lastScrollTop) {
                    if (scrollPercentage > 80) {
                        page++;
                        var sliced = getData();
                        if (sliced.length > 0) {
                           
                            var dataold = api.getData();
                            for (var i = 0; i < sliced.length; i++) {
                                dataold[dataold.length] = sliced[i];
                            }
                            api.loadData(dataold);
                          
                        }
                        
                    }
                        

                }
                lastScrollTop = scrollTop;
            });

            if (data.length > 0) {
               //ar alldata = data//.slice(page * 100, (page + 1) * 100)
                var sliced = getData();
                api.loadData(sliced);
            }
                

            if (scope.onReady != undefined && typeof (scope.onReady) == 'function') {
                scope.onReady(api);
            }
        }
    }
})