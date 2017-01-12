(function (app) {

    'use strict';

    BEParentChildRelationGridDirective.$inject = ['VR_GenericData_BEParentChildRelationAPIService', 'VR_GenericData_BEParentChildRelationService', 'VRNotificationService'];

    function BEParentChildRelationGridDirective(VR_GenericData_BEParentChildRelationAPIService, VR_GenericData_BEParentChildRelationService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BEParentChildRelationGridCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BEParentChildRelation/BEParentChildRelationRuntime/Templates/BEParentChildRelationGridTemplate.html'
        };

        function BEParentChildRelationGridCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataSource = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_BEParentChildRelationAPIService.GetFilteredBEParentChildRelations(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onBEParentChildRelationAdded = function (addedBEParentChildRelation) {
                    gridAPI.itemAdded(addedBEParentChildRelation);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editBEParentChildRelation,
                    haspermission: hasEditPermission
                }];
                function editBEParentChildRelation(dataItem) {
                    var onBEParentChildRelationUpdated = function (updatedBEParentChildRelation) {
                        gridAPI.itemUpdated(updatedBEParentChildRelation);
                    };

                    VR_GenericData_BEParentChildRelationService.editBEParentChildRelation(dataItem.Entity.BEParentChildRelationId, onBEParentChildRelationUpdated);
                }
                function hasEditPermission() {
                    return VR_GenericData_BEParentChildRelationAPIService.HasEditBEParentChildRelationPermission();
                }
            }
        }
    }

    app.directive('vrGenericdataBeparentchildrelationGrid', BEParentChildRelationGridDirective);

})(app);