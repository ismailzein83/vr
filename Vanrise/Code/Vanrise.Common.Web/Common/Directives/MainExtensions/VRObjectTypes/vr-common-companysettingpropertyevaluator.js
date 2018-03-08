(function (app) {

    'use strict';

    CompanySettingPropertyEvaluator.$inject = ['VRCommon_CompanySettingPropertyEnum', 'UtilsService', 'VRUIUtilsService', 'VRCommon_CompanySettingsAPIService','VRCommon_CompanySettingFieldEnum'];

    function CompanySettingPropertyEvaluator(VRCommon_CompanySettingPropertyEnum, UtilsService, VRUIUtilsService, VRCommon_CompanySettingsAPIService, VRCommon_CompanySettingFieldEnum) {
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

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.propertyEnums = UtilsService.getArrayEnum(VRCommon_CompanySettingPropertyEnum);

                $scope.scopeModel.propertyFields = UtilsService.getArrayEnum(VRCommon_CompanySettingFieldEnum);

                $scope.scopeModel.onPropertyFieldSelectionChanged = function (value) {
                    if (value != undefined && value.value == VRCommon_CompanySettingFieldEnum.Contact.value)
                        $scope.scopeModel.showContactTypes = true;
                    else {
                        $scope.scopeModel.selectedPropertyEnum = undefined;
                        $scope.scopeModel.selectedContactType = undefined;
                        $scope.scopeModel.showContactTypes = false;
                    }
                 
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var promise = VRCommon_CompanySettingsAPIService.GetCompanyContactTypes().then(function (response) {
                        $scope.scopeModel.contactTypes = response;

                        if (payload != undefined && payload.objectPropertyEvaluator != undefined && payload.objectPropertyEvaluator.ContactType != undefined) {
                            $scope.scopeModel.selectedContactType = UtilsService.getItemByVal($scope.scopeModel.contactTypes, payload.objectPropertyEvaluator.ContactType, "Name");
                        }

                    });

                    promises.push(promise);

                    if (payload != undefined && payload.objectPropertyEvaluator != undefined) {
                        $scope.scopeModel.selectedField = UtilsService.getItemByVal($scope.scopeModel.propertyFields, payload.objectPropertyEvaluator.Field, "value");

                        if (payload.objectPropertyEvaluator.ContactPropertyEnum != undefined)
                         $scope.scopeModel.selectedPropertyEnum = UtilsService.getItemByVal($scope.scopeModel.propertyEnums, payload.objectPropertyEvaluator.ContactPropertyEnum, "value");
                    }
                };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.Common.MainExtensions.VRObjectTypes.CompanySettingPropertyEvaluator, Vanrise.Common.MainExtensions",
                        ContactType: $scope.scopeModel.selectedContactType != undefined ? $scope.scopeModel.selectedContactType.Name : undefined,
                        ContactPropertyEnum: $scope.scopeModel.selectedPropertyEnum != undefined?$scope.scopeModel.selectedPropertyEnum.value:undefined,
                        Field: $scope.scopeModel.selectedField.value,
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
