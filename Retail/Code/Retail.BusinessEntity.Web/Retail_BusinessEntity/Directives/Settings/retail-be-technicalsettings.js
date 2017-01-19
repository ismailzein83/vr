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

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                    accountTypeSelectorAPI = api;
                    accountTypeSelectorPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var includedAccountTypes;

                    if (payload != undefined && payload.data != undefined) {
                        includedAccountTypes = payload.data.IncludedAccountTypes;
                    }

                    //Loading AccountType selector
                    var accountTypeSelectorLoadPromise = getAccountTypeSelectorLoadPromise();
                    promises.push(accountTypeSelectorLoadPromise);


                    function getAccountTypeSelectorLoadPromise() {
                        var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        accountTypeSelectorPromiseDeferred.promise.then(function () {

                            var selectorPayload = {
                                selectedIds: includedAccountTypes != undefined ? includedAccountTypes.AcountTypeIds : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, selectorPayload, accountTypeSelectorLoadDeferred);
                        });

                        return accountTypeSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var obj = {
                        $type: "Retail.BusinessEntity.Entities.RetailBETechnicalSettings, Retail.BusinessEntity.Entities",
                        IncludedAccountTypes: {
                            AcountTypeIds: accountTypeSelectorAPI.getSelectedIds()
                        }
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