"use strict";

app.directive("retailInvoiceInvoicetypeZajilsubscriberinvoicesettings", ["UtilsService", "VRNotificationService","VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ZajilnvoiceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Zajil/Directives/MainExtensions/ZajilInvoiceSettings/Templates/ZajilnvoiceSettingsTemplate.html"

        };

        function ZajilnvoiceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            $scope.scopeModel = {};

            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var companyExtendedInfoSelectorAPI;
            var companyExtendedInfoReadyDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorApi = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCompanyExtentedInfoSelectorReady = function (api) {
                    companyExtendedInfoSelectorAPI = api;
                    companyExtendedInfoReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var businessEntityDefinitionSelectorLoadPromise = getBusinessEntityDefinitionSelectorLoadPromise();
                    promises.push(businessEntityDefinitionSelectorLoadPromise);
                    promises.push(loadCompanyExtendedInfoDefinitionSelector());

                    function getBusinessEntityDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };
                            if (payload != undefined && payload.extendedSettingsEntity != undefined) {
                                selectorPayload.selectedIds = payload.extendedSettingsEntity.AccountBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });
                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }

                    function loadCompanyExtendedInfoDefinitionSelector() {
                        var extendedInfoSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        companyExtendedInfoReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    AccountPartDefinitionIds: getAccountPartDefinitionIds('F6630722-4E85-4DF2-915F-F9942074743C')
                                },
                                selectedIds: payload != undefined && payload.extendedSettingsEntity != undefined? payload.extendedSettingsEntity.CompanyExtendedInfoPartdefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(companyExtendedInfoSelectorAPI, selectorPayload, extendedInfoSelectorLoadDeferred);
                        });
                        return extendedInfoSelectorLoadDeferred.promise;
                    };
                    return UtilsService.waitMultiplePromises(promises);

                };


                api.getData = function () {
                    return {
                        $type: "Retail.Zajil.MainExtensions.ZajilSubscriberInvoiceSettings, Retail.Zajil.MainExtensions",
                        AccountBEDefinitionId: beDefinitionSelectorApi.getSelectedIds(),
                        CompanyExtendedInfoPartdefinitionId: companyExtendedInfoSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getAccountPartDefinitionIds(id) {
                var partDefinitionIds = [];
                partDefinitionIds.push(id);
                return partDefinitionIds;
            }
        }

        return directiveDefinitionObject;

    }
]);