
app.service('Retail_BE_AccountActionService', ['VRModalService', 'UtilsService', 'VRNotificationService', 'SecurityService',
    function (VRModalService, UtilsService, VRNotificationService, SecurityService) {

        var actionTypes = [];

        function getActionTypeIfExist(actionTypeName) {
            for (var i = 0; i < actionTypes.length; i++) {
                var actionType = actionTypes[i];
                if (actionType.ActionTypeName == actionTypeName)
                    return actionType;
            }
        }

        function registerActionType(actionType) {
            actionTypes.push(actionType);
        }

        function registerEditAccount() {

            var actionType = {
                ActionTypeName: "Edit",
                actionMethod: function (payload) {

                }
            };
            registerActionType(actionType);
        }

        return ({
            getActionTypeIfExist: getActionTypeIfExist,
            registerActionType: registerActionType,
            registerEditAccount: registerEditAccount
        });
    }]);
