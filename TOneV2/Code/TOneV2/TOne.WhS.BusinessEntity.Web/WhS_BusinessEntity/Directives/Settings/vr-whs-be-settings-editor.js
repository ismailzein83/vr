﻿'use strict';

app.directive('vrWhsBeSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new beSettingsEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/BESettingsTemplate.html"
        };

        function beSettingsEditorCtor(ctrl, $scope, $attrs) {

            var cachingExpirationIntervalsDirectiveAPI;
            var cachingExpirationIntervalsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var technicalNumberPlanSettingsDirectiveAPI;
            var technicalNumberPlanSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCachingExpirationIntervalsDirectiveReady = function (api) {
                    cachingExpirationIntervalsDirectiveAPI = api;
                    cachingExpirationIntervalsDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onTechnicalNumberPlanSettingsDirectiveReady = function (api) {
                    technicalNumberPlanSettingsDirectiveAPI = api;
                    technicalNumberPlanSettingsDirectiveReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([cachingExpirationIntervalsDirectiveReadyDeferred.promise, technicalNumberPlanSettingsDirectiveReadyDeferred.promise]).then(function () {
                        defineAPI();
                    });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var cachingExpirationIntervals;
                    var technicalNumberPlanSettings;

                    if (payload != undefined && payload.data != undefined) {
                        cachingExpirationIntervals = payload.data.CachingExpirationIntervals;
                        technicalNumberPlanSettings = payload.data.TechnicalNumberPlanSettings;
                    }

                    var promises = [];

                    var cachingExpirationIntervalsDirectiveLoadPromise = loadCachingExpirationIntervals();
                    promises.push(cachingExpirationIntervalsDirectiveLoadPromise);

                    var technicalNumberPlanSettingsDirectiveLoadPromise = loadTechnicalNumberPlanSettings();
                    promises.push(technicalNumberPlanSettingsDirectiveLoadPromise);

                    function loadCachingExpirationIntervals() {
                        var cachingExpirationIntervalsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                        var cachingExpirationIntervalsDirectivePayload = {
                            cachingExpirationIntervals: cachingExpirationIntervals
                        };
                        VRUIUtilsService.callDirectiveLoad(cachingExpirationIntervalsDirectiveAPI, cachingExpirationIntervalsDirectivePayload, cachingExpirationIntervalsDirectiveLoadDeferred);

                        return cachingExpirationIntervalsDirectiveLoadDeferred.promise;
                    }

                    function loadTechnicalNumberPlanSettings() {
                        var technicalNumberPlanSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                        var technicalNumberPlanSettingsDirectivePayload = {
                            technicalNumberPlanSettings: technicalNumberPlanSettings
                        };
                        VRUIUtilsService.callDirectiveLoad(technicalNumberPlanSettingsDirectiveAPI, technicalNumberPlanSettingsDirectivePayload, technicalNumberPlanSettingsDirectiveLoadDeferred);

                        return technicalNumberPlanSettingsDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.BusinessEntitySettingsData, TOne.WhS.BusinessEntity.Entities",
                        CachingExpirationIntervals: cachingExpirationIntervalsDirectiveAPI.getData(),
                        TechnicalNumberPlanSettings: technicalNumberPlanSettingsDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);