
app.service('VRCommon_OverriddenConfigGroupService', ['VRModalService',
    function (VRModalService) {
        return ({
            addOverriddenConfigGroup: addOverriddenConfigGroup,
        });

        function addOverriddenConfigGroup(onOverriddenConfigGroupAdded) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onOverriddenConfigGroupAdded = onOverriddenConfigGroupAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/Common/Views/OverriddenConfigGroup/OverriddenConfigGroupEditor.html', parameters, settings);
        }

    }]);
