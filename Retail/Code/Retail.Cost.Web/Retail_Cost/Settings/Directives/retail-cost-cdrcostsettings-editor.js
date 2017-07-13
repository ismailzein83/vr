'use strict';

app.directive('retailCostCdrcostsettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CostSettingsEditor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Cost/Settings/Directives/Templates/CDRCostSettingsEditorTemplate.html'
        };

        function CostSettingsEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.data != undefined) {
                        $scope.scopeModel.durationMargin = payload.data.DurationMargin;
                        $scope.scopeModel.attemptDateTimeMargin = payload.data.AttemptDateTimeMargin;
                        $scope.scopeModel.attemptDateTimeOffset = payload.data.AttemptDateTimeOffset;
                        $scope.scopeModel.maxBatchDurationInMinutes = payload.data.MaxBatchDurationInMinutes;
                    }
                };

                api.getData = function () {

                    var data = {
                        $type: 'Retail.Cost.Entities.CDRCostSettingData, Retail.Cost.Entities',
                        DurationMargin: $scope.scopeModel.durationMargin,
                        AttemptDateTimeMargin: $scope.scopeModel.attemptDateTimeMargin,
                        AttemptDateTimeOffset: $scope.scopeModel.attemptDateTimeOffset,
                        MaxBatchDurationInMinutes: $scope.scopeModel.maxBatchDurationInMinutes
                    };

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }
        }
    }]);