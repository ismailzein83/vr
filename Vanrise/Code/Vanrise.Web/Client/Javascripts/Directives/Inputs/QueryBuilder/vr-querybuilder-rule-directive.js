(function(app) {

    "use strict";

    var inputEnum = {
        Text: { value: 0, description: "text" },
        Switch: { value: 2, description: "switch" },
        Select: { value: 4, description: "select" },
        Datetime: { value: 4, description: "datetime" }
    };

    var typeEnum = {
        String: { value: 0, description: "string", input: inputEnum.Text },
        Integer: { value: 1, description: "integer", input: inputEnum.Text },
        Double: { value: 2, description: "double", input: inputEnum.Text },
        Date: { value: 3, description: "date", input: inputEnum.Datetime },
        Time: { value: 4, description: "time", input: inputEnum.Datetime },
        Datetime: { value: 5, description: "datetime", input: inputEnum.Datetime },
        Boolean: { value: 6, description: "boolean", input: inputEnum.Switch },
        Array: { value: 6, description: "boolean", input: inputEnum.Select }
    };

    app.constant('QueryBuilderFilterOperatorEnum', {
        Equal: { value: 0, description: "equal", types: [typeEnum.Array, typeEnum.Boolean, typeEnum.Double] },
        NotEqual: { value: 1, description: "not equal", types: [typeEnum.Array, typeEnum.Double] },
        In: { value: 2, description: "in", types: [typeEnum.Array, typeEnum.Double] },
        NotIn: { value: 3, description: "not in", types: [typeEnum.Array, typeEnum.Double] },
        IsNull: { value: 4, description: "is null", types: [typeEnum.Array, typeEnum.Double] },
        IsNotNull: { value: 5, description: "is not null", types: [typeEnum.Array, typeEnum.Double] },
        Less: { value: 6, description: "less", types: [typeEnum.Double] },
        LessOrEqual: { value: 7, description: "less or equal", types: [typeEnum.Double] },
        Greater: { value: 8, description: "greater", types: [typeEnum.Double] },
        GreaterOrEqual: { value: 9, description: "greater or equal", types: [typeEnum.Double] },
        Between: { value: 10, description: "between", types: [typeEnum.Double], hasMultipleInput: true },
        NotBetween: { value: 11, description: "not between", types: [typeEnum.Double], hasMultipleInput: true }

    });

    app.constant('QueryBuilderFilterTypeEnum', typeEnum);
    app.constant('QueryBuilderFilterInputEnum', inputEnum);
    
})(app);

(function (app) {

    "use strict";

    vrDirectiveObj.$inject = ['QueryBuilderFilterInputEnum', 'QueryBuilderFilterTypeEnum', 'QueryBuilderFilterOperatorEnum'];

    function vrDirectiveObj(inputEnum , typeEnum,operatorEnum) {

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

                function onOperatorSelectionChanged(selectedItem) {
                    if (selectedItem === undefined || selectedItem === null)
                        ctrl.hasMultipleInput = false;
                    else if (selectedItem.hasMultipleInput === undefined)
                        ctrl.hasMultipleInput = false;
                    else
                        ctrl.hasMultipleInput = selectedItem.hasMultipleInput;
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
                return "/Client/Javascripts/Directives/Inputs/QueryBuilder/vr-querybuilder-rule.html";
            }
        };
    }

    
    app.directive('vrQuerybuilderRule', vrDirectiveObj);

})(app);