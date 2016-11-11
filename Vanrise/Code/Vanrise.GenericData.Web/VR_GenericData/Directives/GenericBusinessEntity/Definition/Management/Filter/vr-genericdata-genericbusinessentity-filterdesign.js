'use strict';
app.directive('vrGenericdataGenericbusinessentityFilterdesign', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new FilterdesignCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/Management/Filter/Templates/GenericBusinessEntityFilterDesign.html';
            }

        };

        function FilterdesignCtor(ctrl, $scope) {
            function initializeController() {
                ctrl.fields = [];
                ctrl.selectedFields = [];
                ctrl.onRemoveField = function (dataItem) {
                    var index = ctrl.selectedFields.indexOf(dataItem);
                    if (index != -1)
                        ctrl.selectedFields.splice(index, 1);
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        ctrl.fields.length = 0;
                        ctrl.selectedFields.length = 0;
                        for (var i = 0; i < payload.recordTypeFields.length; i++) {
                            var field = payload.recordTypeFields[i];
                            ctrl.fields.push({ fieldPath: field.Name, fieldTitle: field.Name });
                        }
                        if (payload.selectedFields != undefined) {
                            for (var i = 0; i < payload.selectedFields.length; i++) {
                                var selectedField = payload.selectedFields[i];
                                ctrl.selectedFields.push({
                                    fieldPath: selectedField.FieldPath,
                                    fieldTitle: selectedField.FieldTitle,
                                    isRequired: selectedField.IsRequired
                                });
                            }
                        }
                    }
                };
                api.getData = function () {
                    var fields = [];
                    for (var i = 0; i < ctrl.selectedFields.length; i++) {
                        var field = ctrl.selectedFields[i];
                        fields.push({
                            FieldTitle: field.fieldTitle,
                            FieldPath: field.fieldPath,
                            IsRequired: field.isRequired
                        });
                    }
                    return { Fields: fields };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);