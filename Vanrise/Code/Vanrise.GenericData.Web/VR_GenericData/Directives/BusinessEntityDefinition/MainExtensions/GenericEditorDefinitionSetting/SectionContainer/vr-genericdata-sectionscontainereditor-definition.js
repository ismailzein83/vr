"use strict";

app.directive("vrGenericdataSectionscontainereditorDefinition", ["UtilsService", "VRUIUtilsService", "VRLocalizationService", "VR_GenericData_GenericBEDefinitionService",
    function (UtilsService, VRUIUtilsService, VRLocalizationService, VR_GenericData_GenericBEDefinitionService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SectionsContainer($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/SectionContainer/Templates/SectionsContainerDefinitionSettingTemplate.html"
        };

        function SectionsContainer($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            var indexSection = 1;

            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];
                $scope.scopeModel.showAddButton = true;

                $scope.scopeModel.dragsettings = {
                    handle: '.handeldrag'
                };

                $scope.scopeModel.isValid = function () {
                    if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
                        return "You Should add at least one section.";

                    if (ctrl.datasource.length > 0 && checkColNum())
                        return "Sections should be aligned on the same row";

                    if (ctrl.datasource.length > 0 && checkDuplicateName())
                        return "Title in each section should be unique.";

                    return null;
                };

                $scope.scopeModel.isLocalizationEnabled = VRLocalizationService.isLocalizationEnabled();

                $scope.scopeModel.addSectionContainer = function () {
                    var dataItem = {
                        entity: {
                            ColNum: 6,
                            SectionTitle: "Section " + (indexSection++)
                        }
                    };

                    dataItem.onTextResourceSelectorReady = function (api) {
                        dataItem.textResourceSeletorAPI = api;
                        var setLoader = function (value) { dataItem.isFieldTextResourceSelectorLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.textResourceSeletorAPI, undefined, setLoader);
                    };

                    dataItem.onEditorDirectiveReady = function (api) {
                        dataItem.editorDirectiveAPI = api;
                        var setLoader = function (value) { dataItem.isEditorDirectiveLoading = value; };
                        var payload = {
                            context: getContext(),
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.editorDirectiveAPI, payload, setLoader);
                    };

                    ctrl.datasource.push(dataItem);
                };

                $scope.scopeModel.editSectionContainer = function (dataItem) {
                    var sectionEntityObject = UtilsService.cloneObject(dataItem);

                    var onSectionSettingsChanged = function (sectionItem) {
                        dataItem.entity.ColNum = sectionItem.ColNum != undefined ? parseInt(sectionItem.ColNum) : 6;
                    };
                    VR_GenericData_GenericBEDefinitionService.openGenericBESectionContainerEditor(onSectionSettingsChanged, sectionEntityObject, getContext());
                };

                $scope.scopeModel.removeSectionContainer = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };

                $scope.scopeModel.sectionSettings = {
                    sortable: true,
                    headerEditable: true,
                    oneditclicked: $scope.scopeModel.editSectionContainer
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        api.clearDataSource();
                        context = payload.context;
                        if (payload.settings != undefined && payload.settings.SectionContainers != undefined) {
                            var sectionContainers = payload.settings.SectionContainers;
                            for (var i = 0; i < sectionContainers.length; i++) {
                                var item = sectionContainers[i];

                                var sectionObject = {
                                    payload: item,
                                    textResourceReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    textResourceLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    editorDefinitionSettingsReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    editorDefinitionSettingsLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                };
                                if ($scope.scopeModel.isLocalizationEnabled)
                                    promises.push(sectionObject.textResourceLoadPromiseDeferred.promise);

                                promises.push(sectionObject.editorDefinitionSettingsLoadPromiseDeferred.promise);
                                prepareSection(sectionObject);
                            }
                        }
                    }
                    return UtilsService.waitPromiseNode({ promises: promises });
                };

                api.getData = function () {
                    var sections;
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0) {
                        sections = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            sections.push({
                                SectionTitle: currentItem.entity.SectionTitle,
                                ColNum: currentItem.entity.ColNum,
                                SectionSettings: currentItem.editorDirectiveAPI != undefined ? currentItem.editorDirectiveAPI.getData() : currentItem.oldSettings,
                                TextResourceKey: currentItem.textResourceSeletorAPI != undefined ? currentItem.textResourceSeletorAPI.getSelectedValues() : currentItem.oldTextResourceKey
                            });
                        }
                    }

                    return {
                        $type: "Vanrise.GenericData.MainExtensions.SectionsContainerEditorDefinitionSetting, Vanrise.GenericData.MainExtensions",
                        SectionContainers: sections
                    };
                };

                api.clearDataSource = function () {
                    ctrl.datasource.length = 0;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function prepareSection(sectionObject) {
                var dataItem = {
                    entity: {
                        SectionTitle: sectionObject.payload.SectionTitle,
                        ColNum: sectionObject.payload.ColNum,
                    },
                    oldSettings: sectionObject.payload.SectionSettings,
                    oldTextResourceKey: sectionObject.payload.TextResourceKey
                };

                dataItem.onTextResourceSelectorReady = function (api) {
                    dataItem.textResourceSeletorAPI = api;
                    sectionObject.textResourceReadyPromiseDeferred.resolve();
                };

                sectionObject.textResourceReadyPromiseDeferred.promise.then(function () {
                    var textResourcePayload = { selectedValue: sectionObject.payload.TextResourceKey };
                    VRUIUtilsService.callDirectiveLoad(dataItem.textResourceSeletorAPI, textResourcePayload, sectionObject.textResourceLoadPromiseDeferred);
                });

                dataItem.onEditorDirectiveReady = function (api) {
                    dataItem.editorDirectiveAPI = api;
                    sectionObject.editorDefinitionSettingsReadyPromiseDeferred.resolve();
                };

                sectionObject.editorDefinitionSettingsReadyPromiseDeferred.promise.then(function () {
                    var sectionPayload = {
                        settings: sectionObject.payload.SectionSettings,
                        context: getContext(),
                    };
                    VRUIUtilsService.callDirectiveLoad(dataItem.editorDirectiveAPI, sectionPayload, sectionObject.editorDefinitionSettingsLoadPromiseDeferred);
                });

                ctrl.datasource.push(dataItem);
            }

            function getContext() {

                var currentContext = {
                    getFields: function () {
                        return context.getFields();
                    },
                    getFilteredFields: function () {
                        return context.getRecordTypeFields();
                    },
                    getRecordTypeFields: function () {
                        return context.getRecordTypeFields();
                    },
                    getDataRecordTypeId: function () {
                        return context.getDataRecordTypeId();
                    },
                    getFieldType: function (fieldName) {
                        return context.getFieldType(fieldName);
                    }
                };
                return currentContext;
            }

            function checkDuplicateName() {
                for (var i = 0; i < ctrl.datasource.length; i++) {
                    var currentItem = ctrl.datasource[i];
                    for (var j = 0; j < ctrl.datasource.length; j++) {
                        if (i != j && ctrl.datasource[j].entity.SectionTitle == currentItem.entity.SectionTitle)
                            return true;
                    }
                }
                return false;
            }

            function checkColNum() {
                var sum = 0;
                for (var i = 0; i < ctrl.datasource.length; i++) {
                    var currentItem = ctrl.datasource[i];
                    sum += currentItem.entity.ColNum;
                }

                if (sum >= 12)
                    $scope.scopeModel.showAddButton = false;
                else
                    $scope.scopeModel.showAddButton = true;

                if (sum > 12)
                    return true;
                return false;
            }
        }

        return directiveDefinitionObject;
    }
]);