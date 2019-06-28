'use strict';
app.directive('vrSecUserSelector', ['VR_Sec_UserAPIService', 'VR_Sec_UserService', 'UtilsService', 'VRUIUtilsService',

    function (VR_Sec_UserAPIService, VR_Sec_UserService, UtilsService, VRUIUtilsService) {

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
                isdisabled: "=",
                customlabel: "@",
                onitemadded: "=",
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {

				var ctrl = this;
				var multipleselection;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                ctrl.label = "User";

                if ($attrs.ismultipleselection != undefined) {
                    ctrl.selectedvalues = [];
                    ctrl.label = "Users";
                    multipleselection = "ismultipleselection";
                }

                $scope.addNewUser = function () {
                    var onUserAdded = function (userObj) {
                        if (userObj.Entity.Status == 1 || (ctrl.filter != undefined && ctrl.filter.ExcludeInactive == false)) {
                            ctrl.datasource.push(userObj.Entity);
                            if ($attrs.ismultipleselection != undefined)
                                ctrl.selectedvalues.push(userObj.Entity);
                            else
                                ctrl.selectedvalues = userObj.Entity;

                            if (ctrl.onitemadded != null && typeof (ctrl.onitemadded) == 'function')
                                ctrl.onitemadded(userObj.Entity);
                        }

                    };
                    VR_Sec_UserService.addUser(onUserAdded);
                };

                ctrl.haspermission = function () {
                    return VR_Sec_UserAPIService.HasAddUserPermission();
                };

                var ctor = new userCtor(ctrl, $scope, $attrs);
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
                return getUserTemplate(attrs);
            }

        };


        function getUserTemplate(attrs) {

            var multipleselection = "";

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewUser"';

            var hidelabel = '';
            if (attrs.hidelabel != undefined)
                hidelabel = "hidelabel";

            return '<vr-columns  colnum="{{ctrl.normalColNum}}">'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="UserId" isrequired="ctrl.isrequired" ' + hidelabel
                + ' label="{{ctrl.label}}" ' + addCliked + ' datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="User" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" haspermission="ctrl.haspermission"></vr-select>'
                + '</vr-columns>';
        }

        function userCtor(ctrl, $scope, attrs) {

            var selectorApi;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorApi.clearDataSource();
                    var selectedIds;
                    ctrl.filter;
                    if (payload != undefined) {
                        ctrl.filter = payload.filter;
                        selectedIds = payload.selectedIds;

                        if (payload.fieldTitle != undefined) {
                            ctrl.label = payload.fieldTitle;
                        }
                    }
                    return VR_Sec_UserAPIService.GetUsersInfo(UtilsService.serializetoJson(ctrl.filter)).then(function (response) {
                        ctrl.datasource.length = 0;

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'UserId', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('UserId', attrs, ctrl);
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);