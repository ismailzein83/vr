﻿(function (app) {

    'use strict';

    AccountManagerdefinitionSelectorDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function AccountManagerdefinitionSelectorDirective(UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                onselectionchanged: '=',
                ismultipleselection: '@',
                isrequired: '=',
                normalColNum: '@',
                selectedvalues: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined && $attrs.ismultipleselection != null)
                    ctrl.selectedvalues = [];
                var genericAccountManagerDefinitionSelector = new GenericAccountManagerDefinitionSelector(ctrl, $scope, $attrs);
                genericAccountManagerDefinitionSelector.initializeController();
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

        function GenericAccountManagerDefinitionSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorApi = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };
                beDefinitionSelectorPromiseDeferred.promise.then(function () {
                    if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                });
            }

            function getDirectiveAPI() {
                var directiveAPI = {};

                directiveAPI.load = function (payload) {
                    var selectedIds;
                    var filter;
                    var selectFirstItem;
                    if (payload) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                        selectFirstItem = payload.selectFirstItem;
                    }

                    var promises = [];
                    promises.push(loadBusinessEntityDefinitionSelector());
                       

                    function loadBusinessEntityDefinitionSelector() {
                        if (filter == undefined)
                            filter = {};
                        if (filter.Filters == undefined)
                            filter.Filters = [];
                        filter.Filters.push({
                            $type: "Vanrise.AccountManager.Business.AccountManagerBEDefinitionFilter, Vanrise.AccountManager.Business",
                        });
                        var payloadSelector = {
                            selectedIds: selectedIds,
                            filter: filter,
                            selectFirstItem:selectFirstItem
                        };
                        return beDefinitionSelectorApi.load(payloadSelector);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                directiveAPI.getSelectedIds = function () {
                    return beDefinitionSelectorApi.getSelectedIds();
                };
                directiveAPI.hasSingleItem = function () {
                    return beDefinitionSelectorApi.hasSingleItem();
                };
                return directiveAPI;
            }
        }

        function getDirectiveTemplate(attrs) {
            var ismultipleselection = '';
            var label = 'Account Manager Definitions';
            if (attrs.ismultipleselection != undefined && attrs.ismultipleselection != null) {
                ismultipleselection = ' ismultipleselection';
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;
            return '<vr-columns colnum="{{ctrl.normalColNum}}"> '
                  +  ' <vr-genericdata-businessentitydefinition-selector on-ready="scopeModel.onBusinessEntityDefinitionSelectorReady"'
                 +    ' isrequired="ctrl.isrequired" '
                  +   ' selectedvalues="ctrl.selectedvalues" '
                   +   'customlabel="' + label + '"'
                       + ismultipleselection 
                  +   ' onselectionchanged="ctrl.onselectionchanged">'
                +  ' </vr-genericdata-businessentitydefinition-selector>'
                   +' </vr-columns>';
        }

        return directiveDefinitionObject;
    }

    app.directive('vrAccountmanagerAccountmanagerdefinitionSelector', AccountManagerdefinitionSelectorDirective);

})(app);
