'use strict';

app.directive('vrWhsBeSettingsCachingexpirationintervals', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new cachingExpirationIntervalsEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/BESettingsCachingExpirationIntervalsTemplate.html"
        };

        function cachingExpirationIntervalsEditorCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var cachingExpirationIntervals;

                    if (payload != undefined ) {
                        cachingExpirationIntervals = payload.cachingExpirationIntervals;
                    }

                    if (cachingExpirationIntervals != undefined && cachingExpirationIntervals.TodayEntitiesIntervalInMinutes != undefined)
                        $scope.scopeModel.todayEntitiesIntervalInMinutes = cachingExpirationIntervals.TodayEntitiesIntervalInMinutes;

                    if (cachingExpirationIntervals != undefined && cachingExpirationIntervals.PreviousEntitiesIntervalInMinutes != undefined)
                        $scope.scopeModel.previousEntitiesIntervalInMinutes = cachingExpirationIntervals.PreviousEntitiesIntervalInMinutes;

                };

                api.getData = function () {
                    return {
                        TodayEntitiesIntervalInMinutes: $scope.scopeModel.todayEntitiesIntervalInMinutes,
                        PreviousEntitiesIntervalInMinutes: $scope.scopeModel.previousEntitiesIntervalInMinutes
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);