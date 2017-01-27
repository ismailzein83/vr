"use strict";

app.directive("retailBeTechnicalsettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RetailBETechnicalSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Settings/Templates/RetailBETechnicalSettingsTemplate.html"
        };

        function RetailBETechnicalSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountTypeSelectorAPI;
            var accountTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var applicationVisibilitySelectorAPI;
            var applicationVisibilityPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                    accountTypeSelectorAPI = api;
                    accountTypeSelectorPromiseDeferred.resolve();
                $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                    accountTypeSelectorAPI = api;
                    accountTypeSelectorPromiseDeferred.resolve();
                };
                };
                $scope.scopeModel.onApplicationVisibilitySelectorReady = function (api) {
                    applicationVisibilitySelectorAPI = api;
                    applicationVisibilityPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var includedAccountTypes;
                    var vrRetailVisibilityId;

                    if (payload != undefined && payload.data != undefined) {
                        includedAccountTypes = payload.data.IncludedAccountTypes;
                        vrRetailVisibilityId = payload.data.VRRetailVisibilityId;
                    }

                    //Loading AccountType selector
                    var accountTypeSelectorLoadPromise = getAccountTypeSelectorLoadPromise();
                    promises.push(accountTypeSelectorLoadPromise);

                    //Loading AccountType selector
                    var retailVisibilitySelectorLoadPromise = getRetailVisibilitySelectorLoadPromise();
                    promises.push(retailVisibilitySelectorLoadPromise);


                    function getAccountTypeSelectorLoadPromise() {
                        var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        accountTypeSelectorPromiseDeferred.promise.then(function () {

                            var selectorPayload = {
                                filter: {
                                    IncludeHiddenAccountTypes: true
                                },
                                selectedIds: includedAccountTypes != undefined ? includedAccountTypes.AcountTypeIds : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, selectorPayload, accountTypeSelectorLoadDeferred);
                        });

                        return accountTypeSelectorLoadDeferred.promise;
                    }
                    function getRetailVisibilitySelectorLoadPromise() {
                        var retailVisibilitySelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        applicationVisibilityPromiseDeferred.promise.then(function () {

                            var selectorPayload = {
                                selectedIds: vrRetailVisibilityId
                            };
                            VRUIUtilsService.callDirectiveLoad(applicationVisibilitySelectorAPI, selectorPayload, retailVisibilitySelectorLoadDeferred);
                        });

                        return retailVisibilitySelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var obj = {
                        $type: "Retail.BusinessEntity.Entities.RetailBETechnicalSettings, Retail.BusinessEntity.Entities",
                        IncludedAccountTypes: {
                            AcountTypeIds: accountTypeSelectorAPI.getSelectedIds()
                        },
                        VRRetailVisibilityId: applicationVisibilitySelectorAPI.getSelectedIds()
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);