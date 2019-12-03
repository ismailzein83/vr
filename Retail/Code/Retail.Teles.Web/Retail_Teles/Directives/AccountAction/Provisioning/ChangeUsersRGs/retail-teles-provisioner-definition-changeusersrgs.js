(function (app) {

    'use strict';

    ProvisionerDefinitionsettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'Retail_Teles_NewRGNoMatchHandlingEnum', 'Retail_Teles_NewRGMultiMatchHandlingEnum', 'Retail_Teles_ExistingRGNoMatchHandling'];

    function ProvisionerDefinitionsettingsDirective(UtilsService, VRUIUtilsService, Retail_Teles_NewRGNoMatchHandlingEnum, Retail_Teles_NewRGMultiMatchHandlingEnum, Retail_Teles_ExistingRGNoMatchHandling) {
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
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/ChangeUsersRGs/Templates/ChangeUserRGsDefinitionTemplate.html"

        };
        function ProvisionerDefinitionsettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var classFQTN;
            var newRoutingGroupConditionAPI;
            var newRoutingGroupConditionReadyDeferred = UtilsService.createPromiseDeferred();

            var existingRoutingGroupConditionAPI;
            var existingRoutingGroupConditionReadyDeferred = UtilsService.createPromiseDeferred();

            var companyTypeAPI;
            var companyTypeReadyDeferred = UtilsService.createPromiseDeferred();

            var siteTypeAPI;
            var siteTypeReadyDeferred = UtilsService.createPromiseDeferred();

            var userTypeAPI;
            var userTypeReadyDeferred = UtilsService.createPromiseDeferred();

            var conectionTypeAPI;
            var conectionTypeReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.newRGNoMatchHandlings = UtilsService.getArrayEnum(Retail_Teles_NewRGNoMatchHandlingEnum);
                $scope.scopeModel.newRGMultiMatchHandlings = UtilsService.getArrayEnum(Retail_Teles_NewRGMultiMatchHandlingEnum);
                $scope.scopeModel.existingRGNoMatchHandlings = UtilsService.getArrayEnum(Retail_Teles_ExistingRGNoMatchHandling);

                $scope.scopeModel.onConectionTypeReady = function (api) {
                    conectionTypeAPI = api;
                    conectionTypeReadyDeferred.resolve();
                };

                $scope.scopeModel.onCompanyAccountTypeSelectorReady = function (api) {
                    companyTypeAPI = api;
                    companyTypeReadyDeferred.resolve();
                };

                $scope.scopeModel.onSiteAccountTypeSelectorReady = function (api) {
                    siteTypeAPI = api;
                    siteTypeReadyDeferred.resolve();
                };
                $scope.scopeModel.onUserAccountTypeSelectorReady = function (api) {
                    userTypeAPI = api;
                    userTypeReadyDeferred.resolve();
                };
                $scope.scopeModel.onNewRoutingGroupConditionReady = function (api) {
                    newRoutingGroupConditionAPI = api;
                    newRoutingGroupConditionReadyDeferred.resolve();
                };
                $scope.scopeModel.onExistingRoutingGroupConditionReady = function (api) {
                    existingRoutingGroupConditionAPI = api;
                    existingRoutingGroupConditionReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([conectionTypeReadyDeferred.promise, companyTypeReadyDeferred.promise, siteTypeReadyDeferred.promise, userTypeReadyDeferred.promise, newRoutingGroupConditionReadyDeferred.promise, existingRoutingGroupConditionReadyDeferred.promise]).then(function () {
                    defineAPI();
                });


            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var settings;
                    if (payload != undefined) {
                        settings = payload.provisionerDefinitionSettings != undefined ? payload.provisionerDefinitionSettings : payload.ExtendedSettings;
                        classFQTN = payload.classFQTN;
                        if (settings != undefined) {
                            $scope.scopeModel.saveChangesToAccountState = settings.SaveChangesToAccountState;
                            $scope.scopeModel.actionType = settings.ActionType;
                            $scope.scopeModel.selectedNewRGNoMatchHandling = UtilsService.getItemByVal($scope.scopeModel.newRGNoMatchHandlings, settings.NewRGNoMatchHandling, "value");
                            $scope.scopeModel.selectedNewRGMultiMatchHandling = UtilsService.getItemByVal($scope.scopeModel.newRGMultiMatchHandlings, settings.NewRGMultiMatchHandling, "value");
                            $scope.scopeModel.selectedExistingRGNoMatchHandling = UtilsService.getItemByVal($scope.scopeModel.existingRGNoMatchHandlings, settings.ExistingRGNoMatchHandling, "value");
                        }
                    }

                    var promises = [];

                    function loadNewRoutingGroupCondition() {
                        var newRoutingGroupConditionLoadDeferred = UtilsService.createPromiseDeferred();

                        newRoutingGroupConditionReadyDeferred.promise.then(function () {
                            var newRoutingGroupConditionPayload;
                            if (settings != undefined) {
                                newRoutingGroupConditionPayload = { routingGroupCondition: settings.NewRoutingGroupCondition };
                            }
                            VRUIUtilsService.callDirectiveLoad(newRoutingGroupConditionAPI, newRoutingGroupConditionPayload, newRoutingGroupConditionLoadDeferred);
                        });
                        return newRoutingGroupConditionLoadDeferred.promise;
                    }
                    function loadExistingRoutingGroupCondition() {
                        var existingRoutingGroupConditionLoadDeferred = UtilsService.createPromiseDeferred();

                        existingRoutingGroupConditionReadyDeferred.promise.then(function () {
                            var existingRoutingGroupConditionPayload;
                            if (settings != undefined) {
                                existingRoutingGroupConditionPayload = { routingGroupCondition: settings.ExistingRoutingGroupCondition };
                            }
                            VRUIUtilsService.callDirectiveLoad(existingRoutingGroupConditionAPI, existingRoutingGroupConditionPayload, existingRoutingGroupConditionLoadDeferred);
                        });
                        return existingRoutingGroupConditionLoadDeferred.promise;
                    }

                    promises.push(loadNewRoutingGroupCondition());
                    promises.push(loadExistingRoutingGroupCondition());

                    promises.push(loadConnectionTypes());
                    function loadConnectionTypes() {
                        var conectionTypeLoadDeferred = UtilsService.createPromiseDeferred();

                        conectionTypeReadyDeferred.promise.then(function () {
                            var conectionTypePayload;
                            if (settings != undefined) {
                                conectionTypePayload = { selectedIds: settings.VRConnectionId };
                            }
                            VRUIUtilsService.callDirectiveLoad(conectionTypeAPI, conectionTypePayload, conectionTypeLoadDeferred);
                        });
                        return conectionTypeLoadDeferred.promise;
                    }

                    promises.push(loadCompanyTypes());

                    function loadCompanyTypes() {
                        var companyTypeLoadDeferred = UtilsService.createPromiseDeferred();

                        companyTypeReadyDeferred.promise.then(function () {
                            var companyTypePayload;
                            if (settings != undefined) {
                                companyTypePayload = { selectedIds: settings.CompanyTypeId };
                            }
                            VRUIUtilsService.callDirectiveLoad(companyTypeAPI, companyTypePayload, companyTypeLoadDeferred);
                        });
                        return companyTypeLoadDeferred.promise;
                    }

                    promises.push(loadSiteTypes());

                    function loadSiteTypes() {
                        var siteTypeLoadDeferred = UtilsService.createPromiseDeferred();

                        siteTypeReadyDeferred.promise.then(function () {
                            var siteTypePayload;
                            if (settings != undefined) {
                                siteTypePayload = { selectedIds: settings.SiteTypeId };
                            }
                            VRUIUtilsService.callDirectiveLoad(siteTypeAPI, siteTypePayload, siteTypeLoadDeferred);
                        });
                        return siteTypeLoadDeferred.promise;
                    }

                    promises.push(loadUserTypes());

                    function loadUserTypes() {
                        var userTypeLoadDeferred = UtilsService.createPromiseDeferred();

                        userTypeReadyDeferred.promise.then(function () {
                            var userTypePayload;
                            if (settings != undefined) {
                                userTypePayload = { selectedIds: settings.UserTypeId };
                            }
                            VRUIUtilsService.callDirectiveLoad(userTypeAPI, userTypePayload, userTypeLoadDeferred);
                        });
                        return userTypeLoadDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: classFQTN,
                        SaveChangesToAccountState: $scope.scopeModel.saveChangesToAccountState,
                        ActionType: $scope.scopeModel.saveChangesToAccountState ? $scope.scopeModel.actionType : undefined,
                        NewRoutingGroupCondition: newRoutingGroupConditionAPI.getData(),
                        ExistingRoutingGroupCondition: existingRoutingGroupConditionAPI.getData(),
                        VRConnectionId: conectionTypeAPI.getSelectedIds(),
                        NewRGNoMatchHandling: $scope.scopeModel.selectedNewRGNoMatchHandling.value,
                        NewRGMultiMatchHandling: $scope.scopeModel.selectedNewRGMultiMatchHandling.value,
                        ExistingRGNoMatchHandling: $scope.scopeModel.selectedExistingRGNoMatchHandling.value,
                        CompanyTypeId: companyTypeAPI.getSelectedIds(),
                        SiteTypeId: siteTypeAPI.getSelectedIds(),
                        UserTypeId: userTypeAPI.getSelectedIds()
                    };

                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }


            }
        }
    }

    app.directive('retailTelesProvisionerDefinitionChangeusersrgs', ProvisionerDefinitionsettingsDirective);

})(app);