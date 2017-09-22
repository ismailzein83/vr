'use strict';

app.directive('vrWhsBeRatechangetypesettingsGrid', ['WhS_BE_RateChangeTypeEnum', 'UtilsService',
    function (WhS_BE_RateChangeTypeEnum, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var rateChangeTypeSettingsGrid = new RateChangeTypeSettingsGrid($scope, ctrl, $attrs);
                rateChangeTypeSettingsGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/RateChangeTypeSettingsGridTemplate.html'
        };

        function RateChangeTypeSettingsGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.gridData = [];
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    var rateChangeTypeList = [];
                    var rateChangeTypeEnumList = UtilsService.getArrayEnum(WhS_BE_RateChangeTypeEnum);

                    for (var i = 0, l = rateChangeTypeEnumList.length; i < l; i++) {

                        var rateChangeType = rateChangeTypeEnumList[i];

                        switch (rateChangeType) {
                            case (WhS_BE_RateChangeTypeEnum.NotChanged):
                                {
                                    var dataItem = {
                                        RateChangeType: rateChangeType,
                                        RateChangeTypeDescription: (dataRetrievalInput.Query != undefined) ? dataRetrievalInput.Query.NotChanged : null,
                                        isrequired: ctrl.isrequired
                                    };
                                    rateChangeTypeList.push(dataItem);
                                    break;
                                }
                            case (WhS_BE_RateChangeTypeEnum.New):
                                {
                                    var dataItem = {
                                        RateChangeType: rateChangeType,
                                        RateChangeTypeDescription: (dataRetrievalInput.Query != undefined) ? dataRetrievalInput.Query.NewRate : null,
                                        isrequired: ctrl.isrequired
                                    };
                                    rateChangeTypeList.push(dataItem);
                                    break;
                                }
                            case (WhS_BE_RateChangeTypeEnum.Increase):
                                {
                                    var dataItem = {
                                        RateChangeType: rateChangeType,
                                        RateChangeTypeDescription: (dataRetrievalInput.Query != undefined) ? dataRetrievalInput.Query.IncreasedRate : null,
                                        isrequired: ctrl.isrequired
                                    };
                                    rateChangeTypeList.push(dataItem);
                                    break;
                                }
                            case (WhS_BE_RateChangeTypeEnum.Decrease):
                                {
                                    var dataItem = {
                                        RateChangeType: rateChangeType,
                                        RateChangeTypeDescription: (dataRetrievalInput.Query != undefined) ? dataRetrievalInput.Query.DecreasedRate : null,
                                        isrequired: ctrl.isrequired
                                    };
                                    rateChangeTypeList.push(dataItem);
                                    break;
                                }
                        }
                    }

                    onResponseReady({
                        Data: rateChangeTypeList
                    });
                };
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    return gridAPI.retrieveData(payload.rateChangeTypeSettings);
                };

                api.getData = function () {
                    var rateChangeTypeSettings = {};

                    for (var i = 0, l = $scope.scopeModel.gridData.length; i < l; i++) {

                        var dataItem = $scope.scopeModel.gridData[i];

                        switch (dataItem.RateChangeType) {
                            case (WhS_BE_RateChangeTypeEnum.NotChanged):
                                {
                                    rateChangeTypeSettings.NotChanged = dataItem.RateChangeTypeDescription;
                                    break;
                                }
                            case (WhS_BE_RateChangeTypeEnum.New):
                                {
                                    rateChangeTypeSettings.NewRate = dataItem.RateChangeTypeDescription;
                                    break;
                                }
                            case (WhS_BE_RateChangeTypeEnum.Increase):
                                {
                                    rateChangeTypeSettings.IncreasedRate = dataItem.RateChangeTypeDescription;
                                    break;
                                }
                            case (WhS_BE_RateChangeTypeEnum.Decrease):
                                {
                                    rateChangeTypeSettings.DecreasedRate = dataItem.RateChangeTypeDescription;
                                    break;
                                }
                        }
                    }
                    return rateChangeTypeSettings;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);