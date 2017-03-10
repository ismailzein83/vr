(function (app) {

    'use strict';

    ChildBERelationsViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'VR_GenericData_BEParentChildRelationService', 'VR_GenericData_BEParentChildRelationAPIService'];

    function ChildBERelationsViewDirective(UtilsService, VRNotificationService, VR_GenericData_BEParentChildRelationService, VR_GenericData_BEParentChildRelationAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ChildBERelationsViewCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountViews/MainExtensions/Templates/ChildBERelationsViewTemplate.html'
        };

        function ChildBERelationsViewCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var accountViewDefinition;
            var accountBEDefinitionId;
            var parentAccountId;
            var relationDefinitionId;
            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.hasAddPermission = false;
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onChildBERelationAdded = function () {
                    var onChildBERelationAdded = function (addedChildBERelation) {
                        gridAPI.onBEParentChildRelationAdded(addedChildBERelation);
                    };

                    VR_GenericData_BEParentChildRelationService.addBEParentChildRelation(onChildBERelationAdded, accountViewDefinition.Settings.BEParentChildRelationDefinitionId, parentAccountId);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    $scope.scopeModel.isGridLoading = true;

                    if (payload != undefined) {
                        accountViewDefinition = payload.accountViewDefinition;
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        parentAccountId = payload.parentAccountId;
                        relationDefinitionId = accountViewDefinition != undefined && accountViewDefinition.Settings != undefined && accountViewDefinition.Settings.BEParentChildRelationDefinitionId || undefined;
                    }
                    if (relationDefinitionId != undefined)
                        checkHasAddChildBERelationPermission(relationDefinitionId);
                    return gridAPI.load(buildGridPayload(payload)).then(function () {
                        $scope.scopeModel.isGridLoading = false;
                    });
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildGridPayload(loadPayload) {

                var payload = {
                    RelationDefinitionId: accountViewDefinition.Settings.BEParentChildRelationDefinitionId,
                    ParentBEId: parentAccountId
                };
                return payload;
            }

            function checkHasAddChildBERelationPermission(relationDefinitionId) {
                VR_GenericData_BEParentChildRelationAPIService.HasAddChildRelationPermission(relationDefinitionId).then(function (response) {
                    $scope.scopeModel.hasAddPermission = response;
                });
            }
        }
    }

    app.directive('retailBeChildberelationsView', ChildBERelationsViewDirective);

})(app);