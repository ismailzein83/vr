"use strict";

app.directive("vrWhsBeCodegroupGrid", ["UtilsService", "VRNotificationService", "WhS_BE_CodeGroupAPIService", "WhS_BE_CodeGroupService",
function (UtilsService, VRNotificationService, WhS_BE_CodeGroupAPIService, WhS_BE_CodeGroupService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var codeGroupGrid = new CodeGroupGrid($scope, ctrl, $attrs);
            codeGroupGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CodeGroup/Templates/CodeGroupGridTemplate.html"

    };

    function CodeGroupGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var disabCountry;
        this.initializeController = initializeController;

        function initializeController() {
            if ($attrs.hidecountrycolumn != undefined) {
                $scope.hidecountrycolumn = disabCountry = true;
            }
            $scope.codegroups = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onCodeGroupAdded = function (codeGroupObject) {
                        gridAPI.itemAdded(codeGroupObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_CodeGroupAPIService.GetFilteredCodeGroups(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }



        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editCodeGroupe,
                haspermission: hasUpdateCodeGroupPermission
            }];
        }

        function hasUpdateCodeGroupPermission() {
            return WhS_BE_CodeGroupAPIService.HasUpdateCodeGroupPermission();
        }

        function editCodeGroupe(codeGroupObj) {
            var onCodeGroupUpdated = function (codeGroupObj) {
                gridAPI.itemUpdated(codeGroupObj);
            };

            WhS_BE_CodeGroupService.editCodeGroup(codeGroupObj.Entity.CodeGroupId, onCodeGroupUpdated, disabCountry);
        }

    }

    return directiveDefinitionObject;

}]);
