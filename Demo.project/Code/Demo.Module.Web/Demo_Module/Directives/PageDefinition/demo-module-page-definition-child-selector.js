"use strict"
app.directive("demoModulePageDefinitionChildSelector", ["UtilsService", "VRNotificationService", "Demo_Module_PageDefinitionService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_PageDefinitionService, VRUIUtilsService, VRCommon_ObjectTrackingService) {
    
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var pageDefinitionChildSelector = new PageDefinitionChildSelector($scope, ctrl, $attrs);
            pageDefinitionChildSelector.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/PageDefinition/Templates/PageDefinitionChildSelectorTemplate.html"
    };

    function PageDefinitionChildSelector($scope, ctrl) {

        var subviewDirectiveApi;
        var fieldDirectiveApi;
        var subviewSettingsEntity;
        var directiveSelectedPromiseDeferred;
        var subviewSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var fieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        $scope.scopeModel = {};
        this.initializeController = initializeController;

        function initializeController() {
            
            $scope.scopeModel.onSubviewSelectorReady = function (api) {
                   
                subviewDirectiveApi=api;
                subviewSelectorReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.onFieldSelectorReady=function(api){

                fieldDirectiveApi = api;
                fieldSelectorReadyPromiseDeferred.resolve();
                subviewSelectorReadyPromiseDeferred.promise.then(function (response) {
                    defineAPI();
                });
            }

            $scope.scopeModel.onSubviewDirectiveChanged = function () {

                if (subviewDirectiveApi != undefined) {
                    var data = subviewDirectiveApi.getData();
                    if (data.PageDefinitionId != undefined) {

                        if (directiveSelectedPromiseDeferred != undefined) {

                            directiveSelectedPromiseDeferred.resolve();
                        }
                        else {
                                var fieldPayload = {
                                    pageDefinitionId: data.PageDefinitionId
                                };

                                loadFieldDirective(fieldPayload)
                        }
                    }
                }
            }
        }

        function loadDirective(directivePayload) {

            var subviewDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            VRUIUtilsService.callDirectiveLoad(subviewDirectiveApi, directivePayload, subviewDirectiveLoadDeferred);

            return subviewDirectiveLoadDeferred.promise;
        }

        function loadFieldDirective(fieldDirectivePayload) {
            var promises = [];

            var fieldDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                if (directiveSelectedPromiseDeferred != undefined)
                    promises.push(directiveSelectedPromiseDeferred.promise);
                UtilsService.waitMultiplePromises(promises).then(function (response) {

                    VRUIUtilsService.callDirectiveLoad(fieldDirectiveApi, fieldDirectivePayload, fieldDirectiveLoadDeferred);
                    directiveSelectedPromiseDeferred = undefined;
                });

            return fieldDirectiveLoadDeferred.promise;
        }

        function loadDirectives() {

            if (subviewSettingsEntity!=undefined && subviewSettingsEntity.PageDefinitionId != undefined) {

                var promises = [];
                directiveSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                var payload = {
                    subviewSettingsEntity: subviewSettingsEntity
                };

                promises.push(loadDirective(payload));
                var fieldPayload = {
                    pageDefinitionId: subviewSettingsEntity.PageDefinitionId,
                    subviewSettingsEntity: subviewSettingsEntity

                };
                promises.push(loadFieldDirective(fieldPayload));
                return UtilsService.waitMultiplePromises(promises);

            }

            else return loadDirective();

        }

        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                if(payload!=undefined)
                    subviewSettingsEntity = payload.subviewSettingsEntity;
                    return loadDirectives();

            }
            
            api.getData = function () {
                var data = {};
                data= subviewDirectiveApi.getData();
                data.FieldName = fieldDirectiveApi.getData();
                return data;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);


        }


        }

        return directiveDefinitionObject;
   
}]);

