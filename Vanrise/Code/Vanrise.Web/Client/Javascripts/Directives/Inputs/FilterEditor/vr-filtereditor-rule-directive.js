﻿(function (app) {

    "use strict";

    function vrDirectiveObj(inputEnum, typeEnum, operatorEnum, inputTypeEnum, utilsService) {

        return {
            restrict: 'E',
            require: '^form',
            scope: {
                filters: '=',
                deleterule: '=',
                outputfilter:'='
            },
            controller: function ($scope) {
                var ctrl = this;
                $scope.$on("$destroy", function () {
                    watch();
                });
                
                function getField(filter) {
                    if (filter) {
                        return filter.field;
                    }
                    return undefined;
                }

                function getOperator(operator) {
                    if (operator) {
                        return operator.value;
                    }
                    return undefined;
                }

                function setOutput() {
                    ctrl.outputfilter = {
                        field: getField(ctrl.selectedFilter),
                        operator: getOperator(ctrl.selectedOperator)
                    };

                    if ((ctrl.filterInput === ctrl.inputEnum.Select) && (ctrl.hasInput)) {
                        ctrl.outputfilter.value = [ctrl.selectedFilterValues];
                    }
                    else if ((ctrl.filterInput === ctrl.inputEnum.Datetime) && (ctrl.hasMultipleInput) && (ctrl.hasInput)) {
                        ctrl.outputfilter.value = [ctrl.fromDate, ctrl.toDate];
                    }
                    else if ((ctrl.filterInput === ctrl.inputEnum.Datetime) && (ctrl.hasInput)) {
                        ctrl.outputfilter.value = [ctrl.fromDate];
                    }
                    else if ((ctrl.filterInput === ctrl.inputEnum.Text) && (ctrl.hasMultipleInput) && (ctrl.hasInput)) {
                        ctrl.outputfilter.value = [ctrl.inputValueFrom, ctrl.inputValueTo];
                    }
                    else if ((ctrl.filterInput === ctrl.inputEnum.Text) && (ctrl.hasInput)) {
                        ctrl.outputfilter.value = [ctrl.inputValueFrom];
                    }
                    else if ((ctrl.filterInput === ctrl.inputEnum.Switch) && (ctrl.hasInput)) {
                        ctrl.outputfilter.value = [ctrl.switchValue];
                    }
                }

                var watch = function () {

                    $scope.$watch('ctrl.fromDate', function (newValue) {
                        ctrl.fromDate = newValue;
                        setOutput();
                    });

                    $scope.$watch('ctrl.toDate', function (newValue) {
                        ctrl.toDate = newValue;
                        setOutput();
                    });


                    $scope.$watch('ctrl.switchValue', function (newValue) {
                        ctrl.switchValue = newValue;
                        setOutput();
                    });

                    $scope.$watch('ctrl.inputValueFrom', function (newValue) {
                        ctrl.inputValueFrom = newValue;
                        setOutput();
                    });

                    $scope.$watch('ctrl.inputValueTo', function (newValue) {
                        ctrl.inputValueTo = newValue;
                        setOutput();
                    });
                };

                function onLoad() {
                    ctrl.inputEnum = inputEnum;
                    ctrl.filterInput = {};
                    ctrl.operators = [];
                    ctrl.filterValues = [];
                    ctrl.inputValueFrom = '';
                    ctrl.inputValueTo = '';
                    ctrl.switchValue = false;
                    ctrl.filterInputCount = 0;
                    ctrl.hasMultipleInput = false;
                    ctrl.selectedFilterValues = [];
                    ctrl.selectedFilter = ctrl.filters[0];
                    setOutput();
                    watch();
                }
                
                function getSelectOperator(selectedtype) {
                    var result = [];
                    for (var item in operatorEnum) {
                        if (operatorEnum.hasOwnProperty(item)) {
                            var types = operatorEnum[item].types;
                            for (var i = 0, len = types.length; i < len; i++) {
                                if (selectedtype.value === types[i].value) {
                                    result.push(operatorEnum[item]);
                                    break;
                                }
                            }
                        }
                    }
                    return result;
                }

                function onfilterselectionChanged(selectedItem) {
                    ctrl.operators = getSelectOperator(selectedItem.type);
                    ctrl.selectedOperator = ctrl.operators[0];
                    ctrl.filterInput = selectedItem.type.input;
                    ctrl.filterValues = selectedItem.values;
                    setOutput();
                }

                function changeInput(operatorItem) {
                    ctrl.hasInput = true;
                    ctrl.hasMultipleInput = false;

                    if (operatorItem === undefined || operatorItem === null || operatorItem.InputType === undefined) {
                        ctrl.hasInput = true;
                        ctrl.hasMultipleInput = false;
                        return;
                    }

                    if (operatorItem.InputType === inputTypeEnum.NoInput) {
                        ctrl.hasInput = false;
                        ctrl.hasMultipleInput = false;
                        return;
                    }

                    if (operatorItem.InputType === inputTypeEnum.MultipleInput) {
                        ctrl.hasInput = true;
                        ctrl.hasMultipleInput = true;
                        return;
                    }
                    setOutput();
                }

                function onOperatorSelectionChanged(selectedItem) {
                    changeInput(selectedItem);
                    setOutput();
                }

                function onValueSelectionChanged() {
                    setOutput();
                }

                function customvalidateDate(toDate) {
                    return utilsService.validateDates(ctrl.fromDate, toDate);
                }

                angular.extend(this, {
                    onfilterselectionChanged: onfilterselectionChanged,
                    onOperatorSelectionChanged: onOperatorSelectionChanged,
                    onValueSelectionChanged: onValueSelectionChanged,
                    customvalidateDate: customvalidateDate
                });

                onLoad();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function () {
                return "/Client/Javascripts/Directives/Inputs/FilterEditor/vr-filtereditor-rule.html";
            }
        };
    }

    vrDirectiveObj.$inject = ['FilterEditorInputEnum', 'FilterEditorFieldTypeEnum', 'FilterEditorOperatorEnum', 'FilterEditorInputTypeEnum', 'UtilsService'];

    app.directive('vrFiltereditorRule', vrDirectiveObj);

})(app);