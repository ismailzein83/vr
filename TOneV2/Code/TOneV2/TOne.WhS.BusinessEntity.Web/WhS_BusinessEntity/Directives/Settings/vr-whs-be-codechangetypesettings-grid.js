'use strict';

app.directive('vrWhsBeCodechangetypesettingsGrid', ['WhS_BE_CodeChangeTypeEnum', 'UtilsService',
    function (WhS_BE_CodeChangeTypeEnum, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var codeChangeTypeSettingsGrid = new CodeChangeTypeSettingsGrid($scope, ctrl, $attrs);
                codeChangeTypeSettingsGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/CodeChangeTypeSettingsGridTemplate.html'
        };

        function CodeChangeTypeSettingsGrid($scope, ctrl, $attrs) {

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
                    var codeChangeTypeList = [];
                    var codeChangeTypeEnumList = UtilsService.getArrayEnum(WhS_BE_CodeChangeTypeEnum);

                    for (var i = 0, l = codeChangeTypeEnumList.length; i < l; i++) {

                        var codeChangeType = codeChangeTypeEnumList[i];

                        switch (codeChangeType) {
                            case (WhS_BE_CodeChangeTypeEnum.New):
                                {
                                    var dataItem = {
                                        CodeChangeType: codeChangeType,
                                        CodeChangeTypeDescription: (dataRetrievalInput.Query != undefined) ? dataRetrievalInput.Query.NewCode : null
                                    };
                                    codeChangeTypeList.push(dataItem);
                                    break;
                                }
                            case (WhS_BE_CodeChangeTypeEnum.Closed):
                                {
                                    var dataItem = {
                                        CodeChangeType: codeChangeType,
                                        CodeChangeTypeDescription: (dataRetrievalInput.Query != undefined) ? dataRetrievalInput.Query.ClosedCode : null
                                    };
                                    codeChangeTypeList.push(dataItem);
                                    break;
                                }
                            case (WhS_BE_CodeChangeTypeEnum.NotChanged):
                                {
                                    var dataItem = {
                                        CodeChangeType: codeChangeType,
                                        CodeChangeTypeDescription: (dataRetrievalInput.Query != undefined) ? dataRetrievalInput.Query.NotChangedCode : null
                                    };
                                    codeChangeTypeList.push(dataItem);
                                    break;
                                }
                        }
                    }

                    onResponseReady({
                        Data: codeChangeTypeList
                    });
                };
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    return gridAPI.retrieveData(payload.codeChangeTypeSettings);
                };

                api.getData = function () {
                    var codeChangeTypeSettings = {};

                    for (var i = 0, l = $scope.scopeModel.gridData.length; i < l; i++) {

                        var dataItem = $scope.scopeModel.gridData[i];

                        switch (dataItem.CodeChangeType) {
                            case (WhS_BE_CodeChangeTypeEnum.New):
                                {
                                    codeChangeTypeSettings.NewCode = dataItem.CodeChangeTypeDescription;
                                    break;
                                }
                            case (WhS_BE_CodeChangeTypeEnum.Closed):
                                {
                                    codeChangeTypeSettings.ClosedCode = dataItem.CodeChangeTypeDescription;
                                    break;
                                }
                            case (WhS_BE_CodeChangeTypeEnum.NotChanged):
                                {
                                    codeChangeTypeSettings.NotChangedCode = dataItem.CodeChangeTypeDescription;
                                    break;
                                }
                        }
                    }
                    return codeChangeTypeSettings;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);