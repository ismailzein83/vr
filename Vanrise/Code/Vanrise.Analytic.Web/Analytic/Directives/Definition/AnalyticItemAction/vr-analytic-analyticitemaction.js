(function (app) {

    'use strict';

    AnalyticItemActionDirective.$inject = ['VR_Analytic_AnalyticItemActionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function AnalyticItemActionDirective(VR_Analytic_AnalyticItemActionAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                label: '@',
                customvalidate: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var analyticitemaction = new Analyticitemaction($scope, ctrl, $attrs);
                analyticitemaction.initializeController();
            },
            controllerAs: "analytiItemActionCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Action'";
            if (attrs.hidelabel != undefined) {
                label = "Actions";
                withemptyline = '';
            }


            var template =
                '<vr-row>'
                   + '<vr-columns colnum="{{analytiItemActionCtrl.normalColNum * 2}}">'
                     + '<vr-textbox isrequired="true" value="actionTitle" label="Title"></vr-textbox>'
                + '</vr-columns>'
                + '<vr-common-vrlocalizationtextresource-selector on-ready="onLocalizationTextResourceSelectorReady" normal-col-num="{{analytiItemActionCtrl.normalColNum* 2}}"></vr-common-vrlocalizationtextresource-selector>'
                   + '<vr-columns colnum="{{analytiItemActionCtrl.normalColNum * 2}}">'
                     + ' <vr-select on-ready="onSelectorReady" datasource="templateConfigs" selectedvalues="selectedTemplateConfig" datavaluefield="ExtensionConfigurationId" datatextfield="Title"'
                     + label + ' isrequired="analytiItemActionCtrl.isrequired" hideremoveicon></vr-select>'
               + '</vr-row>'
            + '<vr-row>'
               + '<vr-directivewrapper directive="selectedTemplateConfig.Editor" on-ready="onDirectiveReady" normal-col-num="{{analytiItemActionCtrl.normalColNum}}" isrequired="analytiItemActionCtrl.isrequired" customvalidate="analytiItemActionCtrl.customvalidate"></vr-directivewrapper>'
             + '</vr-row>';
            return template;

        }
        function Analyticitemaction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;
            var action;
            var localizationTextResourceSelectorAPI;
            var localizationTextResourceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.templateConfigs = [];
                $scope.selectedTemplateConfig;

                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.onLocalizationTextResourceSelectorReady = function (api) {
                    localizationTextResourceSelectorAPI = api;
                    localizationTextResourceSelectorReadyPromiseDeferred.resolve();
                };

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directivePayload = undefined;
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var titleResourceKey;
                    if (payload != undefined) {
                        action = payload.itemAction;
                        if (action != undefined) {
                            $scope.actionTitle = action.Title;
                            titleResourceKey = action.TitleResourceKey;
                        }
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();
                        var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var payloadDirective = action;

                            VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                        });
                        promises.push(loadDirectivePromiseDeferred.promise);

                    }

                    var getAnalyticItemActionsTemplateConfigsPromise = getAnalyticItemActionsTemplateConfigs();
                    promises.push(getAnalyticItemActionsTemplateConfigsPromise);
                    var loadLocalizationTextResourceSelectorPromise = loadLocalizationTextResourceSelector();
                    promises.push(loadLocalizationTextResourceSelectorPromise);
                    return UtilsService.waitMultiplePromises(promises);

                    function getAnalyticItemActionsTemplateConfigs() {
                        return VR_Analytic_AnalyticItemActionAPIService.GetAnalyticItemActionsTemplateConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.templateConfigs.push(response[i]);
                                }
                                if (action != undefined)
                                    $scope.selectedTemplateConfig = UtilsService.getItemByVal($scope.templateConfigs, action.ConfigId, 'ExtensionConfigurationId');
                                else
                                    $scope.selectedTemplateConfig = $scope.templateConfigs[0];
                            }
                        });
                    }

                    function loadLocalizationTextResourceSelector() {
                        var localizationTextResourceSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        var localizationTextResourcePayload = { selectedValue: titleResourceKey };

                        localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, localizationTextResourcePayload, localizationTextResourceSelectorLoadPromiseDeferred);
                        });
                        return localizationTextResourceSelectorLoadPromiseDeferred.promise;
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data;
                    if ($scope.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.selectedTemplateConfig.ExtensionConfigurationId;
                            data.Title = $scope.actionTitle;
                            data.TitleResourceKey = localizationTextResourceSelectorAPI != undefined ? localizationTextResourceSelectorAPI.getSelectedValues() : undefined;
                        }
                    }
                    return data;
                }
            }
        }
    }

    app.directive('vrAnalyticAnalyticitemaction', AnalyticItemActionDirective);

})(app);