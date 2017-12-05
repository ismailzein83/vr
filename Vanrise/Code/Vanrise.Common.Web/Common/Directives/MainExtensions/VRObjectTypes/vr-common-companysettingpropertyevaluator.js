(function (app) {

    'use strict';

    CompanySettingPropertyEvaluator.$inject = ['VRCommon_CompanySettingPropertyEnum', 'UtilsService', 'VRUIUtilsService', 'VRCommon_CompanySettingsAPIService'];

    function CompanySettingPropertyEvaluator(VRCommon_CompanySettingPropertyEnum, UtilsService, VRUIUtilsService, VRCommon_CompanySettingsAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var companySettingPropertyEvaluatorSecurity = new CompanySettingPropertyEvaluatorSecurity($scope, ctrl, $attrs);
                companySettingPropertyEvaluatorSecurity.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/MainExtensions/VRObjectTypes/Templates/CompanySettingPropertyEvaluatorTemplate.html'
        };

        function CompanySettingPropertyEvaluatorSecurity($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var contactTypeSelectorAPI;
            var propertyEnumSelector;

            function initializeController() {

                $scope.scopeModel = {};

                var promises = [];
                var contactTypesDeferred = UtilsService.createPromiseDeferred();
                var propertyEnumsDeferred = UtilsService.createPromiseDeferred();
                var loadPromise = UtilsService.createPromiseDeferred();
                promises.push(contactTypesDeferred.promise);
                promises.push(propertyEnumsDeferred.promise);

                $scope.scopeModel.propertyEnums = UtilsService.getArrayEnum(VRCommon_CompanySettingPropertyEnum);
                promises.push(loadPromise.promise);
                VRCommon_CompanySettingsAPIService.GetCompanyContactTypes().then(function (response) {
                    $scope.scopeModel.contactTypes = response;
                    loadPromise.resolve();
            });

                $scope.scopeModel.onContactTypeSelectorReady = function (api) {
                    contactTypeSelectorAPI = api;
                    contactTypesDeferred.resolve();
                };

                $scope.scopeModel.onPropertyEnumSelectorReady = function (api) {
                    propertyEnumSelector = api;
                    propertyEnumsDeferred.resolve();
                };

                UtilsService.waitMultiplePromises(promises).then(function () { defineAPI(); });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.objectPropertyEvaluator != undefined) {
                        $scope.scopeModel.selectedContactType = UtilsService.getItemByVal($scope.scopeModel.contactTypes, payload.objectPropertyEvaluator.ContactType, "Name");
                        $scope.scopeModel.selectedPropertyEnum = UtilsService.getItemByVal($scope.scopeModel.propertyEnums, payload.objectPropertyEvaluator.ContactPropertyEnum, "value");
                    }
                };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.Common.MainExtensions.VRObjectTypes.CompanySettingPropertyEvaluator, Vanrise.Common.MainExtensions",
                        ContactType: $scope.scopeModel.selectedContactType.Name,
                        ContactPropertyEnum: $scope.scopeModel.selectedPropertyEnum.value,
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('vrCommonCompanysettingpropertyevaluator', CompanySettingPropertyEvaluator);

})(app);
