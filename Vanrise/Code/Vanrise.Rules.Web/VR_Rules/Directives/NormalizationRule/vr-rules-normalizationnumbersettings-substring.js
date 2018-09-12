"use strict";

app.directive("vrRulesNormalizationnumbersettingsSubstring", ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var ctor = new DirectiveConstructor($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/VR_Rules/Directives/NormalizationRule/Templates/SubstringDirectiveTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        var substringStartDirectionSelectorAPI;
        var substringStartDirectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        ctrl.startIndex = undefined;
        ctrl.length = undefined;

        function initializeController() {

            ctrl.onSubstringStartDirectionSelectorReady = function (api) {
                substringStartDirectionSelectorAPI = api;
                substringStartDirectionSelectorReadyDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                var startDirection;

                if (payload != undefined) {
                    ctrl.startIndex = payload.StartIndex;
                    ctrl.length = payload.Length;
                    startDirection = payload.StartDirection;
                }

                var substringStartDirectionSelectorLoadPromise = loadSubstringStartDirectionSelector();
                promises.push(substringStartDirectionSelectorLoadPromise);

                function loadSubstringStartDirectionSelector() {
                    var substringStartDirectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    substringStartDirectionSelectorReadyDeferred.promise.then(function () {

                        var substringStartDirectionSelectorPayload = { selectFirstItem: true };
                        if (startDirection != undefined) {
                            substringStartDirectionSelectorPayload.selectedIds = startDirection;
                        }
                        VRUIUtilsService.callDirectiveLoad(substringStartDirectionSelectorAPI, substringStartDirectionSelectorPayload, substringStartDirectionSelectorLoadDeferred);
                    });

                    return substringStartDirectionSelectorLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
                return {
                    $type: "Vanrise.Rules.Normalization.MainExtensions.SubstringActionSettings, Vanrise.Rules.Normalization",
                    StartDirection: substringStartDirectionSelectorAPI.getSelectedIds(),
                    StartIndex: ctrl.startIndex,
                    Length: ctrl.length
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
