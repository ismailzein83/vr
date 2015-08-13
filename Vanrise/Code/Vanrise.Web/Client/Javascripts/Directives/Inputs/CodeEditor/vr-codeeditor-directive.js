(function (app) {

    "use strict";

    vrDirectiveObj.$inject = ['BaseDirService'];

    function vrDirectiveObj(BaseDirService) {

        return {
            restrict: 'E',
            //require: '^form',
            scope: {
                value: '='
            },
            controller: function ($scope, $element) {

            },
            compile: function (element, attrs) {

                return {
                    pre: function ($scope, iElem, iAttrs, formCtrl) {
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
            template: function (element, attrs) {
                return '<textarea ui-codemirror="ctrl.editorOptions" ng-model="ctrl.value"></textarea>'
            }

        };

    }

    app.directive('vrCodeEditor', vrDirectiveObj);

})(app);



