'use strict';

app.directive('retailTelesAccountactiondefinitionsettingsUnmappingtelesaccount', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new UnmappingTelesAccountActionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Teles/Directives/AccountAction/UnmappingTelesAccountAction/Templates/UnmappingTelesAccountActionSettingsTemplate.html'
        };

        function UnmappingTelesAccountActionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var accountBEDefinitionId;
            var companyTypeAPI;
            var companyTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            var siteTypeAPI;
            var siteDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCompanyAccountTypeSelectorReady = function (api) {
                    companyTypeAPI = api;
                    companyTypeDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onSiteAccountTypeSelectorReady = function (api) {
                    siteTypeAPI = api;
                    siteDirectiveReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([companyTypeDirectiveReadyDeferred.promise, siteDirectiveReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
                
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        var accountActionDefinitionSettings = payload.accountActionDefinitionSettings;
                    }

                    promises.push(loadCompanyTypes());

                    function loadCompanyTypes() {
                        var companyTypePayload;
                        if (accountActionDefinitionSettings != undefined) {
                            companyTypePayload = { selectedIds: accountActionDefinitionSettings.CompanyTypeId };
                        }
                        return companyTypeAPI.load(companyTypePayload);
                    }

                    promises.push(loadSiteTypes());

                    function loadSiteTypes() {
                        var siteTypePayload;
                        if (accountActionDefinitionSettings != undefined) {
                            siteTypePayload = { selectedIds: accountActionDefinitionSettings.SiteTypeId };
                        }
                        return siteTypeAPI.load(siteTypePayload);
                    }

                };

                api.getData = function () {
                    return {
                        $type: 'Retail.Teles.Business.AccountBEActionTypes.UnmappingTelesAccountActionSettings, Retail.Teles.Business',
                        CompanyTypeId: companyTypeAPI.getSelectedIds(),
                        SiteTypeId: siteTypeAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);