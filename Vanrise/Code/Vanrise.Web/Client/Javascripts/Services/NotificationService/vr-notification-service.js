'use strict';

app.service('VRNotificationService', function (VRModalService, VRNavigationService, InsertOperationResultEnum, UpdateOperationResultEnum, DeleteOperationResultEnum, AuthenticateOperationResultEnum, $q, notify, $location) {

    return ({
        showConfirmation: showConfirmation,
        showInformation: showInformation,
        showPromptWarning: showPromptWarning,
        showSuccess: showSuccess,
        showError: showError,
        showWarning: showWarning,
        notifyException: notifyException,
        notifyExceptionWithClose: notifyExceptionWithClose,
        notifyOnItemAdded: notifyOnItemAdded,
        notifyOnItemUpdated: notifyOnItemUpdated,
        notifyOnItemDeleted: notifyOnItemDeleted,
        notifyOnUserAuthenticated: notifyOnUserAuthenticated
    });

    function showConfirmation(message) {
        var settings = {
            size: "small"
        };
        var deferred = $q.defer();
        settings.onScopeReady = function (modalScope) {
            if (message != null && message != undefined && message != "")
                modalScope.message = message;
            else
                modalScope.message = "Are you sure you want to continue?";

            modalScope.yesClicked = function () {
                modalScope.modalContext.closeModal();
                deferred.resolve(true);
            };
            modalScope.noClicked = function () {
                modalScope.modalContext.closeModal();
                deferred.resolve(false);
            };
        };
        VRModalService.showModal('/Client/Javascripts/Services/NotificationService/vr-confirmation-modal.html', null, settings);
        return deferred.promise;
    }

    function showPromptWarning(message) {
        var settings = {
            size: "small"
        };
        var deferred = $q.defer();
        settings.onScopeReady = function (modalScope) {
            if (message != null && message != undefined && message != "")
                modalScope.message = message;
            else
                modalScope.message = "Prompt message!!";
            modalScope.type = "alert  alert-warning";
            modalScope.okClicked = function () {
                modalScope.modalContext.closeModal();
                deferred.resolve(true);
            };
           
        };
        VRModalService.showModal('/Client/Javascripts/Services/NotificationService/vr-prompt-modal.html', null, settings);
        return deferred.promise;
    }

    function showInformation(message) {
        showNotificationMessage(message, "alert  alert-info");
    }

    function showSuccess(message) {
        showNotificationMessage(message, "alert  alert-success");
    }

    function showError(message) {
        showNotificationMessage(message, "alert alert-danger");
    }

    function showWarning(message) {
        showNotificationMessage(message, "alert  alert-warning");
    }

    function showNotificationMessage(message, cssClasses) {
        notify.closeAll();
        notify({ message: message, classes: cssClasses });
        setTimeout(function () {
            notify.closeAll();
        }, 5000);
    }

    function notifyException(error, scope) {
        showError("An error has occured");
    }

    function notifyExceptionWithClose(error, scope) {
        if (scope != undefined && scope.modalContext != undefined) {
            scope.modalContext.closeModal();
            notifyException(error, scope);
        }
        else {
            var parameters = {
                error: error,
                previousUrl: $location.url()
            };
            VRNavigationService.goto("/Common/Views/Error", parameters);
        }
    }

    function notifyOnItemAction(notificationType, message, operationOutput) {
        var additionalMessage = operationOutput.Message;
        if (operationOutput.ShowExactMessage)
            message = additionalMessage;
        else if (additionalMessage != null && additionalMessage != undefined && additionalMessage != "")
            message += " (" + additionalMessage + ")";

        switch (notificationType) {
            case "success":
                showSuccess(message); break;
            case "warning":
                showWarning(message); break;
            case "error":
                showError(message); break;
        }
    }

    function notifyOnItemAdded(itemType, insertOperationOutput, keyProperty) {//insertOperationOutput is of type InsertOperationOutput
        switch (insertOperationOutput.Result) {
            case InsertOperationResultEnum.Succeeded.value:
                var msg = itemType + " added successfully";
                notifyOnItemAction("success", msg, insertOperationOutput);
                return true;
                break;
            case InsertOperationResultEnum.Failed.value:
                var msg = "Failed to add " + itemType;
                notifyOnItemAction("error", msg, insertOperationOutput);
                break;
            case InsertOperationResultEnum.SameExists.value:
                if (keyProperty == null || keyProperty == undefined || keyProperty == "")
                    keyProperty = "key";
                var msg = itemType + " with the same " + keyProperty + " already exists";
                notifyOnItemAction("warning", msg, insertOperationOutput);
                break;
        }
        return false;
    }
    function notifyOnItemDeleted(itemType, deleteOperationOutput, usedIn) {//updateOperationOutput is of type UpdateOperationOutput
        switch (deleteOperationOutput.Result) {
            case DeleteOperationResultEnum.Succeeded.value:
                var msg = itemType + " deleted successfully";
                notifyOnItemAction("success", msg, deleteOperationOutput);
                return true;
                break;
            case DeleteOperationResultEnum.InUse.value:
                var msg = "Failed to delete " + itemType + ". It is already in use";
                if (usedIn != null && usedIn != undefined && usedIn != "")
                    msg += " in " + usedIn;
                notifyOnItemAction("error", msg, deleteOperationOutput);
                break;
            case DeleteOperationResultEnum.Failed.value:
                var msg = "Failed to delete " + itemType;
                notifyOnItemAction("error", msg, deleteOperationOutput);
                break;
        }
        return false;
    }
    function notifyOnItemUpdated(itemType, updateOperationOutput, keyProperty) {//updateOperationOutput is of type UpdateOperationOutput
        switch (updateOperationOutput.Result) {
            case UpdateOperationResultEnum.Succeeded.value:
                var msg = itemType + " updated successfully";
                notifyOnItemAction("success", msg, updateOperationOutput);
                return true;
                break;
            case UpdateOperationResultEnum.Failed.value:
                var msg = "Failed to update " + itemType;
                notifyOnItemAction("error", msg, updateOperationOutput);
                break;
            case UpdateOperationResultEnum.SameExists.value:
                if (keyProperty == null || keyProperty == undefined || keyProperty == "")
                    keyProperty = "key";
                var msg = itemType + " with the same " + keyProperty + " already exists";
                notifyOnItemAction("warning", msg, updateOperationOutput);
                break;
        }
        return false;
    }

    function notifyOnUserAuthenticated(authenticateOperationOutput, onValidationNeeded, onExpiredPasswordChangeNeeded) {
        switch (authenticateOperationOutput.Result) {
            case AuthenticateOperationResultEnum.Succeeded.value:
                return true;
                break;
            case AuthenticateOperationResultEnum.Inactive.value:
                showError(authenticateOperationOutput.Message != undefined ? authenticateOperationOutput.Message : "Login Failed. Inactive User"); break;
            case AuthenticateOperationResultEnum.WrongCredentials.value:
                showError("Login Failed. Invalid Credentials"); break;
            case AuthenticateOperationResultEnum.UserNotExists.value:
                showError("Login Failed. User does not exist"); break;
            case AuthenticateOperationResultEnum.Failed.value:
                showError("Login Failed. An error occurred"); break;
            case AuthenticateOperationResultEnum.ActivationNeeded.value:
                onValidationNeeded();
                showError("Activation Needed"); break;
            case AuthenticateOperationResultEnum.PasswordExpired.value:
                onExpiredPasswordChangeNeeded();
                showError("Password has been expired"); break;
        }
        return false;
    }
});