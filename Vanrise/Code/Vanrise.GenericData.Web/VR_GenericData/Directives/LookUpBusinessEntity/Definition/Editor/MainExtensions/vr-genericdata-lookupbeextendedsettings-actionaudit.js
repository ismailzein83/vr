"use strict";

app.directive("vrGenericdataLookupbeextendedsettingsActionaudit", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_GenericData_ActionAuditLKUPTypeEnum",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_GenericData_ActionAuditLKUPTypeEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ActionAuditExtendedSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/LookUpBusinessEntity/Definition/Editor/MainExtensions/Templates/ActionAuditExtendedSettingsTemplate.html"

        };

        function ActionAuditExtendedSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;

            var actionAuditLKUPTypeSelectorAPI;
            var actionAuditLKUPTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.lookUpTypes = UtilsService.getArrayEnum(VR_GenericData_ActionAuditLKUPTypeEnum);

                $scope.scopeModel.onLookUpTypeSelectorReady = function (api) {
                    actionAuditLKUPTypeSelectorAPI = api;
                    actionAuditLKUPTypeSelectorReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([actionAuditLKUPTypeSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    return {
                        $type: "Vanrise.Common.MainExtensions.VRActionAuditLKUPBEDefinitionSettings, Vanrise.Common.MainExtensions",
                        Type: $scope.scopeModel.selectedLookUpType.value
                    };
                };

                api.load = function (payload) {
                    var promises = [];
                    var businessEntityDefinitionId;
                    if (payload != undefined) {
                        if (payload.settings != undefined) {
                            $scope.scopeModel.selectedLookUpType = UtilsService.getItemByVal($scope.scopeModel.lookUpTypes, payload.settings.Type, "value");
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);