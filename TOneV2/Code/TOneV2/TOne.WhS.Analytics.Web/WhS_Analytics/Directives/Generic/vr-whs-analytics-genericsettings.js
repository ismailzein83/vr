"use strict";

app.directive("vrWhsAnalyticsGenericsettings", ['WhS_Analytics_GenericAnalyticMeasureEnum',
function (WhS_Analytics_GenericAnalyticMeasureEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var genericGridSettings = new GenericGridSettings($scope, ctrl, $attrs);
            genericGridSettings.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: function () {
            return "/Client/Modules/WhS_Analytics/Directives/Generic/Templates/GenericSettingsTemplate.html";
        }
    };
    function GenericGridSettings($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        function initializeController() {
            ctrl.measureThresholds = [];

            defineAPI();

        }

        function defineAPI() {
            var api = {};

            api.getData = function () {

                var selectedThresholds = {};

                for (var i = 0; i < ctrl.measureThresholds.length; i++) {
                    if (ctrl.measureThresholds[i].value == WhS_Analytics_GenericAnalyticMeasureEnum.ASR.value)
                        selectedThresholds.asr = ctrl.measureThresholds[i].enteredValue;

                    if (ctrl.measureThresholds[i].value == WhS_Analytics_GenericAnalyticMeasureEnum.ACD.value)
                        selectedThresholds.acd = ctrl.measureThresholds[i].enteredValue;

                    if (ctrl.measureThresholds[i].value == WhS_Analytics_GenericAnalyticMeasureEnum.Attempts.value)
                        selectedThresholds.attempts = ctrl.measureThresholds[i].enteredValue;
                }

                return selectedThresholds;
            };

            api.load = function (payload) {

                if (payload != undefined) {

                    if (payload.measureThresholds != undefined) {
                        for (var i = 0; i < payload.measureThresholds.length; i++) {
                            for (var p in WhS_Analytics_GenericAnalyticMeasureEnum) {
                                if (WhS_Analytics_GenericAnalyticMeasureEnum[p].value == payload.measureThresholds[i])
                                    ctrl.measureThresholds.push(WhS_Analytics_GenericAnalyticMeasureEnum[p]);
                            }
                        }
                    }
                }

            };

            if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }
    }
    return directiveDefinitionObject;

}]);