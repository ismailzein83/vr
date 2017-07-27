'use strict';
app.directive('demoModuleRoomSelector', ['VRNotificationService', 'Demo_Module_RoomAPIService', 'Demo_Module_RoomService','UtilsService','VRUIUtilsService',
function (VRNotificationService, Demo_Module_RoomAPIService, Demo_Module_RoomService, UtilsService,VRUIUtilsService) {

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
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            $scope.addNewRoom = function () {
                var onRoomAdded = function (roomObj) {
                    ctrl.datasource.push(roomObj.Entity);
                    if ($attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(roomObj.Entity);
                    else
                        ctrl.selectedvalues = roomObj.Entity;
                };
                Demo_Module_RoomService.addRoom(onRoomAdded);
            };

            var roomSelector = new RoomSelector(ctrl, $scope, $attrs);
            roomSelector.initializeController();
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
            return getRoomTemplate(attrs);
        }

    };

    function getRoomTemplate(attrs) {

        var multipleselection = "";
        var label = "Room";
        if (attrs.ismultipleselection != undefined) {
            label = "Rooms";
            multipleselection = "ismultipleselection";
        }

        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewRoom"';

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns colnum="{{ctrl.normalColNum}}"  haschildcolumns  ><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Name" datavaluefield="RoomId" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Room" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '>'

            + '</vr-select></vr-columns>';
    }

    function RoomSelector(ctrl, $scope, attrs) {

        var selectorAPI;

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };

            $scope.scopeModel.onCancelSearch = function (api) {
                $scope.scopeModel.searchroom = undefined;
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                selectorAPI.clearDataSource();

                var selectedIds;
                var filter;
                if (payload != undefined) {
                    if (payload.selectedIds!=undefined) {
                        selectedIds = [];
                    selectedIds.push(payload.selectedIds);
                    }
                    filter = payload.filter;
                }
                return Demo_Module_RoomAPIService.GetRoomsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'RoomId', attrs, ctrl);
                        }
                    }
                });


                var promise;
                var def = UtilsService.createPromiseDeferred();
                setTimeout(function () { pre = getRoomsInfo(attrs, ctrl, selectedIds, buildingIds); def.resolve();}, 3000);
                def.promise.then(function () { return pre; })

            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('RoomId', attrs, ctrl);
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);