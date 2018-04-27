'use strict';

app.directive('vrRuntimeRuntimenodeconfigurationSelector', ['VRRuntime_RuntimeNodeConfigurationAPIService','VRCommon_SMSMessageTypeAPIService', 'UtilsService', 'VRUIUtilsService',

    function (VRRuntime_RuntimeNodeConfigurationAPIService, VRCommon_SMSMessageTypeAPIService, UtilsService, VRUIUtilsService) {
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
                normalColNum: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var runtimeNodeConfigurationSelector = new RuntimeNodeConfigurationSelector(ctrl, $scope, $attrs);
                runtimeNodeConfigurationSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function RuntimeNodeConfigurationSelector(ctrl, $scope, attrs) {

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

                    var runtimeNodeConfiguration;

                    if (payload != undefined) {
                        runtimeNodeConfiguration = payload.runtimeNodeConfiguration;
                    }


                    var serializedFilter = UtilsService.serializetoJson(runtimeNodeConfiguration) != undefined ? UtilsService.serializetoJson(runtimeNodeConfiguration) : {};

                    VRRuntime_RuntimeNodeConfigurationAPIService.GetRuntimeNodeConfigurationsInfo(serializedFilter).then(function (response) {
                        selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }
                        if (runtimeNodeConfiguration != undefined) {
                            $scope.ctrl.selectedvalues =
                                UtilsService.getItemByVal($scope.ctrl.datasource, runtimeNodeConfiguration.RuntimeNodeConfigurationId, 'RuntimeNodeConfigurationId');
                        }
                    });

                };

                api.getSelectedId = function () {
                    if ($scope.ctrl.selectedvalues != null) {
                        return $scope.ctrl.selectedvalues.RuntimeNodeConfigurationId;
                    }
                    return null;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {
            var label = "Runtime Node Configuration";
            var hideremoveicon = '';

            if (attrs.hideremoveicon != undefined)
                hideremoveicon = 'hideremoveicon';

            return '<vr-select datatextfield="Name" datavaluefield="RuntimeNodeConfigurationId" isrequired="ctrl.isrequired" label="' + label +
                       '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label +
                       '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' customvalidate="ctrl.customvalidate">' +
                   '</vr-select>';
        }

    }]);