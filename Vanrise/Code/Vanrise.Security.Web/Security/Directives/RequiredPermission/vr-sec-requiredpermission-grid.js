"use strict";

app.directive("vrSecRequiredpermissionGrid", ['VR_Sec_RequiredPermissionService', 'VR_Sec_BusinessEntityNodeAPIService', 'VR_Sec_BusinessEntityAPIService', 'UtilsService', 'VRNotificationService',
function (VR_Sec_RequiredPermissionService, VR_Sec_BusinessEntityNodeAPIService, VR_Sec_BusinessEntityAPIService, UtilsService, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/Security/Directives/RequiredPermission/Templates/RequiredPermissionTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        $scope.beList = [];
        var treeAPI;
        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            //RequiredPermission.Entries
            ctrl.requiredPermissions = [];
          
            ctrl.addRequiredPermission = function () {
                VR_Sec_RequiredPermissionService.addRequiredPermission(ctrl.requiredPermissions, onSavePermissionObject);
            };
            ctrl.removeRequiredPermission = function (obj) {
                ctrl.requiredPermissions.splice(ctrl.requiredPermissions.indexOf(obj), 1);
            };

            ctrl.requiredPermissionGridMenuActions = [{
                name: 'Edit',
                clicked: editRequiredPermission
            }];
            function editRequiredPermission(obj) {
                VR_Sec_RequiredPermissionService.editRequiredPermission(obj, ctrl.requiredPermissions, onSavePermissionObject);
            }

            api.getData = function () {
                var entries = [];
                for (var i = 0; i < ctrl.requiredPermissions.length; i++) {
                    entries[entries.length] = {
                        EntityId: ctrl.requiredPermissions[i].EntityId,
                        PermissionOptions: ctrl.requiredPermissions[i].PermissionOptions
                    }
                 }
                return {
                    Entries: entries
                };
            };


            api.load = function (payload) {
                if (payload != undefined && payload.data != undefined) {
                    ctrl.requiredPermissions.length = 0;
                    if (payload.data && payload.data.Entries.length > 0) {
                        var ids = [];
                        var basiclist = [];
                        for (var y = 0; y < payload.data.Entries.length; y++) {
                            var currentObj = payload.data.Entries[y];
                            ids[ids.length] = currentObj.EntityId;
                            basiclist[basiclist.length] = currentObj
                        }
                        return VR_Sec_BusinessEntityAPIService.GetBusinessEntitiesByIds(ids).then(function (response) {
                            if (response) {
                                for (var i = 0; i < response.length; i++) {
                                    ctrl.requiredPermissions.push({
                                        EntityId: response[i].EntityId,
                                        PermissionOptions: basiclist[UtilsService.getItemIndexByVal(basiclist, response[i].EntityId, "EntityId")].PermissionOptions,
                                        EntityName: response[i].Name,
                                        EntType: 1
                                    });
                                }
                            }
                        });
                    }
                }
            };
            function onSavePermissionObject(obj) {
                var permission = UtilsService.getItemByVal(ctrl.requiredPermissions, obj.EntityId, "EntityId");
                if (permission != null) {
                    ctrl.requiredPermissions[ctrl.requiredPermissions.indexOf(permission)] = obj;
                }
                else
                    ctrl.requiredPermissions.push(obj);
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }


      
    }

    return directiveDefinitionObject;
}]);
