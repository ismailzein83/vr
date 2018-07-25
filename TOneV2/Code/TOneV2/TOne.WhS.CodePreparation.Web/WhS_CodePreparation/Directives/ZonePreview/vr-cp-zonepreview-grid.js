"use strict";

app.directive("vrCpZonepreviewGrid", ["WhS_CP_CodePreparationPreviewAPIService", "WhS_CP_ZoneChangeTypeEnum", "VRUIUtilsService", "VRNotificationService",
function (WhS_CP_CodePreparationPreviewAPIService, WhS_CP_ZoneChangeTypeEnum, VRUIUtilsService, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var zonePreviewGrid = new ZonePreviewGrid($scope, ctrl, $attrs);
            zonePreviewGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_CodePreparation/Directives/ZonePreview/Templates/CodePreparationZonePreviewGridTemplate.html"

    };

    function ZonePreviewGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var drillDownManager;
        var onlyModified;
        var processInstanceId;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.changedZones = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(getDirectiveTabs(), gridAPI);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.load = function (query) {
                        processInstanceId = query.ProcessInstanceId;
                        onlyModified = query.OnlyModified;
                        return gridAPI.retrieveData(query);
                    };

                    return directiveAPI;
                }
            };


            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_CP_CodePreparationPreviewAPIService.GetFilteredZonePreview(dataRetrievalInput)
                    .then(function (response) {

                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                mapDataNeeded(response.Data[i]);
                                drillDownManager.setDrillDownExtensionObject(response.Data[i]);
                            }

                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
        }

        function getDirectiveTabs() {
            var directiveTabs = [];

            var codesTab = {
                title: "Codes",
                directive: "vr-cp-codepreview-grid",
                loadDirective: function (directiveAPI, zoneDataItem) {
                    zoneDataItem.codeGridAPI = directiveAPI;

                    var codeGridPayload = {
                        ProcessInstanceId: processInstanceId,
                        ZoneName: zoneDataItem.ZoneName,
                        OnlyModified: onlyModified,
                        ShowZoneName : false
                    };

                    return zoneDataItem.codeGridAPI.load(codeGridPayload);
                }
            };

            directiveTabs.push(codesTab);
            return directiveTabs;
        }


        function mapDataNeeded(dataItem) {
            if (!onlyModified) {
                if (dataItem.NewCodes > 0 || dataItem.DeletedCodes > 0 || dataItem.CodesMovedTo > 0 || dataItem.CodesMovedFrom > 0) {
                    dataItem.ZoneStatusIconUrl = "Client/Modules/WhS_BusinessEntity/Images/Modified.png";
                    dataItem.ZoneStatusIconTooltip = "Modified";
                }
            }

            switch (dataItem.ChangeTypeZone) {

                case WhS_CP_ZoneChangeTypeEnum.New.value:
                    dataItem.ZoneStatusIconUrl = WhS_CP_ZoneChangeTypeEnum.New.icon;
                    dataItem.ZoneStatusIconTooltip = WhS_CP_ZoneChangeTypeEnum.New.label;
                    break;

                case WhS_CP_ZoneChangeTypeEnum.Closed.value:
                    dataItem.ZoneStatusIconUrl = WhS_CP_ZoneChangeTypeEnum.Closed.icon;
                    dataItem.ZoneStatusIconTooltip = WhS_CP_ZoneChangeTypeEnum.Closed.label;
                    break;

                case WhS_CP_ZoneChangeTypeEnum.Renamed.value:
                    dataItem.ZoneStatusIconUrl = WhS_CP_ZoneChangeTypeEnum.Renamed.icon;
                    dataItem.ZoneStatusIconTooltip = WhS_CP_ZoneChangeTypeEnum.Renamed.label;
                    break;
            }
        }

    }

    return directiveDefinitionObject;

}]);
