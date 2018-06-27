'use strict';

app.directive('businessprocessVrWorkflowArgumentdirectionSelector', ['UtilsService', 'VRUIUtilsService', 'VRWorkflowArgumentDirection',

function (UtilsService, VRUIUtilsService, VRWorkflowArgumentDirection) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                normalColNum: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;

                var argumentDirectionSelector = new ArgumentDirectionSelector(ctrl, $scope, $attrs);
                argumentDirectionSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function ArgumentDirectionSelector(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    selectorAPI.clearDataSource();
                    ctrl.datasource = UtilsService.getArrayEnum(VRWorkflowArgumentDirection);

                    var selectedIds;
                   
                    if (payload) {
                        selectedIds = payload.selectedIds;
                    }

                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var label = "Direction";
            var hideremoveicon = '';

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            if (attrs.hideremoveicon != undefined)
                hideremoveicon = 'hideremoveicon';

            return '<vr-select datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired"' +
                       '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label +
                       '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' customvalidate="ctrl.customvalidate">' +
                   '</vr-select>';
        }

    }]);