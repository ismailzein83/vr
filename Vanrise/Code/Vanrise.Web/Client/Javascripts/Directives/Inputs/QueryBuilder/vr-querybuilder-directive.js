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

            },
            compile: function () {

                return {
                    pre: function ($scope) {

                    }
                }
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function () {
                return "/Client/Javascripts/Directives/Inputs/QueryBuilder/vr-querybuilder.html";
            }
        };
    }

    //vrDirectiveObj.$inject = [];
    app.directive('vrQuerybuilder', vrDirectiveObj);

})(app);