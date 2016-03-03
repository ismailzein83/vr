'use strict';
app.directive('vrGenericdataGenericbusinessentityGriddesign', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new FielddesignCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Templates/GenericBusinessEntityGridDesign.html';
            }

        };

        function FielddesignCtor(ctrl, $scope) {
            var gridAPI;
            function initializeController() {
                ctrl.fields = [];
                ctrl.selectedFields = [];
                ctrl.onRemoveField = function (dataItem) {
                    var index = $scope.scopeModal.selectedFields.indexOf(dataItem);
                    if (index != -1)
                        ctrl.selectedFields.splice(index, 1);
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        ctrl.fields.length = 0;
                        for (var i = 0; i < payload.recordTypeFields.length; i++) {
                            var field = payload.recordTypeFields[i];
                            ctrl.fields.push({ fieldPath: field.Name, fieldTitle: field.Name });
                        }
                    }
                }

                api.getData = function () {
                    var fields = [];
                    for (var i = 0; i < ctrl.fields.length; i++) {
                        var field = ctrl.fields[i];
                        fields.push({
                            FieldTitle: field.FieldTitle,
                            FieldPath: field.FieldPath
                        });
                    }
                    return { Fields: fields };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);