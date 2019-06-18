
//(function (app) {

//    'use strict';

//    GenericEditorSubViewDefinitionDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

//    function GenericEditorSubViewDefinitionDirective(UtilsService, VRUIUtilsService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new GenericEditorSubViewDefinitionCtrol($scope, ctrl);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEViewDefinition/Templates/GenericEditorSubViewViewTemplate.html'
//        };

//        function GenericEditorSubViewDefinitionCtrol($scope, ctrl) {
//            this.initializeController = initializeController;

//            var context;
//            var editorSettings;

//            var editorDefinitionAPI;
//            var editorDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                $scope.scopeModel = {};

//                $scope.scopeModel.onGenericBEEditorDefinitionDirectiveReady = function (api) {
//                    editorDefinitionAPI = api;
//                    editorDefinitionReadyPromiseDeferred.resolve();
//                };

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    console.log(payload);

//                    var initialPromises = [];
//                    if (payload != undefined) {
//                        context = payload.context;
//                        var parameterEntity = payload.parameterEntity;
//                        if (parameterEntity != undefined)
//                           editorSettings =  parameterEntity.GenericEditorDefinitionSetting;
//                    }

//                    initialPromises.push(loadEditorDefinitionDirective());
//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            var directivePromises = [];
//                            return {
//                                promises: directivePromises,
//                            };
//                        }
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };


//                api.getData = function () {
//                    return {
//                        $type: "Vanrise.GenericData.MainExtensions.GenericItemInformationView, Vanrise.GenericData.MainExtensions"
//                    };
//                };
//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }

//            function loadEditorDefinitionDirective() {
//                var loadEditorDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
//                editorDefinitionReadyPromiseDeferred.promise.then(function () {
//                    var editorPayload = {
//                        settings: editorSettings,
//                        context: getContext()
//                    };

//                    VRUIUtilsService.callDirectiveLoad(editorDefinitionAPI, editorPayload, loadEditorDefinitionDirectivePromiseDeferred);
//                });
//                return loadEditorDefinitionDirectivePromiseDeferred.promise;
//            }

//            function getContext() {
//                var currentContext = context;
//                if (currentContext == undefined)
//                    currentContext = {};

//                return currentContext;
//            }
//        }
//    }

//    app.directive('vrGenericdataGenericbeGenericeditorsubviewDefinition', GenericEditorSubViewDefinitionDirective);

//})(app);