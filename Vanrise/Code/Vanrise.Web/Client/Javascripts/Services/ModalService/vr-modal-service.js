(function (app) {
    
    "use strict";

    function serviceObj($modal, $rootScope, VRNavigationService, $q, UtilsService) {
        return ({
            showModal: showModal
        });

        function showModal(viewUrl, parameters, settings) {
            var deferred = $q.defer();
            var modalScope = $rootScope.$new();

            var modalUrl = viewUrl;
            var backdrop = "static";
            var modalInstance;
            modalScope.modalContext = {};

            var onhideModal = function () {

            };
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
                
                if (UtilsService.isContextReadOnly(modalScope) === true) {
                  
                    setTimeout(function () {
                        $('.modal-header').eq($('.modal-dialog').length - 1).css({
                            backgroundColor: "#969696"
                        });
                        $('.modal-header').eq($('.modal-dialog').length - 1).attr('readonly', 'true');
                    }, 100);
                   
                }
                   
                if (settings.autoclose != undefined ) {
                    backdrop = settings.autoclose;
                }
                   
            }

            modalScope.$on('modal.hide.before', function () {
                if ($('.modal-header').eq($('.modal-dialog').length - 2).attr('readonly') == undefined) {
                    $('.modal-header').eq($('.modal-dialog').length - 2).css({
                        backgroundColor: "#20407D"
                    });
                }
                if (typeof (modalScope.modalContext.onModalHide) == "function") modalScope.modalContext.onModalHide();
            });

            modalInstance = $modal({ scope: modalScope, templateUrl: modalUrl, backdrop: backdrop, show: true, animation: "am-fade-and-scale" ,onHide:onhideModal });
            return deferred.promise;
        }
    }

    serviceObj.$inject = ['$modal', '$rootScope', 'VRNavigationService', '$q', 'UtilsService'];
    app.service('VRModalService', serviceObj);


})(app);





