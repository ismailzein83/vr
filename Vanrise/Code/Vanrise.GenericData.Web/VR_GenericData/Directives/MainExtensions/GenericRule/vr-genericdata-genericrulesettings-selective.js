(function (app) {

    'use strict';

    GenericRuleDefinitionSettingsSelectiveDirective.$inject = ['VR_GenericData_GenericRuleTypeConfigAPIService', 'UtilsService', 'VRUIUtilsService'];

    function GenericRuleDefinitionSettingsSelectiveDirective(VR_GenericData_GenericRuleTypeConfigAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericRuleDefinitionSettingsSelective = new GenericRuleDefinitionSettingsSelective($scope, ctrl, $attrs);
                genericRuleDefinitionSettingsSelective.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/GenericRule/Templates/GenericRuleDefinitionSettingsSelectiveTemplate.html"
        };

        function GenericRuleDefinitionSettingsSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directivePayload;

            function initializeController() {
                ctrl.disableType = false;
                $scope.scopeModel = {};
                $scope.scopeModel.settingsTemplates = [];
                $scope.scopeModel.selectedSettingsTemplate;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, undefined);
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    
                    return VR_GenericData_GenericRuleTypeConfigAPIService.GetGenericRuleTypes().then(function (response) {
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModel.settingsTemplates.push(response[i]);
                            }
                            if (payload != undefined && payload.ConfigId != undefined) {
                                ctrl.disableType = true;
                                directivePayload = payload;
                                $scope.scopeModel.selectedSettingsTemplate = UtilsService.getItemByVal($scope.scopeModel.settingsTemplates, payload.ConfigId, 'ExtensionConfigurationId');
                            } else if (payload != undefined && payload.settingsTypeName != undefined) {
                                ctrl.disableType = true;
                                $scope.scopeModel.selectedSettingsTemplate = UtilsService.getItemByVal($scope.scopeModel.settingsTemplates, payload.settingsTypeName, 'Name');
                            }
                        }
                    });
                };

                api.getData = function () {
                    var data = directiveAPI.getData();
                    data.ConfigId = $scope.scopeModel.selectedSettingsTemplate.ExtensionConfigurationId;
                    return data;
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataGenericruledefinitionsettingsSelective', GenericRuleDefinitionSettingsSelectiveDirective);

})(app);