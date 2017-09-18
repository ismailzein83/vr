"use strict";
app.directive("partnerportalCustomeraccessDidtileruntimesettings", ["UtilsService", "VRUIUtilsService", "PartnerPortal_CustomerAccess_DIDAPIService","VRNotificationService",
    function (UtilsService, VRUIUtilsService, PartnerPortal_CustomerAccess_DIDAPIService, VRNotificationService) {

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
                var ctor = new DIDTileRuntimeSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/PartnerPortal_CustomerAccess/Elements/DID/Directives/Templates/DIDTileRuntimeSettings.html"
        };
        function DIDTileRuntimeSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var definitionSettings;
            var gridAPI;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.tileTitle = ctrl.title;

                $scope.dataSource = [];
                
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return PartnerPortal_CustomerAccess_DIDAPIService.GetFilteredDIDs(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined) {
                            }
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
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
                       return gridAPI.retrieveData(getQuery());
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                };
               
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };
            function getQuery()
            { 
                if(definitionSettings == undefined)
                    return;
                return {
                    VRConnectionId :definitionSettings.VRConnectionId,
                    WithSubAccounts:definitionSettings.WithSubAccounts
                };
            }

        };

        return directiveDefinitionObject;
    }
]);