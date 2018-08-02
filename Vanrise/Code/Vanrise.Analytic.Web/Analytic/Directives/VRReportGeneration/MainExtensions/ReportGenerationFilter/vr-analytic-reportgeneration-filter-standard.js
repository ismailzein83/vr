(function (app) {

    'use strict';

    VrreportgenerationFilterStandardDirective.$inject = [];

    function VrreportgenerationFilterStandardDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrreportgenerationFilterStandard = new VrreportgenerationFilterStandard(ctrl, $scope, $attrs);
                vrreportgenerationFilterStandard.initializeController();
            },
            controllerAs: 'Ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };
        function VrreportgenerationFilterStandard(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {

                if (ctrl.onReady != undefined) {
                    ctrl.onReady(getDirectiveAPI());
                }

                function getDirectiveAPI() {
                    var api = {};
                    api.load = function (payload) {

                    };

                    api.getData = function () {
                        var returnValue = {
                            $type: 'Vanrise.Analytic.MainExtensions.StandardReportGenerationFilter,Vanrise.Analytic.MainExtensions'
                        };
                        return returnValue;
                    };
                    return api;

                }
            }
        }

        function getDirectiveTemplate(attrs) {
            return '';
        }
    }

    app.directive('vrAnalyticReportgenerationFilterStandard', VrreportgenerationFilterStandardDirective);

})(app);