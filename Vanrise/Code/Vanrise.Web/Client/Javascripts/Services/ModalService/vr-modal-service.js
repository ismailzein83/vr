(function (app) {
    
    "use strict";

    function serviceObj($modal, $rootScope, VRNavigationService, $q) {
        return ({
            showModal: showModal
        });

        function showModal(viewUrl, parameters, settings) {
            var deferred = $q.defer();
            var modalScope = $rootScope.$new();

            var modalUrl = viewUrl;
            var modalInstance;
            modalScope.modalContext = {};
            modalScope.modalContext.closeModal = function () {
                if (modalInstance) modalInstance.hide();
                deferred.resolve();
            };

            VRNavigationService.setParameters(modalScope, parameters);

            if (settings != undefined && settings != null) {
                if (settings.useModalTemplate === true) {
                    modalUrl = '/Client/Javascripts/Services/ModalService/vr-modal-service.html';
                    modalScope.templateUrl = viewUrl;
                }

                modalScope.modalWidth = settings.width;
                modalScope.modalMaxHeight = settings.maxHeight;

                modalScope.title = settings.title;

                if (settings.onScopeReady != undefined)
                    settings.onScopeReady(modalScope);
            }

            modalScope.$on('modal.hide.before', function () {
                if (typeof (modalScope.modalContext.onModalHide) == "function") modalScope.modalContext.onModalHide();
            });

            modalInstance = $modal({ scope: modalScope, templateUrl: modalUrl, show: true, animation: "am-fade-and-scale" });
            return deferred.promise;
        }
    }

    serviceObj.$inject = ['$modal', '$rootScope', 'VRNavigationService', '$q'];
    app.service('VRModalService', serviceObj);


})(app);





