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
                 height: 320
            })
            var data = [];
            var api = $(elem).handsontable('getInstance');
            
            for (var i = 0; i < scope.data.Rows.length; i++) {
                var row = scope.data.Rows[i];
                data[i] = row.Cells;
                for (var j = 0; j < row.Cells.length; j++) {
                    data[i][j] = row.Cells[j].Value;
                }

            }

            

            if (data.length > 0)
                api.loadData(data);
            if (scope.onReady != undefined && typeof (scope.onReady) == 'function') {
                scope.onReady(api);
            }
        }
    }
})