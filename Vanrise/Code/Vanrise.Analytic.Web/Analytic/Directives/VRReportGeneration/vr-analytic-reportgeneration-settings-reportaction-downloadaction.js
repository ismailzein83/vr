(function (app) {

    'use strict';

    VrreportgenerationSettingsReportactionDownloadactionDirective.$inject = [];

    function VrreportgenerationSettingsReportactionDownloadactionDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrreportgenerationSettingsReportactionDownloadaction = new VrreportgenerationSettingsReportactionDownloadaction(ctrl, $scope, $attrs);
                vrreportgenerationSettingsReportactionDownloadaction.initializeController();
            },
            controllerAs: 'Ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function VrreportgenerationSettingsReportactionDownloadaction(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var directiveAPI;

            function initializeController() {
                ctrl.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    if (ctrl.onReady != undefined) {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};
                api.load = function (payload) {
                    return directiveAPI.load(payload);
                };

                api.getData = function () {
                    var returnValue;
                    var directiveData = directiveAPI.getData();

                    if (directiveData != undefined) {
                        returnValue = {
                            $type: 'Vanrise.Analytic.MainExtensions.VRReportGeneration.DownloadFileAction,Vanrise.Analytic.MainExtensions',
                            FileGenerator: directiveData
                        };
                    }

                    return returnValue;
                };
                return api;
            }
        }

        function getDirectiveTemplate(attrs) {
            return '<vr-analytic-automatedreport-filegenerator on-ready="Ctrl.onDirectiveReady" normalColNum="2" " ></vr-analytic-automatedreport-filegenerator>';
        }
    }

    app.directive('vrAnalyticReportgenerationSettingsReportactionDownloadaction', VrreportgenerationSettingsReportactionDownloadactionDirective);

})(app);