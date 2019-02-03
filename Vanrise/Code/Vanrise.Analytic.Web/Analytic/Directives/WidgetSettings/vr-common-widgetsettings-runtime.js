(function (app) {

    'use strict';

    WidgetSettingsRuntime.$inject = ['VR_Analytic_AnalyticConfigurationAPIService', 'VR_Analytic_AnalyticTypeEnum', 'UtilsService', 'VRUIUtilsService', 'ColumnWidthEnum', 'VR_Analytic_AnalyticItemConfigAPIService'];

    function WidgetSettingsRuntime(VR_Analytic_AnalyticConfigurationAPIService, VR_Analytic_AnalyticTypeEnum, UtilsService, VRUIUtilsService, ColumnWidthEnum, VR_Analytic_AnalyticItemConfigAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                type: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var widgets = new Widgets($scope, ctrl, $attrs);
                widgets.initializeController();
            },
            controllerAs: "searchSettingsCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/WidgetSettings/Templates/WidgetSettingsRuntimeTemplate.html"
        };

        function Widgets($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

        

            var directiveAPI;
            var directiveReadyDeferred;
           

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

              
                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directiveReadyDeferred.resolve();
                };


                defineAPI();
            }
            function defineAPI() {
                var api = {};


                api.load = function (payload) {
                    var definitionSettings;
                    console.log(payload);
                    var promises = [];

                    if (payload != undefined) {
                        console.log(payload);
                        definitionSettings = payload.definitionSettings;

                        promises.push(loadDirective());

                    }
                    return UtilsService.waitMultiplePromises(promises);

                     


                };

                api.getData = function () {
                   
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function loadDirective() {
                //var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                //directiveReadyDeferred.promise.then(function () {
                //    var directivePayload;
                //    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadPromiseDeferred);
                //})
                //return directiveLoadPromiseDeferred.promise;
            }
            
        }
    }

    app.directive('vrCommonWidgetsettingsRuntime', WidgetSettingsRuntime);

})(app);