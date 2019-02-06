(function (app) {

    'use strict';

    WidgetSettingsRuntime.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Analytic_AnalyticConfigurationAPIService', 'VRCommon_VRTimePeriodAPIService','VRDateTimeService'];

    function WidgetSettingsRuntime(UtilsService, VRUIUtilsService, VR_Analytic_AnalyticConfigurationAPIService,VRCommon_VRTimePeriodAPIService, VRDateTimeService) {
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


            var definitionSettings;
            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();
            var templateConfigs = [];
            var from;
            var to;
          

            function initializeController() {
                $scope.scopeModel = {};
              

                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    var promises = [];
                    VR_Analytic_AnalyticConfigurationAPIService.GetWidgetsTemplateConfigs().then(function (response) {
                      
                        if (response != null) {

                            for (var i = 0; i < response.length; i++) {
                                templateConfigs.push(response[i]);
                            }
                        }
                        if (payload != undefined) {
                            definitionSettings = payload.definitionSettings;
                            var matchItem = UtilsService.getItemByVal(templateConfigs, definitionSettings.Settings.ConfigId, "ExtensionConfigurationId");
                          
                            if (matchItem == null)
                                return;
                        
                            $scope.scopeModel.runtimeEditor = matchItem.RuntimeEditor;
                            if ($scope.scopeModel.runtimeEditor != undefined) {
                                $scope.scopeModel.onDirectiveReady = function (api) {
                                    directiveAPI = api;
                                    directiveReadyDeferred.resolve();
                                };
                            }
                            var timePeriodInput = {
                                TimePeriod: definitionSettings.TimePeriod,
                                EffectiveDate: VRDateTimeService.getTodayDate()
                            };
                            var getPeriodRangePromise = VRCommon_VRTimePeriodAPIService.GetTimePeriod(timePeriodInput).then(function (response) {
                                if (response != undefined) {
                                    from = response.From;
                                    to = response.To
                                }
                                
                            });
                        }
                        getPeriodRangePromise.then(function () {
                            promises.push(loadDirective());
                            return UtilsService.waitMultiplePromises(promises).then(function () {
                                $scope.scopeModel.isLoading = false;
                            });
                        });
                       
                    });

                };

                api.getData = function () {

                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
          
            function loadDirective() {
                $scope.scopeModel.isLoading = true;
                var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                directiveReadyDeferred.promise.then(function () {
                    var directivePayload = {
                        Settings: definitionSettings.Settings,
                        TableId: definitionSettings.AnalyticTableId,
                        FromTime: from,
                        FilterGroup: definitionSettings.RecordFilter,
                        ToTime: to,
                    };
                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadPromiseDeferred);
                });
           
                return directiveLoadPromiseDeferred.promise;
            }
        }
    }

    app.directive('vrCommonWidgetsettingsRuntime', WidgetSettingsRuntime);

})(app);