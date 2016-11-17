(function (app) {

    'use strict';

    fieldmappingCellfieldmappingDirective.$inject = ["VRCommon_TextManipulationService"];

    function fieldmappingCellfieldmappingDirective(VRCommon_TextManipulationService) {
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
            controllerAs: "cellfieldmappingCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var label = "";
            if (attrs.label != undefined)
                label = "label='"+attrs.label+"'";
            return ' <vr-columns colnum="{{cellfieldmappingCtrl.normalColNum * 1.5}}" >'
                          + ' <vr-cellviewer ' + label + ' on-select="cellfieldmappingCtrl.selectCell" on-update="cellfieldmappingCtrl.updateRange" value="cellObject" customvalidate="cellfieldmappingCtrl.validate()" isrequired="cellfieldmappingCtrl.isrequired" type="cellfieldmappingCtrl.type"> </vr-cellviewer>'
                           + '</vr-columns>'
                 + ' <vr-columns colnum="{{cellfieldmappingCtrl.normalColNum *0.5}}" >'
                 + ' <span class="glyphicon  glyphicon-edit" ng-click="cellfieldmappingCtrl.editCellManipulation()" ng-if="cellfieldmappingCtrl.showEditButton" style="font-size:15px;cursor:pointer"></span>'
                 + '</vr-columns>'
                 + ' <vr-columns colnum="{{cellfieldmappingCtrl.normalColNum *0.5}}" >'
                + ' <span class="glyphicon glyphicon-check" ng-click="cellfieldmappingCtrl.editCellManipulation()" ng-if="cellfieldmappingCtrl.showCheckButton" style="color:green;font-size:15px;cursor:pointer"></span>'
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
                    if ($scope.cellObject != undefined && context != undefined) {
                        var row = context.getFirstRowIndex();
                        if (row != undefined && $scope.cellObject != undefined && $scope.cellObject.sheet != row.sheet)
                            return "Error sheet index.";
                        if (row == undefined || $scope.cellObject.row != row.row)
                            return "Error row index.";

                    }
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
                            if (textManipulationSettings != undefined)
                            {
                                ctrl.showEditButton = false;
                                ctrl.showCheckButton = true;
                            }
                        }
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
                            $type: "Vanrise.ExcelConversion.MainExtensions.FieldMappings.CellFieldMapping, Vanrise.ExcelConversion.MainExtensions",
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

    app.directive('vrExcelconversionFieldmappingCellfieldmapping', fieldmappingCellfieldmappingDirective);

})(app);