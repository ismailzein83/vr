'use strict';

app.directive('retailBeAccountmanagerAccountsubviewdefinition', ['UtilsService', 'VRUIUtilsService', 'VRNavigationService',
function (UtilsService, VRUIUtilsService, VRNavigationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new AccountSubViewsDefinitionCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Extensions/AccountManager/Templates/AccountSubViewDefinitionTemplate.html"
    };

    function AccountSubViewsDefinitionCtor(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;

        var context;
        var assignmentDefinitionId;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.assignmentDefinitions = [];
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    context = payload.context;
                    if (payload.assignmentDefinitionId != undefined) {

                        assignmentDefinitionId = payload.assignmentDefinitionId;
                    }
                }
                if (context != undefined) {
                    for (var i = 0 ; i < context.length; i++) {
                        $scope.scopeModel.assignmentDefinitions.push(context[i]);
                    }
                    if (assignmentDefinitionId != undefined) {
                        $scope.scopeModel.selectedAssignmentDefinition = UtilsService.getItemByVal($scope.scopeModel.assignmentDefinitions, assignmentDefinitionId, 'AccountManagerAssignementDefinitionId');
                    }

                }
                return UtilsService.waitMultiplePromises(promises);

            };
            api.getData = function () {
                var obj = {
                    $type: "Retail.BusinessEntity.Business.RetailAccountAccountManagerSubViewDefinition,Retail.BusinessEntity.Business",
                    AccountManagerAssignementDefinitionId: $scope.scopeModel.selectedAssignmentDefinition.AccountManagerAssignementDefinitionId
                };

                return obj;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

    }

    return directiveDefinitionObject;
}]);