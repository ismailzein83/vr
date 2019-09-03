(function (app) {

    'use strict';

    GenericBE360DegreeViewer.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBEDefinitionAPIService'];

    function GenericBE360DegreeViewer(UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBE360DegreeViewer($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,

            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/Management/Templates/GenericBE360DegreeViewerTemplate.html"
        };

        function GenericBE360DegreeViewer($scope, ctrl) {
            this.initializeController = initializeController;

            var idFieldName;
            var businessEntityDefinitionId;
            var genericBEDefinitionSettings;
            

            var gridDirectiveAPI;
            var gridDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridDirectiveReady = function (api) {
                    gridDirectiveAPI = api;
                    gridDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                       var parentBusinessEntityDefinitionId = payload.parentBusinessEntityDefinitionId;
                       var genericBusinessEntityId = payload.genericBusinessEntityId;
                       var fieldName = payload.fieldName;
                       var modalContext = payload.modalContext;
                        var hideCloseButton = payload.hideCloseButton;
                    }

                    var getGenericBEField360DegreeViewDataPromise = getGenericBEField360DegreeViewData();
                    initialPromises.push(getGenericBEField360DegreeViewDataPromise);

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [loadGrid()];
                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    function getGenericBEField360DegreeViewData() {
                        var getGenericBEField360DegreeViewDataLoadDeferred = UtilsService.createPromiseDeferred();

                        var input = {
                            ParentBusinessEntityDefinitionId: parentBusinessEntityDefinitionId,
                            FieldName: fieldName
                        };

                        VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEField360DegreeViewData(input).then(function (response) {
                            if (response) {
                                businessEntityDefinitionId = response.BusinessEntityDefinitionId;
                                idFieldName = response.IdFieldName;
                                genericBEDefinitionSettings = response.GenericBEDefinitionSettings;
                            }

                            getGenericBEField360DegreeViewDataLoadDeferred.resolve();
                        });

                        return getGenericBEField360DegreeViewDataLoadDeferred.promise;
                    }

                    function loadGrid() {
                        var gridDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        gridDirectiveReadyDeferred.promise.then(function () {
                            var gridPayload = {
                                query: {
                                    FilterGroup: undefined,
                                    Filters: [{ FieldName: idFieldName, FilterValues: [genericBusinessEntityId] }],
                                    OrderType: genericBEDefinitionSettings.OrderType,
                                    AdvancedOrderOptions: genericBEDefinitionSettings.AdvancedOrderOptions
                                },
                                businessEntityDefinitionId: businessEntityDefinitionId,
                                modalContext: modalContext,
                                fullScreenModeOn: true,
                                hideCloseButton: hideCloseButton
                            };

                            VRUIUtilsService.callDirectiveLoad(gridDirectiveAPI, gridPayload, gridDirectiveLoadDeferred);
                        });

                        return gridDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbusinessentity360degreeViewer', GenericBE360DegreeViewer);

})(app);