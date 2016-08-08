(function (appControllers) {

    "use strict";

    MailMessageTypeManagementController.$inject = ['$scope', 'VRCommon_MailMessageTypeService', 'UtilsService', 'VRUIUtilsService'];

    function MailMessageTypeManagementController($scope, VRCommon_MailMessageTypeService, UtilsService, VRUIUtilsService) {

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
                var onMailMessageTypeAdded = function (addedMailMessageType) {
                    gridAPI.onMailMessageTypeAdded(addedMailMessageType);
                }
                VRCommon_MailMessageTypeService.addMailMessageType(onMailMessageTypeAdded);
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

    appControllers.controller('VRCommon_MailMessageTypeManagementController', MailMessageTypeManagementController);

})(appControllers);