"use strict";
app.directive("partnerportalCustomeraccessAnalytictileruntimesettings", ["UtilsService", "VRUIUtilsService", "PartnerPortal_CustomerAccess_AnalyticAPIService",
    function (UtilsService, VRUIUtilsService, PartnerPortal_CustomerAccess_AnalyticAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "=",
                title:'='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AnalyticTileRuntimeSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/PartnerPortal_CustomerAccess/Elements/Analytic/Directives/Templates/AnalyticTileRuntimeSettings.html"
        };
        function AnalyticTileRuntimeSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
         
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.tileTitle = ctrl.title;
                $scope.scopeModel.fields = [];
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
                        promises.push(loadLiveBalance());
                    }
                    function loadLiveBalance()
                    {
                        return PartnerPortal_CustomerAccess_AnalyticAPIService.GetAnalyticTileInfo(definitionSettings).then(function (response) {
                            if (response != undefined && response.Fields != undefined)
                            {
                                $scope.scopeModel.fields = response.Fields;
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