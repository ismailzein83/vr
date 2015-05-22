'use strict';

app.service('VRNotificationService', function (VRModalService, VRNavigationService, $q, notify, $location) {

    return ({
        showConfirmation: showConfirmation,
        showInformation: showInformation,
        showSuccess: showSuccess,
        showError: showError,
        showWarning: showWarning,
        notifyException: notifyException,
        notifyExceptionWithClose: notifyExceptionWithClose,
        notifyItemAddedSuccessfully: notifyItemAddedSuccessfully,
        notifyItemUpdatedSuccessfully: notifyItemUpdatedSuccessfully,
        notifyItemDeletedSuccessfully: notifyItemDeletedSuccessfully
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

    function notifyException(error) {
        showError("Error has been occured");
    }

    function notifyExceptionWithClose(error) {
        var parameters = {
            error: error,
            previousUrl: $location.url()
        }
        VRNavigationService.goto("/Error", parameters);
    }

    function notifyItemAddedSuccessfully(itemType) {
        showSuccess(itemType + " added successfully");
    }

    function notifyItemUpdatedSuccessfully(itemType) {
        showSuccess(itemType + " updated successfully");
    }

    function notifyItemDeletedSuccessfully(itemType) {
        showSuccess(itemType + " deleted successfully");
    }
});