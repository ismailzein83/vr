'use strict';

app.directive('vrCommonNationalSettingsEditor', ['UtilsService', 'VRUIUtilsService',
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
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/Settings/NationalSettings/Templates/NationalSettings.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {
            var countryDirectiveApi;
            var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };

            function initializeController() {
                $scope.scopeModel = {};

                countryReadyPromiseDeferred.promise.then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    var countryPayload;
                    if (payload != undefined && payload.data != undefined) {
                        countryPayload = { selectedIds: payload.data.NationalCountries };
                    }

                    VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, countryPayload, countryLoadPromiseDeferred);

                    return countryLoadPromiseDeferred.promise;
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Entities.NationalSettings, Vanrise.Entities",
                        nationalCountries: countryDirectiveApi.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);