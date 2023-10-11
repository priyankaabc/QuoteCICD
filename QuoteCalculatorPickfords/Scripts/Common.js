var datatypeEnum, typeEnum;
var dateFormat = "dd/MM/yyyy";


$(function () {
    if (typeof datatypeEnum == "undefined") {
        datatypeEnum = {
            "json": "json",
            "text": "text"
        };
    }

    if (typeof typeEnum == "undefined") {
        typeEnum = {
            "get": "get",
            "post": "post"
        };
    }
});

function callwebservice(ajaxurl, parameter, callbackFunction, isErrorHandle, dataTypem, typeEnum) {
 
    if (typeof (parameter) === 'undefined') {
        parameter = '';
    }
    try {
        $.support.cors = true;
        $.ajax({
            url: ajaxurl,
            cache: false,
            dataType: dataTypem,
            data: parameter,
            timeout: 40000,
            type: typeEnum,
            success: function (data) {
                callbackFunction(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (isErrorHandle === true) {
                    callbackFunction("error");
                }
                else {
                    if (errorThrown !== "") {
                        showMessageOnly("The following error occured: " + errorThrown, 'popup-error');
                    }
                    else {
                        showMessageOnly("There is an error while connecting to server. Please try again!", 'popup-error');
                    }
                }
            }
        });
    }
    catch (e) {
        showMessageOnly("Errour occurred " + e, 'popup-error');
    }
}

function showMessage(url, message, alertClass) {
    bootbox.alert(message, function () {
        window.location.href = url;
    }, alertClass);
}

function showMessageOnly(message, alertClass) {
    bootbox.alert(message, '', alertClass);
}

function showConfirmBox(message, callback) {
    bootbox.confirm(message, callback, 'popup-confirmation');
}

function clearInputById(id) {
    $("#" + id).val("");
}

function setFocusById(id) {
    $("#" + id).focus();
}

function setInputValueById(id, value) {
    return $("#" + id).val(value);
}

function setInputValue(element, value) {
    return $(element).val(value);
}

function removeElement(element) {
    $(element).remove();
}

function setCssProperty(element, cssProperty, propertyValue) {
    $(element).css(cssProperty, propertyValue);
}

function setCssPropertyWithLocation(element, location, cssProperty, propertyValue) {
    $(element).find(location).css(cssProperty, propertyValue);
}

function setAttribute(element, attribute, value) {
    $(element).attr(attribute, value);
}

function getLength(element) {
    return $(element).length;
}

function removeAttributeById(id, attribute) {
    $("#" + id).removeAttr(attribute);
}

function getInputValueById(id) {
    return $("#" + id).val();
}

function getInputValue(element) {
    return $(element).val();
}

function getLocalJsonData(key) {
    return JSON.parse(localStorage.getItem(key));
}

function setLocalJsonData(key, jsonData) {
    localStorage.setItem(key, JSON.stringify(jsonData));
}

function getWindowPathName() {
    return window.pathArray;
}

function OnchangetoClearIndexCommon(e) {
    if (this.value && this.selectedIndex === -1) {
        this.value("");
        return;
    }
}
function clearAllData(formId) {
    $("#" + formId + " input[type='text']").each(function (i, e) {
        e.value = "";
    });
    $("#" + formId + " input[type='date']").val('');
    $("#" + formId + " textarea").val('');
    $("#" + formId + " input[type='hidden']").val('');

    $("#" + formId + " select").each(function (i, e) {
        e.selectedIndex = 0;
    });

    $("#" + formId + " input[type='checkbox']").prop('checked', false);
}


function cleartextbox(primaryId, textboxId) {
    var pId = $("#" + primaryId).val();
    if (pId === 0 || pId == undefined) {
        $("#" + textboxId).val("");
    }
}


function appendHTML(id, html) {
    $("#" + id).append(html);
}

function getLocalValue(key) {
    return localStorage.getItem(key);
}

function setLocalValue(key, value) {
    localStorage.setItem(key, value);
}

function setActiveMenulink() {
    $('#sideBar li').removeClass('active');
    $('#sideBar li:has(a[id="' + window.controllerName + '"])').addClass('active');
    //$('#sidebar ul li:has(a[id="' + window.controllerName + '"])').addClass('active');
}

