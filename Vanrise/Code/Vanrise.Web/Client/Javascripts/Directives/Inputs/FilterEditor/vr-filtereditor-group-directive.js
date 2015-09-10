(function (app) {

    "use strict";

    function vrDirectiveObj(baseDirService) {

        return {
            restrict: 'E',
            require: '^form',
            scope: {
                filters: '='
            },
            controller: function () {
                var ctrl = this;

                ctrl.rules = [];
                ctrl.condition = "AND";

                function toggle() {
                    if (ctrl.condition === "OR")
                        ctrl.condition = "AND";
                    else
                        ctrl.condition = "OR";
                }
                function deleteRule(rule) {
                    var index = ctrl.rules.indexOf(rule);
                    ctrl.rules.splice(index, 1);
                }

                function addRule() {
                    var rule = { id: baseDirService.guid() };
                    rule.deleteRule = function() { deleteRule(rule); };
                    ctrl.rules.push(rule);
                }

                function addGroup() {

                }

                function deleteGroup() {
                    
                }

                angular.extend(this, {
                    deleteRule: deleteRule,
                    addRule: addRule,
                    addGroup: addGroup,
                    deleteGroup: deleteGroup,
                    toggle: toggle
                });

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