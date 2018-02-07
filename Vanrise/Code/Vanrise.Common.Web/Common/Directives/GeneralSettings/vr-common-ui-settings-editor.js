'use strict';

app.directive('vrCommonUiSettingsEditor', ['UtilsService', 'VRUIUtilsService','VRLocalizationService',
    function (UtilsService, VRUIUtilsService, VRLocalizationService) {

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
            var languageSelectorAPI;
            var languageSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var languageId;
            $scope.scopeModel = {};
            $scope.scopeModel.onViewSelectorReady = function (api) {
                viewSelectorAPI = api;
                viewSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onPageSelectorReady = function (api) {
                pagesizeSelectorAPI = api;
                pagesizeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onLanguageSelectorReady = function (api) {
                languageSelectorAPI = api;
                languageSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.isLocalizationEnabled = VRLocalizationService.isLocalizationEnabled();
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.maxSearchRecordCount = 1000;
                    if (payload != undefined && payload.data != undefined) {
                        $scope.scopeModel.normalPrecision = payload.data.NormalPrecision;
                        $scope.scopeModel.longPrecision = payload.data.LongPrecision;
                        $scope.scopeModel.maxSearchRecordCount = payload.data.MaxSearchRecordCount;
                        languageId = payload.data.DefaultLanguageId;

                        $scope.scopeModel.horizontalLine = payload.data.HorizontalLine;
                        $scope.scopeModel.alternativeColor = payload.data.AlternativeColor;
                        $scope.scopeModel.verticalLine = payload.data.VerticalLine;

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
                    var languageSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                 

                    languageSelectorReadyDeferred.promise.then(function () {
                        promises.push(languageSelectorLoadDeferred.promise);
                        var selectorPayload = {};
                        selectorPayload.selectedIds = languageId;
                        VRUIUtilsService.callDirectiveLoad(languageSelectorAPI, selectorPayload, languageSelectorLoadDeferred);
                    });

                    return UtilsService.waitMultiplePromises(promises);
                };
               
                api.getData = function () {
                    return {
                        DefaultViewId: viewSelectorAPI.getSelectedIds(),
                        NormalPrecision: $scope.scopeModel.normalPrecision,
                        LongPrecision: $scope.scopeModel.longPrecision,
                        GridPageSize: pagesizeSelectorAPI.getSelectedIds(),
                        MaxSearchRecordCount: $scope.scopeModel.maxSearchRecordCount,
                        DefaultLanguageId: languageSelectorAPI != undefined ? languageSelectorAPI.getSelectedIds() : undefined,
                        HorizontalLine: $scope.scopeModel.horizontalLine,
                        AlternativeColor: $scope.scopeModel.alternativeColor,
                        VerticalLine: $scope.scopeModel.verticalLine
                    };
                };
              
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);