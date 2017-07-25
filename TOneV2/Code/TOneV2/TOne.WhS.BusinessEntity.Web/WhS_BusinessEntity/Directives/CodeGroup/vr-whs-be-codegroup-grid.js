"use strict";

app.directive("vrWhsBeCodegroupGrid", ["UtilsService", "VRNotificationService", "WhS_BE_CodeGroupAPIService", "WhS_BE_CodeGroupService", "VRUIUtilsService", "WhS_BE_SaleCodeAPIService",
function (UtilsService, VRNotificationService, WhS_BE_CodeGroupAPIService, WhS_BE_CodeGroupService, VRUIUtilsService, WhS_BE_SaleCodeAPIService) {

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
        var gridDrillDownTabsObj;
        var codes;
        var menuActions;
        this.initializeController = initializeController;

        function initializeController() {
            if ($attrs.hidecountrycolumn != undefined) {
                $scope.hidecountrycolumn = disabCountry = true;
            }
            $scope.codegroups = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var drillDownDefinitions = WhS_BE_CodeGroupService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.menuActions, true);
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onCodeGroupAdded = function (codeGroupObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(codeGroupObject);
                        gridAPI.itemAdded(codeGroupObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_CodeGroupAPIService.GetFilteredCodeGroups(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                               
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }


        function defineMenuActions() {
            
            $scope.gridMenuActions = function (dataItem) {

                var menuItems = [] ;
             
                if(dataItem.AllowEdit ==  true)
                    menuItems[menuItems.length] = {
                        name: "Edit",
                        clicked: editCodeGroupe,
                        haspermission: hasUpdateCodeGroupPermission

                    }

                return menuItems; 
            };


            
        }
        
        function hasUpdateCodeGroupPermission() {
            return WhS_BE_CodeGroupAPIService.HasUpdateCodeGroupPermission();
        }

        function editCodeGroupe(codeGroupObj) {
            var onCodeGroupUpdated = function (codeGroupObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(codeGroupObj);
                gridAPI.itemUpdated(codeGroupObj);
            };

            WhS_BE_CodeGroupService.editCodeGroup(codeGroupObj.Entity.CodeGroupId, onCodeGroupUpdated, disabCountry);
        }

    }

    return directiveDefinitionObject;

}]);
