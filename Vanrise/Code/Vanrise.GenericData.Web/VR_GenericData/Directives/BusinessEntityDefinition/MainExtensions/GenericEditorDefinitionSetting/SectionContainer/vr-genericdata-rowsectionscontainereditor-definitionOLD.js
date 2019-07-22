//"use strict";

//app.directive("vrGenericdataRowsectionscontainereditorDefinition", ["UtilsService", "VR_GenericData_GenericBEDefinitionService", "VRNotificationService",
//    function (UtilsService, VR_GenericData_GenericBEDefinitionService, VRNotificationService) {

//        var directiveDefinitionObject = {
//            restrict: "E",
//            scope: {
//                onReady: "="
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new SectionsContainer($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/SectionContainer/Templates/RowSectionsContainerDefinitionSettingTemplate.html"
//        };

//        function SectionsContainer($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var context;

//            function initializeController() {
//                ctrl.datasource = [];

//                ctrl.isValid = function () {
//                    if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
//                        return "You Should add at least one section.";

//                    if (ctrl.datasource.length > 0 && checkDuplicateName())
//                        return "Title in each section should be unique.";

//                    return null;
//                };

//                ctrl.addSectionContainer = function () {
//                    var onSectionContainerAdded = function (addedItem) {
//                        ctrl.datasource.push(addedItem);
//                    };

//                    VR_GenericData_GenericBEDefinitionService.addGenericBESectionContainer(onSectionContainerAdded, getContext());
//                };

//                ctrl.disableAddSectionContainer = function () {
//                    if (context == undefined)
//                        return true;

//                    var dataRecordTypeId = context.getDataRecordTypeId();
//                    if (dataRecordTypeId == undefined)
//                        return true;

//                    var recordTypeFields = context.getRecordTypeFields();
//                    if (recordTypeFields == undefined || recordTypeFields.length == 0)
//                        return true;

//                    return false;
//                };

//                ctrl.removeSectionContainer = function (dataItem) {
//                    VRNotificationService.showConfirmation().then(function (response) {
//                        if (response) {
//                            var index = ctrl.datasource.indexOf(dataItem);
//                            ctrl.datasource.splice(index, 1);
//                        }
//                    });
//                };

//                defineMenuActions();
//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    if (payload != undefined) {
//                        api.clearDataSource();
//                        context = payload.context;
//                        if (payload.settings != undefined && payload.settings.RowSectionContainers != undefined) {
//                            var sectionContainers = payload.settings.RowSectionContainers;
//                            for (var i = 0; i < sectionContainers.length; i++) {
//                                var item = sectionContainers[i];
//                                ctrl.datasource.push(item);
//                            }
//                        }
//                    }
//                };

//                api.getData = function () {
//                    var sections;
//                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0) {
//                        sections = [];
//                        for (var i = 0; i < ctrl.datasource.length; i++) {
//                            var currentItem = ctrl.datasource[i];
//                            sections.push({
//                                SectionTitle: currentItem.SectionTitle,
//                                ColNum: currentItem.ColNum,
//                                SectionSettings: currentItem.SectionSettings,
//                                TextResourceKey: currentItem.TextResourceKey
//                            });
//                        }
//                    }

//                    return {
//                        $type: "Vanrise.GenericData.MainExtensions.RowSectionsContainerEditorDefinitionSetting, Vanrise.GenericData.MainExtensions",
//                        RowSectionContainers: sections
//                    };
//                };

//                api.clearDataSource = function () {
//                    ctrl.datasource.length = 0;
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }

//            function defineMenuActions() {
//                $scope.gridMenuActions = [
//                    {
//                        name: "Edit",
//                        clicked: editSectionContainer
//                    }];
//            }

//            function editSectionContainer(sectionObj) {
//                var obj = UtilsService.cloneObject(sectionObj);
//                var onSectionContainerUpdated = function (section) {
//                    var index = ctrl.datasource.indexOf(sectionObj);
//                    ctrl.datasource[index] = section;
//                };
//                VR_GenericData_GenericBEDefinitionService.editGenericBESectionContainer(onSectionContainerUpdated, obj, getContext());
//            }

//            function getContext() {

//                var currentContext = {
//                    getFields: function () {
//                        return context.getFields();
//                    },
//                    getFilteredFields: function () {
//                        return context.getRecordTypeFields();
//                    },
//                    getRecordTypeFields: function () {
//                        return context.getRecordTypeFields();
//                    },
//                    getDataRecordTypeId: function () {
//                        return context.getDataRecordTypeId();
//                    },
//                    getFieldType: function (fieldName) {
//                        return context.getFieldType(fieldName);
//                    }
//                };
//                return currentContext;
//            }

//            function checkDuplicateName() {
//                for (var i = 0; i < ctrl.datasource.length -1; i++) {
//                    var currentItem = ctrl.datasource[i];
//                    for (var j = i + 1; j < ctrl.datasource.length; j++) {
//                        if (ctrl.datasource[j].SectionTitle == currentItem.SectionTitle)
//                            return true;
//                    }
//                }
//                return false;
//            }
//        }

//        return directiveDefinitionObject;
//    }
//]);