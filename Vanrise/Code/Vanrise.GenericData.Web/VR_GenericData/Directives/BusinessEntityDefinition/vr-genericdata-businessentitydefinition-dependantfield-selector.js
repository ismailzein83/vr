'use strict';

app.directive('vrGenericdataBusinessentitydefinitionDependantfieldSelector', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                ismultipleselection: "@",
                onReady: '=',
                onselectionchanged: '=',
                customlabel: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];
                var ctor = new DependantFieldGridCtor(ctrl, $scope, $attrs);
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
            template: function (element, attrs) {
                return getDependentFieldSelectorTemplate(attrs);
            }
        };

        function getDependentFieldSelectorTemplate(attrs) {
            var label = '';
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = 'label= "' + attrs.customlabel + '"';
            return '<vr-select ' + multipleselection + ' on-ready="ctrl.onDependantFieldsSelectorReady" ' +
                ' datasource = "ctrl.datasource" ' +
                ' selectedvalues="ctrl.selectedvalues" ' +
                ' datatextfield = "fieldTitle" ' +
                label +
                ' datavaluefield = "fieldName" ' +
                ' onselectionchanged = "ctrl.onselectionchanged" ' +
                ' entityName = "Dependant Field" isrequired = "true"  hideremoveicon ></vr-select>';
        }

        function DependantFieldGridCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var dependantDataRecordFieldAPI;
            var dependantDataRecordFielPromiseDeferred = UtilsService.createPromiseDeferred();

            var context;
            var subscribedEvent;
            function initializeController() {
                $scope.$on("$destroy", function () {
                    if (context != undefined)
                        context.unSubscribeToFieldChangeEvent(subscribedEvent);
                });
                ctrl.onDependantFieldsSelectorReady = function (api) {
                    dependantDataRecordFieldAPI = api;
                    dependantDataRecordFielPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    if (payload != undefined) {
                        context = payload.context;
                        subscribedEvent = subscribeEvent;
                        context.subscribeToFieldChangeEvent(subscribedEvent);
                        ctrl.datasource = context.getFields();
                        selectedIds = payload.selectedIds;
                    }

                    if (selectedIds != undefined)
                        VRUIUtilsService.setSelectedValues(selectedIds, 'fieldName', attrs, ctrl);
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('fieldName', attrs, ctrl);
                };

                api.getData = function () {
                    return ctrl.datasource;
                };
                api.getSelectedValues = function () {
                    return ctrl.selectedvalues;
                };
                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function subscribeEvent() {
                ctrl.datasource = context.getFields();
                var isFieldExist = false;
                for (var i = 0; i < ctrl.datasource.length; i++) {
                    var recordType = ctrl.datasource[i];
                    if (ctrl.selectedvalues != undefined && recordType.fieldName == ctrl.selectedvalues.fieldName) {
                        ctrl.selectedvalues = recordType;
                        isFieldExist = true;
                    }
                }
                if (!isFieldExist)
                    ctrl.selectedvalues = undefined;
            }
        }

        return directiveDefinitionObject;
    }]);