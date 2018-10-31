"use strict";

app.directive("demoModuleEmployeeWorkFulltime", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl=this;
                var ctor = new Fulltime($scope, ctrl, $attrs)

                    ctor.initializeController();
                
                },
                controllerAs:"ctrl",
                bindToController:true,
                compile: function (element, attrs) {

                },
                templateUrl: "/Client/Modules/Demo_Module/Directives/Employee/MainExtensions/Templates/FulltimeTemplate.html"
           
            
        };

        function Fulltime($scope, ctrl, $attrs)
        {
            this.initializeController = initializeController;
            var currencyDirectiveApi;
            var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
                $scope.scopeModel.onCurrencyDirectiveReady = function (api) {
                    currencyDirectiveApi = api;
                    currencyReadyPromiseDeferred.resolve();

                }
            }

            function defineAPI() {
                var api = {};
                var workPayload;
                api.load = function (payload) {
                    if (payload != undefined) {
                        workPayload = payload.workEntity;
                    }

                    if(payload!=undefined && payload.workEntity!=undefined)
                    {
                        $scope.scopeModel.TotalSalary = payload.workEntity.TotalSalary;

                    }

                    var promises = [];
                    function loadCurrencySelector() {
                        var currencyPayload = {};
                        if (workPayload != undefined)
                            currencyPayload.CurrencyID = workPayload.CurrencyId;
                        return currencyDirectiveApi.load(currencyPayload);
                    }
                    promises.push(loadCurrencySelector());

                    return UtilsService.waitMultiplePromises(promises);
                };
               
                api.getData = function () {

                    var currencyData = currencyDirectiveApi.getSelectedIds();
                    return {
                        $type: "Demo.Module.MainExtension.Employee.FullTimeWork,Demo.Module.MainExtension",
                        TotalSalary: $scope.scopeModel.TotalSalary,
                        CurrencyID: currencyData.ID,
                        CurrencyName: currencyData.Name
                    };

                };
                currencyReadyPromiseDeferred.promise.then(function (response) {

                    if(ctrl.onReady != null)
                    ctrl.onReady(api);

                });

            }
        }

        return directiveDefinitionObject;

    }
]);