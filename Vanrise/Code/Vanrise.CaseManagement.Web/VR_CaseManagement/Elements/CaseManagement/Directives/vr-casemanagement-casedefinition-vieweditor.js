"use strict";

app.directive("vrCasemanagementCasedefinitionVieweditor", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ViewEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_CaseManagement/Elements/CaseManagement/Directives/Templates/CaseViewEditor.html"
        };
        function ViewEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var caseDefinitionSelectorApi;
            var caseDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCaseDefinitionSelectorReady = function (api) {
                    caseDefinitionSelectorApi = api;
                    caseDefinitionSelectorPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    promises.push(loadCaseDefinitionSelector());

                    function loadCaseDefinitionSelector() {
                        var caseDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        caseDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var payloadSelector = {
                                selectedIds: payload != undefined ? getVRCaseDefinitionIdsFromSettings(payload.Settings) : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(caseDefinitionSelectorApi, payloadSelector, caseDefinitionSelectorLoadDeferred);
                        });
                        return caseDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.CaseManagement.Entities.VRCaseViewSettings, Vanrise.CaseManagement.Entities",
                        Settings: buildVRCaseDefinitionViewSettings()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function buildVRCaseDefinitionViewSettings() {
                var settings = [];
                var selectedVRCaseDefinitions = caseDefinitionSelectorApi.getSelectedIds();
                if (selectedVRCaseDefinitions != undefined)
                {
                    for (var i = 0 ; i < selectedVRCaseDefinitions.length; i++) {
                        settings.push({ VRCaseDefinitionId: selectedVRCaseDefinitions[i] });
                    }
                }
                return settings;
            }
            function getVRCaseDefinitionIdsFromSettings(items) {
                var settings = [];
                for (var i = 0 ; i < items.length; i++) {
                    settings.push(items[i].VRCaseDefinitionId);
                }
                return settings;
            }


        }

        return directiveDefinitionObject;
    }
]);