'use strict';
app.directive('vrGenericdataGenericbusinessentityDefinitionRow', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new RowCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/Editor/Templates/DefinitionRowTemplate.html';
            }

        };

        function RowCtor(ctrl, $scope) {
            var gridAPI;
            function initializeController() {
                ctrl.fields = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        ctrl.fields.length = 0;
                        ctrl.fields = payload.fields;
                    }
                };

                api.applyChanges = function (changes) {
                    if (changes != undefined) {
                        ctrl.fields.length = 0;
                        ctrl.fields = changes;
                    }
                };
                api.getData = function () {
                    var fields = [];
                    for (var i = 0; i < ctrl.fields.length; i++) {
                        var field = ctrl.fields[i];
                        fields.push({
                            FieldTitle: field.FieldTitle,
                            FieldPath: field.FieldPath,
                            IsRequired: field.IsRequired,
                            IsDisabled: field.IsDisabled,
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