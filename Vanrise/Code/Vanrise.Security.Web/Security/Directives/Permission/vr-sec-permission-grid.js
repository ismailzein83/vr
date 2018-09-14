(function (app) {

    "use strict";

    PermissionGridDirective.$inject = ["VR_Sec_PermissionAPIService", "VR_Sec_PermissionService", "VR_Sec_HolderTypeEnum", "VR_Sec_PermissionFlagEnum", "VRNotificationService", "UtilsService"];

    function PermissionGridDirective(VR_Sec_PermissionAPIService, VR_Sec_PermissionService, VR_Sec_HolderTypeEnum, VR_Sec_PermissionFlagEnum, VRNotificationService, UtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var permissionGrid = new PermissionGrid($scope, ctrl, $attrs);
                permissionGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Security/Directives/Permission/Templates/PermissionGrid.html"
        };

        function PermissionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridQuery;
            var businessEntityNode;

            function initializeController() {
                ctrl.permissions = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady && typeof ctrl.onReady == "function") {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Sec_PermissionAPIService.GetFilteredEntityPermissions(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                addPermissionDataItem(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });

                    function addPermissionDataItem(permission) {
                        permission.Entity.HolderType = permission.Entity.HolderType == VR_Sec_HolderTypeEnum.User.value ? VR_Sec_HolderTypeEnum.User.description : VR_Sec_HolderTypeEnum.Group.description;
                        permission.PermissionFlagsDescription = buildPermissionFlagsDescription(permission.Entity.PermissionFlags);

                        permission.isInherited = !(permission.Entity.EntityType == businessEntityNode.EntType && permission.Entity.EntityId.toLowerCase() == businessEntityNode.EntityId.toLowerCase());
                        permission.PermissionType = permission.isInherited ? "Inherited (" + permission.Entity.PermissionPath + ")" : "Direct";

                        ctrl.permissions.push(permission);

                        function buildPermissionFlagsDescription(permissionFlags) {
                            var allowFlags = "";
                            var denyFlags = "";

                            angular.forEach(permissionFlags, function (item) {
                                if (item.Value == VR_Sec_PermissionFlagEnum.Allow.value)
                                    allowFlags = allowFlags + item.Name + ", ";
                                else
                                    denyFlags = denyFlags + item.Name + ", ";
                            });

                            var result = "";
                            if (allowFlags.length > 0) {
                                allowFlags = allowFlags.substring(0, allowFlags.length - 2);
                                result = "Allow (" + allowFlags + ")";
                            }

                            if (denyFlags.length > 0) {
                                denyFlags = denyFlags.substring(0, denyFlags.length - 2);
                                if (allowFlags.length > 0)
                                    result = result + " | ";
                                result = result + "Deny (" + denyFlags + ")";
                            }

                            return result;
                        }
                    }
                };

                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    gridQuery = query;
                    businessEntityNode = query ? query.BusinessEntityNode : null;
                    return gridAPI.retrieveData(query);
                };

                api.onPermissionAdded = function (permissionObject) {
                    gridAPI.retrieveData(gridQuery);
                };

                return api;
            }

            function defineMenuActions() {
                var gridMenuActions = [{
                    name: "Edit",
                    clicked: editPermissions,
                    //permissions: "Root/Administration Module/System Entities:Assign Permissions"
                    haspermission: hasEditSystemEntitiesPermission
                }, {
                    name: "Delete",
                    clicked: deletePermission,
                    //permissions: "Root/Administration Module/System Entities:Assign Permissions"
                    haspermission: hasDeleteSystemEntitiesPermission
                }];

                ctrl.menuActions = function (dataItem) {
                    return dataItem.isInherited ? null : gridMenuActions;
                };
            }
            function hasEditSystemEntitiesPermission() {
                return VR_Sec_PermissionAPIService.HasUpdatePermissionsPermission();
            }

            function hasDeleteSystemEntitiesPermission() {
                return VR_Sec_PermissionAPIService.HasDeleteSystemEntitiesPermission();
            }

            function editPermissions(dataItem) {
                var onPermissionsUpdated = function (updatedPermissions) {
                    gridAPI.retrieveData(gridQuery);
                };
                
                VR_Sec_PermissionService.editPermissions
                (
                    dataItem.Entity.HolderType,
                    dataItem.Entity.HolderId,
                    dataItem.Entity.EntityType,
                    dataItem.Entity.EntityId,
                    businessEntityNode ? businessEntityNode.Name : null,
                    UtilsService.cloneObject(dataItem.Entity.PermissionFlags, true),
                    businessEntityNode.PermissionOptions,
                    onPermissionsUpdated
                );
            }

            function deletePermission(dataItem) {
                var onPermissionDeleted = function () {
                    gridAPI.itemDeleted(dataItem);
                };
                VR_Sec_PermissionService.deletePermission($scope, dataItem, onPermissionDeleted);
            }
        }

        return directiveDefinitionObject;
    }

    app.directive("vrSecPermissionGrid", PermissionGridDirective);

})(app);
