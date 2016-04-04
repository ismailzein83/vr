'use strict';

app.directive('vrExcelWb', ['ExcelConversion_ExcelAPIService', function (excelAPIService) {
  

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            fileid: '=',
            onReady:'='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.datasource = [];
            var sheetsArray = [];
            ctrl.previewExcel = function () {
                excelAPIService.ReadExcelFile(ctrl.fileid).then(function (response) {
                    ctrl.datasource = response;
                  
                });
            }
          
            $scope.$watch('ctrl.fileid', function () {
                if (ctrl.fileid != null && ctrl.fileid != undefined && ctrl.fileid != 0) {
                    ctrl.previewExcel();
                }
                else {

                    ctrl.datasource.length = 0;
                }
            });

            var excelWoorkBook = new ExcelWoorkBook(ctrl, $scope, $attrs);
            excelWoorkBook.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        template: function (element, attrs) {
            return getWorkBookTemplate(attrs);
        }

    };

    function getWorkBookTemplate(attrs) {        
        return '<vr-tabs>'
               + '<vr-tab ng-repeat="dataItem in ctrl.datasource.Sheets"  header="dataItem.Name" >'
               + '<vr-excel-ws data="dataItem" on-ready="onReadyWoorkSheet" ></vr-excel-ws>'
               + '</vr-tab>'
               + '</vr-tabs>';
    }
    function ExcelWoorkBook(ctrl, $scope, attrs) {

        var excelWBAPI;
        var wbsApis = [];
        $scope.onReadyWoorkSheet = function (api) {
            wbsApis[wbsApis.length] = api;

        }
        function initializeController() {          
                defineAPI();
        }
        function defineAPI() {
            var api = {};
            api.clearAtIndex = function (i) {
                wbsApis[i].clear();
            }
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }


    return directiveDefinitionObject;

}]);