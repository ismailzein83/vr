'use strict';

app.directive('whsJazzTechnicalSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new beSettingsEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/WhS_Jazz/Elements/Directives/Settings/Templates/TechnicalSettings.html'
        };

        function beSettingsEditorCtor(ctrl, $scope, $attrs) {


            var analyticTableDirectiveAPI;
            var analyticTableDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onAnalyticTableDirectiveReady = function (api) {
                    analyticTableDirectiveAPI = api;
                    analyticTableDirectiveReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([analyticTableDirectiveReadyDeferred.promise]).then(function () {
                        defineAPI();
                    });

            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    var promises = [];
                    if (payload != undefined && payload.data != undefined) {
                        selectedIds = payload.data.AnalyticTableId;
                    }


                    var analyticTableDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    analyticTableDirectiveReadyDeferred.promise.then(function () {

                        var analyticTableDirectiveReadyDeferredDirectivePayload = {
                            selectedIds: selectedIds
                        };
                        VRUIUtilsService.callDirectiveLoad(analyticTableDirectiveAPI, analyticTableDirectiveReadyDeferredDirectivePayload, analyticTableDirectiveLoadDeferred);

                    });
                   
                    promises.push(analyticTableDirectiveLoadDeferred.promise);


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Jazz.Business.JazzTechnicalSettingData, TOne.WhS.Jazz.Business",
                        AnalyticTableId: analyticTableDirectiveAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]); 