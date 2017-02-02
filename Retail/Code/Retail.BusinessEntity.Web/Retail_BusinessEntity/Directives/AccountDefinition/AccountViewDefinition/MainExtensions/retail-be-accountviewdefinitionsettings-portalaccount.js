"use strict";

app.directive("retailBeAccountviewdefinitionsettingsPortalaccount", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new PortalAccountViewDefinitionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountViewDefinition/MainExtensions/Templates/PortalAccountViewSettingsTemplate.html"
        };

        function PortalAccountViewDefinitionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var nameAccountGenericFieldDefinitionSelectorAPI;
            var nameAccountGenericFieldDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onNameAccountGenericFieldDefinitionSelectorReady = function (api) {
                    nameAccountGenericFieldDefinitionSelectorAPI = api;
                    nameAccountGenericFieldDefinitionSelectorPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    console.log(payload);

                    var accountBEDefinitionId;
                    var name;
                    var email

                    //var beParentChildRelationDefinitionId;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        name = payload.accountViewDefinitionSettings != undefined ? payload.accountViewDefinitionSettings.Name : undefined;
                        email = payload.accountViewDefinitionSettings != undefined ? payload.accountViewDefinitionSettings.Email : undefined;

                        //beParentChildRelationDefinitionId = payload.accountViewDefinitionSettings != undefined ? payload.accountViewDefinitionSettings.BEParentChildRelationDefinitionId : undefined;
                    }

                    //Loading Name selector
                    var nameAccountGenericFieldDefinitionSelectorLoadPromise = getNameAccountGenericFieldDefinitionSelectorLoadPromise();
                    promises.push(nameAccountGenericFieldDefinitionSelectorLoadPromise);


                    function getNameAccountGenericFieldDefinitionSelectorLoadPromise() {
                        var nameAccountGenericFieldDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        nameAccountGenericFieldDefinitionSelectorPromiseDeferred.promise.then(function () {

                            var nameAccountGenericFieldDefinitionSelectorPayload = {
                                accountBEDefinitionId: accountBEDefinitionId
                            };
                            if (name != undefined) {
                                nameAccountGenericFieldDefinitionSelectorPayload.genericFieldDefinition = { Name: name }
                            };
                            VRUIUtilsService.callDirectiveLoad(nameAccountGenericFieldDefinitionSelectorAPI, nameAccountGenericFieldDefinitionSelectorPayload, nameAccountGenericFieldDefinitionSelectorLoadDeferred);
                        });

                        return nameAccountGenericFieldDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var name = nameAccountGenericFieldDefinitionSelectorAPI.getData();

                    console.log(name);

                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.AccountViews.PortalAccount, Retail.BusinessEntity.MainExtensions",
                        Name: name != undefined ? name : undefined
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