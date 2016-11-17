'use strict';
app.directive('vrBiSummaryTemplate', ['UtilsService', '$compile', 'VRNotificationService', 'VRUIUtilsService', 'VR_BI_BIConfigurationAPIService',
function (UtilsService, $compile, VRNotificationService, VRUIUtilsService, VR_BI_BIConfigurationAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new biChart(ctrl, $scope);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/BI/Directives/Summary/Templates/BISummaryTemplate.html"

    };

    function biChart(ctrl, $scope) {
        var measureDirectiveAPI;
        var measureReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var timeEntityDirectiveAPI;
        var timeEntityReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        function initializeController() {
            $scope.onMeasureDirectiveReady = function (api) {
                measureDirectiveAPI = api;
                measureReadyPromiseDeferred.resolve();
            };

            $scope.onTimeEntityDirectiveReady = function (api) {
                timeEntityDirectiveAPI = api;
                timeEntityReadyPromiseDeferred.resolve();
            };

            $scope.selectedMeasureTypes = [];
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var measureTypes;
                if ($scope.selectedMeasureTypes.length > 0) {
                    measureTypes = [];
                    for (var i = 0; i < $scope.selectedMeasureTypes.length; i++) {

                        measureTypes.push($scope.selectedMeasureTypes[i].Name);
                    }
                }
                return {
                    $type: "Vanrise.BI.Entities.SummaryDirectiveSetting, Vanrise.BI.Entities",
                    MeasureTypes: measureTypes,
                    TimeEntity: ctrl.selectedTimeEntity != undefined ? ctrl.selectedTimeEntity.Name : undefined
                };
            };

            api.load = function (payload) {
                var promises = [];
                var loadMeasurePromiseDeferred = UtilsService.createPromiseDeferred();
                measureReadyPromiseDeferred.promise.then(function () {
                    var measurePayload = { selectedIds: payload != undefined ? payload.MeasureTypes : undefined };

                    VRUIUtilsService.callDirectiveLoad(measureDirectiveAPI, measurePayload, loadMeasurePromiseDeferred);

                });

                promises.push(loadMeasurePromiseDeferred.promise);

                var loadTimeEntityPromiseDeferred = UtilsService.createPromiseDeferred();
                timeEntityReadyPromiseDeferred.promise.then(function () {
                    var timeEntityPayload = { selectedIds: payload != undefined ? payload.TimeEntity : undefined };

                    VRUIUtilsService.callDirectiveLoad(timeEntityDirectiveAPI, timeEntityPayload, loadTimeEntityPromiseDeferred);

                });
                promises.push(loadTimeEntityPromiseDeferred.promise);

                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);