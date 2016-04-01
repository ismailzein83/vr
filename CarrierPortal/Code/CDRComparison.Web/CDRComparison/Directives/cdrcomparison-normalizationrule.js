(function (app) {

    'use strict';

    NormalizationRuleDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function NormalizationRuleDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var normalizationRule = new NormalizationRule($scope, ctrl, $attrs);
                normalizationRule.initializeController();
            },
            controllerAs: "normalizationCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/CDRComparison/Directives/Templates/NormalizationRuleTemplate.html"
        };

        function NormalizationRule($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var settingsDirectiveAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSettingsDirectiveReady = function (api) {
                    settingsDirectiveAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var settings;

                    if (payload != undefined) {
                        $scope.scopeModel.fieldToNormalize = payload.FieldToNormalize;
                        settings = payload.NormalizationSettings;
                    }

                    var loadSettingsDirectivePromise = loadSettingsDirective();
                    promises.push(loadSettingsDirectivePromise);

                    return UtilsService.waitMultiplePromises(promises);

                    function loadSettingsDirective() {
                        var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                        var payload = {
                            settings: settings,
                            isNotRequired: (ctrl.isrequired == undefined || ctrl.isrequired == false)
                        };
                        VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, payload, settingsDirectiveLoadDeferred);
                        return settingsDirectiveLoadDeferred.promise;
                    }
                };

                api.getData = function () {
                    var data = {
                        FieldToNormalize: $scope.scopeModel.fieldToNormalize,
                        CriteriaFields: null,
                        NormalizationSettings: settingsDirectiveAPI.getData()
                    };
                    return (data.NormalizationSettings != undefined) ? data : undefined;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('cdrcomparisonNormalizationrule', NormalizationRuleDirective);

})(app);