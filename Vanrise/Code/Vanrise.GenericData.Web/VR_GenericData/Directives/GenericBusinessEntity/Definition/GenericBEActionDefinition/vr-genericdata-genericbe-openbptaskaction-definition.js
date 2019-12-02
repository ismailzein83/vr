//(function (app) {

//    'use strict';

//    OpenBPTaskGenericBEDefinitionActionDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

//    function OpenBPTaskGenericBEDefinitionActionDirective(UtilsService, VRUIUtilsService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new DownloadFileGenericBEDefinitionActionCtor($scope, ctrl);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEActionDefinition/Templates/OpenBPTaskGenericBEDefinitionActionTemplate.html'
//        };

//        function DownloadFileGenericBEDefinitionActionCtor($scope, ctrl) {
//            this.initializeController = initializeController;

//            var dataRecordTypeFieldsSelectorAPI;
//            var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                $scope.scopeModel = {};

//                $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
//                    dataRecordTypeFieldsSelectorAPI = api;
//                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
//                };

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var context = payload.context;
//                    if (context != undefined && context.showSecurityGridCallBack != undefined && typeof (context.showSecurityGridCallBack) == 'function')
//                        context.showSecurityGridCallBack(false);

//                    function loadDataRecordFieldsSelector() {
//                        var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
//                        dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
//                            var dataRecordTypeFieldsPayload = {
//                                dataRecordTypeId: context.getDataRecordTypeId(),
//                            };
//                            if (payload.settings != undefined && payload.settings.TaskIdFieldName != undefined) {
//                                dataRecordTypeFieldsPayload.selectedIds = payload.settings.TaskIdFieldName;
//                            }
//                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
//                        });
//                        return loadDataRecordTypeFieldsSelectorPromiseDeferred.promise;
//                    }

//                    return UtilsService.waitPromiseNode({ promises: [loadDataRecordFieldsSelector()] });
//                };


//                api.getData = function () {
//                    return {
//                        $type: "Vanrise.GenericData.MainExtensions.OpenBPTaskGenericBEAction, Vanrise.GenericData.MainExtensions",
//                        TaskIdFieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds(),
//                    };
//                };
//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }
//        }
//    }

//    app.directive('vrGenericdataGenericbeOpenbptaskactionDefinition', OpenBPTaskGenericBEDefinitionActionDirective);

//})(app);