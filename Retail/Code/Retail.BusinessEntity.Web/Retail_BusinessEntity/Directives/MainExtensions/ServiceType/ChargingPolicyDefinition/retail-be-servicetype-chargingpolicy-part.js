(function (app) {

    'use strict';

    ServicetypeChargingpolicyPartDirective.$inject = ['Retail_BE_ServiceTypeAPIService', 'UtilsService', 'VRUIUtilsService'];

    function ServicetypeChargingpolicyPartDirective(Retail_BE_ServiceTypeAPIService, UtilsService, VRUIUtilsService) {
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
                var servicetypeChargingpolicyPart = new ServicetypeChargingpolicyPart($scope, ctrl, $attrs);
                servicetypeChargingpolicyPart.initializeController();
            },
            controllerAs: "chargingpolicypartCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Part'";
            if (attrs.hidelabel != undefined) {
                label = "Parts";
                withemptyline = '';
            }


            var template =

                    '<vr-columns colnum="{{chargingpolicypartCtrl.normalColNum * 2}}">'
                     + ' <vr-select on-ready="onSelectorReady" datasource="templateConfigs" selectedvalues="selectedTemplateConfig" datavaluefield="ExtensionConfigurationId" datatextfield="Title"'
                     + label + ' isrequired="chargingpolicypartCtrl.isrequired" hideremoveicon></vr-select></vr-columns>'
            + '<vr-row>'
               + '<vr-directivewrapper directive="selectedTemplateConfig.DefinitionEditor" on-ready="onDirectiveReady" normal-col-num="{{chargingpolicypartCtrl.normalColNum}}" isrequired="chargingpolicypartCtrl.isrequired" customvalidate="chargingpolicypartCtrl.customvalidate"></vr-directivewrapper>'
             + '</vr-row>';
            return template;

        }
        function ServicetypeChargingpolicyPart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;
            var part;
            function initializeController() {
                $scope.templateConfigs = [];
                $scope.selectedTemplateConfig;

                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
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
                    if (payload != undefined) {
                        part = payload.PartDefinitionSettings;
                        var partTypeConfigId = payload.partTypeConfigId;
                        if (part != undefined)
                        {
                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                            directiveReadyDeferred.promise.then(function () {
                                directiveReadyDeferred = undefined;
                                var payloadDirective = part;

                                VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                            });
                            promises.push(loadDirectivePromiseDeferred.promise);
                        }
                        var getChargingPolicyPartTemplateConfigsPromise = getChargingPolicyPartTemplateConfigs(partTypeConfigId);
                        promises.push(getChargingPolicyPartTemplateConfigsPromise);
                    }

                  

                   

                    function getChargingPolicyPartTemplateConfigs(partTypeConfigId) {
                        return Retail_BE_ServiceTypeAPIService.GetChargingPolicyPartTemplateConfigs(partTypeConfigId).then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.templateConfigs.push(response[i]);
                                }
                                if (part != undefined)
                                    $scope.selectedTemplateConfig = UtilsService.getItemByVal($scope.templateConfigs, part.ConfigId, 'ExtensionConfigurationId');
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
                        var directiveData = directiveAPI.getData();
                        if(directiveData !=undefined)
                        {
                            directiveData.ConfigId = $scope.selectedTemplateConfig.ExtensionConfigurationId;
                            directiveData.PartTitle = $scope.selectedTemplateConfig.Title;
                        }
                        data = {
                            $type:"Retail.BusinessEntity.Entities.ChargingPolicyDefinitionPart,Retail.BusinessEntity.Entities",
                            PartDefinitionSettings: directiveData
                        };
                    }
                    return data;
                }
            }
        }
    }

    app.directive('retailBeServicetypeChargingpolicyPart', ServicetypeChargingpolicyPartDirective);

})(app);