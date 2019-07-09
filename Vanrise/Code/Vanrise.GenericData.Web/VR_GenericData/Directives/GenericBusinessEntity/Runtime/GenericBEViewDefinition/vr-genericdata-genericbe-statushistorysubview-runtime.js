(function (app) {

    'use strict';

    StatusHistoryGenericBERuntimeViewDirective.$inject = ['UtilsService', 'VRNotificationService','VR_GenericData_GenericBEDefinitionAPIService'];

    function StatusHistoryGenericBERuntimeViewDirective(UtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new StatusHistoryGenericBERuntimeViewCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/GenericBEViewDefinition/Templates/StatusHistoryGenericBERuntimeViewTemplate.html'
        };

        function StatusHistoryGenericBERuntimeViewCtor($scope, ctrl) {
            this.initializeController = initializeController;

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
                    var gridPayload = {};

                    var businessEntityDefinitionId;
                    var businessEntityId;
                    var genericBEGridView;
                    var parentBEEntity;
					var statusMappingFiled;

					if (payload != undefined) {
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
						genericBEGridView = payload.genericBEGridView; 
						businessEntityId = payload.genericBusinessEntityId;
                        parentBEEntity = payload.parentBEEntity;
                        if (genericBEGridView != undefined && genericBEGridView.Settings != undefined && parentBEEntity != undefined && parentBEEntity.FieldValues != undefined) {
							statusMappingFiled = genericBEGridView.Settings.StatusMappingFiled;
                        }
                    }
                    gridPayload.query = {
                        BusinessEntityDefinitionId: businessEntityDefinitionId,
						BusinessEntityId: businessEntityId,
						FieldName: statusMappingFiled
                    };
                    return gridAPI.loadGrid(gridPayload).then(function () {
                        $scope.scopeModel.isGridLoading = false;
                    });
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }   

    app.directive('vrGenericdataGenericbeStatushistorysubviewRuntime', StatusHistoryGenericBERuntimeViewDirective);

})(app);