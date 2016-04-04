'use strict';

app.directive('vrExcelWb', ['ExcelConversion_ExcelAPIService', function (excelAPIService) {
  

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            fileid: '=',
            onReady: '=',
            sheetindex:"="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            
           
            ctrl.scopeModel = {};
            ctrl.scopeModel.datasource = [];
            ctrl.previewExcel = function () {
                ctrl.scopeModel.length = 0;
                ctrl.tabsApi.removeAllTabs();
                ctrl.scopeModel.tabObjects.length = 0;
                excelAPIService.ReadExcelFile(ctrl.fileid).then(function (response) {
                    ctrl.scopeModel.datasource = response;
                  
                });
            }
           
            $scope.$watch('ctrl.fileid', function () {                
                if (ctrl.fileid != null && ctrl.fileid != undefined && ctrl.fileid != 0) {
                    ctrl.previewExcel();
                }
                else {
                    ctrl.scopeModel.datasource.length = 0;
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
        return '<vr-tabs on-ready="onReadyTabs" >'
               + '<vr-tab  ng-repeat="dataItem in ctrl.scopeModel.datasource.Sheets"  header="dataItem.Name" tabobject="ctrl.scopeModel.tabObjects[$index]" >'
               + '<vr-excel-ws data="dataItem" on-ready="onReadyWoorkSheet" ></vr-excel-ws>'
               + '</vr-tab>'
               + '</vr-tabs>';
    }
    function ExcelWoorkBook(ctrl, $scope, attrs) {

        var excelWBAPI;

        ctrl.scopeModel.tabObjects = [];
        
        function initializeController() {
            $scope.onReadyTabs = function (api) {
                ctrl.tabsApi = api;
                defineAPI();
            }
                
        }
        function defineAPI() {

            $scope.onReadyWoorkSheet = function (api) {
                var index = ctrl.scopeModel.tabObjects.length - 1;
                var selectedIndex = ctrl.sheetindex != undefined ? ctrl.sheetindex : 0;
                if (index == selectedIndex)
                    ctrl.scopeModel.tabObjects[index].isSelected = true;

                ctrl.scopeModel.tabObjects[index].api = api;

            }
           
            var api = {};
            api.clearAtIndex = function (i) {
                ctrl.scopeModel.tabObjects[i].api.clear();
            }
            api.getSelectedSheetApi = function () {
                for (var index in ctrl.scopeModel.tabObjects) {
                    if (ctrl.scopeModel.tabObjects[index].isSelected == true) {
                        return ctrl.scopeModel.tabObjects[index].api;
                    }
                }
                return null;
            }
            api.getSelectedSheet = function () {
                for (var index in ctrl.scopeModel.tabObjects) {
                    if (ctrl.scopeModel.tabObjects[index].isSelected == true) {
                        return index ;
                    }
                }
            }
            api.setSelectedSheet = function (index) {
                ctrl.sheetindex = index;
                ctrl.scopeModel.tabObjects[index].isSelected = true;
            }
            api.getAPIsArray = function () {
                return ctrl.tabObjects;
            }
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }


    return directiveDefinitionObject;

}]);