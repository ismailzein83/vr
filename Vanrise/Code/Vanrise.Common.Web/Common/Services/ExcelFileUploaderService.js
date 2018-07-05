
app.service('VRCommon_ExcelFileUploaderService', ['VRModalService',
function (VRModalService) {

    function addExcelSheets(onExcelAdded) {

        var settings = {};
        var parameters = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onExcelAdded = onExcelAdded;

        };

        VRModalService.showModal('/Client/Modules/Common//Views/ExcelFileUploader/ExcelFileUploaderEditor.html', parameters, settings);
    };

  
    return {
        addExcelSheets: addExcelSheets
    };

}]);