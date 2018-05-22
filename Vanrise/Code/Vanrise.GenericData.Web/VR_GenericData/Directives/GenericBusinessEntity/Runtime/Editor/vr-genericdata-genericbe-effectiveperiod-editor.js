(function (app) {

    'use strict';

    EffectivePeriodEditorDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function EffectivePeriodEditorDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                label: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new EffectivePeriodEditor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,

            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/Editor/Templates/EffectivePeriodEditorTemplate.html"
        };

        function EffectivePeriodEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var effectivePeriodAPI;
            var effectivePeriodDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onEffectivePeriodReady = function (api) {
                    effectivePeriodAPI = api;
                    effectivePeriodDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                        var promises = [];

                        promises.push(loadEffectivePeriodDirective());

                        function loadEffectivePeriodDirective() {
                            var efectivePeriodDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                            effectivePeriodDirectiveReadyDeferred.promise.then(function () {
                                var effectivePeriodPayload;
                                if (payload != undefined) {
                                    effectivePeriodPayload = {
                                        BED: payload.BED,
                                        EED: payload.EED
                                    };
                                }
                                VRUIUtilsService.callDirectiveLoad(effectivePeriodAPI, effectivePeriodPayload, efectivePeriodDirectiveLoadDeferred);
                            });
                            return efectivePeriodDirectiveLoadDeferred.promise;
                        }

                        return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (obj) {
                    var effectivePeriod = effectivePeriodAPI.getData();
                    if (obj != undefined && effectivePeriod != undefined) {
                        obj.BED = effectivePeriod.BED;
                        obj.EED = effectivePeriod.EED;
                    }
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }
    app.directive('vrGenericdataGenericbeEffectiveperiodEditor', EffectivePeriodEditorDirective);
})(app);