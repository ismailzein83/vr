"use strict";

app.directive("demoModuleMemberSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'Demo_Module_MemberService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, Demo_Module_MemberService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new MemberSearch($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Demo_Module/Directives/Member/templates/MemberSearchTemplate.html"

    };

    function MemberSearch($scope, ctrl, $attrs) {

        var gridAPI;
        var FamilyId;

        this.initializeController = initializeController;
        var context;
        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.addMember = function () {
                var FamilyIdItem = { FamilyId: FamilyId };
                var onMemberAdded = function (obj) {
                    gridAPI.onMemberAdded(obj);
                };
                Demo_Module_MemberService.addMember(onMemberAdded, FamilyIdItem);
            };
         
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    FamilyId = payload.familyId;
                }
                return gridAPI.load(getGridQuery());
            };
            api.onMemberAdded = function (memberObject) {
                gridAPI.onMemberAdded(memberObject);
            };


            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

        function getGridQuery() {
            var payload = {
                query: { FamilyIds: [FamilyId] },
                FamilyId: FamilyId,
                hideFamilyColumn:true
            };
            return payload;
        }
    }

    return directiveDefinitionObject;

}]);
