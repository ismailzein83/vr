"use strict";

app.directive("vrSecBusinessentityGrid", ['VRNotificationService', 'VR_Sec_BusinessEntityAPIService', 'VR_Sec_BusinessEntityDefinitionService', function (VRNotificationService, VR_Sec_BusinessEntityAPIService, VR_Sec_BusinessEntityDefinitionService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var entitiesGrid = new EntitiesGrid($scope, ctrl, $attrs);
            entitiesGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Security/Directives/BusinessEntity/Templates/BusinessEntityGridTemplate.html"

    };

    function EntitiesGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.entities = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onBusinessEntityAdded = function (businessEntity) {
                        gridAPI.itemAdded(businessEntity);
                    };
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Sec_BusinessEntityAPIService.GetFilteredBusinessEntities(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editBusinessEntity,
                haspermission: hasUpdateBusinessEntityPermission // System Entities:Assign Permissions
            }];
        }
        function hasUpdateBusinessEntityPermission() {
            return VR_Sec_BusinessEntityAPIService.HasUpdateBusinessEntityPermission();
        }
        function editBusinessEntity(businessEntityObj) {
            var onBusinessEntityUpdated = function (businessEntity) {
                gridAPI.itemUpdated(businessEntity);
            };

            VR_Sec_BusinessEntityDefinitionService.updateBusinessEntityDefinition(businessEntityObj.Entity.EntityId, onBusinessEntityUpdated);
        }
    }

    return directiveDefinitionObject;

}]);