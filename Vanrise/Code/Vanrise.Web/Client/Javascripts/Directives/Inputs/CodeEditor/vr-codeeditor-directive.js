(function (app) {

    "use strict";

    //vrDirectiveObj.$inject = [];

    function vrDirectiveObj() {

        return {
            restrict: 'E',
            //require: '^form',
            scope: {
                value: '='
            },
            controller: function () {

            },
            compile: function () {

                return {
                    pre: function ($scope) {
                        var ctrl = $scope.ctrl;

                        ctrl.editorOptions = {
                            lineWrapping: true,
                            lineNumbers: true,
                            mode: 'text/x-csharp'
                        };
                    }
                }
            },

            controllerAs: 'ctrl',
            bindToController: true,
            template: function () {
                return '<textarea ui-codemirror="ctrl.editorOptions" ng-model="ctrl.value"></textarea>';
            }

        };

    }

    app.directive('vrCodeEditor', vrDirectiveObj);

})(app);



