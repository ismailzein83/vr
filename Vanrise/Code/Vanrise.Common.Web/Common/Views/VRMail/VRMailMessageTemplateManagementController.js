(function (appControllers) {

    "use strict";

    MailMessageTemplateManagementController.$inject = ['$scope', 'VRCommon_VRMailMessageTemplateAPIService', 'VRCommon_VRMailMessageTemplateService', 'UtilsService', 'VRUIUtilsService'];

    function MailMessageTemplateManagementController($scope, VRCommon_VRMailMessageTemplateAPIService, VRCommon_VRMailMessageTemplateService, UtilsService, VRUIUtilsService) {

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
                var onMailMessageTemplateAdded = function (addedMailMessageTemplate) {
                    gridAPI.onMailMessageTemplateAdded(addedMailMessageTemplate);
                };
                VRCommon_VRMailMessageTemplateService.addMailMessageTemplate(onMailMessageTemplateAdded);
            };

            $scope.scopeModel.hasAddMailMessageTemplatePermission = function () {
                return VRCommon_VRMailMessageTemplateAPIService.HasAddMailMessageTemplatePermission();
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

    appControllers.controller('VRCommon_VRMailMessageTemplateManagementController', MailMessageTemplateManagementController);

})(appControllers);