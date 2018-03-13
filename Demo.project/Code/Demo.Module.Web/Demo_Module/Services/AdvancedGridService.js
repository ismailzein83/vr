"use strict";
app.service("Demo_Module_AdvancedGridService", ["VRModalService",
    function (VRModalService) {

        function addGridItem(onGridItemAdded, context) {
            console.log("in add grid service");
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onGridItemAdded = onGridItemAdded;
            };
            var parameters = {
                //context: context
            };
            VRModalService.showModal("/Client/Modules/Demo_Module/Views/AdvancedGridEditor.html", parameters, settings);
        };
        function editGridItem(gridItemEntity, onGridItemUpdated, context) {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onGridItemUpdated = onGridItemUpdated;
            };
            var parameters = {
                gridItemEntity: gridItemEntity,
                //context: context
            };
            VRModalService.showModal("/Client/Modules/Demo_Module/Views/AdvancedGridEditor.html", parameters, settings);
        };
        return {
            addGridItem: addGridItem,
            editGridItem: editGridItem
        }
}]);