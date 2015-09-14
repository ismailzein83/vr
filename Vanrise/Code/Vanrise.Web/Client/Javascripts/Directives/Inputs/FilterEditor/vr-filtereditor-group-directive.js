(function (app) {

    "use strict";

    function vrDirectiveObj(baseDirService) {

        return {
            restrict: 'E',
            require: '^form',
            scope: {
                filters: '=',
                maingroup: '=',
                ondelete: '=',
                result: '='
            },
            controller: function () {
                var ctrl = this;

                function setResult() {
                    ctrl.result['rules'] = ctrl.rules;
                    ctrl.result['condition'] = ctrl.condition;
                    ctrl.result['groups'] = ctrl.groups;
                }

                function onLoad() {
                    ctrl.result = {};
                    ctrl.rules = [];
                    ctrl.groups = [];
                    ctrl.condition = "AND";
                    setResult();
                }

                function toggle() {
                    if (ctrl.condition === "OR")
                        ctrl.condition = "AND";
                    else
                        ctrl.condition = "OR";
                    setResult();
                }

                function deleteRule(rule) {
                    var index = ctrl.rules.indexOf(rule);
                    ctrl.rules.splice(index, 1);
                    setResult();
                }

                function addRule() {
                    var rule = { id: baseDirService.guid(),filter:[] };
                    rule.deleteRule = function() { deleteRule(rule); };
                    ctrl.rules.push(rule);
                    setResult();
                }

                

                function deleteGroup() {
                    if (ctrl.ondelete)
                        ctrl.ondelete();
                }

                function addGroup() {
                    var group = { id: baseDirService.guid(), filters: ctrl.filters, rules: ctrl.rules };
                    group.deleteGroup = function () {
                        var index = ctrl.groups.indexOf(group);
                        ctrl.groups.splice(index, 1);
                        setResult();
                    };
                    ctrl.groups.push(group);
                    setResult();
                }

                angular.extend(this, {
                    deleteRule: deleteRule,
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

    vrDirectiveObj.$inject = ['BaseDirService'];
    app.directive('vrFiltereditorGroup', vrDirectiveObj);

})(app);