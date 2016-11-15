'use strict';

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

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCachingExpirationIntervalsDirectiveReady = function (api) {
                    cachingExpirationIntervalsDirectiveAPI = api;
                    cachingExpirationIntervalsDirectiveReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([cachingExpirationIntervalsDirectiveReadyDeferred.promise]).then(function () {
                        defineAPI();
                    });

            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var cachingExpirationIntervals;

                    if (payload != undefined && payload.data != undefined) {
                        cachingExpirationIntervals = payload.data.CachingExpirationIntervals;
                    }

                    var promises = [];

                    //Loading CachingExpirationIntervals Directive
                    var cachingExpirationIntervalsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    var cachingExpirationIntervalsDirectivePayload = {
                        cachingExpirationIntervals: cachingExpirationIntervals
                    };
                    VRUIUtilsService.callDirectiveLoad(cachingExpirationIntervalsDirectiveAPI, cachingExpirationIntervalsDirectivePayload, cachingExpirationIntervalsDirectiveLoadDeferred);
                    promises.push(cachingExpirationIntervalsDirectiveLoadDeferred.promise);


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.BusinessEntitySettingsData, TOne.WhS.BusinessEntity.Entities",
                        CachingExpirationIntervals: cachingExpirationIntervalsDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);