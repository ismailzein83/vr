(function (app) {

    "use strict";

    function serviceObj($modal, $rootScope, VRNavigationService, $q, UtilsService, MobileService) {
        return ({
            showModal: showModal
        });

        function showModal(viewUrl, parameters, settings) {
            var deferred = $q.defer();
            var modalScope = $rootScope.$new();
            $rootScope.$broadcast("hide-all-menu");
            var modalUrl = viewUrl;
            var backdrop = "static";
            var modalInstance;
            modalScope.modalContext = {};

            var onhideModal = function () {

            };
            modalScope.modalContext.closeModal = function () {
                if (modalInstance) modalInstance.hide();
                deferred.resolve();
                modalScope.$destroy();
            };

            var sizeOptions = {
                small: "vr-modal-sm",
                medium: "vr-modal-md",
                large: "vr-modal-lg",
                xlarge: "vr-modal-xl"
            };

            VRNavigationService.setParameters(modalScope, parameters);
            if (settings != undefined && settings != null) {
                if (settings.useModalTemplate === true) {
                    modalUrl = '/Client/Javascripts/Services/ModalService/vr-modal-service.html';
                    modalScope.templateUrl = viewUrl;
                    var num = 50;
                    if (settings.width != undefined)
                        num = parseFloat(settings.width.substring(0, settings.width.length));

                    if (settings.size != undefined) {
                        classmodal = sizeOptions[settings.size];
                    }
                    else {
                        var classmodal = "vr-modal-sm";
                        if (num >= 50)
                            classmodal = "vr-modal-md";
                        if (num >= 80)
                            classmodal = "vr-modal-lg";
                        if (num > 90)
                            classmodal = "vr-modal-xl";
                    }
                    modalScope.resClass = classmodal;
                } else {
                    var classmodal;
                    if (settings.size != undefined) {
                        classmodal = sizeOptions[settings.size];
                    }
                    modalScope.resClass = classmodal;
                }

                modalScope.modalWidth = settings.width;
                modalScope.modalMaxHeight = settings.maxHeight;

                modalScope.title = settings.title;
                modalScope.showFooter = MobileService.isMobile();
                if (settings.onScopeReady != undefined)
                    settings.onScopeReady(modalScope);

                if (UtilsService.isContextReadOnly(modalScope) === true) {

                    setTimeout(function () {
                        $('.modal-header').eq($('.modal-dialog').length - 1).addClass('vr-modal-readonly');
                        $('.modal-header').eq($('.modal-dialog').length - 1).attr('readonly', 'true');
                    }, 100);

                }

                if (settings.autoclose != undefined) {
                    backdrop = settings.autoclose;
                }

            }

            modalScope.$on('modal.hide.before', function () {
                if ($('.modal-header').eq($('.modal-dialog').length - 2).attr('readonly') == undefined) {
                    $('.modal-header').eq($('.modal-dialog').length - 2).removeClass('vr-modal-header-inback');
                    $('.modal-header').eq($('.modal-dialog').length - 2).addClass('vr-modal-header');
                }

                if (MobileService.isMobile()) {
                    if ($('.expandable-row-content').length == 0) {
                        $('body').removeClass('full-mobile-body');
                    }

                    if ($('.modal-dialog').length > 1) {
                        $('body').addClass('full-mobile-body');
                    }
                    


                    if ($('.expandable-row-content').length > 1) {
                        $('.expandable-row-content').addClass('full-view');
                    }
                    if ($('.modal-dialog').length == 1) {
                        $('.expandable-row-content').removeClass('full-view');
                    }
                }


                if (typeof (modalScope.modalContext.onModalHide) == "function") modalScope.modalContext.onModalHide();
            });
            modalScope.$on('modal.show', function () {
                if (MobileService.isMobile()) {
                    if ($('.expandable-row-content').length > 1) {
                        $('.expandable-row-content').addClass('full-view');
                    }
                }
            });

            //modalScope.$on("$destroy", function () {
            //   // $(window).off("resize.Viewport");
            //});



            var animationClass = MobileService.isMobile() ? "" : "am-fade-and-scale";
            modalInstance = $modal({ scope: modalScope, templateUrl: modalUrl, backdrop: backdrop, show: true, animation: animationClass, onHide: onhideModal });
            return deferred.promise;
        }
    }

    serviceObj.$inject = ['$modal', '$rootScope', 'VRNavigationService', '$q', 'UtilsService', 'MobileService'];
    app.service('VRModalService', serviceObj);


})(app);





