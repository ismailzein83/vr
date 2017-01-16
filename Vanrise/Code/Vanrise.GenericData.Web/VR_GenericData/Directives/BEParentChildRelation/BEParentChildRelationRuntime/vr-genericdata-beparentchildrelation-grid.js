(function (app) {

    'use strict';

    BEParentChildRelationGridDirective.$inject = ['VRNotificationService', 'UtilsService', 'VR_GenericData_BEParentChildRelationAPIService', 'VR_GenericData_BEParentChildRelationService', 'VR_GenericData_BEParentChildRelationDefinitionAPIService'];

    function BEParentChildRelationGridDirective(VRNotificationService, UtilsService, VR_GenericData_BEParentChildRelationAPIService, VR_GenericData_BEParentChildRelationService, VR_GenericData_BEParentChildRelationDefinitionAPIService) {
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

            var beParentChildRelationDefinitionId = "271a98fb-0704-4519-ae0d-01969b9ac0e0";

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.ParentBENameHeaderText = "";
                $scope.scopeModel.ChildBENameHeaderText = "";
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
            }
            function defineAPI() {
                var api = {};

                api.load = function (query) {

                    var loadGridPromiseDeferred = UtilsService.createPromiseDeferred();

                    getGridColumnNames().then(function () {
                        gridAPI.retrieveData(query).then(function () {
                            loadGridPromiseDeferred.resolve();
                        }).catch(function () {
                            loadGridPromiseDeferred.reject();
                        });
                    });

                    return loadGridPromiseDeferred.promise;
                };

                api.onBEParentChildRelationAdded = function (addedBEParentChildRelation) {
                    gridAPI.itemAdded(addedBEParentChildRelation);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function getGridColumnNames() {
                return VR_GenericData_BEParentChildRelationDefinitionAPIService.GetBEParentChildRelationGridColumnNames(beParentChildRelationDefinitionId).then(function (response) {
                    $scope.scopeModel.ParentBENameHeaderText = response[0];
                    $scope.scopeModel.ChildBENameHeaderText = response[1];
                });
            }
        }
    }

    app.directive('vrGenericdataBeparentchildrelationGrid', BEParentChildRelationGridDirective);

})(app);