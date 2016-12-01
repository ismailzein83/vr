(function (app) {

    'use strict';

    ChargingpolicyParttypeDirective.$inject = ['Retail_BE_ChargingPolicyAPIService', 'UtilsService', 'VRUIUtilsService'];

    function ChargingpolicyParttypeDirective(Retail_BE_ChargingPolicyAPIService, UtilsService, VRUIUtilsService) {
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
            controllerAs: "parttypeCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Part Type'";
            if (attrs.hidelabel != undefined) {
                label = "Part Types";
                withemptyline = '';
            }


            var template =
                '<vr-row>' +
                '<vr-columns colnum="{{parttypeCtrl.normalColNum * 2}}">' +
                ' <vr-select on-ready="onSelectorReady" datasource="templateConfigs" selectedvalues="selectedTemplateConfig" datavaluefield="ExtensionConfigurationId" datatextfield="Title" onselectionchanged="onPartTypeSelectionChanged"' +
                label + ' isrequired="parttypeCtrl.isrequired" hideremoveicon></vr-select>' +
                ' </vr-columns>' +
                '<retail-be-servicetype-chargingpolicy-part on-ready="partDirectiveReady"  isrequired="true" normal-col-num="{{parttypeCtrl.normalColNum}}"></retail-be-servicetype-chargingpolicy-part>' +
                '</vr-row>';
            return template;

        }

        function ChargingpolicyDefinition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var selectorAPI;

            var directiveAPI;

            var partDirectiveAPI;
            var partDirectiveReadyDeffered;
            var partType;
            var context;
            function initializeController() {
                $scope.templateConfigs = [];
                $scope.selectedTemplateConfig;

                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.partDirectiveReady = function (api) {
                    partDirectiveAPI = api;
                    if (partDirectiveReadyDeffered)
                        partDirectiveReadyDeffered.resolve();
                };

                $scope.onPartTypeSelectionChanged = function () {
                    var directivePayload = $scope.selectedTemplateConfig != undefined ? {
                        partTypeConfigId: $scope.selectedTemplateConfig.ExtensionConfigurationId
                    } : undefined;
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, partDirectiveAPI, directivePayload, setLoader, partDirectiveReadyDeffered);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    context = payload.context; 
                    if (payload != undefined) {
                        partType = payload.partType;
                        if (partType != undefined) {
                            var loadPartPromiseDeferred = UtilsService.createPromiseDeferred();
                            partDirectiveReadyDeffered = UtilsService.createPromiseDeferred();
                            partDirectiveReadyDeffered.promise.then(function () {
                                partDirectiveReadyDeffered = undefined;
                                var payloadDirective = {
                                    PartDefinitionSettings: partType.PartDefinitionSettings,
                                    partTypeConfigId: partType.PartTypeId
                                };
                                VRUIUtilsService.callDirectiveLoad(partDirectiveAPI, payloadDirective, loadPartPromiseDeferred);
                            });
                            promises.push(loadPartPromiseDeferred.promise);
                        }
                    }

                    var getChargingPolicyPartTypeTemplateConfigsPromise = getChargingPolicyPartTypeTemplateConfigs();
                    promises.push(getChargingPolicyPartTypeTemplateConfigsPromise);

                    return UtilsService.waitMultiplePromises(promises);

                    function getChargingPolicyPartTypeTemplateConfigs() {
                        return Retail_BE_ChargingPolicyAPIService.GetChargingPolicyPartTypeTemplateConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    var item = response[i];
                                    if (context != undefined && !context.checkIfPartTypeUsed(item.ExtensionConfigurationId))
                                        $scope.templateConfigs.push(item);
                                }
                                if (partType != undefined)
                                    $scope.selectedTemplateConfig = UtilsService.getItemByVal($scope.templateConfigs, partType.PartTypeId, 'ExtensionConfigurationId');
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
                    if ($scope.selectedTemplateConfig != undefined && partDirectiveAPI != undefined) {
                        data = partDirectiveAPI.getData();
                        if (data != undefined) {
                            data.PartTypeId = $scope.selectedTemplateConfig.ExtensionConfigurationId;
                            data.PartTypeTitle = $scope.selectedTemplateConfig.Title;
                        }
                    }
                    return data;
                }

            }
        }
    }

    app.directive('retailBeServicetypeChargingpolicyParttype', ChargingpolicyParttypeDirective);

})(app);