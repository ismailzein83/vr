'use strict';

app.service('VRNotificationService', function (VRModalService, VRNavigationService, InsertOperationResultEnum, UpdateOperationResultEnum, $q, notify, $location) {

    return ({
        showConfirmation: showConfirmation,
        showInformation: showInformation,
        showSuccess: showSuccess,
        showError: showError,
        showWarning: showWarning,
        notifyException: notifyException,
        notifyExceptionWithClose: notifyExceptionWithClose,
        notifyOnItemAdded: notifyOnItemAdded,
        notifyOnItemUpdated: notifyOnItemUpdated
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
            case InsertOperationResultEnum.Failed.value: console.log(insertOperationOutput.Message);
                if (insertOperationOutput.Message != undefined) {
                    showError(insertOperationOutput.Message); break;
                }

                else {
                    showError("Failed to add " + itemType); break;
                }
            case InsertOperationResultEnum.SameExists.value: 
                switch (itemType) {
                    case "Widget": showWarning("Same Widget Name already exists"); break;
                    case "User": showWarning("Same Email already exists"); break;
                    default: showWarning(itemType + " with the same key already exists"); break;
                }
                break;
        }
        return false;
    }

    function notifyOnItemUpdated(itemType, updateOperationOutput) {//updateOperationOutput is of type UpdateOperationOutput
        switch (updateOperationOutput.Result) {
            case UpdateOperationResultEnum.Succeeded.value: showSuccess(itemType + " updated successfully");
                return true;
                break;
            case UpdateOperationResultEnum.Failed.value: showError("Failed to update " + itemType); break;
        }
        return false;
    }
});