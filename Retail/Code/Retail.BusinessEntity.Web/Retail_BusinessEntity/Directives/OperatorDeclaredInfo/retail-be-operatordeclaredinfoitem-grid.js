"use strict";

app.directive("retailBeOperatordeclaredinfoitemGrid", [
    'Retail_BE_AccountTypeService',
    'Retail_BE_ServiceTypeAPIService',
    'Retail_Be_TrafficDirectionEnum',
    'UtilsService',
    'VRNotificationService',
function (Retail_BE_AccountTypeService, Retail_BE_ServiceTypeAPIService, Retail_Be_TrafficDirectionEnum, UtilsService, VRNotificationService) {

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
            }
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/OperatorDeclaredInfo/OperatorDeclaredInfoItemGridTemplate.html'
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
            ctrl.items = [];

            ctrl.addItem = function () {
                var onOperatorDeclaredInfoItemAdded = function (obj) {
                    ctrl.items.push(obj);
                }
                Retail_BE_AccountTypeService.addOperatorDeclaredInfoItem(onOperatorDeclaredInfoItemAdded);
            }
            ctrl.removeitem = function (obj) {
                ctrl.items.splice(ctrl.items.indexOf(obj), 1);
            }

            ctrl.requiredPermissionGridMenuActions = [{
                name: 'Edit',
                clicked: editRequiredPermission
            }];
            function editRequiredPermission(obj) {
                var onOperatorDeclaredInfoItemUpdated = function (updatedObj) {
                    ctrl.items[ctrl.items.indexOf(obj)] = updatedObj;
                }
                Retail_BE_AccountTypeService.editOperatorDeclaredInfoItem(obj, onOperatorDeclaredInfoItemUpdated);
            }

            api.getData = function () {              
                return {
                    Items: ctrl.items
                }
            };


            api.load = function (payload) {
                    if (payload.Items.length > 0) {
                        return Retail_BE_ServiceTypeAPIService.GetServiceTypesInfo(UtilsService.serializetoJson({})).then(function (response) {
                            for (var i = 0; i < payload.Items; i++) {
                                var obj = payload.Items[i];
                                obj.ServiceTypeName = UtilsService.getItemByVal(response, obj.ServiceTypeId, "ServiceTypeId").Title;
                                obj.ServiceTypeName = Retail_Be_TrafficDirectionEnum[obj.TrafficDirection].description;

                                ctrl.items.push(obj);
                            }
                        });
                    }
             }
           
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }



    }

    return directiveDefinitionObject;
}]);
