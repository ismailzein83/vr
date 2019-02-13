(function (app) {

    'use strict';

    FigurestilesettingsDefinition.$inject = ['UtilsService', 'VRUIUtilsService'];

    function FigurestilesettingsDefinition(UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var figurestilesettingsDefinition = new FigurestilesettingsDefinition(ctrl, $scope, $attrs);
                figurestilesettingsDefinition.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Common/Directives/FiguresTileSettings/Templates/FiguresTileSettingsDefinitionTemplate.html'
        };

        function FigurestilesettingsDefinition(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var viewSelectorAPI;
            var viewSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var queryGridAPI;
            var queryGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var viewId;
            var settings;

            var queries;
            var itemsToDisplay;

            var figureIconSelectorAPI;
            var figureIconSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};
             
                $scope.scopeModel.onViewsSelectorReady = function (api) {
                    viewSelectorAPI = api;
                    viewSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onQueriesGridReady = function (api) {
                    queryGridAPI = api;
                    queryGridReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onFigureIconSelectorReady = function (api) {
                    figureIconSelectorAPI = api;
                    figureIconSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                         settings = payload.tileExtendedSettings;
                        viewId = settings.ViewId;
                        queries = settings.Queries;
                        itemsToDisplay = settings.ItemsToDisplay;
                    }
                    var promises = [];

                    var viewSelectorPromise = loadViewSelector();
                    promises.push(viewSelectorPromise);

                    var gridQueriesPromise = loadQueriesGrid();
                    promises.push(gridQueriesPromise);

                    var figureIconPromise = loadFigureIconSelector();
                    promises.push(figureIconPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = queryGridAPI.getData();
                    return {
                        $type: 'Vanrise.Common.MainExtensions.VRTile.FiguresTileSettings,Vanrise.Common.MainExtensions',
                        ViewId: viewSelectorAPI.getSelectedIds(),
                        Queries: data.queries,
                        ItemsToDisplay: data.itemsToDisplay,
                        IconPath: figureIconSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                function loadViewSelector() {
                    var viewSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    viewSelectorReadyPromiseDeferred.promise.then(function () {
                        var viewSelectorPayload;
                        if (viewId != undefined)
                            viewSelectorPayload = {
                                selectedIds: viewId
                            };
                        VRUIUtilsService.callDirectiveLoad(viewSelectorAPI, viewSelectorPayload, viewSelectorLoadDeferred);
                    });

                    return viewSelectorLoadDeferred.promise;
                }
                function loadFigureIconSelector() {
                    var loadFigureIconSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                    figureIconSelectorReadyDeferred.promise.then(function () {
                        var figureIconSelectorPayload;
                        if (settings != undefined) {
                            figureIconSelectorPayload = {
                                selectedIds: settings.IconPath
                            };
                        };
                        VRUIUtilsService.callDirectiveLoad(figureIconSelectorAPI, figureIconSelectorPayload, loadFigureIconSelectorPromiseDeferred);
                    });
                    return loadFigureIconSelectorPromiseDeferred.promise;
                }
                function loadQueriesGrid() {
                    var gridQueriesLoadDeferred = UtilsService.createPromiseDeferred();

                    queryGridReadyPromiseDeferred.promise.then(function () {
                        var gridQueriesPayload;
                        if (queries != undefined || itemsToDisplay != undefined)
                            gridQueriesPayload = {
                                queries: queries,
                                itemsToDisplay: itemsToDisplay
                            };
                        VRUIUtilsService.callDirectiveLoad(queryGridAPI, gridQueriesPayload, gridQueriesLoadDeferred);
                    });

                    return gridQueriesLoadDeferred.promise;
                }

            }
        }
        return directiveDefinitionObject;
    }

    app.directive('vrCommonFigurestilesettingsDefinition', FigurestilesettingsDefinition);

})(app);
