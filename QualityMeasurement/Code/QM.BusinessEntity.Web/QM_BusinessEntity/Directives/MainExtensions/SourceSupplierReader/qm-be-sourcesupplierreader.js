"use strict";

app.directive("qmBeSourcesupplierreader", ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'QM_BE_SupplierAPIService', function (UtilsService, VRUIUtilsService, VRNotificationService, QM_BE_SupplierAPIService) {
    
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
        templateUrl: function (element, attrs) {
         
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
       
        return "/Client/Modules/QM_BusinessEntity/Directives/MainExtensions/SourceSupplierReader/Templates/SourceSupplierReader.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        $scope.connectionString = undefined;
        var sourceTemplateDirectiveAPI ;
        var sourceDirectiveReadyPromiseDeferred =  UtilsService.createPromiseDeferred();;
        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            $scope.sourceTypeTemplates = [];
            $scope.onSourceTypeDirectiveReady = function (api) {
                sourceTemplateDirectiveAPI = api;
                sourceDirectiveReadyPromiseDeferred.resolve();
            }
            var api = {};

            api.getData = function () {
                var schedulerTaskAction;
                if ($scope.selectedSourceTypeTemplate != undefined) {
                    if (sourceTemplateDirectiveAPI != undefined) {
                        schedulerTaskAction = {};
                        schedulerTaskAction.$type = "QM.BusinessEntity.Business.SupplierSyncTaskActionArgument, QM.BusinessEntity.Business",
                        schedulerTaskAction.SourceSupplierReader = sourceTemplateDirectiveAPI.getData();
                        schedulerTaskAction.SourceSupplierReader.ConfigId = $scope.selectedSourceTypeTemplate.TemplateConfigID;
                    }
                }
                return schedulerTaskAction;
            };


            api.load = function (payload) {
                var promises = [];
                var loadSupplierSourcePromise = QM_BE_SupplierAPIService.GetSupplierSourceTemplates().then(function (response) {
                    var sourceConfigId;
                    if (payload != undefined && payload.data != undefined && payload.data.SourceSupplierReader != undefined)
                    sourceConfigId = payload.data.SourceSupplierReader.ConfigId;
                    angular.forEach(response, function (item) {
                        $scope.sourceTypeTemplates.push(item);
                    });

                    if (sourceConfigId != undefined)
                        $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "TemplateConfigID");

                });
                promises.push(loadSupplierSourcePromise);

                var loadSourceTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
                sourceDirectiveReadyPromiseDeferred.promise.then(function () {
                    var obj;
                    if (payload != undefined && payload.data != undefined && payload.data.SourceSupplierReader != undefined)
                        obj = {
                            connectionString: payload.data.SourceSupplierReader.ConnectionString
                        };
                    VRUIUtilsService.callDirectiveLoad(sourceTemplateDirectiveAPI, obj, loadSourceTemplatePromiseDeferred);
                });

                promises.push(loadSourceTemplatePromiseDeferred.promise);
                return UtilsService.waitMultiplePromises(promises);
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
