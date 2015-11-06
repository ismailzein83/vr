'use strict';
app.directive('vrWhsBeZoneServiceConfigSelector', ['WhS_BE_ZoneServiceConfigAPIService', 'WhS_BE_MainService', 'UtilsService', '$compile', function (WhS_BE_ZoneServiceConfigAPIService, WhS_BE_MainService, UtilsService, $compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            type: "=",
            onReady: '=',
            label: "@",
            ismultipleselection: "@",
            hideselectedvaluessection: '@',
            onselectionchanged: '=',
            isrequired: '@',
            isdisabled: "=",
            selectedvalues: "=",
            showaddbutton: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            $scope.selectedZoneServiceConfigValues;
            if ($attrs.ismultipleselection != undefined)
                $scope.selectedZoneServiceConfigValues = [];

            $scope.addNewZoneServiceConfig = function () {
                var onZoneServiceConfigAdded = function (zoneServiceConfigObj) {
                    $scope.datasource.length = 0;
                    return getAllZoneServiceConfigs($scope, WhS_BE_ZoneServiceConfigAPIService).then(function (response) {
                        if ($attrs.ismultipleselection == undefined)
                            $scope.selectedZoneServiceConfigValues = UtilsService.getItemByVal($scope.datasource, zoneServiceConfigObj.ZoneServiceConfigId, "ZoneServiceConfigId");
                    }).catch(function (error) {
                    }).finally(function () {

                    });;
                };
                WhS_BE_MainService.addZoneServiceConfig(onZoneServiceConfigAdded);
            }
            $scope.datasource = [];
            var beZoneServiceConfig = new BeZoneServiceConfig(ctrl, $scope, WhS_BE_ZoneServiceConfigAPIService, $attrs);
            beZoneServiceConfig.initializeController();
            $scope.onselectionchanged = function () {
                ctrl.selectedvalues = $scope.selectedZoneServiceConfigValues;
                if (ctrl.onselectionchanged != undefined) {
                    var onvaluechangedMethod = $scope.$parent.$eval(ctrl.onselectionchanged);
                    if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                        onvaluechangedMethod();
                    }
                }

            }

        },
        controllerAs: 'ctrl',
        bindToController: true,
        link: function preLink($scope, iElement, iAttrs) {
            var ctrl = $scope.ctrl;
            $scope.$watch('ctrl.isdisabled', function () {
                var template = getBeZoneServiceConfigTemplate(iAttrs, ctrl);
                iElement.html(template);
                $compile(iElement.contents())($scope);
            });
        }

    };
    function getBeZoneServiceConfigTemplate(attrs, ctrl) {
        var label;
        var disabled = "";
        if (ctrl.isdisabled)
            disabled = "vr-disabled='true'"

        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";

        var hideselectedvaluessection = "";
        if (attrs.hideselectedvaluessection != undefined)
            hideselectedvaluessection = "hideselectedvaluessection";
        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewZoneServiceConfig"';
        if (attrs.ismultipleselection != undefined)
            return ' <vr-select ismultipleselection datasource="datasource" ' + required + ' ' + hideselectedvaluessection + ' selectedvalues="selectedZoneServiceConfigValues" ' + disabled + ' onselectionchanged="onselectionchanged" datatextfield="Name" datavaluefield="ZoneServiceConfigId"'
                   + 'entityname="ZoneServiceConfig" label="ZoneServiceConfig" ' + addCliked + '></vr-select>';
        else
            return '<div vr-loader="isLoadingDirective" style="display:inline-block;width:100%">'
               + ' <vr-select datasource="datasource" selectedvalues="selectedZoneServiceConfigValues" ' + required + ' ' + hideselectedvaluessection + ' onselectionchanged="onselectionchanged"  ' + disabled + ' datatextfield="Name" datavaluefield="ZoneServiceConfigId"'
               + 'entityname="ZoneServiceConfig" label="ZoneServiceConfig" ' + addCliked + '></vr-select></div>';
    }
    function BeZoneServiceConfig(ctrl, $scope, WhS_BE_ZoneServiceConfigAPIService, $attrs) {

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};


            api.getData = function () {
                return $scope.selectedZoneServiceConfigValues;
            }
            api.getDataId = function () {
                return $scope.selectedZoneServiceConfigValues.ZoneServiceConfigId;
            }
            api.getIdsData = function () {
                return getIdsList($scope.selectedZoneServiceConfigValues, "ZoneServiceConfigId");
            }
            api.setData = function (selectedIds) {
                if ($attrs.ismultipleselection != undefined) {
                    for (var i = 0; i < selectedIds.length; i++) {
                        var selectedZoneServiceConfigValue = UtilsService.getItemByVal($scope.datasource, selectedIds[i], "ZoneServiceConfigId");
                        if (selectedZoneServiceConfigValue != null)
                            $scope.selectedZoneServiceConfigValues.push(selectedZoneServiceConfigValue);
                    }
                } else {
                    var selectedZoneServiceConfigValue = UtilsService.getItemByVal($scope.datasource, selectedIds, "ZoneServiceConfigId");
                    if (selectedZoneServiceConfigValue != null)
                        $scope.selectedZoneServiceConfigValues = selectedZoneServiceConfigValue;
                }
            }
            function getIdsList(tab, attname) {
                var list = [];
                for (var i = 0; i < tab.length ; i++)
                    list[list.length] = tab[i][attname];
                return list;

            }
            api.load = function () {

                return WhS_BE_ZoneServiceConfigAPIService.GetAllZoneServiceConfigs().then(function (response) {
                    angular.forEach(response, function (itm) {
                        $scope.datasource.push(itm);
                    });
                }).catch(function (error) {
                }).finally(function () {

                });;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }

    function getAllZoneServiceConfigs($scope, WhS_BE_ZoneServiceConfigAPIService) {
        return WhS_BE_ZoneServiceConfigAPIService.GetAllZoneServiceConfigs().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.datasource.push(itm);
            });
        });
    }
    return directiveDefinitionObject;
}]);

