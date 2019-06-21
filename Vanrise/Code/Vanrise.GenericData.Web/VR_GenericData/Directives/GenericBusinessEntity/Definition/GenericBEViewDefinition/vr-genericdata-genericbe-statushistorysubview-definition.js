//(function (app) {

//    'use strict';

//    StatusHistoryGenericBEDefinitionViewDirective.$inject = ['UtilsService', 'VRUIUtilsService','VR_GenericData_GenericBEDefinitionAPIService'];

//    function StatusHistoryGenericBEDefinitionViewDirective(UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new HistoryGenericBEDefinitionViewCtor($scope, ctrl);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEViewDefinition/Templates/StatusHistoryGenericBEDefinitionViewTemplate.html'
//        };

//        function HistoryGenericBEDefinitionViewCtor($scope, ctrl) {
//            this.initializeController = initializeController;
//            var mappingFieldSelectorAPI;
//            var mappingFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.onMappingFieldSelectorReady = function (api) {
//                    mappingFieldSelectorAPI = api;
//                    mappingFieldSelectorReadyPromiseDeferred.resolve();
//                };
//                defineAPI();
//            }
            
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];
//                    var parameterEntity;
//                    var mappingFieldPayload = {};

//                    if (payload != undefined) {
//                        var context = payload.context;
//                        if (context != undefined) {
//                            mappingFieldPayload.dataRecordTypeId = context.getDataRecordTypeId();
//                        }
//                        if (payload.parameterEntity != undefined) {
//                            parameterEntity = payload.parameterEntity;
//                            mappingFieldPayload.selectedIds = parameterEntity != undefined ? parameterEntity.MappingFieldName : undefined;
//                        }
//                    }
//                    var loadMappingFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

//                    mappingFieldSelectorReadyPromiseDeferred.promise.then(function () {
//                        VRUIUtilsService.callDirectiveLoad(mappingFieldSelectorAPI, mappingFieldPayload, loadMappingFieldSelectorPromiseDeferred);
//                    });

//                    promises.push(loadMappingFieldSelectorPromiseDeferred.promise);
//                    return UtilsService.waitPromiseNode({ promises: promises });
//                };


//                api.getData = function () {
//                    return {
//                        $type: "Vanrise.GenericData.MainExtensions.GenericBEStatusHistorySubview, Vanrise.GenericData.MainExtensions",
//                        MappingFieldName: mappingFieldSelectorAPI.getSelectedIds()
//                    };
//                };
//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }
//        }
//    }

//    app.directive('vrGenericdataGenericbeStatushistorysubviewDefinition', StatusHistoryGenericBEDefinitionViewDirective);

//})(app);