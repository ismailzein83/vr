//(function (app) {

//    'use strict';

//    GenericBusinessEntity360DegreeView.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBusinessEntityAPIService', 'VR_GenericData_GenericBusinessEntityService', 'VR_GenericData_GenericBEDefinitionAPIService', 'VR_GenericData_RecordQueryLogicalOperatorEnum', 'VRCommon_ModalWidthEnum', 'VR_GenericData_GenericBECustomActionService'];

//    function GenericBusinessEntity360DegreeView(UtilsService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityAPIService, VR_GenericData_GenericBusinessEntityService, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_RecordQueryLogicalOperatorEnum, VRCommon_ModalWidthEnum, VR_GenericData_GenericBECustomActionService) {
//        return {
//            restrict: "E",
//            scope: {
//                onReady: "="
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new GenericBusinessEntity360DegreeView($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,

//            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/Management/Templates/GenericBusinessEntity360DegreeViewTemplate.html"
//        };

//        function GenericBusinessEntity360DegreeView($scope, ctrl) {
//            this.initializeController = initializeController;

//            var businessEntityDefinitionId;
//            var directiveSettings;
//            var genericBusinessEntityId;
//            var parentBEEntity;
//            var genericEditorDefinitionSetting;
//            var genericBEDefinitionSettings;

//            var editorRuntimeDirectiveAPI;
//            var editorRuntimeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                $scope.scopeModel = {};

//                $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
//                    editorRuntimeDirectiveAPI = api;
//                    editorRuntimeDirectiveReadyPromiseDeferred.resolve();
//                };
//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var initialPromises = [];

//                    if (payload != undefined) {
//                        directiveSettings = payload.directiveSettings;
//                        parentBEEntity = payload.dataItem;

//                        if (directiveSettings != undefined) {
//                            businessEntityDefinitionId = directiveSettings.BusinessEntityDefinitionId;
//                            genericEditorDefinitionSetting = directiveSettings.DirectiveSettings;

//                        }
//                    }

//                    $scope.scopeModel.isEditorDirectiveLoading = true;
//                    if (businessEntityDefinitionId != undefined)
//                        initialPromises.push(loadBusinessEntityDefinitionSettings());

//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            var directivePromises = [];

//                            directivePromises.push(loadEditorRuntimeDirective());

//                            return {
//                                promises: directivePromises,
//                            };
//                        }
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
//                        $scope.scopeModel.isEditorDirectiveLoading = false;
//                    });
//                };

//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }

//            function loadBusinessEntityDefinitionSettings() {
//                return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(businessEntityDefinitionId).then(function (response) {
//                    if (response != undefined)
//                        genericBEDefinitionSettings = response;
//                });
//            }

//            function loadEditorRuntimeDirective() {
//                var loadEditorRuntimeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
//                editorRuntimeDirectiveReadyPromiseDeferred.promise.then(function () {
//                    var defaultValues = {};

//                    if (parentBEEntity != undefined && parentBEEntity.FieldValues != undefined) {
//                        var fieldValues = parentBEEntity.FieldValues;
//                        for (var key in fieldValues) {
//                            if (key != "$type" && !fieldValues[key].isHidden)
//                                defaultValues[key] = fieldValues[key].Value;
//                        }
//                    }

//                    var runtimeEditorPayload = {
//                        selectedValues: defaultValues,
//                        dataRecordTypeId: genericBEDefinitionSettings != undefined ? genericBEDefinitionSettings.DataRecordTypeId : undefined,
//                        definitionSettings: genericEditorDefinitionSetting,
//                        runtimeEditor: genericEditorDefinitionSetting != undefined ? genericEditorDefinitionSetting.RuntimeEditor : undefined
//                    };
//                    VRUIUtilsService.callDirectiveLoad(editorRuntimeDirectiveAPI, runtimeEditorPayload, loadEditorRuntimeDirectivePromiseDeferred);
//                });
//                return loadEditorRuntimeDirectivePromiseDeferred.promise;
//            }
//        }

//    }

//    app.directive('vrGenericdataGenericbusinessentity360degreeView', GenericBusinessEntity360DegreeView);


//})(app);