
app.service('VRCommon_TextManipulationService', ['VRModalService',
    function (VRModalService) {
        function editTextManipulation(onTextManipulationSave) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onTextManipulationSave = onTextManipulationSave;
            };
            var parameters = {
            };

            VRModalService.showModal('/Client/Modules/Common/Views/TextManipulation/TextManipulationEditor.html', parameters, settings);
        }
        return ({
            editTextManipulation: editTextManipulation
        });
    }]);
