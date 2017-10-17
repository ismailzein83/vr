(function (app) {

    'use strict';

    AccountstatusSelector.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_AccountBalance_AccountStatusEnum','VRDateTimeService'];

    function AccountstatusSelector(UtilsService, VRUIUtilsService, VR_AccountBalance_AccountStatusEnum, VRDateTimeService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectitem: "=",
                ondeselectitem: "=",
                onselectionchanged: '=',
                ismultipleselection: "@",
                isrequired: "=",
                customlabel: "@",
                normalColNum: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

                var ctor = new AccountStatus(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function AccountStatus(ctrl, $scope, attrs) {
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
                    ctrl.datasource.length = 0;

                    var filter;
                    var selectedIds;
                    var selectFirstItem;
                    var dontShowInActive;

                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                        selectFirstItem = payload.selectFirstItem;
                        dontShowInActive = payload.dontShowInActive;
                    }
                    setTimeout(function () {
                        ctrl.datasource.push(VR_AccountBalance_AccountStatusEnum.Active);
                        if (dontShowInActive) {
                            ctrl.datasource.push(VR_AccountBalance_AccountStatusEnum.ActiveAndExpired);
                        } else {
                            ctrl.datasource.push(VR_AccountBalance_AccountStatusEnum.All);
                        }
                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                        } else if (selectFirstItem == true) {
                            var defaultValue = attrs.ismultipleselection != undefined ? [ctrl.datasource[0].value] : ctrl.datasource[0].value;
                            VRUIUtilsService.setSelectedValues(defaultValue, 'value', attrs, ctrl);
                        }
                    });
                   
                };

                api.getData = function () {
                    return getData();
                };

                function getData() {
                    var selectedValue = VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                    if (selectedValue != undefined) {
                        var returnedObj = {};
                        switch (selectedValue) {
                            case VR_AccountBalance_AccountStatusEnum.Active.value:
                                returnedObj.Status = selectedValue;
                                returnedObj.IsEffectiveInFuture = true;
                                returnedObj.EffectiveDate = VRDateTimeService.getNowDateTime();
                                break;
                            case VR_AccountBalance_AccountStatusEnum.All.value:
                                returnedObj.Status = undefined;
                                returnedObj.IsEffectiveInFuture = undefined;
                                returnedObj.EffectiveDate = undefined;
                                break;
                            case VR_AccountBalance_AccountStatusEnum.ActiveAndExpired.value:
                                returnedObj.Status = VR_AccountBalance_AccountStatusEnum.Active.value;
                                returnedObj.IsEffectiveInFuture = undefined;
                                returnedObj.EffectiveDate = undefined;
                                break;
                        }
                        return returnedObj;
                    }
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = 'Account Status';
            if (attrs.ismultipleselection != undefined) {
                label = 'Account Statuses';
                multipleselection = 'ismultipleselection';
            }

            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }

            var hideselectedvaluessection = (attrs.hideselectedvaluessection != undefined) ? 'hideselectedvaluessection' : null;

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : null;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + '<vr-select on-ready="ctrl.onSelectorReady"'
                    + ' datasource="ctrl.datasource"'
                    + ' selectedvalues="ctrl.selectedvalues"'
                    + ' onselectionchanged="ctrl.onselectionchanged"'
                    + ' onselectitem="ctrl.onselectitem"'
                    + ' ondeselectitem="ctrl.ondeselectitem"'
                    + ' datavaluefield="value"'
                    + ' datatextfield="description"'
                    + ' ' + multipleselection
                    + ' ' + hideselectedvaluessection
                    + ' isrequired="ctrl.isrequired"'
                    + ' ' + hideremoveicon
                    + ' label="' + label + '"'
                    + ' entityName="' + label + '"'
                + '</vr-select>'
            + '</vr-columns>';
        }
    }

    app.directive('vrAccountbalanceAccountstatusSelector', AccountstatusSelector);

})(app);
