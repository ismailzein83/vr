(function (app) {

    'use strict';

    excelconversionListmapping.$inject = ['UtilsService', 'VRUIUtilsService'];

    function excelconversionListmapping(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var listmapping = new Listmapping($scope, ctrl, $attrs);
                listmapping.initializeController();
            },
            controllerAs: "listMappingCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/ExcelConversion/Directives/ListMapping/Templates/ListMappingTemplate.html"
        };

        function Listmapping($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            var listName;
            function initializeController() {
                ctrl.fieldMappings = [];
                ctrl.updateLastRowIndexRange = function () {
                    if (context != undefined) {
                        var range = context.getSelectedCell();
                        if (range != undefined)
                        {
                            ctrl.lastRowIndex = {
                                row: range[0],
                                col: range[1],
                            }
                        }
                        
                    }

                }
                ctrl.selectLastRowIndex = function () {
                    if (context != undefined) {
                        context.setSelectedCell(ctrl.lastRowIndex.row, ctrl.lastRowIndex.col);
                    }
                }
                ctrl.updateFirstRowIndexRange = function () {
                    if (context != undefined) {
                        var range = context.getSelectedCell();
                        if (range != undefined) {
                            ctrl.firstRowIndex = {
                                row: range[0],
                                col: range[1],
                                sheet: context.getSelectedSheet(),
                            }
                        }
                       
                    }

                }
                ctrl.selectFirstRowIndex = function () {
                    if (context != undefined) {
                        context.setSelectedCell(ctrl.firstRowIndex.row, ctrl.firstRowIndex.col);
                    }
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        listName = payload.listName;
                        ctrl.fieldMappings = payload.fieldMappings;
                        for (var i = 0; i < ctrl.fieldMappings.length; i++)
                        {
                            addAPIExtension(ctrl.fieldMappings[i])
                        }
                      
                    }
                    function addAPIExtension(dataItem)
                    {
                        dataItem.onFieldMappingReady = function (api) {
                            dataItem.fieldMappingAPI = api;
                            var payload = {
                                context: context
                            };
                            var setLoader = function (value) {
                                $scope.isLoadingDirective = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, payload, setLoader);
                        }
                        dataItem.normalColNum = ctrl.normalColNum;
                        dataItem.validate = function () {
                            if (dataItem.fieldMappingAPI != undefined) {
                                var obj = dataItem.fieldMappingAPI.getData();
                                if (obj != undefined) {
                                    if (ctrl.firstRowIndex == undefined || obj.RowIndex != ctrl.firstRowIndex.row)
                                        return "Error row index.";
                                }
                            }
                            return null;
                        }
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var fieldMappings ;
                    if(ctrl.fieldMappings.length>0)
                    {
                        fieldMappings = [];
                        for(var i=0;i<ctrl.fieldMappings.length;i++)
                        {
                            var fieldMapping = ctrl.fieldMappings[i];
                            if(fieldMapping.fieldMappingAPI !=undefined)
                                fieldMappings.push(fieldMapping.fieldMappingAPI .getData());
                        }
                       
                    }
                    var data = {
                        ListName:listName,
                        SheetIndex: ctrl.firstRowIndex.sheet,
                        FirstRowIndex: ctrl.firstRowIndex.row,
                        LastRowIndex:ctrl.lastRowIndex.row,
                        FieldMappings: fieldMappings,
                    };
                    return data;
                }
            }
        }
    }

    app.directive('excelconversionListmapping', excelconversionListmapping);

})(app);