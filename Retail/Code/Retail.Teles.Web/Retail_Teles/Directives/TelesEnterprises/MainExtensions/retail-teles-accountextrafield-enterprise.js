'use strict';

app.directive('retailTelesAccountextrafieldEnterprise', ['UtilsService','VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new EnterpriseAccountExtraField($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Teles/Directives/TelesEnterprises/MainExtensions/Templates/EnterpriseAccountExtraFieldTemplate.html'
        };

        function EnterpriseAccountExtraField($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorApi = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};
                
                api.load = function (payload) {
                    var promises = [];
                    //Loading BusinessEntityDefinition Selector
                    var businessEntityDefinitionSelectorLoadPromise = getBusinessEntityDefinitionSelectorLoadPromise();
                    promises.push(businessEntityDefinitionSelectorLoadPromise);


                    function getBusinessEntityDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorPromiseDeferred.promise.then(function () {

                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: " Retail.Teles.Business.EnterpriseBEDefinitionFilter,  Retail.Teles.Business"
                                    }]
                                },
                                selectedIds: payload != undefined && payload.accountExtraFieldDefinitionSettings != undefined ? payload.accountExtraFieldDefinitionSettings.EnterpriseBEDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }


                    if (payload != undefined) {
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.Teles.Business.TelesEnterpriseExtraFieldDefinition, Retail.Teles.Business',
                        EnterpriseBEDefinitionId: beDefinitionSelectorApi.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);