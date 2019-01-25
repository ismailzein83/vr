'use strict';

app.directive('vrCommonMasterlayoutSettingsEditor', ['UtilsService', 'VRUIUtilsService', 'MasterLayoutMenuOptionEnum',
function (UtilsService, VRUIUtilsService, MasterLayoutMenuOptionEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Common/Directives/MasterLayoutSettings/Templates/MasterLayoutTemplateSettings.html"
    };

    function settingEditorCtor(ctrl, $scope, $attrs) {

        var menuOptionApi;
        var menuOptionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        $scope.scopeModel = {};
        $scope.scopeModel.onMenuOptionSelectReady = function (api) {
            menuOptionApi = api;
            menuOptionSelectorReadyDeferred.resolve();
        };

        $scope.scopeModel.onMenuOptionSelectIonChanged = function () {
            var selectedId = menuOptionApi.getSelectedIds()
            if (selectedId != undefined) {
                $scope.fullMenu = false;
                $scope.moduleFilter = false;
                $scope.noMenu = false;
                switch (selectedId) {
                    case MasterLayoutMenuOptionEnum.FullMenu.value:
                        $scope.fullMenu = true;
                        
                        break;
                    case MasterLayoutMenuOptionEnum.ModuleFilteredMenu.value:
                        $scope.moduleFilter = true;
                        $scope.scopeModel.expandedMenu = false;
                        $scope.scopeModel.showApplicationTiles = true;
                        break;
                    case MasterLayoutMenuOptionEnum.NoMenu.value:
                        $scope.noMenu = true;
                        $scope.scopeModel.expandedMenu = false;
                        $scope.scopeModel.isBreadcrumbVisible = true;
                        $scope.scopeModel.showApplicationTiles = true;
                        $scope.scopeModel.showModuleTiles = true;
                        break;
                    default:
                        $scope.fullMenu = true;
                        break;
                }
            }
        };
        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                var menuOptionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(menuOptionSelectorLoadDeferred.promise);
                if (payload != undefined && payload.data != undefined) {
                    $scope.scopeModel.expandedMenu = payload.data.ExpandedMenu;
                    $scope.scopeModel.isBreadcrumbVisible = payload.data.IsBreadcrumbVisible;
                    $scope.scopeModel.showApplicationTiles = payload.data.ShowApplicationTiles;
                    $scope.scopeModel.showModuleTiles = payload.data.ShowModuleTiles;

                    $scope.scopeModel.tilesMode = payload.data.TilesMode;
                    $scope.scopeModel.moduleTilesMode = payload.data.ModuleTilesMode;
                }

                menuOptionSelectorReadyDeferred.promise.then(function () {
                    var selectorPayload = {
                        selectedIds: payload != undefined && payload.data != undefined ? payload.data.MenuOption : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(menuOptionApi, selectorPayload, menuOptionSelectorLoadDeferred);
                });


                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.Entities.MasterLayoutSettingData, Vanrise.Entities",
                    MenuOption: menuOptionApi.getSelectedIds(),
                    ExpandedMenu: $scope.scopeModel.expandedMenu,
                    IsBreadcrumbVisible: $scope.scopeModel.isBreadcrumbVisible,
                    ShowApplicationTiles: $scope.scopeModel.showApplicationTiles,
                    ShowModuleTiles: $scope.scopeModel.showModuleTiles,
                    TilesMode: $scope.scopeModel.tilesMode,
                    ModuleTilesMode: $scope.scopeModel.moduleTilesMode
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);
