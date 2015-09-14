(function (app) {

    "use strict";

    function vrDirectiveObj() {
        
        return {
            restrict: 'E',
            require: '^form',
            scope: {
                filters: '=',
                result: '='
            },
            controller: function () {

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