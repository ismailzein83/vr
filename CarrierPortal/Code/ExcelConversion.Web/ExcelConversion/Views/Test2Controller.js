(
    function (appControllers) {
        "use strict";

        function testController($scope, UtilsService, VRNotificationService, VRUIUtilsService, excelAPIService) {

            var WoorkBookApi;
            defineScope();
            load();
            function defineScope() {
                $scope.onReadyWoorkBook = function (api) {
                    WoorkBookApi = api;
                }
                $scope.selectCell = function () {
                    var a = parseInt($scope.row);
                    var b = parseInt($scope.col);
                    if (WoorkBookApi != undefined && WoorkBookApi.getSelectedSheetApi() != undefined)
                        WoorkBookApi.getSelectedSheetApi().selectCell(a, b, a, b);

                }

                $scope.updateRange = function (e) {
                    var range = WoorkBookApi.getSelectedSheetApi().getSelected();
                    $scope.row = range[0];
                    $scope.col = range[1];
                    $scope.sheetindex = 0; // range[1];
                }
                $scope.onFieldMappingReady = function(api)
                {
                    var payload = {
                        context: buildContext()
                    };
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, payload, setLoader);
                }

            }


            function load() {

            }
            function buildContext()
            {
                var context = {
                    getSelectedCell: getSelectedCell,
                    setSelectedCell: setSelectedCell
                }
                return context;
            }
            function setSelectedCell(row, col) {
                var a = parseInt(row);
                var b = parseInt(col);
                if (WoorkBookApi != undefined && WoorkBookApi.getSelectedSheetApi() != undefined)
                    WoorkBookApi.getSelectedSheetApi().selectCell(a, b, a, b);
            }
            function getSelectedCell()
            {
               return WoorkBookApi.getSelectedSheetApi().getSelected();
            }
        }

        testController.$inject = ['$scope','UtilsService','VRNotificationService','VRUIUtilsService','ExcelConversion_ExcelAPIService'];
        appControllers.controller('Test2Controller', testController);
    }
)(appControllers);