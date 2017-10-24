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
            var gridAPI;
            var assignmentdefinitons;
            var gridReadyPromise = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    gridReadyPromise.resolve();
                };
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        gridReadyPromise.promise.then(function () {
                            var gridPayload;
                            if (payload.businessEntityDefinitionSettings != undefined)
                                gridPayload = {
                                    assignmentdefinitons: payload.businessEntityDefinitionSettings.AssignmentDefinitions
                                };
                            gridAPI.loadGrid(gridPayload);
                        });
                    }
                };
                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.AccountManager.Business.AccountManagerBEDefinitionSettings, Vanrise.AccountManager.Business",
                        AssignmentDefinitions: gridAPI.getData()
                    };

                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        
        }

        return directiveDefinitionObject;
    }]);