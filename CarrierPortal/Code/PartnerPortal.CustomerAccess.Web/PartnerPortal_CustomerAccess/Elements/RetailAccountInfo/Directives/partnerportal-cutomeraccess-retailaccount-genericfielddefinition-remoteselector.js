﻿(function (app) {

    'use strict';

    AccountGenericFieldDefinitionRemoteSelector.$inject = ['UtilsService', 'VRUIUtilsService', 'PartnerPortal_CustomerAccess_RetailAccountInfoAPIService'];

    function AccountGenericFieldDefinitionRemoteSelector(UtilsService, VRUIUtilsService, PartnerPortal_CustomerAccess_RetailAccountInfoAPIService) {
        return {
            restrict: "E",
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
                var ctor = new AccountGenericFieldDefinitionRemoteSelectorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function AccountGenericFieldDefinitionRemoteSelectorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var accountBEDefinitionId;
            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    selectorAPI.clearDataSource();
                    var selectedIds;
                    var connectionId;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        connectionId = payload.connectionId;
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                    }

                    return PartnerPortal_CustomerAccess_RetailAccountInfoAPIService.GetRemoteGenericFieldDefinitionsInfo(connectionId, accountBEDefinitionId).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined)
                                VRUIUtilsService.setSelectedValues(selectedIds, 'Name', $attrs, ctrl);
                        }
                    });
                   

                };
                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('Name', $attrs, ctrl);
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Remote Account Field";

            if (attrs.ismultipleselection != undefined) {
                label = "Remote Account Fields";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                      + '<vr-select ' + multipleselection + ' datatextfield="Title" datavaluefield="Name" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" '
                            + ' selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" '
                            + ' hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate">'
                       + '</vr-select>'
                   + '</vr-columns>';
        }
    }

    app.directive('partnerportalCutomeraccessRetailaccountGenericfielddefinitionRemoteselector', AccountGenericFieldDefinitionRemoteSelector);

})(app);
