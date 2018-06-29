"use strict";

app.directive("vrAnalyticAutomatedreportserialnumberSequence", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Analytic_AutomatedReport_DateCounterTypeEnum",
function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Analytic_AutomatedReport_DateCounterTypeEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope:
        {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var ctor = new SequenceCtor($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Handler/Templates/AutomatedReportSequenceSerialNumberPartTemplate.html"
    };

    function SequenceCtor($scope, ctrl, $attrs) {

        var context;

        this.initializeController = initializeController;
        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.dateCounterTypes = UtilsService.getArrayEnum(VR_Analytic_AutomatedReport_DateCounterTypeEnum);
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return {
                    $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers.VRAutomatedReportSequenceSerialNumberPart, Vanrise.Analytic.MainExtensions",
                    DateCounterType: $scope.scopeModel.selectedDateCounterType != undefined ? $scope.scopeModel.selectedDateCounterType.value : null,
                    PaddingLeft: $scope.scopeModel.paddingLeft,
                };
            };

            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    context = payload.context;
                    if (payload.concatenatedPartSettings != undefined) {
                        $scope.scopeModel.selectedDateCounterType = UtilsService.getItemByVal($scope.scopeModel.dateCounterTypes, payload.concatenatedPartSettings.DateCounterType, 'value');
                        $scope.scopeModel.paddingLeft = payload.concatenatedPartSettings.PaddingLeft;
                    }
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }

    }

    return directiveDefinitionObject;

}
]);