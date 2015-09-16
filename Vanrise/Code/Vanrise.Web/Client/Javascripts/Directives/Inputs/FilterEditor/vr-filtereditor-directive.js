﻿(function (app) {

    "use strict";

    function vrDirectiveObj(operatorEnum, utilsService) {
        
        return {
            restrict: 'E',
            require: '^form',
            scope: {
                filters: '=',
                result: '=',
                resultString: '=',
                showresult: '='
            },
            controller: function () {
                var ctrl = this;
                
                function onLoad() {
                    ctrl.resultString = "";
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

                                if (rule.filter.value.length === 1)
                                    valueFrom = rule.filter.value[0];
                                else {
                                    valueFrom = rule.filter.value[0];
                                    valueTo = rule.filter.value[1];
                                }
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
                                    result += "  " + condition + " ( " + filterToString(group.rules) + " ) ";
                            }
                        }
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