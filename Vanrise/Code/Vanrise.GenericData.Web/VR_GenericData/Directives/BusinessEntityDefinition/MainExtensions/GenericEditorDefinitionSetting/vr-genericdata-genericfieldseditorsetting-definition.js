(function (app) {

    'use strict';

    GenericFieldsEditorDefinitionDirective.$inject = ['VRNotificationService', 'VR_GenericData_GenericBEDefinitionService'];

    function GenericFieldsEditorDefinitionDirective(VRNotificationService, VR_GenericData_GenericBEDefinitionService) {
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
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/GenericFieldsEditorDefinitionSettingTemplate.html'
        };

        function GenericFieldsEditorDefinitionCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var context;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.addField = function () {
                    var onBEFieldAdded = function (fieldObj) {
                        $scope.scopeModel.datasource.push(fieldObj);
                    };
                    VR_GenericData_GenericBEDefinitionService.addGenericBEField(onBEFieldAdded, context, getFilteredFields());
                };

                $scope.scopeModel.isValid = function () {
                    if ($scope.scopeModel.datasource == undefined || $scope.scopeModel.datasource.length == 0)
                        return "You Should add at least one Field.";

                    for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                        var firstField = $scope.scopeModel.datasource[i];
                        for (var j = i + 1; j < $scope.scopeModel.datasource.length; j++) {
                            var secondField = $scope.scopeModel.datasource[j];
                            if (firstField.FieldPath == secondField.FieldPath)
                                return "Field Name Should be Unique!";
                        }
                    }

                    return null;
                };

                defineAPI();

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var settings;
                    var fields;

                    if (payload != undefined) {
                        settings = payload.settings;
                        context = payload.context;

                        if (settings != undefined) {
                            fields = settings.Fields;
                        }
                    }

                    if (fields != undefined && fields.length > 0) {
                        for (var i = 0; i < fields.length; i++) {
                            $scope.scopeModel.datasource.push(fields[i]);
                        }
                    }
                };


                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericFieldsEditorDefinitionSetting, Vanrise.GenericData.MainExtensions",
                        Fields: getFields()
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                    {
                        name: "Edit",
                        clicked: editField
                    },
                    {
                        name: "Delete",
                        clicked: deleteField
                    }];

                $scope.scopeModel.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editField(fieldEntityObj) {
                var onBEFieldUpdated = function (fieldObj) {
                    var index = $scope.scopeModel.datasource.indexOf(fieldEntityObj);
                    $scope.scopeModel.datasource[index] = fieldObj;
                };

                VR_GenericData_GenericBEDefinitionService.editGenericBEField(onBEFieldUpdated, context, getFilteredFields(), fieldEntityObj);
            }

            function deleteField(fieldObj) {
                VRNotificationService.showConfirmation().then(function (response) {
                    if (response) {
                        var index = $scope.scopeModel.datasource.indexOf(fieldObj);
                        if (index != -1)
                            $scope.scopeModel.datasource.splice(index, 1);
                    }
                });
            }

            function getFilteredFields() {
                var filteredFields = [];
                if (context != undefined) {
                    var allFields = context.getRecordTypeFields();
                    for (var i = 0; i < allFields.length; i++) {
                        var field = allFields[i];
                        filteredFields.push({ FieldPath: field.Name, FieldTitle: field.Title });
                    }
                }
                return filteredFields;
            }

            function getFields() {
                var fields = [];
                for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                    var currentField = $scope.scopeModel.datasource[i];
                    fields.push({
                        FieldPath: currentField.FieldPath,
                        FieldTitle: currentField.FieldTitle,
                        IsRequired: currentField.IsRequired,
                        IsDisabled: currentField.IsDisabled,
                        ShowAsLabel: currentField.ShowAsLabel,
                        FieldWidth: currentField.FieldWidth,
                        FieldViewSettings: currentField.FieldViewSettings,
                        TextResourceKey: currentField.TextResourceKey,
                        DefaultFieldValue: currentField.DefaultFieldValue,
                    });
                }

                return fields;
            }
        }
    }

    app.directive('vrGenericdataGenericfieldseditorsettingDefinition', GenericFieldsEditorDefinitionDirective);

})(app);