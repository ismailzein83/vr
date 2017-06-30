(function (app) {

    'use strict';

    TelesAccountConditionDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'Retail_Teles_ConditionTypeEnum'];

    function TelesAccountConditionDirective(UtilsService, VRUIUtilsService, Retail_Teles_ConditionTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TelesAccountConditionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountCondition/Templates/TelesAccountConditionTemplate.html"
        };

        function TelesAccountConditionCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var companyTypeAPI;
            var companyTypeReadyDeferred = UtilsService.createPromiseDeferred();

            var siteTypeAPI;
            var siteTypeReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.conditionTypes = UtilsService.getArrayEnum(Retail_Teles_ConditionTypeEnum);
                
                $scope.scopeModel.onCompanyAccountTypeSelectorReady = function (api) {
                    companyTypeAPI = api;
                    companyTypeReadyDeferred.resolve();
                };

                $scope.scopeModel.onSiteAccountTypeSelectorReady = function (api) {
                    siteTypeAPI = api;
                    siteTypeReadyDeferred.resolve();
                };
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        var accountCondition = payload.accountCondition;
                        if (accountCondition != undefined)
                        {
                            $scope.scopeModel.selectedConditionType = UtilsService.getItemByVal($scope.scopeModel.conditionTypes, accountCondition.ConditionType, 'value');
                            $scope.scopeModel.actionType = accountCondition.ActionType;
                        }

                    }
                    promises.push(loadCompanyTypes());

                    function loadCompanyTypes() {
                        var companyTypeLoadDeferred = UtilsService.createPromiseDeferred();

                        companyTypeReadyDeferred.promise.then(function () {
                            var companyTypePayload;
                            if (accountCondition != undefined) {
                                companyTypePayload = { selectedIds: accountCondition.CompanyTypeId };
                            }
                            VRUIUtilsService.callDirectiveLoad(companyTypeAPI, companyTypePayload, companyTypeLoadDeferred);
                        });
                        return companyTypeLoadDeferred.promise
                    }

                    promises.push(loadSiteTypes());

                    function loadSiteTypes() {
                        var siteTypeLoadDeferred = UtilsService.createPromiseDeferred();

                        siteTypeReadyDeferred.promise.then(function () {
                            var siteTypePayload;
                            if (accountCondition != undefined) {
                                siteTypePayload = { selectedIds: accountCondition.SiteTypeId };
                            }
                            VRUIUtilsService.callDirectiveLoad(siteTypeAPI, siteTypePayload, siteTypeLoadDeferred);
                        });
                        return siteTypeLoadDeferred.promise
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        $type: "Retail.Teles.Business.TelesAccountCondition, Retail.Teles.Business",
                        ConditionType: $scope.scopeModel.selectedConditionType.value,
                        CompanyTypeId: companyTypeAPI.getSelectedIds(),
                        SiteTypeId: siteTypeAPI.getSelectedIds(),
                        ActionType: $scope.scopeModel.actionType
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailTelesAccountconditionTelesaccount', TelesAccountConditionDirective);

})(app);