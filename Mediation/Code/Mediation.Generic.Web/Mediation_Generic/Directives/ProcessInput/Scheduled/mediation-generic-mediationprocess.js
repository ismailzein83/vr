"use strict";

app.directive("mediationGenericMediationprocess", ["VRUIUtilsService", "UtilsService",
    function (VRUIUtilsService, UtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var directiveConstructor = new DirectiveConstructor($scope, ctrl);
                directiveConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: "/Client/Modules/Mediation_Generic/Directives/ProcessInput/Scheduled/Templates/MediationProcessTemplate.html"
        };

        function DirectiveConstructor($scope, ctrl) {

            this.initializeController = initializeController;
            var mediationDefinitionSelectorAPI;
            var mediationDefinitionSelectorAPIReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onMediationDefintionSelectorReady = function (api) {
                    mediationDefinitionSelectorAPI = api;
                    mediationDefinitionSelectorAPIReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};
                api.getData = function () {
                    return {
                        $type: "Mediation.Generic.BP.Arguments.MediationProcessInput, Mediation.Generic.BP.Arguments",
                        MediationDefinitionId: mediationDefinitionSelectorAPI.getSelectedIds()
                    };
                };

                api.load = function (payload) {
                    var mediationDefinitionId;
                    if (payload != undefined && payload.data != undefined)
                        mediationDefinitionId = payload.data.MediationDefinitionId;
                    var promises = [];
                    promises.push(loadMediationDefinitionSelector(mediationDefinitionId));
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadMediationDefinitionSelector(mediationDefinitionId) {
                var mediationDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                mediationDefinitionSelectorAPIReadyDeferred.promise.then(function () {
                    var payload = {
                        selectedIds: mediationDefinitionId
                    };
                    VRUIUtilsService.callDirectiveLoad(mediationDefinitionSelectorAPI, payload, mediationDefinitionSelectorLoadDeferred);
                });

                return mediationDefinitionSelectorLoadDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);
