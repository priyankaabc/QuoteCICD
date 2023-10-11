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

//var validationMessageTemplate = kendo.template(
//    "<div id='#=field#_validationMessage' " +
//    "class='k-widget k-tooltip k-tooltip-validation k-invalid-msg field-validation-error'" +
//    "style='margin: 0.5em;display:'' !important' data-for='#=field#' " +
//    "data-val-msg-for='#=field#'>" +
//    "#=message#" +
//    "<div class='k-callout k-callout-n'></div>" +
//    "</div>");

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

function setKendoComboboxFocusById(id) {
    $("#" + id).data("kendoComboBox").input.focus();
}


function setupFormValidation(id) {
    $("#" + id).kendoValidator();
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

function setKendoDatePickerValue(dateTimePickerId, dateTimeValue) {
    $("#" + dateTimePickerId).data("kendoDatePicker").value(dateTimeValue);
}

function getKendoDatePickerValue(datepickerId) {
    return $("#" + datepickerId).data('kendoDatePicker').value();
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

function changeStatus(controllerName, action, reloadAction, parameter, message, id, gridId) {
 
    var grid = $("#" + gridId).data("kendoGrid");
    bootbox.confirm(message, function (result) {
        if (result === true) {
            callwebservice(getWindowPathName() + '/' + controllerName + '/' + action, parameter, function (data) {
                if (data === "") {
                    if (grid != undefined) {
                        grid.dataSource.read();
                        return;
                    }
                }
            }, true, datatypeEnum.text, typeEnum.post);
        }
    }, 'popup-confirmation');
}

function changeAllowDatasetStatus(controllerName, action, reloadAction, parameter, message, id, gridId) {
    var grid = $("#" + gridId).data("kendoGrid");
    bootbox.confirm(message, function (result) {
        if (result === true) {
            callwebservice(getWindowPathName() + controllerName + '/' + action, parameter, function (data) {
                if (data === "") {
                    if (grid != undefined) {
                        grid.dataSource.read();
                        return;
                    }
                }
            }, true, datatypeEnum.text, typeEnum.post);
        }
    }, 'popup-confirmation');
}


function saveGridState(gridId, pageName) {
    var grid = $("#" + gridId).data("kendoGrid");
    var savedState = kendo.stringify(grid.getOptions());
    var parameter = "pageName=" + pageName + "&gridState=" + savedState;
    var controllerName = 'General';
    var action = 'SaveGridState';
    callwebservice(getWindowPathName() + controllerName + '/' + action, parameter, function (data) { }, true, datatypeEnum.text, typeEnum.post);
}

function loadGridState(gridId, options) {
    var grid = $("#" + gridId).data("kendoGrid");
    if (options) {
        grid.setOptions(JSON.parse(options));
    }
}

function cleartextbox(primaryId, textboxId) {
    var pId = $("#" + primaryId).val();
    if (pId === 0 || pId == undefined) {
        $("#" + textboxId).val("");
    }
}

function readKendoGrid(id) {
    var grid = $("#" + id).data("kendoGrid");
    grid.dataSource.read();
}

function readkendoDropDownList(id) {
    var grid = $("#" + id).data("kendoDropDownList");
    grid.dataSource.read();
}

function readkendoComboBox(id) {
    var grid = $("#" + id).data("kendoComboBox");
    grid.dataSource.read();
}

function bindKendDropdownlist(id, textField, valueField, data, placeHolder) {
    $("#" + id).kendoDropDownList({
        dataTextField: textField,
        dataValueField: valueField,
        dataSource: data,
        optionLabel: placeHolder
    }).data("kendoDropDownList");
}

function bindKendDropdownlistWithChangeEvent(id, textField, valueField, data, placeHolder, onChange) {
    $("#" + id).kendoDropDownList({
        dataTextField: textField,
        dataValueField: valueField,
        dataSource: data,
        optionLabel: placeHolder,
        change: onChange
    }).data("kendoDropDownList");
}

function bindKendCombobox(id, textField, valueField, data, placeHolder) {
    $("#" + id).kendoComboBox({
        dataTextField: textField,
        dataValueField: valueField,
        dataSource: data,
        placeholder: placeHolder
    }).data("kendoComboBox");
}

function bindKendComboboxWithChangeEvent(id, textField, valueField, data, placeHolder, onSelect, onChange) {
    $("#" + id).kendoComboBox({
        dataTextField: textField,
        dataValueField: valueField,
        dataSource: data,
        placeholder: placeHolder,
        select: onSelect,
        change: onChange
    }).data("kendoComboBox");
}

function clearKendoGridDatasource(id) {
    $("#" + id).data('kendoGrid').dataSource.data([]);
}

function clearbindKendDropdownlist(id) {
    $("#" + id).data('kendoDropDownList').dataSource.data([]);
}

function setKendoDropdownlistValue(id, value) {
    $("#" + id).data("kendoDropDownList").value(value);
}

function getKendoDropdownlistValue(id) {
    return $("#" + id).data('kendoDropDownList').value();
}

function getKendComboboxValue(id) {
    return $("#" + id).data('kendoComboBox').value();
}


function getKendoMultiSelectValue(id) {
    return $("#" + id).data('kendoMultiSelect').value();
}


function getKendComboboxText(id) {
    return $("#" + id).data('kendoComboBox').text();
}

function setKendComboboxValue(id, value) {
    $("#" + id).data('kendoComboBox').value(value);
}

function clearKendoDropdownlist(id) {
    var ddl = $("#" + id).data("kendoDropDownList");
    ddl.dataSource.data({}); // clears dataSource
    ddl.text(""); // clears visible text
    ddl.value("");
}

function clearkendoComboBox(id) {
    var ddl = $("#" + id).data("kendoComboBox");
    ddl.dataSource.data({}); // clears dataSource
    ddl.text(""); // clears visible text
    ddl.value("");
}

function openKendoWindow(id) {
    var dialog = $('#' + id);
    dialog.data("kendoWindow").open();
    dialog.data('kendoWindow').center();
}

function openKendoWindowMaximized(id) {
    var dialog = $('#' + id).data("kendoWindow");
    dialog.open();
    dialog.center();
    dialog.maximize();
}

function closeKendoWindow(id) {
    var dialog = $('#' + id);
    dialog.data("kendoWindow").close();
}

function convertDateToString(value) {
    if (value != null && value !== '')
        return kendo.toString(value, dateFormat);
    else return "";
}

function appendHTML(id, html) {
    $("#" + id).append(html);
}

function setMonthStartDate(datePickerId) {
    var date = new Date();
    $("#" + datePickerId).kendoDatePicker({ value: new Date(date.getFullYear(), date.getMonth(), 1) });
}

function setMonthEndDate(datePickerId) {
    var date = new Date();
    $("#" + datePickerId).kendoDatePicker({ value: new Date(date.getFullYear(), date.getMonth() + 1, 0) });
}

function setMonthStartDateWithChange(datePickerId, onChange) {
    var date = new Date();
    $("#" + datePickerId).kendoDatePicker({ value: new Date(date.getFullYear(), date.getMonth(), 1), change: onChange });
}

function setMonthEndDateWithChange(datePickerId, onChange) {
    var date = new Date();
    $("#" + datePickerId).kendoDatePicker({ value: new Date(date.getFullYear(), date.getMonth() + 1, 0), change: onChange });
}

function setCurrentDate(datePickerId) {
    $("#" + datePickerId).kendoDatePicker({ value: new Date(), format: dateFormat });
}

function setKendoDatePickerMinValue(dateTimePickerId, dateTimeValue) {
    $("#" + dateTimePickerId).data("kendoDatePicker").min(dateTimeValue);
}

function setKendoDatePickerMaxValue(dateTimePickerId, dateTimeValue) {
    $("#" + dateTimePickerId).data("kendoDatePicker").max(dateTimeValue);
}

function getLocalValue(key) {
    return localStorage.getItem(key);
}

function setLocalValue(key, value) {
    localStorage.setItem(key, value);
}

function setkendoNumericTextBoxValue(id, value) {
    $("#" + id).data("kendoNumericTextBox").value(value);
}


function getkendoNumericTextBoxValue(id) {

    return $("#" + id).data("kendoNumericTextBox").value();
}

function onError(gridName) {
    return function (e) {
        try {
            if (e.errors) {
                var grid = $('#' + gridName).data('kendoGrid');
                if (grid != undefined) {
                    if (grid.editable != null || grid.editable != undefined) {
                        grid.one("dataBinding", function (element) {
                            element.preventDefault(); // cancel grid rebind
                            for (var error in e.errors) {
                                if (e.errors.hasOwnProperty(error)) {
                                    showMessageKendo(grid.editable.element, error, e.errors[error].errors);
                                }
                            }
                        });
                        return;
                    }
                }
                e.sender.cancelChanges();
                var message = "";
                $.each(e.errors, function (key, value) {
                    if ('errors' in value) {
                        $.each(value.errors, function () {
                            message += this + "\n\n";
                        });
                    }
                });
                if ((message.indexOf('The DELETE statement conflicted with the REFERENCE constraint') !== -1) ||
                    (message.indexOf('The DELETE statement conflicted with the SAME TABLE REFERENCE') !== -1)) {
                    showMessageOnly('This record is linked with another record and cannot be deleted.', 'popup-error');

                } else {
                    showMessageOnly(message, 'popup-error');
                }
            } else {
                showMessageOnly('Error occured while processing the record.', 'popup-error');
            }
        } catch (err) {
            showMessageOnly('Error occured while processing the record.', 'popup-error');
        }
    };
}

function showMessageKendo(container, name, errors) {
    container.find("[data-valmsg-for=" + name + "],[data-val-msg-for=" + name + "]")
        .replaceWith(validationMessageTemplate({ field: name, message: errors[0] }));

    setFocusById(name);
}

function setActiveMenulink() {
   
    $('#sideBar li').removeClass('active');
    $('#sideBar li:has(a[id="' + window.controllerName + '"])').addClass('active');
    //$('#sidebar ul li:has(a[id="' + window.controllerName + '"])').addClass('active');
}














