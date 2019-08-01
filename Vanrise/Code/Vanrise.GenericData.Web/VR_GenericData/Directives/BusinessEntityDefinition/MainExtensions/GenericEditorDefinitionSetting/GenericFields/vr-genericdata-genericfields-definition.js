(function (app) {

    'use strict';

    GenericFieldsDefinitionDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRLocalizationService', 'VR_GenericData_DataRecordFieldAPIService'];

    function GenericFieldsDefinitionDirective(UtilsService, VRUIUtilsService, VRLocalizationService, VR_GenericData_DataRecordFieldAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericFieldsEditorDefinitionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/GenericFields/Templates/GenericFieldsDefinitionTemplate.html'
        };

        function GenericFieldsEditorDefinitionCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var context;
            var fields;
            var setFieldsNumber;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.fields = [];

                $scope.scopeModel.genericFieldsSortSettings = {
                    handle: '.vr-control-label'
                };

                $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onFieldSelected = function (item) {
                    prepareAddedField(item);
                };

                $scope.scopeModel.onFieldDeselected = function (item) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.fields, item.Name, "FieldPath");
                    $scope.scopeModel.fields.splice(index, 1);

                    if (setFieldsNumber != undefined)
                        setFieldsNumber($scope.scopeModel.fields.length);
                };

                $scope.scopeModel.deselectAllFields = function () {
                    $scope.scopeModel.fields.length = 0;

                    if (setFieldsNumber != undefined)
                        setFieldsNumber(0);
                };

                defineAPI();
            }

            function prepareAddedField(item) {
                var dataItem = {};

                dataItem.onFieldTypeGenericDesignEditorReady = function (api) {
                    dataItem.fieldTypeGenereicDesignEditorAPI = api;
                    var setLoader = function (value) { dataItem.isFieldTypeGenericDesignEditorLoading = value; };
                    var payload = {
                        FieldPath: item.Name,
                        FieldTitle: item.Title
                    };
                    var directivePayload = {
                        context: context,
                        fieldTypeEntity: payload
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.fieldTypeGenereicDesignEditorAPI, directivePayload, setLoader);
                };

                $scope.scopeModel.fields.push(dataItem);

                if (setFieldsNumber != undefined)
                    setFieldsNumber($scope.scopeModel.fields.length);
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];
                    $scope.scopeModel.fields.length = 0;

                    if (payload != undefined) {
                        context = payload.context;
                        fields = payload.fields;
                        setFieldsNumber = payload.setFieldsNumber;
                    }

                    initialPromises.push(loadFieldNameSelector());
                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var childPromises = [];
                            if (fields != undefined) {
                                for (var i = 0; i < fields.length; i++) {
                                    var field = fields[i];

                                    var fieldObject = {
                                        payload: field,
                                        genreicDesignEditorReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        genreicDesignEditorLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    };
                                    childPromises.push(fieldObject.genreicDesignEditorLoadPromiseDeferred.promise);
                                    prepareDataItem(fieldObject);
                                }
                            }
                            return { promises: childPromises };
                        }
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return getFields();
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function loadFieldNameSelector() {
                var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                    var selectedIds = [];

                    if (fields != undefined && fields.length > 0) {
                        for (var i = 0; i < fields.length; i++) {
                            var field = fields[i];
                            selectedIds.push(field.FieldPath);
                        }
                    }

                    var typeFieldsPayload = {
                        dataRecordTypeId: context.getDataRecordTypeId(),
                        selectedIds: selectedIds
                    };
                    VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, typeFieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                });
                return loadDataRecordTypeFieldsSelectorPromiseDeferred.promise;
            }

            function prepareDataItem(field) {
                var payload = field.payload;
                
                var dataItem = {
                    id: $scope.scopeModel.fields.length + 1,
                };

                dataItem.onFieldTypeGenericDesignEditorReady = function (api) {
                    dataItem.fieldTypeGenereicDesignEditorAPI = api;
                    field.genreicDesignEditorReadyPromiseDeferred.resolve();
                };

                field.genreicDesignEditorReadyPromiseDeferred.promise.then(function () {
                    var fieldTypeEntityPayload = payload;
                    if (payload != undefined) {
                        fieldTypeEntityPayload.oldTextResourceKey = payload.TextResourceKey;
                        fieldTypeEntityPayload.oldRuntimeViewSettings = payload.FieldViewSettings;
                        fieldTypeEntityPayload.oldFieldTypeRumtimeSettings = payload.DefaultFieldValue;
                    }

                    var fieldTypePayload = {
                        context: context,
                        fieldTypeEntity: fieldTypeEntityPayload
                    };
                    VRUIUtilsService.callDirectiveLoad(dataItem.fieldTypeGenereicDesignEditorAPI, fieldTypePayload, field.genreicDesignEditorLoadPromiseDeferred);
                });

                $scope.scopeModel.fields.push(dataItem);
            }


            function getFields() {
                var fields = [];
                for (var i = 0; i < $scope.scopeModel.fields.length; i++) {
                    var currentField = $scope.scopeModel.fields[i];
                    var data = currentField.fieldTypeGenereicDesignEditorAPI.getData();
                    if (data != undefined)
                        fields.push(data);
                }
                return fields;
            }
        }
    }

    app.directive('vrGenericdataGenericfieldsDefinition', GenericFieldsDefinitionDirective);

})(app);