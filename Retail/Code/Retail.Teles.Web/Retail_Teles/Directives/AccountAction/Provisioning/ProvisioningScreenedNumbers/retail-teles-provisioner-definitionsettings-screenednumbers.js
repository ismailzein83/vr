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
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/ProvisioningScreenedNumbers/Templates/ProvisioningScreenedNumbersDefinitionSettingsTemplate.html"

        };
        function ProvisionerDefinitionsettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;

            var conectionTypeAPI;
            var conectionTypeReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
            
                $scope.scopeModel.enterpriseMaxCalls = 10;
                $scope.scopeModel.enterpriseMaxCallsPerUser = 3;
                $scope.scopeModel.enterpriseMaxRegistrations = 10;
                $scope.scopeModel.enterpriseMaxRegsPerUser = 10;
                $scope.scopeModel.enterpriseMaxSubsPerUser = 3;
                $scope.scopeModel.enterpriseMaxBusinessTrunkCalls = 10;
                $scope.scopeModel.enterpriseMaxUsers = 20;

                $scope.scopeModel.siteMaxCalls = 10;
                $scope.scopeModel.siteMaxCallsPerUser = 3;
                $scope.scopeModel.siteMaxRegistrations = 10;
                $scope.scopeModel.siteMaxRegsPerUser = 10;
                $scope.scopeModel.siteMaxSubsPerUser = 3;
                $scope.scopeModel.siteMaxBusinessTrunkCalls = 10;
                $scope.scopeModel.siteMaxUsers = 1;
               
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
                        if (provisionerDefinitionSettings != undefined) {
                            $scope.scopeModel.actionType = provisionerDefinitionSettings.ActionType;
                            $scope.scopeModel.countryCode = provisionerDefinitionSettings.CountryCode;
                            $scope.scopeModel.centrexFeatSet = provisionerDefinitionSettings.CentrexFeatSet;
                            $scope.scopeModel.enterpriseMaxCalls = provisionerDefinitionSettings.EnterpriseMaxCalls;
                            $scope.scopeModel.enterpriseMaxCallsPerUser = provisionerDefinitionSettings.EnterpriseMaxCallsPerUser;
                            $scope.scopeModel.enterpriseMaxRegistrations = provisionerDefinitionSettings.EnterpriseMaxRegistrations
                            $scope.scopeModel.enterpriseMaxRegsPerUser = provisionerDefinitionSettings.EnterpriseMaxRegsPerUser;
                            $scope.scopeModel.enterpriseMaxSubsPerUser = provisionerDefinitionSettings.EnterpriseMaxSubsPerUser
                            $scope.scopeModel.enterpriseMaxBusinessTrunkCalls = provisionerDefinitionSettings.EnterpriseMaxBusinessTrunkCalls;
                            $scope.scopeModel.enterpriseMaxUsers = provisionerDefinitionSettings.EnterpriseMaxUsers;

                            $scope.scopeModel.siteMaxCalls = provisionerDefinitionSettings.SiteMaxCalls;
                            $scope.scopeModel.siteMaxCallsPerUser = provisionerDefinitionSettings.SiteMaxCallsPerUser;
                            $scope.scopeModel.siteMaxRegistrations = provisionerDefinitionSettings.SiteMaxRegistrations
                            $scope.scopeModel.siteMaxRegsPerUser = provisionerDefinitionSettings.SiteMaxRegsPerUser;
                            $scope.scopeModel.siteMaxSubsPerUser = provisionerDefinitionSettings.SiteMaxSubsPerUser
                            $scope.scopeModel.siteMaxBusinessTrunkCalls = provisionerDefinitionSettings.SiteMaxBusinessTrunkCalls;
                            $scope.scopeModel.siteMaxUsers = provisionerDefinitionSettings.SiteMaxUsers;
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
                        $type: "Retail.Teles.Business.Provisioning.ProvisioningScreenedNumbersDefinitionSettings,Retail.Teles.Business",
                        ActionType: $scope.scopeModel.actionType,
                        VRConnectionId: conectionTypeAPI.getSelectedIds(),
                        CountryCode: $scope.scopeModel.countryCode,
                        CentrexFeatSet: $scope.scopeModel.centrexFeatSet,
                        EnterpriseMaxCalls : $scope.scopeModel.enterpriseMaxCalls,
                        EnterpriseMaxCallsPerUser: $scope.scopeModel.enterpriseMaxCallsPerUser,
                        EnterpriseMaxRegistrations: $scope.scopeModel.enterpriseMaxRegistrations,
                        EnterpriseMaxRegsPerUser: $scope.scopeModel.enterpriseMaxRegsPerUser,
                        EnterpriseMaxSubsPerUser: $scope.scopeModel.enterpriseMaxSubsPerUser,
                        EnterpriseMaxBusinessTrunkCalls: $scope.scopeModel.enterpriseMaxBusinessTrunkCalls,
                        EnterpriseMaxUsers: $scope.scopeModel.enterpriseMaxUsers,

                        SiteMaxCalls : $scope.scopeModel.siteMaxCalls,
                        SiteMaxCallsPerUser: $scope.scopeModel.siteMaxCallsPerUser,
                        SiteMaxRegistrations: $scope.scopeModel.siteMaxRegistrations,
                        SiteMaxRegsPerUser: $scope.scopeModel.siteMaxRegsPerUser,
                        SiteMaxSubsPerUser: $scope.scopeModel.siteMaxSubsPerUser,
                        SiteMaxBusinessTrunkCalls: $scope.scopeModel.siteMaxBusinessTrunkCalls,
                        SiteMaxUsers: $scope.scopeModel.siteMaxUsers,
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailTelesProvisionerDefinitionsettingsScreenednumbers', ProvisionerDefinitionsettingsDirective);

})(app);