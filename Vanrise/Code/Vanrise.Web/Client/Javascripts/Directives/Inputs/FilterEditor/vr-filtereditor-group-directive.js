(function (app) {

    "use strict";

    function vrDirectiveObj() {

        return {
            restrict: 'E',
            require: '^form',
            scope: {
                filters: '=',
                maingroup: '=',
                ondelete: '=',
                result: '='
            },
            controller: function ($scope) {
                var ctrl = this;
                var count = 0;
                
                function onLoad() {
                    $scope.scopeFilters = ctrl.filters;
                    ctrl.result = {};
                    ctrl.rules = {};
                    ctrl.groups = {};
                    ctrl.condition = "AND";
                    ctrl.result['rules'] = ctrl.rules;
                    ctrl.result['condition'] = ctrl.condition;
                    ctrl.result['groups'] = ctrl.groups;
                }

                function toggle() {
                    if (ctrl.condition === "OR")
                        ctrl.condition = "AND";
                    else
                        ctrl.condition = "OR";
                }

                function addRule() {
                    var key = count++;
                    var rule = { filter: [] };
                    rule.deleteRule = function () { delete ctrl.rules[key]; };
                    ctrl.rules[key] = rule;
                }

                function deleteGroup() {
                    if (ctrl.ondelete)
                        ctrl.ondelete();
                }

                function addGroup() {
                    var key = count++;
                    var group = { rules: ctrl.rules };
                    group.deleteGroup = function () { delete ctrl.groups[key];};
                    ctrl.groups[key] = group;
                }

                angular.extend(this, {
                    addRule: addRule,
                    addGroup: addGroup,
                    deleteGroup: deleteGroup,
                    toggle: toggle
                });

                onLoad();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function () {
                return "/Client/Javascripts/Directives/Inputs/FilterEditor/vr-filtereditor-group.html";
            }
        };
    }

    app.directive('vrFiltereditorGroup', vrDirectiveObj);

})(app);