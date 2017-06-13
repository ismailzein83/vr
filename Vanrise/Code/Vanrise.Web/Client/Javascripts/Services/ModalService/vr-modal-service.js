﻿(function (app) {
    
    "use strict";

    function serviceObj($modal, $rootScope, VRNavigationService, $q, UtilsService) {
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

           
            VRNavigationService.setParameters(modalScope, parameters);
            if (settings != undefined && settings != null) {
                if (settings.useModalTemplate === true) {
                    modalUrl = '/Client/Javascripts/Services/ModalService/vr-modal-service.html';
                    modalScope.templateUrl = viewUrl;
                    var num = 50;
                    if (settings.width != undefined)
                        num = parseFloat(settings.width.substring(0, settings.width.length));
                    var sizeOptions = {
                        small: "vr-modal-sm",
                        medium: "vr-modal-md",
                        large: "vr-modal-lg",
                        xlarge: "vr-modal-xl"
                    };
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
                }

                modalScope.modalWidth = settings.width;
                modalScope.modalMaxHeight = settings.maxHeight;

                modalScope.title = settings.title;

                if (settings.onScopeReady != undefined)
                    settings.onScopeReady(modalScope);
                
                if (UtilsService.isContextReadOnly(modalScope) === true) {
                  
                    setTimeout(function () {
                        $('.modal-header').eq($('.modal-dialog').length - 1).addClass('vr-modal-readonly');
                        $('.modal-header').eq($('.modal-dialog').length - 1).attr('readonly', 'true');
                    }, 100);
                   
                }
                   
                if (settings.autoclose != undefined ) {
                    backdrop = settings.autoclose;
                }
                   
            }

            modalScope.$on('modal.hide.before', function () {
                if ($('.modal-header').eq($('.modal-dialog').length - 2).attr('readonly') == undefined) {
                    $('.modal-header').eq($('.modal-dialog').length - 2).removeClass('vr-modal-header-inback');
                    $('.modal-header').eq($('.modal-dialog').length - 2).addClass('vr-modal-header');

                }
                if (typeof (modalScope.modalContext.onModalHide) == "function") modalScope.modalContext.onModalHide();
            });
            //modalScope.$on("$destroy", function () {
            //   // $(window).off("resize.Viewport");
            //});
            modalInstance = $modal({ scope: modalScope, templateUrl: modalUrl, backdrop: backdrop, show: true, animation: "am-fade-and-scale" ,onHide:onhideModal });
            return deferred.promise;
        }
    }

    serviceObj.$inject = ['$modal', '$rootScope', 'VRNavigationService', '$q', 'UtilsService'];
    app.service('VRModalService', serviceObj);


})(app);





