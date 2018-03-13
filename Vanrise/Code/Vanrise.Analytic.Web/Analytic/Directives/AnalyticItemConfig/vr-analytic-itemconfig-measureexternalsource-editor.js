(function (app) {
    "use strict";

    ItemconfigMeasureExternalSourceEditorDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function ItemconfigMeasureExternalSourceEditorDirective(UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var itemconfigMeasureExternalSourceEditor = new ItemconfigMeasureExternalSourceEditor($scope, ctrl, $attrs);
                itemconfigMeasureExternalSourceEditor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Analytic/Directives/AnalyticItemConfig/Templates/MeasureExternalSourceEditorTemplate.html';
            }

        };
        function ItemconfigMeasureExternalSourceEditor($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var context;
            var tableId;
           
            var externalSourceSelectiveAPI;
            var externalSourceReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};
       
                $scope.scopeModel.onExternalSourceSelectiveReady = function (api) {
                    externalSourceSelectiveAPI = api;
                    externalSourceReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([externalSourceReadyDeferred.promise]).then(function () {
                    
                    defineAPI();
                });
               
            }
            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    
                    var configEntity;
                    var promises = [];
                    
                    if (payload != undefined) {
                        tableId = payload.tableId;                       
                        configEntity = payload.ConfigEntity;
                        context = payload.context;
                    }
                    
                    promises.push(loadExternalSourceSelective());

                    function loadExternalSourceSelective() {
                        
                        var payload = {
                            tableId: tableId,
                            context: getContext(),
                            entity: configEntity != undefined ? configEntity.ExtendedSettings : undefined
                        };
                        return externalSourceSelectiveAPI.load(payload);
                    }
                    
                    return UtilsService.waitMultiplePromises(promises);
                };
                api.getData = function () {
                    return {
                        $type: "Vanrise.Analytic.Entities.AnalyticMeasureExternalSourceConfig, Vanrise.Analytic.Entities",
                        ExtendedSettings: externalSourceSelectiveAPI.getData()
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined) 
                    currentContext = {};
              
                return currentContext;
            }

        }

       
        return directiveDefinitionObject;
    }
    app.directive("vrAnalyticItemconfigMeasureexternalsourceEditor", ItemconfigMeasureExternalSourceEditorDirective);
})(app);
