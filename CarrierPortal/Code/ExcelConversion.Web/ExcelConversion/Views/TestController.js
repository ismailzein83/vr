(
    function (appControllers) {
        "use strict";

        function testController($scope, UtilsService, VRNotificationService, VRUIUtilsService, excelAPIService) {
            
            var WoorkBookApi;
            defineScope();
            load()
            function defineScope() {
                $scope.onReadyWoorkBook = function (api) {
                    WoorkBookApi = api;
                }
                $scope.ClearFirst =  function () {
                    WoorkBookApi.clearAtIndex(0);
                }
            }

          
            function load() {
               
            }

           
        //    var data2 = [];
        //    for (var i = 0; i < 1000; i++) {
        //        data2[i] = [];
        //        for (var j = 0; j < 7; j++) {
        //            data2[i][j] = "cell" + i + "// " + j;
        //        }
        //    }
        //    $("#example1").handsontable({
        //       // data:  JSON.parse(JSON.stringify(data2)),
        //        rowHeaders: true,
        //        colHeaders: true,
        //        //columns: [{ data: 'Value' }],
        //        width: 584,
        //        height: 320,
        //        autoRowSize: false,
        //        autoColSize: false,
        //    })
        //    var hot2 = $("#example1").handsontable('getInstance');

        //    hot2.addHook('afterSelectionEnd', function (a, b, c, d) {

        //        $scope.startrow = a;
        //        $scope.startcol = b;
        //        $scope.endrow = c;
        //        $scope.endcol = d;
        //        $scope.$apply();
        //    });
        //    excelAPIService.ReadExcelFile(341).then(function (response) {
        //        var data = [];
        //        for (var i = 0; i < response.Sheets[0].Rows.length; i++) {
        //            var row = response.Sheets[0].Rows[i];
        //            data[i] = row.Cells;
        //            for (var j = 0; j < row.Cells.length; j++) {
        //                data[i][j] = row.Cells[j].Value
        //            }
                   
        //        }
        //        hot2.loadData(data);
        //    });


        //    $scope.selectCell = function () {
        //        var a = parseInt($scope.startrow);
        //        var b = parseInt($scope.startcol);
        //        var c = parseInt($scope.endrow);
        //        var d = parseInt($scope.endcol);
        //        hot2.selectCell(a, b, c, d);
        //    }

        }

        testController.$inject = [
            '$scope',
            'UtilsService',
            'VRNotificationService',
            'VRUIUtilsService',
            'ExcelConversion_ExcelAPIService'
        ];
        appControllers.controller('TestController', testController);
    }
)(appControllers);