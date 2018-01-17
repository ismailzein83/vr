(function (app) {

    'use strict';

    GuidFieldTypeFilterEditorDirective.$inject = [];

    function GuidFieldTypeFilterEditorDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var guidFieldTypeFilterEditor = new GuidFieldTypeFilterEditor(ctrl, $scope, $attrs);
                guidFieldTypeFilterEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function GuidFieldTypeFilterEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                if (ctrl.onReady != undefined) {
                    ctrl.onReady(getDirectiveAPI());
                }
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.label = payload != undefined ? payload.fieldTitle : "";
                };

                api.getData = function () {
                    return ctrl.selectedValue;
                };

                api.getValuesAsArray = function () {
                    var value = api.getData();
                    if (value == undefined)
                        return undefined;
                    return [value];
                };

                return api;
            }
        }

        function getDirectiveTemplate(attrs) {
            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                       '<vr-textbox  isrequired="ctrl.isrequired" label="{{ctrl.label}}" onselectionchanged="ctrl.onselectionchanged" value="ctrl.selectedValue" customvalidate="ctrl.customvalidate">' +
                       '</vr-textbox>' +
                   '</vr-columns>';
        }
    }

    app.directive('vrGenericdataFieldtypeGuidFiltereditor', GuidFieldTypeFilterEditorDirective);

})(app);