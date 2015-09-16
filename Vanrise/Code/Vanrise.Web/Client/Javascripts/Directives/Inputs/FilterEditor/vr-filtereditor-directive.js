(function (app) {

    "use strict";

    function vrDirectiveObj(operatorEnum, utilsService) {
        
        return {
            restrict: 'E',
            require: '^form',
            scope: {
                filters: '=',
                result: '=',
                resultstring: '=',
                showresult: '='
            },
            controller: function () {
                var ctrl = this;
                
                function onLoad() {
                    ctrl.resultstring = "";
                }

                function getField(fieldName, operator, valueFrom, valueTo, condition) {
                    if (condition === undefined) condition = " ";
                    if (valueFrom === undefined) valueFrom = " ";
                    
                    operator = utilsService.getEnum(operatorEnum, "value", operator);

                    if (operator) operator = operator.description;

                    if (valueTo === undefined)
                        return " ( " + fieldName + " " + operator + " " + valueFrom + " ) " + condition + " ";
                    else
                        return " ( " + fieldName + " " + operator + " " + valueFrom + " AND " + valueTo + " ) " + condition + " ";
                }

                function getValue(value) {
                    if (value instanceof Date)
                        return value.toISOString().substring(0, 10);
                    return value;
                }

                function filterToString(filter) {
                    
                    var rules = filter.rules;
                    var condition = filter.condition;
                    var result = "";

                    if (rules === undefined || rules === null) return "";
                    var index = 0;
                    var lastItemIndex = Object.keys(rules).length;
                    for (var prop in rules) {
                        if (rules.hasOwnProperty(prop)) {
                            index++;
                            var rule = rules[prop];
                            var valueFrom, valueTo;
                            if (rule.filter.value) {
                                valueFrom = getValue(rule.filter.value[0]);
                                if (rule.filter.value.length === 2)
                                    valueTo = getValue(rule.filter.value[1]);
                            }
                            if (index === lastItemIndex)
                                result += getField(rule.filter.field, rule.filter.operator, valueFrom, valueTo);
                            else
                                result += getField(rule.filter.field, rule.filter.operator, valueFrom, valueTo, condition);
                        }
                    }

                    if (filter.groups !== undefined) {

                        for (var item in filter.groups) {
                            if (filter.groups.hasOwnProperty(item)) {
                                var group = filter.groups[item];
                                if (group.rules !== undefined)
                                    if (result === '')
                                        result +=  " ( " + filterToString(group.rules) + " ) ";
                                    else
                                        result += "  " + condition + " ( " + filterToString(group.rules) + " ) ";
                            }
                        }
                    }
                    return result;
                }

                function getResult() {
                    if (ctrl.result !== undefined)
                        ctrl.resultstring = filterToString(ctrl.result);
                    return ctrl.resultstring;
                }

                angular.extend(this, {
                    getResult: getResult
                });

                onLoad();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function () {
                return "/Client/Javascripts/Directives/Inputs/FilterEditor/vr-filtereditor.html";
            }
        };
    }

    vrDirectiveObj.$inject = ['FilterEditorOperatorEnum', 'UtilsService'];
    app.directive('vrFiltereditor', vrDirectiveObj);

})(app);