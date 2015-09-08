(function (app) {

    "use strict";

    function vrDirectiveObj() {

        return {
            restrict: 'E',
            require: '^form',
            scope: {
                filters: '='
            },
            controller: function () {
                this.deleteRule1 = function() {
                    console.log('deleteRule1');
                };
                this.deleteRule2 = function () {
                    console.log('deleteRule2');
                };
            },
            compile: function () {

                return {
                    pre: function ($scope) {
                        var ctrl = $scope.ctrl;

                    }
                }
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function () {
                return "/Client/Javascripts/Directives/Inputs/QueryBuilder/vr-querybuilder-group.html";
            }
        };
    }

    //vrDirectiveObj.$inject = [];
    app.directive('vrQuerybuilderGroup', vrDirectiveObj);

})(app);