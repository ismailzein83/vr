"use strict";

app.directive("retailBeAccountviewdefinitionsettingsChildberelation", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountBEDefinitionViewEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountViewDefinition/MainExtensions/Templates/ChildBERelationViewSettingsTemplate.html"
        };
        function AccountBEDefinitionViewEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beParentChildRelationDefinitionSelectorAPI;
            var beParentChildRelationDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBEParentChildRelationDefinitionSelectorReady = function (api) {
                    beParentChildRelationDefinitionSelectorAPI = api;
                    beParentChildRelationDefinitionSelectorPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var beParentChildRelationDefinitionId;

                    if (payload != undefined && payload.accountViewDefinitionSettings != undefined) {
                        beParentChildRelationDefinitionId = payload.accountViewDefinitionSettings.BEParentChildRelationDefinitionId;
                    }

                    //Loading BEParentChildRelationDefinition selector
                    var beParentChildRelationDefinitionSelectorLoadPromise = getBEParentChildRelationDefinitionSelectorLoadPromise();
                    promises.push(beParentChildRelationDefinitionSelectorLoadPromise);


                    function getBEParentChildRelationDefinitionSelectorLoadPromise() {
                        var beParentChildRelationDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beParentChildRelationDefinitionSelectorPromiseDeferred.promise.then(function () {

                            var selectorPayload;
                            if (payload != undefined) {
                                selectorPayload = {
                                    selectedIds: beParentChildRelationDefinitionId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(beParentChildRelationDefinitionSelectorAPI, selectorPayload, beParentChildRelationDefinitionSelectorLoadDeferred);
                        });

                        return beParentChildRelationDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.AccountViews.ChildBERelation, Retail.BusinessEntity.MainExtensions",
                        BEParentChildRelationDefinitionId: beParentChildRelationDefinitionSelectorAPI.getSelectedIds()
                    };

                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);