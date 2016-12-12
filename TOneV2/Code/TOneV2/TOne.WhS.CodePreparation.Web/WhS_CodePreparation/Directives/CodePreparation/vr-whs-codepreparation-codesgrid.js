"use strict";

app.directive('vrWhsCodepreparationCodesgrid', ['VRNotificationService', 'VRUIUtilsService', 'WhS_CP_CodePrepAPIService', 'UtilsService', 'WhS_CP_CodeItemDraftStatusEnum', 'WhS_CP_CodeItemStatusEnum',
function (VRNotificationService, VRUIUtilsService, WhS_CP_CodePrepAPIService, UtilsService, WhS_CP_CodeItemDraftStatusEnum, WhS_CP_CodeItemStatusEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
            selectedcodes: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new saleCodesGrid($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_CodePreparation/Directives/CodePreparation/Templates/CodePreparationSaleCodesGridTemplate.html"

    };

    function saleCodesGrid($scope, ctrl, $attrs) {
        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.salecodes = [];
            $scope.ZoneName;

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        $scope.ZoneName = query.ZoneName;
                        $scope.ShowDraftStatus = query.ShowDraftStatus;
                        $scope.ShowSelectCode = query.ShowSelectCode;
                        return gridAPI.retrieveData(query);
                    };

                    directiveAPI.onCodeAdded = function (codeItemObject) {
                        gridAPI.itemAdded(codeItemObject);
                    };

                    directiveAPI.onCodeClosed = function (codeItemObject) {
                        gridAPI.itemDeleted(codeItemObject);
                    };
                    directiveAPI.clearUpdatedItems = gridAPI.clearUpdatedItems;
                    directiveAPI.getSelectedCodes = function () {
                        return ctrl.selectedcodes;
                    };

                    directiveAPI.isZoneClosed = function ()
                    {
                    	for (var i = 0; i < $scope.salecodes.length; i++)
                    	{
                    		var saleCode = $scope.salecodes[i];

                    		if (saleCode.DraftStatus == WhS_CP_CodeItemDraftStatusEnum.ExistingNotChanged.value) {
                    			if (saleCode.EED == null)
                    				return false;
                    		}
                    		else if (saleCode.DraftStatus != WhS_CP_CodeItemDraftStatusEnum.ExistingClosed.value && saleCode.DraftStatus != WhS_CP_CodeItemDraftStatusEnum.ClosedZoneCode.value)
                    			return false;
                    	}
                    	return true;
                    };

                    directiveAPI.hideShowRenameZone = function () {
                        for (var i = 0; i < $scope.salecodes.length; i++) {
                            var saleCode = $scope.salecodes[i];

                            if (saleCode.DraftStatus == WhS_CP_CodeItemDraftStatusEnum.ExistingClosed.value || saleCode.DraftStatus == WhS_CP_CodeItemDraftStatusEnum.MovedFrom.value
                                || saleCode.DraftStatus == WhS_CP_CodeItemDraftStatusEnum.MovedTo.value) {
                                if (saleCode.EED == null)
                                    return false;
                            }
                        }
                        return true;
                    };

                    return directiveAPI;
                }
            };

            $scope.onCodeChecked = function (dataItem) {
                if (ctrl.selectedcodes != undefined) {
                    var index = UtilsService.getItemIndexByVal(ctrl.selectedcodes, dataItem.Code, 'Code');
                    if (index >= 0 && !dataItem.IsSelected) {
                        ctrl.selectedcodes.splice(index, 1);
                    }
                    else if (index <= 0 && dataItem.IsSelected) {
                        ctrl.selectedcodes.push(dataItem);
                    }
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_CP_CodePrepAPIService.GetCodeItems(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0 ; i < response.Data.length; i++) {
                                mapDataNeeded(response.Data[i]);
                            }
                        }

                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };


            function mapDataNeeded(dataItem) {
                dataItem.ShowDraftStatusIcon = true;
                dataItem.ShowStatusIcon = dataItem.Status != null;
                dataItem.ShowSelectCode = $scope.ShowSelectCode && dataItem.DraftStatus == WhS_CP_CodeItemDraftStatusEnum.ExistingNotChanged.value && dataItem.Status == null;

                switch (dataItem.DraftStatus) {
                    case WhS_CP_CodeItemDraftStatusEnum.ExistingNotChanged.value:
                        dataItem.ShowDraftStatusIcon = false;
                        break;

                    case WhS_CP_CodeItemDraftStatusEnum.MovedFrom.value:
                        dataItem.DraftStatusIconUrl = WhS_CP_CodeItemDraftStatusEnum.MovedFrom.icon;
                        dataItem.DraftStatusIconTooltip = WhS_CP_CodeItemDraftStatusEnum.MovedTo.label + " " + dataItem.OtherCodeZoneName;
                        break;
                    case WhS_CP_CodeItemDraftStatusEnum.MovedTo.value:
                        dataItem.DraftStatusIconUrl = WhS_CP_CodeItemDraftStatusEnum.MovedTo.icon;
                        dataItem.DraftStatusIconTooltip = WhS_CP_CodeItemDraftStatusEnum.MovedFrom.label + " " + dataItem.OtherCodeZoneName;
                        break;
                    case WhS_CP_CodeItemDraftStatusEnum.ExistingClosed.value:
                        dataItem.DraftStatusIconUrl = WhS_CP_CodeItemDraftStatusEnum.ExistingClosed.icon;
                        dataItem.DraftStatusIconTooltip = WhS_CP_CodeItemDraftStatusEnum.ExistingClosed.label;
                        break;
                    case WhS_CP_CodeItemDraftStatusEnum.New.value:
                        dataItem.DraftStatusIconUrl = WhS_CP_CodeItemDraftStatusEnum.New.icon;
                        dataItem.DraftStatusIconTooltip = WhS_CP_CodeItemDraftStatusEnum.New.label;
                        break;
                    case WhS_CP_CodeItemDraftStatusEnum.ClosedZoneCode.value:
                        dataItem.DraftStatusIconUrl = WhS_CP_CodeItemDraftStatusEnum.ClosedZoneCode.icon;
                        dataItem.DraftStatusIconTooltip = WhS_CP_CodeItemDraftStatusEnum.ClosedZoneCode.label;
                    
                }


                if (dataItem.Status == WhS_CP_CodeItemStatusEnum.PendingEffective.value) {
                    dataItem.StatusIconUrl = WhS_CP_CodeItemStatusEnum.PendingEffective.icon;
                    dataItem.StatusIconTooltip = WhS_CP_CodeItemStatusEnum.PendingEffective.label;
                }
                else if (dataItem.Status == WhS_CP_CodeItemStatusEnum.PendingClosed.value) {
                    dataItem.StatusIconUrl = WhS_CP_CodeItemStatusEnum.PendingClosed.icon;
                    dataItem.StatusIconTooltip = WhS_CP_CodeItemStatusEnum.PendingClosed.label;
                }

            }
        }
    }

    return directiveDefinitionObject;

}]);
