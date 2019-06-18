//(function (app) {

//    'use strict';

//    BulkAddGenericBEDefinitionCustomActionDirective.$inject = ['UtilsService','VRUIUtilsService', 'VRNotificationService'];

//    function BulkAddGenericBEDefinitionCustomActionDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new BulkAddCustomActionCtor($scope, ctrl);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBECustomActionDefinition/Templates/BulkAddGenericBEDefinitionCustomActionTemplate.html'
//        };

//        function BulkAddCustomActionCtor($scope, ctrl) {

//            var dataRecordTypeFieldsSelectorAPI;
//            var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            var editorDefinitionAPI;
//            var editorDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            this.initializeController = initializeController;

//            function initializeController() {
//                $scope.scopeModel = {};
                
//                $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
//                    dataRecordTypeFieldsSelectorAPI = api;
//                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
//                };

//                $scope.scopeModel.onGenericBEEditorDefinitionDirectiveReady = function (api) {
//                    editorDefinitionAPI = api;
//                    editorDefinitionReadyPromiseDeferred.resolve();
//                };

//                defineAPI();
//            }

 
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];
//                    $scope.scopeModel.rangeVariableName = "GeneratedRangeInfo";
//                    if (payload != undefined) {
//                        var context = payload.context;
//                        var dataRecordTypeId = context.getDataRecordTypeId();
//                        var fieldsPayload = {
//                            dataRecordTypeId: dataRecordTypeId,
//                        };
//                        var editorPayload = {
//                            context: context
//                        };
//                        if (payload.settings != undefined) {
//                            if (payload.settings.RangeVariableName != undefined)
//                                $scope.scopeModel.rangeVariableName = payload.settings.RangeVariableName;
//                            fieldsPayload.selectedIds = payload.settings.RangeFieldName;
//                            editorPayload.settings = payload.settings.Settings;
//                        }
//                        if (dataRecordTypeId != undefined) {
//                            var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

//                            dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
//                                VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, fieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
//                            });
//                            promises.push(loadDataRecordTypeFieldsSelectorPromiseDeferred.promise);

//                            var loadEditorDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

//                            editorDefinitionReadyPromiseDeferred.promise.then(function () {
//                                VRUIUtilsService.callDirectiveLoad(editorDefinitionAPI, editorPayload, loadEditorDefinitionDirectivePromiseDeferred);
//                            });
//                            promises.push(loadEditorDefinitionDirectivePromiseDeferred.promise);
//                        }
//                    }

//                    return UtilsService.waitPromiseNode({ promises: promises });
//                };


//                api.getData = function () {
//                    return {
//                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBECustomAction.GenericBEBulkAddCustomAction, Vanrise.GenericData.MainExtensions",
//                        RangeVariableName: $scope.scopeModel.rangeVariableName,
//                        RangeFieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds(),
//                        Settings: editorDefinitionAPI.getData()
//                    };
//                };
//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }
//        }
//    }

//    app.directive('vrGenericdataGenericbeBulkaddCustomactionDefinition', BulkAddGenericBEDefinitionCustomActionDirective);

//})(app);