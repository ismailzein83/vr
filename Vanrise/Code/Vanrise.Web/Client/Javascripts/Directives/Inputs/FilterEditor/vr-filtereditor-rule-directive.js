(function (app) {

    "use strict";

    vrDirectiveObj.$inject = ['FilterEditorInputEnum', 'FilterEditorFieldTypeEnum', 'FilterEditorOperatorEnum', 'FilterEditorInputTypeEnum'];

    function vrDirectiveObj(inputEnum , typeEnum,operatorEnum , inputTypeEnum) {

        return {
            restrict: 'E',
            require: '^form',
            scope: {
                filters: '=',
                deleterule:'='
            },
            controller: function () {
                var ctrl = this;
                

                function onLoad() {
                    ctrl.inputEnum = inputEnum;
                    ctrl.filterInput = {};
                    ctrl.operators = [];
                    ctrl.filterValues = [];
                    ctrl.inputValueFrom = '';
                    ctrl.inputValueTo = '';
                    ctrl.switchValue = '';
                    ctrl.filterInputCount = 0;
                    ctrl.hasMultipleInput = false;
                    ctrl.selectedFilterValues = [];
                    ctrl.selectedFilter = ctrl.filters[0];
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
                }

                function onOperatorSelectionChanged(selectedItem) {
                    changeInput(selectedItem);
                }

                function onValueSelectionChanged() {
                    
                }

                angular.extend(this, {
                    onfilterselectionChanged: onfilterselectionChanged,
                    onOperatorSelectionChanged: onOperatorSelectionChanged,
                    onValueSelectionChanged: onValueSelectionChanged
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

    
    app.directive('vrFiltereditorRule', vrDirectiveObj);

})(app);