var modal = {title: "", message: ""};

function hideModal(flag){
    $(popupBody).slideUp();
    popUp.className = "";
    return flag;
}

function showDialog(title, message, isAlert)
{
    modalTitle.innerText = title;
    modalMessage.innerHTML = message;
    popUp.className = "modal-backdrop fade in";
    $(popupBody).slideDown();
    if (isAlert != undefined && isAlert == true) {
        modalAlertButtons.className = "";
        modalConfirmButtons.className = "hide";
    }
    else {
        modalAlertButtons.className = "hide";
        modalConfirmButtons.className = "";
    }
    return false;
}