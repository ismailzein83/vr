'use strict';

app.directive('retailBeTechnicalSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new technicalSettingsEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Settings/Templates/TechnicalSettingsEditorTemplate.html'
        };

        function technicalSettingsEditorCtor(ctrl, $scope, $attrs) {

            var accountGridDefinitionDirectiveAPI;
            var accountGridDefinitionDirectiveDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onAccountGridDefinition = function (api) {
                    accountGridDefinitionDirectiveAPI = api;
                    accountGridDefinitionDirectiveDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([accountGridDefinitionDirectiveDeferred.promise]).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var accountGridDefinition;

                    if (payload != undefined && payload.data != undefined) {
                        accountGridDefinition = payload.data.GridDefinition;
                    }

                    //Loading AccountGridDefinition Directive
                    var accountGridDefinitionLoadPromise = getAccountGridDefinitionLoadPromise();
                    promises.push(accountGridDefinitionLoadPromise);


                    function getAccountGridDefinitionLoadPromise(){
                        var accountGridDefitnionLoadDeferred = UtilsService.createPromiseDeferred();

                        var accountGridDefinitionPayload = {
                            accountGridDefinition: accountGridDefinition
                        };
                        VRUIUtilsService.callDirectiveLoad(accountGridDefinitionDirectiveAPI, accountGridDefinitionPayload, accountGridDefitnionLoadDeferred);

                        return accountGridDefitnionLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Retail.BusinessEntity.Entities.RetailBETechnicalSetting, Retail.BusinessEntity.Entities",
                        GridDefinition: accountGridDefinitionDirectiveAPI.getData()
                    };

                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);