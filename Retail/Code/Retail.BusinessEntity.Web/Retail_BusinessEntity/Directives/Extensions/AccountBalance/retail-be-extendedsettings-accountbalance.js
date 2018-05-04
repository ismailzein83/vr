"use strict";

app.directive("retailBeExtendedsettingsAccountbalance", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new AccountBalanceTemplate($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Extensions/AccountBalance/Templates/AccountBalanceTemplate.html"

        };

        function AccountBalanceTemplate($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var classificationSelectorAPI;
            var classificationSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var selectedBusinessEntityDefinitionDeferred;

            var extendedSettingsEntity;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorApi = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onClassificationSelectorReady = function (api) {
                    classificationSelectorAPI = api;
                    classificationSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onBusinessEntityDefinitionSelectorChanged = function (value) {
                    if (value != undefined) {
                        if (selectedBusinessEntityDefinitionDeferred != undefined)
                            selectedBusinessEntityDefinitionDeferred.resolve();
                        else {
                            var classificationSelectorPayload = { AccountBEDefinitionId: beDefinitionSelectorApi.getSelectedIds() };
                            var setLoader = function (value) {
                                $scope.scopeModel.isClassificationSelectorLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, classificationSelectorAPI, classificationSelectorPayload, setLoader);
                        }
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    
                    var promises = [];

                    if (payload != undefined) {
                        extendedSettingsEntity = payload.extendedSettingsEntity;
                            selectedBusinessEntityDefinitionDeferred = UtilsService.createPromiseDeferred();
                            promises.push(loadClassificationSelector());
                    }
                    
                    var businessEntityDefinitionSelectorLoadPromise = getBusinessEntityDefinitionSelectorLoadPromise();
                    promises.push(businessEntityDefinitionSelectorLoadPromise);

                    function getBusinessEntityDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };
                            if (payload != undefined) {
                                selectorPayload.selectedIds = extendedSettingsEntity != undefined ? extendedSettingsEntity.AccountBEDefinitionId : undefined;
                            }
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });
                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }

                    function loadClassificationSelector() {
                        var classificationSelectorPayload;
                        var classificationSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([selectedBusinessEntityDefinitionDeferred.promise, classificationSelectorReadyDeferred.promise]).then(function () {
                            classificationSelectorPayload = {
                                AccountBEDefinitionId: extendedSettingsEntity.AccountBEDefinitionId,
                                selectedIds: getSelectedClassifications()
                            };

                            selectedBusinessEntityDefinitionDeferred = undefined;

                            var setLoader = function (value) {
                                $scope.scopeModel.isClassificationSelectorLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoad(classificationSelectorAPI, classificationSelectorPayload, classificationSelectorLoadDeferred);
                        });
                        return classificationSelectorLoadDeferred.promise;
                    }

                    function getSelectedClassifications() {
                        var classifications = extendedSettingsEntity.Classifications;
                        var classificationList = [];
                        for (var i = 0; i < classifications.length; i++) {
                            classificationList.push(classifications[i].AccountClassification);
                        }
                        return classificationList;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.Business.SubscriberAccountBalanceSetting ,Retail.BusinessEntity.Business",
                        AccountBEDefinitionId: beDefinitionSelectorApi.getSelectedIds(),
                        Classifications: getClassifications()
                    };
                };

                function getClassifications() {
                    var classifications = classificationSelectorAPI.getSelectedIds();
                    var classificationList = [];
                    for (var i = 0; i < classifications.length; i++){
                        classificationList.push({
                            AccountClassification: classifications[i]
                            });
                    }
                    return classificationList;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);