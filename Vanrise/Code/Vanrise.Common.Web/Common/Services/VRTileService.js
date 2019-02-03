
(function (appControllers) {

    'use strict';

    VRTileService.$inject = ['VRModalService', 'UtilsService'];

    function VRTileService(VRModalService, UtilsService) {
        return {
            addVRTile: addVRTile,
            editVRTile: editVRTile,
            editFiguresTileQuery: editFiguresTileQuery,
            addFiguresTileQuery: addFiguresTileQuery

        };

        function addVRTile(onVRTileAdded) {
            var modalParameters = {

            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVRTileAdded = onVRTileAdded;
            };

            VRModalService.showModal('/Client/Modules/Common/Directives/VRTile/Templates/VRTileEditor.html', modalParameters, modalSettings);
        }

        function editVRTile(vrTileEntity, onVRTileUpdated) {
            var modalParameters = {
                vrTileEntity: vrTileEntity
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVRTileUpdated = onVRTileUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Directives/VRTile/Templates/VRTileEditor.html', modalParameters, modalSettings);
        }

        function editFiguresTileQuery(figuresTileQueryEntity, onFigureTileQueryUpdated, figureTileQueries) {
            var modalParameters = {
                figuresTileQueryEntity: figuresTileQueryEntity,
                figureTileQueries: figureTileQueries
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onFigureTileQueryUpdated = onFigureTileQueryUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/VRTile/FiguresTileQueryEditor.html', modalParameters, modalSettings);
        }

        function addFiguresTileQuery(onFigureTileQueryAdded,figureTileQueries) {
            var modalParameters = {
                figureTileQueries: figureTileQueries
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onFigureTileQueryAdded = onFigureTileQueryAdded;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/VRTile/FiguresTileQueryEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VRCommon_VRTileService', VRTileService);

})(appControllers);