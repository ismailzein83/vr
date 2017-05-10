"use strict";
app.directive("partnerportalCustomeraccessAnalytictiledefinitionsettings", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AnalyticTileDefinitionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/PartnerPortal_CustomerAccess/Elements/Analytic/Directives/Templates/AnalyticTileDefinitionSettings.html"
        };
        function AnalyticTileDefinitionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var analyticQueriesApi;
            var analyticQueriesPromiseDeferred = UtilsService.createPromiseDeferred();
           
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onAnalyticQueriesReady = function (api) {
                    analyticQueriesApi = api;
                    analyticQueriesPromiseDeferred.resolve();
                };
              
                UtilsService.waitMultiplePromises([analyticQueriesPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var tileExtendedSettings;
                    var invoiceViewerTypeIds;
                    if (payload != undefined) {
                        tileExtendedSettings = payload.tileExtendedSettings;
                     
                    }

                    function loadAnalyticQueries()
                    {
                        var payloadAnalyticQueries = {};
                        if (tileExtendedSettings != undefined) {
                            payloadAnalyticQueries.orderedMeasuresIds = tileExtendedSettings.OrderedMeasureIds;
                            payloadAnalyticQueries.analyticQueries = tileExtendedSettings.Queries;
                        };
                        return analyticQueriesApi.load(payloadAnalyticQueries);
                    }
                 
                    promises.push(loadAnalyticQueries());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var queriesData =  analyticQueriesApi.getData();
                    return {
                        $type: "PartnerPortal.CustomerAccess.Business.AnalyticDefinitionSettings, PartnerPortal.CustomerAccess.Business",
                        Queries: queriesData != undefined ? queriesData.Queries : undefined,
                        OrderedMeasureIds: queriesData != undefined ? queriesData.OrderedMeasureIds : undefined,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };


        };

        return directiveDefinitionObject;
    }
]);