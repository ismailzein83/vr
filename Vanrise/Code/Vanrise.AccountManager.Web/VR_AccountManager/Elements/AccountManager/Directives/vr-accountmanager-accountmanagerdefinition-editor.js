'use strict';

app.directive('vrAccountmanagerAccountmanagerdefinitionEditor', ['UtilsService', 'VRUIUtilsService','VR_AccountManager_AccountManagerService',
function (UtilsService, VRUIUtilsService,VR_AccountManager_AccountManagerService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountManagerDefinitionEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_AccountManager/Elements/AccountManager/Directives/Template/AccountManagerDefinitionEditorTemplate.html'
        };

        function AccountManagerDefinitionEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;
            var assignmentdefinitons;

            var gridAPI;
            var gridReadyPromise = UtilsService.createPromiseDeferred();

            var subViewGridAPI;
            var subViewGridReady = UtilsService.createPromiseDeferred();

            var extensionDefinitionsSelectorAPI;
            var extensionDefinitionsSelector = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    gridReadyPromise.resolve();
                };
                $scope.onSubViewGridReady = function (api) {
                    subViewGridAPI = api;
                    subViewGridReady.resolve();
                };
                $scope.onSelectiveReady = function (api) {
                    extensionDefinitionsSelectorAPI = api;
                    extensionDefinitionsSelector.resolve();

                }
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                        gridReadyPromise.promise.then(function () {
                            var gridPayload;
                            if(payload != undefined)
                            if (payload.businessEntityDefinitionSettings != undefined)
                                gridPayload = {
                                    assignmentdefinitons: payload.businessEntityDefinitionSettings.AssignmentDefinitions
                                };
                            gridAPI.loadGrid(gridPayload);
                        });
                        subViewGridReady.promise.then(function () {
                            var gridPayload;
                            if (payload != undefined)
                                if (payload.businessEntityDefinitionSettings != undefined)
                                    gridPayload = {
                                        subViews: payload.businessEntityDefinitionSettings.SubViews
                                    };
                            subViewGridAPI.loadGrid(gridPayload);
                        });
                        extensionDefinitionsSelector.promise.then(function () {
                            if (payload != undefined)
                            {
                            var selectorPayload = {
                                extendedSettings: payload.businessEntityDefinitionSettings.ExtendedSettings
                            };
                        }
                            extensionDefinitionsSelectorAPI.load(selectorPayload);
                        })
                };
                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.AccountManager.Business.AccountManagerBEDefinitionSettings, Vanrise.AccountManager.Business",
                        AssignmentDefinitions: gridAPI.getData(),
                        SubViews: subViewGridAPI.getData(),
                        ExtendedSettings: extensionDefinitionsSelectorAPI.getData()
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        
        }

        return directiveDefinitionObject;
    }]);