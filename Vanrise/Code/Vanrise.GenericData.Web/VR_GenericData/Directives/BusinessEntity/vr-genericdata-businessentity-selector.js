(function (app) {

    'use strict';

    BusinessEntitySelectorDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_BusinessEntityDefinitionAPIService'];

    function BusinessEntitySelectorDirective(UtilsService, VRUIUtilsService, VR_GenericData_BusinessEntityDefinitionAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BusinessEntitySelectorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrlBE",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntity/Templates/BusinessEntitySelectorTemplate.html"
        };

        function BusinessEntitySelectorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var businessEntityDefinitionId;
            var businessEntityDefinitionEntity;
            var filter;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var directivePayload = {
                        businessEntityDefinitionId: businessEntityDefinitionId,
                        filter: filter
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var selectedIds;

                    if (payload != undefined) {
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    var businessEntityDefinitionPromise = getBusinessEntityDefinition(businessEntityDefinitionId);
                    promises.push(businessEntityDefinitionPromise);

                    if (selectedIds != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function getBusinessEntityDefinition(businessEntityDefinitionId) {
                        return VR_GenericData_BusinessEntityDefinitionAPIService.GetBusinessEntityDefinition(businessEntityDefinitionId).then(function (response) {

                            businessEntityDefinitionEntity = response;

                            if (businessEntityDefinitionEntity != undefined && businessEntityDefinitionEntity.Settings != undefined)
                                $scope.scopeModel.Editor = businessEntityDefinitionEntity.Settings.SelectorUIControl;
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = {
                                businessEntityDefinitionId: businessEntityDefinitionId,
                                selectedIds: selectedIds,
                                filter: filter
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getSelectedIds = function () {
                    return directiveAPI.getSelectedIds();
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataBusinessentitySelector', BusinessEntitySelectorDirective);

})(app);