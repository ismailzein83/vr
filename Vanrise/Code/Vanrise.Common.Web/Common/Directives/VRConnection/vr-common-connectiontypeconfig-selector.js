(function (app) {

    'use strict';

    ConnectionTypeSettingsDirective.$inject = ['VRCommon_VRConnectionAPIService', 'VRUIUtilsService'];

    function ConnectionTypeSettingsDirective(VRCommon_VRConnectionAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                ismultipleselection: '@',
                isdisabled: '@',
                isrequired: '=',
                hideremoveicon: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined && $attrs.ismultipleselection != null)
                    ctrl.selectedvalues = [];

                var typeSettings = new TypeSettings(ctrl, $scope, $attrs);
                typeSettings.initializeController();
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
                return getDirectiveTemplate(attrs);
            }
        };

        function TypeSettings(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var directiveAPI = {};

                directiveAPI.load = function (payload) {

                    var selectedIds;
                    var filter;
                    var connectionTypeIds;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;

                        if (filter != undefined && filter.connectionTypeIds != undefined && filter.connectionTypeIds.length > 0) {
                            connectionTypeIds = [];
                            for (var i = 0; i < filter.connectionTypeIds.length; i++) {
                                connectionTypeIds.push(filter.connectionTypeIds[i].toLowerCase());
                            }
                        }
                    }

                    return VRCommon_VRConnectionAPIService.GetVRConnectionConfigTypes().then(function (response) {
                        selectorAPI.clearDataSource();
                        angular.forEach(response, function (item) {
                            if (connectionTypeIds == undefined || connectionTypeIds.indexOf(item.ExtensionConfigurationId.toLowerCase()) > -1)
                                ctrl.datasource.push(item);
                        });

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'ExtensionConfigurationId', attrs, ctrl);
                        } else if (connectionTypeIds != undefined && connectionTypeIds.length == 1) {
                            VRUIUtilsService.setSelectedValues(connectionTypeIds[0], 'ExtensionConfigurationId', attrs, ctrl);
                        }
                    });
                };

                directiveAPI.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('ExtensionConfigurationId', attrs, ctrl);
                };

                if (ctrl.onReady && typeof ctrl.onReady == 'function')
                    ctrl.onReady(directiveAPI);
            }
        }

        function getDirectiveTemplate(attrs) {

            var label = attrs.label ? attrs.label : 'Connection Types';
            var ismultipleselection = '';
            if (attrs.ismultipleselection != undefined && attrs.ismultipleselection != null) {
                label = label == 'Connection Type' ? 'Connection Types' : label;
                ismultipleselection = ' ismultipleselection';
            }

            var hideremoveicon = (attrs.hideremoveicon != undefined && attrs.hideremoveicon != null) ? 'hideremoveicon' : null;

            return '<div>'
                + '<vr-select on-ready="ctrl.onSelectorReady"'
                + ' label="' + label + '"'
                + ' datasource="ctrl.datasource"'
                + ' selectedvalues="ctrl.selectedvalues"'
                + ' onselectionchanged="ctrl.onselectionchanged"'
                + ' onselectitem="ctrl.onselectitem"'
                + ' ondeselectitem="ctrl.ondeselectitem"'
                + ' datavaluefield="ExtensionConfigurationId"'
                + ' datatextfield="Title"'
                + ismultipleselection
                + ' vr-disabled="ctrl.isdisabled"'
                + ' isrequired="ctrl.isrequired"'
                + hideremoveicon
                + ' entityName="' + label + '">'
                + '</vr-select>'
                + '</div>';
        }

        return directiveDefinitionObject;
    }

    app.directive('vrCommonConnectiontypeconfigSelector', ConnectionTypeSettingsDirective);
})(app);