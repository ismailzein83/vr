(function (appControllers) {

    "use strict";

    SMSMessageTemplateManagementController.$inject = ['$scope', 'VRCommon_SMSMessageTemplateAPIService', 'VRCommon_SMSMessageTemplateService', 'UtilsService', 'VRUIUtilsService'];

    function SMSMessageTemplateManagementController($scope, VRCommon_SMSMessageTemplateAPIService, VRCommon_SMSMessageTemplateService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };

            $scope.scopeModel.add = function () {
                var onSMSMessageTemplateAdded = function (addedSMSMessageTemplate) {
                    gridAPI.onSMSMessageTemplateAdded(addedSMSMessageTemplate);
                };
                VRCommon_SMSMessageTemplateService.addSMSMessageTemplate(onSMSMessageTemplateAdded);
            };

            $scope.scopeModel.hasAddSMSMessageTemplatePermission = function () {
                return VRCommon_SMSMessageTemplateAPIService.HasAddSMSMessageTemplatePermission();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }
        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
            };
        }
    }

    appControllers.controller('VRCommon_SMSMessageTemplateManagementController', SMSMessageTemplateManagementController);

})(appControllers);