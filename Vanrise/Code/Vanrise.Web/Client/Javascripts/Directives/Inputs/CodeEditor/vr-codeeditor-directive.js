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
            controller: function ($scope, $element) {
                var ctrl = this;
                ctrl.readOnly = UtilsService.isContextReadOnly(scope) || attrs.readonly != undefined;

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
                };
            },

            controllerAs: 'ctrl',
            bindToController: true,
            template: function () {
                return '<textarea  ng-readonly="ctrl.readOnly" ui-codemirror="ctrl.editorOptions" ng-model="ctrl.value"></textarea>';
            }

        };

    }

    app.directive('vrCodeEditor', vrDirectiveObj);

})(app);



