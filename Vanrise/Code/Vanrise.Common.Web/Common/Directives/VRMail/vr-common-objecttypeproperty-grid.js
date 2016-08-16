'use strict';

app.directive('vrCommonObjecttypepropertyGrid', ['VRCommon_VRObjectTypeDefinitionAPIService', 'VRNotificationService', function (VRCommon_VRObjectTypeDefinitionAPIService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var objectTypePropertyGrid = new ObjectTypePropertyGrid($scope, ctrl, $attrs);
            objectTypePropertyGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Common/Directives/VRMail/Templates/VRObjectTypePropertyGridTemplate.html'
    };

    function ObjectTypePropertyGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;
        var objectVariable;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.objectTypeProperties = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            defineMenuActions();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                if (payload != undefined)
                    objectVariable = payload.objectVariable;

                return VRCommon_VRObjectTypeDefinitionAPIService.GetVRObjectTypeDefinition(objectVariable.VRObjectTypeDefinitionId).then(function (response) {

                    var objectTypeDefinition = response;
                    var properties = objectTypeDefinition.Settings.Properties;
                    var property;
                    for (var i = 0; i < properties.length; i++) {
                        property = properties[i];
                        extendVariableObject(property);
                        $scope.scopeModel.objectTypeProperties.push(property);
                    }
                });
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function extendVariableObject(property) {

            property.ValueExpression = "@Model.GetVal(\"" + objectVariable.ObjectName + "\",\"" + property.Name + "\")";
        }

        function defineMenuActions() {
            $scope.scopeModel.menuActions = [];
        }
    }
}]);
