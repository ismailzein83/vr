(
    function (appControllers) {
        "use strict";

        function testController($scope, UtilsService, VRNotificationService, VRUIUtilsService, VR_Sec_UserAPIService) {
            

            defineScope();
            load()
            function defineScope() {

            }

          
            function load() {
               
            }

           
            var data2 = [];
            for (var i = 0; i < 1000; i++) {
                data2[i] = [];
                for (var j = 0; j < 7; j++) {
                    data2[i][j] = "cell" + i + "// " + j;
                }
            }
            $("#example1").handsontable({
               // data:  JSON.parse(JSON.stringify(data2)),
                rowHeaders: true,
                colHeaders: true,
                //columns: [{ data: 'UserId' }, { data: 'Name' }],
                width: 584,
                height: 320,
                autoRowSize: false,
                autoColSize: false,
            })
            var hot2 = $("#example1").handsontable('getInstance');

            hot2.addHook('afterSelectionEnd', function (a, b, c, d) {

                $scope.startrow = a;
                $scope.startcol = b;
                $scope.endrow = c;
                $scope.endcol = d;
                $scope.$apply();
            });
            //VR_Sec_UserAPIService.GetUsersInfo(null).then(function (response) {
            //    hot2.loadData(response)
            //});

            hot2.loadData(data2);

            $scope.selectCell = function () {
                var a = parseInt($scope.startrow);
                var b = parseInt($scope.startcol);
                var c = parseInt($scope.endrow);
                var d = parseInt($scope.endcol);
                hot2.selectCell(a, b, c, d);
            }

        }

        testController.$inject = [
            '$scope',
            'UtilsService',
            'VRNotificationService',
            'VRUIUtilsService',
            'VR_Sec_UserAPIService'
        ];
        appControllers.controller('TestController', testController);
    }
)(appControllers);