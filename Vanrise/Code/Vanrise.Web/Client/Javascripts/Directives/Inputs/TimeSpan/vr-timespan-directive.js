(function (app) {

    "use strict";

    vrTimespan.$inject = ['UtilsService'];

    function vrTimespan(UtilsService) {

        return {
            restrict: 'E',
            scope: {
                value: '=',
                label: '@',
                isrequired: '=',
                hidelabel: '@',
                dontacceptzero: '@',
                dontacceptnegative: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                ctrl.validateTimeOffset = function (value) {
                    var validateTimeOffset = UtilsService.validateTimeOffset(value);
                    if (validateTimeOffset != null)
                        return validateTimeOffset;

                    if ($attrs.dontacceptzero != undefined) {
                        if (value == "00.00:00:00" || value == "0.00:00:00" || value == "00:00:00")
                            return "Time Offset should different than zero";
                    }

                    if ($attrs.dontacceptnegative != undefined) {
                        if (value != undefined && value[0] == "-")
                            return "Time Offset should be positive";
                    }

                    return null;
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

                var label = ' label="{{ctrl.label}}" ';
                if (attrs.hidelabel != undefined)
                    label = '';

                return '<vr-textbox ' + label + ' value="ctrl.value" ' + isrequired + '  customvalidate="ctrl.validateTimeOffset(ctrl.value)"></vr-textbox>';
            }
        };
    }

    app.directive('vrTimespan', vrTimespan);
})(app);



