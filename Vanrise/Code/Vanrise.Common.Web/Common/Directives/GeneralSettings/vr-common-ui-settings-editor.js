'use strict';

app.directive('vrCommonUiSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/GeneralSettings/Templates/UITemplateSettings.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {
            var viewSelectorAPI;
            var viewSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var pagesizeSelectorAPI;
            var pagesizeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            $scope.scopeModel = {};
            $scope.scopeModel.onViewSelectorReady = function (api) {
                viewSelectorAPI = api;
                viewSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onPageSelectorReady = function (api) {
                pagesizeSelectorAPI = api;
                pagesizeSelectorReadyDeferred.resolve();
            };
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.data != undefined) {
                        $scope.scopeModel.normalPrecision = payload.data.NormalPrecision;
                        $scope.scopeModel.longPrecision = payload.data.LongPrecision;

                    }

                    var promises = [];

                    var viewSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(viewSelectorLoadDeferred.promise);

                    viewSelectorReadyDeferred.promise.then(function () {
                        var selectorPayload = {
                            selectedIds:  payload != undefined && payload.data != undefined ? payload.data.DefaultViewId : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(viewSelectorAPI, selectorPayload, viewSelectorLoadDeferred);
                    });

                    var pagesizeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(pagesizeSelectorLoadDeferred.promise);

                    pagesizeSelectorReadyDeferred.promise.then(function () {
                        var pageSizePayload = {
                            selectedIds: payload != undefined && payload.data != undefined ? payload.data.GridPageSize : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(pagesizeSelectorAPI, pageSizePayload, pagesizeSelectorLoadDeferred);
                    });


                    return UtilsService.waitMultiplePromises(promises);
                };
               
                api.getData = function () {
                    return {
                        DefaultViewId: viewSelectorAPI.getSelectedIds(),
                        NormalPrecision: $scope.scopeModel.normalPrecision,
                        LongPrecision: $scope.scopeModel.longPrecision,
                        GridPageSize: pagesizeSelectorAPI.getSelectedIds()
                    };
                };
              
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);