(function (app) {

    'use strict';

    ProvisionerDefinitionsettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ProvisionerDefinitionsettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ProvisionerDefinitionsettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/ChangeUsersRGs/Templates/RevertUsersRGsProvisionerDefinitionSettingsTemplate.html"

        };
        function ProvisionerDefinitionsettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;
            var conectionTypeAPI;
            var conectionTypeReadyDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onConectionTypeReady = function (api) {
                    conectionTypeAPI = api;
                    conectionTypeReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var provisionerDefinitionSettings;
                    if (payload != undefined) {
                        mainPayload = payload;
                        provisionerDefinitionSettings = payload.provisionerDefinitionSettings;
                        if(provisionerDefinitionSettings != undefined)
                        {
                            $scope.scopeModel.actionType = provisionerDefinitionSettings.ActionType;

                        }

                    }

                    var promises = [];
                    promises.push(loadConectionTypes());
                    function loadConectionTypes() {
                        var conectionTypeLoadDeferred = UtilsService.createPromiseDeferred();

                        conectionTypeReadyDeferred.promise.then(function () {
                            var conectionTypePayload;
                            if (provisionerDefinitionSettings != undefined) {
                                conectionTypePayload = { selectedIds: provisionerDefinitionSettings.VRConnectionId };
                            }
                            VRUIUtilsService.callDirectiveLoad(conectionTypeAPI, conectionTypePayload, conectionTypeLoadDeferred);
                        });
                        return conectionTypeLoadDeferred.promise
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.Teles.Business.RevertUsersRGsDefinitionSettings,Retail.Teles.Business",
                        ActionType: $scope.scopeModel.actionType,
                        VRConnectionId: conectionTypeAPI.getSelectedIds(),
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailTelesProvisionerDefinitionsettingsRevertusersrgs', ProvisionerDefinitionsettingsDirective);

})(app);