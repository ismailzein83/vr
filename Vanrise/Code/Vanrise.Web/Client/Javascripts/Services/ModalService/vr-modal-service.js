'use strict';

app.service('VRModalService', function ($modal, $rootScope, VRNavigationService, $q, notify) {

    return ({
        showModal: showModal
    });

    function showModal(viewUrl, parameters, settings) {
        var deferred = $q.defer();
        var modalScope = $rootScope.$new();
        
        var modalUrl = viewUrl;
        
        modalScope.modalContext = {};
        modalScope.modalContext.closeModal = function () {
            modalInstance.hide();
            deferred.resolve();
        };
        VRNavigationService.setParameters(modalScope, parameters);

        if (settings != undefined && settings != null) {
            if (settings.useModalTemplate == true) {
                modalUrl = '/Client/Javascripts/Services/ModalService/vr-modal-service.html';
                modalScope.templateUrl = viewUrl;
            }

            modalScope.modalWidth = settings.width;
            modalScope.modalMaxHeight = settings.maxHeight;

            modalScope.title = settings.title;

            if (settings.onScopeReady != undefined)
                settings.onScopeReady(modalScope);
        }

        var modalInstance = $modal({ scope: modalScope, templateUrl: modalUrl, show: true, animation: "am-fade-and-scale" });
        return deferred.promise;
    }
});

