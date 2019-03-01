'use strict';

app.directive('vrWhsBeSettingsTechnicalnumberplan', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {

    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            isrequired: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var ctor = new technicalNumberPlanSettingsEditorCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/BETechnicalNumberPlanSettingsTemplate.html"
    };

    function technicalNumberPlanSettingsEditorCtor(ctrl, $scope, $attrs) {

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var technicalNumberPlanSettings;

                if (payload != undefined) {
                    technicalNumberPlanSettings = payload.technicalNumberPlanSettings;
                }

                if (technicalNumberPlanSettings != undefined) {
                    $scope.scopeModel.maxTechnicalZoneCount = technicalNumberPlanSettings.MaxTechnicalZoneCount;
                }
            };

            api.getData = function () {
                return {
                    MaxTechnicalZoneCount: $scope.scopeModel.maxTechnicalZoneCount
                };
            };

            if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }


    }
}]);