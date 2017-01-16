(function (app) {

    'use strict';

    ChildBERelationsViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'VR_GenericData_BEParentChildRelationService'];

    function ChildBERelationsViewDirective(UtilsService, VRNotificationService, VR_GenericData_BEParentChildRelationService) {
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

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onChildBERelationAdded = function () {
                    var onChildBERelationAdded = function (addedChildBERelation) {
                        gridAPI.onBEParentChildRelationAdded(addedChildBERelation);
                    };

                    VR_GenericData_BEParentChildRelationService.addBEParentChildRelation(accountViewDefinition.Settings.BEParentChildRelationDefinitionId, parentAccountId, undefined, onChildBERelationAdded);
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
                    }

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
        }
    }

    app.directive('retailBeChildberelationsView', ChildBERelationsViewDirective);

})(app);