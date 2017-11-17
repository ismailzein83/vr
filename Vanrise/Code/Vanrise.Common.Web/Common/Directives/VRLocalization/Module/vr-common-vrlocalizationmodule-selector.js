'use strict';

app.directive("vrCommonLocalizationmoduleSelector", ['VRCommon_VRLocalizationModuleAPIService', 'UtilsService', 'VRUIUtilsService',

    function (VRCommon_VRLocalizationModuleAPIService, UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                hideremoveicon: '@',
                normalColNum: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];
                var localizationModuleSelector = new LocalizationModuleSelector(ctrl, $scope, $attrs);
                localizationModuleSelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };
        function LocalizationModuleSelector(ctrl, $scope, attrs) {
            var selectorAPI;

            this.initializeController = function () {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            };

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    var filter;

                    if (payload != undefined) {

                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }
                    return VRCommon_VRLocalizationModuleAPIService.GetVRLocalizationModuleInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();

                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'LocalizationModuleId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('LocalizationModuleId', attrs, ctrl);
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Module";

            if (attrs.ismultipleselection != undefined) {
                label = "Modules";
                multipleselection = "ismultipleselection";
            }

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                       '<vr-select ' + multipleselection + ' datatextfield="Name" datavaluefield="LocalizationModuleId" isrequired="ctrl.isrequired" label="' + label +
                           '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label +
                           '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" customvalidate="ctrl.customvalidate">' +
                       '</vr-select>' +
                   '</vr-columns>';
        }
    }

]);