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

            var userTypeAPI;
            var userTypeReadyDeferred = UtilsService.createPromiseDeferred();

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
                $scope.scopeModel.onUserAccountTypeSelectorReady = function (api) {
                    userTypeAPI = api;
                    userTypeReadyDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([companyTypeReadyDeferred.promise, siteTypeReadyDeferred.promise, userTypeReadyDeferred.promise]).then(function () {
                    defineAPI();
                });

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
                        var companyTypePayload;
                        if (accountCondition != undefined) {
                            companyTypePayload = { selectedIds: accountCondition.CompanyTypeId };
                        }
                        return companyTypeAPI.load(companyTypePayload);
                    }

                    promises.push(loadSiteTypes());

                    function loadSiteTypes() {
                        var siteTypePayload;
                        if (accountCondition != undefined) {
                            siteTypePayload = { selectedIds: accountCondition.SiteTypeId };
                        }
                        return siteTypeAPI.load(siteTypePayload);
                    }

                    promises.push(loadUserTypes());

                    function loadUserTypes() {
                        var userTypePayload;
                        if (accountCondition != undefined) {
                            userTypePayload = { selectedIds: accountCondition.UserTypeId };
                        }
                        return userTypeAPI.load(userTypePayload);
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        $type: "Retail.Teles.Business.TelesAccountCondition, Retail.Teles.Business",
                        ConditionType: $scope.scopeModel.selectedConditionType.value,
                        CompanyTypeId: companyTypeAPI.getSelectedIds(),
                        SiteTypeId: siteTypeAPI.getSelectedIds(),
                        UserTypeId: userTypeAPI.getSelectedIds(),
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