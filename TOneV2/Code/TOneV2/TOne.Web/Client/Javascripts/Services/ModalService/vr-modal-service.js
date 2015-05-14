'use strict';

app.service('VRModalService', function ($modal, $rootScope, VRNavigationService) {

    return ({
        showModal: showModal
    });

    function showModal(viewUrl, useModalTemplate, parameters, settings) {
        var modalScope = $rootScope.$new();
        modalScope.modalContext = {};
        VRNavigationService.setParameters(modalScope, parameters);

        var modalUrl;
        if (useModalTemplate == true) {
            modalUrl = '/Client/Javascripts/Services/ModalService/vr-modal-service.html';
            modalScope.templateUrl = viewUrl;
            
        }
        else {
            modalUrl = viewUrl;
        }

        if (settings != undefined && settings != null) {
            modalScope.modalWidth = settings.width;
            modalScope.modalMaxHeight = settings.maxHeight;
        }

        var modalInstance = $modal({ scope: modalScope, template: modalUrl, show: true, animation: "am-fade-and-scale" });
        modalScope.modalContext.closeModal = function () {
            modalInstance.hide();
        };
        return modalScope;
    }
});