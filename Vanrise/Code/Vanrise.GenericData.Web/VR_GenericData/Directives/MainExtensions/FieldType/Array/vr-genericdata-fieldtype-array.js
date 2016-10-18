(function (app) {

    'use strict';

    ArrayFieldTypeEditorDirective.$inject = [];

    function ArrayFieldTypeEditorDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var arrayFieldTypeEditor = new ArrayFieldTypeEditor(ctrl, $scope, $attrs);
                arrayFieldTypeEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Array/Templates/ArrayFieldTypeTemplate.html'
        };

        function ArrayFieldTypeEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var fieldTypeSelectiveAPI;

            function initializeController() {
                ctrl.onFieldTypeSelectiveReady = function (api) {
                    fieldTypeSelectiveAPI = api;

                    if (ctrl.onReady != undefined) {
                        ctrl.onReady(getDirectiveAPI());
                    }
                }
            }

            function getDirectiveAPI() {
                var api = {};
                api.load = function (payload) {
                    var fieldTypeSelectivePayload;

                    if (payload != undefined) {
                        fieldTypeSelectivePayload = payload.FieldType;
                    }

                    return fieldTypeSelectiveAPI.load(fieldTypeSelectivePayload);
                };
                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.MainExtensions.DataRecordFields.FieldArrayType, Vanrise.GenericData.MainExtensions',
                        FieldType: fieldTypeSelectiveAPI.getData()
                    };
                };
                return api;
            }
        }
    }

    app.directive('vrGenericdataFieldtypeArray', ArrayFieldTypeEditorDirective);

})(app);