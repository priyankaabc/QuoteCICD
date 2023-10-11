
var DataTable = $('#tblRoleList').DataTable();

$(document).ready(function () {
    $("#menu_Role").addClass("active");
    BindRoleList();
});
var currentPageNumber = 0;
var currentPageSize = 10;
function BindRoleList() {
    /*debugger*/
    try {
        if (DataTable != 'undefined') {
            currentPageSize = DataTable.page.len();
            DataTable.destroy();
        }
        currentPageNumber = DataTable.page.info().page

        DataTable = $('#tblRoleList').DataTable({
            colReorder: {
                enable: false
            },
            serverSide: true,
            //processing: true,
            language: {
                'processing': '<div class="loader"></div>'
            },
            ajax: {
                url: '/Role/GetRoleList',
                type: 'POST',
                data: function (d) {
                    d.start = DataTable.page.info().start;
                }
                //data: function (d) {
                //    d.ImportQuoteId = 10;
                //}
            },
            "lengthMenu": [10, 20, 50, 100, 200, 500],
            "pageLength": currentPageSize,
            displayStart: currentPageNumber * currentPageSize,
            "order": [],
            //dom: 'Blfrtip',
            "bFilter": true,
            //"aaData": data.Data,
            "aoColumns": [

                {
                    "mData": "RoleName",
                    "className": "text-left"
                },
                {
                    "mData": "IsActive",
                    "className": "text-left text-center",
                    "render": function (data, type, row, meta) {
                        var result = '';
                        if (row.IsActive) {
                            result = '<button title="Active" onclick="openActivePopup(' + row.RoleId + ', ' + row.IsActive + ')" class="btn  text-success" ><i class="fa-solid fa-check fa-xl "></i></button>';
                        }
                        else {
                            result = '<a href = "javascript:void(0)" title = "In Active" onclick = "openActivePopup(' + row.RoleId + ', ' + row.IsActive + ')" class="btn text-danger" > <i class="fa-solid fa-xmark fa-xl "></i></a >';
                        }
                        return result;
                    }
                },
                {
                    "mData": "IsActive",
                    "bSortable": false,
                    "className": "text-center",
                    "render": function (data, type, row, meta) {
                        var ActionButtonEdit = '<a href="javascript:void(0)" title="Edit" onclick="openAddEditRoleModal(' + row.RoleId + ')"  data-toggle="modal" data-target="#manageRolemodalPopup" class="btn pr-2 edit-icon" ><i class="fa-regular fa-pen-to-square"></i></a>';
                        var ActionButtonDelete = '<a href="javascript:void(0)" onclick="DeleteRole(\'' + row.RoleId + '\')" title="Delete "class="btn delete-icon"><i class="fa-regular fa-trash-can"></i></a>';
                        var ActionButtonAssign = '<a /*title="View Quotes"*/ href="javascript:;" onclick="openRoleModal(' + row.RoleId + ')" data-toggle="modal" data-target="#usermodalPopup" class="btn btn-link btn-sm px-1 datatable-btn text-decoration-none">Assign Rights</a>';
                        var result = '';
                        result = result + ActionButtonEdit;
                        result = result + '&nbsp;&nbsp;&nbsp;' + ActionButtonDelete;
                        result = result + '&nbsp;&nbsp;&nbsp;' + ActionButtonAssign;
                        return result;
                    }
                }

            ],
        });
        $(".dataTables_length").css('clear', 'none');
        $(".dataTables_length").css('margin-right', '20px');
        $(".dataTables_info").css('clear', 'none');
        $(".dataTables_info").css('padding', '0');
    } catch (e) {
        alert(e.message);
    }
}
function openRoleModal(roleId) {
    $.ajax({
        url: '/Role/ManageRole',
        async: false,
        type: 'GET',
        data: { roleId: roleId },
        success: function (data) {
            $('#divrolesmodalPopup').html(data);
        }
    });
}
function openAddEditRoleModal(Id) {
    /*debugger*/
    $.ajax({
        url: '/Role/AddEdit',
        async: false,
        type: 'GET',
        data: { Id: Id },
        success: function (data) {
            $('#divmanageRolemodalPopup').html(data);
        }
    });
}


function cancelClick() {
    $("#usermodalPopup, #manageRolemodalPopup").modal('hide');
}

function checkvalidation() {
    if ($("#ManageRole").validate() != true) {
        return false;
    }
}
function ManageRole() {
    return false;
}
function AddEditRoles() {
    if ($("#RoleName").val().trim() === "") {
        $(".ValidateName").removeClass('d-none');
    } else {
        $(".ValidateName").addClass('d-none');
        //$('#RoleForm').submit();

        $.ajax({
            url: '/Role/AddEdit',
            async: false,
            type: 'POST',
            data: $('#EditRoleForm').serialize(),
            success: function (data) {
                if (data.result == 1) {
                    //SucessMessage("Quote Added Successfully");
                    toastr.success("Role Added Successfully");
                }
                else if (data.result == 2) {
                    toastr.success("Role Updated Successfully");
                    //ShowErrorMessage("Please select at least one item");
                }
                else {
                    toastr.error("Same Role Already Exist");
                }
            },
        })
        cancelClick()
        BindRoleList()
    }
}
function updateroles() {
    var rolelistjson = [];
    var rolesList = $("#tblassignroles tbody tr");
    rolesList.each(function () {
        var isview = false, isinsert = false, isedit = false, isdelete = false, ischangeStatus = false;
        item = {};
        var tdlists = $('td input[type=checkbox]', this);
        tdlists.each(function () {

            if ($(this)[0].id.includes('IsView') && $(this).is(":checked")) {
                isview = true;
            }
            else if ($(this)[0].id.includes('IsInsert') && $(this).is(":checked")) {
                isinsert = true;
            }
            else if ($(this)[0].id.includes('IsEdit') && $(this).is(":checked")) {
                isedit = true;
            }
            else if ($(this)[0].id.includes('IsDelete') && $(this).is(":checked")) {
                isdelete = true;
            }
            else if ($(this)[0].id.includes('IsChangeStatus') && $(this).is(":checked")) {
                ischangeStatus = true;
            }
        });
        item["RoleMenuMapId"] = $('td input[type=text]', this)[1].value;
        item["RoleId"] = $("#RoleId").val();
        item["RoleName"] = $("#RoleName").val();
        item["MenuId"] = $('td input[type=text]', this)[0].value;
        item["MenuName"] = $('td', this)[0].innerHTML;
        item["IsView"] = isview;
        item["IsInsert"] = isinsert;
        item["IsEdit"] = isedit;
        item["IsDelete"] = isdelete;
        item["IsChangeStatus"] = ischangeStatus;
        rolelistjson.push(item);
    });
    $.ajax({
        url: '/Role/ManageRole',
        async: false,
        type: 'Post',
        data: JSON.stringify(rolelistjson),
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#usermodalPopup").modal('hide');

        }
    });
}

function DeleteRole(RoleId) {
    bootbox.confirm({
        message: "Are you sure , you want to delete this Record?",
        buttons: {
            cancel: {
                label: "No",
                className: "btn-cancel min-auto cancel"
            },
            confirm: {
                label: "Yes",
                className: "btn-warning min-auto  confirm"
            }
        },
        callback: function (result) {
            if (result) {
                $.ajax({
                    url: '/Role/DeleteRole',
                    async: false,
                    type: 'POST',
                    data: { RoleId: RoleId },
                    success: function (data) {
                        if (data.IsSuccess) {
                            toastr.success(data.Message);
                            BindRoleList();
                        }
                        else {
                            toastr.error(data.Message);
                        }
                    }
                });
            }

        }
    });
}

function checkAll(fieldName, checked) {


    var IsChecked = document.querySelectorAll(`.${fieldName}`)

    IsChecked.forEach(function (checkbox) {
        checkbox.checked = checked;
    });
}

function checkBoxClicked(checkbox) {
    var previousSpan = checkbox.parentElement.previousElementSibling;
    if (previousSpan && previousSpan.tagName === "SPAN") {
        previousSpan.classList.add("changedCheckbox");
    }
}

function openActivePopup(Id, IsActive) {

    var text = "";
    if (IsActive) {
        text = "Are you sure you want to deactivate this role ?";
    }
    else {
        text = "Are you sure you want to activate this role ?";
    }
    bootbox.confirm({
        message: text,
        buttons: {
            cancel: {
                label: "No",
                className: "btn-cancel min-auto cancel"
            },
            confirm: {
                label: "Yes",
                className: "btn-warning min-auto confirm"
            }
        },
        callback: function (result) {
            if (result) {
                $.ajax({
                    type: 'POST',
                    url: '/Role/ChangeStatus',
                    dataType: 'json',
                    data: { Id: Id, IsActive: IsActive },
                    cache: false,
                    async: false,
                    success: function (data) {
                        if (data) {
                            toastr.success(data.Message);
                            //$('#BaggageQuoteGrid').data('kendoGrid').dataSource.read();
                            BindRoleList();
                        }
                    },
                    error: function () {
                        alert("We are facing some problem please try again later");
                        //$('#BaggageQuoteGrid').data('kendoGrid').dataSource.read();
                    }
                });
            }

        }
    });
}
//$(document).ready(function () {
//    BindRoleList();
//});

//function BindRoleList() {
//    try {
//        debugger;
//        var dtuserList = $('#tblRoleList').DataTable();
//        if (dtuserList != 'undefined') {
//            dtuserList.destroy();
//        }


//        dtuserList = $('#tblRoleList').DataTable({
//            "order": [0, 'desc'],
//            "dom": 'frtlip',
//            "iDisplayLength": 10,
//            "bFilter": true,
//            "proccessing": true,
//            "serverSide": true,
//            "ajax": {
//                "url": encodeURI('@Url.Action("GetRoleList", "Role")'),
//                "type": "POST",
//                "dataType": "JSON"
//            },
//            "aoColumns": [

//                {
//                    "mData": "RoleName",
//                    "className": "text-left"
//                },
//                {
//                    "mData": "IsActive",
//                    "className": "text-left",
//                    "render": function (data, row, meta) {
//                        if (meta.IsActive) {
//                            return "Yes";
//                        }
//                        else {
//                            return "No";
//                        }


//                    }
//                },
//                {
//                    "mData": "id",
//                    "className": "text-center",
//                    "render": function (data, type, row, meta) {
//                        //var ActionButtonEdit = '<a href="javascript:void(0)" title="Edit" onclick="openUserModal(' + row.id +')"  data-toggle="modal" data-target="#usermodalPopup"><i class="fa fa-pencil text-primary font-15"></i></a>';
//                        //var ActionButtonDelete = '<a href="javascript:void(0)" onclick="deleteUser(' + row.id +')" title="Delete"><i class="fa fa-trash text-primary font-15"></i></a>';
//                        var result = '';
//                        //result = result + ActionButtonEdit;
//                        //result = result +'&nbsp;&nbsp;&nbsp;'+ ActionButtonDelete;
//                        return result;
//                    }
//                }

//            ],
//            "initComplete": function () {

//                var dataTable = $('#tblRoleList').DataTable();
//                $('[data-toggle="tooltip"]').tooltip();

//            },
//            "fnDrawCallback": function () {

//            },
//            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
//                if (aData.RoleId == 0) {
//                    $('td', nRow).css('background-color', '#FFA500');
//                    $('td', nRow).css('color', '#ffffff ');
//                }
//            }
//        });

//        $(".dataTables_length").css('clear', 'none');
//        $(".dataTables_length").css('margin-right', '20px');
//        $(".dataTables_info").css('clear', 'none');
//        $(".dataTables_info").css('padding', '0');
//    } catch (e) {
//        alert(e.message);
//    }
//}
//function reqEnd(para) {

//    if (para.response.Errors == undefined) {
//        if (para.type == 'update' || para.type == 'create' || para.type == 'destroy') {
//            $('#gridRoleMaster').data('kendoGrid').dataSource.read();
//        }
//        if (para.response.Message != undefined) {
//            window.SucessMessage(para.response.Message);
//        }
//    }
//}
//function OnEdit(e) {
//    e.preventDefault();
//    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
//    window.location.href = "@Url.Action("AddEdit", "EmailTemplate")?id=" + dataItem.Id;
//}

//function assignRights(e) {
//    e.preventDefault();
//    var tr = $(e.target).closest("tr");
//    var data = this.dataItem(tr);
//    var urllink = '@Url.Action("RoleMenusView", "Role")';

//    setLocalValue('RoleId', data.RoleId);

//    var window = $("#myWindow").data("kendoWindow");
//    window.content("");
//    window.title("Assign Access Rights");
//    window.refresh({
//        url: urllink

//    });

//    window.center();
//    window.open();
//}

//function additionalInfo() {
//    return {
//        roleId: getLocalValue('RoleId')
//    };
//}

//function onActivate() {
//    CheckAllView();
//    CheckAllEdit();
//    CheckAllAdd();
//    CheckAllDelete();
//    CheckAllStatusChange();
//}