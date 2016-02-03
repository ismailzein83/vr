(function (app) {

    'use strict';

    GenericRuleDefinitionSettingsSelectiveDirective.$inject = ['VR_GenericData_GenericRuleDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function GenericRuleDefinitionSettingsSelectiveDirective(VR_GenericData_GenericRuleDefinitionAPIService, UtilsService, VRUIUtilsService) {
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
                    // directivePayload is cloned in case the next statement is executed before the directive has loaded
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, UtilsService.cloneObject(directivePayload, true), setLoader, undefined);
                    directivePayload = undefined;
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinitionSettingsTemplates().then(function (response) {
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModel.settingsTemplates.push(response[i]);
                            }
                            if (payload != undefined && payload.ConfigId != undefined) {
                                directivePayload = payload;
                                $scope.scopeModel.selectedSettingsTemplate = UtilsService.getItemByVal($scope.scopeModel.settingsTemplates, payload.ConfigId, 'TemplateConfigID');
                            }
                        }
                    });
                };

                api.getData = function () {
                    var data = directiveAPI.getData();
                    data.ConfigId = $scope.scopeModel.selectedSettingsTemplate.TemplateConfigID;
                    return data;
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataGenericruledefinitionsettingsSelective', GenericRuleDefinitionSettingsSelectiveDirective);

})(app);