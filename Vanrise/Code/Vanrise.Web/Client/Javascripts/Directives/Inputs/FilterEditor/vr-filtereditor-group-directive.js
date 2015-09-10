(function (app) {

    "use strict";

    function vrDirectiveObj(baseDirService) {

        return {
            restrict: 'E',
            require: '^form',
            scope: {
                filters: '=',
                maingroup: '=',
                ondelete:'='
            },
            controller: function () {
                var ctrl = this;

                function onLoad() {
                    ctrl.rules = [];
                    ctrl.groups = [];
                    ctrl.condition = "AND";
                }
                
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

                function deleteGroup() {
                    if (ctrl.ondelete)
                        ctrl.ondelete();
                }

                function addGroup() {
                    var group = { id: baseDirService.guid(), filters: ctrl.filters };
                    group.deleteGroup = function () {
                        var index = ctrl.groups.indexOf(group);
                        ctrl.groups.splice(index, 1);
                    };
                    ctrl.groups.push(group);
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