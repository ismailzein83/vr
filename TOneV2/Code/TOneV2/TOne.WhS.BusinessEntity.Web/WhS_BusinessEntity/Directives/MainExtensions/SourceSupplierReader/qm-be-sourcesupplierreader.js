"use strict";

app.directive("qmBeSourcesupplierreader", ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'Whs_BE_SupplierAPIService',
    function (UtilsService, VRUIUtilsService, VRNotificationService, Whs_BE_SupplierAPIService) {
    
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
            }
        },
        templateUrl: "/Client/Modules/Whs_BusinessEntity/Directives/MainExtensions/SourceSupplierReader/Templates/SourceSupplierReader.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        var sourceTemplateDirectiveAPI;
        var sourceDirectiveReadyPromiseDeferred;

        function initializeController() {
            $scope.sourceTypeTemplates = [];
            $scope.onSourceTypeDirectiveReady = function (api) {
                sourceTemplateDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingSourceTypeDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceTemplateDirectiveAPI, undefined, setLoader, sourceDirectiveReadyPromiseDeferred);
            }

            defineAPI();
        }

        function defineAPI() {
           
            var api = {};

            api.getData = function () {
                var schedulerTaskAction;
                if ($scope.selectedSourceTypeTemplate != undefined) {
                    if (sourceTemplateDirectiveAPI != undefined) {
                        schedulerTaskAction = {};
                        schedulerTaskAction.$type = "TOne.WhS.DBSync.Business.SupplierSyncTaskActionArgument, TOne.WhS.DBSync.Business",
                        schedulerTaskAction.SourceSupplierReader = sourceTemplateDirectiveAPI.getData();
                        schedulerTaskAction.SourceSupplierReader.ConfigId = $scope.selectedSourceTypeTemplate.TemplateConfigID;
                    }
                }
                console.log('schedulerTaskAction')
                console.log(schedulerTaskAction)

                console.log('sourceTemplateDirectiveAPI.getData()')
                console.log(sourceTemplateDirectiveAPI.getData())


                return schedulerTaskAction;
            };

            api.load = function (payload) {
                var promises = [];
                var sourceConfigId;

                if (payload != undefined && payload.data != undefined && payload.data.SourceSupplierReader != undefined) {
                    sourceConfigId = payload.data.SourceSupplierReader.ConfigId;
                }

                var loadSupplierSourcePromise = Whs_BE_SupplierAPIService.GetSupplierSourceTemplates().then(function (response) {
                    
                    angular.forEach(response, function (item) {
                        $scope.sourceTypeTemplates.push(item);
                    });

                    if (sourceConfigId != undefined)
                        $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "TemplateConfigID");

                });
                promises.push(loadSupplierSourcePromise);

                if (sourceConfigId != undefined)
                {
                    sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    var loadSourceTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
                    sourceDirectiveReadyPromiseDeferred.promise.then(function () {
                        var sourceSupplierReaderPayload;

                        if (payload != undefined && payload.data != undefined && payload.data.SourceSupplierReader != undefined) {
                            sourceSupplierReaderPayload = {
                                connectionString: payload.data.SourceSupplierReader.ConnectionString
                            };
                        }

                        VRUIUtilsService.callDirectiveLoad(sourceTemplateDirectiveAPI, sourceSupplierReaderPayload, loadSourceTemplatePromiseDeferred);
                    });

                    promises.push(loadSourceTemplatePromiseDeferred.promise);
                }
                
                return UtilsService.waitMultiplePromises(promises);
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);
