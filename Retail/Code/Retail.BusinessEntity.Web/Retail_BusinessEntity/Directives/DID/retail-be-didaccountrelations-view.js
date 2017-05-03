(function (app) {

    'use strict';

    DIDAccountBERelationsViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'VR_GenericData_BEParentChildRelationService', 'Retail_BE_DIDAPIService'];

    function DIDAccountBERelationsViewDirective(UtilsService, VRNotificationService, VR_GenericData_BEParentChildRelationService, Retail_BE_DIDAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DIDAccountBERelationsViewCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/DID/Templates/DIDAccountRelationsViewTemplate.html'
        };

        function DIDAccountBERelationsViewCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var didId, relationDefinitionId;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.showAddButton = true;
                $scope.scopeModel.hasAddPermission = true; //TODO: Security

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onParentBERelationAdded = function () {
                    var onParentBERelationAdded = function (addedParentBERelation) {
                        Retail_BE_DIDAPIService.IsDIDAssignedToParentWithoutEED(relationDefinitionId, didId).then(function (response) {
                            $scope.scopeModel.showAddButton = !response;
                        });

                        gridAPI.onBEParentChildRelationAdded(addedParentBERelation);
                    };

                    VR_GenericData_BEParentChildRelationService.addBEParentChildRelation(onParentBERelationAdded, relationDefinitionId, undefined, didId);
                };

                $scope.scopeModel.onAddOrEditAccountDIDRelation = function () {
                    Retail_BE_DIDAPIService.IsDIDAssignedToParentWithoutEED(relationDefinitionId, didId).then(function (response) {
                        $scope.scopeModel.showAddButton = !response;
                    });
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        didId = payload.didId;
                    }

                    var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                    Retail_BE_DIDAPIService.GetAccountDIDRelationDefinitionId().then(function (response) {
                        relationDefinitionId = response;

                        var isDIDAssignedToParentWithoutEEDPromise = Retail_BE_DIDAPIService.IsDIDAssignedToParentWithoutEED(relationDefinitionId, didId).then(function (response) {
                            $scope.scopeModel.showAddButton = !response;
                        });

                        var payload = {
                            RelationDefinitionId: relationDefinitionId,
                            ChildBEId: didId
                        };
                        var gridLoadPromise = gridAPI.load(payload);

                        UtilsService.waitMultiplePromises([isDIDAssignedToParentWithoutEEDPromise, gridLoadPromise]).then(function () {
                            loadDirectivePromiseDeferred.resolve();
                        });
                    });

                    return loadDirectivePromiseDeferred.promise;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeDidaccountrelationsView', DIDAccountBERelationsViewDirective);

})(app);