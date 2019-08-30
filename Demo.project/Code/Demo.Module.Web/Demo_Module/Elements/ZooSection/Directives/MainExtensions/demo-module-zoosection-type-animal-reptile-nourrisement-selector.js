app.directive('demoModuleZoosectionTypeAnimalReptileNourrisementSelector', ['VRUIUtilsService', 'ZooSectionTypeAnimalReptileNourrisementEnum', 'UtilsService',
    function (VRUIUtilsService, ZooSectionTypeAnimalReptileNourrisementEnum, UtilsService) {

        'use strict';
        
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: "=",
                onselectitem: "=",
                ondeselectitem: "=",
                hideremoveicon: '@',
                normalColNum: '@',
                isdisabled: '='
            },
            controller: function ($scope, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var nourriseementSelector = new NourrisementSelector(ctrl, $scope, $attrs);
                nourriseementSelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {

            var label = 'Nourrisement';

            var multipleselection = '';
            if (attrs.ismultipleselection != undefined) {
                label = 'Nourrisements';
                multipleselection = 'ismultipleselection';
            }

            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined) {
                hideremoveicon = 'hideremoveicon';
            }

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + '<span vr-disabled="ctrl.isdisabled">'
                + '<vr-select on-ready="scopeModel.onSelectorReady" ' + multipleselection + ' datatextfield="description" datavaluefield="value"  isrequired="ctrl.isrequired" '
                + ' label="' + label + '" ' + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Nourrisement" onselectitem="ctrl.onselectitem" '
                + ' ondeselectitem = "ctrl.ondeselectitem"' + hideremoveicon + ' >'
                + '</vr-select>'
                + '</span>'
                + '</vr-columns>';
        }

        function NourrisementSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            
            var selectorAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }

                    var zooSectionTypeAnimalReptileNourrisementArray = UtilsService.getArrayEnum(ZooSectionTypeAnimalReptileNourrisementEnum);
                    for (var i = 0; i < zooSectionTypeAnimalReptileNourrisementArray.length; i++) {
                        ctrl.datasource.push(zooSectionTypeAnimalReptileNourrisementArray[i]);
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

        return directiveDefinitionObject;
    }]);