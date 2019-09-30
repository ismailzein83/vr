(function (appControllers) {

    "use strict";
    LogSettingsModuleFilterSpecificModule.$inject = ['UtilsService', 'VRUIUtilsService', 'VRCommon_ReceivedRequestLogAPIService'];
    function LogSettingsModuleFilterSpecificModule(UtilsService, VRUIUtilsService, VRCommon_ReceivedRequestLogAPIService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var logSettingsModuleFilterSpecificModule = new LogSettingsModuleFilterSpecificModuleController($scope, ctrl, $attrs);
                logSettingsModuleFilterSpecificModule.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Security/Directives/Settings/LogSettings/Templates/SpecificModuleLogSettingsFilterTemplate.html",
        };
        function LogSettingsModuleFilterSpecificModuleController($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.modules = [];
                $scope.scopeModel.isAddButtonDisabled = true;

                $scope.scopeModel.addModule = function () {
                    $scope.scopeModel.modules.push($scope.scopeModel.moduleName);
                    $scope.scopeModel.moduleName = undefined;
                };

                $scope.scopeModel.validateValue = function () {
                    if ($scope.scopeModel.moduleName == undefined || $scope.scopeModel.moduleName == null || $scope.scopeModel.moduleName == '') {
                        $scope.scopeModel.isAddButtonDisabled = true;
                        return null;
                    }
                    if (UtilsService.contains($scope.scopeModel.modules, $scope.scopeModel.moduleName)) {
                        $scope.scopeModel.isAddButtonDisabled = true;
                        return 'Module already exists';
                    }
                    $scope.scopeModel.isAddButtonDisabled = false;
                    return null;
                };

                $scope.scopeModel.validateModules = function () {
                    if ($scope.scopeModel.modules == undefined || $scope.scopeModel.modules.length == 0)
                        return "At least one module should be added.";
                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined && payload.moduleFilterSettings != undefined) {
                        $scope.scopeModel.modules = payload.moduleFilterSettings.IncludedModules;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Security.Entities.SpecificModule, Vanrise.Security.Entities ",
                        IncludedModules: $scope.scopeModel.modules
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
        return directiveDefinitionObject;
    }
    app.directive("vrSecLogsettingsModulefilterSpecificmodule", LogSettingsModuleFilterSpecificModule);
})(appControllers);