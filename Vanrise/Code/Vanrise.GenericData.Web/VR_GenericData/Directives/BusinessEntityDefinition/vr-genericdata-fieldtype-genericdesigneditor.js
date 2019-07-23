"use strict";
app.directive("vrGenericdataFieldtypeGenericdesigneditor", ["UtilsService", "VRUIUtilsService","VR_GenericData_GenericBEDefinitionService",
    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var fieldTypeGenericEditorDesign = new FieldTypeGenericEditorDesignCtrl($scope, ctrl, $attrs, $element);
                fieldTypeGenericEditorDesign.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/Templates/FieldTypeGenericEditorDesign.html"
        };
        function FieldTypeGenericEditorDesignCtrl($scope, ctrl, $attrs, $element) {
            this.initializeController = initializeController;

            var context;
            var fieldTypeEntity;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.openSettingsModal = function () {
                    var fieldTypeEntityObject = UtilsService.cloneObject(fieldTypeEntity);

                    var onFiledTypeSettingsChanged = function (fieldTypeItem) {
                        fieldTypeEntity = fieldTypeItem;
                        $scope.scopeModel.fieldWidth = fieldTypeItem.FieldWidth != undefined ? fieldTypeItem.FieldWidth : 4;
                    };
                    VR_GenericData_GenericBEDefinitionService.openFieldTypeGenericEditorDesignModal(onFiledTypeSettingsChanged, fieldTypeEntityObject, getContext());
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        fieldTypeEntity = payload.fieldTypeEntity;
                        $scope.scopeModel.fieldPath = fieldTypeEntity.FieldPath;
                        $scope.scopeModel.fieldTitle = fieldTypeEntity.FieldTitle;

                        if (fieldTypeEntity != undefined) {
                            $scope.scopeModel.fieldWidth = fieldTypeEntity.FieldWidth != undefined ? fieldTypeEntity.FieldWidth : 4;
                        }
                    }

                    var rootPromiseNode = {
                        promises: []
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    if (fieldTypeEntity == undefined)
                        fieldTypeEntity = {};
                    fieldTypeEntity.FieldTitle = $scope.scopeModel.fieldTitle;

                    return fieldTypeEntity;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
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
]);