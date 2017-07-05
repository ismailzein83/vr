'use strict';

app.directive('retailZajilDidConvertorEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailZajilDidConvertorEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Zajil/Directives/MainExtensions/DID/Templates/DIDConvertorEditor.html"
        };

        function retailZajilDidConvertorEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var accountDefinitionSelectorApi;
            var accountDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();


            $scope.scopeModel = {};

            $scope.scopeModel.onAccountDefinitionSelectorReady = function (api) {
                accountDefinitionSelectorApi = api;
                accountDefinitionSelectorPromiseDeferred.resolve();
            };

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        $scope.scopeModel.bedColumn = payload.BEDColumn;
                        $scope.scopeModel.accountColumn = payload.SourceAccountIdColumn;
                        $scope.scopeModel.sourceIdColumn = payload.SourceIdColumn;
                        $scope.scopeModel.internationalColumn = payload.InternationalColumn;
                        $scope.scopeModel.didColumn = payload.DIDColumn;
                        $scope.scopeModel.didSoColumn = payload.DIDSoColumn;
                        $scope.scopeModel.NbOfChannelsColumn = payload.NumberOfChannelsColumn;

                    }

                    var promises = [];
                    var loadAccountDefinitionTypePromise = getAccountDefinitionSelectorLoadPromise();
                    promises.push(loadAccountDefinitionTypePromise);

                    function getAccountDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        accountDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };

                            if (payload != undefined) {
                                selectorPayload.selectedIds = payload.AccountBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(accountDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    };

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Retail.Zajil.MainExtensions.Convertors.DIDConvertor, Retail.Zajil.MainExtensions",
                        Name: "Zajil DID Convertor",
                        DIDColumn: $scope.scopeModel.didColumn,
                        SourceAccountIdColumn: $scope.scopeModel.accountColumn,
                        SourceIdColumn: $scope.scopeModel.sourceIdColumn,
                        BEDColumn: $scope.scopeModel.bedColumn,
                        AccountBEDefinitionId: accountDefinitionSelectorApi.getSelectedIds(),
                        InternationalColumn: $scope.scopeModel.internationalColumn,
                        DIDSoColumn: $scope.scopeModel.didSoColumn,
                        NumberOfChannelsColumn: $scope.scopeModel.NbOfChannelsColumn
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);