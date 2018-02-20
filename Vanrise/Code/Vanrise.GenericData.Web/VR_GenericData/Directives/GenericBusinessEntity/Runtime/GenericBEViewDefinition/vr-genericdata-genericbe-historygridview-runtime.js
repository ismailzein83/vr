(function (app) {

    'use strict';

    HistoryGenericBEDefinitionViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'VR_GenericData_GenericBusinessEntityService'];

    function HistoryGenericBEDefinitionViewDirective(UtilsService, VRNotificationService, VR_GenericData_GenericBusinessEntityService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HistoryGenericBEDefinitionViewCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/GenericBEViewDefinition/Templates/HistoryGenericBEDefinitionViewTemplate.html'
        };

        function HistoryGenericBEDefinitionViewCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var businessEntityDefinitionId;
            var genericBusinessEntityId;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };


            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    $scope.scopeModel.isGridLoading = true;

                    if (payload != undefined) {
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        genericBusinessEntityId = payload.genericBusinessEntityId;
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
                return {
                    ObjectId: genericBusinessEntityId,
                    EntityUniqueName: VR_GenericData_GenericBusinessEntityService.getEntityUniqueName(businessEntityDefinitionId)
                };
            }
        }
    }

    app.directive('vrGenericdataGenericbeHistorygridviewRuntime', HistoryGenericBEDefinitionViewDirective);

})(app);