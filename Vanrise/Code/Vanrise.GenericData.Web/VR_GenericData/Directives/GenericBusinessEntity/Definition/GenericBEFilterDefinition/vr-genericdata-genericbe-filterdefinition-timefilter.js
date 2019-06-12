(function (app) {

    'use strict';

    GenericBETimeFilterDirective.$inject = ['UtilsService', 'VRNotificationService','VRValidationService'];

    function GenericBETimeFilterDirective(UtilsService, VRNotificationService, VRValidationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TimeFilterCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEFilterDefinition/Templates/TimeFilterDefiitionTemplate.html'
        };

        function TimeFilterCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var context;
            var settings;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.validateDates = function () {
                    return VRValidationService.validateTimeRange($scope.scopeModel.defaultFromDate, $scope.scopeModel.defaultToDate);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        settings = payload.settings;

                        if (settings != undefined) {
                            $scope.scopeModel.defaultFromDate = settings.DefaultFromDate;
                            $scope.scopeModel.defaultToDate = settings.DefaultToDate;
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.TimeFilterDefinitionSettings, Vanrise.GenericData.MainExtensions",
                        DefaultFromDate: $scope.scopeModel.defaultFromDate,
                        DefaultToDate: $scope.scopeModel.defaultToDate
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeFilterdefinitionTimefilter', GenericBETimeFilterDirective);

})(app);