"use strict";
app.directive("cpMultinetAccountadditionalinfotileruntimesettings", ["UtilsService", "VRUIUtilsService", "CP_MultiNet_AccountAdditionalInfoAPIService",
    function (UtilsService, VRUIUtilsService, CP_MultiNet_AccountAdditionalInfoAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "=",
                title: '=',
                index:'='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountAdditionalInfoTileRuntimeSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/CP_MultiNet/Elements/AccountAdditionalInfo/Directives/Templates/AccountAdditionalInfoTileRuntimeSettings.html"
        };
        function AccountAdditionalInfoTileRuntimeSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
         
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.tileTitle = ctrl.title;
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var definitionSettings;
                    if (payload != undefined)
                    {
                        definitionSettings = payload.definitionSettings;
                    }
                    if(definitionSettings != undefined)
                    {
                        promises.push(loadFilteredDIDs());
                    }
                    function loadFilteredDIDs()
                    {
                        return CP_MultiNet_AccountAdditionalInfoAPIService.GetClientAccountAdditionalInfo(definitionSettings.VRConnectionId).then(function (response) {
                            if (response != undefined)
                            {
                                $scope.scopeModel.accountAdditionalInfo = response;
                            }
                        });
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };


        };

        return directiveDefinitionObject;
    }
]);