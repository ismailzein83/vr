"use strict";

app.directive("vrGenericdataSectionscontainereditorDefinition", ["UtilsService", "VRUIUtilsService", "VRLocalizationService",
    function (UtilsService, VRUIUtilsService, VRLocalizationService) {

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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/SectionsContainerDefinitionSettingTemplate.html"
        };

        function SectionsContainer($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            var gridAPI;
            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];

                $scope.scopeModel.isValid = function () {
                    if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
                        return "You Should add at least one section.";

                    if (ctrl.datasource.length > 0 && checkDuplicateName())
                        return "Title in each section should be unique.";

                    return null;
                };

                $scope.scopeModel.isLocalizationEnabled = VRLocalizationService.isLocalizationEnabled();

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();

                };
                $scope.scopeModel.addSectionContainer = function () {
                    var dataItem = {
                        entity: {}
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

                    gridAPI.expandRow(dataItem);

                    ctrl.datasource.push(dataItem);
                };

                $scope.scopeModel.removeSectionContainer = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };

                $scope.scopeModel.disableAddSectionContainer = function () {
                    if (context == undefined)
                        return true;

                    var dataRecordTypeId = context.getDataRecordTypeId();
                    if (dataRecordTypeId == undefined)
                        return true;

                    var recordTypeFields = context.getRecordTypeFields();
                    if (recordTypeFields == undefined || recordTypeFields.length == 0)
                        return true;

                    return false;
                };

            }
            function prepareSection(sectionObject) {
                var dataItem = {
                    entity: {
                        SectionTitle: sectionObject.payload.SectionTitle,
                        ColNum: sectionObject.payload.ColNum,
                    },
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
                    var setLoader = function (value) { dataItem.isEditorDirectiveLoading = value; };

                    var sectionPayload = {
                        settings: sectionObject.payload.SectionSettings,
                        context: getContext(),
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.editorDirectiveAPI, sectionPayload, setLoader);
                };
                ctrl.datasource.push(dataItem);
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
                                };
                                if ($scope.scopeModel.isLocalizationEnabled)
                                    promises.push(sectionObject.textResourceLoadPromiseDeferred.promise);
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
                                SectionSettings: currentItem.editorDirectiveAPI != undefined ? currentItem.editorDirectiveAPI.getData() : undefined,
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
        }

        return directiveDefinitionObject;
    }
]);