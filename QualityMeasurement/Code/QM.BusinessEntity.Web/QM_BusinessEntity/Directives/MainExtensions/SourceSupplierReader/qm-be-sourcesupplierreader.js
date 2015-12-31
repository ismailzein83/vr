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
                
               
            };


            api.load = function (payload) {
                var promises = [];
                var loadSupplierSourcePromise = QM_BE_SupplierAPIService.GetSupplierSourceTemplates().then(function (response) {
                    if ($scope.schedulerTaskAction != undefined && $scope.schedulerTaskAction.data != undefined && $scope.schedulerTaskAction.data.SourceSupplierReader != undefined)
                        sourceConfigId = $scope.schedulerTaskAction.data.SourceSupplierReader.ConfigId;
                    angular.forEach(response, function (item) {
                        $scope.sourceTypeTemplates.push(item);
                    });

                    if (sourceConfigId != undefined)
                        $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "TemplateConfigID");

                });
                
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
