"use strict"
app.directive("demoModuleMemberGrid", ["UtilsService", "VRNotificationService", "Demo_Module_MemberAPIService", "Demo_Module_MemberService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_MemberAPIService, Demo_Module_MemberService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var memberGrid = new MemberGrid($scope, ctrl, $attrs);
            memberGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/Member/templates/MemberGridTemplate.html"
    };

    function MemberGrid($scope, ctrl) {

        var gridApi;
        var familyId;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.members = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (payload) {
                        var query = payload.query;
                        familyId = payload.familyId;
                        if (payload.hideFamilyColumn)
                            $scope.scopeModel.hideFamilyColumn = payload.hideFamilyColumn;
                        return gridApi.retrieveData(query);
                    };

                    directiveApi.onMemberAdded = function (member) {
                        gridApi.itemAdded(member);
                    };
                    return directiveApi;
                };
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object
             
                return Demo_Module_MemberAPIService.GetFilteredMembers(dataRetrievalInput)
                .then(function (response) {
                    console.log(response)
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            defineMenuActions();
        };

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editMember,

            }];
        };
        function editMember(member) {
            var onMemberUpdated = function (member) {
                gridApi.itemUpdated(member);
            };
            var familyIdItem = familyId != undefined ? { FamilyId: familyId } : undefined;
            Demo_Module_MemberService.editMember(member.MemberId, onMemberUpdated, familyIdItem);
        };


    };
    return directiveDefinitionObject;
}]);
