(function (app) {

    'use strict';

    fieldmappingFixedCellDirective.$inject = ["VRCommon_TextManipulationService"];

    function fieldmappingFixedCellDirective(VRCommon_TextManipulationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                type: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var cellfieldmapping = new CellFieldMapping($scope, ctrl, $attrs);
                cellfieldmapping.initializeController();
            },
            controllerAs: "fixedCellmappingCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var label = "";
            if (attrs.label != undefined)
                label = "label='" + attrs.label + "'";
            return ' <vr-columns colnum="{{fixedCellmappingCtrl.normalColNum * 1.5}}" >'
                          + ' <vr-cellviewer ' + label + ' on-select="fixedCellmappingCtrl.selectCell" on-update="fixedCellmappingCtrl.updateRange" value="cellObject" customvalidate="fixedCellmappingCtrl.validate()" isrequired="fixedCellmappingCtrl.isrequired" type="fixedCellmappingCtrl.type"> </vr-cellviewer>'
                           + '</vr-columns>'
                 + ' <vr-columns colnum="{{fixedCellmappingCtrl.normalColNum *0.5}}" >'
                 + ' <span class="glyphicon  glyphicon-edit" ng-click="fixedCellmappingCtrl.editCellManipulation()" ng-if="fixedCellmappingCtrl.showEditButton" style="font-size:15px;cursor:pointer"></span>'
                 + '</vr-columns>'
                 + ' <vr-columns colnum="{{fixedCellmappingCtrl.normalColNum *0.5}}" >'
                + ' <span class="glyphicon glyphicon-check" ng-click="fixedCellmappingCtrl.editCellManipulation()" ng-if="fixedCellmappingCtrl.showCheckButton" style="color:green;font-size:15px;cursor:pointer"></span>'
                 + '</vr-columns>';
        }

        function CellFieldMapping($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            var textManipulationSettings;
            function initializeController() {
                ctrl.updateRange = function () {
                   
                    if (context != undefined) {
                        var range = context.getSelectedCell();
                        if (range != undefined) {
                            $scope.cellObject = {
                                row: range[0],
                                col: range[1],
                                sheet: context.getSelectedSheet(),
                            };
                        }

                    }

                };
                ctrl.validate = function () {
                    /*if ($scope.cellObject != undefined && context != undefined) {
                        var row = context.getFirstRowIndex();
                        if (row != undefined && $scope.cellObject != undefined && $scope.cellObject.sheet != row.sheet)
                            return "Error sheet index.";

                    }*/
                    return null;
                };
                ctrl.selectCell = function () {
                    if (context != undefined && $scope.cellObject != undefined) {
                        context.setSelectedCell($scope.cellObject.row, $scope.cellObject.col, $scope.cellObject.sheet);
                    }
                };

                ctrl.showEditButton = true;
                ctrl.showCheckButton = false;

                ctrl.editCellManipulation = function () {
                    var onTextManipulationSave = function (textManipulationObj) {
                        if (textManipulationObj != undefined && textManipulationObj.textManipulationSettings != undefined) {
                            textManipulationSettings = textManipulationObj.textManipulationSettings;
                            ctrl.showEditButton = false;
                            ctrl.showCheckButton = true;

                        }
                        else {
                            ctrl.showEditButton = true;
                            ctrl.showCheckButton = false;
                            textManipulationSettings = undefined;
                        }
                    };
                    VRCommon_TextManipulationService.editTextManipulation(onTextManipulationSave, textManipulationSettings);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.fieldMapping != undefined) {
                            $scope.cellObject = {
                                row: payload.fieldMapping.RowIndex,
                                col: payload.fieldMapping.CellIndex,
                                sheet: payload.fieldMapping.SheetIndex
                            };
                            textManipulationSettings = payload.fieldMapping.ManipulationSettings;
                            if (textManipulationSettings != undefined) {
                                ctrl.showEditButton = false;
                                ctrl.showCheckButton = true;
                            }
                        }
                        if (payload.showEditButton != undefined)
                            ctrl.showEditButton = payload.showEditButton;
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data;
                    if ($scope.cellObject != undefined) {
                        data = {
                            $type: "Vanrise.ExcelConversion.MainExtensions.FixedCellFieldMapping, Vanrise.ExcelConversion.MainExtensions",
                            SheetIndex: $scope.cellObject.sheet,
                            RowIndex: $scope.cellObject.row,
                            CellIndex: $scope.cellObject.col,
                            ManipulationSettings: textManipulationSettings
                        };
                    }
                    return data;
                }
            }
        }
    }

    app.directive('vrExcelconversionFieldmappingFixedcell', fieldmappingFixedCellDirective);

})(app);