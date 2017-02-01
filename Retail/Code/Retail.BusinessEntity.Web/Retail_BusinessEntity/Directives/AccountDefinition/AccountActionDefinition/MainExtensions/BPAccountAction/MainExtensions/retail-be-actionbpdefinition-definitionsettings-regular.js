(function (app) {

    'use strict';

    RegularActionbpdefinitionDefinitionsettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function RegularActionbpdefinitionDefinitionsettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var regularActionbpdefinitionDefinitionsettings = new RegularActionbpdefinitionDefinitionsettings($scope, ctrl, $attrs);
                regularActionbpdefinitionDefinitionsettings.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/MainExtensions/BPAccountAction/MainExtensions/Templates/RegularActionBPDefinitionSettingsTemplate.html"

        };
        function RegularActionbpdefinitionDefinitionsettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;

            var provisionerdefinitionSettingAPI;
            var provisionerdefinitionSettingReadyDeferred = UtilsService.createPromiseDeferred();

            var statusDefinitionSelectorAPI;
            var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};
                
                $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                    statusDefinitionSelectorAPI = api;
                    statusDefinitionSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onProvisionerDefinitionSettingReady = function (api) {
                    provisionerdefinitionSettingAPI = api;
                    provisionerdefinitionSettingReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    console.log(payload);
                    var promises = [];

                    if (payload != undefined) {
                        mainPayload = payload;
                        if(payload.entityType !=undefined)
                        {
                            var statusDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                            statusDefinitionSelectorReadyDeferred.promise.then(function () {
                                var statusDefinitionSelectorPayload = {
                                    selectedIds: payload != undefined && payload.bpDefinitionSettings != undefined ? payload.bpDefinitionSettings.NewStatusDefinitionId : undefined,
                                    filter:  { EntityType: payload.entityType }
                                };
                                VRUIUtilsService.callDirectiveLoad(statusDefinitionSelectorAPI, statusDefinitionSelectorPayload, statusDefinitionSelectorLoadDeferred);
                            });
                            promises.push(statusDefinitionSelectorLoadDeferred.promise);

                        }
                    }
               
                    var provisionerdefinitionSettingLoadDeferred = UtilsService.createPromiseDeferred();

                    provisionerdefinitionSettingReadyDeferred.promise.then(function () {
                        var definitionSettingPayload = payload != undefined && payload.bpDefinitionSettings != undefined ? { provisionerDefinitionSettings: payload.bpDefinitionSettings.ProvisionerDefinitionSettings } : undefined;
                        VRUIUtilsService.callDirectiveLoad(provisionerdefinitionSettingAPI, definitionSettingPayload, provisionerdefinitionSettingLoadDeferred);
                    });
                    promises.push(provisionerdefinitionSettingLoadDeferred.promise);
                    
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainActionBPs.Entities.RegularActionBPDefinitionSettings, Retail.BusinessEntity.MainActionBPs.Entities",
                        ProvisionerDefinitionSettings: provisionerdefinitionSettingAPI.getData(),
                        NewStatusDefinitionId: statusDefinitionSelectorAPI.getSelectedIds()
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBeActionbpdefinitionDefinitionsettingsRegular', RegularActionbpdefinitionDefinitionsettingsDirective);

})(app);