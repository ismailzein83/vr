'use strict';

app.directive('vrWhsRoutingCustomerrouteSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new settingsCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/CustomerRoute/Templates/CustomerRouteSettingsTemplate.html"
        };


        function settingsCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var admindUsersSelectorApi;
            var adminUsersSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var approvalTaskUsersSelectorApi;
            var approvalTaskUsersSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onAdmindUsersSelectorReady = function (api) {
                    admindUsersSelectorApi = api;
                    adminUsersSelectorReadyDeferred.resolve();
                };

                $scope.onApprovalTaskUsersSelectorReady = function (api) {
                    approvalTaskUsersSelectorApi = api;
                    approvalTaskUsersSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var adminUsersIds;
                    var approvalTaskUsersIds;

                    if (payload != undefined) {
                        if (payload.CustomerRoute != undefined) {
                            ctrl.customerRouteNumberOfOptions = payload.CustomerRoute.NumberOfOptions;
                            ctrl.customerRouteIndexCommandTimeout = payload.CustomerRoute.IndexesCommandTimeoutInMinutes;
                            ctrl.customerRouteMaxDOP = payload.CustomerRoute.MaxDOP;
                            ctrl.customerRouteKeepBackupsForRemovedOptions = payload.CustomerRoute.KeepBackUpsForRemovedOptions;
                        }

                        if (payload.ProductRoute != undefined) {
                            ctrl.productRouteIndexCommandTimeout = payload.ProductRoute.IndexesCommandTimeoutInMinutes;
                            ctrl.productRouteMaxDOP = payload.ProductRoute.MaxDOP;
                            ctrl.productRouteKeepBackupsForRemovedOptions = payload.ProductRoute.KeepBackUpsForRemovedOptions;
                            ctrl.generateCostAnalysisByCustomer = payload.ProductRoute.GenerateCostAnalysisByCustomer;
                            ctrl.productRouteIncludeBlockedZonesInCalculation = payload.ProductRoute.IncludeBlockedZonesInCalculation;
                        }

                        if (payload.IncludedRules != undefined) {
                            ctrl.includeRateTypeRules = payload.IncludedRules.IncludeRateTypeRules;
                            ctrl.includeExtraChargeRules = payload.IncludedRules.IncludeExtraChargeRules;
                            ctrl.includeTariffRules = payload.IncludedRules.IncludeTariffRules;
                        }

                        if (payload.PartialRoute != undefined) {
                            ctrl.needsApproval = payload.PartialRoute.NeedsApproval;
                        }

                        if (payload.UsersRole != undefined) {
                            adminUsersIds = payload.UsersRole.AdminUsersIds;
                            approvalTaskUsersIds = payload.UsersRole.ApprovalTaskUsersIds;
                        }
                    }

                    promises.push(loadAdminUserSelector(), loadApprovalTaskUserSelector());

                    function loadAdminUserSelector() {
                        var adminUsersSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        adminUsersSelectorReadyDeferred.promise.then(function () {

                            var adminSelectorPayload;
                            if (adminUsersIds != undefined)
                                adminSelectorPayload = { selectedIds: adminUsersIds };

                            VRUIUtilsService.callDirectiveLoad(admindUsersSelectorApi, adminSelectorPayload, adminUsersSelectorLoadDeferred);
                        });
                        return adminUsersSelectorLoadDeferred.promise;
                    }

                    function loadApprovalTaskUserSelector() {
                        var approvalTaskusersSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        approvalTaskUsersSelectorReadyDeferred.promise.then(function () {

                            var approvalTaskSelectorPayload;
                            if (approvalTaskUsersIds != undefined)
                                approvalTaskSelectorPayload = { selectedIds: approvalTaskUsersIds };

                            VRUIUtilsService.callDirectiveLoad(approvalTaskUsersSelectorApi, approvalTaskSelectorPayload, approvalTaskusersSelectorLoadDeferred);
                        });
                        return approvalTaskusersSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "TOne.WhS.Routing.Entities.RouteBuildConfiguration, TOne.WhS.Routing.Entities",
                        CustomerRoute: {
                            $type: "TOne.WhS.Routing.Entities.CustomerRouteBuildConfiguration, TOne.WhS.Routing.Entities",
                            NumberOfOptions: ctrl.customerRouteNumberOfOptions,
                            KeepBackUpsForRemovedOptions: ctrl.customerRouteKeepBackupsForRemovedOptions,
                            IndexesCommandTimeoutInMinutes: ctrl.customerRouteIndexCommandTimeout,
                            MaxDOP: ctrl.customerRouteMaxDOP
                        },
                        ProductRoute: {
                            $type: "TOne.WhS.Routing.Entities.ProductRouteBuildConfiguration, TOne.WhS.Routing.Entities",
                            KeepBackUpsForRemovedOptions: ctrl.productRouteKeepBackupsForRemovedOptions,
                            IndexesCommandTimeoutInMinutes: ctrl.productRouteIndexCommandTimeout,
                            MaxDOP: ctrl.productRouteMaxDOP,
                            GenerateCostAnalysisByCustomer: ctrl.generateCostAnalysisByCustomer,
                            IncludeBlockedZonesInCalculation: ctrl.productRouteIncludeBlockedZonesInCalculation
                        },
                        PartialRoute: {
                            $type: "TOne.WhS.Routing.Entities.PartialRouteBuildConfiguration, TOne.WhS.Routing.Entities",
                            NeedsApproval: ctrl.needsApproval
                        },
                        UsersRole: {
                            $type: "TOne.WhS.Routing.Entities.UsersRoleConfiguration, TOne.WhS.Routing.Entities",
                            AdminUsersIds: admindUsersSelectorApi.getSelectedIds(),
                            ApprovalTaskUsersIds: approvalTaskUsersSelectorApi.getSelectedIds()
                        },
                        IncludedRules: {
                            $type: "TOne.WhS.Routing.Entities.IncludedRulesConfiguration, TOne.WhS.Routing.Entities",
                            IncludeRateTypeRules: ctrl.includeRateTypeRules,
                            IncludeExtraChargeRules: ctrl.includeExtraChargeRules,
                            IncludeTariffRules: ctrl.includeTariffRules
                        }
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);