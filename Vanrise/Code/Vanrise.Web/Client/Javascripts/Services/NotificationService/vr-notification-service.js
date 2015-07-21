'use strict';

app.service('VRNotificationService', function (VRModalService, VRNavigationService, InsertOperationResultEnum, UpdateOperationResultEnum, DeleteOperationResultEnum,$q, notify, $location) {

    return ({
        showConfirmation: showConfirmation,
        showInformation: showInformation,
        showSuccess: showSuccess,
        showError: showError,
        showWarning: showWarning,
        notifyException: notifyException,
        notifyExceptionWithClose: notifyExceptionWithClose,
        notifyOnItemAdded: notifyOnItemAdded,
        notifyOnItemUpdated: notifyOnItemUpdated,
        notifyOnItemDeleted: notifyOnItemDeleted
    });

    function showConfirmation(message) {
        var settings = {
            width: "40%"
        };
        var deferred = $q.defer();
        settings.onScopeReady = function (modalScope) {
            modalScope.message = message;
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
        }, 3000);
    }

    function notifyException(error, scope) {
        showError("Error has been occured");
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
            }
            VRNavigationService.goto("/Error", parameters);
        }
    }

    function notifyOnItemAdded(itemType, insertOperationOutput) {//insertOperationOutput is of type InsertOperationOutput
        switch (insertOperationOutput.Result) {
            case InsertOperationResultEnum.Succeeded.value: showSuccess(itemType + " added successfully");
                return true;
                break;
            case InsertOperationResultEnum.Failed.value:
                if (insertOperationOutput.Message != undefined) {
                    showError(insertOperationOutput.Message); break;
                }

                else {
                    showError("Failed to add " + itemType); break;
                }
            case InsertOperationResultEnum.SameExists.value: 
                switch (itemType) {
                    case "View": showWarning("Same View Name already exists"); break;
                    case "Widget": showWarning("Same Widget Name already exists"); break;
                    case "User": showWarning("Same Email already exists"); break;
                    default: showWarning(itemType + " with the same key already exists"); break;
                }
                break;
        }
        return false;
    }
    function notifyOnItemDeleted(itemType, deleteOperationOutput) {//updateOperationOutput is of type UpdateOperationOutput
        switch (deleteOperationOutput.Result) {
            case DeleteOperationResultEnum.Succeeded.value: showSuccess(itemType + " deleted successfully");
                return true;
                break;
            case DeleteOperationResultEnum.InUse.value:
                showError("Failed to delete " + itemType + " because it is already in use");
                break;
            case DeleteOperationResultEnum.Failed.value:
                if (deleteOperationOutput.Message != null) {
                    showError(deleteOperationOutput.Message); break;
                }
                else {
                    showError("Failed to delete " + itemType); break;
                }
               
        }
        return false;
    }
    function notifyOnItemUpdated(itemType, updateOperationOutput) {//updateOperationOutput is of type UpdateOperationOutput
        switch (updateOperationOutput.Result) {
            case UpdateOperationResultEnum.Succeeded.value: showSuccess(itemType + " updated successfully");
                return true;
                break;
            case UpdateOperationResultEnum.Failed.value:
                if (updateOperationOutput.Message != undefined) {
                    showError(updateOperationOutput.Message); break;
                }

                else {
                    showError("Failed to update " + itemType); break;
                }
            case UpdateOperationResultEnum.SameExists.value:
                switch (itemType) {
                    case "View": showWarning("Same View Name already exists"); break;
                    case "Widget": showWarning("Same Widget Name already exists"); break;
                    default: showWarning(itemType + " with the same key already exists"); break;
                }
                break;
        }
        return false;
    }
});