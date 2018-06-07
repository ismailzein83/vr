(function (appControllers) {
    "use strict";

    memberManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_MemberService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_CompanyService'];

    function memberManagementController($scope, VRNotificationService, Demo_Module_MemberService, UtilsService, VRUIUtilsService, Demo_Module_CompanyService) {

        var gridApi;
        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                api.load(getFilter());
            };

            $scope.scopeModel.search = function () {
                return gridApi.load(getFilter());
            };


            $scope.scopeModel.addMember = function () {
                var onMemberAdded = function (member) {
                    if (gridApi != undefined) {
                       gridApi.onMemberAdded(member);
                    }
                };
                Demo_Module_MemberService.addMember(onMemberAdded);
            };
        };

        function load() {

        }


        function getFilter() {
            return {
                query: {
                    Name: $scope.scopeModel.name
                }
            };
        };

    };

    appControllers.controller('Demo_Module_MemberManagementController', memberManagementController);
})(appControllers);