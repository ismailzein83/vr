(function (app) {

    "use strict";

    function vrDirectiveObj() {
        
        return {
            restrict: 'E',
            require: '^form',
            scope: {
                filters: '=',
                result: '=',
                resultString:'='
            },
            controller: function () {
                var ctrl = this;
                ctrl.resultString = "";

                function getField(fieldName, operator, valueFrom, valueTo, condition) {
                    if (condition === undefined) condition = " ";
                    if (valueFrom === undefined) valueFrom = " ";

                    if (valueTo === undefined)
                        return " ( " + fieldName + " " + operator + " " + valueFrom + " ) " + condition + " ";
                    else
                        return " ( " + fieldName + " " + operator + " " + valueFrom + " AND " + valueTo + " ) " + condition + " ";
                }

                function filterToString(filter) {
                    var rules = filter.rules;
                    var condition = filter.condition;
                    var result = "";

                    if (rules === undefined || rules === null) return "";

                    rules.forEach(function (rule, index) {
                        var valueFrom, valueTo;
                        if (rule.filter.value) {

                            if (rule.filter.value.length === 1)
                                valueFrom = rule.filter.value[0];
                            else {
                                valueFrom = rule.filter.value[0];
                                valueTo = rule.filter.value[1];
                            }
                        }

                        if (rule.filter.field !== undefined && rule.filter.field.field !== undefined && rule.filter.operator !== undefined) {

                            if (index === rules.length - 1)
                                result += getField(rule.filter.field.field, rule.filter.operator.description, valueFrom, valueTo);
                            else
                                result += getField(rule.filter.field.field, rule.filter.operator.description, valueFrom, valueTo, condition);
                        }

                    });

                    if (filter.group !== undefined) {
                        filter.group.forEach(function (group) {
                            if (group !== undefined && group !== null && group.rules !== undefined)
                                result += "  " + condition + " ( " + filterToString(group.rules) + " ) ";
                        });
                    }
                    return result;
                }

                function getResult() {
                    if (ctrl.result !== undefined)
                        ctrl.resultString = filterToString(ctrl.result);
                    return ctrl.resultString;
                }

                angular.extend(this, {
                    getResult: getResult
                });
                
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function () {
                return "/Client/Javascripts/Directives/Inputs/FilterEditor/vr-filtereditor.html";
            }
        };
    }

    //vrDirectiveObj.$inject = [];
    app.directive('vrFiltereditor', vrDirectiveObj);

})(app);