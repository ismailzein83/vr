"use strict";

app.directive("vrGenericdataGenericbusinessentityVieweditor", ["UtilsService", "VRNotificationService","VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericBusinessEntityViewEditor = new GenericBusinessEntityViewEditor($scope, ctrl, $attrs);
                genericBusinessEntityViewEditor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Templates/GenericBusinessEntityViewEditor.html"
        };
        function GenericBusinessEntityViewEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
              
                $scope.scopeModel.onDataRecordTypeSelectorDirectiveReady = function (api) {
                    beDefinitionSelectorApi = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Entities.GenericBEViewSettings, Vanrise.GenericData.Entities",
                        BusinessEntityDefinitionId: beDefinitionSelectorApi.getSelectedIds()

                    };
                };
                api.load = function (payload) {
                    var promises = [];
                    promises.push(loadBusinessEntityDefinitionSelector());
                    function loadBusinessEntityDefinitionSelector() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var payloadSelector = {
                                selectedIds: payload != undefined ? payload.BusinessEntityDefinitionId : undefined,
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.GenericData.Business.GenericBusinessEntityDefinitionFilter,Vanrise.GenericData.Business"
                                    }]
                                }
                            };
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, payloadSelector, businessEntityDefinitionSelectorLoadDeferred);
                        });
                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);