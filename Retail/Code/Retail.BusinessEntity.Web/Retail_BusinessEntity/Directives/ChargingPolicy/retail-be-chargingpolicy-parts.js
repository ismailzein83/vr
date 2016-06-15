'use strict';

app.directive('retailBeChargingpolicyParts', ['Retail_BE_ServiceTypeAPIService', 'UtilsService', 'VRUIUtilsService', function (Retail_BE_ServiceTypeAPIService, UtilsService, VRUIUtilsService)
{
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var chargingPolicyParts = new ChargingPolicyParts($scope, ctrl, $attrs);
            chargingPolicyParts.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/ChargingPolicy/Templates/ChargingPolicyPartsTemplate.html'
    };

    function ChargingPolicyParts($scope, ctrl, $attrs)
    {
        this.initializeController = initializeController;

        function initializeController()
        {
            $scope.scopeModel = {};
            $scope.scopeModel.tabs = [];
            defineAPI();
        }

        function defineAPI()
        {
            var api = {};

            api.load = function (payload) {
                $scope.scopeModel.tabs.length = 0;

                var partDefinitions;
                var parts;

                if (payload != undefined) {
                    partDefinitions = payload.partDefinitions;
                    parts = payload.parts;
                }

                if (partDefinitions == undefined)
                    return;

                var tabLoadPromises = [];

                for (var i = 0; i < partDefinitions.length; i++) {
                    var partDefinition = partDefinitions[i];
                    var part = (parts != undefined) ? parts[i] : undefined;

                    var tab = buildTab(partDefinition, part);
                    tabLoadPromises.push(tab.directiveLoadDeferred.promise);
                    $scope.scopeModel.tabs.push(tab);
                }

                function buildTab(partDefinition, part) {
                    var tab = {};
                    tab.header = partDefinition.PartTypeTitle;

                    tab.directiveReadyDeferred = UtilsService.createPromiseDeferred();
                    tab.directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    tab.onDirectiveReady = function (api) {
                        tab.directiveAPI = api;
                        tab.directiveReadyDeferred.resolve();
                    };
                    
                    tab.directiveReadyDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(tab.directiveAPI, part, tab.directiveLoadDeferred);
                    });

                    setRuntimeEditor();

                    function setRuntimeEditor() {
                        return Retail_BE_ServiceTypeAPIService.GetChargingPolicyPartTemplateConfigs(partDefinition.PartTypeId).then(function (partTemplates) {
                            var partTemplate = UtilsService.getItemByVal(partTemplates, partDefinition.PartDefinitionSettings.ConfigId, 'ExtensionConfigurationId');
                            tab.runtimeEditor = partTemplate.RuntimeEditor;
                        });
                    }

                    return tab;
                }

                return UtilsService.waitMultiplePromises(tabLoadPromises);
            };

            api.getData = function () {
                var data = [];
                for (var i = 0; i < $scope.scopeModel.tabs.length; i++) {
                    var tab = $scope.scopeModel.tabs[i];
                    var tabData = tab.directiveAPI.getData();
                    data.push(tabData);
                }
                return data;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
