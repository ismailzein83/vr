//(function (app) {

//    'use strict';

//    FigurestilesettingsDefinition.$inject = ['UtilsService', 'VRUIUtilsService'];

//    function FigurestilesettingsDefinition(UtilsService, VRUIUtilsService) {

//        var directiveDefinitionObject = {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var figurestilesettingsDefinition = new FigurestilesettingsDefinition(ctrl, $scope, $attrs);
//                figurestilesettingsDefinition.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            compile: function (element, attrs) {
//                return {
//                    pre: function ($scope, iElem, iAttrs, ctrl) {

//                    }
//                };
//            },
//            templateUrl: '/Client/Modules/Common/Directives/FiguresTileSettings/Templates/FiguresTileSettingsDefinitionTemplate.html'
//        };

//        function FigurestilesettingsDefinition(ctrl, $scope, attrs) {
//            this.initializeController = initializeController;

//            var viewSelectorAPI;
//            var viewSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            var queryGridAPI;
//            var queryGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {

//                $scope.scopeModel = {};

//                $scope.scopeModel.onViewsSelectorReady = function (api) {
//                    viewSelectorAPI = api;
//                    viewSelectorReadyPromiseDeferred.resolve();
//                };

//                $scope.scopeModel.onQueriesGridReady = function (api) {
//                    queryGridAPI = api;
//                    queryGridReadyPromiseDeferred.resolve();
//                };

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];

//                    var viewSelectorPromise = loadViewSelector();
//                    promises.push(viewSelectorPromise);

//                    var gridQueriesPromise = loadQueriesGrid();
//                    promises.push(gridQueriesPromise);


//                    return UtilsService.waitMultiplePromises(promises);
//                };

//                api.getData = function () {

//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);

//                function loadViewSelector() {
//                    var viewSelectorLoadDeferred = UtilsService.createPromiseDeferred();

//                    viewSelectorReadyPromiseDeferred.promise.then(function () {
//                        var viewSelectorPayload = { selectedIds: undefined};
//                        VRUIUtilsService.callDirectiveLoad(viewSelectorAPI, viewSelectorPayload, viewSelectorLoadDeferred);
//                    });

//                    return viewSelectorLoadDeferred.promise;
//                }
//                function loadQueriesGrid() {
//                    var gridQueriesLoadDeferred = UtilsService.createPromiseDeferred();

//                    queryGridReadyPromiseDeferred.promise.then(function () {
//                        var gridQueriesPayload = {};
//                        VRUIUtilsService.callDirectiveLoad(queryGridAPI, gridQueriesPayload, gridQueriesLoadDeferred);
//                    });

//                    return gridQueriesLoadDeferred.promise;
//                }

//            }
//        }
//        return directiveDefinitionObject;
//    }

//    app.directive('vrCommonFigurestilesettingsDefinition', FigurestilesettingsDefinition);

//})(app);
