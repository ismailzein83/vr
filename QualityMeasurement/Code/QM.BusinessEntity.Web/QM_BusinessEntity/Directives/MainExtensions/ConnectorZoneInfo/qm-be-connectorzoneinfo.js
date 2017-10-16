"use strict";

app.directive("qmBeConnectorzoneinfo", ['UtilsService', 'VRUIUtilsService', 'QM_BE_ConnectorZoneAPIService',
    function (UtilsService, VRUIUtilsService, QM_BE_ConnectorZoneAPIService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: "/Client/Modules/QM_BusinessEntity/Directives/MainExtensions/ConnectorZoneInfo/Templates/ConnectorZoneInfoTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        //var sourceTypeDirectiveAPI;
        //var sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
     
        var supplierDirectiveAPI;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var profileDirectiveAPI;
        var profileReadyPromiseDeferred = UtilsService.createPromiseDeferred();



        function initializeController() {
            //$scope.sourceTypeTemplates = [];
            $scope.suppliers = [];
            $scope.selectedSupplier = [];

            //$scope.onSourceTypeDirectiveReady = function (api) {
            //    sourceTypeDirectiveAPI = api;
            //    var setLoader = function (value) { $scope.isLoadingSourceTypeDirective = value };
            //    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceTypeDirectiveAPI, undefined, setLoader, sourceDirectiveReadyPromiseDeferred);
            //}


            $scope.onProfileDirectiveReady = function (api) {
                profileDirectiveAPI = api;
                profileReadyPromiseDeferred.resolve();
            };

            $scope.onSupplierDirectiveReady = function (api) {
                supplierDirectiveAPI = api;
                supplierReadyPromiseDeferred.resolve();
            };
            defineAPI();
        }

        function defineAPI() {

            var api = {};
            api.getData = function () {
                //var CLITestConnectorObj = sourceTypeDirectiveAPI.getData();
                //CLITestConnectorObj.ConfigId = $scope.selectedSourceTypeTemplate.TemplateConfigID;
                return {
                    $type: "QM.CLITester.iTestIntegration.VIConnectorSyncTaskActionArgument, QM.CLITester.iTestIntegration",
                    SupplierId: supplierDirectiveAPI.getSelectedIds(),
                    ProfileId: profileDirectiveAPI.getSelectedIds(),
                    //CLITestConnector: CLITestConnectorObj,
                    MaximumRetryCount: $scope.maximumRetryCount,
                    ParallelThreadsCount: $scope.parallelThreadsCount,
                    DownloadResultWaitTime: $scope.downloadResultWaitTime,
                    TimeOut: $scope.timeOut
                };
            };

            api.load = function (payload) {

                var promises = [];
                if (payload != undefined && payload.data != undefined)
                    $scope.maximumRetryCount = payload.data.MaximumRetryCount;

                if (payload != undefined && payload.data != undefined)
                    $scope.timeOut = payload.data.TimeOut;

                if (payload != undefined && payload.data != undefined)
                    $scope.downloadResultWaitTime = payload.data.DownloadResultWaitTime;

                if (payload != undefined && payload.data != undefined)
                    $scope.parallelThreadsCount = payload.data.ParallelThreadsCount;

                if (payload != undefined && payload.data != undefined)
                    $scope.timeout = payload.data.TimeOut;

                var profileLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                profileReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload;
                        if (payload != undefined && payload.data != undefined)
                            directivePayload = {
                                selectedIds: payload.data.ProfileId
                            };
                        VRUIUtilsService.callDirectiveLoad(profileDirectiveAPI, directivePayload, profileLoadPromiseDeferred);
                    });

                promises.push(profileLoadPromiseDeferred.promise);

                var supplierLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                supplierReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload;
                        if (payload != undefined && payload.data != undefined)
                            directivePayload = {
                                selectedIds: payload.data.SupplierId
                            };
                        VRUIUtilsService.callDirectiveLoad(supplierDirectiveAPI, directivePayload, supplierLoadPromiseDeferred);
                    });

                promises.push(supplierLoadPromiseDeferred.promise);


                //var loadConnectorZone = QM_BE_ConnectorZoneAPIService.GetConnectorZoneTemplates().then(function (response) {
                //    var sourceConfigId;
                //    angular.forEach(response, function (item) {
                //        $scope.sourceTypeTemplates.push(item);
                //    });
                //    if (payload != undefined && payload.data != undefined && payload.data.CLITestConnector != undefined)
                //         sourceConfigId = payload.data.CLITestConnector.ConfigId;


                //    if (sourceConfigId != undefined)
                //        $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "TemplateConfigID");

                //    else 
                //        $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, $scope.sourceTypeTemplates[0].TemplateConfigID, "TemplateConfigID");

                //});


                //promises.push(loadConnectorZone);
                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
