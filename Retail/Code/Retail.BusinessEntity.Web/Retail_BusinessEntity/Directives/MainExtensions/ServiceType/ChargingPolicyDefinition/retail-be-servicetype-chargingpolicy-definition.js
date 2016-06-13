(function (app) {

    'use strict';

    ChargingpolicyDefinitionDirective.$inject = ['Retail_BE_ServiceTypeAPIService', 'UtilsService', 'VRUIUtilsService'];

    function ChargingpolicyDefinitionDirective(Retail_BE_ServiceTypeAPIService, UtilsService, VRUIUtilsService) {
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
                var chargingpolicyDefinition = new ChargingpolicyDefinition($scope, ctrl, $attrs);
                chargingpolicyDefinition.initializeController();
            },
            controllerAs: "chargingpolicyCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Charging Policy'";
            if (attrs.hidelabel != undefined) {
                label = "Charging Policies";
                withemptyline = '';
            }


            var template =
                '<vr-row>'
                   + '<vr-columns colnum="{{chargingpolicyCtrl.normalColNum * 2}}">'
                     + ' <vr-select on-ready="onSelectorReady" datasource="templateConfigs" selectedvalues="selectedTemplateConfig" datavaluefield="ExtensionConfigurationId" datatextfield="Title"'
                     + label + ' isrequired="chargingpolicyCtrl.isrequired" hideremoveicon></vr-select>'
               + '</vr-row>'
            + '<vr-row>'
               + '<vr-directivewrapper directive="selectedTemplateConfig.Editor" on-ready="chargingpolicyCtrl.onDirectiveReady" normal-col-num="{{chargingpolicyCtrl.normalColNum}}" isrequired="chargingpolicyCtrl.isrequired" customvalidate="chargingpolicyCtrl.customvalidate"></vr-directivewrapper>'
             + '</vr-row>';
            return template;

        }
        function ChargingpolicyDefinition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;
            var chargingPolicy;
            function initializeController() {
                $scope.templateConfigs = [];
                $scope.selectedTemplateConfig;

                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                ctrl.onDirectiveReady = function (api) {
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
                    if (payload != undefined) {
           
                        chargingPolicy = payload.chargingPolicy;
                        if (chargingPolicy != undefined)
                        {
                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                            directiveReadyDeferred.promise.then(function () {
                                directiveReadyDeferred = undefined;
                                var payloadDirective = { chargingPolicy: chargingPolicy };

                                VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                            });
                            promises.push(loadDirectivePromiseDeferred.promise);
                        }
                    }

                    var getChargingPolicyTemplateConfigsPromise = getChargingPolicyTemplateConfigs();
                    promises.push(getChargingPolicyTemplateConfigsPromise);

                    return UtilsService.waitMultiplePromises(promises);

                    function getChargingPolicyTemplateConfigs() {
                        return Retail_BE_ServiceTypeAPIService.GetChargingPolicyTemplateConfigs().then(function (response) {
                          
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.templateConfigs.push(response[i]);
                                }
                                if (chargingPolicy != undefined)
                                    $scope.selectedTemplateConfig = UtilsService.getItemByVal($scope.templateConfigs, chargingPolicy.ConfigId, 'ExtensionConfigurationId');
                                else
                                    $scope.selectedTemplateConfig = $scope.templateConfigs[0];
                            }
                        });
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
                        }
                    }
                    return data;
                }
            }
        }
    }

    app.directive('retailBeServicetypeChargingpolicyDefinition', ChargingpolicyDefinitionDirective);

})(app);