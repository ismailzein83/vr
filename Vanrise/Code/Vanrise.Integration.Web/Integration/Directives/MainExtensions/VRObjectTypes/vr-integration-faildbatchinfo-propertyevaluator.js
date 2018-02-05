(function (app) {

    'use strict';

    FaildBatchInfoPropertyEvaluator.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService', 'VR_Integration_FailedBatchInfoFieldsEnum'];

    function FaildBatchInfoPropertyEvaluator(UtilsService, VRUIUtilsService, VRNotificationService, VR_Integration_FailedBatchInfoFieldsEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var splObjectType = new FaildBatchInfoPropertyEvaluatorObject($scope, ctrl, $attrs);
                splObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Integration/Directives/MainExtensions/VRObjectTypes/Templates/FaildBatchInfoPropertyEvaluatorTemplate.html'


        };
        function FaildBatchInfoPropertyEvaluatorObject($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.failedBatchInfoFields = UtilsService.getArrayEnum(VR_Integration_FailedBatchInfoFieldsEnum);

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };


            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.objectPropertyEvaluator != undefined)
                        $scope.scopeModel.selectedFailedBatchInfo = UtilsService.getItemByVal($scope.scopeModel.failedBatchInfoFields, payload.objectPropertyEvaluator.FailedBatchInfoField, "value");

                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Integration.MainExtensions.FailedBatchInfoPropertyEvaluator, Vanrise.Integration.MainExtensions",
                        FailedBatchInfoField: $scope.scopeModel.selectedFailedBatchInfo.value
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrIntegrationFaildbatchinfoPropertyevaluator', FaildBatchInfoPropertyEvaluator);

})(app);