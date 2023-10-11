var DataTable = $('#tblUserList').DataTable();

$(document).ready(function () {
    $("#menu_UserMaster").addClass("active");
    BindUserList();
});
var currentPageNumber = 0;
var currentPageSize = 10;
function BindUserList() {
    try {
        if (DataTable != 'undefined') {
            currentPageSize = DataTable.page.len();
            DataTable.destroy();
        }
        currentPageNumber = DataTable.page.info().page

        DataTable = $('#tblUserList').DataTable({
            
            colReorder: {
                enable: false
            },
            serverSide: true,
            //processing: true,
             language: {
                 'processing': '<div class="loader"></div>'
            },
            ajax: {
                url: '/UserMaster/GetUserList',
                type: 'POST',
                data: function (d) {
                    d.start = DataTable.page.info().start;
                }
                //data: function (d) {
                //    d.ImportQuoteId = 10;
                //}
            },
            "lengthMenu": [10,20, 50, 100, 200, 500],
            "pageLength": currentPageSize,
            displayStart: currentPageNumber * currentPageSize,
            "order": [[5, "desc"]],
            //dom: 'Blfrtip',
            "bFilter": true,
            //"aaData": data.Data,
            "aoColumns": [

                {
                    "mData": "email",
                    "className": "text-left",
                     "width" : "300px"
                },
                {
                    "mData": "username",
                    "className": "text-left",
                },
                {
                    "mData": "RoleName",
                    "className": "text-left",
                   
                },
                {
                    "mData": "CompanyName",
                    "className": "text-left",
                },
                {
                    "mData": "SalesRepCode",
                    "className": "text-left",
                    "width": "50px"
                },
                {
                    "mData": "IsActive",
                    "className": "text-left text-center",
                    "render": function (data, type, row, meta) {
                        var result = '';
                        if (row.IsActive) {
                            result = '<button title="Active" onclick="openActivePopup(' + row.id + ', ' + row.IsActive +')" class="btn  text-success" ><i class="fa-solid fa-check fa-xl "></i></button>';
                        }
                        else {
                            result = '<a href = "javascript:void(0)" title = "In Active" onclick = "openActivePopup(' + row.id + ', ' + row.IsActive +')" class="btn  text-danger" > <i class="fa-solid fa-xmark fa-xl"></i></a >';
                        }
                        return result;
                    }
                },
                {
                    "mData": "id",
                    "bSortable": false,
                    "className": "text-center nowrap-actions",
                    "render": function (data, type, row, meta) {
                        var ActionButtonEdit = '<a href="javascript:void(0)" title="Edit" onclick="openUserModal(' + row.id + ')"  data-toggle="modal" data-target="#usermodalPopup" class="btn pr-2 edit-icon" ><i class="fa-regular fa-pen-to-square"></i></a>';
                        var ActionButtonDelete = '<a href="javascript:void(0)" onclick="DeleteUser(' + row.id + ')" title="Delete" class="btn delete-icon"><i class="fa-regular fa-trash-can"></i></a>';
                        var result = '';
                        result = result + ActionButtonEdit;
                        result = result + '&nbsp;&nbsp;&nbsp;' + ActionButtonDelete;
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
function openUserModal(userId) {
    $.ajax({
        url: '/UserMaster/ManageUsers',
        async: false,
        type: 'GET',
        data: { userId: userId },
        success: function (data) {
            $('#divUsermodalPopup').html(data);
        }
    });
}
function DeleteUser(Id) {
    bootbox.confirm({
        message: "Are you sure you want to delete this user?",
        buttons: {
            cancel: {
                label: "No",
                className: "btn-cancel min-auto cancel"
            },
            confirm: {
                label: "Yes, Delete User",
                className: "btn-warning confirm"
            }
        },
        callback: function (result) {
            if (result) {
                $.ajax({
                    type: 'POST',
                    url: '/UserMaster/DeleteUser',
                    dataType: 'json',
                    data: { userId: Id },
                    cache: false,
                    async: false,
                    success: function (data) {
                        if (data) {
                            toastr.success(data.Message);
                            //$('#BaggageQuoteGrid').data('kendoGrid').dataSource.read();
                            BindUserList();
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

function openActivePopup(Id, IsActive) {
    
    var text = "";
    if (IsActive) {
        text = "Are you sure you want to deactivate this user ?";
    }
    else {
        text = "Are you sure you want to activate this user ?";
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
                    url: '/UserMaster/ChangeStatus',
                    dataType: 'json',
                    data: { Id: Id, IsActive: IsActive},
                    cache: false,
                    async: false,
                    success: function (data) {
                        if (data) {
                            toastr.success(data.Message);
                            //$('#BaggageQuoteGrid').data('kendoGrid').dataSource.read();
                            BindUserList();
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

