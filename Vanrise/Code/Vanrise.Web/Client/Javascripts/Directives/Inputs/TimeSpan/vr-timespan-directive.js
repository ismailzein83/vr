(function (app) {

    "use strict";

    vrTimespan.$inject = ['UtilsService'];

    function vrTimespan(UtilsService) {

        return {
            restrict: 'E',
            scope: {
                value: '=',
                label: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                $scope.scopeModel = {};

                $scope.scopeModel.validateTimeOffset = function (value) {
                    return UtilsService.validateTimeOffset(value);
                };
            },
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs) {
                        var ctrl = $scope.ctrl;
                    }
                };
            },

            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                var isrequired = '';

                if (attrs.isrequired == '')
                    isrequired = 'isrequired="true"';
                else if (attrs.isrequired != undefined)
                    isrequired = 'isrequired="ctrl.isrequired"';
                else if (attrs.isrequired == undefined)
                    isrequired = 'isrequired="false"';


                var timeSpan =
                       '<vr-textbox label="{{ctrl.label}}" value="ctrl.value" ' + isrequired + '  customvalidate="scopeModel.validateTimeOffset(ctrl.value)"></vr-textbox>';

                return timeSpan;
            }

        };

    }

    app.directive('vrTimespan', vrTimespan);

})(app);



