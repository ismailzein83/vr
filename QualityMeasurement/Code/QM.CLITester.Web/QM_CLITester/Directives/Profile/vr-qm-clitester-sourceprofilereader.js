"use strict";

app.directive("vrQmClitesterSourceprofilereader", ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'QM_CLITester_ProfileAPIService', function (UtilsService, VRUIUtilsService, VRNotificationService, QM_CLITester_ProfileAPIService) {
    
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
       
        return "/Client/Modules/QM_CLITester/Directives/Profile/Templates/SourceProfileReaderDirectiveTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;
        var sourceTemplateDirectiveAPI;
        var sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();;
       
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
               
                var sourceProfileReader;
                sourceProfileReader = sourceTemplateDirectiveAPI.getData();
                sourceProfileReader.ConfigID = $scope.selectedSourceTypeTemplate.ExtensionConfigurationId;

                return {
                    $type: "QM.CLITester.Business.ProfileSyncTaskActionArgument, QM.CLITester.Business",
                    SourceProfileReader: sourceProfileReader
                };
            };


            api.load = function (payload) {
                var promises = [];
                var loadProfileSourcePromise = QM_CLITester_ProfileAPIService.GetProfileSourceTemplates().then(function (response) {
                    var sourceConfigId;
                    if (payload != undefined && payload.data != undefined && payload.data.SourceProfileReader != undefined)
                        sourceConfigId = payload.data.SourceProfileReader.ConfigId;
                    angular.forEach(response, function (item) {
                        $scope.sourceTypeTemplates.push(item);
                    });

                    if (sourceConfigId != undefined)
                        $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "ExtensionConfigurationId");

                });
                promises.push(loadProfileSourcePromise);

                var loadSourceTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
                sourceDirectiveReadyPromiseDeferred.promise.then(function () {
                    var obj;
                    if (payload != undefined && payload.data != undefined && payload.data.SourceSupplierReader != undefined)
                        obj = payload.data.SourceSupplierReader;
                    VRUIUtilsService.callDirectiveLoad(sourceTemplateDirectiveAPI, obj, loadSourceTemplatePromiseDeferred);
                });

                promises.push(loadSourceTemplatePromiseDeferred.promise);

            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
