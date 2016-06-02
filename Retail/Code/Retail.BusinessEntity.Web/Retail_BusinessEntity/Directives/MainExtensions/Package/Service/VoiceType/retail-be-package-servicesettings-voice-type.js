(function (app) {

    'use strict';

    BePackageServicesettingsVoiceTypeDirective.$inject = ['Retail_BE_PackageAPIService', 'UtilsService', 'VRUIUtilsService'];

    function BePackageServicesettingsVoiceTypeDirective(Retail_BE_PackageAPIService, UtilsService, VRUIUtilsService) {
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
                var bePackageServicesettingsVoiceType = new BePackageServicesettingsVoiceType($scope, ctrl, $attrs);
                bePackageServicesettingsVoiceType.initializeController();
            },
            controllerAs: "voiceTypeCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Voice Type'";
            if (attrs.hidelabel != undefined) {
                label = "";
                withemptyline = '';
            }
            var template =
        
                  '<vr-columns colnum="{{voiceTypeCtrl.normalColNum}}">'
                 + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                 + ' datasource="scopeModel.templateConfigs"'
                 + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                 + 'datavaluefield="ExtensionConfigurationId"'
                 + ' datatextfield="Title"'
                 + label
                 + ' isrequired="true"'
                 + 'hideremoveicon>'
                 + '</vr-select>'
                 + ' </vr-columns>'
              + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig !=undefined" directive="scopeModel.selectedTemplateConfig.Editor" on-ready="scopeModel.onDirectiveReady" normal-col-num="{{voiceTypeCtrl.normalColNum}}" isrequired="voiceTypeCtrl.isrequired" customvalidate="voiceTypeCtrl.customvalidate" type="voiceTypeCtrl.type"></vr-directivewrapper>' ;
            return template;

        }
        function BePackageServicesettingsVoiceType($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;


            var directivePayload;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };

                defineAPI();

            }

            function defineAPI() {
                var api = {};
                var voiceType;
                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        if (payload.voiceType != undefined) {
                            voiceType = payload.voiceType;
                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                            directiveReadyDeferred.promise.then(function () {
                                directiveReadyDeferred = undefined;
                                var payloadDirective = {
                                    voiceType: payload.voiceType
                                };
                                VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                            });
                            promises.push(loadDirectivePromiseDeferred.promise);
                        }
                    }
                    var getVoiceTypesTemplateConfigsPromise = getVoiceTypesTemplateConfigs();
                    promises.push(getVoiceTypesTemplateConfigsPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                }
                function getVoiceTypesTemplateConfigs() {
                    return Retail_BE_PackageAPIService.GetVoiceTypesTemplateConfigs().then(function (response) {
                        if (selectorAPI != undefined)
                            selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModel.templateConfigs.push(response[i]);
                            }
                            if (voiceType != undefined)
                                $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, voiceType.ConfigId, 'ExtensionConfigurationId');
                            //else
                            //$scope.selectedTemplateConfig = $scope.templateConfigs[0];
                        }
                    });
                }

            }
        }
    }

    app.directive('retailBePackageServicesettingsVoiceType', BePackageServicesettingsVoiceTypeDirective);

})(app);