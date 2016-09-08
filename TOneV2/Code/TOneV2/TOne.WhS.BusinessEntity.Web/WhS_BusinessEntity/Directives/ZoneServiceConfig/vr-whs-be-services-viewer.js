'use strict';
app.directive('vrWhsBeServicesViewer', [
    'WhS_BE_ZoneServiceConfigAPIService', 'UtilsService', 'VRUIUtilsService',
    function (WhS_BE_ZoneServiceConfigAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.items = [];
                ctrl.selectedItems = [];
                ctrl.getSelectedItemColor = function (item) {
                    return item.Settings.Color;
                }
                var ctor = new zoneServicesViewerCtor(ctrl, $scope, $attrs);
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
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/ZoneServiceConfig/Templates/ZoneServiceViewer.html"
        };
       
        function zoneServicesViewerCtor(ctrl, $scope, attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    ctrl.selectedItems.length = 0;

                    var selectedIds;
                    var service;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        service = payload.service;
                    }
                    if (service != undefined) {
                        ctrl.selectedItems.push(service);
                    }
                    else
                        return getAllZoneServiceConfigs(ctrl, selectedIds);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        function getAllZoneServiceConfigs(ctrl, selectedIds) {
            return WhS_BE_ZoneServiceConfigAPIService.GetAllZoneServices().then(function (response) {

                ctrl.items.length = 0;

                angular.forEach(response, function (itm) {
                    ctrl.items.push(itm);
                });
                if (selectedIds != undefined) {
                    setSelctedColors(selectedIds, ctrl);
                }
            });

            function setSelctedColors(selectedIds, ctrl) {
                for (var i = 0; i < selectedIds.length; i++) {
                    var selectedValue = UtilsService.getItemByVal(ctrl.items, selectedIds[i], 'ZoneServiceConfigId');
                    if (selectedValue != null)
                        ctrl.selectedItems.push(selectedValue);
                }
            }
        }

        return directiveDefinitionObject;
    }]);

