(function (app) {

    'use strict';

    ProvisionerAccountsettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ProvisionerAccountsettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ProvisionerAccountsettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/ProvisioningScreenedNumbers/Templates/ProvisionAccountSettingsTemplate.html"

        };
        function ProvisionerAccountsettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;

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

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var provisionAccountSettings;
                    if (payload != undefined) {
                        mainPayload = payload;
                        provisionAccountSettings = payload.provisionAccountSettings;
                        $scope.scopeModel.showEnterpriseSettings = payload.showEnterpriseSettings;
                        if (provisionAccountSettings != undefined) {
                           
                            if (provisionAccountSettings.EnterpriseAccountSetting != undefined)
                            {
                                $scope.scopeModel.enterpriseMaxCalls = provisionAccountSettings.EnterpriseAccountSetting.EnterpriseMaxCalls;
                                $scope.scopeModel.enterpriseMaxCallsPerUser = provisionAccountSettings.EnterpriseAccountSetting.EnterpriseMaxCallsPerUser;
                                $scope.scopeModel.enterpriseMaxRegistrations = provisionAccountSettings.EnterpriseAccountSetting.EnterpriseMaxRegistrations;
                                $scope.scopeModel.enterpriseMaxRegsPerUser = provisionAccountSettings.EnterpriseAccountSetting.EnterpriseMaxRegsPerUser;
                                $scope.scopeModel.enterpriseMaxSubsPerUser = provisionAccountSettings.EnterpriseAccountSetting.EnterpriseMaxSubsPerUser;
                                $scope.scopeModel.enterpriseMaxBusinessTrunkCalls = provisionAccountSettings.EnterpriseAccountSetting.EnterpriseMaxBusinessTrunkCalls;
                                $scope.scopeModel.enterpriseMaxUsers = provisionAccountSettings.EnterpriseAccountSetting.EnterpriseMaxUsers;
                            }
                            $scope.scopeModel.centrexFeatSet = provisionAccountSettings.CentrexFeatSet;
                           
                            if (provisionAccountSettings.SiteAccountSetting != undefined)
                            {
                                $scope.scopeModel.siteMaxCalls = provisionAccountSettings.SiteAccountSetting.SiteMaxCalls;
                                $scope.scopeModel.siteMaxCallsPerUser = provisionAccountSettings.SiteAccountSetting.SiteMaxCallsPerUser;
                                $scope.scopeModel.siteMaxRegistrations = provisionAccountSettings.SiteAccountSetting.SiteMaxRegistrations;
                                $scope.scopeModel.siteMaxRegsPerUser = provisionAccountSettings.SiteAccountSetting.SiteMaxRegsPerUser;
                                $scope.scopeModel.siteMaxSubsPerUser = provisionAccountSettings.SiteAccountSetting.SiteMaxSubsPerUser;
                                $scope.scopeModel.siteMaxBusinessTrunkCalls = provisionAccountSettings.SiteAccountSetting.SiteMaxBusinessTrunkCalls;
                                $scope.scopeModel.siteMaxUsers = provisionAccountSettings.SiteAccountSetting.SiteMaxUsers;
                            }
                     
                        }

                    }

                    var promises = [];

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        CentrexFeatSet: $scope.scopeModel.centrexFeatSet,
                        SiteAccountSetting:{
                            SiteMaxCalls: $scope.scopeModel.siteMaxCalls,
                            SiteMaxCallsPerUser: $scope.scopeModel.siteMaxCallsPerUser,
                            SiteMaxRegistrations: $scope.scopeModel.siteMaxRegistrations,
                            SiteMaxRegsPerUser: $scope.scopeModel.siteMaxRegsPerUser,
                            SiteMaxSubsPerUser: $scope.scopeModel.siteMaxSubsPerUser,
                            SiteMaxBusinessTrunkCalls: $scope.scopeModel.siteMaxBusinessTrunkCalls,
                            SiteMaxUsers: $scope.scopeModel.siteMaxUsers,
                        }
                    };
                    if ($scope.scopeModel.showEnterpriseSettings)
                    {
                        data.EnterpriseAccountSetting = {
                            EnterpriseMaxCalls: $scope.scopeModel.enterpriseMaxCalls,
                            EnterpriseMaxCallsPerUser: $scope.scopeModel.enterpriseMaxCallsPerUser,
                            EnterpriseMaxRegistrations: $scope.scopeModel.enterpriseMaxRegistrations,
                            EnterpriseMaxRegsPerUser: $scope.scopeModel.enterpriseMaxRegsPerUser,
                            EnterpriseMaxSubsPerUser: $scope.scopeModel.enterpriseMaxSubsPerUser,
                            EnterpriseMaxBusinessTrunkCalls: $scope.scopeModel.enterpriseMaxBusinessTrunkCalls,
                            EnterpriseMaxUsers: $scope.scopeModel.enterpriseMaxUsers,
                        };
                    }
                    return data;
                }
            }
        }
    }

    app.directive('retailTelesProvisionerProvisionaccountSettings', ProvisionerAccountsettingsDirective);

})(app);